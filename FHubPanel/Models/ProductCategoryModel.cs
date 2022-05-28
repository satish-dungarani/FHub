using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class ProductCategoryModel : BaseModels
    {
        public ProductCategoryModel()
        {
            this.ImgFullPath = "/Content/dist/img/CategoryNoImage.png";
        }
        public int PCId { get; set; }
        public int RefVendorId { get; set; }
        public string ProdCategoryName { get; set; }
        public string ProdCategoryDesc { get; set; }
        public int? RefPCId { get; set; }
        public int? Ord { get; set; }
        public string ProdCategoryImg { get; set; }
        public string ImgFullPath { get; set; }
    }
}