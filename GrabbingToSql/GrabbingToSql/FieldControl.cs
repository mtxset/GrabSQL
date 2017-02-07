using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public partial class FieldControl : UserControl
    {
        public FieldControl()
        {
            InitializeComponent();
        }

        public void SetDataTables(DataSet ds)
        {
            dataGridOverview.DataSource = ds.Tables["Overview"];
            dataGridFillingHistory.DataSource = ds.Tables["FilingHistory"];
            dataGridPeople.DataSource = ds.Tables["People"];
        }

    }
}
