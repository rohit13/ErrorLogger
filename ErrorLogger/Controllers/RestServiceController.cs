using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ErrorLogger.Models;
using Models;

namespace ErrorLogger.Controllers
{

    public class RestServiceController : ApiController
    {
        //[Authorize (Roles = "User, Admin")]
        public HttpResponseMessage Post([FromBody] ErrorLogModel ts)
        {
            HttpResponseMessage result;
            if (ts != null)
            {
                using (rshar102DataBaseEntities context = new rshar102DataBaseEntities())
                {
                    try
                    {
                        Application application = new Application();
                        foreach (var app in context.Applications)
                        {
                            if (app.AppId == Convert.ToInt32(ts.ApplicationID))
                            {
                                application = app;
                            }
                        }
                        if (application.AppId == 0) {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Application Id is invalid. Does not exist in database.");
                        }
                        Log dummy = new Log()
                        {
                            //LogId = Convert.ToInt32(ts.ErrorLogID),
                            AppId = Convert.ToInt32(ts.ApplicationID),
                            LogMessage = ts.ErrorMessage,
                            LogCategory = ts.ErrorCategory,
                            Timestamp = Convert.ToDateTime(DateTime.Now),
                            Application = application
                        };
                        context.Logs.Add(dummy);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal Server Error occurred while handling the request. Please try again later.");
                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created, ts);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest, "Server failed to save data");
            }
            return result;
        }
    }
}
