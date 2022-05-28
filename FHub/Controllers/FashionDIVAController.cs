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
    public class FashionDIVAController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/fashiondiva
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                int _VendorCount, _AppUserCount, _ProductCount;
                _VendorCount = db.Vendors.ToList().Count;
                _AppUserCount = db.AppUsers.ToList().Count;
                _ProductCount = db.ProductMas.ToList().Count + db.CatalogMas.ToList().Count; 
                return Json(new
                {
                    Result = true,
                    Code = HttpStatusCode.OK,
                    Data = new
                    {
                        VendorCount = _VendorCount,
                        AppUserCount = _AppUserCount,
                        ProductCount = _ProductCount
                    },
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new
                {
                    Result = false,
                    Code = HttpStatusCode.BadRequest,
                    Data = new
                    {
                        VendorCount = 0,
                        AppUserCount = 0,
                        ProductCount = 0
                    },
                    Message = ""
                });
            }
        }

        //// GET api/fashiondiva/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/fashiondiva
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/fashiondiva/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/fashiondiva/5
        //public void Delete(int id)
        //{
        //}
    }
}
