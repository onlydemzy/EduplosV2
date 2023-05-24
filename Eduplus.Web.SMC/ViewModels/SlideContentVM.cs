using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplus.Web.SMC.ViewModels
{
    public class SlideContentVM
    {
        public string ImageId { get; set; }
        public string  Foto { get; set; }
        public string Description { get; set; }
        public bool IncludeInSlide { get; set; }
        public DateTime InsertDate { get; set; }
        public string Title { get; set; }
    }
}