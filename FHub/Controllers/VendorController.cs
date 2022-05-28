using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using FHub.Models;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public class VendorController : ApiController
    {
        public FHubDBEntities db = new FHubDBEntities();
        // GET api/vendor
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/vendor/5
        [HttpGet]
        public IHttpActionResult GetVendor(int VendorId)
        {
            sp_Vendor_Select_Result _ObjVendor;
            try
            {
                _ObjVendor = db.sp_Vendor_Select(VendorId).FirstOrDefault();
                if (_ObjVendor == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjVendor, Message = "No Data Found!" });

                return Json(new
                {
                    Result = "Success",
                    Code = HttpStatusCode.OK,
                    Data = new
                    {
                        VendorId = _ObjVendor.VendorId,
                        VendorName = _ObjVendor.VendorName,
                        Address = _ObjVendor.Address,
                        Landmark = _ObjVendor.Landmark,
                        Country = _ObjVendor.Country,
                        State = _ObjVendor.State,
                        City = _ObjVendor.City,
                        Pincode = _ObjVendor.Pincode,
                        ContactName = _ObjVendor.ContactName,
                        ContactNo1 = _ObjVendor.ContactNo1,
                        ContactNo2 = _ObjVendor.ContactNo2,
                        MobileNo1 = _ObjVendor.MobileNo1,
                        MobileNo2 = _ObjVendor.MobileNo2,
                        FaxNo = _ObjVendor.FaxNo,
                        EmailId = _ObjVendor.EmailId,
                        WebSite = _ObjVendor.WebSite,
                        LogoImg = _ObjVendor.LogoImg,
                        ThumbnailImgPath = _ObjVendor.ThumbnailImgPath,
                        IsActive = _ObjVendor.IsActive,
                    }
                    ,
                    Message = "Data Found!"
                });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again Later!" });
            }
        }

        // POST api/vendor
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/vendor/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/vendor/5
        //public void Delete(int id)
        //{
        //}
    }
}
