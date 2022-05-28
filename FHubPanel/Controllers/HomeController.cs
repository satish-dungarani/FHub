using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;

namespace FHubPanel.Controllers
{
    public class HomeController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        public ActionResult Index()
        {
            return View(GetDashboardDetail());
        }

        public DashboardModel GetDashboardDetail()
        {
            try
            {
                DashboardModel _ObjDB = new DashboardModel();
                if ((string)Session["Role"] == "Vendor")
                {

                    _ObjDB.ActiveMember = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and VendorStatus = 'Approved'").ToList().Count;
                    _ObjDB.InActiveMember = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and AppUserStatus = 'Deleted'").ToList().Count;
                    _ObjDB.RejectedMember = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and VendorStatus = 'Rejected'").ToList().Count;
                    _ObjDB.TotalMember = db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;

                    _ObjDB.TotalCatalogs = db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;
                    _ObjDB.TotalProducts = db.sp_ProductMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"]).ToList().Count;

                    _ObjDB.ResponseGiven = db.sp_EnquiryList_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and EnquiryRepDate Is Not NULL ").ToList().Count;
                    _ObjDB.ResponsePending = db.sp_EnquiryList_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and EnquiryRepDate Is NULL ").ToList().Count;
                    _ObjDB.TotalInquiry = _ObjDB.ResponseGiven + _ObjDB.ResponsePending;

                    _ObjDB.CateWiseProd = db.sp_ProductCategory_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and RefPCId is NULL").Select(x => new CategoryWiseProduct()
                    {
                        CategoryName = x.ProdCategoryName,
                        NoofProducts = db.sp_ProductMas_SelectWhere(" and RefProdCategory = '" + x.ProdCategoryName + "' and RefVendorId = " + (int)Session["VendorId"]).ToList().Count
                    }).ToList();

                    //Most Active User List
                    //string _Condition = " and RefAUId In (select top 10 RefAUId from AppLog Where LogType = 'Product' OR LogType = 'Catalog' Group by RefAUId,RefVendorId Having RefVendorId = " + (int)Session["VendorId"] + " Order By Count(RefAUId) Desc) and RefVendorId = " + (int)Session["VendorId"] + " Order By ApproveDate";
                    _ObjDB.MostActive = db.sp_VendorAssociation_MostVisitedUser((int)Session["VendorId"]).Distinct().Select(x => new MostActiveUser()
                    {
                        AppUserName = x.AUName,
                        AssDate = x.ApproveDate
                    }).ToList();

                    string _Condition = " and CatId IN (Select top 10 RefId From " +
                                 " (" +
                                 " Select RefId,RefAUId From " +
                                 " (" +
                                 " select RefId,RefAUId,RefVendorId,InsDate" +
                                 " from AppLog" +
                                 " Where LogType = 'Catalog'" +
                                 " and RefVendorId = " + (int)Session["VendorId"] +
                                 " and (InsDate between (getdate() -15) and getdate())" +
                                 " Group By RefAUId,RefId,InsDate,RefVendorID" +
                                 " ) x" +
                                 " Group By RefId,RefAUId" +
                                 " ) z" +
                                 " Group By RefId" +
                                 " Order By Count(RefAUId) Desc)";
                    _ObjDB.MostCat = db.sp_CatalogMas_SelectWhere(_Condition).Select(x => new MostViewCatalog()
                    {
                        CatName = x.CatName,
                        CatCode = x.CatCode,
                        TotalProduct = db.sp_ProductMas_SelectWhere(" and RefCatId =" + x.CatId).ToList().Count
                    }).ToList();

                }
                return _ObjDB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult editSession(int inRefMasterID)
        {
            try
            {
                Session["RefMasterId"] = inRefMasterID;
                return Json(new { Saved = "Yes" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult DailyAppVisitorData()
        {
            try
            {
                int iCounter = 0;
                int iArrayLength = db.sp_AppLog_DailyAppVisitor((int)Session["VendorId"]).ToList().Count;
                dynamic[] _WV = new dynamic[iArrayLength];
                var _WeeklyVisitor = new Dictionary<string, dynamic>();
                foreach (var _Obj in db.sp_AppLog_DailyAppVisitor((int)Session["VendorId"]).OrderBy(x => x.FromToDate).ToList())
                {
                    _WeeklyVisitor = new Dictionary<string, dynamic>();
                    _WeeklyVisitor["name"] = _Obj.FromToDate;
                    _WeeklyVisitor["drilldown"] = _Obj.FromToDate;
                    _WeeklyVisitor["y"] = _Obj.VisitorCount;

                    _WV[iCounter] = _WeeklyVisitor;
                    iCounter++;
                }

                var _DailyVisitor = new Dictionary<string, dynamic>();
                iCounter = 0;
                dynamic[] _DV = new dynamic[iArrayLength];
                foreach (var _Obj in db.sp_AppLog_DailyAppVisitor((int)Session["VendorId"]).ToList())
                {
                    _DailyVisitor = new Dictionary<string, dynamic>();

                    _DailyVisitor["name"] = _Obj.FromToDate;
                    _DailyVisitor["id"] = _Obj.FromToDate;

                    var _DrildownSingledata = new dynamic[2];
                    var _Drildowndata = new dynamic[db.sp_AppLog_DailyAppVisitorPerDay((int)Session["VendorId"]).Where(x => x.RefWeel == _Obj.FromToDate).ToList().Count];
                    int iDCounter = 0;
                    foreach (var _ObjData in db.sp_AppLog_DailyAppVisitorPerDay((int)Session["VendorId"]).Where(x => x.RefWeel == _Obj.FromToDate).OrderBy(x => x.DateValue).ToList())
                    {
                        _DrildownSingledata = new dynamic[2];
                        _DrildownSingledata[0] = _ObjData.DateValue.Value.ToString("dd/MM/yyyy");
                        _DrildownSingledata[1] = _ObjData.VisitorCount;

                        _Drildowndata[iDCounter] = _DrildownSingledata;
                        iDCounter++;
                    }
                    _DailyVisitor["data"] = _Drildowndata;

                    _DV[iCounter] = _DailyVisitor;
                    iCounter++;
                }

                return Json(new { WeeklyVisitors = _WV, DailyVisitor = _DV }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}