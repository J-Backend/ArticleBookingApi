using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Reply
{
    public class Reply<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Method { get; set; }
        public int TotalRecords { get; set; }
    }
}