using Eduplus.Domain.CoreModule;
using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eduplus.Web.SMC.Controllers
{
    [RoutePrefix("api/ContentManagementOut")]
    public class ContentManagementOutController : ApiController
    {
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAppImagesService _appImagesService;
        private readonly IArticleService _articleService;

        public ContentManagementOutController(IGeneralDutiesService generalDuties, IAppImagesService appImagesService, IArticleService articleService)
        {
            _generalDuties = generalDuties;
            _appImagesService = appImagesService;
            _articleService = articleService;
        }
        public ContentManagementOutController() { }
        //[EnableCors(origins:"http://localhost:4200",headers:"*",methods:"*")]
        [HttpGet]
        [Route("FetchSlideContents")]
        public  List<SlideContentVM> FetchSlideContents()
        {
            var sc= _appImagesService.GetAllSlideImages();
            List<SlideContentVM> vm = new List<SlideContentVM>();
            foreach(var s in sc)
            {
                vm.Add(new SlideContentVM
                {
                    ImageId = s.ImageId,
                    Description = s.Description,
                    Title = s.Title,
                    Foto= "As an increasingly powerful, interactive, and dynamic medium for delivering information, the World Wide Web (Web) in combination with information technology (e.g., LAN, WAN, Internet, etc.) has found many applications. One popular application has been for educational use, such as Web-based, distance, distributed or online learning. The use of the Web as an educational tool has pro-vided learners and educators with a wider range of new and interesting learning experiences and teaching environments, not possible in traditional in class education (Khan, 1997). Web-based learning environments have been Material published as part of this publication,"// either on-line or in print, is copyrighted by the Informing Science Institute. Permission to make digital or paper copy of part or all of these works for personal or classroom use is granted without fee provided that the copies are not made or distributed for profit or commercial advantage AND that copies 1) bear this notice in full and 2) give the full citation on the first page. It is permissible to abstract these works so long as credit is given. To copy in all other cases or to republish or to post on a server or to redistribute to lists requires specific permission and payment of a fee. Contact Publisher@InformingScience.org to request redistribution permission. "
                });
            }
            return vm;
        }
    }
   
}
