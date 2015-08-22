using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SymposiumVoting.Models;
using System.Text;
using System.IO;
using Novacode;
using System.Diagnostics;

namespace SymposiumVoting.Controllers
{
    public class ProjectsController : Controller
    {
        private mainDBContext db = new mainDBContext();

        // GET: /Projects/

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                ViewBag.Courses = db.Courses.Distinct().ToList();
                ViewBag.Symposiums = db.Symposiums.ToList();
                return View(db.Projects.ToList());
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }


        public ActionResult QrCodeToWord(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                
                Symposium symposium = db.Symposiums.Find(id);

                ViewBag.Projects = symposium.Projects.ToList();
                return View(symposium);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        [WordDocument(DefaultFilename = "Projects")]
        public ActionResult ExportPlanToWord(int? id, string course_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (course_id != null && id != null)
                {
                    ViewBag.Projects = db.Projects.Where(p => p.CourseNumber == course_id && p.symposiumID == id).ToList();

                    Symposium symposium = db.Symposiums.Find(id);
                    ViewBag.Count = symposium.Projects.Count();
                    foreach (var item in symposium.Projects)
                    {
                        item.Course = db.Courses.Find(item.courseID);
                        db.Projects.Add(item);
                    }
                    db.SaveChanges();
                }
                else if (id != null)
                {
                    Symposium symposium = db.Symposiums.Find(id);
                    ViewBag.Count = symposium.Projects.Count();
                    ViewBag.Projects = symposium.Projects.ToList();
                    foreach (var item in symposium.Projects)
                    {
                        item.Course = db.Courses.Find(item.courseID);
                        db.Projects.Add(item);
                    }
                    db.SaveChanges();
                }
                else if (course_id != null)
                {
                    ViewBag.Count = db.Projects.Where(p => p.CourseNumber == course_id).Count();
                    ViewBag.Projects = db.Projects.Where(p => p.CourseNumber == course_id).ToList();
                    Course course = db.Courses.Where(c => c.courseNumber == course_id).FirstOrDefault();
                    foreach (var item in ViewBag.Projects)
                    {
                        item.Course = db.Courses.Find(item.courseID);
                        db.Projects.Add(item);
                    }
                    db.SaveChanges();
                }

                return PartialView("QrCodeToWord");
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }




        public ActionResult desktop_vote()
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
            bool val1 = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            ViewBag.user_exist = user_exist;
            Symposium symposium = db.Symposiums.Where(s => s.live == true).FirstOrDefault();
            ViewBag.Symposiums = db.Symposiums.Where(s => s.live == true).FirstOrDefault();
            if (symposium != null)
            {
                ViewBag.Projects = symposium.Projects.ToList();
            }
            return View();

        }

