using Eduplos.Domain.ArticleModule;
using Eduplos.DTO.ArticleModule;

namespace Eduplos.ObjectMappings
{
    public static class ArticleModuleMappings
    {
        public static EventDTO ArticleToEventDTO(Article art)
        {
            var _event = new EventDTO
            {
                ArticleId = art.ArticleId,
                Title = art.Title,
                Content = art.Content,
                Date = art.PostedDate.Date
            };
            return _event;
        }
        public static ArticleDTO ArticleToArticleDTO(Article art)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleId = art.ArticleId;
            dto.Title = art.Title;
            if(!string.IsNullOrEmpty(art.ImageId))
            {
                dto.Photo = art.Photo.Foto;
                dto.ImageId = art.ImageId;
            }
            dto.Content = art.Content;
            dto.PostedDate = art.PostedDate;
            dto.Priority = (byte)art.Priority;
            dto.Type = art.Type;
            dto.WrittenBy = art.WrittenBy;
            return dto;
        }
        public static ArticleListDTO ArticleToArticleListDTO(Article art)
        {
            ArticleListDTO dto = new ArticleListDTO();
            dto.ArticleId = art.ArticleId;
            dto.Title = art.Title;

            dto.Type = art.Type;
            return dto;
        }


    }
}
