using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GrabbingToSql;

namespace TestR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WorkingWSql SqlObj = new WorkingWSql("yuk","yurak123","192.168.0.103", "companieshouse");

            List<Dictionary<string, string>> peopleDic = new List<Dictionary<string, string>>();
            Parser parser = new Parser();
            var tab = Parser.PageTab.FilingHistory;

            DataTable table = parser.SetupTable(tab);

            List<string> companies = new List<string>();
            companies.Add("10571534");
            companies.Add("10537975");


            //TODO: parse the text in list box
            //TODO: implement async row adding
            foreach (string s in companies)
            {
                if (s.Length != 0)
                {
                    parser.ParseHTML(out peopleDic, s, tab);
                }

                foreach (Dictionary<string, string> item in peopleDic)
                {
                    if (item != null)
                        parser.AddNewRow(item, ref table);
                }
            }

            DataSet DataSetToUpdate = new DataSet();
            DataSetToUpdate.Tables.Add(table);
            SqlObj.UpdateTable(ref DataSetToUpdate);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
