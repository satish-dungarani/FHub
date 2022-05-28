using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class CatalogModel : BaseModels
    {

        public CatalogModel()
        {
            this.FullImgPath = "/Content/dist/img/CatalogueNoImage.png";
            this.IsActive = true;
            ProductList = new List<sp_ProductMas_SelectWhere_Result>();
        }
        public int CatId { get; set; }
        public int RefVendorId { get; set; }
        public string CatCode { get; set; }
        public string CatName { get; set; }
        public string CatImg { get; set; }
        public string CatDescription { get; set; }
        public Nullable<DateTime> CatLaunchDate { get; set; }
        public bool IsFullset { get; set; }
        public bool IsActive { get; set; }
        public VendorModel Vendor { get; set; }
        public string FullImgPath { get; set; }
        public int? TotalItem { get; set; }
        public List<sp_ProductMas_SelectWhere_Result> ProductList { get; set; }
    }
}