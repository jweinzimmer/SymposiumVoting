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
    public class VotesController : Controller
    {
        private mainDBContext db = new mainDBContext();

        // GET: /Votes/
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                return View(db.Votes.ToList());
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // GET: /Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Vote vote = db.Votes.Find(id);
                if (vote == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Voter = db.Voters.Find(vote.voterID);
                ViewBag.Project = db.Projects.Find(vote.projectID);
                return View(vote);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        public ActionResult no_id()
        {

            return View();
        }


        [HttpPost]
        public ActionResult Vote(FormCollection form)
        {

            try
            {

                string voting_id = form["voting_id"];
                int amount = 0;
                int project_id = Convert.ToInt32(form["ID"]);
                string message = "";
                Project project = db.Projects.Find(project_id);
                Symposium symposium = db.Symposiums.Find(project.symposiumID);
                if (Session["YourSessionKey"] == null)
                {
                    string z = form["voting_id"];
                    int va;
                    if (form["voting_id"] == null || form["voting_id"] == "" || int.TryParse(z, out va) == true)
                    {
                        message = "Please enter a valid voting ID.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                }
                if (Session["YourSessionKey"] == null)
                {
                    if (form["voting_id"] == null || form["voting_id"] == "")
                    {
                        message = "Please enter a valid voting ID.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                }
                if (form["voting_id"] != null && db.Voters.Where(xa => xa.voting_id == voting_id).FirstOrDefault() != null)
                {
                    voting_id = form["voting_id"];
                    Session["YourSessionKey"] = voting_id;
                }
                else if (Session["YourSessionKey"] == null)
                {
                    message = "Please enter a valid voting ID.";
                    return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                }
                string x = form["amount"];
                int value;
                if (int.TryParse(x, out value))
                {
                    if (form["amount"] == "")
                    {
                        message = "Please enter an amount.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                    else
                    {

                        amount = Convert.ToInt32(form["amount"]);
                    }
                }
                else
                {
                    message = "Please enter an amount.";
                    return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                }




                int min = symposium.min;
                int max = symposium.max;
                Voter[] sym_voters = symposium.Voters.ToArray();
                string session_string = Session["YourSessionKey"].ToString();
                Voter voter = sym_voters.Where(v => v.voting_id == session_string).FirstOrDefault();
                if (voter == null)
                {
                    message = "This voter does not exist";
                    return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                }

                if (symposium.vote_style == "range")
                {
                    if (amount > 10 || amount < 1)
                    {
                        message = "Please choose a number between 1 and 10.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }

                }


                if (voter.is_eligible_to_vote(symposium, project, amount, min, max))
                {
                    Vote vote = SymposiumVoting.Models.Vote.Cast(project, voter, amount);
                    db.Votes.Add(vote);
                    db.SaveChanges();

                    List<int> list = new List<int>();

                    foreach (var item in project.Votes)
                    {

                        list.Add(item.amount);
                    }
                    int over_all_amount = list.Sum();
                    if (project.total_votes != over_all_amount)
                    {
                        project.total_votes = over_all_amount;
                        db.Entry(project).State = EntityState.Modified;
                        db.SaveChanges();
                    }








                    return RedirectToAction("success", new { id = voter.ID });
                }
                else
                {
                    return RedirectToAction("failure", new { id = voter.ID, amount = amount, project_id = project.ID, symposium_id = symposium.ID, min = min, max = max });
                }



                string error = "An Unexpected Error Occured";
                return RedirectToAction("project_vote", "Projects", new { id = project_id, message = error });

            }
            catch
            {
                return View("Projects", "random_error");
            }


            //   if (sym_voters == 0)
            //{
            //    string message = "This voter does not exist";
            //    return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
            //}
            
        }
            
            

        
        public ActionResult success(int? id)
        {
            int voter_amount = 0;
            Voter voter = db.Voters.Find(id);
            foreach (var item in voter.Votes)
            {

                voter_amount = voter_amount + item.amount;
            }
            ViewBag.amount_remaining = voter_amount;
            ViewBag.Symposium = db.Symposiums.Find(voter.symposiumID);
            return View(voter);
        }
        public ActionResult failure(int id, int amount, int project_id, int symposium_id, int min, int max)
        {
            Voter voter = db.Voters.Find(id);
            Project project = db.Projects.Find(project_id);
            Symposium symposium = db.Symposiums.Find(symposium_id);
            Boolean under_min = false;
            Boolean over_max = false;
            if (amount < min)
            {
                under_min = true;
            }
            else if (amount > max)
            {
                over_max = true;
            }
            Boolean under_min_over_max = voter.has_more_votes(symposium, project, amount, min, max);
            Boolean already_voted = voter.has_voted_for_project(project);
            Boolean not_registered = voter.not_registered_under_symposium(symposium);
            Boolean over_range = false;
            if (amount > 10)
            {
                over_range = true;
            }

            int voter_amount = 0;
            foreach (var item in voter.Votes)
            {

                voter_amount = voter_amount + item.amount;
            }
            int AmountRemaining = symposium.limit - voter_amount;
            ViewBag.Voter_amount = voter_amount;
            string message = "";
            if (symposium.vote_style == "amount")
                {
                    if(not_registered){
                        message = "This Voter ID does not exist under this symposium.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                    else if (already_voted)
    
                    {
                        message = "You have already voted for this project.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }

                    else if (voter_amount == symposium.limit)
                    {
                        message = "You have used all of your vote dollars.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                    else if (under_min)
                    {
                        message = "You have went under your minimum votes allowed for this project.";
                        return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                    else if (over_max)
                    {
                         message = "You have went over your maximum votes allowed for this project.";
                         return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }
                        else
                        {
                            message = "You went over your vote currency amount. You can only contribute " + AmountRemaining + " currency left.";
                            return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                        }
                }
                    else
                    {
                        if(not_registered){
                            message = "This Voter ID does not exist under this symposium.";
                            return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                        }
                        else if (already_voted)
                        {
                             message = "You Have Already voted for this project";
                             return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                        }
                        else if (over_range)
                        {
                            message = "You have entered a number over 10 please choose a number smaller.";
                            return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                        }
                        else { 
                         message = "You Have no more votes remaining, Thank you for voting!";
                         return RedirectToAction("project_vote", "Projects", new { id = project_id, message = message });
                    }

            //return View(voter);
        }
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
