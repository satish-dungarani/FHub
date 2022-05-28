using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FHubPanel.Models;
using Newtonsoft.Json;


namespace FHub.Controllers
{
    public class EnquiryListController : ApiController
    {
        FHubDBEntities db = new FHubDBEntities();
        // GET api/enquirylist
        public async Task<IHttpActionResult> GetEnquiry(int? AUId, int VendorId)
        {
            List<EnquiryModel> _ObjEnq;
            try
            {
                if (AUId != null)
                {
                    if (db.AppUsers.Find(AUId) == null)
                        return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No AppUser Found!" });
                }

                _ObjEnq = db.sp_EnquiryList_SelectForAPI(AUId, VendorId).Select(x => new EnquiryModel()
                {
                    Id = x.Id,
                    auid = x.RefAUId,
                    cid = x.CatId,
                    ccode = x.CatCode,
                    cname = x.CatName,
                    pid = x.RefProdId,
                    pcode = x.ProdCode,
                    pname = x.ProdName,
                    fabric = x.Fabric,
                    color = x.Color,
                    thumb = x.ThumbnailImgPath,
                    rprice = x.RetailPrice,
                    tprice = x.WholeSalePrice,
                    Remark = x.Remark,
                    edate = x.EnquiryDate.ToString(),
                    Status = x.Status,
                    reply = x.RepRemark,
                    rdate = x.EnquiryRepDate.ToString(),
                    rby = x.RepAUName,
                    auname = x.AUName,
                    mno = x.MobileNo,
                    compname = x.CompanyName
                }).ToList();
                if (_ObjEnq.Count == 0 || _ObjEnq == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = _ObjEnq, Message = "No Data Found!" });

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = _ObjEnq, Message = "Enquiry list get successfully." });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // GET api/enquirylist/5
        public IHttpActionResult GetEnquiryDelete(int EnqId, bool IsDelete)
        {
            try
            {
                EnquiryList _ObjEnq = db.EnquiryLists.Find(EnqId);
                if (_ObjEnq == null)
                    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = EnqId, Message = "No Data Found!" });

