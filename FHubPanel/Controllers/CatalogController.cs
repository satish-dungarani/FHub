using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.Entity.Core.Objects;
using OfficeOpenXml;
//using ClosedXML.Excel;

namespace FHubPanel.Controllers
{
    public class CatalogController : BaseController
    {
        FHubDBEntities db = new FHubDBEntities();
        CatalogModel _ObjCat = new CatalogModel();
        int _SuccessUploadCnt = 0, _FailUploadCnt = 0, _RecordCnt = 0;
        //
        // GET: /Catalog/

        public ActionResult Index()
        {
            ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Catalog" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
            ViewBag.MasterType = "Catalogue";
            return View();
        }

        public JsonResult AjaxHandler(string Search, int? pageIndex, int? pageSize)
        {
            try
            {

                //CompanyProfile _ObjCP = db.CompanyProfiles.FirstOrDefault();

                List<CatalogModel> _ObjCatLIst = new List<CatalogModel>();
                _ObjCatLIst = db.sp_CatalogMas_SelectForAdmin((int)Session["VendorId"], Search, pageSize, pageIndex).Select(x => new CatalogModel()
                {
                    CatId = x.CatId,
                    CatCode = x.CatCode,
                    CatName = x.CatName,
                    CatDescription = x.CatDescription,
                    CatImg = x.CatImg,
                    FullImgPath = x.ThumbnailImgPath + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    CatLaunchDate = x.CatLaunchDate,
                    IsActive = Convert.ToBoolean(x.IsActive),
                    IsFullset = Convert.ToBoolean(x.IsFullset),
                    TotalItem = x.TotalProduct
                }).ToList();

                return Json(new
                {
                    Result = "OK",
                    data = _ObjCatLIst,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "ERROR",
                    data = "",
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditSession(int Id)
        {
            try
            {
                Session["CatId"] = Id;
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Manage()
        {
            try
            {
                ViewBag.Icon = db.sp_RetrieveMenuRightsWise_Select(CommanClass._Role).Where(x => x.ControllerName == "Catalog" && x.ActionName == "Index").FirstOrDefault().MenuIcon;
                @ViewBag.MasterType = "Catalogue";
                int CatId = 0;
                if (Session["CatId"] != null && Session != null)
                    CatId = (int)Session["CatId"];

                if (CatId != 0)
                {
                    //CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    //_ObjComp.FolderPath = _ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Original/";
                    _ObjCat = db.sp_CatatlogMas_SelectBaseOnCatId(CatId).Select(x => new CatalogModel()
                    {
                        CatId = x.CatId,
                        RefVendorId = x.RefVendorId,
                        CatCode = x.CatCode,
                        CatName = x.CatName,
                        CatDescription = x.CatDescription,
                        CatLaunchDate = x.CatLaunchDate,
                        CatImg = x.CatImg,
                        FullImgPath = x.CatImg != "" && x.CatImg != null ? x.OriginalImgPath : @"\Content\dist\img\CatalogueNoImage.png",
                        IsFullset = Convert.ToBoolean(x.IsFullset),
                        IsActive = Convert.ToBoolean(x.IsActive),
                        InsUser = x.InsUser,
                        InsDate = x.InsDate,
                        InsTerminal = x.InsTerminal,
                        UpdUser = x.UpdUser,
                        UpdDate = x.UpdDate,
                        UpdTerminal = x.UpdTerminal
                    }).FirstOrDefault();
                }

                Session["CatId"] = null;

                return View(_ObjCat);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Server Error.Please try again later!");
            }
        }

        public ActionResult Save(CatalogModel _ObjParam, HttpPostedFileBase fileLogo)
        {
            try
            {

                if (fileLogo != null)
                {
                    string _Ext = fileLogo.ContentType.Split('/')[1];
                    if (_Ext != "jpg" && _Ext != "jpeg" && _Ext != "png")
                    {
                        Session["CatId"] = _ObjParam.CatId;
                        TempData["Warning"] = "Select proper image file!";
                        return RedirectToAction("Manage");
                    }
                }

                int Result = db.sp_CatalogMas_Save(_ObjParam.CatId, (int)Session["VendorId"], _ObjParam.CatCode, _ObjParam.CatName,
                    _ObjParam.CatDescription, _ObjParam.CatImg, _ObjParam.CatLaunchDate, _ObjParam.IsFullset, _ObjParam.IsActive,
                    CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                if (Result != 0)
                {
                    if (_ObjParam.CatId == 0)
                    {
                        TempData["Success"] = "Catalogue Successfully created.";
                    }
                    else
                    {
                        TempData["Success"] = "Catalogue Successfully update.";
                    }

                    if (fileLogo != null)
                    {
                        CompanyProfile _Objcomp = db.CompanyProfiles.FirstOrDefault();
                        string _Path = Server.MapPath(_Objcomp.FolderPath + (int)Session["VendorId"] + "/Catalog/");
                        if (System.IO.File.Exists(_Path + "Original/" + _ObjParam.CatImg))
                        {
                            System.IO.File.Delete(_Path + "Original/" + _ObjParam.CatImg);
                            System.IO.File.Delete(_Path + "Thumbnail/" + _ObjParam.CatImg);
                        }
                            

                        if (Directory.Exists(_Path))
                        {
                            string _FileName = fileLogo.FileName.Substring(0, fileLogo.FileName.LastIndexOf('.'));
                            if (_FileName.Length > 15)
                                _FileName = _FileName.Substring(0, 15);

                            _FileName += "-" + (int)Session["VendorId"] + "-" + System.DateTime.Now.ToString("hhmmssfff");
                            string _Extension = fileLogo.ContentType.Split('/')[1];
                            fileLogo.SaveAs(_Path + "Original/" + _FileName + "." + _Extension);

                            System.Drawing.Image _Img = System.Drawing.Image.FromFile(_Path + "Original/" + _FileName + "." + _Extension);
                            //System.Drawing.Image ThumbImg = _Img.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                            //ThumbImg.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension);
                            //_Img.Dispose();

                            var thumbnailBit = new Bitmap(CataWidth, CataHegiht);
                            var thumbnailGraph = Graphics.FromImage(thumbnailBit);
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            var imageRectangle = new Rectangle(0, 0, CataWidth, CataHegiht);
                            thumbnailGraph.DrawImage(_Img, imageRectangle);
                            thumbnailBit.Save(_Path + "Thumbnail/" + _FileName + "." + _Extension, _Img.RawFormat);
                            _Img.Dispose();

                            CatalogMa _ObjPCat = db.CatalogMas.Find(Result);
                            _ObjPCat.CatImg = _FileName + "." + _Extension;
                            _ObjPCat.RefStoreId = null;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    TempData["Success"] = "Server Error. Please try again later!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Session["PCId"] = _ObjParam.PCId;
                TempData["Error"] = "Server Error. Please try again later!";
                return RedirectToAction("Index");
            }
        }


        //Delete product category 
        public JsonResult Delete(int Id)
        {
            try
            {
                CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                CatalogMa _Obj = db.CatalogMas.Find(Id);

                if (_Obj == null)
                    return Json(new { _result = true, _Message = "No Data Found!" }, JsonRequestBehavior.AllowGet);

                if (db.sp_ProductMas_SelectWhere(" and RefCatId = '" + Id + "'").ToList().Count > 0)
                    return Json(new { _result = false, _Message = "Catalogue already in used. You can't delete this Catalogue!" }, JsonRequestBehavior.AllowGet);

                db.CatalogMas.Remove(_Obj);
                db.SaveChanges();

                if (System.IO.File.Exists(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Original/" + _Obj.CatImg))
                {
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Original/" + _Obj.CatImg));
                    System.IO.File.Delete(Server.MapPath(_ObjComp.FolderPath + (int)Session["VendorId"] + "/Catalog/Thumbnail/" + _Obj.CatImg));
                }

                db.sp_DeleteLog_Save((int)Session["VendorId"], "Catalog", _Obj.CatId, CommanClass._Terminal);

                return Json(new { _result = true, _Message = "Successfully Deleted!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { _result = false, _Message = "Server Error. Please try again later!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Check Duplicate value
        public JsonResult isValueExists(string pCatalogCode)
        {
            try
            {
                int RCounter = db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and CatCode = '" + pCatalogCode + "'").ToList().Count;
                if (RCounter > 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult FileFormatDownload()
        {
            try
            {
                byte[] _FileByte = System.IO.File.ReadAllBytes(Server.MapPath("/Content/CatalogBulkUpload.xlsx"));
                return File(_FileByte, System.Net.Mime.MediaTypeNames.Application.Octet, "CatalogBulkUpload.xlsx");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BulkCatalogData(HttpPostedFileBase ExcelFile)
        {
            string FilePath = "";
            try
            {
                if (ExcelFile != null)
                {
                    string _Extention = ExcelFile.ContentType.Split('/')[1];
                    if (_Extention != "vnd.openxmlformats-officedocument.spreadsheetml.sheet" && _Extention != "vnd.ms-excel")
                        throw new Exception("Please select proper file.");

                    //CompanyProfile _ObjComp = db.CompanyProfiles.FirstOrDefault();
                    //string _ServerPath = Server.MapPath(_ObjComp.FolderPath + CommanClass._VendorId);
                    //if (Directory.Exists(_ServerPath))
                    //    ExcelFile.SaveAs(_ServerPath + "/" + ExcelFile.FileName);
                    //else
                    //    throw new Exception("Invalid User.");

                    //FilePath = _ServerPath + "/" + ExcelFile.FileName;
                    BulkSave(ExcelFile);
                }

                //if (System.IO.File.Exists(FilePath))
                //    System.IO.File.Delete(FilePath);

                TempData["Success"] = "Total Catalogue " + _RecordCnt + " and Success fully uploded " + _SuccessUploadCnt + " and Fail to upload " + _FailUploadCnt;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //if (System.IO.File.Exists(FilePath))
                //    System.IO.File.Delete(FilePath);
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
                //throw ex;
            }
        }

        public DataTable ExcelToDatatable(HttpPostedFileBase ExcelFile)
        {
            DataTable _Dt = new DataTable();
            try
            {
                if ((ExcelFile != null) && (ExcelFile.ContentLength > 0) && !string.IsNullOrEmpty(ExcelFile.FileName))
                {
                    DateTime _date;
                    bool _bool;
                    string fileName = ExcelFile.FileName;
                    string fileContentType = ExcelFile.ContentType;
                    byte[] fileBytes = new byte[ExcelFile.ContentLength];
                    var data = ExcelFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(ExcelFile.ContentLength));
                    using (var package = new ExcelPackage(ExcelFile.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        if (noOfCol != 5)
                            throw new Exception("Invalid columns available!");

                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            if (rowIterator == 1)
                            {
                                _Dt.Columns.Add("CatCode", typeof(string));
                                _Dt.Columns.Add("CatName", typeof(string));
                                _Dt.Columns.Add("CatDescription", typeof(string));
                                _Dt.Columns.Add("CatLaunchDate", typeof(DateTime));
                                _Dt.Columns.Add("IsFullSet", typeof(bool));
                            }
                            else
                            {


                                _Dt.Rows.Add();
                                if (workSheet.Cells[rowIterator, 1].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 1].Value.ToString()))
                                    throw new Exception("Enter Catalogue Code at Line No. " + (_RecordCnt + 2));
                                else if (workSheet.Cells[rowIterator, 1].Value.ToString().IndexOf('-') != -1 || workSheet.Cells[rowIterator, 1].Value.ToString().IndexOf('_') != -1)
                                    throw new Exception("In Catalogue Code '-' and '_' not allowed. Invalid format at Line No. " + (_RecordCnt + 2));
                                else if (_Dt.Select(" CatCode = '" + workSheet.Cells[rowIterator, 1].Value.ToString() + "'").ToList().Count > 0)
                                    throw new Exception("Catalogue code " + _Dt.Rows[_Dt.Rows.Count - 1][0].ToString() + " is already exists in excel file at Line No. " + (_RecordCnt + 2));
                                _Dt.Rows[_Dt.Rows.Count - 1][0] = workSheet.Cells[rowIterator, 1].Value.ToString();

                                if (workSheet.Cells[rowIterator, 2].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 2].Value.ToString()))
                                    throw new Exception("Enter Catalogue Name at Line No. " + (_RecordCnt + 2));
                                _Dt.Rows[_Dt.Rows.Count - 1][1] = workSheet.Cells[rowIterator, 2].Value.ToString();

                                if (workSheet.Cells[rowIterator, 3].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 3].Value.ToString()))
                                    _Dt.Rows[_Dt.Rows.Count - 1][2] = null;
                                else
                                    _Dt.Rows[_Dt.Rows.Count - 1][2] = workSheet.Cells[rowIterator, 3].Value.ToString();

                                if (workSheet.Cells[rowIterator, 4].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 4].Value.ToString()))
                                    throw new Exception("Enter Launch Date at Line No. " + (_RecordCnt + 2));
                                else if (!DateTime.TryParse(workSheet.Cells[rowIterator, 4].Value.ToString(), out _date))
                                    throw new Exception("Launch Date not in correct format at Line No. " + (_RecordCnt + 2));
                                _Dt.Rows[_Dt.Rows.Count - 1][3] = Convert.ToDateTime(workSheet.Cells[rowIterator, 4].Value);

                                if (workSheet.Cells[rowIterator, 5].Value == null || string.IsNullOrEmpty(workSheet.Cells[rowIterator, 5].Value.ToString()) || workSheet.Cells[rowIterator, 5].Value.ToString().ToUpper() == "NO")
                                    workSheet.Cells[rowIterator, 5].Value = false;
                                else if (workSheet.Cells[rowIterator, 5].Value.ToString().ToUpper() == "YES")
                                    workSheet.Cells[rowIterator, 5].Value = true;
                                //throw new Exception("Enter Full Set at Line No. " + (_RecordCnt + 1));
                                //else if (!bool.TryParse(workSheet.Cells[rowIterator, 5].Value.ToString(), out _bool))
                                //    throw new Exception("Full Set value is not in correct format at Line No. " + (_RecordCnt + 1));
                                _Dt.Rows[_Dt.Rows.Count - 1][4] = Convert.ToBoolean(workSheet.Cells[rowIterator, 5].Value);


                                if (db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and CatCode = '" + _Dt.Rows[_Dt.Rows.Count - 1][0].ToString() + "'").ToList().Count > 0)
                                    throw new Exception("Catalogue code " + _Dt.Rows[_Dt.Rows.Count - 1][0].ToString() + " is already exists in table at Line No. " + (_RecordCnt + 2));

                                _RecordCnt++;
                            }
                        }
                    }
                }

                #region "Excel upload by connection string"
                //string FileExtention = FilePath.Substring(FilePath.LastIndexOf('.') + 1, (FilePath.Length - FilePath.LastIndexOf('.')) - 1);
                //string _CnnStr = string.Empty;
                //if (FileExtention.ToLower() == "xls")
                //{
                //    _CnnStr = ConfigurationManager.ConnectionStrings["ConnectionForxls"].ConnectionString;
                //}
                //else if (FileExtention.ToLower() == "xlsx")
                //{
                //    _CnnStr = ConfigurationManager.ConnectionStrings["ConnectionForxlsx"].ConnectionString;
                //}

                //_CnnStr = string.Format(_CnnStr, FilePath);
                //OleDbConnection _Con = new OleDbConnection(_CnnStr);
                //OleDbCommand _Cmd = new OleDbCommand();
                //OleDbDataAdapter _Da = new OleDbDataAdapter();

                //// get name of first sheet
                //_Con.Open();
                //DataTable _DtExcelShema;
                //_DtExcelShema = _Con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //string SheetName = _DtExcelShema.Rows[0]["TABLE_NAME"].ToString();
                //_Con.Close();


                //_Dt.Columns.Add("CatCode", typeof(string));
                //_Dt.Columns.Add("CatName", typeof(string));
                //_Dt.Columns.Add("CatDescription", typeof(string));
                //_Dt.Columns.Add("CatLaunchDate", typeof(DateTime));
                //_Dt.Columns.Add("IsFullSet", typeof(bool));

                ////read data from first sheet
                //_Con.Open();
                //_Cmd.CommandText = "Select *  From [" + SheetName + "]";
                //_Cmd.Connection = _Con;
                //_Da.SelectCommand = _Cmd;
                //_Da.Fill(_Dt);
                //_Con.Close();
                #endregion

                #region "Excel data read manualy"
                //using (XLWorkbook workBook = new XLWorkbook(FilePath))
                //{
                //    //Read the first Sheet from Excel file.
                //    IXLWorksheet workSheet = workBook.Worksheet(1);

                //    //Create a new DataTable.
                //    //DataTable _Dt = new DataTable();

                //    //Loop through the Worksheet rows.
                //    bool firstRow = true;
                //    foreach (IXLRow row in workSheet.Rows())
                //    {
                //        //Use the first row to add columns to DataTable.
                //        if (firstRow)
                //        {
                //            _Dt.Columns.Add("CatCode", typeof(string));
                //            _Dt.Columns.Add("CatName", typeof(string));
                //            _Dt.Columns.Add("CatDescription", typeof(string));
                //            _Dt.Columns.Add("CatLaunchDate", typeof(DateTime));
                //            _Dt.Columns.Add("IsFullSet", typeof(bool));
                //            //foreach (IXLCell cell in row.Cells())
                //            //{
                //            //    _Dt.Columns.Add(cell.Value.ToString());
                //            //}
                //            firstRow = false;
                //        }
                //        else
                //        {
                //            //Add rows to DataTable.
                //            _Dt.Rows.Add();
                //            int i = 0;
                //            foreach (IXLCell cell in row.Cells())
                //            {
                //                _Dt.Rows[_Dt.Rows.Count - 1][i] = cell.Value.ToString();
                //                i++;
                //            }
                //        }

                //    }
                //}

                //using (SpreadsheetDocument doc = SpreadsheetDocument.Open(FilePath, false))
                //{
                //    //Read the first Sheet from Excel file.
                //    Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                //    //Get the Worksheet instance.
                //    Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                //    //Fetch all the rows present in the Worksheet.
                //    IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                //    //Create a new DataTable.
                //    //DataTable dt = new DataTable();

                //    //Loop through the Worksheet rows.
                //    foreach (Row row in rows)
                //    {
                //        //Use the first row to add columns to DataTable.
                //        if (row.RowIndex.Value == 1)
                //        {
                //            _Dt.Columns.Add("CatCode", typeof(string));
                //            _Dt.Columns.Add("CatName", typeof(string));
                //            _Dt.Columns.Add("CatDescription", typeof(string));
                //            _Dt.Columns.Add("CatLaunchDate", typeof(DateTime));
                //            _Dt.Columns.Add("IsFullSet", typeof(bool));
                //            //foreach (Cell cell in row.Descendants<Cell>())
                //            //{
                //            //    _Dt.Columns.Add(GetValue(doc, cell));
                //            //}
                //        }
                //        else
                //        {
                //            //Add rows to DataTable.
                //            _Dt.Rows.Add();
                //            int i = 0;
                //            foreach (Cell cell in row.Descendants<Cell>())
                //            {
                //                _Dt.Rows[_Dt.Rows.Count - 1][i] = GetValue(doc, cell);
                //                i++;
                //            }
                //        }
                //    }

                //}
                #endregion

                return _Dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //private string GetValue(SpreadsheetDocument doc, Cell cell)
        //{
        //    string value = cell.CellValue.InnerText;
        //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        //    {
        //        return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
        //    }
        //    return value;
        //}

        public void BulkSave(HttpPostedFileBase ExcelFile)
        {
            try
            {
                bool _rsltval = false;
                DataTable _Dt = ExcelToDatatable(ExcelFile);
                //foreach (DataRow _Dr in _Dt.Rows)
                //{
                //    if (string.IsNullOrEmpty(_Dr["CatCode"].ToString()))
                //        throw new Exception("Enter Catalog Code at Line No. " + (_RecordCnt + 1));
                //    else if (string.IsNullOrEmpty(_Dr["CatName"].ToString()))
                //        throw new Exception("Enter Catalog Name at Line No. " + (_RecordCnt + 1));
                //    else if (string.IsNullOrEmpty(_Dr["CatLaunchDate"].ToString()))
                //        throw new Exception("Enter Launch Date at Line No. " + (_RecordCnt + 1));
                //    else if (!DateTime.TryParse(_Dr["CatLaunchDate"].ToString(), out _date))
                //        throw new Exception("Launch Date not in correct format at Line No. " + (_RecordCnt + 1));
                //    else if (!bool.TryParse(_Dr["IsFullSet"].ToString(), out _bool))
                //        throw new Exception("Full Set value is not in correct format at Line No. " + (_RecordCnt + 1));
                //    else if (db.sp_CatalogMas_SelectWhere(" and RefVendorId = " + (int)Session["VendorId"] + " and CatCode = '" + _Dr["CatCode"].ToString() + "'").ToList().Count > 0)
                //        throw new Exception("Catalog code " + _Dr["CatCode"].ToString() + " is already exists in table at Line No. " + (_RecordCnt + 1));

                //    _RecordCnt++;
                //}

                foreach (DataRow _Dr in _Dt.Rows)
                {
                    _rsltval = db.sp_CatalogMas_Save(0, (int)Session["VendorId"], _Dr["CatCode"].ToString(), _Dr["CatName"].ToString(),
                                    _Dr["CatDescription"].ToString(), null, Convert.ToDateTime(_Dr["CatLaunchDate"]),
                                    string.IsNullOrEmpty(_Dr["IsFullset"].ToString()) ? false : Convert.ToBoolean(_Dr["IsFullset"]), true,
                                    CommanClass._User, CommanClass._Terminal).FirstOrDefault().HasValue;

                    if (_rsltval)
                        _SuccessUploadCnt++;
                    else
                        _FailUploadCnt++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}