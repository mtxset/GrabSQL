using System;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Diagnostics;

namespace GrabbingToSql
{
    public static class Global
    {
        public static Dictionary<string, Dictionary<string, string>> fieldDictionaries = new Dictionary<string, Dictionary<string, string>>()
        {

        };
    }

    class Parser
    {
        private HtmlDocument htmlDoc;
        private HtmlWeb webClient;
        private String initialSite;

        // TODO: load list and fill ALL lists from config, pageTAB, initialSite, formats for pageTabs
        private Dictionary<string, List<string>> htmlFields;

        public class ConfigLoader
        {
            public Dictionary<string, string> LoadFields(PageTab tab = PageTab.Overview)
            {
                string prefixName;

                switch (tab)
                {
                    case PageTab.Overview:
                        prefixName = "Overview";
                        break;
                    case PageTab.FilingHistory:
                        prefixName = "FilingHistory";
                        break;
                    case PageTab.People:
                        prefixName = "People";
                        break;
                    default:
                        goto case PageTab.Overview;
                }

                Dictionary<string, string> tempDic = new Dictionary<string, string>();

                string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "/fields.ini";
                IniFile file = new IniFile(path);

                string val = file.IniReadValue("Settings", prefixName + "FieldCount");
                int fieldCount = int.Parse(val);

                if (fieldCount <= 0)
                    return tempDic;

                for (int i = 0; i < fieldCount; i++)
                {
                    tempDic.Add(file.IniReadValue(prefixName + "FieldNames", i.ToString()), file.IniReadValue(prefixName + "SQLFieldNames", i.ToString()));
                }

                return tempDic;
            }
        }

        public enum PageTab
        {
            Overview = 0,
            FilingHistory = 1,
            People = 2
        }

        private string ReturnPageTab(PageTab tab)
        {
            switch(tab)
            {
                case PageTab.Overview:
                    return "Overview";
                case PageTab.FilingHistory:
                    return "FilingHistory";
                case PageTab.People:
                    return "People";
                default:
                    goto case PageTab.Overview;
            }
        }

        public void AddNewRow(Dictionary<string, string> dicDB, ref DataTable dataT)
        {
            dataT.Rows.Add( dicDB.Values.ToArray() );
        }

        private Dictionary<string, string> EmptyDictionary(PageTab tab)
        {
            Dictionary<string, string> tDic = new Dictionary<string, string>();
            string tabName = ReturnPageTab(tab);

            for (int i = 0; i < htmlFields[ tabName ].Count; i++)
            {
                tDic.Add( htmlFields[ tabName][i], "" );
            }
            return tDic;
        }

        private string ReplaceTrim( string sb )
        {
            string ts = Regex.Replace(sb.ToString(), @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            ts = ts.Trim();

            return ts;
        }

        private string[] ReplaceSplitTrim( StringBuilder sb)
        {
            string ts = Regex.Replace(sb.ToString(), @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            string[] strData = ts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i<strData.Length; i++)
            {
                strData[i] = strData[i].Trim();
            }

            return strData;
        }

        private StringBuilder NodeCollectionToSBInnerHtml(HtmlNodeCollection tempNodes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode item in tempNodes)
            {
                sb.Append(item.InnerHtml).AppendLine();
            }

            return sb;
        }

        private StringBuilder NodeCollectionToSB( HtmlNodeCollection tempNodes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode item in tempNodes)
            {
                sb.Append(item.InnerText).AppendLine();
            }

            return sb;
        }

        public DataTable SetupTable( PageTab tab = PageTab.Overview )
        {
            string tabName = ReturnPageTab(tab);

            DataTable tb1 = new DataTable(tabName);

            for (int i = 0; i < htmlFields[tabName].Count; i++)
            {
                tb1.Columns.Add(htmlFields[tabName][i]);
            }

            return tb1;
        }

        private void Init()
        {
            htmlDoc = new HtmlDocument();
            webClient = new HtmlWeb();
            initialSite = "https://beta.companieshouse.gov.uk/";

            htmlFields = new Dictionary<string, List<string>>();

            //TODO: add to array (implement log stuff)
            htmlFields.Add("Overview", new List<string>());
            htmlFields.Add("People", new List<string>());
            htmlFields.Add("FilingHistory", new List<string>());

            fillHtmlFields();
        }

        private void fillHtmlFields()
        {
            ConfigLoader cLoader = new ConfigLoader();
            Dictionary<string, string> tDicO = cLoader.LoadFields(PageTab.Overview);
            Dictionary<string, string> tDicP = cLoader.LoadFields(PageTab.People);
            Dictionary<string, string> tDicF = cLoader.LoadFields(PageTab.FilingHistory);

            for (int i = 0; i < tDicO.Count; i++)
            {
                htmlFields["Overview"].Add(tDicO.Keys.ElementAt(i));
            }

            for (int i = 0; i < tDicP.Count; i++)
            {
                htmlFields["People"].Add(tDicP.Keys.ElementAt(i));
            }

            for (int i = 0; i < tDicF.Count; i++)
            {
                htmlFields["FilingHistory"].Add(tDicF.Keys.ElementAt(i));
            }
        }

        public Parser()
        {
            Init();
        }

