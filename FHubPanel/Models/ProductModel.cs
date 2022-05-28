using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class ProductModel : BaseModels
    {
        public ProductModel()
        {
            this.IsActive = true;
            this.FullImgPath = "/Content/dist/img/ProductNoImage.png";
        }
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public int RefVendorId { get; set; }
        public int VendorName { get; set; }
        public Nullable<int> RefCatId { get; set; }
        public int CatalogName { get; set; }
        public string ProdCode { get; set; }
        public string ProdDescription { get; set; }
        public string RefProdCategory { get; set; }
        public string RefColor { get; set; }
        public string RefColorList { get; set; }
        public string RefProdType { get; set; }
        public string RefSize { get; set; }
        public string RefSizeList { get; set; }
        public string RefFabric { get; set; }
        public string RefDesign { get; set; }
        public string RefBrand { get; set; }
        public string Celebrity { get; set; }
        public string ProdImgPath { get; set; }
        public Nullable<System.DateTime> ActivetillDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<decimal> RetailPrice { get; set; }
        public Nullable<decimal> WholeSalePrice { get; set; }
        public string FullImgPath { get; set; }
        public int ChangeImgId { get; set; }

    }
}