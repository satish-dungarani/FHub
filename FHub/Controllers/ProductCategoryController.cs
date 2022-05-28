using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
//using System.Web.Mvc;
//using FHub.Models;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public class ProductCategoryController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();

        // GET api/productcategory
        // Get Category list base on vendor id 
        public async Task<IHttpActionResult> GetPC(int VendorId)
        {
            //List<sp_ProductCategory_SelectWhere_Result> _ObjPCList;
            List<ProductCategoryAPIModel> _ObjPCList;
            try
            {
                string _StrCondition = " and RefVendorId = " + VendorId;

                _ObjPCList = db.sp_ProductCategory_SelectWhere(_StrCondition).Select(x => new ProductCategoryAPIModel()
                {
                    PCId = x.PCId,
                    RefVendorId = x.RefVendorId,
                    VendorName = x.VendorName,
                    ProdCategoryName = x.ProdCategoryName,
                    ProdCategoryDesc = x.ProdCategoryDesc,
                    ProdCategoryImg = x.ProdCategoryImg,
                    ThumbnailImgPath = x.ThumbnailImgPath,
                    RefPCId = x.RefPCId,
                    Ord = x.Ord
                }).ToList();

                //_ObjDeletePC = db.sp_DeleteLog_SelectBaseOnDate(VendorId, "Category", DateTime.Parse(LUDate,)).ToList();

                if (_ObjPCList == null || _ObjPCList.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjPCList, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjPCList, Message = "Product Category retrive successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        //// GET api/productcategory/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/productcategory
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/productcategory/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/productcategory/5
        //public void Delete(int id)
        //{
        //}
    }
}
