using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CourseProject.Models;
//using MarkdownSharp;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index( )
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            HomeViewModel Model = new HomeViewModel();
            int Length = 10;
            if (DB.Tasks.Any(c => c.Deleted == false))
            {
                List<UserTask> Active = DB.Tasks.Where(c => c.Deleted == false).ToList();

                List<UserTask> Reversed = Active;
                Reversed.Reverse();
                if (Reversed.Count < 10)
                    Length = Reversed.Count;
                Model.LatestTasks = Reversed.Take(Length);

                Length = 10;
                List<UserTask> Rated = Active.Where(c => c.TaskRatingCount > 0).ToList();
                if (Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).ToList().Count < 10)
                    Length = Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).ToList().Count;
                Model.RatedTasks = Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).Take(Length);


                Length = 10;
                if (Active.Where(c => c.SolveCount == 0).ToList().Count < 10)
                    Length = Active.Where(c => c.SolveCount == 0).ToList().Count;
                Model.UnsolvedTasks = Active.Where(c => c.SolveCount == 0).Take(Length);


                Length = 10;
                System.Collections.Generic.IEnumerable<ApplicationUser> RatedUsers = DB.Users.Where(c => c.Rating > 0);
                if (RatedUsers.OrderByDescending(c => c.Rating).ToList().Count < 10)
                    Length = RatedUsers.OrderByDescending(c => c.Rating).ToList().Count;

                Model.RatedUsers = RatedUsers.OrderByDescending(c => c.Rating).Take(Length);

                
            }
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            HttpCookie NewCookie = new HttpCookie("Nickname");
            NewCookie.Value = DB.Users.First(c => c.Id == CurrentUserID).NickName;
            HttpContext.Response.Cookies.Add(NewCookie);
            return View(Model);
            
        }

        public ActionResult About( )
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact( )
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FullTable()
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            System.Collections.Generic.IEnumerable<UserTask> Model = DB.Tasks.Where(c=>c.Deleted==false).AsEnumerable();
            return View(Model);
        }

        


        [HttpGet]
        public string CompanionGetAllTasks()
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            List<UserTask> Temp = DB.Tasks.Where(c=>c.UserTaskID>0).ToList();           

            
            

            XmlSerializer xsSubmit = new XmlSerializer(typeof(List<UserTask>));
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, Temp);
                var xml = sww.ToString();
                return xml;
            }
            
        }


        [HttpGet]
        public string CompanionGetTask(int id, string UserID)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            UserTask Temp = DB.Tasks.First(c => c.UserTaskID == id);
            if (UserID == Temp.UserID)
                return "YOU CREATED THIS TASK";
            else
            {
                if (DB.Solves.Any(c => c.TaskID == id && c.UserID == UserID) == true)
                    return "YOU ALREADY SOLVED THIS";
                else
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(UserTask));
                    using (StringWriter sww = new StringWriter())
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, Temp);
                        var xml = sww.ToString();
                        return xml;
                    }
                }
            }

        }

        [HttpGet]
        public string CompanionSolvingTask(string Answer, int id, string UserID)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            if (DB.Answers.Any(c => c.AnswerText == Answer && c.TaskID == id) == true)
            {
                if (Answer != "" && Answer != null)
                {
                    if (DB.Answers.Where(c => (c.TaskID == id) && (c.AnswerText == Answer)).ToList().Count != 0)
                    {
                        Solves NewSolve = new Solves();
                        NewSolve.TaskID = id;
                        NewSolve.UserID = UserID;
                        DB.Solves.Add(NewSolve);
                        DB.Tasks.First(c => c.UserTaskID == id).SolveCount++;
                        DB.Entry(NewSolve).State = System.Data.Entity.EntityState.Added;
                        DB.SaveChanges();
                    }
                }
                return "success";
            }
            else
                return "fail";
        }


        [HttpPost]
        public void ChangeNickname(string value)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            DB.Users.First(c => c.Id == CurrentUserID).NickName = value;
            DB.SaveChanges();
            Response.Cookies["Nickname"].Value = value;
        }

       /* [HttpGet]
        public string CompanionNeedsView(int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            Markdown translator = new Markdown();
            return translator.Transform(DB.Tasks.First(c => c.UserTaskID == id).TaskText);
 
        }  */
        
    }
}