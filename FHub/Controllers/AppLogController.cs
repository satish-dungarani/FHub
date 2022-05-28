using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FHubPanel.Models;
//using FHub.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FHub.Controllers
{
    public class AppLogController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();  
        // GET api/applog
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/applog/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/applog
        public async Task<IHttpActionResult> Post(JToken _ObjParam)
        {
            try
            {
                JsonSerializer serialize = new JsonSerializer();
                AppLog _Obj = (AppLog)serialize.Deserialize(new JTokenReader(_ObjParam), typeof(AppLog));

                if (_Obj.RefAUId != 0)
                    _Obj.InsTerminal = db.sp_AppUser_Select(_Obj.RefAUId).FirstOrDefault().DeviceID;

                int AppLigId = db.sp_AppLog_Save(_Obj.RefAUId, _Obj.RefVendorId, _Obj.LogType, _Obj.RefId, _Obj.LogDesc, _Obj.RefAUId, _Obj.InsTerminal).FirstOrDefault().Value;
                if(AppLigId == 0)
                    return Json(new { Result = "Error", Code = HttpStatusCode.ExpectationFailed, Data = new { Id = AppLigId }, Message = "Server Error.Applog not created!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new {Id = AppLigId }, Message = "Log Successfully generated."});
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error.Try again later!"});
            }
        }

        //// PUT api/applog/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/applog/5
        //public void Delete(int id)
        //{
        //}
    }
}
