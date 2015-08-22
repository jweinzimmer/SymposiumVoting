using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SymposiumVoting.Models;

namespace SymposiumVoting.Controllers
{
    public class SymposiumsController : Controller
    {
        private mainDBContext db = new mainDBContext();
        public ActionResult deploy(int id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                Symposium live_symposium = db.Symposiums.Where(s => s.live == true && s.ID != id).FirstOrDefault();
                if (live_symposium != null)
                {
                    live_symposium.live = false;
                }
                Symposium symposium = db.Symposiums.Find(id);
                if (symposium.live == false)
                {
                    symposium.live = true;
                }
                else
                {
                    symposium.live = false;
                }
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }

        }

        public ActionResult report_index()
        {
            Session["YourSessionKey"] = null;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                return View(db.Symposiums.ToList());
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        public ActionResult reports(int id, int? course_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {

                Symposium symposium = db.Symposiums.Find(id);

                foreach (var project in symposium.Projects)
                {
                    List<int> list = new List<int>();

                    foreach (var vote in project.Votes)
                    {

                        list.Add(vote.amount);
                    }
                    int over_all_amount = list.Sum();
                    if (project.total_votes != over_all_amount)
                    {
                        project.total_votes = over_all_amount;
                        db.Entry(project).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }

                var top_three = symposium.Projects.OrderByDescending(p => p.total_votes).Take(3);


                //ViewBag.Projects = symposium.Projects.ToList();
                return View(symposium);
            }
            else
            {

                return RedirectToAction("desktop_vote", "Projects");
            }

           

        }
        // GET: /Symposiums/
        public ActionResult Index()
        {
            Session["YourSessionKey"] = null;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                return View(db.Symposiums.ToList());
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // GET: /Symposiums/Details/5
        [HttpPost]
        public ActionResult delete_projects(int symposium_id)
        {
            Symposium symposium = db.Symposiums.Find(symposium_id);
            foreach (var project in symposium.Projects.ToList())
            {
                Project found_project = db.Projects.Find(project.ID);
                db.Projects.Remove(found_project);
            
            }
            
            db.SaveChanges();
            return RedirectToAction("Details", "Symposiums", new { id = symposium.ID });
        }
        public ActionResult voter_ids(int? id)
        {
            Symposium symposium = db.Symposiums.Find(id);
            ViewBag.Symposium = symposium;
            ViewBag.Voter_IDS = symposium.Voters.Select(x => x.voting_id).ToList();
            return View();
        }
        [WordDocument(DefaultFilename = "VotingIDS")]
        public ActionResult export_voter_ids(int? id)
        {
            Symposium symposium = db.Symposiums.Find(id);
            ViewBag.Symposium = symposium;
            ViewBag.Voter_IDS = symposium.Voters.Select(x => x.voting_id).ToList();
            return PartialView("voter_ids");
        }
        public ActionResult Details(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                Symposium live_symposium = db.Symposiums.Where(s => s.live == true).FirstOrDefault();

                ViewBag.Live_symposium = live_symposium;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
          
                Symposium symposium = db.Symposiums.Find(id);
                if (symposium.vote_style == "amount" || symposium.vote_style == "range")
                {
                    foreach (var project in symposium.Projects)
                    {
                        List<int> list = new List<int>();

                        foreach (var vote in project.Votes)
                        {

                            list.Add(vote.amount);
                        }
                        int over_all_amount = list.Sum();
                        if (project.total_votes != over_all_amount)
                        {
                            project.total_votes = over_all_amount;
                            db.Entry(project).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }
                else
                {
                    foreach (var project in symposium.Projects)
                    {
                        List<int> list = new List<int>();


                        int over_all_amount = project.Votes.Count();
                        if (project.total_votes != over_all_amount)
                        {
                            project.total_votes = over_all_amount;
                            db.Entry(project).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }
                if (symposium.Projects.Where(x => x.total_votes != 0).FirstOrDefault() != null)
                {
                    ViewBag.Projects = symposium.Projects.ToList().OrderByDescending(p => p.total_votes);
                }
                else
                {
                    ViewBag.Projects = symposium.Projects.ToList().OrderBy(p => p.Name);
                }
                ViewBag.Voter_Count = symposium.Voters.Count();
                ViewBag.Symposium = symposium;
                if (symposium == null)
                {
                    return HttpNotFound();
                }

               
                return View(symposium);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // GET: /Symposiums/Create
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
            
            
        }

        // POST: /Symposiums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form, [Bind(Include="ID,year,semester,vote_style,limit,user_id,min,max")] Symposium symposium)
        {
            if (ModelState.IsValid)
            {

                symposium.min = Convert.ToInt32(form["min"]);
                symposium.max = Convert.ToInt32(form["max"]);
                symposium.vote_style = form["vote_style"];
                db.Symposiums.Add(symposium);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(symposium);
        }

        // GET: /Symposiums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Symposium symposium = db.Symposiums.Find(id);
                if (symposium == null)
                {
                    return HttpNotFound();
                }
                string[] vote_styles = new string[] { "standard", "amount", "range" };
                ViewBag.Vote_Styles = vote_styles;
                return View(symposium);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // POST: /Symposiums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form, [Bind(Include="ID,year,semester,vote_style,limit,user_id,min,max")] Symposium symposium)
        {
            if (ModelState.IsValid)
            {
                db.Entry(symposium).State = EntityState.Modified;
                symposium.vote_style = form["vote_style"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(symposium);
        }

        // GET: /Symposiums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Symposium symposium = db.Symposiums.Find(id);
                if (symposium == null)
                {
                    return HttpNotFound();
                }
                return View(symposium);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
            }
        

        // POST: /Symposiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Symposium symposium = db.Symposiums.Find(id);
            db.Symposiums.Remove(symposium);
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
