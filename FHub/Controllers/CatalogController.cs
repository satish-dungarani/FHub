using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
//using FHub.Models;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public class CatalogController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/catalog
        //Retrive catalog data base on Vendor Id and Category Id
        public async Task<IHttpActionResult> GetCatalog(int AUId, int? VendorId, string Category,string LUDate, int? PageSize, int? PageIndex)
        {
            List<CatalogApiModel> _ObjCatList;
            List<sp_DeleteLog_SelectBaseOnDate_Result> _ObjDeleteCat;
            try
            {
                if (db.AppUsers.Find(AUId) == null)
                    return Json(new { Result = "Error", Code = HttpStatusCode.NonAuthoritativeInformation, Data = "", DeletedData = "", Message = "Invalid User!" });
                else if (db.sp_VendorAssociation_SelectWhere(" and RefVendorId =" + VendorId + " and RefAUId = " + AUId).ToList().Count == 0)
                    return Json(new { Result = "Error", Code = HttpStatusCode.NonAuthoritativeInformation, Data = "", DeletedData = "", Message = "Invalid User!" });

                _ObjCatList = db.sp_CatalogMas_Select(VendorId, Category, Convert.ToDateTime(LUDate), PageSize, PageIndex).Select(x => new CatalogApiModel()
                {
                        cid = x.CatId,
                        ccode = x.CatCode,
                        cname = x.CatName,
                        thumb = x.ThumbnailImgPath,
                        pcatg = x.RefProdCategory,
                        color = x.RefColor,
                        fabric = x.RefFabric,
                        size = x.RefSize,   
                        rtype = x.RefProdType,
                        design = x.RefDesign,
                        brand = x.RefBrand,
                        tprod = x.TotalProduct,
                        tprice = x.TotalPrice,
                        trprice = x.TotalRetailPrice,
                        awprice = x.AvgWholeSalePrice,
                        ldate = x.CatLaunchDate == null ? "" : x.CatLaunchDate.Value.ToString("dd-MMM-yyyy"),
                        set = x.IsFullset
                }).ToList();

                _ObjDeleteCat = db.sp_DeleteLog_SelectBaseOnDate(VendorId, "Catalog", Convert.ToDateTime(LUDate)).ToList();

                if ( _ObjCatList.Count == 0 && _ObjDeleteCat.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjCatList, DeletedData = _ObjDeleteCat, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjCatList, DeletedData = _ObjDeleteCat, Message = "Catalog retrive successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", DeletedData = "", Message = "Server Error. Try again later!" });
            }
        }
        // GEt api/Catalog?VendorId=1&CategoryId=1&ProdType=SemiStitched,Stitched&Fabric=Georgette,Silk,cotton&Design=Embroidered,Plain&Brand=Raymond,LinenClub,Scabal
        //Retrive catalog data base on Filter
        public async Task<IHttpActionResult> GetCatalogFilter(int VendorId, string Category, string CatCode, string ProdType, string Fabric, string Design, string Brand, decimal StartPrice, decimal EndPrice, int PageSize, int PageIndex, bool IsFullset, string OrderBy)
        {
            List<sp_CatalogMas_Filter_Result> _ObjCatFilterList;
            try
            {
                //Order By 
                // 1 LD = LaunchDate
                // 2 LHP = Low to High Price
                // 2 HLP = High to Low Price

                _ObjCatFilterList = db.sp_CatalogMas_Filter(VendorId, Category, CatCode, PageSize, PageIndex, ProdType, Fabric, Design, Brand, StartPrice, EndPrice, IsFullset, OrderBy).ToList();
                if (_ObjCatFilterList == null || _ObjCatFilterList.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjCatFilterList, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjCatFilterList, Message = "Catalog retrive successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // GET api/catalog/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/catalog
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/catalog/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/catalog/5
        //public void Delete(int id)
        //{
        //}
    }
}
