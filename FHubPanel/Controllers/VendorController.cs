using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Configuration;

namespace FHubPanel.Controllers
{
    public class VendorController : BaseController
    {
        //Entity Model object
        FHubDBEntities db = new FHubDBEntities();
        // Vendor Model object
        VendorModel _ObjVendor = new VendorModel();
        string _RegMsg = "";
        //static int VendorHeight = Convert.ToInt32(ConfigurationManager.AppSettings["VendorHeight"].ToString());
        //static int VendorWidth = Convert.ToInt32(ConfigurationManager.AppSettings["VendorWidth"].ToString());
        //
        // GET: /Vendor/

        public VendorController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public VendorController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Vendor" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Vendor";

            return View();
        }

        //Get Data from Vendor
        public JsonResult AjaxHandler()
        {
            List<sp_Vendor_SelectWhere_Result> _ObjVendorList;
            try
            {
                _ObjVendorList = db.sp_Vendor_SelectWhere("").ToList();
                var result = from a in _ObjVendorList
                             select new[]{
                                 a.VendorName,
                                 a.Address + " " + a.Landmark + " " + a.State + " " + a.City + " "+ a.Country + " " + a.Pincode ,
                                 a.ContactName, a.MobileNo1,a.EmailId,Convert.ToString(a.IsActive), Convert.ToString(a.VendorId)
                             };

                return Json(new
                {
                    sEcho = "",
                    iTotalRecords = _ObjVendorList.Count,
                    iTotalDisplayRecords = _ObjVendorList.Count,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Manage Vandor Code for Edit
        public JsonResult EditSession(int Id)
        {
            try
            {
                Session["VendorId"] = Id;
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult VendorProfile()
        {
            try
            {
                if ((int)Session["VendorId"] != 0 && CommanClass._Role == "Vendor")
                {
                    Session["VendorId"] = (int)Session["VendorId"];
                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ResetVendorCode()
        {
            try
            {
                string _VendorCode = db.sp_Vendor_ResetCode().FirstOrDefault();
                if (!string.IsNullOrEmpty(_VendorCode))
                    return Json(new { Result = true, VendorCode = _VendorCode }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Result = false, VendorCode = _VendorCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Manage Vendor Detail
        public ActionResult Manage()
        {
            try
            {
                CompanyProfile _ObjCompProf = db.CompanyProfiles.FirstOrDefault();
                if (CommanClass._Role != "Vendor")
                    ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Vendor" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                else
                    ViewBag.Icon = "fa fa-user";
                ViewBag.MasterType = "Vendor Profile";
                ViewBag.Role = CommanClass._Role;
                if (CommanClass._Role == "Admin")
                    ViewBag.RedirectLink = "/Vendor/Index";
                else
                    ViewBag.RedirectLink = "/Home/Index";

                int _VendorId = 0;

                if (Session["VendorId"] != null && Session != null)
                    _VendorId = (int)Session["VendorId"];

                if (_VendorId != 0)
                {
                    _ObjVendor = db.sp_Vendor_Select(_VendorId).Select(x => new VendorModel()
                    {
                        VendorId = x.VendorId,
                        VendorName = x.VendorName,
                        UserName = x.UserName,
                        VendorCode = x.VendorCode,
                        Address = x.Address,
                        Landmark = x.Landmark,
                        Country = x.Country,
                        State = x.State,
                        City = x.City,
                        Pincode = x.Pincode,
                        ContactName = x.ContactName,
                        ContactNo1 = x.ContactNo1,
                        ContactNo2 = x.ContactNo2,
                        MobileNo1 = x.MobileNo1,
                        MobileNo2 = x.MobileNo2,
                        FaxNo = x.FaxNo,
                        EmailId = x.EmailId,
                        WebSite = x.WebSite,
                        LogoImg = x.LogoImg,
                        LogoFullPath = x.LogoImg == "" || x.LogoImg == null ? "/Content/dist/img/no_image_available.jpg" : _ObjCompProf.FolderPath + Convert.ToString(x.VendorId) + "/" + x.LogoImg + "?" + DateTime.Now.ToString("ddMMyyyyHHmmssfff"),
                        IsActive = Convert.ToBoolean(x.IsActive),
                        AboutUs = x.AboutUs,
                        ProdDispName = x.ProdDispName,
                        CatDispName = x.CatDispName,
                        BGImage = x.BGImage,
                        BGImageFullPath = x.ThumbnailBGImgPath == null ? "/Content/dist/img/no_image_available.jpg" : x.ThumbnailBGImgPath,
                        InsUser = x.InsUser,
                        InsTerminal = x.InsTerminal,
                        InsDate = x.InsDate,
                    }).FirstOrDefault();
                }

                if (CommanClass._Role != "Vendor")
                {
                    Session["VendorId"] = null;
                }


                return View(_ObjVendor);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        //Save Vendor
        public async Task<ActionResult> Save(VendorModel _ObjParam, HttpPostedFileBase fileLogo, HttpPostedFileBase fileBGImg)
        {
            try
            {
                bool _registerstatus = true;
                if (ModelState.IsValid)
                {
                    if (fileLogo != null)
                    {
                        string _Ext = fileLogo.ContentType.Split('/')[1];
                        if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                        {
                            Session["PCId"] = _ObjParam.VendorId;
                            TempData["Warning"] = "Select proper image file!";
                            return RedirectToAction("Manage");
                        }
                    }

                    if (fileBGImg != null)
                    {
                        string _Ext = fileBGImg.ContentType.Split('/')[1];
                        if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                        {
                            //Session["PCId"] = _ObjParam.VendorId;
                            TempData["Warning"] = "Select proper image file!";
                            return RedirectToAction("Manage");
                        }
                    }

                    if (_ObjParam != null)
                    {

                        if (_ObjParam.VendorId == 0)
                        {
                            var user = new ApplicationUser() { UserName = _ObjParam.UserName };
                            var result = await UserManager.CreateAsync(user, _ObjParam.Password);
                            if (result.Succeeded)
                                _registerstatus = true;
                            else
                                _registerstatus = false;
                        }

                        if (_registerstatus)
                        {
                            //SignInAsync(user, isPersistent: false);
                            int Result = db.sp_Vendor_Save(_ObjParam.VendorId, _ObjParam.VendorName, _ObjParam.UserName, _ObjParam.VendorCode, _ObjParam.Address, _ObjParam.Landmark, _ObjParam.Country,
                                    _ObjParam.State, _ObjParam.City, _ObjParam.Pincode, _ObjParam.ContactName, _ObjParam.ContactNo1, _ObjParam.ContactNo2,
                                    _ObjParam.MobileNo1, _ObjParam.MobileNo2, _ObjParam.FaxNo, _ObjParam.EmailId, _ObjParam.WebSite, _ObjParam.LogoImg, _ObjParam.IsActive,
                                    _ObjParam.SubscriptionType, _ObjParam.ReferenceCode, _ObjParam.AboutUs, _ObjParam.ProdDispName, _ObjParam.CatDispName,
                                    _ObjParam.BGImage, CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                            if (Result != 0)
                            {
                                Session["VendorName"] = _ObjParam.VendorName;
                                Session["VendorCode"] = _ObjParam.VendorCode;

                                CompanyProfile _ObjCompProf = db.CompanyProfiles.FirstOrDefault();
                                string _Path = Server.MapPath(_ObjCompProf.FolderPath + Result);
                                if (_ObjParam.VendorId == 0)
                                {
                                    if (!Directory.Exists(_Path))
                                    {
                                        Directory.CreateDirectory(_Path);
                                        Directory.CreateDirectory(_Path + "/Catalog");
                                        Directory.CreateDirectory(_Path + "/Catalog/Original");
                                        Directory.CreateDirectory(_Path + "/Catalog/Thumbnail");
                                        Directory.CreateDirectory(_Path + "/Category");
                                        Directory.CreateDirectory(_Path + "/Category/Original");
                                        Directory.CreateDirectory(_Path + "/Category/Thumbnail");
                                        Directory.CreateDirectory(_Path + "/Products");
                                        Directory.CreateDirectory(_Path + "/Products/Original");
                                        Directory.CreateDirectory(_Path + "/Products/Thumbnail");
                                        Directory.CreateDirectory(_Path + "/Slider");
                                        Directory.CreateDirectory(_Path + "/Slider/Original");
                                        Directory.CreateDirectory(_Path + "/Slider/Thumbnail");
                                    }
                                    //string _Extention = fileLogo.FileName.Split('.')[1];
                                    //string _FullPath = Path.Combine(_Path, Result + "." + _Extention);
                                    //fileLogo.SaveAs(_FullPath);

                                    //Vendor _ObjVendorUpd = db.Vendors.Find(Result);
                                    //_ObjVendorUpd.LogoImg = Result + "/" + Result + "." + _Extention;
                                    //db.SaveChanges();

                                    TempData["Success"] = "Vendor Register Successfully.";
                                }
                                else
                                {
                                    //string _Extention = fileLogo.FileName.Split('.')[1];
                                    //string _FullPath = Path.Combine(_Path, Result + "." + _Extention);
                                    //fileLogo.SaveAs(_FullPath);
                                    TempData["Success"] = "Vendor Profile Updated.";
                                }

                                if (fileLogo != null)
                                {
                                    string _Extention = fileLogo.FileName.Split('.')[1];
                                    string _FileName = Result + "-" + System.DateTime.Now.ToString("hhmmssfff");
                                    string _FullPath = Path.Combine(_Path, _FileName + "." + _Extention);
                                    fileLogo.SaveAs(_FullPath);

                                    System.Drawing.Image _Image = System.Drawing.Image.FromFile(_FullPath);

                                    var thumbnailbit = new Bitmap(VendorWidth, VendorHeight);
                                    var thumbnailgraphic = Graphics.FromImage(thumbnailbit);
                                    thumbnailgraphic.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbnailgraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    thumbnailgraphic.SmoothingMode = SmoothingMode.HighQuality;

                                    var imageRectangle = new Rectangle(0, 0, VendorWidth, VendorHeight);
                                    thumbnailgraphic.DrawImage(_Image, imageRectangle);
                                    thumbnailbit.Save(Path.Combine(_Path, "Thumb" + _FileName + "." + _Extention));
                                    _Image.Dispose();

                                    Vendor _ObjVendorUpd = await db.Vendors.FindAsync(Result);
                                    _ObjVendorUpd.LogoImg = _FileName + "." + _Extention;
                                    db.SaveChanges();

                                    Session["VendorImg"] = _ObjCompProf.FolderPath + Result + "/Thumb" + _FileName + "." + _Extention;
                                }
                                if (fileBGImg != null)
                                {
                                    string _BGExtention = fileBGImg.FileName.Split('.')[1];
                                    string _BGFileName = "BG-" + Result + "-" + System.DateTime.Now.ToString("hhmmssfff");
                                    string _BGFullPath = Path.Combine(_Path, _BGFileName + "." + _BGExtention);
                                    fileBGImg.SaveAs(_BGFullPath);

                                    System.Drawing.Image _Image = System.Drawing.Image.FromFile(_BGFullPath);

                                    // BG Image For Product
                                    var thumbnailbit = new Bitmap(ProdWidth, ProdHeight);
                                    var thumbnailgraphic = Graphics.FromImage(thumbnailbit);
                                    thumbnailgraphic.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbnailgraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    thumbnailgraphic.SmoothingMode = SmoothingMode.HighQuality;

                                    var imageRectangle = new Rectangle(0, 0, ProdWidth, ProdHeight);
                                    thumbnailgraphic.DrawImage(_Image, imageRectangle);
                                    thumbnailbit.Save(Path.Combine(_Path, "ThumbProd" + _BGFileName + "." + _BGExtention));

                                    // BG Image For Catalogue
                                    thumbnailbit = new Bitmap(CataWidth, CataHegiht);
                                    thumbnailgraphic = Graphics.FromImage(thumbnailbit);
                                    thumbnailgraphic.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbnailgraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    thumbnailgraphic.SmoothingMode = SmoothingMode.HighQuality;

                                    imageRectangle = new Rectangle(0, 0, CataWidth, CataHegiht);
                                    thumbnailgraphic.DrawImage(_Image, imageRectangle);
                                    thumbnailbit.Save(Path.Combine(_Path, "ThumbCata" + _BGFileName + "." + _BGExtention));

                                    _Image.Dispose();

                                    Vendor _ObjVendorUpd = await db.Vendors.FindAsync(Result);
                                    if (_ObjVendorUpd.BGImage != null)
                                        System.IO.File.Delete(_Path + "/" + _ObjVendorUpd.BGImage);

                                    _ObjVendorUpd.BGImage = _BGFileName + "." + _BGExtention;
                                    db.SaveChanges();


                                }

                            }
                            else
                                TempData["Error"] = "Server Error. Please try again later!";
                        }
                        else
                        {
                            TempData["Error"] = _RegMsg;
                        }
                    }
                }

                if (CommanClass._Role == "Vendor" && CommanClass._Role != "")
                    return RedirectToAction("Index", "Home");
                else if (CommanClass._Role == "Admin" && CommanClass._Role != "")
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Server Error. Please try again later!";
                return RedirectToAction("Index");
            }
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //Update Vendor Status
        public JsonResult UpdateStatus(int Id, bool CurrentStatus)
        {
            try
            {
                Vendor _ObjV = db.Vendors.Find(Id);
                if (_ObjV == null)
                    return Json(false, JsonRequestBehavior.AllowGet);

                _ObjV.IsActive = !CurrentStatus;
                _ObjV.UpdUser = CommanClass._User;
                _ObjV.UpdTerminal = CommanClass._Terminal;
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //Delete vendor 
        public JsonResult Delete(int Id)
        {
            try
            {
                Vendor _Obj = db.Vendors.Find(Id);
                if (_Obj == null)
                    return Json(new { _result = true, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);
                db.Vendors.Remove(_Obj);
                db.SaveChanges();

                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                if (Directory.Exists(Server.MapPath(_ObjComp.FolderPath + Id)))
                    Directory.Delete(Server.MapPath(_ObjComp.FolderPath + Id));

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error. Please try again later!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Check Duplicate value
        public JsonResult isValueExists(string pUserName)
        {
            try
            {
                int RCounter = db.sp_Vendor_Select(null).Where(x => x.UserName.ToUpper() == pUserName.ToUpper()).ToList().Count;
                if (RCounter > 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

    }
}