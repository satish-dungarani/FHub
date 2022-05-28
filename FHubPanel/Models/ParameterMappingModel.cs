using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class ParameterMappingModel
    {
        public int RefMasterId { get; set; }
        public string MasterValName { get; set; }
        public int RefVendorId { get; set; }
        public int RefVendorValId { get; set; }
        public string RefVendorValName { get; set; }
        public string RefVendorValDesc { get; set; }
        public string VendorName { get; set; }
        public int RefStoreId { get; set; }
        public int RefStoreValId { get; set; }
        public string RefStoreValName { get; set; }
        public string RefStoreValDesc { get; set; }
        public string StoreName { get; set; }
        //For Mapped = M, UnMapped = U, AlreadyInMaster = A
        public string MappedStatus { get; set; }
        
    }
}