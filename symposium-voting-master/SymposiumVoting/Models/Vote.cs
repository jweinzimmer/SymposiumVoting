using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SymposiumVoting.Models
{
    public class Vote
    {
        
        public int ID { get; set; }
        [Required]
        public int amount { get; set; }
        public Voter Voter { get; set; }
        public Project Project { get; set; }
        [ForeignKey("Voter")]
        public int voterID { get; set; }
        [ForeignKey("Project")]
        public int projectID { get; set; }
        public string created_at { get; set; }

        public static Vote Cast(Project project, Voter voter, int amount)
        {
            Vote vote = new Vote();
            vote.amount = amount;
            vote.voterID = voter.ID;
            vote.projectID = project.ID;
            vote.created_at = DateTime.Now.ToString("HH:mm");
            return vote;

        }
    }
}

