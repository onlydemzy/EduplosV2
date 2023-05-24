using Eduplus.Domain.CoreModule;
using Eduplus.DTO.ArticleModule;
using Eduplus.DTO.BursaryModule;
using Eduplus.Services.Contracts;
using Eduplus.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAppImagesService _appImagesService;
        private readonly IArticleService _articleService;
        public HomeController(IGeneralDutiesService generalDuties, IArticleService articleService, IAppImagesService appImagesService)
        {
            _generalDuties = generalDuties;
            _articleService = articleService;
            _appImagesService = appImagesService;

        }
        //[OutputCache(Duration =7200,VaryByParam ="none",Location =System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult InfoView(string view)
        {
            var art = _articleService.FetchFacultyArticle(view);
            if (art == null)
            {
                ViewBag.Photo = null;
                return View();
            }


            ViewBag.title = art.Title;
            if (art.Photo != null)
            {
                ViewBag.Photo = art.Photo;
            }
            else
            {
                ViewBag.Photo = null;
            }
            return View(art);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult _MenuPartial()
        {
            return PartialView();
        }


        public ActionResult OtherLinksPartial()
        {

            return PartialView();
        }
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        public ActionResult FacultyDeptsPartial(string facultyCode)
        {
            var depts = _generalDuties.FetchDepartments(facultyCode);
            return PartialView(depts.ToList());
        }
        public ActionResult CollegeDetail(string college)
        {
            FacultyArticleViewModel vm = new FacultyArticleViewModel();
            List<DeptViewModel> dlist = new List<DeptViewModel>();


            var detail = _articleService.FetchFacultyArticle(college);
            if (detail == null)
            {
                ViewBag.Photo = null;
                return View(new FacultyArticleViewModel { Title = college });
            }
            vm.ArticleId = detail.ArticleId;
            vm.Content = detail.Content;
            ViewBag.Photo = detail.Photo;
            vm.Title = detail.Title;
            //get depts
            var depts = _generalDuties.FetchDepartments(college);
            foreach (var d in depts)
            {
                var dept = new DeptViewModel
                {
                    DeptCode = d.DepartmentCode,
                    Title = d.Title
                };
                dlist.Add(dept);
            }
            vm.Departments = dlist;
            return View(vm);
        }
        #region NEWS/EVENTS OPERATIONS
        public ActionResult EventDetail(int _event)
        {
            var ev = _articleService.GetEvent(_event);
            return View(ev);
        }
        public ActionResult News(string news)
        {
            var dto = _articleService.FetchFacultyArticle(news);
            return View(dto);
        }
        public ActionResult _NewsPartial()
        {
            var news = _articleService.FetchCurrentNews();
            return PartialView(news);
        }
        public ActionResult _EventsPartial()
        {
            var upcomingEvents = _articleService.FetchUpComingEvents();
            List<EventsViewModel> vm = new List<EventsViewModel>();
            if (upcomingEvents == null)
                return View(new List<EventsViewModel>());
            foreach (var e in upcomingEvents)
            {
                EventsViewModel m = new EventsViewModel();
                m.Date = e.Date.ToString("dd, MMM");
                m.Title = e.Title;
                m.EventId = e.ArticleId;

                vm.Add(m);
            }

            return PartialView(vm);
        }
        #endregion

        public async Task<ActionResult> _SlidePartial()
        {
            var pix = _appImagesService.GetAllSlideImages();

            return PartialView(pix);

        }

        public ActionResult Mission()
        {
            var mis = _articleService.FetchLinkArticles("Mission");
            return View(mis);
        }
        public ActionResult Vision()
        {
            var mis = _articleService.FetchLinkArticles("Vision");
            return View(mis);
        }
        public ActionResult CoreValues()
        {
            var mis = _articleService.FetchLinkArticles("CoreValues");
            return View(mis);
        }




    }
}