                if (IsDelete)
                {
                    db.EnquiryLists.Remove(_ObjEnq);
                    db.SaveChanges();
                    return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = EnqId, Message = "Enquiry canceled successfully." });
                }
                else
                    return Json(new { Result = "Error", Code = HttpStatusCode.NoContent, Data = EnqId, Message = "Enquiry cancelation fail!" });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        // POST api/enquirylist
        public async Task<IHttpActionResult> PostEnquiry(JToken _Obj)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                EnquiryList _ObjEnq = (EnquiryList)serializer.Deserialize(new JTokenReader(_Obj), typeof(EnquiryList));

                if (_ObjEnq.Id == null || _ObjEnq.Id == 0)
                {
                    if (db.AppUsers.Find(_ObjEnq.RefAUId) == null)
                        return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid User!" });
                    else if (db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + _ObjEnq.RefVendorId + " and RefAUId = " + _ObjEnq.RefAUId).ToList().Count != 1)
                        return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "Invalid request!" });
                    //else if (db.ProductMas.Find(_ObjEnq.RefProdId) == null)
                    //    return Json(new { Result = "NoData", Code = HttpStatusCode.NotFound, Data = "", Message = "No Such Product Found!" });
                }

                string _Message = db.sp_EnquiryList_Save(_ObjEnq.Id, _ObjEnq.RefAUId, _ObjEnq.RefVendorId, _ObjEnq.RefProdId, _ObjEnq.RefCatId,
                        _ObjEnq.Remark, _ObjEnq.RepRemark, _ObjEnq.Status).FirstOrDefault();

                if (_ObjEnq.Id == 0 || _ObjEnq.Id == null)
                {
                    string _ConditionForAdminAU = " and RefVendorId = " + _ObjEnq.RefVendorId + " and IsAdmin = 1 and IsAdminNotification = 1";
                    string _CompName = db.sp_AppUser_Select(_ObjEnq.RefAUId).FirstOrDefault().CompanyName;
                    string _Name = "";
                    string _Title = "";
                    if (_ObjEnq.RefProdId != null)
                    {
                        _Title = "Product Enquiry";
                        _Name = db.sp_ProductMas_SelectForAdmin(_ObjEnq.RefVendorId, _ObjEnq.RefProdId).FirstOrDefault().ProdName;
                    }
                    else
                    {
                        _Title = "Catalogue Enquiry";
                        _Name = db.sp_CatatlogMas_SelectBaseOnCatId(_ObjEnq.RefCatId).FirstOrDefault().CatName;
                    }

                    string EnqId = db.sp_EnquiryList_SelectForAPI(_ObjEnq.RefAUId, _ObjEnq.RefVendorId).FirstOrDefault().Id.ToString();
                    string _Msg = _CompName + " had made a enquiry for " + _Name + ".";
                    string _CCode = "";
                    string _ImgPath = "";
                    CatalogMa _ObjCatMas = new CatalogMa();
                    if (_ObjEnq.RefCatId != null && _ObjEnq.RefCatId != 0)
                    {
                        _ObjCatMas = db.sp_CatatlogMas_SelectBaseOnCatId(_ObjEnq.RefCatId).Select(x => new CatalogMa()
                        {
                            CatCode = x.CatCode,
                            CatImg = x.ThumbnailImgPath
                        }).FirstOrDefault();
                    }
                    string _ProdId = "";
                    if (_ObjEnq.RefProdId != null)
                    {
                        _ProdId = Convert.ToString(_ObjEnq.RefProdId);
                        _ImgPath = db.sp_ProductMas_SelectWhere(" and ProdId = " + _ObjEnq.RefProdId).FirstOrDefault().ThumbnailImgPath;
                    }

                    if (_ImgPath == null && _ImgPath == "")
                        _ImgPath = _ObjCatMas.CatImg;
                    foreach (var _ObjAU in db.sp_VendorAssociation_SelectWhere(_ConditionForAdminAU).ToList())
                    {
                        FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Msg, _Title, Convert.ToInt32(_ObjAU.RefVendorId), _CCode, _ProdId, EnqId, _ImgPath, "ENQREQ");
                    }
                }
                else
                {
                    var _ObjEnquiry = db.sp_EnquiryList_SelectWhere(" and Id = " + _ObjEnq.Id).FirstOrDefault();
                    var _ObjAU = db.sp_AppUser_Select(_ObjEnquiry.RefAUID).FirstOrDefault();
                    string _CNvalue;
                    string _Title = "";
                    string _ProdId = "";

                    if (_ObjEnquiry.ProdCode != null && _ObjEnquiry.ProdCode != "")
                    {
                        _Title = "Product Enquiry Reply";
                        _CNvalue = _ObjEnquiry.ProdCode + "-" + _ObjEnquiry.ProdName;
                        _ProdId = Convert.ToString(_ObjEnquiry.RefProdId);
                    }
                    else
                    {
                        _CNvalue = _ObjEnquiry.CatCode + "-" + _ObjEnquiry.CatName;
                        _Title = "Catalogue Enquiry Reply";
                    }

                    string _Msg = _ObjEnquiry.VendorName + " had replyed  of your enquiry for " + _CNvalue + ". ";
                    FHubPanel.Controllers.CommanClass.SendAndroidPushNotification(_ObjAU.GCMID, _Msg, _Title, _ObjEnquiry.RefVendorId, _ProdId, _ObjEnquiry.CatCode, Convert.ToString(_ObjEnq.Id), _ObjEnquiry.ThumbnailImgPath, "ENQANS");

                }

                return Json(new { Result = "Success", Code = HttpStatusCode.OK, Data = "", Message = _Message });
            }
            catch (Exception ex)
            {
                CommanClass.ManageError(ex);
                return Json(new { Result = "Exception", Code = HttpStatusCode.BadRequest, Data = "", Message = "Server Error. Try again later!" });
            }
        }

        //// PUT api/enquirylist/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/enquirylist/5
        //public void Delete(int id)
        //{
        //}
    }
}
