using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;
using ClosedXML.Excel;

namespace GrabbingToSql
{
    class Utils
    {
        private Parser _parser;

        void Init()
        {
            ConfigLoader cf = new ConfigLoader();
            _parser = new Parser(cf);
        }

        public Utils()
        {
            Init();
        }

        List<string> deadlineColumns = new List<string>()
        {
            "Accounts",
            "Confirmation statement"
        };

        private void GetDeadLineColumnIndexes(out List<int> deadlineColumnIndexes, ref DataGridView grid)
        {
            List<int> dci = new List<int>();

            foreach (DataGridViewColumn column in grid.Columns)
            {
                foreach (string dateColumnName in deadlineColumns)
                {
                    if (column.Name.Contains(dateColumnName))
                    {
                        dci.Add(column.Index);
                    }
                }
            }
            deadlineColumnIndexes = dci;
        }

        public void ExportDeadlinesXLS(ref DataGridView grid)
        {
            SaveFileDialog file = new SaveFileDialog {DefaultExt = ".xlsx"};
            file.ShowDialog();

            if (string.IsNullOrEmpty ( file.FileName ))
            {
                MessageBox.Show("Sorry, fail was not saved.");
                return;
            }

            List<int> deadlineColumnIndexes;
            GetDeadLineColumnIndexes(out deadlineColumnIndexes, ref grid);

            DataTable dt = _parser.SetupTable(Parser.PageTab.Overview);

            foreach (DataGridViewRow row in grid.Rows)
            {
                foreach (int columnIndex in deadlineColumnIndexes)
                {
                    if (row.Cells[columnIndex].Style.ForeColor != Color.Red &&
                        row.Cells[columnIndex].Style.ForeColor != Color.Yellow) continue;

                    DataRow dr = ( (DataRowView)row.DataBoundItem ).Row;

                    dt.ImportRow(dr);
                    break;
                }
            }

            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(dt);
            wb.SaveAs(file.FileName);
            
            MessageBox.Show($"Saved {dt.Rows.Count} rows");
        }

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            return dTable;
        }

        public void MarkDeadlines(ref DataGridView grid)
        {
            List<int> deadlineColumnIndexes;
            GetDeadLineColumnIndexes(out deadlineColumnIndexes, ref grid);

            foreach (int columnIndex in deadlineColumnIndexes)
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    DateTime dt;
                    if (DateTime.TryParse(row.Cells[columnIndex].Value.ToString(), out dt))
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle();

                        if (dt < DateTime.Now.Date)
                        {
                            style.ForeColor = Color.Red;
                            row.Cells[columnIndex].Style = style;
                        }
                        else if (dt.AddDays(30) < DateTime.Now.Date)
                        {
                            style.ForeColor = Color.Yellow;
                            row.Cells[columnIndex].Style = style;
                        }
                    } 
                }
            }
        }
    }
}
