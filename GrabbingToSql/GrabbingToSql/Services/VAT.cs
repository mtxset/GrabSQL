using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using GrabbingToSql.eu.europa.ec;
using System.Data;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using DataTable = System.Data.DataTable;

namespace GrabbingToSql.Services
{
    public class VATRequest
    {
        public string MemberState { get; set; }
        public string VATNumber { get; set; }
    }

    public class VATResponse
    {
        public string MemberState { get; set; }
        public string VATNumber { get; set; }
        public bool Valid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class VAT
    {
        readonly string _vatConfigFileName = "VAT.xml";

        public List<VATResponse> CheckVATList(ref List<VATRequest> vatRequests)
        {
            var vatRespones = new List<VATResponse>();

            foreach (var item in vatRequests)
            {
                VATResponse tResponse = CheckVAT(item.VATNumber, item.MemberState);
                if (tResponse != null)
                    vatRespones.Add(tResponse);
            }

            return vatRespones;
        }

        public VATResponse CheckVAT(string vatNumber = "227157511", string countryCode = "GB")
        {
            if (String.IsNullOrEmpty(vatNumber))
                return null;

            var vatR = new VATResponse();

            bool valid;
            string name, address;

            var service = new checkVatService();
            DateTime dt = service.checkVat(ref countryCode, ref vatNumber, out valid, out name, out address);

            vatR.Name = name;
            vatR.Valid = valid;
            vatR.Address = address;
            vatR.RequestDate = dt;
            vatR.VATNumber = vatNumber;
            vatR.MemberState = countryCode;

            return vatR;
        }

        public DataTable FormVATDataTable() 
        {
            DataTable dt = new DataTable("VAT");

            dt.Columns.Add("Member State");
            dt.Columns.Add("VAT Number");
            dt.Columns.Add("Valid");
            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("RequestDate", typeof (DateTime));

            return dt;
        }

        public void FillTable(ref List<VATResponse> vatResponses, ref DataTable dt)
        {
            foreach (var item in vatResponses)
            {
                object[] row = new object[dt.Columns.Count];

                row[0] = item.MemberState;
                row[1] = item.VATNumber;
                row[2] = item.Valid ? "VALID" : "INVALID";
                row[3] = item.Name;
                row[4] = item.Address;
                row[5] = item.RequestDate;

                dt.Rows.Add(row);
            }
        }

        public void SaveVATRequests(ref DataGridView gridView)
        {
            if (gridView == null) throw new ArgumentNullException(nameof(gridView));

            XmlSerializer ser = new XmlSerializer(typeof(List<VATRequest>));
            List<VATRequest> requests = new List<VATRequest>();
            int memberStateIndex = 0;
            int vatNumberIndex = 1;

            var dataGridViewColumn = gridView.Columns["Member State"];
            if (dataGridViewColumn != null)
            {
                memberStateIndex = dataGridViewColumn.Index;
            }

            dataGridViewColumn = gridView.Columns["VAT Number"];
            if (dataGridViewColumn != null)
            {
                vatNumberIndex = dataGridViewColumn.Index;
            }

            foreach (DataGridViewRow row in gridView.Rows)
            {
                if (row.IsNewRow) continue;
                VATRequest tempRequest = new VATRequest();

                tempRequest.MemberState = row.Cells[memberStateIndex].Value.ToString();
                tempRequest.VATNumber = row.Cells[vatNumberIndex].Value.ToString();

                requests.Add(tempRequest);
            }

            using (var stream = File.Create(_vatConfigFileName))
            {
                ser.Serialize(stream, requests);
            }

        } 

        public List<VATRequest> ReadVATRequests()
        {
            List<VATRequest> vatRequests = new List<VATRequest>();

            XmlSerializer ser = new XmlSerializer(typeof(List<VATRequest>));

            if (!File.Exists(_vatConfigFileName))
                return vatRequests;

            StreamReader reader = new StreamReader(_vatConfigFileName);

            vatRequests = (List<VATRequest>)ser.Deserialize(reader);
            reader.Close();

            return vatRequests;
        }
    }
}