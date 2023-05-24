using Eduplus.Services.Contracts;
using KS.AES256Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    public class DocuVerifyController : BaseController
    {
        // GET: DocuVerify
        private readonly IAcademicProfileService _acada;
        private readonly IStudentService _student;
        private readonly IGeneralDutiesService _generalDuties;
        public DocuVerifyController(IAcademicProfileService acada, IStudentService student,IGeneralDutiesService gen)
        {
            _acada = acada;
            _student = student;
            _generalDuties = gen;
        }
        public ActionResult Verify(string q)
        {
            string text = DataEncryption.AESDecryptData(q);
            var udata = _generalDuties.GetUserData();
            ViewBag.instu = udata.InstitutionName;

            if (string.IsNullOrEmpty(text))
            {
                
                ViewBag.msg = "Invalid input string";
                return View();
            }
            string[] result = text.Split('_');
            string doc = result[0];
            string displayText="";
            switch(doc)
            {
                case "Transcript":
                    var transcript= _acada.FetchStudentAcademicProfile(result[1], true);
                    displayText="Student Transcript Verified"+"\n\r"+ "Matric No.: " + transcript.RegNo + "\n\r" + "Name: " + transcript.Name + "\n\r" + "Programme:  " + transcript.Programme
                + "\n\r" + "CGPA: " + transcript.CGPA + "\n\r" + "Class of Degree :" + transcript.DegreeClass;
                    break;
                case "AdmissionLetter":
                    var student = _student.FetchStudentSummary(result[1]);
                    displayText = "Student Admission Letter Verified" + "\n\r" + "\n\r" + "Name: " + student.FullName + "\n\r" + "Jamb No: " + student.JambRegNumber + "\n\r" + "Programme:  " + student.Programme;
                    break;
            }
            ViewBag.msg = displayText;
            return View();
        }
    }
}