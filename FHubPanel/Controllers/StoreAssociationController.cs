using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class StoreAssociationController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /StoreAssociation/
        public ActionResult Index()
        {
            ViewBag.MasterType = "Store Association";
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "StoreAssociation" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            return View();
        }

        public JsonResult AjaxHandler(string Search, int PageIndex, int PageSize, string Status, string TabId)
        {
            try
            {
                string _DefStr = "";
                if (TabId.ToUpper() == "TABREQUESTED")
                {
                    if (Status == "All")
                        _DefStr = " and VendorId = " + (int)Session["VendorId"] + " and (VendorStatus <> 'Deleted' and VendorStatus <> 'Cancelled')";
                    else
                        _DefStr = " and VendorId = " + (int)Session["VendorId"] + " and StoreStatus = '" + Status + "' and (VendorStatus <> 'Deleted' and  VendorStatus <> 'Cancelled')";

                }
                else if (TabId.ToUpper() == "TABREQRECEIVED")
                {
                    if (Status == "All")
                        _DefStr = " and RefStoreId = " + (int)Session["VendorId"] + " and (VendorStatus <> 'Deleted' and VendorStatus <> 'Cancelled')";
                    else
                        _DefStr = " and RefStoreId = " + (int)Session["VendorId"] + " and StoreStatus = '" + Status + "' and (VendorStatus <> 'Deleted' and VendorStatus <> 'Cancelled')";
                }

                List<sp_StoreAssociation_SelectWhereWithLazyload_Result> _ObjStoreAss = db.sp_StoreAssociation_SelectWhereWithLazyload(Search, PageSize, PageIndex, _DefStr).ToList();

                return Json(new
                {
                    Result = "OK",
                    data = _ObjStoreAss,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "",
                    data = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SendRequest(string VendorCode)
        {
            try
            {
                string _Msg = "";
                bool _Result = false;
                int _Val = 0;
                Vendor _ObjVendor = db.Vendors.Find((int)Session["VendorId"]);
                if (_ObjVendor.VendorCode == VendorCode)
                {
                    _Msg = "You can not send request to your own store!";
                    _Result = false;
                }
                else
                {
                    sp_Vendor_SelectWhere_Result _ObjVendorId = db.sp_Vendor_SelectWhere(" and VendorCode = '" + VendorCode + "'").FirstOrDefault();
                    if (_ObjVendorId != null)
                    {
                        string _Consition = " and  RefStoreId = " + _ObjVendorId.VendorId + " and VendorId = " + (int)Session["VendorId"] + " and (VendorStatus <> 'Deleted' and VendorStatus <> 'Cancelled')";
                        if (db.sp_StoreAssociation_SelectWhere(_Consition).ToList().Count == 0)
                        {
                            _Val = db.sp_StoreAssociation_Save((int)Session["VendorId"], VendorCode, CommanClass._Terminal).FirstOrDefault().Value;

                            if (_Val != -1)
                            {
                                

                                _Result = true;
                                StoreAssociation _ObjSA = db.StoreAssociations.Find(_Val);
                                Vendor _ObjStore = db.Vendors.Find(_ObjSA.RefStoreId);
                                Vendor _ObjVendorDet = db.Vendors.Find((int)Session["VendorId"]);

                                CommanClass._VendorId = (int)Session["VendorId"];

                                string _Subj = "", _SendText = "", _DetHeader = "";
                                _Subj = "Connect Store Request";
                                _SendText= "You have a request from " + (string)Session["VendorName"] + " for connecting your  store to access your products and catalogues to add in their listing. Kindly review and take appropriate action.";
                                _DetHeader = "Vendor Detail";
                                if (!CommanClass.MailRequestForAssociation(_ObjVendorDet, _ObjStore.EmailId, _Subj, _SendText, _DetHeader))
                                {
                                    _Msg = "Request successfully send , but slow internet connection then main not send!";
                                    _Result = false;
                                }
                            }
                            else
                            {
                                _Msg = "Server Error. Store request send to fail. Try again later!";
                                _Result = false;
                            }
                        }
                        else
                        {
                            _Msg = "Store already associated!";
                            _Result = false;
                        }
                    }
                    else
                    {
                        _Msg = "Wrong store code. Please check your requedted store code!";
                        _Result = false;
                    }
                }

                return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ChangeStatus(int Id, string Status, string sFor, DateTime? UpdDate)
        {
            try
            {
                bool _Result = false;
                string _Msg = "";

                StoreAssociation _ObjSA = db.StoreAssociations.Find(Id);

                // 
                if (UpdDate != null || _ObjSA.UpdDate != null)
                {
                    DateTime _DBUpdDate = Convert.ToDateTime(_ObjSA.UpdDate);
                    DateTime _PageDate = Convert.ToDateTime(UpdDate);
                    if (!_PageDate.ToString("dd-MM-yyyy HH:mm:ss").Equals(_DBUpdDate.ToString("dd-MM-yyyy HH:mm:ss")))
                    {
                        _Msg = "Refresh page record is updated.";
                        _Result = false;
                        return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);
                    }
                }

                string _Subj = "", _SendText = "";

                // Change status of Requested Store
                if (sFor == "Requested")
                {
                    _ObjSA.VendorStatus = Status;
                }
                else if (sFor == "ReqReceived")
                {
                    _ObjSA.StoreStatus = Status;
                    if (Status == "Approved")
                    {
                        _ObjSA.VendorStatus = Status;
                        _ObjSA.ApprovedDate = DateTime.Now;
                        _Subj = "Connect Store " + Status;
                        _SendText = "Your Connect Store request has been accepted. Kindly login to FashionDIVA admin panel and start exploring latest products and catalogues we are dealing with.";
                    }
                    else if (Status == "Rejected")
                    {
                        _Subj = "Associate " + Status;
                        _SendText = "Your Connect Store request has been rejected. ";
                    }

                }

                _ObjSA.UpdUser = (int)Session["VendorId"];
                _ObjSA.UpdDate = System.DateTime.Now; ;
                _ObjSA.UpdTerminal = CommanClass._Terminal;
                db.SaveChanges();

                if (_Subj != "")
                {
                    int _VId = db.sp_StoreAssociation_SelectWhere(" and Id = " + Id).FirstOrDefault().VendorId;
                    Vendor _ObjVendorDet = db.Vendors.Find(_VId);
                    if(!CommanClass.MailOnAction((string)Session["VendorName"], _ObjVendorDet.VendorName, _ObjVendorDet.EmailId,_Subj,_SendText))
                    {
                        _Msg = "Request  "+ Status + "successfully. Slow internet. Mail sending fail!";
                        _Result = true;
                        return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);
                    }
                }

                _Msg = "Store Assocaition successfully " + Status;
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