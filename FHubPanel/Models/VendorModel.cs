using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class VendorModel : BaseModels
    {
        public VendorModel()
        {
            this.LogoFullPath = "/Content/dist/img/CatalogueNoImage.png";
            this.BGImageFullPath = "/Content/dist/img/CategoryNoImage.png";
            this.IsActive = true;
        }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        //[Required]
        public string UserName { get; set; }
        [MinLength(6, ErrorMessage = "Password must be more than 5 charactor.")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage="Password and Confirm Password mismatch.")]
        public string ConfirmPassword { get; set; }
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
        [EmailAddress(ErrorMessage="Wrong Email Address.")]
        public string EmailId { get; set; }
        public string WebSite { get; set; }
        public string LogoImg { get; set; }
        public string LogoFullPath { get; set; }
        public string VendorCode { get; set; }
        public bool IsActive { get; set; }
        public string ReferenceCode { get; set; }
        public string SubscriptionType { get; set; }
        public string AboutUs { get; set; }
        public string ProdDispName { get; set; }
        public string CatDispName { get; set; }
        public string BGImage { get; set; }
        public string BGImageFullPath { get; set; }
    }
}