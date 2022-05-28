using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.Configuration;

namespace FHubPanel.Controllers
{
    public class BaseController : Controller
    {
        public static int ProdHeight = Convert.ToInt32(ConfigurationManager.AppSettings["ProdHeight"].ToString());
        public static int ProdWidth = Convert.ToInt32(ConfigurationManager.AppSettings["ProdWidth"].ToString());
        public static int CataHegiht = Convert.ToInt32(ConfigurationManager.AppSettings["CataHeight"].ToString());
        public static int CataWidth = Convert.ToInt32(ConfigurationManager.AppSettings["CataWidth"].ToString());
        public static int SliderHeight = Convert.ToInt32(ConfigurationManager.AppSettings["SliderHeight"].ToString());
        public static int SliderWidth = Convert.ToInt32(ConfigurationManager.AppSettings["SliderWidth"].ToString());
        public static int VendorHeight = Convert.ToInt32(ConfigurationManager.AppSettings["VendorHeight"].ToString());
        public static int VendorWidth = Convert.ToInt32(ConfigurationManager.AppSettings["VendorWidth"].ToString());
        public static int CategoryHeight = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryHeight"].ToString());
        public static int CategoryWidth = Convert.ToInt32(ConfigurationManager.AppSettings["CategoryWidth"].ToString());
        string _Subj = "", _SendText = "";
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /Baser/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Session["UserName"] == null || Session == null)
            {
                //|| filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper() == "HOME"
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper() == "SETUP")
                {
                    var _Result = new JsonResult();
                    _Result.Data = new { Saved = "No" };
                    _Result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    filterContext.Result = _Result;
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Account/Login");
                }
                return;
            }
            else
            {
                if (Session["VendorId"] != null)
                {
                    EnquirySession();
                    AppUserRequestList();
                    VendorRequestList();
                }

                List<MenuMasterModel> _ObjMenuList = (List<MenuMasterModel>)Session["AccessMenuList"];
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString() != "Vendor" && filterContext.ActionDescriptor.ActionName.ToString() != "Manage" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString() != "Home" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString() != "Setup" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString() != "Base" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString() != "BulkImageUpload")
                {
                    if (_ObjMenuList.Where(x => x.ConstrollerName == filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString()).ToList().Count != 1)
                    {
                        filterContext.Result = new RedirectResult("/Home/Index");
                    }
                }
            }
        }

        public PartialViewResult ChangeRequestStatus(int _Id, string _Status)
        {
            try
            {
                //bool _Valid = true;
                FHubDBEntities db = new FHubDBEntities();
                if (_Status == "Approved")
                {
                    int? _NAppUsers = db.sp_VendorSubDet_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " Order by ValidToDate Desc").FirstOrDefault().NoOfAppUser;
                    int _CountofActiveUser = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and VendorStatus = 'Approved' and AppUserStatus = 'Approved'").ToList().Count;
                    if (_CountofActiveUser >= _NAppUsers)
                    {
                        //_Valid = false;
                        //TempData["Warning"] = "You can not approve appuser request. Appuser association limit is over!";
                        //throw new Exception("");
                        throw new Exception("You can not approve appuser request. Appuser association limit is over!");
                    }
                }


                VendorAssociation _ObjVA = db.VendorAssociations.Find(_Id);
                _ObjVA.VendorStatus = _Status;
                if (_Status == "Approved")
                {
                    _ObjVA.AppUserStatus = _Status;
                    _ObjVA.ApproveDate = System.DateTime.Now;
                }
                db.SaveChanges();

                if (_Status == "Approved")
                {
                    _Subj = "Association Request " + _Status;
                    _SendText = "Your association request has been accepted. Kindly visit store on FashionDiva App for latest updates on products and catalogues.";
                }
                else if (_Status == "Rejected")
                {
                    _Subj = "Association Request " + _Status;
                    _SendText = "Your association request has been rejected. ";
                }

                CommanClass.MailOnAction((string)Session["VendorName"], _ObjVA.AppUser.AUName, _ObjVA.AppUser.EmailId, _Subj, _SendText);

                string _ConditionForAU = " and Id = " + _Id;
                var _ObjAU = db.sp_VendorAssociation_SelectWhere(_ConditionForAU).FirstOrDefault();
                string _Message = _ObjAU.VendorName + " had " + _Status + " your accosiation request. ";
                if (_Status == "Approved")
                    _Message += "Tap to visit our store.";
                FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Message, "Associate Request Status", Convert.ToInt32(_ObjAU.RefVendorId), "", "", "", "","VL");
                //_ObjAU.ThumbnailImgPath
                AppUserRequestList();
                return PartialView("_AppUserRequestPartial", (List<sp_VendorAssociation_SelectWhere_Result>)Session["AppUserRequest"]);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult ChangeStoreRequestStatus(int _Id, string _Status)
        {
            try
            {
                FHubDBEntities db = new FHubDBEntities();
                StoreAssociation _ObjSA = db.StoreAssociations.Find(_Id);
                _ObjSA.StoreStatus = _Status;
                if (_Status == "Approved")
                {
                    _ObjSA.VendorStatus = _Status;
                    _ObjSA.ApprovedDate = System.DateTime.Now;
                }
                _ObjSA.UpdUser = (int)Session["VendorId"];
                _ObjSA.UpdDate = System.DateTime.Now;
                _ObjSA.UpdTerminal = CommanClass._Terminal;
                db.SaveChanges();

                if (_Status == "Approved")
                {
                    _Subj = "Connect Store " + _Status;
                    _SendText = "Your Connect Store request has been accepted. Kindly login to FashionDIVA admin panel and start exploring latest products and catalogues we are dealing with.";
                }
                else if (_Status == "Rejected")
                {
                    _Subj = "Associate " + _Status;
                    _SendText = "Your Connect Store request has been rejected. ";
                }

                int _VId = db.sp_StoreAssociation_SelectWhere(" and Id = " + _Id).FirstOrDefault().VendorId;
                Vendor _ObjVendorDet = db.Vendors.Find(_VId);

                CommanClass.MailOnAction((string)Session["VendorName"], _ObjVendorDet.VendorName, _ObjVendorDet.EmailId, _Subj, _SendText);

                VendorRequestList();
                return PartialView("_VendorRequestPartial", (List<sp_StoreAssociation_SelectWhere_Result>)Session["VendorRequest"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void VendorRequestList()
        {
            try
            {
                Session["VendorRequest"] = db.sp_StoreAssociation_SelectWhere(" and RefStoreId = " + (int)Session["VendorId"] + " and upper(StoreStatus) = 'PENDING' and VendorStatus = 'Requested'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AppUserRequestList()
        {
            try
            {
                Session["AppUserRequest"] = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and upper(VendorStatus) = 'PENDING' and AppUserStatus = 'Requested'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult EnquiryData()
        {
            try
            {
                List<EnquiryList> _ObjEnq = new List<EnquiryList>();
                _ObjEnq = (List<EnquiryList>)Session["Enquiry"];
                return PartialView("_EnquiryPartial", _ObjEnq);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnquirySession()
        {
            try
            {
                Session["Enquiry"] = db.sp_EnquiryList_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and ReadDateTime Is NULL ").Select(x => new sp_EnquiryList_SelectWhere_Result()
                {
                    Id = x.Id,
                    RefAUID = x.RefAUID,
                    RefProdId = x.RefProdId,
                    RefVendorId = x.RefVendorId,
                    EnquiryRepDate = x.EnquiryRepDate,
                    ReadDateTime = x.ReadDateTime,
                    ProdCode = x.ProdCode,
                    ProdName = x.ProdName,
                    Status = x.Status,
                    ThumbnailImgPath = x.ThumbnailImgPath,
                    VendorName = x.VendorName,
                    CompanyName = x.CompanyName,
                    EnquiryDate = x.EnquiryDate,
                    AUName = x.AUName,
                    Remark = x.Remark.Length > 30 ? x.Remark.Substring(0, 22) + "..." : x.Remark
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetEnquiryDetail(int EnquiryId)
        {
            try
            {
                sp_EnquiryList_SelectWhere_Result _ObjEnqDetail = new sp_EnquiryList_SelectWhere_Result();

                if (EnquiryId > 0)
                {
                    _ObjEnqDetail = db.sp_EnquiryList_SelectWhere(" and Id = " + EnquiryId).FirstOrDefault();
                    EnquiryList _Obj = db.EnquiryLists.Find(EnquiryId);
                    _Obj.ReadDateTime = System.DateTime.Now;
                    db.SaveChanges();
                }

                if (_ObjEnqDetail != null)
                    return Json(new { Result = true, Data = _ObjEnqDetail }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Result = false, Data = _ObjEnqDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}