using FHubPanel.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHubPanel.Controllers
{
    public class ProductController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        ProductModel _ObjProd = new ProductModel();
        int _SuccessUploadCnt = 0, _FailUploadCnt = 0, _RecordCnt = 0;
        //static int ProdHeight = Convert.ToInt32(ConfigurationManager.AppSettings["ProdHeight"].ToString());
        //static int ProdWidth = Convert.ToInt32(ConfigurationManager.AppSettings["ProdWidth"].ToString());
        //
        // GET: /Catalog/

        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Product" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            @ViewBag.MasterType = "Product";
            return View();
        }

        public JsonResult AjaxHandler(string Search, int? pageIndex, int? pageSize)
        {
            try
            {

                CompanyProfile _ObjCP = db.CompanyProfiles.FirstOrDefault();

                List<ProductModel> _ObjProdLIst = new List<ProductModel>();
                _ObjProdLIst = db.sp_ProductMas_SelectWhereForAdmin((int)Session["VendorId"], Search, pageSize, pageIndex).Select(x => new ProductModel()
                {
                    ProdId = x.ProdId,
                    ProdCode = x.ProdCode,
                    ProdName = x.ProdName,
                    ProdDescription = x.ProdDescription,
                    RefVendorId = x.RefVendorId,
                    RefProdCategory = x.RefProdCategory,
                    RefProdType = x.RefProdType,
                    RefBrand = x.RefBrand,
                    RefColor = x.RefColor,
                    RefDesign = x.RefDesign,
                    RefFabric = x.RefFabric,
                    RefSize = x.RefSize,
                    RefCatId = x.RefCatId,
                    Celebrity = x.Celebrity,
                    ProdImgPath = x.ProdImgPath,
                    FullImgPath = x.ThumbnailImgPath + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                }).ToList();

                return Json(new
                {
                    Result = "OK",
                    data = _ObjProdLIst,
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
                Session["ProdId"] = Id;
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
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Product" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                @ViewBag.MasterType = "Product";

                CommanClass._VendorId = (int)Session["VendorId"];

                ViewData["CatalogList"] = CommanClass.GetCatalogList();
                ViewData["CategoryList"] = CommanClass.GetProductCategoryList();
                ViewData["ColorList"] = CommanClass.GetColorList();
                ViewData["FabricList"] = CommanClass.GetMasterList((int)CommanClass.MasterList.Fabric);
                ViewData["SizeList"] = CommanClass.GetMasterList((int)CommanClass.MasterList.Size);
                ViewData["BrandList"] = CommanClass.GetMasterList((int)CommanClass.MasterList.Brand);
                ViewData["DesignList"] = CommanClass.GetMasterList((int)CommanClass.MasterList.Design);
                ViewData["ProdTypeList"] = CommanClass.GetMasterList((int)CommanClass.MasterList.ProdType);

                int ProdId = 0;
                if (Session["ProdId"] != null && Session != null)
                    ProdId = (int)Session["ProdId"];

                if (ProdId != 0)
                {
                    CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    _ObjComp.FolderPath = _ObjComp.FolderPath + (int)Session["VendorId"] + "/Products/Original/";
                    _ObjProd = db.sp_ProductMas_SelectForAdmin((int)Session["VendorId"], ProdId).Select(x => new ProductModel()
                    {
                        ProdId = x.ProdId,
                        ProdName = x.ProdName,
                        RefVendorId = x.RefVendorId,
                        ProdCode = x.ProdCode,
                        ProdDescription = x.ProdDescription,
                        RefProdCategory = x.RefProdCategory,
                        RefBrand = x.RefBrand,
                        RefCatId = x.RefCatId,
                        RefSize = x.RefSize,
                        RefSizeList = x.RefSize,
                        RefColor = x.RefColor,
                        RefColorList = x.RefColor,
                        RefDesign = x.RefDesign,
                        RefFabric = x.RefFabric,
                        RefProdType = x.RefProdType,
                        ActivetillDate = x.ActivetillDate,
                        Celebrity = x.Celebrity,
                        RetailPrice = x.RetailPrice,
                        WholeSalePrice = x.WholeSalePrice,
                        ProdImgPath = x.ProdImgPath,
                        FullImgPath = x.ProdImgPath != null && x.ProdImgPath != "" ? x.OriginalImgPath + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff") : @"\Content\dist\img\ProductNoImage.png",
                        IsActive = Convert.ToBoolean(x.IsActive),
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal,
                        UpdUser = x.UpdUser,
                        UpdDate = x.UpdDate,
                        UpdTerminal = x.UpdTerminal
                    }).FirstOrDefault();


                }

                Session["ProdId"] = null;

                return View(_ObjProd);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Server Error.Please try again later!");
            }
        }

        public PartialViewResult GetProductImgDet(int ProdId)
        {
            try
            {
                List<sp_ProductImgDet_SelectForAPI_Result> _ObjProdImgList = db.sp_ProductImgDet_SelectForAPI(ProdId, "PANEL").ToList();

                return PartialView("ProductImageListPartial", _ObjProdImgList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Save(ProductModel _ObjParam, string RefColor, HttpPostedFileBase fileLogo)
        {
            try
            {
                int? _NProduct = db.sp_VendorSubDet_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " Order by ValidToDate Desc").FirstOrDefault().NoOfProducts;
                if (db.sp_ProductMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"]).ToList().Count >= _NProduct)
                {
                    TempData["Warning"] = "Product Upload limit is over of your pack!";
                    return RedirectToAction("Index");
                }
                if (fileLogo != null)
                {
                    string _Ext = fileLogo.ContentType.Split('/')[1];
                    if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                    {
                        Session["ProdId"] = _ObjParam.ProdId;
                        TempData["Warning"] = "Select proper image file!";
                        return RedirectToAction("Manage");
                    }
                }

                int Result = db.sp_ProductMas_Save(_ObjParam.ProdId, _ObjParam.ProdName, (int)Session["VendorId"], _ObjParam.RefCatId, _ObjParam.ProdCode,
                    _ObjParam.ProdDescription, _ObjParam.RefProdCategory, _ObjParam.RefColor, _ObjParam.RefProdType, _ObjParam.RefSize,
                    _ObjParam.RefFabric, _ObjParam.RefDesign, _ObjParam.RefBrand, _ObjParam.Celebrity, null,
                    _ObjParam.ActivetillDate, _ObjParam.IsActive, _ObjParam.RetailPrice, _ObjParam.WholeSalePrice,
                    CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                if (Result != 0)
                {
                    if (fileLogo != null)
                    {
                        CompanyProfile _Objcomp = db.CompanyProfiles.FirstOrDefault();
                        string _Path = Server.MapPath(_Objcomp.FolderPath + (int)Session["VendorId"] + "/Products/");
                        if (Directory.Exists(_Path))
                        {
                            if (_ObjParam.ChangeImgId > 0)
                            {
                                ProductImgDet _ObjProductImg = db.ProductImgDets.Find(_ObjParam.ChangeImgId);
                                if (System.IO.File.Exists(_Path + "Original/" + _ObjProductImg.ImgName))
                                {
                                    System.IO.File.Delete(_Path + "Original/" + _ObjProductImg.ImgName);
                                    System.IO.File.Delete(_Path + "Thumbnail/" + _ObjProductImg.ImgName);
                                }
                            }

                            string _FileName = fileLogo.FileName.Substring(0, fileLogo.FileName.LastIndexOf('.'));
                            if (_FileName.Length > 15)
                                _FileName = _FileName.Substring(0, 15);

                            _FileName += "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff");
                            string _Extension = fileLogo.ContentType.Split('/')[1];
                            fileLogo.SaveAs(_Path + "Original/" + _FileName + "." + _Extension);

                            System.Drawing.Image _Img = System.Drawing.Image.FromFile(_Path + "Original/" + _FileName + "." + _Extension);
                            //System.Drawing.Image ThumbImg = _Img.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                            //ThumbImg.Save(_Path + "Thumbnail/" + Result + "." + _Extension);
                            //_Img.Dispose();

                            var thumbnailBit = new Bitmap(ProdWidth, ProdHeight);
                            var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            var imageRectangle = new Rectangle(0, 0, ProdWidth, ProdHeight);
                            thumbnailGraph.DrawImage(_Img, imageRectangle);
                            thumbnailBit.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension, _Img.RawFormat);
                            _Img.Dispose();

                            int _ImgResult = db.sp_ProductImgDet_Save(_ObjParam.ChangeImgId, Result, _FileName + "." + _Extension, 0, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;
                            if (_ImgResult <= 0)
                            {
                                TempData["Warning"] = "Product Image upload fail.";
                            }
                        }
                    }

                    if (_ObjParam.ProdId == 0)
                    {
                        TempData["Success"] = "Product Successfully created.";
                    }
                    else
                    {
                        TempData["Success"] = "Product Successfully update.";
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

        //Bulk Product Upload
        public ActionResult BulkProductData(HttpPostedFileBase ExcelFile)
        {
            try
            {
                if (ExcelFile != null)
                {
                    string _Extention = ExcelFile.ContentType.Split('/')[1];
                    if (_Extention != "vnd.openxmlformats-officedocument.spreadsheetml.sheet" && _Extention != "vnd.ms-excel")
                        throw new Exception("Please select proper file.");

                    //CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    //string _ServerPath = Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"]);
                    //if (Directory.Exists(_ServerPath))
                    //    ExcelFile.SaveAs(_ServerPath + "/" + ExcelFile.FileName);
                    //else
                    //    throw new Exception("Invalid User.");

                    //FilePath = _ServerPath + "/" + ExcelFile.FileName;
                    BulkSave(ExcelFile);
                }

                //if (System.IO.File.Exists(FilePath))
                //    System.IO.File.Delete(FilePath);

                TempData["Success"] = "Total Catalogue " + _RecordCnt + " and Success fully uploded " + _SuccessUploadCnt + " and Fail to upload " + _FailUploadCnt;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //if (System.IO.File.Exists(FilePath))
                //    System.IO.File.Delete(FilePath);
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
                //throw ex;
            }
        }

        public DataTable ExcelToDatatable(HttpPostedFileBase ExcelFile)
        {
            DataTable _Dt = new DataTable();
            int _exRowCount = 0;
            bool _ThrowEx = false;
            try
            {
                if ((ExcelFile != null) && (ExcelFile.ContentLength > 0) && !string.IsNullOrEmpty(ExcelFile.FileName))
                {
                    DateTime _date;
                    decimal _retprice, _wholesaleprice;
                    string fileName = ExcelFile.FileName;
                    string fileContentType = ExcelFile.ContentType;
                    byte[] fileBytes = new byte[ExcelFile.ContentLength];
                    var data = ExcelFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(ExcelFile.ContentLength));
                    using (var package = new ExcelPackage(ExcelFile.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        int? _NProduct = db.sp_VendorSubDet_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " Order by ValidToDate Desc").FirstOrDefault().NoOfProducts;
                        int _TotalProdCount = db.sp_ProductMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"]).ToList().Count + (noOfRow - 1);
                        if (_TotalProdCount >= _NProduct)
                        {
                            _ThrowEx = true;
                            throw new Exception("No of Products are graterthan to upload limit of your pack!");
                        }

                        _exRowCount = noOfRow;
                        if (noOfCol != 15)
                        {
                            _ThrowEx = true;
                            throw new Exception("Invalid columns available!");
                        }

                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            if (rowIterator == 1)
                            {
                                _Dt.Columns.Add("CatId", typeof(int));
                                _Dt.Columns.Add("CatCode", typeof(string));
                                _Dt.Columns.Add("ProdCode", typeof(string));
                                _Dt.Columns.Add("ProdName", typeof(string));
                                _Dt.Columns.Add("ProdDescription", typeof(string));
                                _Dt.Columns.Add("ProdCategory", typeof(string));
                                _Dt.Columns.Add("Color", typeof(string));
                                _Dt.Columns.Add("ProdType", typeof(string));
                                _Dt.Columns.Add("Size", typeof(string));
                                _Dt.Columns.Add("Fabric", typeof(string));
                                _Dt.Columns.Add("Design", typeof(string));
                                _Dt.Columns.Add("Brand", typeof(string));
                                _Dt.Columns.Add("Celebrity", typeof(string));
                                _Dt.Columns.Add("ActivetillDate", typeof(DateTime));
                                _Dt.Columns.Add("RetailPrice", typeof(decimal));
                                _Dt.Columns.Add("WholeSalePrice", typeof(decimal));

                            }
                            else
                            {
                                _Dt.Rows.Add();

                                //Cat code
                                if (workSheet.Cells[rowIterator, 1].Value != null || !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 1].Value.ToString()))
                                {
                                    if (db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and CatCode = '" + workSheet.Cells[rowIterator, 1].Value.ToString() + "'").ToList().Count != 1)
                                    {
                                        _ThrowEx = true;
                                        throw new Exception("Wrong Catalogue code - " + workSheet.Cells[rowIterator, 1].Value.ToString() + " at Line No. " + (_RecordCnt + 2));
                                    }
                                    else
                                        _Dt.Rows[_Dt.Rows.Count - 1][0] = db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and CatCode = '" + workSheet.Cells[rowIterator, 1].Value.ToString() + "'").FirstOrDefault().CatId;
                                    _Dt.Rows[_Dt.Rows.Count - 1][1] = workSheet.Cells[rowIterator, 1].Value.ToString();
                                }
                                else
                                {
                                    _Dt.Rows[_Dt.Rows.Count - 1][0] = null;
                                    _Dt.Rows[_Dt.Rows.Count - 1][1] = null;
                                }

                                //Prod code
                                if (workSheet.Cells[rowIterator, 2].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 2].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Product Code at Line No. " + (_RecordCnt + 2));
                                }
                                else if (workSheet.Cells[rowIterator, 2].Value.ToString().IndexOf('-') != -1 || workSheet.Cells[rowIterator, 2].Value.ToString().IndexOf('_') != -1)
                                {
                                    _ThrowEx = true;
                                    throw new Exception("In Product Code '-' and '_' not allowed. Invalid format at Line No. " + (_RecordCnt + 2));
                                }
                                else if (_Dt.Select(" ProdCode = '" + workSheet.Cells[rowIterator, 2].Value.ToString() + "'").ToList().Count > 0)
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Product code " + _Dt.Rows[_Dt.Rows.Count - 1][2].ToString() + " is already exists in excel file at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][2] = workSheet.Cells[rowIterator, 2].Value.ToString();

                                //ProdName
                                if (workSheet.Cells[rowIterator, 3].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 3].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Product Name at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][3] = workSheet.Cells[rowIterator, 3].Value.ToString();

                                //Product Desc
                                if (workSheet.Cells[rowIterator, 4].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 4].Value.ToString()))
                                    _Dt.Rows[_Dt.Rows.Count - 1][4] = null;
                                else
                                    _Dt.Rows[_Dt.Rows.Count - 1][4] = workSheet.Cells[rowIterator, 4].Value.ToString();

                                //Product Category
                                if (workSheet.Cells[rowIterator, 5].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 5].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Product Category at Line No. " + (_RecordCnt + 2));
                                }
                                else if (db.sp_ProductCategory_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and ProdCategoryName = '" + workSheet.Cells[rowIterator, 5].Value.ToString() + "'").ToList().Count == 0)
                                {
                                    _ThrowEx = true;
                                    throw new Exception(workSheet.Cells[rowIterator, 5].Value.ToString() + " Not available in Product Category at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][5] = workSheet.Cells[rowIterator, 5].Value.ToString();

                                //Color
                                if (workSheet.Cells[rowIterator, 6].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 6].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Color at Line No. " + (_RecordCnt + 2));
                                }
                                string[] _Colors = workSheet.Cells[rowIterator, 6].Value.ToString().Split(',');
                                string _Colorlist = "";
                                foreach (var _Color in _Colors)
                                {
                                    if (_Colorlist != "")
                                        _Colorlist += ",";
                                    _Colorlist += "'" + _Color + "'";
                                }
                                if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.Color + " and ValueName In (" + _Colorlist + ")").ToList().Count != _Colors.Length)
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Color name of masterlist is mismatch with this " + workSheet.Cells[rowIterator, 6].Value.ToString() + " list at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][6] = workSheet.Cells[rowIterator, 6].Value.ToString();

                                //Prod Type
                                if (workSheet.Cells[rowIterator, 7].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 7].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Product Type at Line No. " + (_RecordCnt + 2));
                                }
                                else if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.ProdType + " and ValueName = '" + workSheet.Cells[rowIterator, 7].Value.ToString() + "'").ToList().Count != 1)
                                {
                                    _ThrowEx = true;
                                    throw new Exception(workSheet.Cells[rowIterator, 7].Value.ToString() + " is not available in product type at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][7] = workSheet.Cells[rowIterator, 7].Value.ToString();

                                //Size
                                if (workSheet.Cells[rowIterator, 8].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 8].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Size at Line No. " + (_RecordCnt + 2));
                                }
                                string[] _Sizes = workSheet.Cells[rowIterator, 8].Value.ToString().Split(',');
                                string _Sizelist = "";
                                foreach (var _Size in _Sizes)
                                {
                                    if (_Sizelist != "")
                                        _Sizelist += ",";
                                    _Sizelist += "'" + _Size + "'";
                                }
                                if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.Size + " and ValueName In (" + _Sizelist + ")").ToList().Count != _Sizes.Length)
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Size of masterlist is mismatch with this " + workSheet.Cells[rowIterator, 8].Value.ToString() + " list at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][8] = workSheet.Cells[rowIterator, 8].Value.ToString();

                                //Fabric
                                if (workSheet.Cells[rowIterator, 9].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 9].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Fabric at Line No. " + (_RecordCnt + 2));
                                }
                                else if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.Fabric + " and ValueName = '" + workSheet.Cells[rowIterator, 9].Value.ToString() + "'").ToList().Count != 1)
                                {
                                    _ThrowEx = true;
                                    throw new Exception(workSheet.Cells[rowIterator, 9].Value.ToString() + " is not available in Fabric master at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][9] = workSheet.Cells[rowIterator, 9].Value.ToString();

                                //Design
                                if (workSheet.Cells[rowIterator, 10].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 10].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Design at Line No. " + (_RecordCnt + 2));
                                }
                                else if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.Design + " and ValueName = '" + workSheet.Cells[rowIterator, 10].Value.ToString() + "'").ToList().Count != 1)
                                {
                                    _ThrowEx = true;
                                    throw new Exception(workSheet.Cells[rowIterator, 10].Value.ToString() + " is not available in Design master at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][10] = workSheet.Cells[rowIterator, 10].Value.ToString();

                                //Brand
                                if (workSheet.Cells[rowIterator, 11].Value != null)
                                {
                                    if (db.sp_MasterValue_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefMasterId = " + (int)CommanClass.MasterList.Brand + " and ValueName = '" + workSheet.Cells[rowIterator, 11].Value.ToString() + "'").ToList().Count != 1)
                                    {
                                        _ThrowEx = true;
                                        throw new Exception(workSheet.Cells[rowIterator, 11].Value.ToString() + " is not available in Brand master at Line No. " + (_RecordCnt + 2));
                                    }
                                    _Dt.Rows[_Dt.Rows.Count - 1][11] = workSheet.Cells[rowIterator, 11].Value.ToString();
                                }
                                else
                                    _Dt.Rows[_Dt.Rows.Count - 1][11] = null;

                                //Celibrity
                                if (workSheet.Cells[rowIterator, 12].Value != null)
                                    _Dt.Rows[_Dt.Rows.Count - 1][12] = workSheet.Cells[rowIterator, 12].Value.ToString();
                                else
                                    _Dt.Rows[_Dt.Rows.Count - 1][12] = null;

                                //Active till date
                                if (workSheet.Cells[rowIterator, 13].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 13].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Active till Date at Line No. " + (_RecordCnt + 2));
                                }
                                else if (!DateTime.TryParse(workSheet.Cells[rowIterator, 13].Value.ToString(), out _date))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Active till Date not in correct format at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][13] = Convert.ToDateTime(workSheet.Cells[rowIterator, 13].Value);

                                //Retail Price
                                if (workSheet.Cells[rowIterator, 14].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 14].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter Retail Price at Line No. " + (_RecordCnt + 2));
                                }
                                else if (!decimal.TryParse(workSheet.Cells[rowIterator, 14].Value.ToString(), out _retprice))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Retail Price not in correct format at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][14] = Convert.ToDecimal(workSheet.Cells[rowIterator, 14].Value.ToString());

                                //Wholesale price
                                if (workSheet.Cells[rowIterator, 15].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 15].Value.ToString()))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Enter WholeSale Price at Line No. " + (_RecordCnt + 2));
                                }
                                else if (!decimal.TryParse(workSheet.Cells[rowIterator, 15].Value.ToString(), out _wholesaleprice))
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Whole Sale Price not in correct format at Line No. " + (_RecordCnt + 2));
                                }
                                _Dt.Rows[_Dt.Rows.Count - 1][15] = workSheet.Cells[rowIterator, 15].Value.ToString();

                                if (db.sp_ProductMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and ProdCode = '" + _Dt.Rows[_Dt.Rows.Count - 1][2].ToString() + "'").ToList().Count > 0)
                                {
                                    _ThrowEx = true;
                                    throw new Exception("Product code " + _Dt.Rows[_Dt.Rows.Count - 1][2].ToString() + " is already exists in table at Line No. " + (_RecordCnt + 2));
                                }

                                _RecordCnt++;
                            }
                        }
                    }
                }

                return _Dt;
            }
            catch (Exception ex)
            {
                if (_ThrowEx)
                    throw ex;
                else
                    throw new Exception(ex.Message + ". Please remove row at last of excel sheet. Reader found " + _exRowCount + " row in sheet!");
            }
        }

        public void BulkSave(HttpPostedFileBase ExcelFile)
        {
            try
            {
                bool _rsltval = false;
                DataTable _Dt = ExcelToDatatable(ExcelFile);

                foreach (DataRow _Dr in _Dt.Rows)
                {
                    _rsltval = db.sp_ProductMas_Save(0, _Dr["ProdName"].ToString(), (int)Session["VendorId"], Convert.ToInt32(_Dr["CatId"]),
                                _Dr["ProdCode"].ToString(), _Dr["ProdDescription"].ToString(),
                                _Dr["ProdCategory"].ToString(), _Dr["Color"].ToString(), _Dr["ProdType"].ToString(),
                                _Dr["Size"].ToString(), _Dr["Fabric"].ToString(), _Dr["Design"].ToString(),
                                _Dr["Brand"].ToString(), _Dr["Celebrity"].ToString(), null,
                                Convert.ToDateTime(_Dr["ActivetillDate"]), true, Convert.ToDecimal(_Dr["RetailPrice"]),
                                Convert.ToDecimal(_Dr["WholeSalePrice"]), CommanClass._User, CommanClass._Terminal).FirstOrDefault().HasValue;

                    if (_rsltval)
                        _SuccessUploadCnt++;
                    else
                        _FailUploadCnt++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileResult FileFormatDownload()
        {
            try
            {
                byte[] _FileByte = System.IO.File.ReadAllBytes(Server.MapPath("/Content/ProductBulkUpload.xlsx"));
                return File(_FileByte, System.Net.Mime.MediaTypeNames.Application.Octet, "ProductBulkUpload.xlsx");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete product category 
        public JsonResult Delete(int Id)
        {
            try
            {
                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                ProductMa _Obj = db.ProductMas.Find(Id);
                

                if (_Obj == null)
                    return Json(new { _result = true, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);

                foreach (var _ImgName in db.sp_ProductImgDet_SelectForAPI(Id, "PANEL").Select(x => x.ImgName).ToList())
                {
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Products/Original/" + _ImgName));
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Products/Thumbnail/" + _ImgName));
                }
                if (db.sp_ProductImgDet_Delete(Id).FirstOrDefault().Value == 1)
                {
                    db.ProductMas.Remove(_Obj);
                    db.SaveChanges();

                    CatalogMa _ObjCatalogue = db.CatalogMas.Find(_Obj.RefCatId);
                    _ObjCatalogue.UpdUser = (int)Session["VendorId"];
                    _ObjCatalogue.UpdDate = System.DateTime.Today;
                    _ObjCatalogue.UpdTerminal = CommanClass._Terminal;
                    db.SaveChanges();

                    db.sp_DeleteLog_Save((int)Session["VendorId"], "Product", _Obj.ProdId, CommanClass._Terminal);
                }
                else
                    return Json(new { _result = false, _Message = "You can not delete this product!" }, JsonRequestBehavior.AllowGet);

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error. Please try again later!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Check Duplicate value
        public JsonResult isValueExists(string pProdCode)
        {
            try
            {
                int RCounter = db.sp_ProductMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and ProdCode = '" + pProdCode + "'").ToList().Count;
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

        public PartialViewResult DeleteProdImg(int ProdImgId)
        {
            try
            {
                ProductImgDet _ObjProdImgDet = db.ProductImgDets.Find(ProdImgId);
                int ProdId = _ObjProdImgDet.RefProdId;

                CompanyProfile _Objcomp = db.CompanyProfiles.FirstOrDefault();

                string _Path = Server.MapPath(_Objcomp.FolderPath + (int)Session["VendorId"] + "/Products/");

                if (System.IO.File.Exists(_Path + "Original/" + _ObjProdImgDet.ImgName))
                {
                    System.IO.File.Delete(_Path + "Original/" + _ObjProdImgDet.ImgName);
                    System.IO.File.Delete(_Path + "Thumbnail/" + _ObjProdImgDet.ImgName);
                }

                db.ProductImgDets.Remove(_ObjProdImgDet);
                db.SaveChanges();

                return PartialView("ProductImageListPartial", db.sp_ProductImgDet_SelectForAPI(ProdId, "PANEL").ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}