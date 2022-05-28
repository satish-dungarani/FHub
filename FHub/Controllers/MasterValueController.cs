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
    public class MasterValueController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/mastervalue?MasterId=1&VendorId=1

        //public IHttpActionResult GetMasterVal(int MasterId, int VendorId)
        //{
        //    List<sp_MasterValue_Select_Result> _ObjMasterList;
        //    try
        //    {
        //        _ObjMasterList = db.sp_MasterValue_Select(null, MasterId, VendorId).ToList();
        //        if (_ObjMasterList == null || _ObjMasterList.Count == 0)
        //            return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjMasterList , Message = "No Data Found!" });

        //        return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjMasterList, Message = "Master value retrieve successfully!" });

        //    }
        //    catch (Exception ex)
        //    {
        //        CommanClass.ManageError(ex);
        //        return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
        //    }
        //}

        //public IHttpActionResult GetMasterValMinMaxPrice(int VendorId, string Category)
        //{
        //    sp_MasterValue_GetMinMaxPrice_Result _ObjMasterMinMaxPrice;
        //    try
        //    {
        //        _ObjMasterMinMaxPrice = db.sp_MasterValue_GetMinMaxPrice(VendorId, Category).FirstOrDefault();
        //        if (_ObjMasterMinMaxPrice == null || _ObjMasterMinMaxPrice.MaxPrice == null || _ObjMasterMinMaxPrice.MinPrice == null)
        //            return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjMasterMinMaxPrice, Message = "No Data Found!" });

        //        return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjMasterMinMaxPrice, Message = "Master value retrieve successfully!" });

        //    }
        //    catch (Exception ex)
        //    {
        //        CommanClass.ManageError(ex);
        //        return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
        //    }
        //}

        //// GET api/mastervalue/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/mastervalue
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/mastervalue/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/mastervalue/5
        //public void Delete(int id)
        //{
        //}
    }
}
