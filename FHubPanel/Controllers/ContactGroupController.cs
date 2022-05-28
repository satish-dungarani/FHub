using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class ContactGroupController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /ContactGroup/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "ContactGroup" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Customer Group";
            return View();
        }

        public JsonResult AjaxHandler(string Search)
        {
            try
            {
                List<sp_GroupMas_SelectBaseOnVendor_Result> _ObjGroup = db.sp_GroupMas_SelectBaseOnVendor(Search, (int)Session["VendorId"]).ToList();
                List<sp_VendorAssociation_SelectWhere_Result> _ObjVAVenRej = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and VendorStatus = 'Rejected'").ToList();
                List<sp_VendorAssociation_SelectWhere_Result> _ObjVAAURej = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and AppUserStatus = 'Deleted'").ToList();
                return Json(new
                {
                    Result = "OK",
                    data = _ObjGroup,
                    VendorRejectedList = _ObjVAVenRej,
                    AURejectedList = _ObjVAAURej,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Error",
                    data = "",
                    VendorRejectedList = "",
                    AURejectedList = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult GrpContactList(int _GroupId)
        {
            try
            {
                List<sp_GroupContact_SelectBaseOnGroupId_Result> _ObjGC = db.sp_GroupContact_SelectBaseOnGroupId(_GroupId).ToList();

                return Json(new
                {
                    Result = "OK",
                    detaildata = _ObjGC,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Error",
                    detaildata = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult FullContactList(int RefGroupId)
        {
            try
            {
                List<GroupContactsModel> _ObjModel = db.sp_VendorAssociation_SelectBaseOnVendor((int)Session["VendorId"], RefGroupId).Select(x => new GroupContactsModel()
                {
                    Id = x.Id,
                    RefAUId = x.RefAUId,
                    RefVendorId = x.RefVendorId,
                    AUName = x.AUName,
                    EmailId = x.EmailId,
                    MobileNo1 = x.MobileNo1,
                    CompanyName = x.CompanyName
                }).ToList();
                return PartialView("_ChooseContactPartial", _ObjModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Save(GroupMa _ObjParam)
        {
            try
            {

                bool _Result = db.sp_GroupMas_Save(_ObjParam.GroupId, (int)Session["VendorId"], _ObjParam.GroupName, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;
                if (_Result)
                {
                    TempData["Success"] = "Group Successfully Created!";
                }
                else
                {
                    TempData["Warning"] = "Group Fail to create!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SaveContact(List<GroupContactsModel> _ObjParam, int RefGroupId)
        {
            try
            {

                if (_ObjParam != null)
                {
                    if (_ObjParam.Count > 0)
                    {
                        foreach (var _Obj in _ObjParam)
                        {
                            if (_Obj.InGroup)
                            {
                                db.sp_GroupContact_Save(_Obj.Id, RefGroupId, _Obj.RefAUId, (int)Session["VendorId"], CommanClass._Terminal);
                            }
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public JsonResult Delete(int Id)
        {
            try
            {
                GroupContactList _objGCL = db.GroupContactLists.Find(Id);
                db.GroupContactLists.Remove(_objGCL);
                db.SaveChanges();

                return Json(new { Result = true, Message = "AppUser successfully remove from group!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteGroup(int Id)
        {
            try
            {
                if(db.sp_GroupContact_DeleteBaseOnGroupId(Id).FirstOrDefault().Value)
                {
                    GroupMa _objGMas = db.GroupMas.Find(Id);
                    db.GroupMas.Remove(_objGMas);
                    db.SaveChanges();
                }

                return Json(new { Result = true, Message = "Group successfully removed!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}