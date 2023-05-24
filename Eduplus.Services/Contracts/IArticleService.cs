using Eduplus.DTO.ArticleModule;
using System.Collections.Generic;

namespace Eduplus.Services.Contracts
{
    public interface IArticleService
    {
        string SaveArticle(ArticleDTO dto, string userId);
        ArticleDTO FetchFacultyArticle(string faculty);
        List<EventDTO> FetchUpComingEvents();
        EventDTO GetEvent(int eventId);
        List<ArticleDTO> FetchCurrentNews();
        List<ArticleListDTO> AllActiveArticles();
        ArticleDTO FetchLinkArticles(string title);
        ArticleDTO FetchArticle(int articleId);
        void DeleteArticle(ArticleListDTO dto, string userId);
    }
}
