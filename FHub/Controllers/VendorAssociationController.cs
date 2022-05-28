using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using FHub.Models;
using FHubPanel.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FHub.Controllers
{
    public class VendorAssociationController : ApiController
    {
        public FHubDBEntities db = new FHubDBEntities();
        // GET api/vendorassociation
        //public IEnumerable<sp_VendorAssociation_Select_Result> GetVA()
        //{
        //    return db.sp_VendorAssociation_Select(null);
        //}

        // GET api/vendorassociation/5
        [HttpGet]
        public IHttpActionResult GetVendorAssociation(int AUId)
        {
            List<sp_VendorAssociation_Select_Result> _ObjVendorAss;
            try
            {
                _ObjVendorAss = db.sp_VendorAssociation_Select(AUId).ToList();
                if (_ObjVendorAss == null || _ObjVendorAss.Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjVendorAss, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjVendorAss, Message = "Data Found!" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }


        // Vendor Association for registration
        // POST api/vendorassociation
        [HttpPost]
        public async Task<IHttpActionResult> PostVendorAssociation(JToken _Obj)
        {
            try
            {
                JsonSerializer serialize = new JsonSerializer();
                VendorAssociation _ObjVA = (VendorAssociation)serialize.Deserialize(new JTokenReader(_Obj), typeof(VendorAssociation));

                if (db.Vendors.Select(x => x.VendorCode.ToUpper() == _ObjVA.VendorCode.ToUpper() && x.IsActive == true).ToList().Count == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Such Vendor Found!" });

                if (_ObjVA.RefVendorId != null && _ObjVA.RefVendorId != 0)
                    _ObjVA.VendorCode = db.Vendors.Find(_ObjVA.RefVendorId).VendorCode;

                int _VAId= db.sp_VendorAssociation_Save(_ObjVA.RefAUId, _ObjVA.VendorCode).FirstOrDefault().Value;

                if (_ObjVA.RefVendorId == null || _ObjVA.RefVendorId == 0)
                {
                    string _Subj ="", _SendText = "", _Detheader = "";
                    //AppUser _ObjAU  = db.AppUsers.Find(_ObjVA.RefAUId);
                    Vendor _ObjVen = new Vendor();
                    _ObjVen = db.sp_Vendor_SelectWhere(" and VendorCode  = '" + _ObjVA.VendorCode + "'").Select(x => new Vendor(){
                        VendorId = x.VendorId,
                        EmailId = x.EmailId,
                    }).FirstOrDefault();
                    Vendor _ObjVd = db.sp_AppUser_Select(_ObjVA.RefAUId).Select(x => new Vendor(){
                        VendorName = x.CompanyName,
                        ContactName = x.AUName,
                        Address = x.Address,
                        City = x.City,
                        State = x.State,
                        Country = x.Country,
                        MobileNo1 = x.MobileNo1,
                        EmailId = x.EmailId
                    }).FirstOrDefault();

                    _Subj = "Association Request";
                    _SendText = "You have a request from " + _ObjVd.VendorName + " for associate with your firm. Kindly review and take appropriate action.";
                    _Detheader = "Registration Detail";
                    FHubPanel.Controllers.CommanClass.MailRequestForAssociation(_ObjVd, _ObjVen.EmailId, _Subj, _SendText, _Detheader);

                    string _ConditionForAdminAU = " and VendorCode = '" + _ObjVA.VendorCode + "' and IsAdmin = 1 and IsAdminNotification = 1";
                    string _Message = _ObjVd.VendorName + " had made a request for association.Please verify.";
                    foreach (sp_VendorAssociation_SelectWhere_Result _ObjAU in db.sp_VendorAssociation_SelectWhere(_ConditionForAdminAU).ToList() )
                    {
                        FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Message, "Associate Request", _ObjVen.VendorId, "", "", "", "", "REGREQ");
                    }

                }

                if (_VAId == 0)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.BadRequest, Data = new { Id = _VAId }, Message = "Vendor Not Found!" });
                else if (_VAId == -1)
                    return Json(new { Result = "Error", Code = HttpStatusCode.BadRequest, Data = "", Message = "Please contact vendor for your registration! " });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { Id = _VAId }, Message = "Thank you for your request!" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        #region "PutMethod"
        //// PUT api/vendorassociation/5
        //[HttpPut]
        //public async Task<IHttpActionResult> PutVendorAssociation(int id, JToken _Obj)
        //{
        //    try
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        VendorAssociation _ObjVA = (VendorAssociation)serializer.Deserialize(new JTokenReader(_Obj), typeof(VendorAssociation));

        //        if (!ModelState.IsValid)
        //            return Json(new { Result = false, Code = HttpStatusCode.BadRequest, Data = "", Msg = "Invalid Data!" });

        //        if (id != _ObjVA.Id)
        //            return Json(new { Result = false, Code = HttpStatusCode.NotFound, Data = new { Id = _ObjVA.Id }, Msg = "No Data Found!" });

        //        db.sp_VendorAssociation_Save(_ObjVA.Id, _ObjVA.RefVendorId, _ObjVA.RefAUId, _ObjVA.VendorCode,
        //                _ObjVA.VendorStatus, _ObjVA.AppUserStatus, _ObjVA.IsNotify, _ObjVA.IsDeleted, _ObjVA.ReqDate, _ObjVA.ApproveDate,
        //                _ObjVA.InsUser, _ObjVA.InsTerminal);


        //        return Json(new { Result = true, Code = HttpStatusCode.OK, Data = new { Id = _ObjVA.Id }, Msg = "Vendor Association Successfully Updated!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        CommanClass.ManageError(ex);
        //        return Json(new { Result = false, Code = HttpStatusCode.BadRequest, Data = "", Msg = "Server Error. Try again Later!" });
        //    }
        //}
        #endregion

        // Update vendor And appuser status api/vendorassociation?id=5&VendorStatus=Pending&AppUserStatus=Request
        [HttpGet]
        public async Task<IHttpActionResult> GetVendorAssociation(int VendorAssId, string AppUserStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(AppUserStatus))
                    return Json(new { Result = false, Code = HttpStatusCode.NoContent, Data = AppUserStatus, Message = "No Data Found!" });

                var _ObjVA = await db.VendorAssociations.FindAsync(VendorAssId);

                if (_ObjVA == null)
                    return Json(new { Result = false, Code = HttpStatusCode.NotFound, Data = _ObjVA, Message = "No Data Found!" });

                //_ObjVA.VendorStatus = VendorStatus;

                //******App User Status ******//
                //Requested
                //Approved
                //Cancelled
                //Deleted

                _ObjVA.AppUserStatus = AppUserStatus;
                await db.SaveChangesAsync();

                return Json(new { Result = true, Code = HttpStatusCode.OK, Data = new { Id = VendorAssId }, Message = "Your request has been " + AppUserStatus });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = false, Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        //// Delete VendorAssociation api/vendorassociation?id=5&IsDeleted=true
        //public async Task<IHttpActionResult> PutVendorAssociationIsDeleted(int id, bool IsDeleted )
        //{
        //    try
        //    {
        //        var _ObjVA = await db.VendorAssociations.FindAsync(id);
        //        if (_ObjVA == null)
        //            return BadRequest("Vendor Association Not exists!");

        //        _ObjVA.IsDeleted = IsDeleted;
        //        await db.SaveChangesAsync();

        //        return StatusCode(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // Notification VendorAssociation api/vendorassociation?id=5&IsNotify=true
        [HttpGet]
        public async Task<IHttpActionResult> GetVendorAssociationIsNotify(int VendorAssId, bool IsNotify)
        {
            try
            {
                var _ObjVA = await db.VendorAssociations.FindAsync(VendorAssId);
                if (_ObjVA == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });

                _ObjVA.IsNotify = IsNotify;
                await db.SaveChangesAsync();

                string _Msg = "";
                if (IsNotify)
                    _Msg = "On";
                else
                    _Msg = "Off";

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { Id = VendorAssId }, Message = "Notifiaction turn " + _Msg });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again Later!" });
            }
        }

        public async Task<IHttpActionResult> GetRateUs(int VendorAssId, int Rate)
        {
            try
            {
                VendorAssociation _obj = await db.VendorAssociations.FindAsync(VendorAssId);
                if (_obj == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Data Found!" });

                _obj.RateVendor = Rate;
                await db.SaveChangesAsync();

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = new { Id = VendorAssId }, Message = "Thank you for rating." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }
    }
}
