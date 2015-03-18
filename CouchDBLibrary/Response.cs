using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchDBLibrary
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }
}
