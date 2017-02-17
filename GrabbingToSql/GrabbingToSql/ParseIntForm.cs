using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public partial class ParseIntForm : Form
    {
        Form1 mainForm;

        public List<string> GetData()
        {
            List<string> ls = new List<string>();

            string[] arr = richTextBox1.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in arr)
            {
                ls.Add(s);
            }

            return ls;
        }

        public ParseIntForm(Form1 form)
        {
            InitializeComponent();

            mainForm = form;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //mainForm.Get(GetData(), Form1.InputDataType.CompanyNumber);
            mainForm.GetTextData(GetData(), Form1.InputDataType.CompanyNumber);
            Close();
        }
    }
}
