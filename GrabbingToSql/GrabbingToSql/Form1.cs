using System;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading.Tasks;

namespace GrabbingToSql
{
    public partial class Form1 : Form
    {
        private List<string> lastTextData;
        private Parser parser;
  
        private List<TabPage> tabPages;

        public enum InputDataType
        {
            CompanyNames = 0,
            CompanyNumber = 1
        }

        public async void GetTextData(List<string> ls, InputDataType type)
        {
            lastTextData = ls;
            foreach (string compValue in ls)
            {
                string tempCompNumber = compValue;

                if (type == InputDataType.CompanyNames)
                    tempCompNumber = await Task.Run(() => parser.TryObtainingCompanyNumber(compValue));

                DataSet ds = await Task.Run(() => parser.ParseAllHTML(tempCompNumber));

                if (ds == null) return;
                string companyName = ds.Tables["Overview"].Rows[0].ItemArray[1].ToString();

                TabPage tp = new TabPage(companyName);

                FieldControl fc = new FieldControl();
                fc.Dock = DockStyle.Fill;
                fc.SetDataTables(ds);
                tp.Controls.Add(fc);

                tabControl1.TabPages.Add(tp);
                tabPages.Add(tp);
            }
        }

        private void Init()
        {
            lastTextData = new List<string>();
            parser = new Parser();
            tabPages = new List<TabPage>();
        }
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Parser parser = new Parser();

            DataSet newSet = parser.ParseAllHTML();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void parseTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
 
        }



        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            //Show dialog

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_ButtonClick(object sender, EventArgs e)
        {

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
    }
}
