using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrabbingToSql.Parser;

namespace GrabbingToSql
{
    public class ConfigLoader : IConfigLoader
    {
        public Dictionary<string, string> LoadFields(PageTab tab = PageTab.Overview)
        {
            string prefixName;
            var tempDic = new Dictionary<string, string>();

            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "/fields.ini";
            IniFile file = new IniFile(path);

            if (tab == PageTab.LoadSiteData)
            {
                tempDic.Add("Site", file.IniReadValue("SiteData", "Site"));
                tempDic.Add("OverviewSyntax", file.IniReadValue("SiteData", "OverviewSyntax"));
                tempDic.Add("PeopleSyntax", file.IniReadValue("SiteData", "PeopleSyntax"));
                tempDic.Add("FilingHistorySyntax", file.IniReadValue("SiteData", "FilingHistorySyntax"));
                return tempDic;
            }

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

}
