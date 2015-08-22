using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace SymposiumVoting.Models
{
    public class Course
    {


        public int ID { get; set; }
        [Required]
        public string courseNumber { get; set; }
        [Required]
        public string courseName { get; set; }
        [ForeignKey("Symposium")]
        public int symposiumID { get; set; }
        public Symposium Symposium { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

        public IEnumerable<Project> GetProjects()
        {
            return this.Projects.OrderByDescending(x => x.total_votes).ToList();
        }

    }
}
