using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMTools.UI.Models
{
    public enum Status : int
    {
        Success = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }

    public class ResponseModel<T> where T : class
    {
        public T data { get; set; }

        public string message { get; set; }

        public Status Status { get; set; }

        public bool success { get; set; }

        public IEnumerable<T> DataCollection { get; set; }
    }
    public class ResponseModel
    {
        public object Data { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }

        public Status Status { get; set; }
    }
}