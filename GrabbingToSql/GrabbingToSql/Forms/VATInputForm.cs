using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GrabbingToSql.Services;

namespace GrabbingToSql.Forms
{
    public partial class VATInputForm : Form
    {
        Form1 mainForm;

        public VATInputForm(Form1 form)
        {
            InitializeComponent();

            mainForm = form;
        }

        public List<VATRequest> GetData()
        {
            List<VATRequest> ls = new List<VATRequest>();

            string[] arr = richTextBox1.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in arr)
            {
                var req = new VATRequest {MemberState = "GB", VATNumber = s};

                ls.Add(req);
            }

            return ls;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.ObtainNewVATData(GetData());
            Close();
        }
    }
}
