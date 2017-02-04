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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
