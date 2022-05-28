using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class EnquiryController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /Enquiry/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Enquiry" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Product Enquiry";
            return View();
        }

        public JsonResult Send(int EnqId, string RepMessage)
        {
            try
            {
                EnquiryList _ObjEnq = db.EnquiryLists.Find(EnqId);
                _ObjEnq.RepRemark = RepMessage;
                _ObjEnq.EnquiryRepDate = System.DateTime.Now;
                _ObjEnq.Status = "R";
                db.SaveChanges();

                var _ObjAU = db.sp_AppUser_Select(_ObjEnq.RefAUId).FirstOrDefault();
                var _ObjEnquiry = db.sp_EnquiryList_SelectWhere(" and Id = " + EnqId).FirstOrDefault();
                string _CNvalue;
                string _Title = "";
                string _ProdId = "" ;

                if (_ObjEnquiry.ProdCode != null && _ObjEnquiry.ProdCode != "")
                {
                    _Title = "Product Enquiry Reply";
                    _CNvalue = _ObjEnquiry.ProdCode + "-" + _ObjEnquiry.ProdName;
                    _ProdId = Convert.ToString(_ObjEnquiry.RefProdId);
                }
                else
                {
                    _CNvalue = _ObjEnquiry.CatCode + "-" + _ObjEnquiry.CatName;
                    _Title = "Catalogue Enquiry Reply";
                }
                    
                string _Message = _ObjEnquiry.VendorName + " had replyed  of your enquiry for " + _CNvalue + ". ";
                FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Message, _Title, _ObjEnquiry.RefVendorId, _ProdId, _ObjEnquiry.CatCode, Convert.ToString(EnqId), _ObjEnquiry.ThumbnailImgPath, "ENQANS");

                TempData["Success"] = "Reply successfully send!";
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult CancelReply(int EnqId)
        {
            try
            {
                EnquiryList _Obj = db.EnquiryLists.Find(EnqId);
                _Obj.Status = "C";
                db.SaveChanges();

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetenquiryList(string _Type)
        {
            try
            {
                if (_Type == "P")
                    ViewBag.Type = "Pending";
                else if (_Type == "R")
                    ViewBag.Type = "Replied";
                else if (_Type == "C")
                    ViewBag.Type = "Canceled";

                List<sp_EnquiryList_SelectWhere_Result> _Obj = db.sp_EnquiryList_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and Status = '" + _Type + "' Order by EnquiryDate Desc").ToList();

                return PartialView("_EnquiryListPartial", _Obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}