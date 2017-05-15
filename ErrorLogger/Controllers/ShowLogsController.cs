using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Data.Common;

namespace ErrorLogger.Controllers
{
    [HandleError(ExceptionType = typeof(DbException), View = "DBConnectionError")]
    [HandleError(ExceptionType = typeof(ApplicationException), View = "Error")]
    public class ShowLogsController : Controller
    {
        private rshar102DataBaseEntities db = new rshar102DataBaseEntities();

        // GET: ShowLogs
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            var logs = db.Logs.Include(l => l.Application);
            return View(logs.ToList());
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult GetLogs(int id, string sortOrder, string searchString)
        {
            ViewBag.AppId = id;
            Application app = db.Applications.Find(id);
            string userId = "";
            if (app != null) {
                userId = app.UserId;
            }
            ViewBag.CategorySortParm = String.IsNullOrEmpty(sortOrder) ? "Category_desc" : "";
            List<Log> result = new List<Log>();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (sortOrder)
                {
                    case "Category_desc":
                        {
                            foreach (Log data in db.Logs.Where(s => s.LogCategory.Contains(searchString)).OrderByDescending(s => s.LogCategory))
                            {
                                if (Session["UserRole"] != null && Session["UserRole"].ToString().Equals("Admin"))
                                {
                                    if (data.AppId.Equals(id))
                                        result.Add(data);
                                }
                                else {
                                    if (userId.Equals(Session["UserId"].ToString()) && data.AppId.Equals(id))
                                        result.Add(data);
                                }
                            }
                        }
                        break;

                    default:
                        foreach (Log data in db.Logs.Where(s => s.LogCategory.Contains(searchString)).OrderBy(s => s.LogCategory))
                        {
                            if (Session["UserRole"] != null && Session["UserRole"].ToString().Equals("Admin"))
                            {
                                if (data.AppId.Equals(id))
                                    result.Add(data);
                            }
                            else
                            {
                                if (userId.Equals(Session["UserId"].ToString()) && data.AppId.Equals(id))
                                    result.Add(data);
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (sortOrder)
                {
                    case "Category_desc":
                        {
                            foreach (Log data in db.Logs.OrderByDescending(s => s.LogCategory))
                            {
                                if (Session["UserRole"] != null && Session["UserRole"].ToString().Equals("Admin"))
                                {
                                    if (data.AppId.Equals(id))
                                        result.Add(data);
                                }
                                else
                                {
                                    if (userId.Equals(Session["UserId"].ToString()) && data.AppId.Equals(id))
                                        result.Add(data);
                                }
                            }
                        }
                        break;
                    default:
                        foreach (Log data in db.Logs.OrderBy(s => s.LogCategory))
                        {
                            if (Session["UserRole"] != null && Session["UserRole"].ToString().Equals("Admin"))
                            {
                                if (data.AppId.Equals(id))
                                    result.Add(data);
                            }
                            else
                            {
                                if (userId.Equals(Session["UserId"].ToString()) && data.AppId.Equals(id))
                                    result.Add(data);
                            }
                        }
                        break;
                }
            }
            return View("Index", result.ToList());
        }

        // GET: ShowLogs/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(log);
        }

        // GET: ShowLogs/Create
        [ChildActionOnly]
        public ActionResult Create()
        {
            ViewBag.AppId = new SelectList(db.Applications, "AppId", "AppName");
            return View();
        }

        // POST: ShowLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ChildActionOnly]
        public ActionResult Create([Bind(Include = "LogId,LogMessage,Timestamp,AppId,LogCategory")] Log log)
        {
            if (ModelState.IsValid)
            {
                db.Logs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppId = new SelectList(db.Applications, "AppId", "AppName", log.AppId);
            return View(log);
        }

        // GET: ShowLogs/Edit/5
        [ChildActionOnly]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            ViewBag.AppId = new SelectList(db.Applications, "AppId", "AppName", log.AppId);
            return View(log);
        }

        // POST: ShowLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ChildActionOnly]
        public ActionResult Edit([Bind(Include = "LogId,LogMessage,Timestamp,AppId,LogCategory")] Log log)
        {
            if (ModelState.IsValid)
            {
                db.Entry(log).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppId = new SelectList(db.Applications, "AppId", "AppName", log.AppId);
            return View(log);
        }

        // GET: ShowLogs/Delete/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(log);
        }

        // POST: ShowLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Log log = db.Logs.Find(id);
            db.Logs.Remove(log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
