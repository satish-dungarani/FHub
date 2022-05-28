using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
//using FHub.Models;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public class WriteToUsController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        //// GET api/writetous
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/writetous/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/writetous
        public async Task<IHttpActionResult> Post(JToken _ObjParam)
        {
            try
            {
                JsonSerializer serialize = new JsonSerializer();
                WriteToU _Obj = (WriteToU)serialize.Deserialize(new JTokenReader(_ObjParam), typeof(WriteToU));

                int Id = db.sp_WriteToUs_Save(_Obj.RefAUId, _Obj.Remark, _Obj.RefAUId, _Obj.InsTerminal).FirstOrDefault().Value;
                if (Id == 0)
                    return Json(new { Result = "Error", Code = HttpStatusCode.ExpectationFailed, Data = new { Id = Id }, Message = "Server Error. Try again later!" });
                
                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { Id = Id }, Message = "Thank you for writing us!" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // PUT api/writetous/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/writetous/5
        //public void Delete(int id)
        //{
        //}
    }
}
