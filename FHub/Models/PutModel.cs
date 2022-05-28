using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHub.Models
{
    public class PutModel
    {
        public int AUId { get; set; }
        public int VendorAssociationId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}