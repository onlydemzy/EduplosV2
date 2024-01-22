using Eduplos.Domain.CoreModule;
using Eduplos.DTO.ArticleModule;
using Eduplos.Services.Contracts;
using KS.Web.Security;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Eduplos.Web.SMC.Controllers
{
    [KSWebAuthorisation]
    public class ContentManagementController : BaseController
    {
        // GET: SitePanel
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAppImagesService _appImagesService;
        private readonly IArticleService _articleService;
        public ContentManagementController(IGeneralDutiesService generalDuties,IAppImagesService appImagesService, IArticleService articleService)
        {
            _generalDuties = generalDuties;
            _appImagesService = appImagesService;
            _articleService = articleService;
        }
        #region SLIDE CONTENT
        [HttpGet]
        public ActionResult UploadSlidePix()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadSlidePix(HttpPostedFileBase image,AppImages model)
        {
            if (ModelState.IsValid)
            {

                byte[] imageData = null;
                if (image == null)
                {

                    ModelState.AddModelError("", "No Picture was selected");
                    return View();

                }
                int filelength = image.ContentLength;
                if (filelength > 500000)
                {
                    ModelState.AddModelError("", "File size exceeded the maximum of 500kb");
                    return View();
                }
               
                   /* using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);

                    }
                //string type = Path.GetExtension(image.FileName).ToLowerInvariant();
                model.Foto = imageData;*/
                model.InsertDate = DateTime.UtcNow.Date;
                model.IncludeInSlide = true;
                model.ImageId = Guid.NewGuid().ToString();
                model.FotoPath = SaveToFile(image,model.ImageId);
                _appImagesService.SaveImage(model, User.UserId);
                return RedirectToAction("ManageSlideContents");
            }

            ModelState.AddModelError("", "Error please try again");
            return View();
        }
        
        string SaveToFile(HttpPostedFileBase image,string id)
        {
             
            string fPath = "";
            var fileName = Path.GetFileName(image.FileName);
            string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + id.Substring(0, 4) + Path.GetExtension(fileName);
            
            string portNo = null;
            if (Request.Url.Port>0)
            {
                portNo =":"+ Request.Url.Port.ToString();
            }

                fPath = Server.MapPath("~/Content/cmsImages/" + newFileName);
            string completePath = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host+portNo+ "/Content/cmsImages/" + newFileName;
            image.SaveAs(fPath);
            return completePath;
        }
         
        public ActionResult ManageSlideContents()
        {
            var slides = _appImagesService.GetAllSlideImages();
            return View(slides);
        }

        public ActionResult DeleteSlide(string imageId)
        {
            _appImagesService.DeleteSlideImage(imageId, User.UserId);
            return RedirectToAction("ManageSlideContents");
        }
        #endregion
        #region ARTICLES
        public ActionResult EditArticle(int? articleId)
        {
            if(articleId>0)
            {
                var article = _articleService.FetchArticle((int)articleId);
                ArticleType(article.Type);
                ArticlePriority(article.Priority);
                return View(article);
            }
            ArticleType();
            ArticlePriority(0);
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditArticle(ArticleDTO article, HttpPostedFileBase image, string types, string date, byte? priority)
        {
            DateTime evdate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(date))
            {
                evdate = Convert.ToDateTime(date);
            }
            if (types == "Article Type" || priority == null)
            {
                ArticleType();
                ArticlePriority(0);
                ModelState.AddModelError("", "Required field is missing");

                return View();
            }

            if (image != null)
            {
                int filelength = image.ContentLength;
                if (filelength > 300000)
                {
                    ArticleType();
                    ArticlePriority(0);
                    ModelState.AddModelError("", "Error: Image size must not exceed 300 kilobytes");

                    return View();
                }

                /*byte[] imageData = null;
                using (var binaryReader = new BinaryReader(image.InputStream))
                {
                    imageData = binaryReader.ReadBytes(image.ContentLength);

                }
                article.Photo = imageData;*/

                article.ImagePath = SaveToFile(image, Guid.NewGuid().ToString());
                
            }
            article.Type = types;
            article.PostedDate = evdate;
            article.Priority = (byte)priority;
            _articleService.SaveArticle(article, User.UserId);
            //check if article contains a script


            return RedirectToAction("Articles");
        }
        void ArticleType(string type=null)
        {
            string[] types = { "Article", "News", "Event", "Linked Content"};
            ViewBag.types = new SelectList(types,type);
        }

        void ArticlePriority(byte priority)
        {
            byte[] priorities = { 1, 2, 3 };
            ViewBag.priority = new SelectList(priorities,priority);
        }
        public ActionResult Articles()
        {
            
            return View();
        }

        
        public JsonResult AllContents()
        {
            var articles = _articleService.AllActiveArticles();
            return this.Json(articles, JsonRequestBehavior.AllowGet);
        }

        public void DeleteArticle(ArticleListDTO article)
        {
            _articleService.DeleteArticle(article, User.UserId);
        }
        #endregion
    }
}