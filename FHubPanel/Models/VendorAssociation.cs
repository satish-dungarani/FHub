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
    
    public partial class VendorAssociation
    {
        public int Id { get; set; }
        public Nullable<int> RefVendorId { get; set; }
        public Nullable<int> RefAUId { get; set; }
        public string VendorCode { get; set; }
        public string VendorStatus { get; set; }
        public string AppUserStatus { get; set; }
        public bool IsNotify { get; set; }
        public Nullable<System.DateTime> ReqDate { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<int> RateVendor { get; set; }
        public int InsUser { get; set; }
        public System.DateTime InsDate { get; set; }
        public string InsTerminal { get; set; }
        public Nullable<int> UpdUser { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<bool> IsAdminNotification { get; set; }
        public Nullable<System.DateTime> VisitDateTime { get; set; }
    
        public virtual AppUser AppUser { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
