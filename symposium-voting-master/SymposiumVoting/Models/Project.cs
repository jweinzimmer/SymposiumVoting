using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace SymposiumVoting.Models
{
    public class Project
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CourseNumber { get; set; }
        public string students { get; set; }
        [ForeignKey("Symposium")]
        public int symposiumID { get; set; }
        public Symposium Symposium { get; set; }

        [ForeignKey("Course")]
        public int courseID { get; set; }
        public Course Course { get; set; }
        public int total_votes { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }


    }


    public class mainDBContext : IdentityDbContext<ApplicationUser>
    {
        public mainDBContext()
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Symposium> Symposiums { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Voter> Voters { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Vote>()
                .HasRequired(vote => vote.Project)
                .WithMany(project => project.Votes)
                .HasForeignKey(vote => vote.projectID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasRequired(project => project.Course)
                .WithMany(course => course.Projects)
                .HasForeignKey(project => project.courseID)
                .WillCascadeOnDelete(false);

            

            base.OnModelCreating(modelBuilder);
       
            
        }

    }
}

