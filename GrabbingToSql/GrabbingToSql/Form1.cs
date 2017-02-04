using System;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Data;

namespace GrabbingToSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser();
            

            DataTable table = parser.SetupTable();

            var companies = richTextBox1.Lines;

            foreach (string s in companies)
            {
                if (s.Length != 0)
                parser.AddNewRow(parser.ParseHTMLCompaniesHouse(parser.GetHtmlByCompany(s)), ref table);
            }

            dataGridView1.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadConfig lc = new LoadConfig();
            Dictionary<string, string> tempDic = lc.LoadFields();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
