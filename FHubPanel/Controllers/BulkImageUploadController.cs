using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;

namespace FHubPanel.Controllers
{
    public class BulkImageUploadController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        static List<ImageUplodError> _ObjUpError = new List<ImageUplodError>();
        static int _UploadCounter = 0;
        static string _ImgType = "";
        //static int ProdHeight = Convert.ToInt32(ConfigurationManager.AppSettings["ProdHeight"].ToString());
        //static int ProdWidth = Convert.ToInt32(ConfigurationManager.AppSettings["ProdWidth"].ToString());
        //static int CataHegiht = Convert.ToInt32(ConfigurationManager.AppSettings["CataHeight"].ToString());
        //static int CataWidth = Convert.ToInt32(ConfigurationManager.AppSettings["CataWidth"].ToString());
        //
        // GET: /BulkImageUpload/
        public ActionResult Index()
        {
            if (_ImgType == "Catalogs")
                ViewBag.ImgType = "Catalogues";
            else
                ViewBag.ImgType = "Products";

            ViewBag.MasterType = "Bulk " + ViewBag.ImgType + " Image Upload";
            return View();
        }

        public JsonResult BulkImgUploadType(string Type)
        {
            try
            {
                _ImgType = Type;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<PartialViewResult> Save(string FNPattern, string FNSeparater, string ImageUploadType)
        {
            try
            {
                if (ImageUploadType.ToUpper() == "CATALOGUES")
                {
                    #region "Bulk Catalog Image Upload"

                    //string FNPattern = FileNamePattern, FNSeparater = Separater;
                    CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    string _ServerOriginalPath = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Original/");
                    string _ServerThumbnailPath = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Thumbnail/");
                    string _FName = "", _Extension = "", _OriginalName = "";
                    foreach (string _FileName in Request.Files)
                    {
                        if (_FileName != null && _FileName != "")
                        {
                            HttpPostedFileBase _File = Request.Files[_FileName];
                            _FName = _File.FileName;
                            _Extension = _File.ContentType.Split('/')[1];
                            _OriginalName = _FName.Substring(0, _FName.LastIndexOf('.'));
                            if (_Extension.ToUpper() == "JPG" || _Extension.ToUpper() == "JPEG" || _Extension.ToUpper() == "PNG")
                            {
                                if (Directory.Exists(_ServerOriginalPath))
                                {
                                    if (!System.IO.File.Exists(Path.Combine(_ServerOriginalPath, _FName)))
                                    {
                                        if (_OriginalName != null && _OriginalName != "")
                                            ImageWithOnlyCatalogCode(_OriginalName, _ServerOriginalPath, _ServerThumbnailPath, _FName, _File);
                                    }
                                    else
                                        ErrorManage(_FName, "Image already exists on server. If you want to overwrite then go on particular Catalogue!");
                                }
                                else
                                    ErrorManage(_FName, "Server Directory Not found.Invalid User!");
                            }
                            else
                                ErrorManage(_FName, "Wrong File Upload. File Must be in Jpg/Jpeg/Png format!");
                        }
                        else
                            ErrorManage(_FName, "File Not Found!");
                    }
                    #endregion
                }
                else
                {
                    #region "Bulk Products Image Upload"

                    //string FNPattern = FileNamePattern, FNSeparater = Separater;
                    CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    string _ServerOriginalPath = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Products/Original/");
                    string _ServerThumbnailPath = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Products/Thumbnail/");
                    string _FName = "", _Extension = "", _OriginalName = "";
                    int? _Ord = 0;
                    int _Parseint = 0;
                    foreach (string _FileName in Request.Files)
                    {
                        if (_FileName != null && _FileName != "")
                        {
                            HttpPostedFileBase _File = Request.Files[_FileName];
                            _FName = _File.FileName;
                            _Extension = _File.ContentType.Split('/')[1];
                            _OriginalName = _FName.Substring(0, _FName.LastIndexOf('.'));
                            if (_Extension.ToUpper() == "JPG" || _Extension.ToUpper() == "JPEG" || _Extension.ToUpper() == "PNG")
                            {
                                if (Directory.Exists(_ServerOriginalPath))
                                {
                                    if (!System.IO.File.Exists(Path.Combine(_ServerOriginalPath, _FName)))
                                    {
                                        if (FNSeparater == "None")
                                        {
                                            if (int.TryParse(_OriginalName, out _Parseint))
                                                _Ord = _Parseint;
                                            if (_OriginalName != null && _OriginalName != "")
                                                ImageWithOnlyProdCode(_OriginalName, _ServerOriginalPath, _ServerThumbnailPath, _FName, _Ord, _File);
                                        }
                                        else
                                        {
                                            string[] _SeparateStr = _OriginalName.Split(Convert.ToChar(FNSeparater));
                                            if (int.TryParse(_SeparateStr[_SeparateStr.ToList().Count - 1], out _Parseint))
                                                _Ord = _Parseint;

                                            if (_SeparateStr.ToList().Count > 0)
                                            {
                                                if (FNPattern == "P")
                                                    ImageWithOnlyProdCode(_SeparateStr[0], _ServerOriginalPath, _ServerThumbnailPath, _FName, _Ord, _File);
                                                else if (FNPattern == "PC")
                                                    ImagesWithProdAndCatCode(_SeparateStr, _ServerOriginalPath, _ServerThumbnailPath, _FName, 1, 0, _Ord, _File);
                                                else if (FNPattern == "CP")
                                                    ImagesWithProdAndCatCode(_SeparateStr, _ServerOriginalPath, _ServerThumbnailPath, _FName, 0, 1, _Ord, _File);
                                            }
                                        }
                                    }
                                    else
                                        ErrorManage(_FName, "Image already exists on server. If you want to overwrite then go on particular product!");
                                }
                                else
                                    ErrorManage(_FName, "Server Directory Not found.Invalid User!");
                            }
                            else
                                ErrorManage(_FName, "Wrong File Upload. File Must be in Jpg/Jpeg/Png format!");
                        }
                        else
                            ErrorManage(_FName, "File Not Found!");
                    }

                    #endregion
                }

                //TempData["Success"] = _UploadCounter;
                return PartialView("DisplayErrorListPartial", _ObjUpError);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ErrorManage(string _FName, string _Msg)
        {
            try
            {
                ImageUplodError _ObjErr = new ImageUplodError();
                _ObjErr.FileName = _FName;
                _ObjErr.Reason = _Msg;
                _ObjUpError.Add(_ObjErr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ImagesWithProdAndCatCode(string[] _SeparateStr, string _ServerOriginalPath, string _ServerThumbnailPath, string _FName, int CatPos, int ProdPos, int? _Ord, HttpPostedFileBase _File)
        {
            try
            {
                int _CatCount = 0, _ProdCount = 0;
                int? _ProdImgId = 0, _ProdId = 0, _CatId = 0;
                if (_SeparateStr.ToList().Count > 1)
                {
                    _CatCount = db.sp_CatalogMas_SelectWhere(" and CatCode = '" + _SeparateStr[CatPos] + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;
                    if (_CatCount == 1)
                    {
                        _CatId = db.sp_CatalogMas_SelectWhere(" and CatCode = '" + _SeparateStr[CatPos] + "' and RefVendorId = " + (int)Session["VendorId"]).FirstOrDefault().CatId;
                        _ProdCount = db.sp_ProductMas_SelectWhere(" and ProdCode = '" + _SeparateStr[ProdPos] + "' and RefCatId = " + _CatId + " and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;
                        if (_ProdCount == 1)
                        {
                            _ProdId = db.sp_ProductMas_SelectWhere(" and ProdCode = '" + _SeparateStr[ProdPos] + "' and RefCatId = " + _CatId + " and RefVendorId = " + (int)Session["VendorId"]).FirstOrDefault().ProdId;

                            if (_ProdId != null && _ProdId != 0)
                            {
                                if (_CatId != null && _CatId != 0)
                                {
                                    _FName = _FName.Substring(0, _FName.LastIndexOf('.')) + "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff") + _FName.Substring(_FName.LastIndexOf('.'), _FName.Length - (_FName.LastIndexOf('.')));
                                    _File.SaveAs(_ServerOriginalPath + _FName);

                                    System.Drawing.Image _Img = System.Drawing.Image.FromFile(_ServerOriginalPath + _FName);
                                    //System.Drawing.Image _ThumbImg = _Img.GetThumbnailImage(100, 150, null, IntPtr.Zero);
                                    //_ThumbImg.Save(_ServerThumbnailPath + _FName);
                                    //_Img.Dispose();

                                    var thumbnailBit = new Bitmap(ProdWidth, ProdHeight);
                                    var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                                    thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                                    thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                    var imageRectangle = new Rectangle(0, 0, ProdWidth, ProdHeight);
                                    thumbnailGraph.DrawImage(_Img, imageRectangle);
                                    thumbnailBit.Save(_ServerThumbnailPath + _FName, _Img.RawFormat);
                                    _Img.Dispose();

                                    _ProdImgId = db.sp_ProductImgDet_Save(0, _ProdId, _FName, _Ord, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;

                                    if (_ProdImgId == 0)
                                    {
                                        System.IO.File.Delete(_ServerOriginalPath + _FName);
                                        System.IO.File.Delete(_ServerThumbnailPath + _FName);
                                        ErrorManage(_FName, "Server Error. Product upload fail!");
                                    }
                                    else
                                        _UploadCounter++;
                                }
                                else
                                    ErrorManage(_FName, "Catalogue code is wrong!");
                            }
                            else
                                ErrorManage(_FName, "Product code is wrong!");
                        }
                        else
                            ErrorManage(_FName, "Product code is wrong!");
                    }
                    else
                        ErrorManage(_FName, "Catalogue code is wrong!");
                }
                else
                    ErrorManage(_FName, "Selected pattern not available in image name!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ImageWithOnlyProdCode(string _ProdCode, string _ServerOriginalPath, string _ServerThumbnailPath, string _FName, int? _Ord, HttpPostedFileBase _File)
        {
            try
            {
                int? _ProdImgId = 0, _ProdId = 0;
                if (db.sp_ProductMas_SelectWhere(" and ProdCode = '" + _ProdCode + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count == 1)
                {
                    _ProdId = db.sp_ProductMas_SelectWhere(" and ProdCode = '" + _ProdCode + "' and RefVendorId = " + (int)Session["VendorId"]).FirstOrDefault().ProdId;

                    if (_ProdId != null && _ProdId != 0)
                    {
                        _FName = _FName.Substring(0, _FName.LastIndexOf('.')) + "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff") + _FName.Substring(_FName.LastIndexOf('.'), _FName.Length - (_FName.LastIndexOf('.')));
                        _File.SaveAs(_ServerOriginalPath + _FName);

                        System.Drawing.Image _Img = System.Drawing.Image.FromFile(_ServerOriginalPath + _FName);
                        //System.Drawing.Image _ThumbImg = _Img.GetThumbnailImage(100, 150, null, IntPtr.Zero);
                        //_ThumbImg.Save(_ServerThumbnailPath + _FName);
                        //_Img.Dispose();

                        var thumbnailBit = new Bitmap(ProdWidth, ProdHeight);
                        var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        var imageRectangle = new Rectangle(0, 0, ProdWidth, ProdHeight);
                        thumbnailGraph.DrawImage(_Img, imageRectangle);
                        thumbnailBit.Save(_ServerThumbnailPath + _FName, _Img.RawFormat);
                        _Img.Dispose();

                        _ProdImgId = db.sp_ProductImgDet_Save(0, _ProdId, _FName, _Ord, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;

                        if (_ProdImgId == 0)
                        {
                            System.IO.File.Delete(_ServerOriginalPath + _FName);
                            System.IO.File.Delete(_ServerThumbnailPath + _FName);
                            ErrorManage(_FName, "Server Error. Product upload fail!");
                        }
                        else
                            _UploadCounter++;
                    }
                    else
                        ErrorManage(_FName, "Product code is wrong!");
                }
                else
                    ErrorManage(_FName, "Product code is wrong!");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ImageWithOnlyCatalogCode(string _CatCode, string _ServerOriginalPath, string _ServerThumbnailPath, string _FName, HttpPostedFileBase _File)
        {
            try
            {
                CatalogModel _ObjCat = new CatalogModel();
                int? _CatImgId = 0;
                if (db.sp_CatalogMas_SelectWhere(" and CatCode = '" + _CatCode + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count == 1)
                {
                    _ObjCat = db.sp_CatalogMas_SelectWhere(" and CatCode = '" + _CatCode + "' and RefVendorId = " + (int)Session["VendorId"]).Select(x => new CatalogModel()
                    {
                        CatId = x.CatId,
                        CatCode = x.CatCode,
                        CatName = x.CatName,
                        CatDescription = x.CatDescription,
                        CatLaunchDate = x.CatLaunchDate,
                        IsFullset = Convert.ToBoolean(x.IsFullset),
                        IsActive = Convert.ToBoolean(x.IsActive),
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal

                    }).FirstOrDefault();

                    if (_ObjCat.CatId != 0)
                    {
                        //_FName += "-" + System.DateTime.Now.ToString("hhmmssfff");
                        string _FileExtention = _FName.Substring(_FName.LastIndexOf('.'), _FName.Length - (_FName.LastIndexOf('.')));
                        _FName = _CatCode + "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff") + _FileExtention;
                        _File.SaveAs(_ServerOriginalPath + _FName);

                        System.Drawing.Image _Img = System.Drawing.Image.FromFile(_ServerOriginalPath + _FName);
                        //System.Drawing.Image _ThumbImg = _Img.GetThumbnailImage(100, 150, null, IntPtr.Zero);
                        //_ThumbImg.Save(_ServerThumbnailPath + _FName);
                        //_Img.Dispose();

                        var thumbnailBit = new Bitmap(CataWidth, CataHegiht);
                        var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        var imageRectangle = new Rectangle(0, 0, CataWidth, CataHegiht);
                        thumbnailGraph.DrawImage(_Img, imageRectangle);
                        thumbnailBit.Save(_ServerThumbnailPath + _FName, _Img.RawFormat);
                        _Img.Dispose();

                        _CatImgId = db.sp_CatalogMas_Save(_ObjCat.CatId, (int)Session["VendorId"], _ObjCat.CatCode, _ObjCat.CatName,
                            _ObjCat.CatDescription, _FName, _ObjCat.CatLaunchDate, _ObjCat.IsFullset, _ObjCat.IsActive,
                            (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;

                        if (_CatImgId == 0)
                        {
                            System.IO.File.Delete(_ServerOriginalPath + _FName);
                            System.IO.File.Delete(_ServerThumbnailPath + _FName);
                            ErrorManage(_FName, "Server Error. Catalogue upload fail!");
                        }
                        else
                            _UploadCounter++;
                    }
                    else
                        ErrorManage(_FName, "Catalogue code is wrong!");
                }
                else
                    ErrorManage(_FName, "Catalogue code is wrong!");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public JsonResult ClearErrorList()
        {
            try
            {
                int _TempCnt = 0;
                _ObjUpError = new List<ImageUplodError>();
                _TempCnt = _UploadCounter;
                _UploadCounter = 0;
                return Json(_TempCnt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

#region "PC Pattern Img Upload"
//if (_SeparateStr.ToList().Count > 1)
//{
//    _CatId = db.sp_CatalogMas_SelectWhere(" and CatCode = '" + _SeparateStr[1] + "' and RefVendorId = " + CommanClass._VendorId).FirstOrDefault().CatId;

//    _ProdId = db.sp_ProductMas_SelectWhere(" and ProdCode = '" + _SeparateStr[0] + "' and RefCatId = " + _CatId + " and RefVendorId = " + CommanClass._VendorId).FirstOrDefault().ProdId;

//    if (_ProdId != null && _ProdId != 0)
//    {
//        if (_CatId != null && _CatId != 0)
//        {
//            _File.SaveAs(_ServerOriginalPath + _FName);

//            System.Drawing.Image _Img = System.Drawing.Image.FromFile(_ServerOriginalPath + _FName);
//            System.Drawing.Image _ThumbImg = _Img.GetThumbnailImage(100, 150, null, IntPtr.Zero);
//            _ThumbImg.Save(_ServerThumbnailPath + _FName);

//            _ProdImgId = db.sp_ProductImgDet_Save(0, _ProdId, _FName, _Ord, CommanClass._VendorId, CommanClass._Terminal).FirstOrDefault().Value;

//            if (_ProdImgId == 0)
//            {
//                System.IO.File.Delete(_ServerOriginalPath + _FName);
//                System.IO.File.Delete(_ServerThumbnailPath + _FName);
//                ErrorManage(_FName, "Server Error. Product upload fail!");
//            }
//            else
//            {
//                _UploadCounter++;
//            }
//        }
//        else
//        {
//            ErrorManage(_FName, "Catalog code is wrong!");
//        }

//    }
//    else
//    {
//        ErrorManage(_FName, "Product code is wrong!");
//    }
//}
#endregion