using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class CatalogApiModel
    {
        public string ccode { get; set; }
        public int cid { get; set; }
        public string thumb { get; set; }
        public string cname { get; set; }
        public Nullable<int> tprod { get; set; }
        public Nullable<decimal> tprice { get; set; }
        public string ldate { get; set; }
        public string pcatg { get; set; }
        public string color { get; set; }
        public string rtype { get; set; }
        public string size { get; set; }
        public string fabric { get; set; }
        public string design { get; set; }
        public string brand { get; set; }
        public Nullable<bool> set { get; set; }
        public Nullable<decimal> trprice { get; set; }
        public Nullable<decimal> awprice { get; set; } 
    }
}