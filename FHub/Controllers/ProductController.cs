using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FHubPanel.Models;
using System.Threading.Tasks;

namespace FHub.Controllers
{
    public class ProductController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/product
        public async Task<IHttpActionResult> GetProduct(int AUId, int VendorId, int? CatId, string Category, string LUDate , int PageSize, int PageIndex)
        {
            List<ProductApiModel> _ObjProdlist;
            List<sp_DeleteLog_SelectBaseOnDate_Result> _ObjDeletedProd;
            try
            {
                if (db.AppUsers.Find(AUId) == null)
                    return Json(new { Result = "Error", Code = HttpStatusCode.NonAuthoritativeInformation, Data = "", DeletedData = "", Message = "Invalid User!" });
                else if (db.sp_VendorAssociation_SelectWhere(" and RefVendorId =" + VendorId + " and RefAUId = " + AUId).ToList().Count == 0)
                    return Json(new { Result = "Error", Code = HttpStatusCode.NonAuthoritativeInformation, Data = "", DeletedData = "", Message = "Invalid request!" });

                _ObjProdlist = db.sp_ProductMas_Select(AUId, VendorId, CatId, Category, Convert.ToDateTime(LUDate), PageSize, PageIndex).Select(x => new ProductApiModel()
                {
                    pid = x.ProdId,
                    pcode = x.ProdCode,
                    pcatg = x.RefProdCategory,
                    color = x.RefColor,
                    rtype = x.RefProdType,
                    size = x.RefSize,
                    fabric = x.RefFabric,
                    design = x.RefDesign,
                    brand = x.RefBrand,
                    rprice= x.RetailPrice,
                    wprice = x.WholeSalePrice,
                    thumb = x.ThumbnailImgPath == null ? "[]" : x.ThumbnailImgPath,
                    CatCode = x.CatCode,
                    wish = x.IsInWishList,
                    rcati = x.RefCatId,
                    set = x.IsFullset,
                    ldate = x.CatLaunchDate == null ? null : x.CatLaunchDate.Value.ToString("dd-MMM-yyyy"),
                    pname = x.ProdName
                }).ToList();

                _ObjDeletedProd = db.sp_DeleteLog_SelectBaseOnDate(VendorId,"Product", Convert.ToDateTime(LUDate)).ToList();

                if (_ObjProdlist.Count == 0 && _ObjDeletedProd.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjProdlist, DeletedData = _ObjDeletedProd, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjProdlist, DeletedData = _ObjDeletedProd, Message = "Retrieve Product list successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error.Try again later!" });
            }

        }

        public async Task<IHttpActionResult> GetProduct(int AUId, int VendorId, int ProdId)
        {
            sp_ProductMas_SelectForAdmin_Result _ObjProd;
            try
            {
                if (db.AppUsers.Find(AUId) == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid User!" });
                else if (db.sp_VendorAssociation_SelectWhere(" and RefVendorId =" + VendorId + " and RefAUId = " + AUId).ToList().Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid request!" });
                if (db.ProductMas.Find(ProdId) == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Product Found!" });

                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();

                _ObjProd = db.sp_ProductMas_SelectForAdmin(VendorId, ProdId).FirstOrDefault();

                _ObjProd.OriginalImgPath = "";
                foreach (var _ObjImgDet in db.sp_ProductImgDet_SelectForAPI(ProdId, "API").ToList())
                {
                    _ObjProd.OriginalImgPath += "{ 'Image' : '" + _ObjImgDet.OriginalImgPath + "'},";
                }
                if (_ObjProd.OriginalImgPath != "")
                    _ObjProd.OriginalImgPath = "[" + _ObjProd.OriginalImgPath.Substring(0, _ObjProd.OriginalImgPath.Length - 1) + "]";

                if (_ObjProd == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjProd, Message = "No Data Found!" });

                return Json(new
                {
                    Result = "Success",
                    Code = HttpStatusCode.OK,
                    Data = new
                    {
                        ProdId = _ObjProd.ProdId,
                        ProdName = _ObjProd.ProdName,
                        CatCode = _ObjProd.CatCode,
                        ProdCode = _ObjProd.ProdCode,
                        ProdDescription = _ObjProd.ProdDescription,
                        RefVendorId = _ObjProd.RefVendorId,
                        VendorName = _ObjProd.VendorName,
                        RefProdCategory = _ObjProd.RefProdCategory,
                        RefColor = _ObjProd.RefColor,
                        RefProdType = _ObjProd.RefProdType,
                        RefSize = _ObjProd.RefSize,
                        RefFabric = _ObjProd.RefFabric,
                        RefDesign = _ObjProd.RefDesign,
                        RefBrand = _ObjProd.RefBrand,
                        Celebrity = _ObjProd.Celebrity,
                        RefCatId = _ObjProd.RefCatId,
                        CatName = _ObjProd.CatName,
                        RetailPrice = _ObjProd.RetailPrice,
                        WholeSalePrice = _ObjProd.WholeSalePrice,
                        ProdImgPath = _ObjProd.ProdImgPath,
                        IsActive = _ObjProd.IsActive,
                        OriginalImgPath = _ObjProd.OriginalImgPath,
                    },
                    Message = "Retrieve Product list successfully."
                });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error.Try again later!" });
            }

        }

        // GET api/product/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/product
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/product/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/product/5
        //public void Delete(int id)
        //{
        //}
    }
}
