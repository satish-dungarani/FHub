using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class WishListModel
    {
        public int Id { get; set; }
        public int auid { get; set; }
        public int vid { get; set; }
        public string pcode{ get; set; }
        public string thumb { get; set; }
        public string brand { get; set; }
        public string color { get; set; }
        public string design { get; set; }
        public string fabric { get; set; }
        public string category{ get; set; }
        public string ptype { get; set; }
        public string size { get; set; }
        public string ccode { get; set; }
        public string pname { get; set; }
        public Nullable<decimal> rprice { get; set; }
        public int pid { get; set; }
    }
}