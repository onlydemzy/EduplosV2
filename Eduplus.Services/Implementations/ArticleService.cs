using System;
using Eduplos.DTO.ArticleModule;
using Eduplos.Services.Contracts;
using KS.Core;
using Eduplos.Domain.CoreModule;
using Eduplos.Domain.ArticleModule;
using Eduplos.ObjectMappings;
using Eduplos.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using Eduplos.Services.UtilityServices;

namespace Eduplos.Services.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string SaveArticle(ArticleDTO dto, string userId)
        {
            var dbArt = _unitOfWork.ArticleRepository.GetFiltered(d=>d.ArticleId==dto.ArticleId|| d.Title==dto.Title).FirstOrDefault();
                        
            if (dbArt==null)//Fresh article
            {
                Article art = new Article();
                art.Content = dto.Content;
                art.Discontinue = false;
                art.Type = dto.Type;
                art.WrittenBy = dto.WrittenBy;
                art.PostedDate = dto.PostedDate;
                art.Title = dto.Title;
                art.Priority = dto.Priority;
                if (dto.ImagePath != null)
                {
                    AppImages foto = new AppImages();
                     
                    foto.IncludeInSlide = false;
                    foto.InsertDate = DateTime.UtcNow;
                    foto.Title = dto.ImageTitle;
                    foto.FotoPath = dto.ImagePath;
                    art.Photo = foto;
                    foto.ImageId = StandardGeneralOps.GeneratePersonId(0);
                    
                }
                _unitOfWork.ArticleRepository.Add(art);
                _unitOfWork.Commit(userId);

                return "Operation Completed successfully";
            }

           

            dbArt.Content = dto.Content;
            dbArt.Discontinue = dto.Discontinue;
            
           if(dto.Type!= "Linked Content")
            {
                dbArt.Type = dto.Type;
                dbArt.Title = dto.Title;
            }
            dbArt.WrittenBy = dto.WrittenBy;
            dbArt.PostedDate = dto.PostedDate;
            
            dbArt.Priority = dto.Priority;
            

            if(dto.Photo!=null && string.IsNullOrEmpty(dto.ImageId))
            {
                AppImages foto = new AppImages();
                foto.IncludeInSlide = false;
                foto.InsertDate = DateTime.UtcNow;
                foto.Description = dto.Title + " image";
                foto.Foto = dto.Photo;
                _unitOfWork.AppImagesRepository.Add(foto);
                dbArt.Photo = foto;
                
            }
            
            _unitOfWork.Commit( userId);
            
            return "Content successfully updated";

        }

        public ArticleDTO FetchArticle(int articleId)
        {
            var article = _unitOfWork.ArticleRepository.Get(articleId);
            if (article == null)
                return new ArticleDTO();

            return ArticleModuleMappings.ArticleToArticleDTO(article);
        }
        public void DeleteArticle(ArticleListDTO dto, string userId)
        {
            var dbArt = _unitOfWork.ArticleRepository.Get(dto.ArticleId);
            _unitOfWork.ArticleRepository.Remove(dbArt);
            _unitOfWork.Commit(userId);
        }
        public ArticleDTO FetchFacultyArticle(string faculty)
        {
            var art = _unitOfWork.ArticleRepository.GetSingle(a => a.Title == faculty);
            if (art == null)
                return null;
            var dto = ArticleModuleMappings.ArticleToArticleDTO(art);
            return dto;
        }

        public ArticleDTO FetchLinkArticles(string title)
        {
            var art = _unitOfWork.ArticleRepository.GetSingle(a => a.Title == title);
            if (art == null)
                return new ArticleDTO();
            var dto = ArticleModuleMappings.ArticleToArticleDTO(art);
            return dto;

        }

        public List<EventDTO> FetchUpComingEvents()
        {
            List<EventDTO> dto = new List<EventDTO>();
            var events = _unitOfWork.ArticleRepository.UpComingEvents().ToList();
            if(events!=null)
            {
                foreach(var e in events)
                {
                    dto.Add(ArticleModuleMappings.ArticleToEventDTO(e));
                }
                return dto.OrderBy(a=>a.Date).ToList();
            }
            else
            {
                return new List<EventDTO>();
            }
        }

        public List<ArticleDTO> FetchCurrentNews()
        {
            var news = _unitOfWork.ArticleRepository.GetFiltered(a => a.Discontinue == false && a.Type=="News")
                .OrderByDescending(a => a.Priority);
            List<ArticleDTO> dto = new List<ArticleDTO>();
            foreach(var n in news)
            {
                dto.Add(ArticleModuleMappings.ArticleToArticleDTO(n));
            }
            return dto;
        }

        public EventDTO GetEvent(int eventId)
        {
            var _vent = _unitOfWork.ArticleRepository.GetSingle(a => a.ArticleId == eventId);
            var dto = ArticleModuleMappings.ArticleToEventDTO(_vent);
            return dto;
        }

        public List<ArticleListDTO> AllActiveArticles()
        {
            var art = _unitOfWork.ArticleRepository.GetAll();
            List<ArticleListDTO> dto = new List<ArticleListDTO>();
            foreach(var a in art)
            {
                dto.Add(ArticleModuleMappings.ArticleToArticleListDTO(a));
            }
            return dto.OrderBy(a => a.Title).OrderBy(a => a.Type).ToList();
        }

        
    }
}
