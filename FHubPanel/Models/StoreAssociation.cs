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
    
    public partial class StoreAssociation
    {
        public int Id { get; set; }
        public int RefStoreId { get; set; }
        public int RefVendorId { get; set; }
        public string StoreCode { get; set; }
        public string StoreStatus { get; set; }
        public string VendorStatus { get; set; }
        public System.DateTime ReqDate { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public int InsUser { get; set; }
        public System.DateTime InsDate { get; set; }
        public string InsTerminal { get; set; }
        public Nullable<int> UpdUser { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
    
        public virtual StoreAssociation StoreAssociation1 { get; set; }
        public virtual StoreAssociation StoreAssociation2 { get; set; }
    }
}