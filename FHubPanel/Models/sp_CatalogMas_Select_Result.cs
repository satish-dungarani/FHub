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
    
    public partial class sp_CatalogMas_Select_Result
    {
        public string CatCode { get; set; }
        public string CatDescription { get; set; }
        public int CatId { get; set; }
        public string CatImg { get; set; }
        public string ThumbnailImgPath { get; set; }
        public string CatName { get; set; }
        public int RefVendorId { get; set; }
        public string VendorName { get; set; }
        public Nullable<int> TotalProduct { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<System.DateTime> CatLaunchDate { get; set; }
        public string RefProdCategory { get; set; }
        public string RefColor { get; set; }
        public string RefProdType { get; set; }
        public string RefSize { get; set; }
        public string RefFabric { get; set; }
        public string RefDesign { get; set; }
        public string RefBrand { get; set; }
        public Nullable<bool> IsFullset { get; set; }
        public Nullable<decimal> TotalRetailPrice { get; set; }
        public Nullable<decimal> AvgWholeSalePrice { get; set; }
    }
}