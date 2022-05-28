using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class ProductApiModel
    {
        public int pid { get; set; }
        public string pcode { get; set; }
        public string pcatg { get; set; }
        public string color { get; set; }
        public string rtype { get; set; }
        public string size { get; set; }
        public string fabric { get; set; }
        public string design { get; set; }
        public string brand { get; set; }
        public Nullable<decimal> rprice { get; set; }
        public Nullable<decimal> wprice { get; set; }
        public string thumb { get; set; }
        public Nullable<bool> wish { get; set; }
        public Nullable<int> rcati { get; set; }
        public string CatCode { get; set; }
        public Nullable<bool> set { get; set; }
        public string ldate { get; set; }
        public string pname { get; set; }
    }
}