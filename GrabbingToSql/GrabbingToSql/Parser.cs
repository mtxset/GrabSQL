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

        private Dictionary<string, string> dicDB;
        private List<string> htmlFields;

        public DataSet GetDataset()
        {
            DataSet temp = new DataSet();

            

            return temp;
        }

        private void Init()
        {
            htmlDoc = new HtmlDocument();
            webClient = new HtmlWeb();
            initialSite = "https://beta.companieshouse.gov.uk/";
            dicDB = new Dictionary<string, string>();
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

        public StringBuilder ParseHTMLCompaniesHouse(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();

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
            return sb;
        }
       
        public HtmlDocument GetHtmlByCompany( string company = "10581927")
        {
            String url = String.Format( "{0}company/{1}", initialSite, company );

            htmlDoc = webClient.Load( url );

            return htmlDoc;
        }
    }
}
