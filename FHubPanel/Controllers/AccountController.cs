using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using FHubPanel.Models;
using System.IO;

namespace FHubPanel.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        FHubDBEntities db = new FHubDBEntities();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }


        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
        //    //clsCommonUI._Terminal = "test";
        //    //clsCommonUI._Terminal = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]).HostName;
        //    clsCommonUI._Terminal = System.Net.Dns.GetHostName();
        //}

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            Session.RemoveAll();
            ViewBag.ReturnUrl = returnUrl;

            LoginViewModel Model = new LoginViewModel();
            if (Request.Cookies["VendorCookie"] != null)
            {
                Model.RememberMe = true;
                Model.UserName = Request.Cookies["VendorCookie"]["UserName"];
                Model.Password = Request.Cookies["VendorCookie"]["VendorPwd"];
            }
            return View(Model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.sp_VendorSubDet_SelectWhere(" and UserName = '" + model.UserName + "' Order by ValidTodate Desc").ToList().Count == 1)
                {
                    DateTime? _ValidDate = db.sp_VendorSubDet_SelectWhere(" and UserName = '" + model.UserName + "' Order by ValidTodate Desc").FirstOrDefault().ValidTodate.Value;
                    if (_ValidDate != null)
                    {
                        if (Convert.ToDateTime(_ValidDate).CompareTo(System.DateTime.Today) < 0)
                        {
                            TempData["Warning"] = "Your account validity is expired. Please contect to your admin to increase validity!";
                            return View(model);
                        }
                    }
                }

                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (model.RememberMe)
                    {
                        Response.Cookies["VendorCookie"]["UserName"] = model.UserName;
                        Response.Cookies["VendorCookie"]["VendorPwd"] = model.Password;
                        Response.Cookies["VendorCookie"].Expires = System.DateTime.Now.AddYears(1);
                    }
                    await SignInAsync(user, model.RememberMe);
                    Session["UserName"] = model.UserName;
                    sp_Vendor_SelectWhere_Result _ObjVendor = db.sp_Vendor_SelectWhere(" and UserName = '" + model.UserName + "'").FirstOrDefault();
                    if (_ObjVendor == null)
                    {
                        Session["Role"] = CommanClass._Role = "Admin";
                        Session["VendorId"] = 0;
                    }
                    else
                    {
                        string _MainPath = db.CompanyProfiles.FirstOrDefault().FolderPath;
                        Session["SinceDate"] = _ObjVendor.InsDate.ToString("MMM. yyyy");
                        Session["VendorCode"] = _ObjVendor.VendorCode;
                        Session["ReferalCode"] = _ObjVendor.ReferalCode;
                        Session["VendorName"] = _ObjVendor.VendorName;
                        Session["VendorImg"] = string.IsNullOrEmpty(_ObjVendor.LogoImg) ? "/Content/dist/img/no_image_available.jpg" : _MainPath + _ObjVendor.VendorId.ToString() + "/Thumb" + _ObjVendor.LogoImg;
                        Session["VendorId"] = _ObjVendor.VendorId;
                        CommanClass._User = _ObjVendor.VendorId;
                        Session["Role"] = CommanClass._Role = "Vendor";

                    }

                    CommanClass._RoleId = db.sp_MasterValue_SelectWhere(" and RefMasterId = " + (int)CommanClass.MasterList.Role + " and RefVendorId = 0 and ValueName = '" + CommanClass._Role + "'").FirstOrDefault().ID;

                    Session["AccessMenuList"] = LoadMenuWithModel(CommanClass._Role);
                    Session["MenuList"] = LoadMenuWithModel(CommanClass._Role);
                    CommanClass._Terminal = "127.0.0.1";
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    if (db.sp_Vendor_SelectWhere(" and UserName = '" + model.UserName + "'").ToList().Count != 1)
                        TempData["Warning"] = "Invalid User Name.";
                    else
                        TempData["Warning"] = "Invalid Password.";
                }
            }
            return View(model);
        }

        //load rights wise menu for display
        public List<MenuMasterModel> LoadMenuWithModel(string pRoleName)
        {
            List<MenuMasterModel> _ObjMenu = new List<MenuMasterModel>();
            _ObjMenu = db.sp_RetrieveMenuRightsWise_Select(pRoleName).Select(x => new MenuMasterModel
            {
                Id = x.ID,
                MenuName = x.MenuName,
                MenuDes = x.MenuDes,
                IsActive = x.IsActive,
                ParentManuId = x.ParentMenuID,
                OrderNo = x.OrderNo,
                ConstrollerName = x.ControllerName,
                MenuIcon = x.MenuIcon,
                ActionName = x.ActionName,
                MenuPath = x.MenuPath
            }).OrderBy(x => x.OrderNo).ToList();

            return _ObjMenu;
        }



        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            VendorModel _Obj = new VendorModel();
            _Obj.SubscriptionType = "Free";
            return View(_Obj);
        }

        //Check Duplicate value
        public JsonResult isValueExists(string pUserName)
        {
            try
            {
                int RCounter = db.sp_Vendor_Select(null).Where(x => x.UserName.ToUpper() == pUserName.ToUpper()).ToList().Count;
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

        //POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(VendorModel _ObjParam)
        {
            try
            {
                bool _registerstatus = true;
                //if (ModelState.IsValid)
                //{
                if (_ObjParam != null)
                {

                    if (_ObjParam.SubscriptionType != "" && _ObjParam.SubscriptionType != null)
                    {
                        if (db.sp_Subscription_SelectBaseOnType(_ObjParam.SubscriptionType).ToList().Count != 1)
                        {
                            //TempData["Error"] = "Wrong subscription type!";
                            return RedirectToAction("Register");
                        }
                    }
                    else
                        return RedirectToAction("Register");

                    if (_ObjParam.ReferenceCode != "" && _ObjParam.ReferenceCode != null)
                    {
                        if (db.sp_Vendor_SelectWhere(" and ReferalCode = '" + _ObjParam.ReferenceCode + "'").ToList().Count != 1)
                        {
                            TempData["Error"] = "Wrong reference code!";
                            return View(_ObjParam);
                        }
                    }

                    if (_ObjParam.VendorId == 0)
                    {
                        var user = new ApplicationUser() { UserName = _ObjParam.UserName };
                        var result = await UserManager.CreateAsync(user, _ObjParam.Password);
                        if (result.Succeeded)
                            _registerstatus = true;
                        else
                            _registerstatus = false;
                    }

                    if (_registerstatus)
                    {
                        CommanClass._User = 0;
                        CommanClass._Terminal = "127.0.0.1";
                        //SignInAsync(user, isPersistent: false);
                        int Result = db.sp_Vendor_Save(_ObjParam.VendorId, _ObjParam.VendorName, _ObjParam.UserName, _ObjParam.VendorCode, _ObjParam.Address, _ObjParam.Landmark, _ObjParam.Country,
                                _ObjParam.State, _ObjParam.City, _ObjParam.Pincode, _ObjParam.ContactName, _ObjParam.ContactNo1, _ObjParam.ContactNo2,
                                _ObjParam.MobileNo1, _ObjParam.MobileNo2, _ObjParam.FaxNo, _ObjParam.EmailId, _ObjParam.WebSite, _ObjParam.LogoImg, _ObjParam.IsActive,
                                _ObjParam.SubscriptionType, _ObjParam.ReferenceCode, _ObjParam.AboutUs, _ObjParam.ProdDispName, _ObjParam.CatDispName,
                                _ObjParam.BGImage, CommanClass._User, CommanClass._Terminal).FirstOrDefault().Value;

                        if (Result != 0)
                        {
                            CompanyProfile _ObjCompProf = db.CompanyProfiles.FirstOrDefault();
                            string _Path = Server.MapPath(_ObjCompProf.FolderPath + Result);
                            if (_ObjParam.VendorId == 0)
                            {
                                if (!Directory.Exists(_Path))
                                {
                                    Directory.CreateDirectory(_Path);
                                    Directory.CreateDirectory(_Path + "/Catalog");
                                    Directory.CreateDirectory(_Path + "/Catalog/Original");
                                    Directory.CreateDirectory(_Path + "/Catalog/Thumbnail");
                                    Directory.CreateDirectory(_Path + "/Category");
                                    Directory.CreateDirectory(_Path + "/Category/Original");
                                    Directory.CreateDirectory(_Path + "/Category/Thumbnail");
                                    Directory.CreateDirectory(_Path + "/Products");
                                    Directory.CreateDirectory(_Path + "/Products/Original");
                                    Directory.CreateDirectory(_Path + "/Products/Thumbnail");
                                    Directory.CreateDirectory(_Path + "/Slider");
                                    Directory.CreateDirectory(_Path + "/Slider/Original");
                                    Directory.CreateDirectory(_Path + "/Slider/Thumbnail");
                                }
                                //string _Extention = fileLogo.FileName.Split('.')[1];
                                //string _FullPath = Path.Combine(_Path, Result + "." + _Extention);
                                //fileLogo.SaveAs(_FullPath);

                                //Vendor _ObjVendorUpd = db.Vendors.Find(Result);
                                //_ObjVendorUpd.LogoImg = Result + "/" + Result + "." + _Extention;
                                //db.SaveChanges();

                                Session["UserName"] = _ObjParam.UserName;
                                Session["SinceDate"] = System.DateTime.Now.Date.ToString("MMM. yyyy");
                                Session["VendorCode"] = db.sp_Vendor_Select(Result).FirstOrDefault().VendorCode;
                                Session["ReferalCode"] = db.sp_Vendor_Select(Result).FirstOrDefault().ReferalCode;
                                Session["VendorImg"] = "/Content/dist/img/no_image_available.jpg";
                                Session["VendorName"] = db.sp_Vendor_Select(Result).FirstOrDefault().VendorName;
                                Session["VendorId"] = Result;
                                CommanClass._User = Result;
                                CommanClass._Role = "Vendor";
                                Session["AccessMenuList"] = LoadMenuWithModel(CommanClass._Role);
                                Session["MenuList"] = LoadMenuWithModel(CommanClass._Role);

                                TempData["Success"] = "Vendor Register Successfully.";
                            }
                            else
                            {
                                //string _Extention = fileLogo.FileName.Split('.')[1];
                                //string _FullPath = Path.Combine(_Path, Result + "." + _Extention);
                                //fileLogo.SaveAs(_FullPath);
                                TempData["Success"] = "Vendor Profile Updated.";
                            }

                            //if (fileLogo != null)
                            //{
                            //    string _Extention = fileLogo.FileName.Split('.')[1];
                            //    string _FullPath = Path.Combine(_Path, Result + "." + _Extention);
                            //    fileLogo.SaveAs(_FullPath);

                            //    System.Drawing.Image _Image = System.Drawing.Image.FromFile(_FullPath);

                            //    var thumbnailbit = new Bitmap(VendorWidth, VendorHeight);
                            //    var thumbnailgraphic = Graphics.FromImage(thumbnailbit);
                            //    thumbnailgraphic.CompositingQuality = CompositingQuality.HighQuality;
                            //    thumbnailgraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            //    thumbnailgraphic.SmoothingMode = SmoothingMode.HighQuality;


                            //    var imageRectangle = new Rectangle(0, 0, VendorWidth, VendorHeight);
                            //    thumbnailgraphic.DrawImage(_Image, imageRectangle);
                            //    thumbnailbit.Save(Path.Combine(_Path, "Thumb" + Result + "." + _Extention));
                            //    _Image.Dispose();

                            //    Vendor _ObjVendorUpd = await db.Vendors.FindAsync(Result);
                            //    _ObjVendorUpd.LogoImg = Result + "." + _Extention;
                            //    db.SaveChanges();
                            //}
                        }
                        else
                            TempData["Error"] = "Server Error. Please try again later!";
                    }
                    else
                    {
                        TempData["Error"] = "Vendor registration failed. Please contact to administrator!";
                    }
                }
                //}

                if (CommanClass._Role == "Vendor" && CommanClass._Role != "")
                    return RedirectToAction("Index", "Home");
                else if (CommanClass._Role == "Admin" && CommanClass._Role != "")
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Server Error. Please try again later!";
                return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool _result = false;
        //        if (model.RegType.ToString().ToUpper() == "REGISTER")
        //        {

        //            var user = new ApplicationUser() { UserName = model.UserName };
        //            var result = await UserManager.CreateAsync(user, model.Password);
        //            if (result.Succeeded)
        //            {
        //                model.UpdUser = clsCommonUI._User;
        //                model.UpdTerminal = clsCommonUI._Terminal;

        //                _result = Convert.ToBoolean(_objUserMaster.SetUserRole(model.UserId, model.RefRoleId, model.UserName, model.UpdUser, model.UpdTerminal));


        //                if (_result)
        //                {

        //                    TempData["Success"] = "User Successfully Registerd.";
        //                    return RedirectToAction("Index", "User");
        //                }
        //                else
        //                {
        //                    TempData["Warning"] = "User already exiest!";
        //                    return RedirectToAction("Index", "User");
        //                }

        //            }
        //            else
        //            {
        //                TempData["Warning"] = "User already exiest!";
        //                return RedirectToAction("Index", "User");
        //            }
        //        }
        //        else
        //        {
        //            //User does not have a password so remove any validation errors caused by a missing OldPassword field
        //            ModelState state = ModelState["OldPassword"];
        //            if (state != null)
        //            {
        //                state.Errors.Clear();
        //            }

        //            if (ModelState.IsValid)
        //            {
        //                var _UserIdentity = await UserManager.FindByNameAsync(model.UserName);
        //                if (_UserIdentity != null)
        //                {
        //                    string _HasherPassword = UserManager.PasswordHasher.HashPassword(model.Password);
        //                    UserStore<ApplicationUser> _Store = new UserStore<ApplicationUser>();
        //                    await _Store.SetPasswordHashAsync(_UserIdentity, _HasherPassword);
        //                    IdentityResult Resetresult = await UserManager.UpdateAsync(_UserIdentity);
        //                    if (Resetresult.Succeeded)
        //                    {
        //                        model.UpdUser = clsCommonUI._User;
        //                        model.UpdTerminal = clsCommonUI._Terminal;

        //                        _result = Convert.ToBoolean(_objUserMaster.SetUserRole(model.UserId, model.RefRoleId, model.UserName, model.UpdUser, model.UpdTerminal));


        //                        if (_result)
        //                        {
        //                            TempData["Success"] = "User Password successfully reset.";
        //                            return RedirectToAction("Index", "User");
        //                        }
        //                        else
        //                        {
        //                            TempData["Warning"] = "You can't reset user password!";
        //                            return RedirectToAction("Index", "User");
        //                        }

        //                        //return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //                    }
        //                    else
        //                    {
        //                        TempData["Warning"] = "You can't reset user password!";
        //                        return RedirectToAction("Index", "User");
        //                    }
        //                }


        //            }
        //            return View("Index", "User");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return RedirectToAction("Index", "User");
        //}

        ////
        //// POST: /Account/Disassociate


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    ViewBag.HasLocalPassword = HasPassword();
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        ////
        //// POST: /Account/Manage
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = model.OldPassword == null ? false : HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //Session.RemoveAll();
                        //return RedirectToAction("Login", "Account");
                        //return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });

                        return Json(new { Result = true, Message = "Password changed successfully!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //AddErrors(result);
                        return Json(new { Result = false, Message = "Old Password is wrong!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field

                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    string _UserName = db.sp_Vendor_SelectWhere(" and VendorCode = '" + (string)Session["VendorCode"] + "'").FirstOrDefault().UserName;
                    var _UserIdentity = await UserManager.FindByNameAsync(_UserName);
                    if (_UserIdentity != null)
                    {
                        string _HasherPassword = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                        UserStore<ApplicationUser> _Store = new UserStore<ApplicationUser>();
                        await _Store.SetPasswordHashAsync(_UserIdentity, _HasherPassword);
                        IdentityResult Resetresult = await UserManager.UpdateAsync(_UserIdentity);

                        if (Resetresult.Succeeded)
                        {
                            TempData["Success"] = "Password successfully reset";
                            return Json(new { Result = true, Message = "Password successfully reset" }, JsonRequestBehavior.AllowGet);
                            //return RedirectToAction("Login");
                        }
                        else
                        {
                            //TempData["Warning"] = "New Password is fail to set!";
                            return Json(new { Result = false, Message = "New Password is fail to set!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(new { Result = false, Message = "No Operation Perform. Please check your data!" }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ActionResult SigninGoogle()
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            //Session.Remove("UserName");
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }


        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ForgetPassword(string UserName)
        {
            try
            {
                string _EmailId = "";
                string _HtmlBody = "", _PlainBody = "";
                bool _Result = false;

                sp_Vendor_SelectWhere_Result _ObjVendor = db.sp_Vendor_SelectWhere(" and UserName = '" + UserName + "'").FirstOrDefault();
                if (_ObjVendor == null)
                    return Json(new { Result = false, Message = "User name is wrong.if you forgot user name than contact to admin!" }, JsonRequestBehavior.AllowGet);

                _EmailId = _ObjVendor.EmailId;

                if (string.IsNullOrEmpty(_EmailId))
                    return Json(new { Result = false, Message = "You do not have Email id.Please contact to admin!" }, JsonRequestBehavior.AllowGet);

                string _Title = "Create a new Password";
                string _MainDesc = "", _MainContent = "", _Description = "";
                string _UniqueCode = System.DateTime.Now.AddHours(2).ToBinary().ToString() + "-" + _ObjVendor.VendorCode;
                string _ResetUrl = "http://admin.fashiondiva.biz/Account/SetPassword/" + _UniqueCode;
                //string _ResetUrl = "http://localhost:1255/Account/SetPassword/" + _UniqueCode;
                _MainDesc = " Forgot your password, huh? No big deal.<br /> To create a new password, just press this button or follow link:<br />";

                _Description = " *You received this email, because it was requested by a <b>Fashion DIVA</b> user. This is part of the procedure to create a new password on the system. If you DID NOT request a new password then please ignore this email and your password will remain the same.";

                _MainContent = " <p style=\"text-align:center\";>";
                _MainContent += " <a style=\" text-decoration:none; display: inline-block;  padding: 6px 12px;    margin-bottom: 0;    font-size: 14px;    font-weight: 400;    line-height: 1.42857143;    text-align: center;    white-space: nowrap;    vertical-align: middle;    -ms-touch-action: manipulation;    touch-action: manipulation;    cursor: pointer;    -webkit-user-select: none;    -moz-user-select: none;    -ms-user-select: none;    user-select: none;    background-image: none;    border: 1px solid transparent; color: #fff;   border-radius: 4px; background-color: #3c8dbc; border-color: #367fa9;\" href='" + _ResetUrl + "'\" >Reset Password</a>";
                _MainContent += " </p>";
                _MainContent += " <span style=\"text-align:left;\">";
                _MainContent += " Link doesn't work? Copy the following link to your browser address bar:";
                _MainContent += " </span>";
                _MainContent += " <p style=\"text-align:center\";>";
                _MainContent += " <nobr><a href=\"" + _ResetUrl + "\" style=\"color: #3366cc;\">" + _ResetUrl + "</a></nobr>";
                _MainContent += " </p>";

                _HtmlBody = CommanClass.GenerateMailBody(_Title, _MainDesc, _MainContent, _Description);
                _PlainBody = " Forgot your password, huh? No big deal. To create a new password, just follow link: " + _ResetUrl;
                _Result = CommanClass.SendMail(_EmailId, "Link for Reset Password", _HtmlBody, _PlainBody);
                if (_Result)
                    return Json(new { Result = _Result, Message = "Mail sent successfully!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Result = _Result, Message = "Mail fail to send. Internet problem!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //throw ex;
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetPassword(string id)
        {
            try
            {
                ManageUserViewModel _Obj = new ManageUserViewModel();
                if (!string.IsNullOrEmpty(id))
                {
                    DateTime _FGDT = DateTime.FromBinary(Convert.ToInt64(id.Substring(0, id.LastIndexOf('-'))));
                    if (_FGDT.Date.Equals(System.DateTime.Now.Date))
                    {
                        if ((_FGDT.Hour > System.DateTime.Now.Hour) || (_FGDT.Hour == System.DateTime.Now.Hour && _FGDT.Minute >= System.DateTime.Now.Minute))
                            Session["VendorCode"] = id.Substring(id.LastIndexOf('-') + 1, id.Length - (id.LastIndexOf('-') + 1));
                        else
                        {
                            TempData["Warning"] = "Please regenerate link for forget password. Time limit is over!";
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        TempData["Warning"] = "Please regenerate link for forget password. Time limit is over!";
                        return RedirectToAction("Login", "Account");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}