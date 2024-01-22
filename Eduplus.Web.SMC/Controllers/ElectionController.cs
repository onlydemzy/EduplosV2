using Eduplos.Services.Contracts;
using Eduplos.Web.SMC.ViewModels;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eduplos.Web.SMC.Controllers
{
    [RoutePrefix("api/Election")]
    public class ElectionController : ApiController
    {
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IStudentsAccountsService _studentAccounts;
        private readonly IStudentService _studentService;
       
        private readonly IAcademicAffairsService _acadaAffairs;
        public readonly Logger logger = LogManager.GetCurrentClassLogger();
        public ElectionController(IGeneralDutiesService generalDuties, IStudentsAccountsService st, IStudentService stud,
            IAcademicAffairsService acada)
        {
            _generalDuties = generalDuties;
            _studentAccounts = st;
            _studentService = stud;
            _acadaAffairs = acada;
        }
        public ElectionController() { }


         
        [HttpGet]
        [Route("AcreditateElectorate")]
        public IHttpActionResult AcreditateElectorate(string phone)
        {
            var st = _studentService.FetchStudentByPhone(phone);
            ElectorateVM vm = new ElectorateVM();
            if (st == null)
            {
                vm.Message = "Phone Number does not exist";
                vm.Status = "020";

            }
            //Check if qualified to vote

            else if (string.IsNullOrEmpty(st.StudentId))
            {
                vm.Message = st.Status;
                vm.Status = "021";

            }
            else
            {
                var chk = _studentService.CanVote(st.StudentId, 20);
                if (chk == false)
                {
                    vm.Message = "Sorry,you are not qualified to Vote. You must pay 2020/2021 session school fee to qualify.";
                    vm.Status = "022";
                }
                else
                {
                    vm.Message = "Qualified to vote";
                    vm.Status = "00";
                    vm.StudentId = st.StudentId;
                    vm.MatricNumber = st.MatricNumber;
                    vm.Phone = st.Phone;
                    vm.ProgrammeCode = st.ProgrammeCode;
                    vm.FullName = st.FullName;
                }
            }
            //var jsonOb = JsonConvert.SerializeObject(vm);
            return Ok(vm);
        }

    }
}
