using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class NotificationController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /Notification/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Notification" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Notification";
            return View(GetNotificationHisteory());
        }

        public List<sp_NotificationMas_Select_Result> GetNotificationHisteory()
        {
            try
            {
                List<sp_NotificationMas_Select_Result> _ObjNotify;

                _ObjNotify = db.sp_NotificationMas_Select((int)Session["VendorId"]).ToList();

                return _ObjNotify;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Manage()
        {
            try
            {
                CommanClass._VendorId = (int)Session["VendorId"];
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Notification" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                ViewBag.MasterType = "Notification";
                ViewData["GroupList"] = CommanClass.GetContactGroupList();
                ViewData["ContactList"] = CommanClass.GetContactList("0");
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Save(NotificationMa _ObjParam)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    if (_ObjParam != null)
                    {
                        bool Result = db.sp_Notification_Save(_ObjParam.NotifyId, (int)Session["VendorId"], System.DateTime.Now.Date, _ObjParam.RefGroupId, _ObjParam.RefAppUserId,
                                _ObjParam.Message, _ObjParam.ImgPath, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;
                        if (Result)
                            TempData["Success"] = "Notification send successfully.!";
                        else
                            TempData["Warninig"] = "Server Error. Notification fail to send!";
                    }
                //}
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetContactList(string GroupIdList)
        {
            try
            {
                CommanClass._VendorId = (int)Session["VendorId"];
                if (string.IsNullOrEmpty(GroupIdList))
                    GroupIdList = "0";
                ViewData["ContactList"] = CommanClass.GetContactList(GroupIdList);
                return PartialView("_ContactListPartial");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}