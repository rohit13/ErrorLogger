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
    public class ShowAppsController : Controller
    {
        private rshar102DataBaseEntities db = new rshar102DataBaseEntities();

        // GET: ShowApps
        [Authorize(Roles = "User")]
        public ActionResult Index()
        {
            var applications = db.Applications.Include(a => a.AspNetUser);
            List<Application> result = new List<Application>();
            foreach (Application app in applications)
            {
                if (app.Status == 1 && Session["UserId"]!=null && app.UserId.Equals(Session["UserId"].ToString()))
                {
                    result.Add(app);
                }
            }
            return View(result.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetApps(string userId)
        {
            List<Application> result = new List<Application>();
            foreach (Application data in db.Applications)
            {
                if (data.UserId.Equals(userId))
                    result.Add(data);
            }
            return View("Index", result.ToList());
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult GetApp(int id)
        {
            Application app = db.Applications.Find(id);
            if (app == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("GetLogs", "ShowLogs", new { id = app.AppId });
        }

        // GET: ShowApps/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(application);
        }

        // GET: ShowApps/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName");
            return View();
        }

        // POST: ShowApps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "AppId,AppName,RegDate,Status,UserId")] Application application)
        {
            if (ModelState.IsValid)
            {
                int max = -99;
                using (rshar102DataBaseEntities context = new rshar102DataBaseEntities())
                {
                    foreach (Application data in context.Applications)
                    {
                        if (data.AppId > max)
                        {
                            max = data.AppId;
                        }
                    }
                }
                application.AppId = max + 1;
                application.Status = 1;
                application.RegDate = DateTime.Now;
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("GetApps","ShowApps", new { userId = application.UserId });
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName", application.UserId);
            return View(application);
        }

        // GET: ShowApps/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }

            ViewBag.Status = new SelectList(new List<SelectListItem> { new SelectListItem { Selected = true, Value = "1", Text="Active" }, new SelectListItem { Selected = false, Value = "0", Text = "Inactive" } }, "Value", "Text");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName", application.UserId);
            return View(application);
        }

        // POST: ShowApps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "AppId,AppName,RegDate,Status,UserId")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.RegDate = DateTime.Now;
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                //if user edits redirect to index, if admin edits redirect to showapps
                //return RedirectToAction("Index");
                return RedirectToAction("GetApps", "ShowApps", new { userId = application.UserId });
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName", application.UserId);
            return View(application);
        }

        // GET: ShowApps/Delete/5
        [Authorize(Roles = "User, Admin")]
        [ChildActionOnly]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(application);
        }

        // POST: ShowApps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
        [ChildActionOnly]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();
            if(Session["UserRole"] !=null && Session["UserRole"].ToString().Equals("Admin"))
                return RedirectToAction("GetApps","ShowApps",new { userId = application.UserId });
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
