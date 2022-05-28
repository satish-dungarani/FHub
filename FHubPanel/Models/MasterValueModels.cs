using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class MasterValueModels : BaseModels
    {
        public MasterValueModels()
        {
            this.IsActive = true;
        }

        public int RefMasterId { get; set; }
        public int RefVendorId { get; set; }
        public int Id { get; set; }
        public string ValueName { get; set; }
        public string ValueDesc { get; set; }
        public decimal OrdNo { get; set; }
        public bool IsActive { get; set; }
      
    }
}