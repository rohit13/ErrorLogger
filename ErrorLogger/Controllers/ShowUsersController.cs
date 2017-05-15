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
    public class ShowUsersController : Controller
    {
        private rshar102DataBaseEntities db = new rshar102DataBaseEntities();

        // GET: ShowUsers
        [Authorize (Roles ="Admin")]
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("GetApps", "ShowApps", new { userId = aspNetUser.Id });
        }

        // GET: ShowUsers/Details/5
        [ChildActionOnly]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(aspNetUser);
        }

        // GET: ShowUsers/Create
        [Authorize(Roles = "Admin")]
        [ChildActionOnly]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShowUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [ChildActionOnly]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,LastLogin,Status")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: ShowUsers/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(aspNetUser);
        }

        // POST: ShowUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,LastLogin,Status")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: ShowUsers/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                //return HttpNotFound();
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(aspNetUser);
        }

        // POST: ShowUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
