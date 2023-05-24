using System;

namespace Eduplus.DTO.ArticleModule
{
    public class EventDTO
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }
        
        public string Content { get; set; }
        public string Type { get; set; }//Latest news events

        public DateTime Date { get; set; }
        
    }
}
