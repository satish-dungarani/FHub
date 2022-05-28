using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class VendorSliderModel : BaseModels
    {
        public VendorSliderModel()
        {
            this.FullImgPath = "/Content/dist/img/no_image_available.jpg";
        }

        public int SliderId { get; set; }
        public int RefVendorId { get; set; }
        public string SliderTitle { get; set; }
        public string SliderImg { get; set; }
        public string SliderUrl { get; set; }
        public Nullable<int> Ord { get; set; }
        public string DisplayPage { get; set; }
        public string Category { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
        public string FullImgPath { get; set; }
        public string Expired { get; set; }
    }
}