using Eduplus.Domain.CoreModule;
using Eduplus.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Eduplus.Web.SMC
{
    public class ApiLogHandler:DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var log = BuildRequestMetadata(request);
                var response = await base.SendAsync(request, cancellationToken);
                log = BuildResponseMetadata(log, response);
                await SendToLog(log);
                return response;
                
            }
            catch(Exception ex)
            {
                
                var resp=request.CreateResponse("Empty or bad parameter supplied");
                return resp;
            }

        }

        private ApiLog BuildRequestMetadata(HttpRequestMessage request)
        {
            ApiLog log = new ApiLog();
            log.RequestMethod = request.Method.Method;
            log.RequestTimestamp = DateTime.UtcNow;
            log.RequestUri = request.RequestUri.ToString();
             
            if(request.Content.Headers.ContentLength>0)
            {
                
                log.RequestContentType = request.Content.Headers.ContentType.MediaType;
                log.RequestContent = request.Content.ReadAsStringAsync().Result;
            }
            else
            {
                log.RequestContent = "Empty Request";
                 
            }
            return log;
        }
        private ApiLog BuildResponseMetadata(ApiLog log, HttpResponseMessage response)
        {
            log.ResponseStatusCode = response.StatusCode.ToString();
            log.ResponseTimestamp = DateTime.UtcNow;
            if(response.Content!=null)
            {
                log.ResponseContentType = response.Content.Headers.ContentType.MediaType;
                log.ResponseContent = response.Content.ReadAsStringAsync().Result;
            }
           
            return log;
        }
        private async Task<bool> SendToLog(ApiLog log)
        {
            // TODO: Write code here to store the logMetadata instance to a pre-configured log store...
            GeneralDutiesService serv = new GeneralDutiesService();
            serv.SaveApiLog(log);
            return true;
        }
    }
}