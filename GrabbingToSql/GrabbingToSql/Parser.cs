using System;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        private List<string> htmlOverviewFields;
        private List<string> htmlPoepleFields;
        private List<string> htmlFillingHistoryFields;

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
                    case PageTab.FillingHistory:
                        prefixName = "Filling";
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
            FillingHistory = 1,
            People = 2
        }

        public void AddNewRow(Dictionary<string, string> dicDB, ref DataTable dataT)
        {
            dataT.Rows.Add( dicDB.Values.ToArray() );
        }

        private Dictionary<string, string> EmptyDictionary(PageTab tab)
        {
            Dictionary<string, string> tDic = new Dictionary<string, string>();

            switch (tab)
            {
                case
            }

            

            for (int i = 0; i < htmlOverviewFields.Count; i++)
            {
                tDic.Add(htmlOverviewFields[i], "");
            }
            return tDic;
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
            switch (tab)
            {
                case PageTab.Overview:
                    return SetupOverviewTable();
                case PageTab.FillingHistory:
                    return SetupFillingHistoryTable();
                case PageTab.People:
                    return SetupPeopleTable();
                default:
                    return SetupOverviewTable();
            }
        }

        private DataTable SetupPeopleTable()
        {
            DataTable tb1 = new DataTable("People");
            for (int i = 0; i < htmlPoepleFields.Count; i++)
            {
                tb1.Columns.Add(htmlPoepleFields[i]);
            }

            return tb1;
        }

        private DataTable SetupFillingHistoryTable()
        {
            throw new NotImplementedException();
        }

        private DataTable SetupOverviewTable()
        {

            DataTable tb1 = new DataTable("Overview");
            for (int i = 0; i < htmlOverviewFields.Count; i++)
            {
                tb1.Columns.Add(htmlOverviewFields[i]);
            }

            return tb1;
        }

        private void Init()
        {
            htmlDoc = new HtmlDocument();
            webClient = new HtmlWeb();
            initialSite = "https://beta.companieshouse.gov.uk/";

            //TODO: add to array (implement log stuff)
            htmlOverviewFields = new List<string>();
            htmlPoepleFields = new List<string>();
            htmlFillingHistoryFields = new List<string>();

            fillHtmlFields();
        }

        private void fillHtmlFields()
        {
            ConfigLoader cLoader = new ConfigLoader();
            Dictionary<string, string> tDic = cLoader.LoadFields( PageTab.Overview );

            for (int i = 0; i < tDic.Count; i++)
            {
                htmlOverviewFields.Add(tDic.Keys.ElementAt(i));
            }

            tDic.Clear(); tDic = cLoader.LoadFields( PageTab.People );

            for (int i = 0; i < tDic.Count; i++)
            {
                htmlPoepleFields.Add(tDic.Keys.ElementAt(i));
            }

        }

        public Parser()
        {
            Init();
        }

        public Dictionary<string, string> ParseHTML(string companyNumber = "10581927", PageTab tab = PageTab.Overview)
        {
            HtmlDocument data = GetHtmlByCompany(companyNumber, tab);
            switch (tab)
            {
                case PageTab.Overview:
                    return ParseHTMLOverviewTab(data);

                case PageTab.FillingHistory:
                    return ParseHTMLFillingHistoryTab(data);

                case PageTab.People:
                    return ParseHTMLPeopleTab(data);

                default:
                    return ParseHTMLOverviewTab(data);
            }
        }

        private Dictionary<string, string> ParseHTMLPeopleTab(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> dicDB = EmptyDictionary();

            //< div class="appointments-list">
            string xcode = "//*[@class=\"appointments-list\"]";
            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes(xcode);

            sb = NodeCollectionToSB(tempNodes);

            string[] strData = ReplaceSplitTrim(sb);

            for (int i = 0; i < strData.Length; i++)
            {
                for (int v = 0; v < htmlOverviewFields.Count; v++)
                {
                    if (strData[i].Contains(htmlOverviewFields[v]))
                    {
                        dicDB[htmlPoepleFields[v]] = strData[i + 1];
                    }
                }
            }

            return dicDB;
        }

        private Dictionary<string, string> ParseHTMLFillingHistoryTab(HtmlDocument data)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> ParseHTMLOverviewTab(HtmlDocument data)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> dicDB = EmptyDictionary();

            //TODO implement try
            //TODO implement reading xcode form config file
            string xcode = "//*[@class=\"grid-row\"] | //dl | //*[@id=\"company-number\"] | //*[@id=\"company-name\"] | //*[@id=\"sic0\"]";
            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes(xcode);

            sb = NodeCollectionToSB( tempNodes );

            string[] strData = ReplaceSplitTrim(sb);

            for (int i = 0; i < strData.Length; i++)
            {
                for (int v = 0; v < htmlOverviewFields.Count; v++)
                {
                    if ( strData[i].Contains( htmlOverviewFields[v] ) )
                    {
                        dicDB[htmlOverviewFields[v]] = strData[i + 1];
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
                default:
                    return null;// TODO raise exception
            }

        }
    }
}
