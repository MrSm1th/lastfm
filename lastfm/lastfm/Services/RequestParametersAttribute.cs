using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lastfm.Services
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RequestParametersAttribute : Attribute
    {
        public HttpMethod HttpMethod { get; private set; }

        public RequestParametersAttribute(HttpMethod method)
        {
            this.HttpMethod = method;
        }

        public bool IsSsl { get; set; }
        public bool AuthNeeded { get; set; }
    }

}
