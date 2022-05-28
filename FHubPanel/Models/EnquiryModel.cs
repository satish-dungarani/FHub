using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class EnquiryModel
    {
        public int Id { get; set; }
        public int auid { get; set; }
        public string auname { get; set; }
        public string mno { get; set; }
        public string compname { get; set; }
        public string ccode { get; set; }
        public Nullable<int> cid { get; set; }
        public string cname { get; set; }
        public Nullable<int> pid { get; set; }
        public string pcode { get; set; }
        public string pname { get; set; }
        public string fabric { get; set; }
        public string color { get; set; }
        public string thumb{ get; set; }
        public Nullable<decimal> rprice { get; set; }
        public Nullable<decimal> tprice { get; set; }
        public string Remark { get; set; }
        public string edate { get; set; }
        public string Status { get; set; }
        public string reply { get; set; }
        public string rdate{ get; set; }
        public string rby { get; set; }
    }
}