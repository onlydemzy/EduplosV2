namespace Eduplus.Domain.ArticleModule

{
    using CoreModule;
    using System;


    public class Article
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }//Latest news events

        public string ImageId { get; set; }

        public DateTime PostedDate { get; set; }

        public string WrittenBy { get; set; }

        public bool Discontinue { get; set; }
        public byte? Priority { get; set; }

        public virtual AppImages Photo { get; set; }
    }
}
