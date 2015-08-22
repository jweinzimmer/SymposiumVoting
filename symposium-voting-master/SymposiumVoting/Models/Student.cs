using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SymposiumVoting.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string course_id { get; set; }
        public string project_name { get; set; }
        public string semester { get; set; }
    }
}