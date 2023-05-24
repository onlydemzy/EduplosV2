using Eduplus.Services.Contracts;
using KS.Domain.HRModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    public class HRMController : BaseController
    {
        private readonly IStaffService _staffService;
        

        public HRMController(IStaffService staffService)
        {
            _staffService = staffService;
            
        }

        // GET: HRM
        [HttpGet]
        public ActionResult Employee()
        {
            
            return View();
        }

        [HttpPost]
        public string CreateStaff(Staff employeeVM)
        {
            
           _staffService.CreateStaff(employeeVM,User.UserId);
            return "OK";
        }
        
    }
}