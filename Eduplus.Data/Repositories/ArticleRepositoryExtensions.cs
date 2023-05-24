using Eduplus.Domain.ArticleModule;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Eduplus.Data.Repositories
{
    public static class ArticleRepositoryExtensions
    {
        public static IEnumerable<Article> UpComingEvents(this IRepository<Article> rep)
        {
            DateTime date = DateTime.UtcNow.Date;
            var events = rep.GetFiltered(a => a.Type == "Event" && DbFunctions.TruncateTime(a.PostedDate) >= date);
            return events;
        }
    }
}
