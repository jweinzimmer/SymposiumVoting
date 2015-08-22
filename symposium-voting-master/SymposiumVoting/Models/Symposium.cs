using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SymposiumVoting.Models
{
    public class Symposium
    {
        public int ID { get; set; }
        [Required]
        public string year { get; set; }
        [Required]
        public string semester { get; set; }
        [Required]
        public string vote_style {get; set;}
        [Required]
        public int limit { get; set; }
        [Required]
        public int min { get; set; }
        [Required]
        public int max { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool live { get; set; }
        public int user_id { get; set; }
   
        

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Voter> Voters { get; set; }

        public IEnumerable<Project> GetProjects()
        {
            return this.Projects.OrderBy(x => x.Name).ToList();
        }
    }
}