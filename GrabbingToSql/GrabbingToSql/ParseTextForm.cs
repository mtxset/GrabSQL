using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public partial class ParseTextForm : Form
    {
        Form1 mainForm;
        
        public List<string> GetData()
        {
            List<string> ls = new List<string>();

            string[] arr = textBox.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in arr)
            {
                ls.Add(s);
            }

            return ls;
        }

        public ParseTextForm(Form1 form)
        {
            InitializeComponent();

            mainForm = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.GetTextData(GetData(), Form1.InputDataType.CompanyNames);
            Close();
        }
    }
}
