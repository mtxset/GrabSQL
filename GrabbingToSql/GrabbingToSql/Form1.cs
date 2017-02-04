using System;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Collections.Generic;

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

            //HtmlAgilityPack.HtmlDocument tempDoc = parser.GetHtmlByCompany();
            parser.ParseHTMLCompaniesHouse(parser.GetHtmlByCompany()).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadConfig lc = new LoadConfig();
            Dictionary<string, string> tempDic = lc.LoadFields();
        }
    }
}
