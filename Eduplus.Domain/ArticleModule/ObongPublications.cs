using Eduplus.Domain.CoreModule;

namespace Eduplus.Domain.ArticleModule
{
    public class ObongPublications
    {
        public int PublicationId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PaperBackId { get; set; }
        public string Path { get; set; }
        public int PublishedYear { get; set; }
        public virtual AppImages Photo { get; set; }
    }
}
