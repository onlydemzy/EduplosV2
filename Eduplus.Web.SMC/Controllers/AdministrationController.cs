using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.ViewModels;
using KS.Web.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    [KSWebAuthorisation]
    public class AdministrationController : BaseController
    {
        // GET: Administration
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAcademicAffairsService _academicAffairs;
        public AdministrationController(IGeneralDutiesService generalDtuies,IAcademicAffairsService academicAffairs)
        {
            _generalDuties = generalDtuies;
            _academicAffairs = academicAffairs;
        }

        
        public ActionResult Departments()
        {
            return View();
        }
       
        public DepartmentDTO SaveDepartment(DepartmentDTO department)
        {
            var dept = _generalDuties.AddUpdateDepartment(department, User.UserId);
            return department;
        }

        public ActionResult Faculties()
        {
            return View();
        }
        
        public Faculty SaveFaculty(Faculty faculty)
        {
           var _faculty= _generalDuties.AddUpdateFaculty(faculty, User.UserId);
            return _faculty;
        }

        
        public ActionResult Programmes()
        {
            return View();
        }
        
        public ProgrammeDTO SaveProgramme(ProgrammeDTO programme)
        {
            var np = _generalDuties.AddUpdateProgramme(programme, User.UserId);
            return np;
        }
       
        #region ACADEMIC CALENDER
        //ACADEMIC SESSIONS
        public ActionResult AcademicSessions()
        {
            return View();
        }

        public ActionResult SessionDetails(int? sessionId)
        {

            Session["sessionid"] = sessionId;
            return View();
        }

        public JsonResult SessionSemesters()
        {
            int sessId = (int)Session["sessionid"];
            if (sessId > 0)
            {
                var semesters = _generalDuties.FetchSingleSession((int)sessId);

                return Json(semesters, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new SessionDTO(), JsonRequestBehavior.AllowGet);
        }

        public void SaveSession(SessionDTO session)
        {
            _generalDuties.SaveSession(session, User.UserId);
        }

        public string SaveSemester(SemesterDTO semester)
        {
            return _generalDuties.SaveSemester(semester, User.UserId);
        }


        #endregion

        #region INSTITUTIONS DATA
        public ActionResult InstitutionData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InstitutionData(UserData model)
        {
            if (!ModelState.IsValid)
                return View();       
            _generalDuties.SaveUserData(model, User.UserId);
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public ActionResult UploadInstitutionImages()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadInstitutionImages(HttpPostedFileBase logo, HttpPostedFileBase regBana,
            HttpPostedFileBase regFooter, HttpPostedFileBase regSign)
        {
            UserData ud = new UserData();
            
            if (logo != null)
            {
                int fl;
                var img = Image2Binary(logo, out fl);
                if (fl == 1)
                {
                    ModelState.AddModelError("", "Error: Logo size must not exceed 100 kilobytes");
                    return View();
                }
                ud.Logo = img;
            }
            if (regBana != null)
            {
                int fl;
                var img = Image2Binary(regBana, out fl);
                if (fl == 1)
                {
                    ModelState.AddModelError("", "Error: Logo size must not exceed 100 kilobytes");
                    return View();
                }
                ud.Regbanner = img;
            }
            if (regFooter != null)
            {
                int fl;
                var img = Image2Binary(regFooter, out fl);
                if (fl == 1)
                {
                    ModelState.AddModelError("", "Error: Logo size must not exceed 100 kilobytes");
                    return View();
                }
                ud.RegFooter = img;
            }
            if (regSign != null)
            {
                int fl;
                var img = Image2Binary(regSign, out fl);
                if (fl == 1)
                {
                    ModelState.AddModelError("", "Error: Logo size must not exceed 100 kilobytes");
                    return View();
                }
                ud.RegSign = img;
            }

            _generalDuties.AddImages2User(ud, User.UserId);

            return RedirectToAction("Index","Home");
        }

        byte[] Image2Binary(HttpPostedFileBase image,out int flag)
        {

            byte[] pix = null;
            if (image != null)
            {
                if (image.ContentLength > 500000)
                {
                    flag = 1;
                    return pix;
                     
                }
                using (var binaryReader = new BinaryReader(image.InputStream))
                {
                    pix = binaryReader.ReadBytes(image.ContentLength);
                    flag = 2;
                }
            }
            else
            {
                flag = 0;
            }
            return pix;
        }
        #endregion
    }
}