using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public class WishListController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/wishlist
        public async Task<IHttpActionResult> Get(int AUId, int VendorId ,int ProdId, bool WishValue)
        {
            try
            {
                if (db.AppUsers.Find(AUId) == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid user!" });
                else if (db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + VendorId +" and RefAUId = " + AUId).ToList().Count != 1)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid rquest!" });
                else if (db.ProductMas.Find(ProdId) == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Product Found!" });

                string _Message  = db.sp_WishList_Save(AUId, VendorId, ProdId, WishValue).FirstOrDefault();
                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = "", Message = _Message });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception" , Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!"});
            }
        }

        public async Task<IHttpActionResult> Get(int AUId, int VendorId)
        {
            try
            {
                List<WishListModel> _ObjWishList;
                if (db.AppUsers.Find(AUId) == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid user!" });

                _ObjWishList = db.sp_WishList_Select(AUId, VendorId).Select(x => new WishListModel() { 
                    Id = x.Id,
                    auid = x.RefAUId,
                    vid = x.RefVendorId,
                    pid = x.RefProdId,
                    pcode =x.ProdCode,
                    pname = x.ProdName,
                    ccode = x.CatCode,
                    brand = x.RefBrand,
                    fabric = x.RefFabric,
                    design = x.RefDesign,
                    color= x.RefColor,
                    size = x.RefSize,
                    category = x.RefProdCategory,
                    ptype = x.RefProdType,
                    rprice = x.RetailPrice,
                    thumb = x.ThumbnailImgPath
                }).ToList();
                if (_ObjWishList == null || _ObjWishList.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjWishList, Message = "No Data Found!"});

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjWishList, Message = "Wish list get successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        //// GET api/wishlist/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/wishlist
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/wishlist/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/wishlist/5
        //public void Delete(int id)
        //{
        //}
    }
}