        public ActionResult projects_scoped_with_courses(int id)
        {

            Symposium symposium = db.Symposiums.Find(id);
            ViewBag.Symposium = symposium;
            var courses = db.Courses.Where(c => c.symposiumID == id);
            ViewBag.Courses = courses;
            List<Project> list = new List<Project>();
            foreach (var item in courses)
            {
             foreach(var project in item.Projects){
                    list.Add(project);
                }
                

            }
            ViewBag.Course_Projects = list.OrderByDescending(p => p.total_votes);
            return View(symposium);


        }
        [HttpPost, ActionName("upload_projects")]
        [ValidateAntiForgeryToken]
        public ActionResult upload_projects(HttpPostedFileBase projects, int symposium_id)
        {
            try
            {

            
                string filename = DateTime.Now.ToString("yyyy-MM-dd");
                string pathToSave = Server.MapPath(@"\StarVote\Content\");
                if (projects.ContentLength > 0)
                {
                    projects.SaveAs(Path.Combine(pathToSave, filename));
                }
                else
                {
                    return RedirectToAction("Details", "Symposiums", new { id = symposium_id });
                }
                var lines = System.IO.File.ReadAllLines(Server.MapPath(@"\StarVote\Content\" + filename)).Select(a => a.Split(';'));
                foreach (var CSVProject in lines)
                {
                    string courseNumber = CSVProject[1].ToString();
                    string courseName = CSVProject[2].ToString();
                    string Name = CSVProject[0].ToString();

                    if (db.Courses.Where(c => c.courseNumber == courseNumber && c.symposiumID == symposium_id).FirstOrDefault() == null)
                    {
                        Course course = new Course { courseName = courseName, courseNumber = courseNumber, symposiumID = symposium_id };
                        db.Courses.Add(course);
                        db.SaveChanges();
                    }
                    if (db.Projects.Where(p => p.Name == Name && p.symposiumID == symposium_id).FirstOrDefault() == null)
                    {
                        Project project = new Project()
                        {
                            Name = CSVProject[0],
                            CourseNumber = CSVProject[1],
                            students = CSVProject[3]
                        };
                        Course found_course = db.Courses.Where(c => c.courseNumber == courseNumber && c.symposiumID == symposium_id).FirstOrDefault();
                        project.symposiumID = symposium_id;
                        project.courseID = found_course.ID;
                        db.Projects.Add(project);
                    }

                }
                db.SaveChanges();
                return RedirectToAction("Details", "Symposiums", new { id = symposium_id });
                }
            catch
            {
                return View("upload_error");
            }
            


        }
        public ActionResult project_vote(int id, string message)
        {
            int project_id = id;
            Boolean session_present;
            Project project = db.Projects.Find(id);
            Symposium symposium = db.Symposiums.Find(project.symposiumID);
            ViewBag.Symposium = symposium;
            ViewBag.Toggle_remaining_button = false;
            if (Session["YourSessionKey"] == null)
            {
                session_present = false;
            }
            else
            {
                session_present = true;
                Voter[] sym_voters = symposium.Voters.ToArray();
                string session_string = Session["YourSessionKey"].ToString();
                Voter voter = sym_voters.Where(v => v.voting_id == session_string).FirstOrDefault();
                int voter_amount = 0;
                if (voter != null)
                {
                    foreach (var item in voter.Votes)
                    {

                        voter_amount = voter_amount + item.amount;
                    }
                    int remaining_amount = symposium.limit - voter_amount;
                    ViewBag.Remaining_Amount = remaining_amount;

                    if (remaining_amount <= symposium.min)
                    {
                        ViewBag.Toggle_remaining_button = true;
                    }
                }
                else
                {
                    Session["YourSessionKey"] = null;
                    session_present = false;
                    message = "Please enter a valid voting ID.";
                    ViewBag.Session_present = false;
                    ViewBag.Message = message;
                    return View(project);
                }
            }
            ViewBag.Project = project;
            ViewBag.Session_present = session_present; 
            ViewBag.Message = message;


            return View(project);

        }
        public ActionResult filter_buttons(int? symposium_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (symposium_id != null)
                {
                    Symposium symposium = db.Symposiums.Find(symposium_id);
                    ViewBag.Symposium = symposium;
                    ViewBag.Projects = symposium.Projects.ToList();
                    Project[] projects = symposium.Projects.Distinct().ToArray();
                    int array_length = projects.Count();
                    int[] course_ids = new int[array_length];
                    for (int i = 0; i < projects.Count(); i++)
                    {
                        course_ids[i] = projects[i].courseID;
                    }

                    Course[] found_courses = new Course[projects.Count()];

                    for (int i = 0; i < projects.Count(); i++)
                    {

                        found_courses[i] = db.Courses.Find(projects[i].courseID);
                    }
                    found_courses = found_courses.Distinct().ToArray();
                    ViewBag.Course_Count = found_courses.Count();
                    ViewBag.Courses = found_courses;

                }

                return View();
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }

        }
        public ActionResult radio_buttons(int? symposium_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (symposium_id != null)
                {
                    Symposium symposium = db.Symposiums.Find(symposium_id);
                    ViewBag.Symposium = symposium;
                    ViewBag.Projects = symposium.Projects.ToList();
                    Project[] projects = symposium.Projects.Distinct().ToArray();
                    int array_length = projects.Count();
                    int[] course_ids = new int[array_length];
                    for (int i = 0; i < projects.Count(); i++)
                    {
                        course_ids[i] = projects[i].courseID;
                    }

                    Course[] found_courses = new Course[projects.Count()];

                    for (int i = 0; i < projects.Count(); i++)
                    {

                        found_courses[i] = db.Courses.Find(projects[i].courseID);
                    }
                    found_courses = found_courses.Distinct().ToArray();
                    ViewBag.Course_Count = found_courses.Count();
                    ViewBag.Courses = found_courses;

                }


                return View();
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        public ActionResult scoped_projects(int? id, string course_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                Symposium symposium = db.Symposiums.Find(id);

                if (course_id == "" || course_id == null)
                {
                    ViewBag.Projects = db.Projects.Where(p => p.symposiumID == id).ToList();
                }
                else if (id == null)
                {
                    var courses = db.Courses.Where(c => c.courseNumber == course_id).ToList();
                    
                    int iterator = 0;
                    int count = 0;
                    foreach (var item in courses)
                    {
                        count = count + item.Projects.Count();
                    }
                    Project[] projects = new Project[count];
                    foreach (var item in courses)
                    {

                        
                        foreach (var project in item.Projects)
                        {

                            projects[iterator] = project;
                            iterator++;
                        }
                        
                    }
                    ViewBag.Projects = projects.ToList();
                }
                else
                {
                    ViewBag.Projects = null;
                    Course found_course = db.Courses.Where(c => c.courseNumber == course_id && c.symposiumID == id).FirstOrDefault();
                    ViewBag.Projects = db.Projects.Where(p => p.courseID == found_course.ID && p.symposiumID == id).ToList();
                }
                return View();
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }

        }
        public ActionResult dropdown_for_courses(int? symposium_id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (symposium_id == null)
                {
                    var symposiums = db.Symposiums.ToList();
                    int i = 0;
                    int count = 0;
                    foreach (var item in symposiums)
                    {

                        count = count + item.Courses.Count();
                    }
                    Course[] found_courses = new Course[db.Courses.Count()];
                    found_courses[0] = db.Courses.First();
                    int ix = 0;
                    var all_courses = db.Courses.ToList();
                    List<Course> list = new List<Course>();
                    foreach (var item in all_courses)
                    {
                        if (list.Where(x => x.courseNumber == item.courseNumber).FirstOrDefault() == null)
                        {
                            list.Add(item);
                            ix++;
                        }
                    }
                    
                       ViewBag.CourseNames = db.Courses.Select(x=> x.courseName).ToArray();
                  
                        ViewBag.CourseNumbers = db.Courses.Select(x => x.courseName);

                    ViewBag.Courses = list;
                    ViewBag.Course_Count = db.Courses.ToList().Count();
                }
                else
                {
                    Symposium found_symposium = db.Symposiums.Find(symposium_id);
                    Project[] projects = found_symposium.Projects.Distinct().ToArray();
                    int array_length = projects.Count();
                    int[] course_ids = new int[array_length];
                    for (int i = 0; i < projects.Count(); i++)
                    {
                        course_ids[i] = projects[i].courseID;
                    }

                    Course[] found_courses = new Course[projects.Count()];

                    for (int i = 0; i < projects.Count(); i++)
                    {

                        found_courses[i] = db.Courses.Find(projects[i].courseID);
                    }
                    found_courses = found_courses.Distinct().ToArray();

                    ViewBag.Course_Count = found_courses.Count();
                    ViewBag.Courses = found_courses;
                }
                return View();
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        public ActionResult code_generation(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = db.Projects.Find(id);

                if (project == null)
                {
                    return HttpNotFound();
                }
                return View(project);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // GET: /Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = db.Projects.Find(id);

                if (project == null)
                {
                    return HttpNotFound();
                }
                return View(project);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }
        

        // GET: /Projects/Create
        public ActionResult Create(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    Project project = new Project();
                    project.symposiumID = db.Symposiums.ToList().First().ID;
                    return View(project);
                }
                else
                {
                    Project project = new Project();
                    Symposium symposium = db.Symposiums.Find(id);
                    @ViewBag.Symposium = symposium;
                    project.symposiumID = symposium.ID;
                    return View(project);
                }
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");  
            }
        }

        // POST: /Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form, [Bind(Include = "ID,Name,CourseNumber,symposiumID,students")] Project project)
        {
            if (ModelState.IsValid)
            {

                string strNames = form["mytext[]"];
                project.students = strNames;
                 Symposium sym = db.Symposiums.Find(project.symposiumID);
                 project.Symposium = sym;
                string[] array = new string[3];
                array[0] = "john";
                array[1] = "Smith";
                array[2] = "Tommy";
                string result1 = ConvertStringArrayToString(array);
                result1 = result1.Remove(result1.Length - 2);
                db.Projects.Add(project);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = project.ID});
            }

            return View(project);
        }
        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(',').Append(' ');
            }
            return builder.ToString();
        }                                                                                       
        // GET: /Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = db.Projects.Find(id);
                if (project == null)
                {
                    return HttpNotFound();
                }
                return View(project);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");  
            }
        }

        // POST: /Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,CourseNumber,symposiumID,students")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: /Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = db.Projects.Find(id);
                if (project == null)
                {
                    return HttpNotFound();
                }
                return View(project);
            }
            else
            {
                return RedirectToAction("desktop_vote", "Projects");
            }
        }

        // POST: /Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Details", "Symposiums", new { id = project.symposiumID });
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
