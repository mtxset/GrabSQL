using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GrabbingToSql.Forms;
using GrabbingToSql.Services;

namespace GrabbingToSql
{
    public partial class Form1 : Form
    {
        private List<string> lastTextData;
        private Parser parser;
        private WorkingWSql mysqlConn;
        private DataTable allOverviewsTable;
        private DataTable vatTable;
        private Utils utils;
        private VAT vat;

        private bool mySqlEnabled = false;
        private string SQLIP = "127.0.0.1";
        private string SQLUser = "mtxset";
        private string SQLPass = "lag007";
        private string SQLDBName = "companieshouse";
        
        public enum InputDataType
        {
            CompanyNames = 0,
            CompanyNumber = 1
        }

        private void UpdateALLOverviewsTab(DataTable dt)
        {
            DataTable tempT = parser.SetupTable(Parser.PageTab.Overview);

            DataRow dr = allOverviewsTable.NewRow();
            for (int column = 0; column < tempT.Columns.Count; column++)
            { 
                if ( tempT.Columns[column].DataType == typeof(DateTime) )
                {
                    DateTime temp;
                    dr[column] = ( DateTime.TryParse(dt.Rows[0][column].ToString(), out temp) ) ? temp : DateTime.MinValue;
                }
                else
                {
                    dr[column] = dt.Rows[0][column].ToString();
                }
            }

            allOverviewsTable.Rows.Add(dr);
            dataGridAll.DataSource = allOverviewsTable;
            utils.MarkDeadlines(ref dataGridAll);
        }

        public async void Get(List<string> ls, InputDataType type)
        {
            foreach(string _compValue in ls)
            {
                if (_compValue.Length <= 0) continue;

                switch (type)
                {
                    case InputDataType.CompanyNumber:
                        {

                            DataTable dt = new DataTable();

                            JSON.Overview.RootObject temp = await parser.RetrieveOverviewJSON(_compValue);

                            parser.FormOverviewDataTable(temp, out dt);

                            object[] _data = dt.Rows[0].ItemArray;
                            allOverviewsTable.Rows.Add(_data);
                            dataGridAll.DataSource = allOverviewsTable;
                        }
                        break;
                }
            }
        }

        ///<summary>
        ///Parse user input
        ///</summary>
        ///<param name="ls">List of company names or numbers</param>
        public async void GetTextData(List<string> ls, InputDataType type)
        {
            lastTextData = ls;
            foreach (string compValue in ls)
            {
                if (compValue.Length <= 0) continue;

                string tempCompNumber = compValue;

                if (type == InputDataType.CompanyNames)
                    tempCompNumber = await Task.Run(() => parser.TryObtainingCompanyNumber(compValue));

                if (tempCompNumber == "") continue;

                DataSet ds = new DataSet();

                if (mySqlEnabled)
                {
                    if (!mysqlConn.ReadTables(tempCompNumber, out ds))
                    {
                        ds = await Task.Run(() => parser.ParseAllHTML(tempCompNumber, true, true, true));

                        if (!mysqlConn.UpdateTable(ref ds))
                        {
                            MessageBox.Show("Sorry, could not update table");
                        }
                    }
                }
                else
                {
                    ds = await Task.Run(() => parser.ParseAllHTML(tempCompNumber, true, true, true));
                }
                    
                if (ds == null) return;

                UpdateALLOverviewsTab( ds.Tables["Overview"] );
                string companyName = "";

                try
                {
                    companyName = ds.Tables["Overview"].Rows[0].ItemArray[1].ToString(); // TODO: 101
                }
                catch (Exception e)
                {
                    MessageBox.Show("Sorry, could not read company name!");
                }

                TabPage tp = new TabPage(companyName);

                FieldControl fc = new FieldControl();
                fc.Dock = DockStyle.Fill;
                fc.SetDataTables(ds);
                tp.Controls.Add(fc);

                tabControl1.TabPages.Add(tp);
            }
        }

        public async void ObtainNewVATData(List<VATRequest> vatRequests)
        {
            PopulateVATDataTable(vatRequests);
            //var res = await Task.Run(() => PopulateVATDataTable(vatRequests));

            UpdateVATGridView();
        }
        
        private void Init()
        {
            lastTextData = new List<string>();
            
            parser = new Parser(new ConfigLoader());
            
            utils = new Utils();
            vat = new VAT();

            allOverviewsTable = parser.SetupTable(Parser.PageTab.Overview);
            vatTable = vat.FormVATDataTable();

            ReadSQLConfig();
            ReadVATAndPopulate();

            if (mySqlEnabled)
            {
                mysqlConn = new WorkingWSql(SQLUser, SQLPass, SQLIP, SQLDBName);
            }
        }

        private void ReadSQLConfig()
        {
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "/fields.ini";
            IniFile file = new IniFile(path);

            try
            {
                if (file.IniReadValue("Settings", "MySQL") == "true")
                {
                    string section = "MYSQL";
                    SQLIP = file.IniReadValue(section, "IP");
                    SQLUser = file.IniReadValue(section, "User");
                    SQLPass = file.IniReadValue(section, "Pass");
                    SQLDBName = file.IniReadValue(section, "DBName");
                    mySqlEnabled = true;
                }
                else
                { 
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Could not read MySQL data from config");
            }

        }

        private bool PopulateVATDataTable(List<VATRequest> vatRequests)
        {
            List<VATResponse> vatResponses = vat.CheckVATList(ref vatRequests);

            vat.FillTable(ref vatResponses, ref vatTable);

            return true;
        }

        private async void ReadVATAndPopulate()
        {
            List<VATRequest> vatRequests = vat.ReadVATRequests();
            
            await Task.Run(() => PopulateVATDataTable(vatRequests));

            UpdateVATGridView();
        }

        private void UpdateVATGridView()
        {
            dataGridVAT.DataSource = vatTable;
            dataGridVAT.AutoResizeColumns();
        }

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void asdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParseTextForm pf = new ParseTextForm(this);
            pf.ShowDialog();
        }

        private void asdToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ParseIntForm pf = new ParseIntForm(this);
            pf.ShowDialog();
        }

        private void exportDeadlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            utils.ExportDeadlinesXLS(ref dataGridAll);
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_3(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void saveVATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vat.SaveVATRequests(ref dataGridVAT);
        }

        private void addVATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VATInputForm vatForm = new VATInputForm(this);
            vatForm.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            vat.SaveVATRequests(ref dataGridVAT);
        }
    }
}
