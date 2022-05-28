using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;

namespace FHubPanel.Controllers
{
    public class VendorSliderController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        VendorSliderModel _ObjSlider = new VendorSliderModel();
        //static int SliderHeight = Convert.ToInt32(ConfigurationManager.AppSettings["SliderHeight"].ToString());
        //static int SliderWidth = Convert.ToInt32(ConfigurationManager.AppSettings["SliderWidth"].ToString());
        //
        // GET: /VendorSlider/
        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "VendorSlider" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            @ViewBag.MasterType = "Banner";
            return View();
        }

        public JsonResult AjaxHandler(string Search, int? pageIndex, int? pageSize)
        {
            try
            {

                CompanyProfile _ObjCP = db.CompanyProfiles.FirstOrDefault();

                List<VendorSliderModel> _ObjSliderLIst = new List<VendorSliderModel>();
                _ObjSliderLIst = db.sp_VendorSlider_SelectWhere((int)Session["VendorId"], Search, pageSize, pageIndex).Select(x => new VendorSliderModel()
                {
                    SliderId = x.SliderId,
                    RefVendorId = x.RefVendorId,
                    SliderTitle = x.SliderTitle,
                    SliderUrl = x.SliderUrl,
                    SDate = x.StartDate.ToShortDateString(),
                    EDate = x.EndDate.ToShortDateString(),
                    SliderImg = x.SliderImg,
                    FullImgPath = x.SliderImg != null && x.SliderImg != "" ? _ObjCP.FolderPath + (int)Session["VendorId"] + "/Slider/Thumbnail/" + x.SliderImg + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff") : @"\Content\dist\img\no_image_available.jpg",
                    Expired = x.EndDate.AddDays(1).CompareTo(System.DateTime.Today) < 1 ? "Expired" : ""
                }).ToList();

                return Json(new
                {
                    Result = "OK",
                    data = _ObjSliderLIst,
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

        public JsonResult EditSession(int Id)
        {
            try
            {
                Session["SliderId"] = Id;
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Manage()
        {
            try
            {
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "VendorSlider" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                @ViewBag.MasterType = "Slider";

                CommanClass._VendorId = (int)Session["VendorId"];
                ViewData["CategoryList"] = CommanClass.GetProductCategoryList();

                int SliderId = 0;
                if (Session["SliderId"] != null && Session != null)
                    SliderId = (int)Session["SliderId"];

                if (SliderId != 0)
                {
                    CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    _ObjComp.FolderPath = _ObjComp.FolderPath + (int)Session["VendorId"] + "/Slider/Original/";
                    _ObjSlider = db.sp_VendorSlider_Select(SliderId).Select(x => new VendorSliderModel()
                    {
                        SliderId = x.SliderId,
                        RefVendorId = x.RefVendorId,
                        SliderTitle = x.SliderTitle,
                        SliderImg = x.SliderImg,
                        SliderUrl = x.SliderUrl,
                        Ord = x.Ord,
                        DisplayPage = x.DisplayPage,
                        Category = x.Category,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        FullImgPath = x.SliderImg != null && x.SliderImg != "" ? _ObjComp.FolderPath + x.SliderImg + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff") : @"\Content\dist\img\no_image_available.jpg",
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal,
                        UpdUser = x.UpdUser,
                        UpdDate = x.UpdDate,
                        UpdTerminal = x.UpdTerminal
                    }).FirstOrDefault();
                }

                Session["SliderId"] = null;

                return View(_ObjSlider);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Server Error.Please try again later!");
            }
        }

        public ActionResult Save(VendorSliderModel _ObjParam, string Category, HttpPostedFileBase fileLogo)
        {
            try
            {
                int? _NSlider = db.sp_VendorSubDet_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " Order By ValidToDate Desc").FirstOrDefault().NoOfSlider;
                if (db.sp_VendorSlider_SelectWhere((int)Session["VendorId"], "", _NSlider, 0).ToList().Count >= _NSlider)
                {
                    TempData["Warning"] = "Banner limit is over. For insert more banner increase your business pack!";
                    return RedirectToAction("Index");
                }

                if (fileLogo != null)
                {
                    string _Ext = fileLogo.ContentType.Split('/')[1];
                    if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                    {
                        Session["SliderId"] = _ObjParam.SliderId;
                        TempData["Warning"] = "Select proper image file!";
                        return RedirectToAction("Manage");
                    }
                }


                int Result = db.sp_VendorSlider_Save(_ObjParam.SliderId, (int)Session["VendorId"], _ObjParam.SliderTitle, _ObjParam.SliderImg,
                    _ObjParam.SliderUrl, _ObjParam.Ord, _ObjParam.DisplayPage, _ObjParam.Category, _ObjParam.StartDate,
                    _ObjParam.EndDate, CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                if (Result != 0)
                {
                    if (_ObjParam.SliderId == 0)
                    {
                        TempData["Success"] = "Slider Successfully created.";
                    }
                    else
                    {
                        TempData["Success"] = "Slider Successfully update.";
                    }

                    if (fileLogo != null)
                    {
                        CompanyProfile _Objcomp = db.CompanyProfiles.FirstOrDefault();
                        string _Path = Server.MapPath(_Objcomp.FolderPath + (int)Session["VendorId"] + "/Slider/");
                        if (Directory.Exists(_Path))
                        {
                            if (System.IO.File.Exists(_Path + "/Original/" + _ObjParam.SliderImg))
                            {
                                System.IO.File.Delete(_Path + "/Original/" + _ObjParam.SliderImg);
                                System.IO.File.Delete(_Path + "/Thumbnail/" + _ObjParam.SliderImg);
                            }
                            string _FileName = fileLogo.FileName.Substring(0, fileLogo.FileName.LastIndexOf('.'));
                            if (_FileName.Length > 15)
                                _FileName = _FileName.Substring(0, 15);

                            _FileName += "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff");
                            string _Extension = fileLogo.ContentType.Split('/')[1];
                            fileLogo.SaveAs(_Path + "Original/" + _FileName + "." + _Extension);

                            System.Drawing.Image _Img = System.Drawing.Image.FromFile(_Path + "Original/" + _FileName + "." + _Extension);
                            //System.Drawing.Image ThumbImg = _Img.GetThumbnailImage(300, 150, null, IntPtr.Zero);
                            //ThumbImg.Save(_Path + "Thumbnail/" + Result + "." + _Extension);
                            //_Img.Dispose();

                            var thumbnailBit = new Bitmap(SliderWidth, SliderHeight);
                            var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            var imageRectangle = new Rectangle(0, 0, SliderWidth, SliderHeight);
                            thumbnailGraph.DrawImage(_Img, imageRectangle);
                            thumbnailBit.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension, _Img.RawFormat);
                            _Img.Dispose();

                            VendorSlider _ObjProduct = db.VendorSliders.Find(Result);
                            _ObjProduct.SliderImg = _FileName + "." + _Extension;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    TempData["Success"] = "Server Error. Please try again later!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Session["PCId"] = _ObjParam.PCId;
                TempData["Error"] = "Server Error. Please try again later!";
                return RedirectToAction("Index");
            }
        }


        //Delete product category 
        public JsonResult Delete(int Id)
        {
            try
            {
                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                VendorSlider _Obj = db.VendorSliders.Find(Id);

                if (_Obj == null)
                    return Json(new { _result = true, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);
                string _Path = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Slider/");
                if (System.IO.File.Exists(_Path + "/Original/" + _Obj.SliderImg))
                {
                    System.IO.File.Delete(_Path + "/Original/" + _Obj.SliderImg);
                    System.IO.File.Delete(_Path + "/Thumbnail/" + _Obj.SliderImg);
                }

                db.VendorSliders.Remove(_Obj);
                db.SaveChanges();

                System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Slider/Original/" + _Obj.SliderImg));
                System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Slider/Thumbnail/" + _Obj.SliderImg));

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error. Please try again later!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}