using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Eduplos.Web.SMC
{
    public class BinaryMediaFormatter : MediaTypeFormatter
    {
        const string binaryContent = "application/octet-stream";
        const int bufferLength = 16384;

        public BinaryMediaFormatter()
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpeg"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpg"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
        }

        public override bool CanReadType(Type type)
        {
            if (type == typeof(byte[]))
                return true;
            else return false;
        }

        public override bool CanWriteType(Type type)
        {

            if (type == typeof(byte[]))
                return true;
            else return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    readStream.CopyTo(ms, bufferLength);
                    taskSource.SetResult(ms.ToArray());
                }
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                if (value == null)
                    value = new byte[0];
                var ms = new MemoryStream((byte[])value);
                ms.CopyTo(writeStream);
                taskSource.SetResult(null);
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }

    }

}       
    
