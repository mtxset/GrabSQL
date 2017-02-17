using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabbingToSql.JSON
{
    static public class FilingHistory
    {
        public class DescriptionValues
        {
            public string made_up_date { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public string document_metadata { get; set; }
        }

        public class Capital
        {
            public string currency { get; set; }
            public string figure { get; set; }
        }

        public class DescriptionValues2
        {
            public List<Capital> capital { get; set; }
            public string date { get; set; }
        }

        public class Data
        {
        }

        public class AssociatedFiling
        {
            public object action_date { get; set; }
            public string category { get; set; }
            public string date { get; set; }
            public string description { get; set; }
            public DescriptionValues2 description_values { get; set; }
            public string original_description { get; set; }
            public string type { get; set; }
            public Data data { get; set; }
        }

        public class Item
        {
            public DescriptionValues description_values { get; set; }
            public string date { get; set; }
            public Links links { get; set; }
            public string action_date { get; set; }
            public string description { get; set; }
            public string category { get; set; }
            public string type { get; set; }
            public int pages { get; set; }
            public string barcode { get; set; }
            public string transaction_id { get; set; }
            public List<AssociatedFiling> associated_filings { get; set; }
            public bool? paper_filed { get; set; }
        }

        public class RootObject
        {
            public int start_index { get; set; }
            public string filing_history_status { get; set; }
            public List<Item> items { get; set; }
            public int total_count { get; set; }
            public int items_per_page { get; set; }
        }
    }
}
