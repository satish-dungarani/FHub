using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class GroupContactsModel
    {
            public int Id { get; set; }
            public Nullable<int> RefAUId { get; set; }
            public Nullable<int> RefVendorId { get; set; }
            public string AUName { get; set; }
            public bool InGroup { get; set; }
            public string EmailId { get; set; }
            public string MobileNo1 { get; set; }
            public string CompanyName { get; set; }
    }
}