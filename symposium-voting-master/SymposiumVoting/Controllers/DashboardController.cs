using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymposiumVoting.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attendee()
        {

            return View();
        }

        public ActionResult Student()
        {

            return View();
        }
        public ActionResult Create_project()
        {

            return View();
        }

        public ActionResult Create_attendee()
        {

            return View();
        }
	}
}