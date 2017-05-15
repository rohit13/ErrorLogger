using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErrorLogger.Models
{
    public class ErrorLogModel
    {
        public ErrorLogModel()
        { }
        //public string ErrorLogID { get; set; }
        //public string UserId { get; set; }
        public string ApplicationID { get; set; }
        public string ErrorCategory { get; set; }
        public string ErrorMessage { get; set; }
        //public string TimeStamp { get; set; }
    }
}