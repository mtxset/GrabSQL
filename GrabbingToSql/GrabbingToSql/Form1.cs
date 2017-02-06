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
            List<Dictionary<string, string>> peopleDic = new List<Dictionary<string, string>>();
            Parser parser = new Parser();
            var tab = Parser.PageTab.FilingHistory;

            DataTable table = parser.SetupTable(tab);

            var companies = richTextBox1.Lines;

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
            /*
            foreach (string s in companies)
            {
                if (s.Length != 0)
                    parser.AddNewRow(parser.ParseHTML(out peopleDic, s, tab), ref table);
            }
             */

            dataGridView1.DataSource = table;
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
    }
}
