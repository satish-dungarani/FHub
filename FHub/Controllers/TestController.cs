using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;
using FHub.Models;

namespace FHub.Controllers
{
    public class TestController : Controller
    {
        HttpClient _Client;
        string _Url = ConfigurationManager.AppSettings["WebApiUrl"].ToString() + "appuser";
        //string _Url = ConfigurationManager.AppSettings["WebApiUrl"].ToString() + "VendorAssociation"; 
        public  TestController()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = new Uri(_Url);
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        //
        // GET: /Test/
        public async Task<ActionResult> Index()
        {
            #region "AppUser Data"
            AppUser _ObjAppUser = new AppUser();

            //_ObjAppUser.AUId = 1;
            //_ObjAppUser.AUName = "Darshan Gangadwala";
            //_ObjAppUser.CompanyName = "GDY";
            //_ObjAppUser.Address = "B/95 Nityanand";
            //_ObjAppUser.LandMark = "Katargam";
            //_ObjAppUser.Country = "India";
            //_ObjAppUser.State = "Gujarat";
            //_ObjAppUser.City = "Surat";
            //_ObjAppUser.Pincode = "395004";
            //_ObjAppUser.ContactNo1 = "5555555";
            //_ObjAppUser.ContactNo2 = null;
            //_ObjAppUser.MobileNo1 = "999999999";
            //_ObjAppUser.MobileNo2 = "Darshan";
            //_ObjAppUser.EmailId = "gangadwaladarshan@gmail.com";
            //_ObjAppUser.WebSite = null;
            //_ObjAppUser.BGID = null;
            //_ObjAppUser.APPID = null;
            //_ObjAppUser.DeviceOS = "Android";
            //_ObjAppUser.IsActive = true;
            //_ObjAppUser.IsNotify = true;
            //_ObjAppUser.InsUser = 1;
            //_ObjAppUser.InsTerminal = "127.0.0.1";
            //_ObjAppUser.UpdUser = 1;
            //_ObjAppUser.UpdTerminal = "127.0.0.1";

            //HttpResponseMessage responseMessage = await _Client.PostAsJsonAsync(_Url, _ObjAppUser);
            //if (responseMessage.IsSuccessStatusCode)
            //    return Json(new { Result = "OK", msg = "Record successfully inserted!" });
            //else
            //    return Json(new { Result = "ERROR", msg = responseMessage.ToString() });

            //HttpResponseMessage respMsg = await _Client.PutAsJsonAsync(_Url + "/" + 1, _ObjAppUser);
            //if (respMsg.IsSuccessStatusCode)
            //    return Json(new { Result = "OK", msg = "Record successfully inserted!" });
            //else
            //    return Json(new { Result = "ERROR", msg = respMsg.ToString() });


            //string _strPut = _Url + "?Id=1&IsActive=" + false + "";
            //HttpResponseMessage responseMsg = await _Client.PutAsJsonAsync(_strPut, false);
            //if (responseMsg.IsSuccessStatusCode)
            //    return Json(new { Result = "OK", msg = "Record successfully inserted!" });
            //else
            //    return Json(new { Result = "ERROR", msg = responseMsg.ToString() });
            #endregion

            #region "VendorAssociation"
            //VendorAssociation _ObjVA = new VendorAssociation();

            //_ObjVA.Id = 2;
            //_ObjVA.RefAUId = 2;
            //_ObjVA.RefVendorId = 0;
            //_ObjVA.VendorCode = "002";
            //_ObjVA.IsActive = true;
            //_ObjVA.IsNotify = true;
            //_ObjVA.IsDeleted = false;
            //_ObjVA.ReqDate = System.DateTime.Now;
            //_ObjVA.ApproveDate = System.DateTime.Now;
            //_ObjVA.InsTerminal = "127.0.0.1";
            //_ObjVA.InsUser = 1;
            //_ObjVA.UpdTerminal = "127.0.0.1";
            //_ObjVA.UpdUser = 1;

            ////HttpResponseMessage _respmag = await _Client.PostAsJsonAsync(_Url, _ObjVA);
            ////if (_respmag.IsSuccessStatusCode)
            ////    return Json(new { Result = "Ok", msg = "Record Successfully inserted!" });
            ////else
            ////    return Json(new { Result = "ERROR", msg = "Record faile to insert!" });

            //HttpResponseMessage _respmag = await _Client.PutAsJsonAsync(_Url + "/" + 2, _ObjVA);

            //HttpResponseMessage _RepMessage = await _Client.PutAsJsonAsync(_Url + "?Id=3&IsDeleted=true", true);

            //if (_respmag.IsSuccessStatusCode)
            //    return Json(new { Result = "Ok", msg = "Record Successfully inserted!" });
            //else
            //    return Json(new { Result = "ERROR", msg = "Record faile to insert!" });

            //if (_RepMessage.IsSuccessStatusCode)
            //    return Json(new { Result = "Ok", msg = "Record Successfully inserted!" });
            //else
            //    return Json(new { Result = "ERROR", msg = "Record faile to insert!" });

            #endregion

            return View();
        }
	}
}