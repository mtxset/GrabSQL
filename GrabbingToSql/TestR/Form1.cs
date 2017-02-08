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

            Parser parser = new Parser();


            List<string> companies = new List<string>();
            companies.Add("10571534");
            companies.Add("10537975");

            DataSet DataSetToUpdate, DS1;
            foreach (string s in companies)
            {
                DataSetToUpdate = parser.ParseAllHTML(s);
                SqlObj.UpdateTable(ref DataSetToUpdate);
                SqlObj.ReadTables(10571534,out DS1);
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
