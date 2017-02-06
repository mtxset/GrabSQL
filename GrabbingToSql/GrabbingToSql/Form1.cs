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
            
            DataTable table = parser.SetupTable(Parser.PageTab.People);

            var companies = richTextBox1.Lines;

            //TODO: parse the text in list box
            //TODO: implement async row adding
            foreach (string s in companies)
            {
                if (s.Length != 0)
                parser.AddNewRow(parser.ParseHTML(s,Parser.PageTab.People), ref table);
            }

            dataGridView1.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
