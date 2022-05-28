using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class ParameterMappingController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        //
        // GET: /ParameterMapping/
        public ActionResult Index()
        {
            ViewBag.MasterType = "Parameter Mapping";
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "ParameterMapping" && x.ActionName == "Index").FirstOrDefault().MenuIcon;

            ViewData["MasterList"] = CommanClass.GetMasterList();
            ViewData["VendorList"] = CommanClass.GetVendorList((int)Session["VendorId"]);
            CommanClass._VendorId = 0;
            ViewData["CatalogueList"] = CommanClass.GetCatalogList();
            ViewData["MasterValueList"] = CommanClass.GetMasterList(0,(int)Session["VendorId"]);
            return View();
        }

        public PartialViewResult GetCatalogueList(int VendorId)
        {
            try
            {
                CommanClass._VendorId = VendorId;
                ViewData["CatalogueList"] = CommanClass.GetCatalogList();

                return PartialView("CatalogueListPartial");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult MasterValueListPartial(int MasterId, int VendorId, int? CatId)
        {
            try
            {

                #region ParmMapping Logic
                //List<ParameterMappingModel> _ObjValue = new List<ParameterMappingModel>();
                //string _defstr = " and RefMasterId = " + MasterId + " and RefVendorId = " + VendorId;
                //List<string> _ProdVal = new List<string>();
                //List<int> _MasterValId = new List<int>();
                //sp_MasterValue_SelectWhere_Result _ObjMaster = new sp_MasterValue_SelectWhere_Result();
                //ParameterMapping  _ObjParamMap = new ParameterMapping();
                //if (CatId != null)
                //{
                //    if (MasterId == (int)CommanClass.MasterList.Color)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefColor).ToList();
                //    else if (MasterId == (int)CommanClass.MasterList.Size)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefSize).ToList();
                //    else if (MasterId == (int)CommanClass.MasterList.Design)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefDesign).ToList();
                //    else if (MasterId == (int)CommanClass.MasterList.Fabric)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefFabric).ToList();
                //    else if (MasterId == (int)CommanClass.MasterList.ProdType)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefProdType).ToList();
                //    else if (MasterId == (int)CommanClass.MasterList.Brand)
                //        _ProdVal = db.sp_ProductMas_SelectWhere(" and RefCatId = " + CatId).Select(x => x.RefBrand).ToList();

                //    foreach (var _ObjVal in _ProdVal)
                //    {
                //        if (MasterId == (int)CommanClass.MasterList.Color || MasterId == (int)CommanClass.MasterList.Size)
                //        {
                //            foreach (var _Obj in _ObjVal.Split(','))
                //            {
                //                if (_ObjValue.Where(x => x.RefStoreValName == _Obj).ToList().Count == 0)
                //                {
                //                    _ObjMaster = db.sp_MasterValue_SelectWhere(" and RefVendorId =" + VendorId + " and ValueName = '" + _Obj + "' and RefMasterId = " + MasterId).FirstOrDefault();
                //                    _ObjParamMap = db.ParameterMappings.Where(x => x.RefStoreValId == _ObjMaster.ID).FirstOrDefault();
                //                    if (_ObjParamMap)
                //                    //_ObjParamMap = db.ParameterMappings.Where(x => new { x.RefStoreId = VendorId && x.RefStoreValId}) 
                //                    _ObjValue.Add(db.sp_MasterValue_SelectWhere(" and RefVendorId =" + VendorId + " and ValueName = '" + _Obj + "' and RefMasterId = " + MasterId).Select(x => new ParameterMappingModel()
                //                    {
                //                        RefStoreValName = x.ValueName,
                //                        RefStoreValId = x.ID,
                //                        RefStoreValDesc = x.ValueDesc,
                //                        RefVendorValId = _ObjMaster.ID,
                //                        RefVendorValName = _ObjMaster.ValueName,
                //                        RefVendorValDesc = _ObjMaster.ValueDesc,
                //                        MappedStatus = _ObjMaster.ID == null ? "" : "A",
                //                        MappedStatus = _ObjParamMap.Id == null? "U" : "M"
                //                    }).FirstOrDefault());
                //                }

                //            }
                //        }
                //        else
                //        {
                //            //_ObjValue.Add(db.sp_MasterValue_SelectWhere(" and RefVendorId =" + VendorId + " and ValueName = '" + _ObjVal + "' and RefMasterId = " + MasterId).FirstOrDefault());
                //        }
                //    }
                //}
                //else
                //{
                //    //_ObjValue = db.sp_MasterValue_SelectWhere(_defstr).ToList();
                //}
                #endregion

                return PartialView(GetParameterMappingList(MasterId, VendorId, CatId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult Save(int RefMasterId, int VendorId, int StoreValId, int VendorValId, int? CatId, int SelectedValId, string MapStatus)
        {
            try
            {
                int _Id = 0;
                int _PMId = 0;
                if (MapStatus == "U" && SelectedValId == 0)
                {
                    MasterValue _ObjMast = db.MasterValues.Find(StoreValId);
                    _Id = db.sp_MasterValue_Save(0, RefMasterId, (int)Session["VendorId"], _ObjMast.ValueName, _ObjMast.ValueDesc, _ObjMast.OrdNo, _ObjMast.IsActive, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;
                }
                else if (MapStatus == "A" && SelectedValId == 0)
                {
                    _Id = VendorValId;
                }
                else 
                {
                    _Id = SelectedValId;
                }

                _PMId = db.sp_ParameterMapping_Save(0, RefMasterId, (int)Session["VendorId"], VendorId, _Id, StoreValId, (int)Session["VendorId"], CommanClass._Terminal).FirstOrDefault().Value;
                if (_PMId == 0)
                    TempData["Warning"] = "Server Error. Try again later!";

                return PartialView("MasterValueListPartial", GetParameterMappingList(RefMasterId, VendorId, CatId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult Remove(int MasterId, int VendorId, int? CatId, int PMId)
        {
            try
            {
                ParameterMapping _ObjPM = db.ParameterMappings.Find(PMId);
                db.ParameterMappings.Remove(_ObjPM);
                db.SaveChanges();

                return PartialView("MasterValueListPartial", GetParameterMappingList(MasterId, VendorId, CatId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<sp_ParameterMapping_Select_Result> GetParameterMappingList(int MasterId, int VendorId, int? CatId)
        {
            try
            {
                ViewData["MasterValueList"] = CommanClass.GetMasterList(MasterId, (int)Session["VendorId"]);
                List<sp_ParameterMapping_Select_Result> _ObjParamMapping = new List<sp_ParameterMapping_Select_Result>();

                _ObjParamMapping = db.sp_ParameterMapping_Select(VendorId, (int)Session["VendorId"], MasterId, CatId).ToList();

                return _ObjParamMapping;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}