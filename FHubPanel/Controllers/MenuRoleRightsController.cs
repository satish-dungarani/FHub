using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class MenuRoleRightsController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /MenuRoleRights/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "MenuRoleRights" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewData["RoleList"] = CommanClass.GetRoleList();
            ViewBag.MasterType = "Role Rights";
            return View();
        }

        public PartialViewResult LoadRoleRights(int RoleId)
        {
            try
            {
                List<MenuRoleRightsModel> _ObjMenuRights = new List<MenuRoleRightsModel>();
                _ObjMenuRights = db.sp_MenuRoleRights_Select(RoleId).Select(x => new MenuRoleRightsModel
                {
                    RefRoleId = Convert.ToInt32(x.RefRoleId),
                    RefMenuId = x.RefMenuId,
                    MenuName = x.MenuName,
                    ParentMenuId = x.ParentMenuID,
                    CanInsert = x.CanInsert,
                    CanUpdate = x.CanUpdate,
                    CanDelete = x.CanDelete,
                    CanView = x.CanView
                }).ToList();
                return PartialView("_LoadRoleRightsPartial", _ObjMenuRights);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult Save(List<MenuRoleRightsModel> _ObjParam)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(_ObjParam.Count > 0)
                    {
                        db.sp_MenuRoleRights_Delete(_ObjParam[0].RefRoleId);
                    }
                    
                    foreach (var _Obj in _ObjParam)
                    {
                        db.sp_MenuRoleRights_Save(_Obj.RefRoleId, _Obj.RefMenuId, _Obj.CanInsert, _Obj.CanUpdate, _Obj.CanDelete, _Obj.CanView, (int)Session["VendorId"], CommanClass._Terminal);
                    }
                }
                return Json(new { Result = true, Message = "Role Roghts Successfully allocated." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}