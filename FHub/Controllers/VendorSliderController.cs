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
    public class VendorSliderController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/vendorslider
        public async Task<IHttpActionResult> Get(int VendorId)
        {
            try
            {
                List<sp_VendorSlider_SelectForAPI_Result> _ObjSlider = db.sp_VendorSlider_SelectForAPI(VendorId).ToList();
                if (_ObjSlider.Count == 0 || _ObjSlider == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjSlider, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjSlider, Message = "Slider Get successfully" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception" , Code = HttpStatusCode.BadRequest, Data = "" , Message = "Server Error. Try again later!"});
            }
        }

        //// GET api/vendorslider/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/vendorslider
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/vendorslider/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/vendorslider/5
        //public void Delete(int id)
        //{
        //}
    }
}
