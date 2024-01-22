using System;
using System.Net;

namespace Eduplos.Domain.CoreModule
{
    public class ApiLog
    {
        public long LogId { get; set; }
        public string RequestContentType { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
        public string RequestContent { get; set; }
        public string ResponseContent { get; set; }

    }
    
}
