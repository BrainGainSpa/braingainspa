using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using braingainspa.Models;

namespace braingainspa.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult myIndex()
        {
            if (Session["Rem_Time"] == null)
            {
                Session["Rem_Time"] = DateTime.Now.AddMinutes(2).AddSeconds(20).ToString("dd-MM-yyyy h:mm:ss tt");
            }
            ViewBag.Rem_Time = Session["Rem_Time"];

            ViewBag.Message = "Modify this template to jump-start your MVC application.";

            return View();
            
        }

        public ActionResult CountUpTimer()
        {

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Default()
        {
            try
            {
                TempData["PID"] = null;
                TempData["RoleID"] = null;
                TempData["Verified"] = null;
                TempData["FN"] = null;
                TempData["LN"] = null;
                TempData["ReferreeCode"] = null;
                TempData["EntityType"] = null;
                TempData["Subscription"] = null;
                TempData["Email"] = null;
                TempData["Subscription"] = null;
                Session["PID"] = null;
            }
            catch (Exception ex)
            {

            }

            //FormsAuthentication.SignOut();
            return View();
        }

        [HttpPost]
        public ActionResult Default(int ot)
        {
            try
            {
                TempData["PID"] = null;
                TempData["RoleID"] = null;
                TempData["Verified"] = null;
                TempData["FN"] = null;
                TempData["LN"] = null;
                TempData["ReferreeCode"] = null;
                TempData["EntityType"] = null;
                TempData["Subscription"] = null;
                TempData["Email"] = null;
                TempData["Subscription"] = null;
                Session["PID"] = null;
            }
            catch (Exception ex)
            {

            }

            FormsAuthentication.SignOut();
            //Session.Abandon();
            //TempData.Add("Verified", 0);
            return View();
        }

        public ActionResult QuizTimer()
        {
            Exam exam = new Exam() { Id = 1, Details = "exameDetails", ReaminTime = new TimeSpan(0, 5, 0) };


            return View(exam);
        }

        public string CreateExam(Exam e1)
        {
            //Save the data into database
            return e1.ReaminTime.ToString();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SkillPractice()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult PastQuestions()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ELibrary()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ReferralPlans()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult BraingainFaces()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Testimonials()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FAQ()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Sitemap()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Privacy()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Terms()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Support()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ReferFriend()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Scholarship()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult EducationalGames()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult News()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult EmekaAlbert()
        {
            //ViewBag.Message = "Your contact page.";
            
            return View();
        }

        public ActionResult KennethBulus()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult KingsleyEbhodaghe()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult StellaObe()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult VirtueOduniyi()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult OluwafemiOduniyi()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GhalibFahad()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }
        
    }
}