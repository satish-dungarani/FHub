using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHubPanel.Models
{
    public partial class MenuMasterModel
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string MenuDes { get; set; }
        public bool IsActive { get; set; }
        public int? ParentManuId { get; set; }
        public int? refMenuGroupId { get; set; }
        public decimal OrderNo { get; set; }
        public string MenuIcon { get; set; }
        public string ConstrollerName { get; set; }
        public string ActionName { get; set; }
        public string MenuPath { get; set; }

    }

    public partial class MenuRoleRightsModel
    {
        public int RefRoleId { get; set; }
        public string MenuName { get; set; }
        public int RefMenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public bool CanInsert { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public int InsUser { get; set; }
        public Nullable<System.DateTime> InsDate { get; set; }
        public string InsTerminal { get; set; }
        public int UpdUser { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
        public string UpdTerminal { get; set; }
    }
}