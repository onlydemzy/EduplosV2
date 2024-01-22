using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.ArticleModule
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }
        
        public string Content { get; set; }
        public string Type { get; set; }//Latest news events

        public string ImageId { get; set; }
        public byte[] Photo { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }

        public DateTime PostedDate { get; set; }

        public string WrittenBy { get; set; }

        public bool Discontinue { get; set; }
        public byte Priority { get; set; }
    }
}
