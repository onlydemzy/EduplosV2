using Eduplus.Domain.CoreModule;
using Eduplus.DTO.UserManagement;
using Eduplus.Services.Contracts;
using Eduplus.Web.ViewModels;
using KS.Core.UserManagement;
using KS.Services.Contract;
using KS.Web.Security;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Org.BouncyCastle.Asn1;

namespace Eduplus.Web.SMC.Controllers
{

    public class  AccountsController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICommunicationService _commService;
        private readonly IAppImagesService _appImagesService;
        private readonly IGeneralDutiesService _generalDutiesService;
        private readonly IStudentService _studentService;
        public readonly Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Account

        // GET: Auth
        public AccountsController(IUserService userService,ICommunicationService commService, IAppImagesService appImagesService,
            IGeneralDutiesService generalDuties, IStudentService studentService)
        {
            _userService = userService;
            _commService = commService;
            _appImagesService = appImagesService;
            _generalDutiesService = generalDuties;
            _studentService = studentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Unauthorised()
        {
            
            
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            
           
           UserData user = System.Web.HttpContext.Current.Cache.Get("userData") as UserData;
            if (user == null)
            {
                user = _generalDutiesService.GetUserData();
                System.Web.HttpContext.Current.Cache.Insert("userData", user);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
               
                return View();
            }
            else
            {
                var model = new LoginModel
                {
                    ReturnUrl = returnUrl
                };
                return View(model);
            }
            
        }
        [AllowAnonymous]
        public ActionResult SendMail()
        {

           /* MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@korrhsolutions.com","AKSCOE");
            mailMessage.To.Add("demzy247@gmail.com");
            mailMessage.Subject = "Test workings";
            mailMessage.Body = "It is wonderfull to know that Jesus is alive";
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("noreply@korrhsolutions.com", "N0t@452#");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = "smtp.zoho.com";
            smtpClient.Port = 587;
             
            smtpClient.EnableSsl = true;
            
            smtpClient.Send(mailMessage);*/
            string msg = "It is wonderfull to know that Jesus is alive" + "\r\n" + "This is comming from inner appliaction service"
                + "\r\n" + "Taking from database records";
            _commService.SendMail("Udeme Bassey Ekim", "demzy247@gmail.com", msg, "Admission Test");
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        
        public ActionResult Login(LoginModel model)
        {
            logger.Debug("Hi I am NLog Debug Level");
            logger.Info("Hi I am NLog Info Level");
            logger.Warn("Hi I am NLog Warn Level");

            if (!ModelState.IsValid)
            {
                return View();
            }
            string[] _roles;
            var user =  _userService.ValidateUser(model.Password, model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username and or password");
                return View();
            }

            CreateUserSession(user, out _roles);
           // _commService.SendMailViaApi(user.Email, "You just logged in now", "Login Try");
            if (string.IsNullOrEmpty(model.ReturnUrl)||!Url.IsLocalUrl(model.ReturnUrl))
            {
                

                if (_roles.Contains("VC"))
                {
                      

                    return RedirectToAction("Dashboard", "Home");

                }
                if (_roles.Contains("Bursar"))
                {
                     
                    return RedirectToAction("Dashboard", "Home");

                }
                if (_roles.Contains("Administrator"))
                {
                     
                    return RedirectToAction("Dashboard", "Home");

                }
                if (_roles.Contains("Student"))
                {
                     
                     
                        return RedirectToAction("Index", "Student");
                     
                }
                if (_roles.Contains("Prospective"))
                {
                    
                    return RedirectToAction("Prospectives", "Admission_Center");
                    
                    
                }
                if (_roles.Contains("Alumnus"))
                {

                    return RedirectToAction("AlumniDashboard", "Alumni");


                }


                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(model.ReturnUrl);
            }

            
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            HttpCookie coky = new HttpCookie("KS_Eduplus_AKCOE");
            coky.Expires = DateTime.UtcNow.AddDays(-1);
            Response.Cookies.Add(coky);
            
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Accounts");
        }
        

        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult AddUser()
        {
            
            return View();
        }

        [KSWebAuthorisation]
        
        public string CreateUser(RegistrationViewModel userVM)
        {
            string msg;
                string[] array;
                array = userVM.PersonId.Split(':');
                string text = array[0];
                string value = array[1];
            User _user = new User
            {
                UserName = userVM.UserName,
                FullName = value,
                UserId = text,
                
                Password = userVM.Password,
                DepartmentCode = userVM.DepartmentCode,
                ProgrammeCode = userVM.ProgrammeCode,
                   
                    CreatedBy=User.UserId
                    
                };
                //register user
                
              _userService.RegisterUser(_user, userVM.Role,out msg, User.UserId);
                //set cookies and login user
        
            return msg;
        }
        [KSWebAuthorisation]
        public ActionResult BulkCreateStudentUsers()
        {
            
            return View();
        }
        [KSWebAuthorisation]
         
        public ActionResult ResetStudentUsersPassword(string programmecode,string yearAdmitted)
        {
                
                
            return View();
            
        }

        
        [KSWebAuthorisation]

        public string SubmitCreateStudentUsers(string faculty)
        {

            
            return "Check";

        }
         

        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View();
        }
        [KSWebAuthorisation]
       
        public ActionResult ChangePassword(string oldPassword,string newPassword,string confirmPassword)
        {

            string msg="";
            if (string.IsNullOrEmpty(oldPassword))
            {
                msg = "Provide a valid password";
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                msg = "Provide a valid new password";
            }
            if (newPassword!=confirmPassword)
            {
                msg = "Passwords do not match";
            }
            if(!string.IsNullOrEmpty(msg))
            {
                ModelState.AddModelError("", msg);
                return View();
            }
            else
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                if(user==null)
                {
                    return RedirectToAction("Login");
                }
                msg = _userService.ChangePassword(user.Username, oldPassword, newPassword, user.UserId);
                
                var req = System.Web.HttpContext.Current.Request;
                string browser= GetUserEnvironment(req);
                string msgBody = "An attempt to change your password has been made." + "\n" +browser+"\n"+ msg;

                //string mailresponse = _commService.SendMail(user.Email, msgBody,"Security Notification");
                Session["pasRmsg"] = msgBody;
                return RedirectToAction("ResetConfirmation");
            }
        }
        public ActionResult ResetConfirmation()
        {
            ViewBag.msg = Session["pasRmsg"];
            //Send email
            
            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(string username)
        {
            var user = _userService.FetchSingleUser(username.Trim());
            Session["PassUser"] = null;
            if (user == null)
            {
                ModelState.AddModelError("", "Username not found");
                return View();

            }
            else {
                
                var dto= new UserDTO
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.UserCode,MaskedMail= _userService.MaskUserEmail(user.UserCode),

                };

                Session["PassUser"] = dto;
                return RedirectToAction("PasswordReset"); }
            
        }
        [AllowAnonymous]
        public ActionResult PasswordReset()
        {
            //generate token and send to user email
            
            return View();
        }
        [AllowAnonymous]
        public JsonResult GetPasswordResetData()
        {
            var user = (UserDTO)Session["PassUser"];

            return Json(user, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public string GeneratePasswordOTP()
        {
            var user = (UserDTO)Session["PassUser"];
            return _userService.GenerateAndSendEmailToken(user);
             
        }
        [AllowAnonymous]
        public string SavePasswordReset(object[] data)
        {
            string username = (string)data[0];
            var newPassword = (string)data[1];
            var userId = (string)data[2];
            var token = (string)data[3];
            //Verify token
            if (_userService.ValidateSimpleToken(userId, token))
            {
                return _userService.ChangePassword(username, null, newPassword, userId);
            }
            else return "Invalid or expired token entered";
            
        }
        public ActionResult ResetUSerPassword()
        {
            return View();
        }

        public void ResetPassword(ChangePasswordViewModel model)
        {

           _userService.ChangePassword(model.Username, "", model.NewPassword, User.UserId);


        }
        public JsonResult FetchUser(string username)
        {
            
            var user = _userService.FetchSingleUser(username);
            if (user == null)
                return this.Json("Empty", JsonRequestBehavior.AllowGet);
            else
            {
                var model = new ChangePasswordViewModel
                {
                    UserId = user.UserId,
                    Username = user.UserName,
                    Fullname = user.FullName,
                    
                    
                };

                return this.Json(model, JsonRequestBehavior.AllowGet);
            }
        }


        

        private void CreateUserSession(User user, out string[] uroles)
        {
            var roles = _userService.FetchUserRoles(user.UserName);
            bool sysAdmin=false;
            var ur = user.UserRoles.Where(a => a.IsSystemAdmin == true).FirstOrDefault();
            if (ur!=null)
            {
                sysAdmin = true;
            }
            var permissions = _userService.FetchUserRolesPermissions(user.UserName);
            var menus = _userService.FetchUserMenus(permissions, sysAdmin).ToList();

            var photo = _appImagesService.GetStudentPhoto(user.UserId);
            var currentSemester = _generalDutiesService.FetchCurrentSemester();
            
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
            serializeModel.UserId = user.UserId;
            serializeModel.FullName = user.FullName;
            serializeModel.Username = user.UserName;
             

            CustomPrincipal principal = new CustomPrincipal(user.UserName);
            principal.DepartmentCode = user.DepartmentCode;
            principal.ProgrammeCode = user.ProgrammeCode;
            principal.Roles = roles;
            principal.Permissions = permissions;
            principal.UserMenus = menus;
            principal.IsSysAdmin = sysAdmin;
             
            principal.Photo = photo;
            principal.FullName = user.FullName;
            principal.Username = user.UserName;
             
            principal.UserId = user.UserId;
           
            //principal.ProgrammeType = user.ProgrammeType;
            
            
            Session["CurrentSemester"] = currentSemester;
            Session["LoggedUser"] = principal;
            Session["UserRole"] = roles[0];
            string userData = JsonConvert.SerializeObject(serializeModel, Formatting.Indented,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            
            FormsAuthenticationTicket ntik = new FormsAuthenticationTicket(1, user.UserName, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(50), false, userData);
           
            string encTicket = FormsAuthentication.Encrypt(ntik);
            var chk = FormsAuthentication.FormsCookieName;
            HttpCookie faCookie = new HttpCookie("KS_Eduplos_AKCOE", encTicket);
            faCookie.HttpOnly=true;
            //faCookie.Secure = true;
            faCookie.Expires = DateTime.UtcNow.AddMinutes(50);
            Response.Cookies.Add(faCookie);
            uroles = roles;
            
        }
        
        public string GetUserEnvironment(HttpRequest request)
        {
            var browser = request.Browser;
            var platform = GetUserPlatform(request);
            return string.Format("{0} {1} / {2}", browser.Browser, browser.Version, platform);
        }

        public string GetUserPlatform(HttpRequest request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        #region TOKEN OPERATIONS
        [AllowAnonymous]
        public string CreateToken(string companyname,string url)
        {

            Token token = new Token();
            token.Company = companyname;
            token.Url = url;
            return _userService.CreateToken(token, User.UserId);
             
        }
        public ActionResult Tokens()
        {
            return View();
        }

        public JsonResult GetTokens()
        {
            return Json(_userService.AllTokens(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TokenDetails(int tokenId)
        {
            if (tokenId <= 0)
            {
                ModelState.AddModelError("", "Invalid Token");
                return View();
            }
            var token = _userService.GetToken(tokenId);
            if(token==null)
            {
                ModelState.AddModelError("", "Invalid Token");
                return View();
            }
            else
            {

                
                return View(token);
            }
        }
        #endregion
        

        #region User operations with Roles
        [KSWebAuthorisation]
        public ActionResult Roles()
        {
            return View();
        }
        [KSWebAuthorisation]
        public JsonResult GetRoles()
        {
            return Json(_userService.AllRoles(),JsonRequestBehavior.AllowGet);

        }
        [KSWebAuthorisation]
        public string AddRole(string newRole)
        {
            RoleDTO dt = new RoleDTO();
            dt.RoleName = newRole;
            return _userService.AddRole(dt, User.UserId);
            
        }
        public JsonResult GetRoleUsers(int roleId)
        {
            return Json(_userService.FetchRoleUsers(roleId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllUsers()
        {
            return Json(_userService.FetchAllUsers(), JsonRequestBehavior.AllowGet);
        }


        [KSWebAuthorisation]
        public string DeleteRole(RoleDTO viewModel)
        {
            return _userService.DeleteRole(viewModel.RoleId, User.UserId);

        }
        [KSWebAuthorisation]
        public string AddUserToRole(UserDTO roleUser)
        {
            return _userService.AddUserToRole(roleUser, User.UserId);
        }
        [KSWebAuthorisation]
        public string RemoveUserFromRole(UserDTO roleUser)
        {
            return _userService.RemoveUserFromRole(roleUser, User.UserId);
        }

        #endregion
        #region Permissions
        public ActionResult  RolePermissions()
        {
            return View();
        }
        public JsonResult GetRolePermissions(int? roleId)
        {
            
            return Json(_userService.RolePermissions(roleId), JsonRequestBehavior.AllowGet);
        }
        public string AddPermissiontoRole(PermissionDTO npermission)
        {

            return _userService.AddPermissiontoRole(npermission, User.UserId);
        }
        public string RemovePermissionFromRole(PermissionDTO perm)
        {

            return _userService.RemovePermissionFromRole(perm, User.UserId);
        }
        #endregion

    }


} 