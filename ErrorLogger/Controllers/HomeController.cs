using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErrorLogger.Controllers
{
    [HandleError(ExceptionType = typeof(DbException), View = "DBConnectionError")]
    [HandleError(ExceptionType = typeof(ApplicationException), View = "Error")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This Error Logger is developed by Rohit Sharma as final project for CSE686.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Please use below information to contact for any support.";

            return View();
        }

        [Authorize(Roles ="Admin")]
        public ActionResult ShowUsers()
        {
            ViewBag.Message = "Your Registered Application  page.";

            return RedirectToAction("Index", "ShowUsers");
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult ShowApps()
        {
            ViewBag.Message = "Your Registered Application  page.";

            return RedirectToAction("Index", "ShowApps");
        }
    }
}