using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class ContactController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /Contact/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Contact" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Customer";
            return View();
        }

        public JsonResult AjaxHandler(string Search, int PageSize, int PageIndex, string VendorStatus)
        {
            try
            {
                List<sp_AppUser_SelectBaseOnVendor_Result> _ObjAppUser = new List<sp_AppUser_SelectBaseOnVendor_Result>();
                _ObjAppUser = db.sp_AppUser_SelectBaseOnVendor(Search, VendorStatus, (int)Session["VendorId"], PageSize, PageIndex).ToList();

                return Json(new
                {
                    Result = "OK",
                    data = _ObjAppUser,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "ERROR",
                    data = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChangeAdminStatus(int VAId, bool Status, string Type)
        {
            try
            {
                bool _Result = false;
                string _Msg = "";
                VendorAssociation _ObjVA = db.VendorAssociations.Find(VAId);
                if (Type.ToUpper() == "ADMIN")
                {
                    _ObjVA.IsAdmin = Status;
                    _Msg = "App User Set as a Admin.";
                }
                else
                {
                    if (Convert.ToBoolean(_ObjVA.IsAdmin))
                    {
                        _ObjVA.IsAdminNotification = Status;
                        _Msg = "App User Admin Notifiaction set.";
                    }
                    else
                    {
                        _Msg = "Please set App User as admin.";
                        return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                _ObjVA.UpdUser = (int)Session["VendorId"];
                _ObjVA.UpdDate = System.DateTime.Now;
                _ObjVA.UpdTerminal = CommanClass._Terminal;
                db.SaveChanges();
                _Result = true;

                return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}