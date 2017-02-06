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
    }
}
