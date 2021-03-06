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
    
    public partial class Vendor
    {
        public Vendor()
        {
            this.AppLogs = new HashSet<AppLog>();
            this.CatalogMas = new HashSet<CatalogMa>();
            this.GroupMas = new HashSet<GroupMa>();
            this.ProductCategories = new HashSet<ProductCategory>();
            this.ProductMas = new HashSet<ProductMa>();
            this.VendorAssociations = new HashSet<VendorAssociation>();
            this.VendorSliders = new HashSet<VendorSlider>();
            this.ParameterMappings = new HashSet<ParameterMapping>();
            this.ParameterMappings1 = new HashSet<ParameterMapping>();
        }
    
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string ContactName { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public string MobileNo1 { get; set; }
        public string MobileNo2 { get; set; }
        public string FaxNo { get; set; }
        public string EmailId { get; set; }
        public string WebSite { get; set; }
        public string LogoImg { get; set; }
        public string VendorCode { get; set; }
        public string ReferalCode { get; set; }
        public string ReferenceCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int InsUser { get; set; }
        public System.DateTime InsDate { get; set; }
        public string InsTerminal { get; set; }
        public Nullable<int> UpdUser { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
        public string AboutUs { get; set; }
        public string ProdDispName { get; set; }
        public string CatDispName { get; set; }
        public string BGImage { get; set; }
    
        public virtual ICollection<AppLog> AppLogs { get; set; }
        public virtual ICollection<CatalogMa> CatalogMas { get; set; }
        public virtual ICollection<GroupMa> GroupMas { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<ProductMa> ProductMas { get; set; }
        public virtual ICollection<VendorAssociation> VendorAssociations { get; set; }
        public virtual ICollection<VendorSlider> VendorSliders { get; set; }
        public virtual ICollection<ParameterMapping> ParameterMappings { get; set; }
        public virtual ICollection<ParameterMapping> ParameterMappings1 { get; set; }
    }
}
