//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FHubPanel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CatalogMa
    {
        public int CatId { get; set; }
        public int RefVendorId { get; set; }
        public string CatCode { get; set; }
        public string CatName { get; set; }
        public string CatImg { get; set; }
        public string CatDescription { get; set; }
        public Nullable<System.DateTime> CatLaunchDate { get; set; }
        public Nullable<bool> IsFullset { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int InsUser { get; set; }
        public System.DateTime InsDate { get; set; }
        public string InsTerminal { get; set; }
        public Nullable<int> UpdUser { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
        public Nullable<int> RefCatId { get; set; }
        public Nullable<int> RefStoreId { get; set; }
    
        public virtual Vendor Vendor { get; set; }
    }
}
