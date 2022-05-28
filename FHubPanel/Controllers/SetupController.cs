using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class SetupController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /Setup/
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.MenuPath == "editSessionMasterID('" + (int)Session["RefMasterId"] + "')" ).FirstOrDefault().MenuIcon;
            ViewBag.MasterType = (CommanClass.MasterList)(int)Session["RefMasterId"] + " Setup";
            ViewBag.SetupId = Session["RefMasterId"];
            return View();
        }

        // Store Unique Id for get Data
        public JsonResult EditSession(int pid)
        {
            try
            {
                
                Session["MasterId"] = pid;
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AjaxHandler()
        {
            List<sp_MasterValue_Select_Result> _ObjMasterValue;
            try
            {
                dynamic result;
                if (Session["RefMasterId"] == null || string.IsNullOrEmpty(Session["RefMasterId"].ToString()))
                {
                    return RedirectToAction("index", "home");
                }
                _ObjMasterValue = db.sp_MasterValue_Select(null, (int)Session["RefMasterId"], (int)Session["VendorId"]).ToList();
                if((int)Session["RefMasterId"] == (int)CommanClass.MasterList.Color)
                {
                    result = from c in _ObjMasterValue
                                 select new[] 
                             { 
                                 "<span><img style=\"background-color:" + c.ValueDesc  + ";border:1px;border-style:solid\" src=\"/Content/dist/img/selectcolor.png\" class=\"img-flag\" /> " + c.ValueName + "</span>",
                                 "",
                                 Convert.ToString(c.OrdNo), 
                                 Convert.ToString(c.IsActive), 
                                 Convert.ToString(c.ID),
                                 Convert.ToString(c.RefVendorId)
                             };
                }
                else
                {
                    result = from c in _ObjMasterValue
                                 select new[] 
                             { 
                                 c.ValueName, c.ValueDesc, 
                                 Convert.ToString(c.OrdNo), 
                                 Convert.ToString(c.IsActive), 
                                 Convert.ToString(c.ID),
                                 Convert.ToString(c.RefVendorId)
                             };
                }
                
                return Json(new
                {
                    sEcho = "",
                    iTotalRecords = _ObjMasterValue.Count,
                    iTotalDisplayRecords = _ObjMasterValue .Count,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        // Get Single Master value and return in object for edit
        public ActionResult Manage()
        {
            MasterValueModels _ObjMaster = new MasterValueModels();
            int _MasterId = 0;
            try
            {
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.MenuPath == "editSessionMasterID('" + (int)Session["RefMasterId"] + "')").FirstOrDefault().MenuIcon;
                ViewBag.MasterType = (CommanClass.MasterList)(int)Session["RefMasterId"] + " Setup";

                if (Session["MasterId"] != null && Session != null)
                    _MasterId = (int)Session["MasterId"];

                if (_MasterId != 0)
                {
                    _ObjMaster = db.sp_MasterValue_Select(_MasterId, null, null).Select(x => new MasterValueModels()
                    {
                        Id = x.ID,
                        RefMasterId = x.RefMasterId,
                        RefVendorId = Convert.ToInt32(x.RefVendorId),
                        ValueName = x.ValueName,
                        ValueDesc = x.ValueDesc,
                        OrdNo = Convert.ToDecimal(x.OrdNo),
                        IsActive = x.IsActive,
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal
                    }).FirstOrDefault();
                }
                Session["MasterId"] = null;
                return View(_ObjMaster);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        // Save Master Data
        [HttpPost]
        public ActionResult Save(MasterValueModels _ObjParam)
        {
            try
            {
                bool Result = false;
                if (_ObjParam != null)
                {
                    Result = db.sp_MasterValue_Save(_ObjParam.Id, (int)Session["RefMasterId"], (int)Session["VendorId"], _ObjParam.ValueName, _ObjParam.ValueDesc,
                                _ObjParam.OrdNo, _ObjParam.IsActive, CommanClass._User, CommanClass._Terminal).FirstOrDefault().HasValue;

                    if (Result)
                    {
                        if (_ObjParam.Id == 0)
                            TempData["Success"] = "Inserted Successfully!";
                        else
                            TempData["Success"] = "Updated Successfully!";
                    }
                    else
                        TempData["Error"] = "Internal Server Error. In MasterValue";
                }
                else
                    return new HttpStatusCodeResult(400, "Master value object is NUll!");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        // To check value already exists or not in master value
        public JsonResult isValueExists(string pValueName)
        {
            int _iCounte = 0;
            bool _Result = true;
            try
            {
                _iCounte = db.sp_MasterValue_SelectWhere(" and Upper(ValueName) = '" + pValueName.ToUpper() + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;
                if (_iCounte > 0)
                    _Result = true;
                else
                    _Result = false;

                return Json(_Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult updateStatus(int Id,bool CurrentStatus)
        {
            bool Result = false;
            try
            {
                MasterValue _ObjMasterValue = db.MasterValues.Find(Id);
                _ObjMasterValue.IsActive = !CurrentStatus;
                _ObjMasterValue.UpdUser = CommanClass._User;
                _ObjMasterValue.UpdTerminal = CommanClass._Terminal;
                db.SaveChanges();
                Result = true;

                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int Id)
        {
            try
            {
                MasterValue _ObjMasterValue = db.MasterValues.Find(Id);
                if(_ObjMasterValue == null)
                    return Json(new { _result = false, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);

                db.MasterValues.Remove(_ObjMasterValue);
                db.SaveChanges();

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error.Please try again later." }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}