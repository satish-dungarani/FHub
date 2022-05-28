using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FHubPanel.Controllers
{
    public class ProductCategoryController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        ProductCategoryModel _ObjPC = new ProductCategoryModel();
        //
        // GET: /ProductCategory/
        public ActionResult Index()
        {

            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "ProductCategory" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            @ViewBag.MasterType = "Product Category";
            return View();
        }

        public JsonResult AjaxHandler(string Search, int? pageIndex, int? pageSize)
        {
            try
            {

                CompanyProfile _ObjCP = db.CompanyProfiles.FirstOrDefault();

                List<ProductCategoryModel> _ObjPCLIst = new List<ProductCategoryModel>();
                _ObjPCLIst = db.sp_ProductCategory_LazyLoad((int)Session["VendorId"], Search, pageSize, pageIndex).Select(x => new ProductCategoryModel()
                {
                    PCId = x.PCId,
                    ProdCategoryName = x.ProdCategoryName,
                    ProdCategoryDesc = x.ProdCategoryDesc,
                    RefPCId = x.RefPCId,
                    Ord = x.Ord,
                    ProdCategoryImg = x.ProdCategoryImg,
                    ImgFullPath = x.ThumbnailImgPath
                }).ToList();

                string PCIdlist = "";
                for (int i = 0; i < _ObjPCLIst.Count; i++)
                {
                    if (i == 0)
                        PCIdlist = Convert.ToString(_ObjPCLIst[i].PCId);
                    else
                        PCIdlist += "," + _ObjPCLIst[i].PCId;
                }

                List<sp_ProductCategory_SelectWhere_Result> _ObjPSClist = new List<sp_ProductCategory_SelectWhere_Result>();
                if (PCIdlist != "")
                {
                    _ObjPSClist = db.sp_ProductCategory_SelectWhere(" and RefPCId In (" + PCIdlist + ")").Select(x => new sp_ProductCategory_SelectWhere_Result()
                    {
                        PCId = x.PCId,
                        ProdCategoryName = x.ProdCategoryName,
                        ProdCategoryDesc = x.ProdCategoryDesc,
                        RefPCId = x.RefPCId,
                        Ord = x.Ord,
                        ProdCategoryImg = x.ProdCategoryImg,
                        ThumbnailImgPath = x.ThumbnailImgPath
                    }).ToList();
                }


                return Json(new
                {
                    Result = "OK",
                    data = _ObjPCLIst,
                    subdata = _ObjPSClist,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "ERROR",
                    data = "",
                    subdata = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditSession(int Id)
        {
            try
            {
                Session["PCId"] = Id;
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
                CommanClass._VendorId = (int)Session["VendorId"];
                ViewData["CategoryList"] = CommanClass.GetProductCategoryList("MAIN");

                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "ProductCategory" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                @ViewBag.MasterType = "Product Category";
                int PCId = 0;
                if (Session["PCId"] != null && Session != null)
                    PCId = (int)Session["PCId"];

                if (PCId != 0)
                {
                    CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    _ObjComp.FolderPath = _ObjComp.FolderPath + (int)Session["VendorId"] + "/Category/Original/";
                    _ObjPC = db.sp_ProductCategory_Select(PCId).Select(x => new ProductCategoryModel()
                    {
                        PCId = x.PCId,
                        RefVendorId = x.RefVendorId,
                        ProdCategoryName = x.ProdCategoryName,
                        ProdCategoryDesc = x.ProdCategoryDesc,
                        RefPCId = x.RefPCId,
                        Ord = x.Ord,
                        ProdCategoryImg = x.ProdCategoryImg,
                        ImgFullPath = x.ProdCategoryImg != "" && x.ProdCategoryImg != null ? _ObjComp.FolderPath + x.ProdCategoryImg + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff") : @"\Content\dist\img\CategoryNoImage.png",
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal,
                        UpdUser = x.UpdUser,
                        UpdDate = x.UpdDate,
                        UpdTerminal = x.UpdTerminal
                    }).FirstOrDefault();
                }

                Session["PCId"] = null;

                return View(_ObjPC);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Server Error.Please try again later!");
            }
        }

        public ActionResult Save(ProductCategoryModel _ObjParam, HttpPostedFileBase fileLogo)
        {
            try
            {
                if (fileLogo != null)
                {
                    string _Ext = fileLogo.ContentType.Split('/')[1];
                    if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                    {
                        Session["PCId"] = _ObjParam.PCId;
                        TempData["Warning"] = "Select proper image file!";
                        return RedirectToAction("Manage");
                    }
                }

                int Result = db.sp_ProductCategory_Save(_ObjParam.PCId, (int)Session["VendorId"], _ObjParam.ProdCategoryName,
                    _ObjParam.ProdCategoryDesc, _ObjParam.RefPCId, _ObjParam.Ord, _ObjParam.ProdCategoryImg, CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                if (Result != 0)
                {
                    if (_ObjParam.PCId == 0)
                    {
                        TempData["Success"] = "Product Category Successfully created.";
                    }
                    else
                    {
                        TempData["Success"] = "Product Category Successfully update.";
                    }

                    if (fileLogo != null)
                    {
                        CompanyProfile _Objcomp = db.CompanyProfiles.FirstOrDefault();
                        string _Path = Server.MapPath(_Objcomp.FolderPath + (int)Session["VendorId"] + "/Category/");

                        if (Directory.Exists(_Path))
                        {
                            string _FileName = fileLogo.FileName.Substring(0, fileLogo.FileName.LastIndexOf('.')).Trim();
                            if (_FileName.Length > 15)
                                _FileName = _FileName.Substring(0, 15).Trim();

                            _FileName += "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff");
                            string _Extension = fileLogo.ContentType.Split('/')[1];
                            string _ExistsFileName = db.sp_ProductCategory_Select(Result).FirstOrDefault().ProdCategoryImg;
                            int _Exists = db.sp_ProductCategory_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and ProdCategoryImg = '" + _FileName + "." + _Extension + "'").ToList().Count;
                            if (_Exists > 0 && _ExistsFileName != fileLogo.FileName)
                            {
                                TempData["Error"] = "Image Name already allocated to another category!";
                                return RedirectToAction("Index");
                            }

                           
                            fileLogo.SaveAs(_Path + "Original/" + _FileName + "." + _Extension);

                            System.Drawing.Image _Img = System.Drawing.Image.FromFile(_Path + "Original/" + _FileName + "." + _Extension);
                            //System.Drawing.Image ThumbImg = _Img.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                            //ThumbImg.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension);
                            //_Img.Dispose();

                            //var thumbnailBit = new Bitmap(FixedSize(_Img, _Img.Width/2, _Img.Height/2));
                            //thumbnailBit.Save(Path.Combine(_Path + "Thumbnail\\", fileLogo.FileName));
                            //_Img.Dispose();

                            var thumbnailBit = new Bitmap(CategoryWidth, CategoryHeight);
                            var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            thumbnailGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            //thumbnailGraph.CompositingMode = CompositingMode.SourceCopy;

                            var imageRectangle = new Rectangle(0, 0, CategoryWidth, CategoryHeight);
                            thumbnailGraph.DrawImage(_Img, imageRectangle);
                            thumbnailBit.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension, _Img.RawFormat);
                            _Img.Dispose();

                            ProductCategory _ObjPCat = db.ProductCategories.Find(Result);
                            _ObjPCat.ProdCategoryImg = _FileName + "." + _Extension;
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


        //private Image FixedSize(Image imgPhoto, int Width, int Height)
        //{
        //    int sourceWidth = imgPhoto.Width;
        //    int sourceHeight = imgPhoto.Height;
        //    int sourceX = 0;
        //    int sourceY = 0;
        //    int destX = 0;
        //    int destY = 0;

        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;

        //    nPercentW = ((float)Width / (float)sourceWidth);
        //    nPercentH = ((float)Height / (float)sourceHeight);
        //    if (nPercentH < nPercentW)
        //    {
        //        nPercent = nPercentH;
        //        destX = System.Convert.ToInt16((Width -
        //                      (sourceWidth * nPercent)) / 2);
        //    }
        //    else
        //    {
        //        nPercent = nPercentW;
        //        destY = System.Convert.ToInt16((Height -
        //                      (sourceHeight * nPercent)) / 2);
        //    }

        //    int destWidth = (int)(sourceWidth * nPercent);
        //    int destHeight = (int)(sourceHeight * nPercent);

        //    Bitmap bmPhoto = new Bitmap(Width, Height,
        //                      PixelFormat.Format24bppRgb);
        //    bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
        //                     imgPhoto.VerticalResolution);

        //    Graphics grPhoto = Graphics.FromImage(bmPhoto);
        //    grPhoto.Clear(Color.Red);
        //    grPhoto.InterpolationMode =
        //            InterpolationMode.HighQualityBicubic;

        //    grPhoto.DrawImage(imgPhoto,
        //        new Rectangle(destX, destY, destWidth, destHeight),
        //        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        //        GraphicsUnit.Pixel);

        //    grPhoto.Dispose();
        //    return bmPhoto;
        //}

        //Delete product category 
        public JsonResult Delete(int Id)
        {
            try
            {
                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                ProductCategory _Obj = db.ProductCategories.Find(Id);

                if (_Obj == null)
                    return Json(new { _result = true, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);

                if (db.sp_ProductMas_SelectWhere(" and RefProdCategory = '" + _Obj.ProdCategoryName + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count > 0)
                    return Json(new { _result = false, _Message = "Product Category already in used. You can't delete this category!" }, JsonRequestBehavior.AllowGet);

                if (db.sp_ProductCategory_SelectWhere(" and RefPCId = " + Id).ToList().Count > 0)
                    return Json(new { _result = false, _Message = "Product Category have subcategory. Then you can't delete this category!" }, JsonRequestBehavior.AllowGet);

                if (_Obj.ProdCategoryImg != null)
                {
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Category/Original/" + _Obj.ProdCategoryImg));
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Category/Thumbnail/" + _Obj.ProdCategoryImg));
                }

                db.ProductCategories.Remove(_Obj);
                db.SaveChanges();

                db.sp_DeleteLog_Save((int)Session["VendorId"], "Category", _Obj.PCId, CommanClass._Terminal);

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error. Please try again later!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Check Duplicate value
        public JsonResult isValueExists(string pProductCategoryName)
        {
            try
            {
                int RCounter = db.sp_ProductCategory_SelectWhere(" and ProdCategoryName = '" + pProductCategoryName + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;
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