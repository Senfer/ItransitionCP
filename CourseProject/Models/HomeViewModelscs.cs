using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CourseProject.Models
{
    public class HomeViewModel 
    {
        public System.Collections.Generic.IEnumerable<UserTask> LatestTasks { get; set; }
        public System.Collections.Generic.IEnumerable<UserTask> RatedTasks { get; set; }
        public System.Collections.Generic.IEnumerable<UserTask> UnsolvedTasks { get; set; }
        public System.Collections.Generic.IEnumerable<ApplicationUser> RatedUsers { get; set; }
    }

    
}