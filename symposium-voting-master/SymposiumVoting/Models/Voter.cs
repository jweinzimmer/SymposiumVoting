using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SymposiumVoting.Models
{
    public class Voter
    {
        public int ID { get; set; }
        public string voting_id { get; set; }
        public  virtual Symposium Symposium { get; set; }
        [ForeignKey("Symposium")]
        public int symposiumID { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
        
        public Boolean has_more_votes(Symposium symposium, Project project, int amount, int min, int max)
        {
            int voter_amount = 0;
            
            foreach (var item in this.Votes)
            {
                
                voter_amount = voter_amount + item.amount;
            }
            int combined_amount = voter_amount + amount;
            int remaining_amount = symposium.limit - voter_amount;
            string symposium_vote_style = symposium.vote_style;
            switch (symposium_vote_style)
            {
                case "standard":
                   if (this.Votes.Count > symposium.limit)
                    {
                        return false;
                    }
                   break;
                case "range":
                   if (amount > 10)
                   {
                       return false;

                   }
                   else if (this.Votes.Count > symposium.limit)
                    {
                        return false;
                    }
                   break;
                case "amount":

                    if (combined_amount > symposium.limit)
                    {
                        return false;
                    }
                    if (remaining_amount > min)
                    {
                         if (amount < min)
                        {
                            return false;
                        }
                     }
                    else if (amount > max)
                    {
                        return false;
                    }

                    break;
               
            }
            return true;

        }

        public Boolean has_not_voted_for_project(Project project)
        {
            foreach (var item in this.Votes)
            {
                if(item.projectID == project.ID)
                {
                    return false;
                }
            }
            return true;

        }
        public Boolean has_voted_for_project(Project project)
        {
            foreach (var item in this.Votes)
            {
                if (item.projectID == project.ID)
                {
                    return true;
                }
 
            }
            return false;

        }

        public Boolean registered_under_symposium(Symposium symposium)

        {
            if(this.symposiumID == symposium.ID)
            {
                return true;
            }
            return false;

        }

        public Boolean not_registered_under_symposium(Symposium symposium)
        {
            if (this.symposiumID == symposium.ID)
            {
                return false; 
            }
            return true;

        }

        public Boolean is_eligible_to_vote(Symposium symposium, Project project, int amount, int min, int max)
        {
            return (this.registered_under_symposium(symposium) &
           this.has_more_votes(symposium, project, amount, min, max) &
           this.has_not_voted_for_project(project));
        }
   
}
   
  
    

}
