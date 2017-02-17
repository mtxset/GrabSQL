using System;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrabbingToSql
{
    public class Parser
    {
        private HtmlDocument _htmlDoc;
        private HtmlWeb _webClient;
        private String _initialSite;

        // TODO: load list and fill ALL lists from config, pageTAB, _initialSite, formats for pageTabs
        private Dictionary<string, List<string>> htmlFields;
        private readonly IConfigLoader _configLoader;

        public enum PageTab
        {
            Overview = 0,
            FilingHistory = 1,
            People = 2,
            LoadSiteData = 3
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

        public void AddNewRow(Dictionary<string, string> dicDb, ref DataTable dataT)
        {
            string[] _buffer = dicDb.Values.ToArray();
            dataT.Rows.Add( _buffer );
        }

        private Dictionary<string, string> EmptyDictionary(PageTab tab)
        {
            Dictionary<string, string> tDic = new Dictionary<string, string>();
            string tabName = ReturnPageTab(tab);

            for (int i = 0; i < htmlFields[ tabName ].Count; i++)
            {
                tDic.Add(htmlFields[tabName][i], null );
            }
            return tDic;
        }

        public string ReplaceTrim( string sb )
        {
            if (sb == null) return null;

            string ts = Regex.Replace(sb, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            ts = ts.Trim();

            return ts;
        }

        private string[] ReplaceSplitTrim( StringBuilder sb)
        {
            string ts = Regex.Replace(sb.ToString(), @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            string[] strData = ts.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

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

        /// <summary>
        /// Fills empty table with culumns and data types, which are read from config file.
        /// </summary>
        /// <param name="dataTable">Empty datatable</param>
        /// <param name="tab">Table name (Overview, Filing History, People)</param>
        /// <returns>True if no error</returns>
        public static bool FormEmptyTable(ref DataTable dataTable, PageTab tab)
        {
            dataTable = new DataTable();

            return false;
        }

        public DataTable SetupTable( PageTab tab = PageTab.Overview )
        {
            string tabName = ReturnPageTab(tab);

            DataTable tb1 = new DataTable(tabName);

            for (int i = 0; i < htmlFields[tabName].Count; i++)
            {
                if (tab == PageTab.Overview)
                {
                    if (i == 5 || i == 6 || i == 8 || i == 4) // fields which are dates
                    { 
                        tb1.Columns.Add(htmlFields[tabName][i], typeof(DateTime));
                        continue;
                    }
                }
               
                /*
                if (tab == PageTab.FilingHistory)
                {
                    if (i == 0)
                    {
                        tb1.Columns.Add(htmlFields[tabName][i], typeof(DateTime));
                        continue;
                    }
                }*/

                tb1.Columns.Add(htmlFields[tabName][i]);
            }

            return tb1;
        }

        private void Init()
        {
            _htmlDoc = new HtmlDocument();
            _webClient = new HtmlWeb();
            _initialSite = "https://beta.companieshouse.gov.uk/";

            htmlFields = new Dictionary<string, List<string>>();

            //TODO: add to array (implement log stuff)
            htmlFields.Add("Overview", new List<string>());
            htmlFields.Add("People", new List<string>());
            htmlFields.Add("FilingHistory", new List<string>());

            FillHtmlFields();
            LoadSiteData();
        }

        private void LoadSiteData()
        {
            
            Dictionary<string, string> tDic = _configLoader.LoadFields(PageTab.LoadSiteData);

            if (tDic["Site"].Length > 0)
                _initialSite = tDic["Site"];
        }

        private void FillHtmlFields()
        {
            Dictionary<string, string> tDicO = _configLoader.LoadFields(PageTab.Overview);
            Dictionary<string, string> tDicP = _configLoader.LoadFields(PageTab.People);
            Dictionary<string, string> tDicF = _configLoader.LoadFields(PageTab.FilingHistory);

            for (int i = 0; i < tDicO.Count; i++)
            {
                htmlFields[ReturnPageTab(PageTab.Overview)].Add(tDicO.Keys.ElementAt(i));
            }

            for (int i = 0; i < tDicP.Count; i++)
            {
                htmlFields[ReturnPageTab(PageTab.People)].Add(tDicP.Keys.ElementAt(i));
            }

            for (int i = 0; i < tDicF.Count; i++)
            {
                htmlFields[ReturnPageTab(PageTab.FilingHistory)].Add(tDicF.Keys.ElementAt(i));
            }
        }

        public Parser(IConfigLoader configLoader)
        {
            _configLoader = configLoader;
            Init();
        }

        public DataSet ParseAllHTML(string companyNumber = "00889821", bool parseOverview = true, bool ParseFilingHistory = true, bool ParsePeople = true)
        {
            var ds = new DataSet();
            PageTab tab;
            var tempDic = new List<Dictionary<string, string>>();
            var parser = new Parser(new ConfigLoader());

            //Overview
            if (parseOverview)
            { 
                tab = Parser.PageTab.Overview;
                DataTable overviewTable = parser.SetupTable(tab);
                parser.AddNewRow(parser.ParseHTML(out tempDic, companyNumber, tab), ref overviewTable);
                ds.Tables.Add(overviewTable);
            }

            // Filing History
            tab = Parser.PageTab.FilingHistory;
            DataTable FilingHistoryTable = parser.SetupTable(tab);
            if (ParseFilingHistory)
            {

                parser.ParseHTML(out tempDic, companyNumber, tab);
                foreach (Dictionary<string, string> item in tempDic)
                {
                    if (item != null)
                        parser.AddNewRow(item, ref FilingHistoryTable);
                }
                ds.Tables.Add(FilingHistoryTable);
                tempDic.Clear();
            }

            tab = Parser.PageTab.People;
            DataTable PeopleTable = parser.SetupTable(tab);
            //People
            if (ParsePeople)
            {

                parser.ParseHTML(out tempDic, companyNumber, tab);
                foreach (Dictionary<string, string> item in tempDic)
                {
                    if (item != null)
                        parser.AddNewRow(item, ref PeopleTable);
                }
                ds.Tables.Add(PeopleTable);
            }

            return ds;
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
                    customDic = ParseHTMLFilingHistoryTab(data, companyNumber);
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
                if ( strData[i].Contains("officer") )
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

                xName = $"//span[@id=\"officer-name-{i}\"]";
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

        private List<Dictionary<string, string>> ParseHTMLFilingHistoryTab(HtmlDocument data, string companyNumber)
        {
            List<Dictionary<string, string>> tempDic = new List<Dictionary<string, string>>();
            StringBuilder sb = new StringBuilder();
            string[] strData;
            int maxPage = 1;

            // Getting max page
            var node = data.DocumentNode.SelectNodes("//ul[@class='pager']");
            if (node != null)
            { 
                sb = NodeCollectionToSB(node);
                strData = ReplaceSplitTrim(sb);

                int r;
                List<int> arr = new List<int>();

                for (int i = 0; i < strData.Length; i++)
                {
                    if (int.TryParse(strData[i], out r))
                        arr.Add(r);
                }
                maxPage = arr.Max();
            }
            // End Getting max page 101

            for (int p = 1; p <= maxPage; p++)
            {
                string url = string.Format("{0}company/{1}/filing-history?page={2}", _initialSite, companyNumber, p);
                data = _webClient.Load(url);

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
            }

            return tempDic;
        }

        public string TryObtainingCompanyNumber(string query = "SUPERCOINS LTD")
        {
            string url = "", companyNumbers = "";
            StringBuilder sb = new StringBuilder();

            url = string.Format("{0}search?q={1}", _initialSite, query);

            HtmlDocument data = _webClient.Load(url);

            string xcode = "//li[@class='type-company']//a[@href]";
            HtmlNode tempNode = data.DocumentNode.SelectSingleNode(xcode);

            if (tempNode == null)
            {
                System.Windows.Forms.MessageBox.Show($"Could not find company: {query}");
                return "";
            }

            string temp = tempNode.Attributes["href"].Value;

            try
            { 
                companyNumbers = temp.Replace("/company/", "");
            }
            catch (Exception e)
            {
                throw new Exception("Sorry, could not find company number!");
            }

            return companyNumbers;
        }

        private async Task<string> GetResponse(string request)
        {
            var uri = new Uri("https://api.companieshouse.gov.uk");

            var cl = new HttpClient {BaseAddress = uri};

            cl.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic NVdNOEhuSXlOaGlVNndNU3hfbkZNZTZWaWQ4R3BaUndMNVlwc1ZUZDo=");
            var response = await cl.GetAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        public bool FormFilingHistoryDataTable(JSON.FilingHistory.RootObject filingHistory, out DataTable dt)
        {
            dt = SetupTable(PageTab.FilingHistory);

            if (filingHistory == null) return false;

            foreach (var item in filingHistory.items)
            {
                object[] data = new object[dt.Columns.Count];

                data[0] = DateTime.Parse(item.date);
                data[1] = item.type;
                data[2] = item.description;
                data[4] = item.links.self;

                dt.Rows.Add(data);
            }

            return true;
        }

        public bool FormOverviewDataTable(JSON.Overview.RootObject overview, out DataTable dt)
        {
            dt = SetupTable(PageTab.Overview);

            if (overview == null)
                return false;

            object[] data = new object[dt.Columns.Count];

            data[0] = overview.company_number;
            data[1] = overview.company_name;
            data[2] = overview.registered_office_address.address_line_1;
            data[3] = overview.company_status;
            data[4] = DateTime.Parse(overview.date_of_creation);
            data[5] = DateTime.Parse(overview.accounts.next_due);
            data[6] = DateTime.Parse(overview.confirmation_statement.next_due);
            data[7] = overview.sic_codes[0];
            data[8] = DateTime.Now;

            dt.Rows.Add(data);

            return true;
        }

        public async Task<JSON.FilingHistory.RootObject> RetriveFilingHistoryJSON(string companyNumber)
        {
            string responseBody = await GetResponse($"company/{companyNumber}/filing-history/");

            JSON.FilingHistory.RootObject o = JsonConvert.DeserializeObject<JSON.FilingHistory.RootObject>(responseBody);

            return o;
        }

        public async Task<JSON.Overview.RootObject> RetrieveOverviewJSON(string companyNumber)
        {
            string responseBody = await GetResponse($"company/{companyNumber}");

            JSON.Overview.RootObject overview = JsonConvert.DeserializeObject<JSON.Overview.RootObject>(responseBody);
            
            return overview;
        }

        public  Dictionary<string, string> ParseHTMLOverviewTab(HtmlDocument data)
        {
            var dicDB = EmptyDictionary(PageTab.Overview);
            if (dicDB == null) throw new ArgumentNullException(nameof(dicDB));
            var tabName = ReturnPageTab(PageTab.Overview);

            var xcode = "//*[@class=\"grid-row\"] | //dl | //*[@id=\"company-number\"] | //*[@id=\"company-name\"] | //*[@id=\"sic0\"] | //*[@id='no-sic']";
            HtmlNodeCollection tempNodes = data.DocumentNode.SelectNodes(xcode);

            if (tempNodes == null)
            {
                string msg = "Sorry, could not find page for query!";
                System.Windows.Forms.MessageBox.Show(msg);
                throw new Exception(msg);
            }

            var sb = NodeCollectionToSB( tempNodes );

            var strData = ReplaceSplitTrim(sb);

            try
            { 
                for (int i = 0; i < strData.Length; i++)
                {
                    for (int v = 0; v < htmlFields[tabName].Count; v++)
                    {
                        string _htmlField = htmlFields[tabName][v];
                        if ( strData[i].Contains( _htmlField ) )
                        {
                            int offset = 1;
                            if (v == 5 || v == 6) // Accounts and Confirmation Statement  CUSTOMDATE different position
                            {
                                offset = 2;
                                strData[i + offset] = strData[i + offset].Replace("due by", "");
                            }
                            dicDB[htmlFields[ tabName ][v]] = strData[i + offset];
                        }
                    }
                }
                // reading company name and SIC
                dicDB[htmlFields["Overview"][1]] = strData[0].Replace("&amp;", "&");
                dicDB[htmlFields["Overview"][7]] = strData[ strData.Length - 2];
            }
            catch
            {
                string msg = "Sorry, could not parse data";
                System.Windows.Forms.MessageBox.Show(msg);
                throw new Exception(msg);
            }

            return dicDB;
        }
       
        private HtmlDocument GetHtmlByCompany( string company = "10581927", PageTab tab = PageTab.Overview)
        {
            string url = "";
            
            switch (tab)
            {
                case PageTab.Overview:
                    url = string.Format("{0}company/{1}", _initialSite, company);
                    break;
                case PageTab.People:

                    url = string.Format("{0}company/{1}/officers", _initialSite, company);
                    break;
                case PageTab.FilingHistory:
                    url = string.Format("{0}company/{1}/filing-history", _initialSite, company);
                    break;
                default:
                    return null;
            }

            _htmlDoc = _webClient.Load(url);

            return _htmlDoc;
        }
    }
}
