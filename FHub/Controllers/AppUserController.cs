using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using FHub.Models;
using FHubPanel.Models;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FHub.Controllers
{
    public class AppUserController : ApiController
    {
        public FHubDBEntities db = new FHubDBEntities();

        // GET api/appuser
        //public IEnumerable<sp_AppUser_Select_Result> GetAppUser()
        //{
        //    return db.sp_AppUser_Select(null);
        //}

        // GET api/appuser/5
        //[Authorize]
        [HttpGet]
        public async Task<IHttpActionResult> GetAppUser(int AUId)
        {
            try
            {
                //JsonSerializer serializer = new JsonSerializer();
                sp_AppUser_Select_Result _Objget = db.sp_AppUser_Select(AUId).FirstOrDefault();
                //AppUser _ObjAppUser = (AppUser)serializer.Deserialize(new JTokenReader(_Obj), typeof(AppUser));
                //var _Obj = serializer.Serialize(new JTokenWriter(), _Objget);

                if (_Objget == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _Objget, Message = "No Data Found!" });
                //throw new Exception();
                //return BadRequest("Record Not Found!");

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _Objget, Message = "Data Found Successfully." });
                //return Ok(_Objget);

            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }

            //return Json(new {Result = (JsonResult)db.sp_AppUser_Select(id).FirstOrDefault();});
        }

        // GET api/appuser/5
        [HttpGet]
        public IHttpActionResult GetAppUserDeviceId(string Deviceid)
        {
            List<sp_AppUser_SelectWhere_Result> _ObjDeviceId;
            try
            {
                _ObjDeviceId = db.sp_AppUser_SelectWhere(Deviceid).ToList();
                if (_ObjDeviceId == null || _ObjDeviceId.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjDeviceId, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjDeviceId, Message = "Data Found Successfully" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }

        }

        // POST api/appuser
        [HttpPost]
        public async Task<IHttpActionResult> PostAppUser(JToken _Obj)
        {
            try
            {

                JsonSerializer serializer = new JsonSerializer();
                AppUser _ObjAppUser = (AppUser)serializer.Deserialize(new JTokenReader(_Obj), typeof(AppUser));

                if (!ModelState.IsValid)
                    return Json(new { Result = "Error", Code = HttpStatusCode.BadRequest, Data = "", Message = "Wrong data passed!" });

                if (_ObjAppUser.DeviceID == null)
                    return Json(new { Result = "Error", Code = HttpStatusCode.BadRequest, Data = "", Message = "Device Id Can not allow null!" });
                else if (_ObjAppUser.MobileNo1 == null)
                    return Json(new { Result = "Error", Code = HttpStatusCode.BadRequest, Data = "", Message = "MobileNo Can not allow null!" });
                //_ObjAppUser.DeviceOS= "Android";
                //_ObjAppUser.IsActive = true;
                //_ObjAppUser.IsNotify= true;
                // db.AppUsers.Add(_ObjAppUser);
                //await db.SaveChangesAsync();

                int _AUId = db.sp_AppUser_Save(_ObjAppUser.AUId, _ObjAppUser.AUName, _ObjAppUser.CompanyName, _ObjAppUser.Address,
                    _ObjAppUser.LandMark, _ObjAppUser.Country, _ObjAppUser.State, _ObjAppUser.City, _ObjAppUser.Pincode,
                    _ObjAppUser.ContactNo1, _ObjAppUser.ContactNo2, _ObjAppUser.MobileNo1, _ObjAppUser.MobileNo2, _ObjAppUser.EmailId,
                    _ObjAppUser.WebSite, _ObjAppUser.GCMID, _ObjAppUser.DeviceID, _ObjAppUser.DeviceOS, _ObjAppUser.IsActive, _ObjAppUser.IsNotify, _ObjAppUser.DefaultView,
                    _ObjAppUser.AppUserImg, _ObjAppUser.InsUser, _ObjAppUser.InsTerminal).FirstOrDefault().Value;

                //return CreatedAtRoute("DefaultApi", new { id = AUId }, _ObjAppUser);
                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = _AUId }, Message = "Register Successfully!" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // PUT api/appuser/5
        [HttpPut]
        public async Task<IHttpActionResult> PutAppUser(JToken _Obj)
        {
            try
            {
                JsonSerializer serialize = new JsonSerializer();
                AppUser _ObjAppUser = (AppUser)serialize.Deserialize(new JTokenReader(_Obj), typeof(AppUser));

                if (!ModelState.IsValid)
                    return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = new { AUId = _ObjAppUser.AUId }, Message = "Wrong data passed!" });

                //if (AUId != _ObjAppUser.AUId)
                //    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = new { AUId = _ObjAppUser.AUId }, Message = "No Data Found!" });

                db.sp_AppUser_Save(_ObjAppUser.AUId, _ObjAppUser.AUName, _ObjAppUser.CompanyName, _ObjAppUser.Address,
                    _ObjAppUser.LandMark, _ObjAppUser.Country, _ObjAppUser.State, _ObjAppUser.City, _ObjAppUser.Pincode,
                    _ObjAppUser.ContactNo1, _ObjAppUser.ContactNo2, _ObjAppUser.MobileNo1, _ObjAppUser.MobileNo2, _ObjAppUser.EmailId,
                    _ObjAppUser.WebSite, _ObjAppUser.GCMID, _ObjAppUser.DeviceID, _ObjAppUser.DeviceOS, _ObjAppUser.IsActive, _ObjAppUser.IsNotify, _ObjAppUser.DefaultView,
                    _ObjAppUser.AppUserImg, _ObjAppUser.UpdUser, _ObjAppUser.UpdTerminal);

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = _ObjAppUser.AUId }, Message = "Profile Update Successfully" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // PUT api/appuser/5
        public async Task<IHttpActionResult> GetDefaultView(int AUId, string DefaultView)
        {
            try
            {
                AppUser _obj = await db.AppUsers.FindAsync(AUId);
                if (_obj == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _obj, Message = "No Data Found!" });

                _obj.DefaultView = DefaultView;
                await db.SaveChangesAsync();

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = AUId }, Message = "Default view set successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = new { AUId = AUId }, Message = "Server Error. Try again later!" });
            }
        }

        // Update Notify Status api/appuser?id=3&IsNotify=true
        [HttpGet]
        public async Task<IHttpActionResult> GetIsNotify(int AUId, bool IsNotify)
        {
            try
            {
                AppUser _obj = await db.AppUsers.FindAsync(AUId);
                if (_obj == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });

                _obj.IsNotify = Convert.ToBoolean(IsNotify);
                await db.SaveChangesAsync();

                string _Msg = "";
                if (IsNotify)
                    _Msg = "On";
                else
                    _Msg = "Off";

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = AUId }, Message = "Notifiaction turn " + _Msg });
            }
            catch (Exception ex)
            {

                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        public async Task<IHttpActionResult> GetRateUs(int AUId, int VendorId, int Rate)
        {
            try
            {
                if (VendorId == 0)
                {
                    AppUser _obj = await db.AppUsers.FindAsync(AUId);
                    if (_obj == null)
                        return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });
                    _obj.RateUs = Rate;
                    await db.SaveChangesAsync();
                    return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = AUId }, Message = "Thank you for rating." });
                }
                else
                {
                    sp_VendorAssociation_SelectWhere_Result _Obj = db.sp_VendorAssociation_SelectWhere(" and RefAUId = " + AUId + " and RefVendorId = " + VendorId).FirstOrDefault();
                    if (_Obj == null)
                        return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });
                    if (!db.sp_VendorAssociation_Rate(VendorId, AUId, Rate).FirstOrDefault().Value)
                        return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });

                    return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { AUId = AUId }, Message = "Thank you for rating." });
                }
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        public async Task<IHttpActionResult> GetRequestList(int VendorId, string VendorStatus)
        {
            try
            {
                List<RequestModel> _ObjRequest = new List<RequestModel>();
                string _defstr = " and RefVendorId =" + VendorId ;
                if (!string.IsNullOrEmpty(VendorStatus))
                    _defstr += " and VendorStatus = '" + VendorStatus + "'";
                _ObjRequest = db.sp_VendorAssociation_SelectWhere(_defstr).Select(x => new RequestModel()
                {
                    Id = x.Id,
                    VendorName = x.VendorName,
                    AUName = x.AUName,
                    CompanyName = x.CompanyName,
                    VendorStatus = x.VendorStatus,
                    AppUserStatus = x.AppUserStatus,
                    ReqDate = x.ReqDate.ToString(),
                    ApproveDate = x.ApproveDate.ToString(),
                    EmailId = x.EmailId,
                    MobileNo  = x.MobileNo1
                }).ToList();
                if (_ObjRequest.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjRequest, Message = "Request list successfully get." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> SetRequestStatus(int RequestId, string VendorStatus)
        {
            try
            {
                VendorAssociation _ObjVA = db.VendorAssociations.Find(RequestId);
                _ObjVA.VendorStatus = VendorStatus;
                if (VendorStatus == "Approved")
                {
                    _ObjVA.AppUserStatus = VendorStatus;
                    _ObjVA.ApproveDate = System.DateTime.Now;
                }
                db.SaveChanges();

                string _Subj = "", _SendText = "";
                _Subj = "Associate " + VendorStatus;
                if (VendorStatus == "Approved")
                {
                    _SendText = "Your association request has been accepted. Kindly visit store on FashionDiva App for latest updates on products and catalogues.";
                }
                else if (VendorStatus == "Rejected")
                {
                    _SendText = "Your association request has been rejected. ";
                }

                FHubPanel.Controllers.CommanClass.MailOnAction(_ObjVA.Vendor.VendorName, _ObjVA.AppUser.AUName, _ObjVA.AppUser.EmailId, _Subj, _SendText);

                string _ConditionForAU = " and Id = " + RequestId;
                var _ObjAU = db.sp_VendorAssociation_SelectWhere(_ConditionForAU).FirstOrDefault();
                string _Message = _ObjAU.VendorName + " had " + VendorStatus + " your accosiation request. ";
                if (VendorStatus == "Approved")
                    _Message += "Tap to visit our store.";
                FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Message, "Associate Request Status", Convert.ToInt32(_ObjAU.RefVendorId), "", "", "", "", "VL");

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = "", Message = "Request " + VendorStatus + " successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> SetGCMID(int AUId, string GCMId,decimal AppVersion )
        {
            try
            {
                AppUser _ObjAU = db.AppUsers.Find(AUId);
                if (_ObjAU == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Dat Found!" });
                _ObjAU.GCMID = GCMId;
                _ObjAU.AppVersion = AppVersion;
                db.SaveChanges();
                
                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = "", Message = "GCM Id and AppVersion successfully updated." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

    }
}
