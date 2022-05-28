using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace FHubPanel.Controllers
{
    public class ProductMergeController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        string _Filter = "";
        //
        // GET: /ProductMerge/
        public ActionResult Index()
        {
            try
            {
                ViewBag.MasterType = "Connect Store";
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "ProductMerge" && x.ActionName == "Index").FirstOrDefault().MenuIcon;

                ViewData["VendorList"] = CommanClass.GetVendorList((int)Session["VendorId"]);
                _Filter = " and RefVendorId = 0";
                ViewData["CatalogueList"] = CommanClass.GetCatalogList(_Filter);

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetCatalogues(int VendorId)
        {
            try
            {
                _Filter = " and RefVendorId = " + VendorId + " and CatId Not In (Select a.RefCatId From CatalogMas a Where a.RefVendorId = " + (int)Session["VendorId"] + " and a.RefCatId Is Not NULL)";
                ViewData["CatalogueList"] = CommanClass.GetCatalogList(_Filter);
                return PartialView("CatalogueListPartial");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult CheckDuplication(string _Val, string _Field)
        {
            try
            {
                bool _Result = false;
                string _defstr = "", _Msg = "";

                if (_Field == "Product Name")
                    _defstr = " and ProdName = '" + _Val + "' and RefVendorId = " + (int)Session["VendorId"];
                else
                    _defstr = " and ProdCode = '" + _Val + "' and RefVendorId = " + (int)Session["VendorId"];

                _Result = db.sp_ProductMas_SelectWhere(_defstr).ToList().Count == 0 ? true : false;

                if (!_Result)
                    _Msg = _Field + " already exist.";

                return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult CheckCatalogueDuplication(string _Val, string _Field)
        {
            try
            {
                bool _Result = false;
                string _defstr = "", _Msg = "";

                if (_Field == "Catalogue Name")
                    _defstr = " and CatName = '" + _Val + "' and RefVendorId = " + (int)Session["VendorId"];
                else
                    _defstr = " and CatCode = '" + _Val + "' and RefVendorId = " + (int)Session["VendorId"];

                _Result = db.sp_CatalogMas_SelectWhere(_defstr).ToList().Count == 0 ? true : false;

                if (!_Result)
                    _Msg = _Field + " already exist.";

                return Json(new { Result = _Result, Message = _Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult CatalogueDetailPartial(int CatId)
        {
            try
            {
                CatalogModel _ObjCata = new CatalogModel();
                if (CatId != 0)
                {
                    _ObjCata = db.sp_CatatlogMas_SelectBaseOnCatId(CatId).Select(x => new CatalogModel()
                    {
                        CatId = x.CatId,
                        CatName = x.CatName,
                        CatCode = x.CatCode,
                        CatLaunchDate = x.CatLaunchDate,
                        CatDescription = x.CatDescription,
                        FullImgPath = string.IsNullOrEmpty(x.CatImg) ? "/Content/dist/img/CatalogueNoImage.png" : x.ThumbnailImgPath,
                        IsFullset = x.IsFullset
                    }).FirstOrDefault();

                    List<sp_ProductMas_SelectWhere_Result> _ObjProdList = new List<sp_ProductMas_SelectWhere_Result>();
                    _ObjProdList = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => new sp_ProductMas_SelectWhere_Result()
                    {
                        ProdCode = x.ProdCode,
                        ProdId = x.ProdId,
                        ProdDescription = x.ProdDescription,
                        ProdImgPath = x.ProdImgPath,
                        ProdName = x.ProdName,
                        RefProdCategory = x.RefProdCategory,
                        RefBrand = x.RefBrand,
                        RefCatId = x.RefCatId,
                        RefColor = x.RefColor,
                        RefDesign = x.RefDesign,
                        RefFabric = x.RefFabric,
                        RefProdType = x.RefProdType,
                        RefSize = x.RefSize,
                        RefVendorId = x.RefVendorId,
                        RetailPrice = x.RetailPrice,
                        WholeSalePrice = x.WholeSalePrice,
                        VendorName = x.VendorName,
                        ThumbnailImgPath = x.ThumbnailImgPath != null ? x.ThumbnailImgPath : "/Content/dist/img/ProductNoImage.png",
                        OriginalImgPath = x.OriginalImgPath != null ? x.OriginalImgPath : "/Content/dist/img/ProductNoImage.png",
                        Celebrity = x.Celebrity,
                        CatName = x.CatName,
                        ActivetillDate = x.ActivetillDate,
                        IsActive = x.IsActive,
                    }).ToList();


                    _ObjCata.ProductList = _ObjProdList;
                }
                return PartialView(_ObjCata);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult Save(CatalogModel _ObjParam, int RefVendorId, int RefCatId)
        {
            try
            {
                //int ProdId = 0;

                //_ImgPath = ""
                string  _FolderPath = "", _GlobalFolderPath = "";
                _FolderPath = Server.MapPath(db.CompanyProfiles.FirstOrDefault().FolderPath);
                _GlobalFolderPath = _FolderPath + "Global";
                if (!Directory.Exists(_GlobalFolderPath))
                    Directory.CreateDirectory(_GlobalFolderPath);

                //_GlobalFolderPath += "/" + RefVendorId;
                if (!Directory.Exists(_GlobalFolderPath + "/Products"))
                {
                    Directory.CreateDirectory(_GlobalFolderPath + "/Products");
                    Directory.CreateDirectory(_GlobalFolderPath + "/Products/Original");
                    Directory.CreateDirectory(_GlobalFolderPath + "/Products/Thumbnail");
                }

                if (!Directory.Exists(_GlobalFolderPath + "/Catalog"))
                {
                    Directory.CreateDirectory(_GlobalFolderPath + "/Catalog");
                    Directory.CreateDirectory(_GlobalFolderPath + "/Catalog/Original");
                    Directory.CreateDirectory(_GlobalFolderPath + "/Catalog/Thumbnail");
                }

                sp_ProductMerege_Save_Result _ObjPM = db.sp_ProductMerege_Save(RefVendorId, (int)Session["VendorId"], RefCatId, _ObjParam.CatCode, _ObjParam.CatName,
                        _ObjParam.CatDescription, _ObjParam.CatLaunchDate, _ObjParam.IsFullset, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault();

                if (_ObjPM.Id > 0 && Convert.ToBoolean(_ObjPM.Result))
                {
                    if (_ObjPM.CatImg != null)
                    {
                        if (!System.IO.File.Exists(_GlobalFolderPath + "/Catalog/Original/" + _ObjPM.CatImg))
                        {
                            System.IO.File.Copy(_FolderPath + RefVendorId + "/Catalog/Original/" + _ObjPM.CatImg, _GlobalFolderPath + "/Catalog/Original/" + _ObjPM.CatImg);
                            System.IO.File.Copy(_FolderPath + RefVendorId + "/Catalog/Thumbnail/" + _ObjPM.CatImg, _GlobalFolderPath + "/Catalog/Thumbnail/" + _ObjPM.CatImg);
                        }
                    }

                    foreach (var _Obj in _ObjParam.ProductList)
                    {
                        List<sp_ProductMaster_SaveFromStore_Result> _ObjImgDet = db.sp_ProductMaster_SaveFromStore(RefVendorId, _Obj.ProdId, _Obj.ProdName, (int)Session["VendorId"], _ObjPM.Id, _Obj.ProdCode,
                                _Obj.ProdDescription, _Obj.RefProdCategory, _Obj.RefColor, _Obj.RefProdType, _Obj.RefSize, _Obj.RefFabric, _Obj.RefDesign, _Obj.RefBrand,
                                _Obj.Celebrity, _Obj.ActivetillDate, _Obj.RetailPrice, _Obj.WholeSalePrice, (int)Session["VendorId"], CommanClass._Terminal).ToList();


                        //if (ProdId == 0)
                        //{
                        //    return Json(new { Resulr = false, Message = "Server Erro.Try again later!", Id = ProdId }, JsonRequestBehavior.AllowGet);
                        //}


                        if (_ObjImgDet.Count > 0)
                        {
                            foreach (var _ObjImg in _ObjImgDet)
                            {
                                if (_ObjImg != null)
                                {
                                    if (!System.IO.File.Exists(_GlobalFolderPath + "/Products/Original/" + _ObjImg.ImgName))
                                    {
                                        System.IO.File.Copy(_FolderPath + RefVendorId + "/Products/Original/" + _ObjImg.ImgName, _GlobalFolderPath + "/Products/Original/" + _ObjImg.ImgName);
                                        System.IO.File.Copy(_FolderPath + RefVendorId + "/Products/Thumbnail/" + _ObjImg.ImgName, _GlobalFolderPath + "/Products/Thumbnail/" + _ObjImg.ImgName);
                                    }
                                }
                            }
                        }
                    }
                }

                return Json(new { Result = _ObjPM.Result, Message = _ObjPM.Msg, Id = _ObjPM.Id }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}