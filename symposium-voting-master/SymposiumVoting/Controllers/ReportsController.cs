using SymposiumVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymposiumVoting.Controllers
{

    public class ReportsController : Controller
    {
        private mainDBContext db = new mainDBContext();
        // GET: Reports
        public ActionResult Index(int symposium_id)
        {
            return View();
        }

        public ActionResult render_line(int symposium_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                int[] int_arry = new int[4] { 1, 7, 7000, 10000 };

                Symposium found_symposium = db.Symposiums.Find(symposium_id);
                int vote_count = 0;
                foreach (var item in found_symposium.Projects)
                {
                    vote_count = vote_count + item.Votes.Count();

                }
                Vote[] found_symposium_votes = new Vote[vote_count];
                int q = 0;
                foreach (var item in found_symposium.Projects)
                {
                    foreach (var vote in item.Votes)
                    {
                        found_symposium_votes[q] = vote;
                        q++;
                    }
                }
                var all_votes = found_symposium_votes.OrderBy(v => v.created_at).ToList();
                DateTime[] times = new DateTime[all_votes.Count()];
                int i = 0;
                foreach (var timestamp in all_votes)
                {

                    times[i] = Convert.ToDateTime(timestamp.created_at);
                    i++;

                }
                ViewBag.Votes = all_votes;
                Symposium symposium = db.Symposiums.Find(symposium_id);
                var array = times.GroupBy(y => (int)(y.Ticks / TimeSpan.TicksPerMinute / 10)).Select(g => new { Value = g.Key, Count = g.Count() }).OrderBy(x => x.Value);
                if (array.FirstOrDefault() != null && array.Count() > 1)
                {
                    String[] x_axis = new String[times.Count()];
                    int iterator = 0;
                    foreach (var item in array)
                    {
                        long new_tick = convert_interval(item.Value);
                        DateTime myDate = new DateTime(new_tick);
                        int minutes = myDate.Minute;
                        String vote_date = myDate.ToString("hh:mm" + "-" + (minutes + 10));
                        x_axis[iterator] = vote_date;
                        iterator++;
                    }
                    DateTime am_or_pm;
                    if (array.FirstOrDefault() != null)
                    {
                        am_or_pm = new DateTime(convert_interval(array.First().Value));
                        string tt = am_or_pm.ToString("tt");
                        ViewBag.Morning_or_Evening = tt;
                    }


                    int[] counts = new int[array.Count()];
                    int r = 0;
                    foreach (var item in array)
                    {
                        counts[r] = item.Count;
                        r++;
                    }
                    ViewBag.Counts = counts;
                    var vote_intervals = x_axis.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    ViewBag.line_x_axis = vote_intervals;
                    return View(symposium);
                }
                else
                {
                    return RedirectToAction("no_information", "Reports");
                }
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        public ActionResult no_information()
        {
            return View();
        }
        public long convert_interval(int ticks)
        {
            long new_tick = ticks * (TimeSpan.TicksPerMinute * 10);
            return new_tick;
        }
        public ActionResult render_pie(int symposium_id)
        {
            Symposium symposium = db.Symposiums.Find(symposium_id);
            return View(symposium);
        }
        public ActionResult render_polar(int symposium_id)
        {
            Symposium symposium = db.Symposiums.Find(symposium_id);
            return View(symposium);
        }
        public ActionResult render_bar(int symposium_id)
        {
            Symposium symposium = db.Symposiums.Find(symposium_id);
            return View(symposium);
        }
        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
