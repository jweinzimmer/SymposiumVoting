using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymposiumVoting.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        [ChildActionOnly]
        public ActionResult layout()
        {
            bool user_exist;
            if (Session["userLogin"] != null)
            {
                user_exist = true;
            }
            else
            {
                user_exist = false;
            }
            ViewBag.user_exist = user_exist;

            return new ContentResult { Content = ViewBag.user_exist.ToString() };
        }
    }
}