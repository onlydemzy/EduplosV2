
using Eduplos.Services.Contracts;
using KS.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eduplos.Web.SMC.Controllers
{
    [KSWebAuthorisation]
    public class ResultIssuesController : BaseController
    {
        private IAcademicProfileService _acadaService;
        private IStaffService _staffService;
        public ResultIssuesController(IAcademicProfileService acada,IStaffService staff)
        {
            _acadaService = acada;
            _staffService = staff;
        }
        public ActionResult NewResultComplain()
        {
            return View();
        }

        public JsonResult FetchSingleScore(int semesterId,string matricNumber,string courseId)
        {
            return Json(_acadaService.SingleStudentScore(semesterId, matricNumber, courseId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCourseLecturer(string deptCode)
        {
            string dept;
            if(string.IsNullOrEmpty(deptCode)||deptCode=="undefined")
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                dept = user.DepartmentCode;
            }
            else
            {
                dept = deptCode;
            }
            return Json(_staffService.FetchStaff(deptCode),JsonRequestBehavior.AllowGet);
        }
    }
}