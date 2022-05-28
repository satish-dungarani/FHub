using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FHubPanel.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace FHubPanel.Controllers
{
    public static class CommanClass
    {
        public static int _VendorId = 0, _User = 0, _RoleId = 0;
        public static string _Terminal = "";
        public static DateTime _SinceDate;
        static FHubDBEntities db = new FHubDBEntities();
        public static string _Role = "";
        static string _MSubJect = "", _PlainText = "", _HtmlText ="", _MDesc = "", _MContent = "", _MDescription = "";

        //Setup Master List 
        public enum MasterList
        {
            Brand = 1,
            Color = 2,
            ProdType = 3,
            Size = 4,
            Fabric = 5,
            Design = 6,
            Role = 7
        }

        public static SelectList GetVendorList(int _VendorId)
        {
            try
            {
                List<SelectListItem> _SelectVendor = new List<SelectListItem>();
                string defstr = " and VendorId = " + _VendorId + " and VendorStatus = 'Approved' and StoreStatus = 'Approved' Order By VendorName";
                foreach (var _Obj in db.sp_StoreAssociation_SelectWhere(defstr).ToList())
                {
                    _SelectVendor.Add(new SelectListItem { Selected = false, Text = _Obj.StoreName, Value = _Obj.RefStoreId.ToString() });
                }
                return new SelectList(_SelectVendor, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetContactList(string GroupIdLIst)
        {
            try
            {
                List<SelectListItem> _ContactList = new List<SelectListItem>();
                foreach (var _Obj in db.sp_VendorAssociation_SelectWhere(" and RefVendorId = " + _VendorId + " and VendorStatus = 'Approved' and RefAUId Not in (Select RefAuId from GroupContactList Where RefGroupId in (" + GroupIdLIst + "))"))
                {
                    _ContactList.Add(new SelectListItem { Selected = false, Value = _Obj.RefAUId.ToString(), Text = _Obj.AUName });
                }
                return new SelectList(_ContactList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetContactGroupList()
        {
            try
            {
                List<SelectListItem> _GroupList = new List<SelectListItem>();
                foreach (var _Obj in db.sp_GroupMas_SelectBaseOnVendor("", _VendorId))
                {
                    _GroupList.Add(new SelectListItem { Selected = false, Value = _Obj.GroupId.ToString(), Text = _Obj.GroupName });
                }
                return new SelectList(_GroupList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetRoleList()
        {
            try
            {
                List<SelectListItem> _RoleList = new List<SelectListItem>();
                foreach (var _Obj in db.sp_MasterValue_Select(null, (int)MasterList.Role, 0))
                {
                    _RoleList.Add(new SelectListItem { Selected = false, Value = _Obj.ID.ToString(), Text = _Obj.ValueName });
                }
                return new SelectList(_RoleList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetCatalogList()
        {
            try
            {
                List<SelectListItem> _CatListItem = new List<SelectListItem>();
                int _PageSize = db.sp_CatatlogMas_SelectBaseOnCatId(null).ToList().Count;
                foreach (var _Obj in db.sp_CatalogMas_SelectForAdmin(_VendorId, "", _PageSize, 0))
                {
                    _CatListItem.Add(new SelectListItem { Selected = false, Value = _Obj.CatId.ToString(), Text = _Obj.CatName + " - " + _Obj.CatCode });
                }

                return new SelectList(_CatListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetCatalogList(string _Filter)
        {
            try
            {
                List<SelectListItem> _CatListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_CatalogMas_SelectWhere(_Filter).ToList())
                {
                    _CatListItem.Add(new SelectListItem { Selected = false, Value = _Obj.CatId.ToString(), Text = _Obj.CatName + " - " + _Obj.CatCode });
                }

                return new SelectList(_CatListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetProductCategoryList()
        {
            try
            {
                string _defstr = " and RefVendorId = " + _VendorId;

                List<SelectListItem> _PCListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_ProductCategory_SelectWhere(_defstr))
                {
                    _PCListItem.Add(new SelectListItem { Selected = false, Value = _Obj.ProdCategoryName, Text = _Obj.ProdCategoryName });
                }

                return new SelectList(_PCListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetProductCategoryList(string _Type)
        {
            try
            {
                string _defstr = " and RefVendorId = " + _VendorId;
                if (_Type == "MAIN")
                    _defstr += " and RefPCId Is NULL";

                List<SelectListItem> _PCListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_ProductCategory_SelectWhere(_defstr))
                {
                    _PCListItem.Add(new SelectListItem { Selected = false, Value = _Obj.PCId.ToString(), Text = _Obj.ProdCategoryName });
                }

                return new SelectList(_PCListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetColorList()
        {
            try
            {
                List<SelectListItem> _ColorListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_MasterValue_Select(null, (int)MasterList.Color, _VendorId))
                {
                    _ColorListItem.Add(new SelectListItem { Selected = false, Value = _Obj.ValueName, Text = _Obj.ValueDesc + "/" + _Obj.ValueName });
                }

                return new SelectList(_ColorListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetMasterList(int MasterId)
        {
            try
            {
                List<SelectListItem> _MasterListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_MasterValue_Select(null, MasterId, _VendorId))
                {
                    _MasterListItem.Add(new SelectListItem { Selected = false, Value = _Obj.ValueName, Text = _Obj.ValueName });
                }

                return new SelectList(_MasterListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetMasterList(int MasterId, int VendorId)
        {
            try
            {
                List<SelectListItem> _MasterListItem = new List<SelectListItem>();
                foreach (var _Obj in db.sp_MasterValue_Select(null, MasterId, VendorId))
                {
                    _MasterListItem.Add(new SelectListItem { Selected = false, Value = _Obj.ID.ToString(), Text = _Obj.ValueName });
                }

                return new SelectList(_MasterListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SelectList GetMasterList()
        {
            try
            {
                List<SelectListItem> _MasterList = new List<SelectListItem>();
                foreach (var _Obj in db.MastersLists.Where(x => x.IsSystem == true).ToList())
                {
                    _MasterList.Add(new SelectListItem { Selected = false, Text = _Obj.MasterName, Value = _Obj.Id.ToString() });
                }

                return new SelectList(_MasterList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMail(string _EmailId, string _Subj, string _HtmlBody, string _PlainBody)
        {
            try
            {
                if (_EmailId != "" && _EmailId != null)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(_EmailId);
                    mail.From = new MailAddress("demomail@test.net");
                    mail.Subject = _Subj;
                    mail.Body = _HtmlBody;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(_HtmlBody, new ContentType("text/html")));
                    //mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(_PlainBody, new ContentType("text/plain")));
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "test.net";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential
                    ("demomail@test.net", "demo!@#123");// Enter seders User name and password
                    smtp.EnableSsl = false;
                    smtp.Send(mail);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GenerateMailBody(string _Title, string _MainDesc, string _MainContent, string _Description)
        {
            try
            {
                string _Body = "<html xmlns=\"http://www.w3.org/1999/xhtml\">";
                _Body += "<head>";
                _Body += " <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
                _Body += " <title>Fashion Diva</title>";
                _Body += " <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>";
                _Body += " </head>";
                _Body += " <body style=\"margin: 0; padding: 0;\">";
                _Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">	";
                _Body += " <tr>";
                _Body += " <td style=\"padding: 10px 0 30px 0;\">";
                _Body += " <table align=\"center\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"border: 10px solid ; border-collapse: collapse;\">";
                _Body += " <tr>";
                _Body += " <td align=\"center\" bgcolor=\"#ffffff\" style=\" border: 10px solid ;padding: 40px 0 30px 0; color: #153643; font-size: 28px; font-weight: bold; font-family: Arial, sans-serif; \">";
                _Body += " <img src=\"http://admin.fashiondiva.biz/Content/dist/img/fashiondiva.png\" alt=\"Fashion DIVA\" width=\"350\" height=\"100\" style=\"display: block;\" />";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " <tr>";
                _Body += " <td bgcolor=\"#ffffff\" style=\"padding: 40px 30px 40px 30px;\">";
                _Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">";
                _Body += " <tr>";
                _Body += " <td style=\"color: #153643; font-family: Arial, sans-serif; font-size: 24px;\">";
                _Body += " <b>" + _Title + "</b>";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " <tr>";
                _Body += " <td style=\"padding: 20px 0 5px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;\">";
                _Body += _MainDesc;
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " <tr>";
                _Body += " <td>";
                _Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">";
                _Body += " <tr>";
                _Body += " <td width=\"260\" valign=\"top\">";
                _Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">";
                _Body += " <tr>";
                _Body += " <td>";
                _Body += _MainContent;
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " <tr>";
                _Body += " <td style=\"padding: 12px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 12px; line-height: 20px;\">";
                _Body += _Description;
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " </table>";
                _Body += " </td>";
                //_Body += " <td style=\"font-size: 0; line-height: 0;\" width=\"20\">";
                //_Body += " &nbsp;";
                //_Body += " </td>";
                //_Body += " <td width=\"260\" valign=\"top\">";
                //_Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">";
                //_Body += " <tr>";
                //_Body += " <td>";
                //_Body += " <img src=\"images/right.gif\" alt=\"\" width=\"100%\" height=\"140\" style=\"display: block;\" />";
                //_Body += " </td>";
                //_Body += " </tr>";
                //_Body += " <tr>";
                //_Body += " <td style=\"padding: 25px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;\">";
                //_Body += " Lorem ipsum dolor sit amet, consectetur adipiscing elit. In tempus adipiscing felis, sit amet blandit ipsum volutpat sed. Morbi porttitor, eget accumsan dictum, nisi libero ultricies ipsum, in posuere mauris neque at erat.";
                //_Body += " </td>";
                //_Body += " </tr>";
                //_Body += " </table>";
                //_Body += " </td>";
                _Body += " </tr>";
                _Body += " </table>";
                //_Body += " </td>";
                //_Body += " </tr>";
                //_Body += " </table>";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " <tr>";
                _Body += " <td bgcolor=\"#ff9064\" style=\"padding: 30px 30px 30px 30px;\">";
                _Body += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">";
                _Body += " <tr>";
                _Body += " <td style=\"color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;\" width=\"75%\">";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " </table>";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " </table>";
                _Body += " </td>";
                _Body += " </tr>";
                _Body += " </table>";
                _Body += " </body>";
                _Body += " </html>";

                return _Body;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static bool MailRequestForAssociation(Vendor _Obj, string _ToEmailId,string _Subj, string _SendText,string _Detheader)
        {
            try
            {
                _MSubJect = _Subj;
                _PlainText= "Dear Admin, \n\n" +
                    _SendText + "\n\n" +

                    _Detheader + "\n" +
                    "Company Name       : " + _Obj.VendorName + "\n" +
                    "Contact Person     : " + _Obj.ContactName + "\n" +
                    "Address            : " + _Obj.Address + "\n" +
                    "City               : " + _Obj.City + "\n" +
                    "State              : " + _Obj.State + "\n" +
                    "Country            : " + _Obj.Country + "\n" +
                    "Contact Number     : " + _Obj.MobileNo1 + "\n" +
                    "Email              : " + _Obj.EmailId + "\n\n" +

                    "Regards,\n" +
                    "FashionDiva";
                
                _MDesc = "Dear Admin,<br/>";
                _MContent = _SendText + "<br/><br/>";
                _MDescription = "<table style='width:70%'>" +
                                    "<tr><td><b>" + _Detheader + "</b></td><td></td></tr>" +
                                    "<tr><td>Company Name     : </td><td>" + _Obj.VendorName + "</td></tr>" +
                                    "<tr><td>Contact Person   : </td><td>" + _Obj.ContactName + "</td></tr>" +
                                    "<tr><td>Address          : </td><td>" + _Obj.Address + "</td></tr>" +
                                    "<tr><td>City             : </td><td>" + _Obj.City + "</td></tr>" +
                                    "<tr><td>State            : </td><td>" + _Obj.State + "</td></tr>" +
                                    "<tr><td>Country          : </td><td>" + _Obj.Country + "</td></tr>" +
                                    "<tr><td>Contact Number   : </td><td>" + _Obj.MobileNo1 + "</td></tr>" +
                                    "<tr><td>Email            : </td><td>" + _Obj.EmailId + "</td></tr>" +

                                    "<tr><td><br/><b>Regards, <br/>" +
                                    "FashsionDiva </b></td></table>";

                _HtmlText = GenerateMailBody(_MSubJect, _MDesc, _MContent, _MDescription);
                return SendMail(_ToEmailId, _MSubJect, _HtmlText, _PlainText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool MailOnAction(string _RegardsName, string _Name, string _ToEmailId,string _Subj,string _SendText)
        {
            try
            {
                _MSubJect = _Subj;
                _PlainText = "Dear Vendor " + _Name + ",\n\n" +
                    _SendText + "\n\n" +

                    "Regards,\n" +
                    _RegardsName;

                _MDesc = "Dear Vendor " + _Name + ",<br/>";
                _MContent = _SendText + "<br/><br/>";
                _MDescription = "</td></tr>" +
                                    
                                    "<tr><td><br/><b>Regards, <br/>" +
                                     _RegardsName + "</b></td> ";

                _HtmlText = GenerateMailBody(_MSubJect, _MDesc, _MContent, _MDescription);
                return SendMail(_ToEmailId, _MSubJect, _HtmlText, _PlainText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string SendAndroidPushNotification(string RegistrationID, string pmessage,string ptitle, int pvendorid, string pccode, string pprodid, string penqid,string pimgpath, string plandingpage)
        {

            string GoogleAppID = "AIzaSyB7LtgDwuCt6aAOG3PX8ujBf6ahHK4cwYY";
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/json";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

            var _data = new
            {
                to = RegistrationID,
                data = new
                {
                    title = ptitle,
                    message = pmessage,
                    vendorId = pvendorid,
                    ccode = pccode,
                    prodId = pprodid,
                    enqId = penqid,
                    image = "http://admin.fashiondiva.biz" + pimgpath,
                    landingPage = plandingpage
                },
                time_to_live = 43200
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(_data);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value.ToString() + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + RegistrationID + "";
            //Console.WriteLine(postData);
            //Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();

            HttpWebResponse httpResponse = (HttpWebResponse)tResponse;
            string statusCode = httpResponse.StatusCode.ToString();

            tReader.Close();
            dataStream.Close();
            tResponse.Close();
            return sResponseFromServer;

            //string GoogleAppID = "AIzaSyAlg3nHBKfkv7nAfoj2kynM6tFss7eMVi4";
            //var SENDER_ID = "882886966518";
            //var value = message;
            //WebRequest tRequest;
            //tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            //tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            //tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

            //tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value.ToString() + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + RegistrationID + "";
            ////Console.WriteLine(postData);
            //Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //tRequest.ContentLength = byteArray.Length;

            //Stream dataStream = tRequest.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();

            //WebResponse tResponse = tRequest.GetResponse();

            //dataStream = tResponse.GetResponseStream();

            //StreamReader tReader = new StreamReader(dataStream);

            //String sResponseFromServer = tReader.ReadToEnd();

            //HttpWebResponse httpResponse = (HttpWebResponse)tResponse;
            //string statusCode = httpResponse.StatusCode.ToString();

            //tReader.Close();
            //dataStream.Close();
            //tResponse.Close();
            //return sResponseFromServer;
        }

    }
}