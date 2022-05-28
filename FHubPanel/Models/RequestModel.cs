using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class RequestModel
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public string LogoImg { get; set; }
        public string AUName { get; set; }
        public string VendorStatus { get; set; }
        public string AppUserStatus { get; set; }
        public string ReqDate { get; set; }
        public string ApproveDate { get; set; }
        public string ThumbnailImgPath { get; set; }
        public string CompanyName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
    }
}