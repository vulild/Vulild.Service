using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpService
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpException(HttpStatusCode status)
        {
            StatusCode = status;
        }
    }


}
