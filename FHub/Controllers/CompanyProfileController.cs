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
    public class CompanyProfileController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/companyprofile
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/companyprofile/5
        public async Task<IHttpActionResult> GetCompanyProfile()
        {
            try
            {
                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                if (_ObjComp == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjComp, Message = "No Data Found!" });

                return Json(new
                {
                    Result = "Success",
                    Code = HttpStatusCode.OK,
                    Data = new
                    {
                        Description = _ObjComp.Description,
                        EmailId = _ObjComp.EmailId,
                        FolderPath = _ObjComp.FolderPath,
                        WebSite = _ObjComp.WebSite,
                        CompanyName = _ObjComp.CompanyName,
                        LogoImg = _ObjComp.LogoImg == null ? "" : _ObjComp.LogoImg,
                        AboutUs = _ObjComp.AboutUs == null ? "" : _ObjComp.AboutUs,
                        Vision = _ObjComp.Vision == null ? "" : _ObjComp.Vision,
                        Mission = _ObjComp.Mission == null ? "" : _ObjComp.Mission 
                    },
                    Message = "Company Profile Get Successfully."
                });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error.Try again later!" });
            }
        }

        //// POST api/companyprofile
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/companyprofile/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/companyprofile/5
        //public void Delete(int id)
        //{
        //}
    }
}
