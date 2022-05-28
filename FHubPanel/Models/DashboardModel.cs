using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public class DashboardModel
    {
        public DashboardModel()
        {
            CateWiseProd = new List<CategoryWiseProduct>();
            MostActive = new List<MostActiveUser>();
            MostCat = new List<MostViewCatalog>();
        }
        public int ActiveMember { get; set; }
        public int InActiveMember { get; set; }
        public int RejectedMember { get; set; }
        public int TotalMember { get; set; }
        public int TotalCatalogs { get; set; }
        public int TotalProducts { get; set; }
        public int ResponseGiven { get; set; }
        public int ResponsePending { get; set; }
        public int TotalInquiry { get; set; }
        public List<CategoryWiseProduct> CateWiseProd { get; set; }
        public List<MostActiveUser> MostActive { get; set; }
        public List<MostViewCatalog> MostCat{ get; set; }

    }

    public class CategoryWiseProduct
    {
        public string CategoryName { get; set; }
        public int NoofProducts { get; set; }
    }

    public class MostActiveUser
    {
        public string AppUserName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> AssDate { get; set; }
    }

    public class MostViewCatalog
    {
        public string CatName { get; set; }
        public string CatCode { get; set; }
        public int TotalProduct { get; set; }
    }
}