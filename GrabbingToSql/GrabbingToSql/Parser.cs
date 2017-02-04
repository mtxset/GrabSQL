using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;

namespace GrabbingToSql
{
    class Parser
    {
        private HtmlDocument htmlDoc;
        private HtmlWeb webClient;
        private String initialSite;

        private List<string> htmlFields;

        public void AddNewRow( Dictionary<string, string> dicDB, ref DataTable dataT )
        {
           
            dataT.Rows.Add(dicDB[htmlFields[0]],
                           dicDB[htmlFields[1]],
                           dicDB[htmlFields[2]],
                           dicDB[htmlFields[3]]);
        }

        public DataTable SetupTable()
        {

            DataTable tb1 = new DataTable("Example1");
            for (int i = 0; i < htmlFields.Count; i++)
            {
                tb1.Columns.Add(htmlFields[i]);
            }

            return tb1;
        }

        private void Init()
        {
            htmlDoc = new HtmlDocument();
            webClient = new HtmlWeb();
            initialSite = "https://beta.companieshouse.gov.uk/";
            htmlFields = new List<string>();
            fillHtmlFields();
        }

        private void fillHtmlFields()
        {
            //TODO load from list
            htmlFields.Add("Registered office address");
            htmlFields.Add("Company status");
            htmlFields.Add("Company type");
            htmlFields.Add("Incorporated on");
        }

        public Parser()
        {
            Init();
        }

        public Dictionary<string, string> ParseHTMLCompaniesHouse(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> dicDB = new Dictionary<string, string>();

            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes("//dl");

            foreach (HtmlNode item in tempNodes)
            {
                sb.Append(item.InnerText).AppendLine();
            }

            // removing unnecessary spaces
            string ts = Regex.Replace(sb.ToString(), @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            string[] strData = ts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < strData.Length; i++)
            {
                strData[i] = strData[i].Trim();
            }
            // removing unnecessary spaces

            for (int i = 0; i < strData.Length; i++)
            {
                for (int v = 0; v < htmlFields.Count; v++)
                {
                    if ( strData[i].Contains( htmlFields[v] ) )
                    {
                        dicDB.Add( htmlFields[v], strData[i+1] );
                    }
                }
            }

            // First part
            return dicDB;
        }
       
        public HtmlDocument GetHtmlByCompany( string company = "10581927")
        {
            String url = String.Format( "{0}company/{1}", initialSite, company );

            htmlDoc = webClient.Load( url );

            return htmlDoc;
        }
    }
}
