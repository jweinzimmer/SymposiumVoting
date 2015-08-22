using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SymposiumVoting.Models;

namespace SymposiumVoting.Controllers
{
    public class VotersController : Controller
    {
        private mainDBContext db = new mainDBContext();

        // GET: /Voters/
        public ActionResult Index()
        {

            return View(db.Voters.ToList());
        }

        // GET: /Voters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = db.Voters.Find(id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    [HttpPost]
        public ActionResult generate_voters(FormCollection form)
        {
            Symposium symposium = db.Symposiums.Find(Convert.ToInt32(form["symposium_id"]));
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string year = symposium.year;
            year = year.Substring(2, year.Length - 2);
            string semester = symposium.semester;
            if (semester == "Spring" || semester == "spring")
            {
                semester = semester.Remove(semester.Length - 5);
            }
            else
            {
                semester = semester.Remove(semester.Length - 3);
            }
      
            for (int i = 1; i <= Convert.ToInt32(form["voter_amount"]); i++)
            {

                Voter voter = new Voter();
                string result = new string(Enumerable.Repeat(chars, 3).Select(s => s[random.Next(s.Length)]).ToArray());
                result = result + semester + year;
                result = Char.ToLowerInvariant(result[0]) + result.Substring(1);
                voter.voting_id = result;
                voter.symposiumID = symposium.ID;
                db.Voters.Add(voter);
               
            }
            db.SaveChanges();
            return RedirectToAction("Details", "Symposiums", new { id = Convert.ToInt32(form["symposium_id"])});
        }
    [HttpPost]
    public ActionResult voters_delete(FormCollection form)
    {
        //Symposium symposium = db.Symposiums.Find(id);
        Symposium symposium = db.Symposiums.Find(Convert.ToInt32(form["symposium_id"]));
        foreach (var item in symposium.Voters.ToList())
        {
            Voter voter = db.Voters.Find(item.ID);
            db.Voters.Remove(voter);
            
        }
        foreach (var item in symposium.Projects)
        {
            Project project = db.Projects.Find(item.ID);
            project.total_votes = 0;
            db.Entry(project).State = EntityState.Modified;
        }
        db.SaveChanges();
        return RedirectToAction("Details", "Symposiums", new { id = symposium.ID });
    }
        // GET: /Voters/Create
        public ActionResult Create(int id)
        {

            Voter voter = new Voter();
            Symposium symposium = db.Symposiums.Find(id);
            voter.Symposium = symposium;
            voter.symposiumID = id;
            return View(voter);
        }

        // POST: /Voters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,voting_id,symposiumID")] Voter voter)
        {
            if (ModelState.IsValid)
            {
                Symposium symposium = db.Symposiums.Find(voter.symposiumID);
                voter.Symposium = symposium;
                db.Voters.Add(voter);
                db.SaveChanges();
                return RedirectToAction("Details", "Symposiums", new { id=voter.symposiumID});
            }

            return View(voter);
        }

        // GET: /Voters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = db.Voters.Find(id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }

        // POST: /Voters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,first_name,last_name,email,phone_number,voting_id")] Voter voter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(voter);
        }

        // GET: /Voters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = db.Voters.Find(id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }

        // POST: /Voters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voter voter = db.Voters.Find(id);
            db.Voters.Remove(voter);
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
