using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web.ViewModels
{
    public class FacultyArticleViewModel
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public byte[] Photo { get; set; }

        public DateTime PostedDate { get; set; }
        public string WrittenBy { get; set; }
        public List<DeptViewModel> Departments { get; set; }

    }

    public class DeptViewModel
    {
        public string DeptCode { get; set; }
        public string Title { get; set; }
    }
}