        public Dictionary<string, string> ParseHTML(out List<Dictionary<string, string>> customDic, string companyNumber = "10581927", PageTab tab = PageTab.Overview)
        {
            List<Dictionary<string, string>> tempDic = new List<Dictionary<string, string>>();
            customDic = tempDic;
            HtmlDocument data = GetHtmlByCompany(companyNumber, tab);

            switch (tab)
            {
                case PageTab.Overview:
                    return ParseHTMLOverviewTab(data);

                case PageTab.FilingHistory:
                    customDic = ParseHTMLFilingHistoryTab(data);
                    break;

                case PageTab.People:
                    customDic = ParseHTMLPeopleTab(data);
                    break;

                default:
                    return ParseHTMLOverviewTab(data);
            }

            return null;
        }

        private List<Dictionary<string, string>> ParseHTMLPeopleTab(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();
            List<Dictionary<string, string>> tempDic = new List<Dictionary<string, string>>();

            string tabName = ReturnPageTab(PageTab.People);
            int officers = 0;

            //< div class="appointments-list">
            string xcode = "//*[@id=\"company-appointments\"]";
            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes(xcode);

            if (tempNodes == null)
            {
                throw new Exception("TempNode was empty. Check xcode in ParseHTMLPeopleTab(); xcode: " + xcode);
            }

            sb = NodeCollectionToSB(tempNodes);

            string[] strData = ReplaceSplitTrim(sb);

            for (int i = 0; i < strData.Length; i++)
            {
                // TODO: so bad..
                if ( strData[i].Contains("officers") )
                {
                    officers = int.Parse(strData[i - 1]);
                }
            }

            string xName = "";
            HtmlNode tNode = null;
            Dictionary<string, string> tDic;
            //TODO: load ids format from log
            for (int i = 1; i <= officers; i++)
            {
                tDic = EmptyDictionary(PageTab.People);

                xName = String.Format("//span[@id=\"officer-name-{0}\"]", i.ToString() );
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null) 
                    tDic["Officer Name"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-address-value-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Correspondence address"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-role-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Role"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-date-of-birth-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Date of Birth"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-nationality-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Nationality"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-country-of-residence-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Country of residence"] = ReplaceTrim(tNode.InnerText);

                xName = String.Format("//*[@id=\"officer-occupation-{0}\"]", i.ToString());
                tNode = data.DocumentNode.SelectSingleNode(xName);
                if (tNode != null)
                    tDic["Occupation"] = ReplaceTrim(tNode.InnerText);

                tempDic.Add(tDic);
            }

            return tempDic;
        }

        
        private List<Dictionary<string, string>> ParseHTMLFilingHistoryTab(HtmlDocument data)
        {
            List<Dictionary<string, string>> tempDic = new List<Dictionary<string, string>>();
            StringBuilder sb = new StringBuilder();

            var node = data.DocumentNode.SelectNodes("//ul[@class='pager']");

            sb = NodeCollectionToSB(node);

            string[] strData = ReplaceSplitTrim(sb);

            int r;
            List<int> arr = new List<int>();

            for (int i = 0; i < strData.Length; i++)
            {
                if (int.TryParse(strData[i], out r))
                    arr.Add(r);
            }

            int maxPage = arr.Max();

            // check maxPage
            //string url = string.Format("{0}company/{1}/filing-history?page={2}", initialSite, company);

            //htmlDoc = webClient.Load(url);

            List<List<string>> table = data.DocumentNode.SelectSingleNode("//table[@id='fhTable']")
               .Descendants("tr")
               .Skip(1)
               .Where(tr => tr.Elements("td").Count() > 1)
               .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
               .ToList();

            Dictionary<string, string> tDic;
            for (int i = 0; i < table.Count; i++)
            {
                tDic = EmptyDictionary(PageTab.FilingHistory);

                //TODO: 101
                tDic["Date"] = table[i][0];
                tDic["Type"] = table[i][1];
                tDic["Description"] = table[i][2];
                tDic["View / Download"] = table[i][3];
                tempDic.Add(tDic);
            }

            return tempDic;
        }

        private Dictionary<string, string> ParseHTMLOverviewTab(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> dicDB = EmptyDictionary(PageTab.Overview);
            string tabName = ReturnPageTab(PageTab.Overview);

            //TODO implement try
            //TODO implement reading xcode form config file
            string xcode = "//*[@class=\"grid-row\"] | //dl | //*[@id=\"company-number\"] | //*[@id=\"company-name\"] | //*[@id=\"sic0\"]";
            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes(xcode);

            sb = NodeCollectionToSB( tempNodes );

            string[] strData = ReplaceSplitTrim(sb);

            for (int i = 0; i < strData.Length; i++)
            {
                for (int v = 0; v < htmlFields[ tabName ].Count; v++)
                {
                    if ( strData[i].Contains( htmlFields[ tabName][v] ) )
                    {
                        dicDB[htmlFields[ tabName ][v]] = strData[i + 1];
                    }
                }
            }

            return dicDB;
        }
       
        private HtmlDocument GetHtmlByCompany( string company = "10581927", PageTab tab = PageTab.Overview)
        {
            string url;
            //TODO: Load from config initial site and format
            //TODO: implement exceptions for web content
            switch (tab)
            {
                case PageTab.Overview:
                    
                   url = string.Format("{0}company/{1}", initialSite, company);

                   htmlDoc = webClient.Load(url);

                   return htmlDoc;

                case PageTab.People:

                    url = string.Format("{0}company/{1}/officers", initialSite, company);

                    htmlDoc = webClient.Load(url);

                    return htmlDoc;

                case PageTab.FilingHistory:

                    url = string.Format("{0}company/{1}/filing-history", initialSite, company);

                    htmlDoc = webClient.Load(url);

                    return htmlDoc;
                default:
                    return null;// TODO raise exception
            }
        }
    }
}
