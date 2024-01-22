using KS.Common;
using System;

namespace Eduplos.Domain.CoreModule
{
    public class AppImages:EntityBase
    {
        public string ImageId { get; set; }
        public byte[] Foto { get; set; }
        public string Description { get; set; }
        public bool IncludeInSlide { get; set; }
        public DateTime InsertDate { get; set; }
        public string Title { get; set; }
        public string FotoPath { get; set; }
    }
}
