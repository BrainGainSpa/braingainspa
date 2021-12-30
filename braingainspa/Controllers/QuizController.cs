using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using braingainspa.Models;
using ClosedXML.Excel;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;

namespace braingainspa.Controllers
{
    enum OptionLetter
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5
    }

    enum Expletives
    {
        CORRECT,
        GOOD,
        VERY_GOOD,
        BRILLIANT,
        WONDERFUL,
        EXCELLENT,
        PERFECT,
        GREAT,
        SUPERB,
        FANTASTIC,
        IMPRESSIVE,
        AWESOME,
        AMAZING,
        EXCEPTIONAL,
        WOW,
        YOU_ARE_GREAT,
        KEEP_MASTERING,
        KEEP_IT_UP,
        OUTSTANDING,
        YOU_ARE_A_STAR
    }

    public class QuizController : Controller
    {
        int sklid = 0;
        static int pos;
        static int neg;
        static bool blansflag = false;

        static int? totalMarks = 0, tmpTotalMarks = 0; // Total Score
        static int POSQuestions = 0; //Questions answered correctly
        static int NEGQuestions = 0; //Questions answered Incorrectly
        static int AttemptedQuestions = 0; //Number of Questions attempted
        static int Scored = 0; //Numbers that have been scored

        static int skipQue = 0;
        static int LastQueNo = 0;
        static int posQue = 0;
        static int negQue = 0;
        static bool ansflag = false;
        static string strAnsSel = "false";
        static int expletive = 0;
        static bool bldisplayinterver = false;
        static bool blsubmitflag;

        static int intSubmit = 0;

        int? qno = 0;

        string strorigques = string.Empty, strorigansexp = string.Empty;

        //-------------------Session Variables------------------------------------//
        static bool blSubmit = false;
        static string varExplitive = String.Empty;
        static string varCorrectAnswer = String.Empty;
        static string varInCorrectAnswer = String.Empty;
        static string varAnswerExplanation = String.Empty;
        //-------------------Session Variables------------------------------------//

        static int AnsID = 0;
        static string FIGSlot1 = String.Empty;
        static string FIGSlot2 = String.Empty;
        static string FIGSlot3 = String.Empty;
        static string FIGSlot4 = String.Empty;

        static bool firsttime = false;

        static bool blVerified = false;
        static string verifiedfilepath = String.Empty;

        private List<SelectListItem> ddlYears = new List<SelectListItem>();

        // GET Quiz
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewSchool()
        {
            ModelState.Clear();

            return View("SchoolDetails");
        }

        [HttpGet]
        public ActionResult NewSchool(School model)
        {
            ViewBag.Save = "New";
            ModelState.Clear();
            AllSchools();
            return View("SchoolDetails", model);
        }

        private ActionResult AllSchools()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //SchoolViewModels model = new SchoolViewModels();

                List<SchoolViewModels> skllist = nqz.Schools.Where(x => x.IsDeleted == false).Select(x => new SchoolViewModels { SchoolID = x.SchoolID, SchoolName = x.SchoolName, Address = x.Address, Phone = x.Phone, Email = x.Email, Website = x.Website }).ToList();
                ViewBag.sklList = skllist.OrderBy(x => x.SchoolName);


            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

            return null;
        }

        [HttpGet]
        public ActionResult SchoolDetails()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                AllSchools();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public async Task<ActionResult> SchoolDetails(School model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (model.SchoolID <= 0)
                {
                    School nskl = new School()
                    {
                        SchoolName = model.SchoolName,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        Website = model.Website,
                        IsDeleted = false,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };
                    nqz.Schools.Add(nskl);
                    await nqz.SaveChangesAsync();

                    ViewBag.Msg = "Record Saved Successfully";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        School nskl = nqz.Schools.SingleOrDefault(x => x.IsDeleted == false && x.SchoolID == model.SchoolID);

                        nskl.SchoolName = model.SchoolName;
                        nskl.Address = model.Address;
                        nskl.Phone = model.Phone;
                        nskl.Email = model.Email;
                        nskl.Website = model.Website;
                        nskl.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();

                        ViewBag.Msg = "Update Completed Successfully";
                    }

                }
                AllSchools();
                ModelState.Clear();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult Schools()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                AllSchools();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }



        [HttpPost]
        public async Task<ActionResult> Schools(School model)
        {

            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (model.SchoolID <= 0)
                {
                    if (model.SchoolName == null)
                    {
                        ViewBag.Err = "Error! Enter A School Name";
                    }
                    else if (model.Address == null)
                    {
                        ViewBag.Err = "Error! Enter the Address";
                    }
                    else if (model.Phone == null)
                    {
                        ViewBag.Err = "Error! Enter a Phone Number";
                    }
                    else if (model.Email == null)
                    {
                        ViewBag.Err = "Error! Enter an Email";
                    }
                    else
                    {
                        School nskl = new School()
                        {
                            SchoolName = model.SchoolName,
                            Address = model.Address,
                            Phone = model.Phone,
                            Email = model.Email,
                            Website = model.Website,
                            IsDeleted = false,
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };
                        nqz.Schools.Add(nskl);
                        await nqz.SaveChangesAsync();

                        ViewBag.Msg = "Record Saved Successfully";
                    }
                }

                else
                {
                    if (ModelState.IsValid)
                    {
                        School nskl = nqz.Schools.SingleOrDefault(x => x.IsDeleted == false && x.SchoolID == model.SchoolID);

                        nskl.SchoolName = model.SchoolName;
                        nskl.Address = model.Address;
                        nskl.Phone = model.Phone;
                        nskl.Email = model.Email;
                        nskl.Website = model.Website;
                        nskl.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();

                        ViewBag.Msg = "Update Completed Successfully";
                    }

                }
                AllSchools();
                ModelState.Clear();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult EditSchool(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            School model = new School();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                School nskl = nqz.Schools.SingleOrDefault(x => x.IsDeleted == false && x.SchoolID == id);

                model.SchoolID = nskl.SchoolID;
                model.SchoolName = nskl.SchoolName;
                model.Address = nskl.Address;
                model.Phone = nskl.Phone;
                model.Email = nskl.Email;
                model.Website = nskl.Website;

                AllSchools();

                return View("Schools", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public async Task<ActionResult> DeleteSchool(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                School nskl = nqz.Schools.SingleOrDefault(x => x.SchoolID == id);
                if (nskl != null)
                {
                    nskl.IsDeleted = true;
                    nskl.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }


                ViewBag.Msg = "Row Delete Completed Successfully";


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity "; //+ ex.InnerException;
                }
                else
                {
                    ViewBag.Err = "An Error Occurred: "; //+ ex.InnerException;
                }
            }
            AllSchools();
            return RedirectToAction("Schools", "Quiz");
        }

        public ActionResult SuperAdminDashboard()
        {

            return View();
        }
        public ActionResult AdminDashboard()
        {

            return View();
        }
        public ActionResult ManagementDashboard()
        {

            return View();
        }
        public ActionResult StudentDashboard()
        {

            return View();
        }
        public ActionResult ParentDashboard()
        {

            return View();
        }
        public ActionResult ClientDashboard()
        {
            QuestionArchives();

            return View();
        }

        //----------------------------------------------FACULTY REGION---------------------------------------------------------------//
        [HttpGet]
        public ActionResult Faculties()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.SchoolList = nqz.Schools.Where(x => x.IsDeleted == false).Select(x => new { x.SchoolID, x.SchoolName }).ToList();
                ViewBag.ResourceTypeList = nqz.ResourceTypes.Where(x => x.IsDeleted == false).Select(x => new { x.ResourceTypeID, x.ResourceTypeName }).ToList();
                Session["SchoolList"] = ViewBag.SchoolList;
                Session["ResourceTypeList"] = ViewBag.ResourceTypeList;

                AllFaculties(0);
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public async Task<ActionResult> Faculties(Faculty model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (ViewBag.SchoolList == null)
                {
                    ViewBag.SchoolList = Session["SchoolList"];
                }
                if (ViewBag.ResourceTypeList == null)
                {
                    ViewBag.ResourceTypeList = Session["ResourceTypeList"];
                }
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (model.FacultyID <= 0)
                {
                    if (model.SchoolID > 0 && model.FacultyName == null)
                    {
                        AllFaculties(model.SchoolID.Value);
                    }
                    else
                    {
                        Faculty dept = nqz.Faculties.FirstOrDefault(x => x.SchoolID == model.SchoolID && x.IsDeleted == false && x.FacultyName == model.FacultyName);
                        if (dept == null)
                        {
                            if (model.SchoolID == null)
                            {
                                ViewBag.Err = "Error! Select a School ID";
                            }
                            else if (model.FacultyName == null)
                            {
                                ViewBag.Err = "Error! Enter A Faculty Name";
                            }
                            else
                            {
                                Faculty nfac = new Faculty()
                                {
                                    SchoolID = model.SchoolID,
                                    FacultyName = model.FacultyName,
                                    ResourceTypeID = model.ResourceTypeID,
                                    Description = model.Description,
                                    IsDeleted = false,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now
                                };
                                nqz.Faculties.Add(nfac);
                                await nqz.SaveChangesAsync();

                                ViewBag.Msg = "Record Saved Successfully";

                                AllFaculties(model.SchoolID.Value);
                                ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.Err = "This Resource Already Exists for the selected Class, therefore, Save transaction failed";
                        }
                    }

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Faculty dept = nqz.Faculties.FirstOrDefault(x => x.SchoolID == model.SchoolID && x.IsDeleted == false && x.FacultyName == model.FacultyName);
                        if (dept != null)
                        {
                            Faculty nfac = nqz.Faculties.SingleOrDefault(x => x.IsDeleted == false && x.FacultyID == model.FacultyID);
                            nfac.SchoolID = model.SchoolID;
                            nfac.FacultyName = model.FacultyName;
                            nfac.ResourceTypeID = model.ResourceTypeID;
                            nfac.Description = model.Description;
                            nfac.Updated = DateTime.Now;
                            await nqz.SaveChangesAsync();

                            ViewBag.Msg = "Record Updated Successfully";

                            AllFaculties(model.SchoolID.Value);
                            ModelState.Clear();
                        }
                        else
                        {
                            ViewBag.Err = "This Resource Already Exists for the selected Class, therefore, Save transaction failed";
                        }
                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult NewFaculties(Faculty model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (ViewBag.SchoolList == null)
                {
                    ViewBag.SchoolList = Session["SchoolList"];
                }
                if (ViewBag.ResourceTypeList == null)
                {
                    ViewBag.ResourceTypeList = Session["ResourceTypeList"];
                }
                ModelState.Clear();
                AllFaculties(0);
                return View("Faculties", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        private ActionResult AllFaculties(int sklid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (sklid > 0)
                {
                    var faclist = (from skl in nqz.Schools
                                   join fac in nqz.Faculties on skl.SchoolID equals fac.SchoolID
                                   join res in nqz.ResourceTypes on fac.ResourceTypeID equals (res.ResourceTypeID)
                                   where fac.IsDeleted == false && skl.SchoolID == 1
                                   select new FacultyViewModels
                                   {
                                       FacultyID = fac.FacultyID,
                                       SchoolID = skl.SchoolID,
                                       SchoolName = skl.SchoolName,
                                       FacultyName = fac.FacultyName,
                                       ResourceTypeID = res.ResourceTypeID,
                                       ResourceTypeName = res.ResourceTypeName,
                                       Description = fac.Description
                                   }).ToList();
                    ViewBag.FacultyList = faclist.OrderBy(x => x.SchoolName);
                }
                else
                {
                    var faclist = (from skl in nqz.Schools
                                   join fac in nqz.Faculties on skl.SchoolID equals fac.SchoolID
                                   join res in nqz.ResourceTypes on fac.ResourceTypeID equals (res.ResourceTypeID)
                                   where fac.IsDeleted == false && skl.SchoolID == 1
                                   select new FacultyViewModels
                                   {
                                       FacultyID = fac.FacultyID,
                                       SchoolID = skl.SchoolID,
                                       SchoolName = skl.SchoolName,
                                       FacultyName = fac.FacultyName,
                                       ResourceTypeID = res.ResourceTypeID,
                                       ResourceTypeName = res.ResourceTypeName,
                                       Description = fac.Description
                                   }).ToList();
                    ViewBag.FacultyList = faclist.OrderBy(x => x.SchoolName);
                }
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult EditFaculty(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            Faculty model = new Faculty();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (ViewBag.SchoolList == null)
                {
                    ViewBag.SchoolList = Session["SchoolList"];
                }
                if (ViewBag.ResourceTypeList == null)
                {
                    ViewBag.ResourceTypeList = Session["ResourceTypeList"];
                }

                Faculty nfac = nqz.Faculties.SingleOrDefault(x => x.IsDeleted == false && x.FacultyID == id);

                model.FacultyID = nfac.FacultyID;
                model.SchoolID = nfac.SchoolID;
                sklid = nfac.SchoolID.Value;
                model.FacultyName = nfac.FacultyName;
                model.ResourceTypeID = nfac.ResourceTypeID;
                model.Description = nfac.Description;

                AllFaculties(model.SchoolID ?? 0);
                return View("Faculties", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");

            }


        }



        [HttpGet]
        public async Task<ActionResult> DeleteFaculty(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Faculty nfac = nqz.Faculties.SingleOrDefault(x => x.FacultyID == id);
                if (nfac != null)
                {
                    sklid = nfac.SchoolID.Value;
                    nfac.IsDeleted = true;
                    nfac.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }
                ViewBag.Msg = "Delete Completed Successfully";
                AllFaculties(sklid);
                return RedirectToAction("Faculties", "Quiz");

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        //-------------------------------------------------Deoartments Region------------------------------------------------------

        [HttpGet]
        public ActionResult Departments()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                AllDepartments(0);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
                }
                else
                {
                    ViewBag.Err = "";
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Departments(Department model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                if (model.DepartmentID <= 0)
                {
                    if (model.FacultyID > 0 && model.DepartmentName == null)
                    {

                    }
                    else
                    {
                        Department dept = nqz.Departments.SingleOrDefault(x => x.FacultyID == model.FacultyID && x.IsDeleted == false && x.DepartmentName == model.DepartmentName);
                        if (dept == null)
                        {
                            if (model.FacultyID == null)
                            {
                                ViewBag.Err = "Error! Select a Faculty ID";
                            }
                            else if (model.DepartmentName == null)
                            {
                                ViewBag.Err = "Error! Enter A Department Name";
                            }
                            else
                            {
                                Department ndept = new Department()
                                {
                                    //DepartmentID = model.DepartmentID,
                                    FacultyID = model.FacultyID,
                                    DepartmentName = model.DepartmentName,
                                    Description = model.Description,
                                    IsDeleted = false,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now
                                };
                                nqz.Departments.Add(ndept);
                                await nqz.SaveChangesAsync();

                                ViewBag.Msg = "Record Saved Successfully";
                            }
                        }
                        else
                        {
                            ViewBag.Err = "This Department Already Exists for the selected Faculty, therefore, Save transaction aborted";
                        }

                        AllDepartments(model.FacultyID.Value);
                        ModelState.Clear();
                    }
                }

                else
                {
                    if (ModelState.IsValid)
                    {
                        Department nfac = nqz.Departments.SingleOrDefault(x => x.IsDeleted == false && x.DepartmentID == model.DepartmentID);

                        nfac.FacultyID = model.FacultyID;
                        nfac.DepartmentName = model.DepartmentName;
                        nfac.Description = model.Description;
                        nfac.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();

                        ViewBag.Msg = "Update Completed Successfully";

                        AllDepartments(model.FacultyID.Value);
                        ModelState.Clear();
                    }

                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
                }
                else
                {
                    ViewBag.Err = "";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult NewDepartments(Department model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();
                ModelState.Clear();
                AllDepartments(0);
            }
            catch (Exception ex)
            {

            }
            return View("Departments", model);

        }

        private ActionResult AllDepartments(int facid)
        {

            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (facid <= 0)
                    {
                        var deplist = (from fac in nqz.Faculties
                                       join dep in nqz.Departments on fac.FacultyID equals dep.FacultyID
                                       where dep.IsDeleted == false
                                       orderby fac.FacultyName
                                       select new DepartmentViewModels
                                       {
                                           DepartmentID = dep.DepartmentID,
                                           FacultyID = fac.FacultyID,
                                           FacultyName = fac.FacultyName,
                                           DepartmentName = dep.DepartmentName,
                                           Description = dep.Description
                                       }).ToList();

                        ViewBag.DepartmentList = deplist;
                    }
                    else
                    {
                        var deplist = (from fac in nqz.Faculties
                                       join dep in nqz.Departments on fac.FacultyID equals dep.FacultyID
                                       where dep.IsDeleted == false && fac.FacultyID == facid
                                       orderby fac.FacultyName
                                       select new DepartmentViewModels
                                       {
                                           DepartmentID = dep.DepartmentID,
                                           FacultyID = fac.FacultyID,
                                           FacultyName = fac.FacultyName,
                                           DepartmentName = dep.DepartmentName,
                                           Description = dep.Description
                                       }).ToList();

                        ViewBag.DepartmentList = deplist;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
                }
                else
                {
                    ViewBag.Err = "An Error Occurred: " + ex.InnerException;
                }
            }

            return null;
        }



        [HttpGet]
        public ActionResult EditDepartment(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            Department model = new Department();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                Department nfac = nqz.Departments.SingleOrDefault(x => x.IsDeleted == false && x.DepartmentID == id);

                model.DepartmentID = nfac.DepartmentID;
                model.FacultyID = nfac.FacultyID;
                sklid = nfac.FacultyID.Value;
                model.DepartmentName = nfac.DepartmentName;
                model.Description = nfac.Description;

                //ViewBag.Msg = "Update Completed Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
                }
                else
                {
                    ViewBag.Err = "An Error Occurred: " + ex.InnerException;
                }

            }
            AllDepartments(sklid);
            return View("Departments", model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Department nfac = nqz.Departments.SingleOrDefault(x => x.DepartmentID == id);
                if (nfac != null)
                {
                    sklid = nfac.FacultyID.Value;
                    nfac.IsDeleted = true;
                    nfac.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }


                ViewBag.Msg = "Delete Completed Successfully";
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
                {
                    //return RedirectToAction("Login", "Account");
                    ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
                }
                else
                {
                    ViewBag.Err = "An Error Occurred: " + ex.InnerException;
                }
            }

            AllDepartments(sklid);
            return RedirectToAction("Departments", "Quiz");
        }

        //-------------------------------------------------Courses Region-------------------------------------------------------
        [HttpGet]
        public ActionResult Courses()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.SchoolID == 1 && x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();
                Session["FacultyList"] = ViewBag.FacultyList;
                //ViewBag.DepartmentList = nqz.Departments.Where(x => x.IsDeleted == false).Select(x => new { x.DepartmentID, x.DepartmentName }).ToList();

                AllCourses(0);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Courses(Cours model)
        {
            ViewBag.Msg = null;
            ViewBag.Err = null;

            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {


                    ViewBag.FacultyList = nqz.Faculties.Where(x => x.SchoolID == 1 && x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                    //ViewBag.DepartmentList = nqz.Departments.Where(x => x.IsDeleted == false).Select(x => new { x.DepartmentID, x.DepartmentName }).ToList();

                    //Session["ClassID"] = model.FacultyID;

                    if (model.CourseID <= 0)
                    {
                        Cours dept = nqz.Courses.SingleOrDefault(x => x.FacultyID == model.FacultyID && x.IsDeleted == false && x.CourseName == model.CourseName);
                        if (dept == null)
                        {
                            if (model.CourseName != null)
                            {
                                if (model.FacultyID == null)
                                {
                                    ViewBag.Err = "Error! Select a Faculty ID";
                                }
                                else if (model.CourseCode == null)
                                {
                                    ViewBag.Err = "Error! Enter A Course Code";
                                }
                                else if (model.CourseName == null)
                                {
                                    ViewBag.Err = "Error! Enter a Course Name";
                                }
                                else
                                {
                                    Cours ncrs = new Cours()
                                    {
                                        FacultyID = model.FacultyID,
                                        CourseCode = model.CourseCode,
                                        CourseName = model.CourseName,
                                        CreditUnits = model.CreditUnits,
                                        IsDeleted = false,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now
                                    };
                                    nqz.Courses.Add(ncrs);
                                    await nqz.SaveChangesAsync();

                                    ViewBag.Msg = "Record Saved Successfully";

                                    AllCourses(int.Parse(model.FacultyID.ToString()));
                                    //ModelState.Clear();
                                }
                            }
                            else
                            {
                                AllCourses(int.Parse(model.FacultyID.ToString()));
                            }

                        }
                        else
                        {
                            ViewBag.Err = "This Subject Already Exists for the selected Class, therefore, Save transaction failed";
                        }
                    }
                    else
                    {

                        if (ModelState.IsValid)
                        {

                            Cours dept = nqz.Courses.SingleOrDefault(x => x.FacultyID == model.FacultyID && x.IsDeleted == false && x.CourseName == model.CourseName);
                            if (dept == null)
                            {

                                Cours ncrs = nqz.Courses.SingleOrDefault(x => x.IsDeleted == false && x.CourseID == model.CourseID);
                                if (ncrs != null)
                                {
                                    ncrs.FacultyID = model.FacultyID;
                                    ncrs.CourseName = model.CourseName;
                                    ncrs.CourseCode = model.CourseCode;
                                    ncrs.CreditUnits = model.CreditUnits;
                                    ncrs.Updated = DateTime.Now;

                                    await nqz.SaveChangesAsync();
                                }
                                ViewBag.Msg = "Update Completed Successfully";

                                AllCourses(model.FacultyID ?? 0);
                                //AllCourses(int.Parse(Session["ClassID"].ToString()));
                                //ModelState.Clear();
                            }
                            else
                            {
                                ViewBag.Err = "This Subject Already Exists for the selected Class, therefore, Save transaction failed";
                            }
                        }

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult NewCourse(Cours model)
        {
            ViewBag.Msg = null;
            ViewBag.Err = null;

            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    ViewBag.FacultyList = nqz.Faculties.Where(x => x.SchoolID == 1 && x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();
                }

                //ViewBag.DepartmentList = nqz.Departments.Where(x => x.IsDeleted == false).Select(x => new { x.DepartmentID, x.DepartmentName }).ToList();

                ModelState.Clear();
                //AllCourses(0);
                return View("Courses", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }


        }

        private ActionResult AllCourses(int crsid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (crsid > 0)
                {
                    var crslist = (from fac in nqz.Faculties
                                   join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                   where crs.IsDeleted == false && crs.FacultyID == crsid
                                   orderby fac.FacultyID
                                   select new CourseViewModels
                                   {
                                       CourseID = crs.CourseID,
                                       FacultyID = fac.FacultyID,
                                       FacultyName = fac.FacultyName,
                                       CourseCode = crs.CourseCode,
                                       CourseName = crs.CourseName,
                                   }).ToList();

                    ViewBag.CourseList = crslist;
                }
                else
                {
                    var crslist = (from fac in nqz.Faculties
                                   join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                   where crs.IsDeleted == false
                                   orderby fac.FacultyID
                                   select new CourseViewModels
                                   {
                                       CourseID = crs.CourseID,
                                       FacultyID = fac.FacultyID,
                                       FacultyName = fac.FacultyName,
                                       CourseCode = crs.CourseCode,
                                       CourseName = crs.CourseName,
                                   }).ToList();

                    ViewBag.CourseList = crslist;
                }

                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult EditCourse(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            Cours model = new Cours();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                ViewBag.FacultyList = nqz.Faculties.Where(x => x.SchoolID == 1 && x.IsDeleted == false).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                //ViewBag.DepartmentList = nqz.Departments.Where(x => x.IsDeleted == false).Select(x => new { x.DepartmentID, x.DepartmentName }).ToList();


                Cours ncrs = nqz.Courses.SingleOrDefault(x => x.IsDeleted == false && x.CourseID == id);

                model.CourseID = id;
                model.FacultyID = ncrs.FacultyID;
                model.CourseName = ncrs.CourseName;
                model.CourseCode = ncrs.CourseCode;
                model.CreditUnits = ncrs.CreditUnits;

                AllCourses(model.FacultyID ?? 0);
                return View("Courses", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Cours ncrs = nqz.Courses.SingleOrDefault(x => x.CourseID == id);
                if (ncrs != null)
                {
                    sklid = ncrs.FacultyID.Value;
                    ncrs.IsDeleted = true;
                    ncrs.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }


                ViewBag.Msg = "Delete Completed Successfully";

                AllCourses(int.Parse(Session["ClassID"].ToString()));
                return RedirectToAction("Courses", "Quiz");
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        //-------------------------------------------------Topics Region-------------------------------------------------------
        [HttpGet]
        public ActionResult Topics()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            ViewBag.Err = null;
            ViewBag.Msg = null;
            Session["ResourceType"] = "Quiz";

            try
            {
                if (Session["ExcelDoc"].ToString() != "Verify")
                {
                    TempData["ExcelERR"] = null;
                    TempData["ExcelMSG"] = null;
                }

                Session["TopicID"] = 0;
                ViewBag.CourseList = (from fac in nqz.Faculties
                                      join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                      where fac.ResourceTypeID == 1 && crs.IsDeleted == false
                                      orderby crs.CourseID
                                      select new
                                      {
                                          CourseID = crs.CourseID,
                                          CourseName = fac.FacultyName + " " + crs.CourseName
                                      });
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();
                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                else
                {
                    Session["CourseList"] = ViewBag.CourseList;
                }


                AllTopics(0);

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Topics(Topic model)
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;
            TempData["ExcelERR"] = null;
            TempData["ExcelMSG"] = null;
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();



                if (model.CourseID <= 0)
                {
                    model.CourseID = int.Parse(Session["CourseID"].ToString());
                }
                else
                {
                    Session["CourseID"] = model.CourseID;
                }

                if (model.TopicID <= 0)
                {
                    Topic tpc = nqz.Topics.FirstOrDefault(x => x.TopicName == model.TopicName && x.IsDeleted == false && x.CourseID == model.CourseID);
                    if (tpc == null)
                    {
                        if (model.TopicName != null)
                        {
                            if (model.CourseID == null)
                            {
                                TempData["ExcelERR"] = "Error! Select a Subject";
                            }
                            else if (model.TopicName == null)
                            {
                                TempData["ExcelERR"] = "Error! Enter a Topic Name";
                            }
                            else
                            {
                                Topic ncrs = new Topic()
                                {
                                    CourseID = model.CourseID,
                                    TopicName = model.TopicName,
                                    Description = model.Description,
                                    IsDeleted = false,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now
                                };
                                nqz.Topics.Add(ncrs);
                                await nqz.SaveChangesAsync();

                                Session["TopicID"] = ncrs.TopicID;
                                //Session["ResTypeID"] = 1;
                                TempData["ExcelMSG"] = "Record Saved Successfully";

                                AllTopics(int.Parse(model.CourseID.ToString()));
                                //ModelState.Clear();
                            }
                        }
                        else
                        {
                            AllTopics(int.Parse(model.CourseID.ToString()));
                        }
                    }
                    else
                    {
                        TempData["ExcelERR"] = "This Topic Already Exists for the selected Subject, therefore, Save transaction aborted";
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Topic tpc = nqz.Topics.FirstOrDefault(x => x.TopicName == model.TopicName && x.IsDeleted == false && x.CourseID == model.CourseID);
                        if (tpc == null)
                        {
                            Topic ncrs = nqz.Topics.SingleOrDefault(x => x.TopicID == model.TopicID);

                            //ncrs.TopicID = model.TopicID;
                            ncrs.CourseID = model.CourseID;
                            ncrs.TopicName = model.TopicName;
                            ncrs.Description = model.Description;
                            ncrs.Updated = DateTime.Now;

                            await nqz.SaveChangesAsync();

                            Session["TopicID"] = model.TopicID;

                            TempData["ExcelMSG"] = "Update Completed Successfully";
                        }
                        else
                        {
                            //TempData["ExcelERR"] = "This Topic Already Exists for the selected Subject, therefore, Save transaction aborted";
                            Topic ncrs = nqz.Topics.SingleOrDefault(x => x.TopicID == model.TopicID);

                            ncrs.TopicID = model.TopicID;
                            //ncrs.CourseID = model.CourseID;
                            ncrs.TopicName = model.TopicName;
                            ncrs.Description = model.Description;
                            ncrs.Updated = DateTime.Now;

                            await nqz.SaveChangesAsync();

                            Session["TopicID"] = model.TopicID;

                            TempData["ExcelMSG"] = "Update Completed Successfully";
                        }
                        AllTopics(int.Parse(model.CourseID.ToString()));

                        //ModelState.Clear();
                    }

                }
                Session["ResTypeID"] = 1;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult NewTopic(Topic model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();
                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }

                //ModelState.Clear();
                model.CourseID = int.Parse(Session["CourseID"].ToString());
                AllTopics(model.CourseID ?? 0);

                return View("Topics", model);

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        private ActionResult AllTopics(int crsid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (crsid > 0)
                {
                    var crslist = (from crs in nqz.Courses
                                   join tpc in nqz.Topics on crs.CourseID equals tpc.CourseID
                                   where tpc.IsDeleted == false && tpc.CourseID == crsid
                                   orderby crs.CourseName
                                   select new TopicViewModels
                                   {
                                       TopicID = tpc.TopicID,
                                       CourseID = crs.CourseID,
                                       TopicName = tpc.TopicName,
                                       CourseName = crs.CourseName,
                                       Description = tpc.Description
                                   }).ToList();

                    ViewBag.TopicList = crslist;
                }
                else
                {
                    var crslist = (from crs in nqz.Courses
                                   join tpc in nqz.Topics on crs.CourseID equals tpc.CourseID
                                   where tpc.IsDeleted == false
                                   orderby crs.CourseName
                                   select new TopicViewModels
                                   {

                                       TopicID = tpc.TopicID,
                                       CourseID = crs.CourseID,
                                       TopicName = tpc.TopicName,
                                       CourseName = crs.CourseName,
                                       Description = tpc.Description
                                   }).ToList();

                    ViewBag.TopicList = crslist;
                }

                Session["CourseID"] = crsid;
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult EditTopic(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            Topic model = new Topic();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                ViewBag.CourseList = Session["CourseList"]; //nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();

                Topic ncrs = nqz.Topics.SingleOrDefault(x => x.IsDeleted == false && x.TopicID == id);

                model.TopicID = ncrs.TopicID;
                model.CourseID = ncrs.CourseID;
                model.TopicName = ncrs.TopicName;
                model.Description = ncrs.Description;

                //ViewBag.Msg = "Update Completed Successfully";
                Session["TopicID"] = id.ToString();
                AllTopics(ncrs.CourseID ?? 0);
                return View("Topics", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DeleteTopic(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Topic ncrs = nqz.Topics.SingleOrDefault(x => x.TopicID == id);
                if (ncrs != null)
                {
                    sklid = ncrs.CourseID.Value;
                    ncrs.IsDeleted = true;
                    ncrs.Updated = DateTime.Now;
                    nqz.SaveChanges();
                }

                TempData["ExcelERR"] = "Delete Completed Successfully";
                AllTopics(ncrs.CourseID ?? 0);
                return RedirectToAction("Topics", "Quiz");
            }
            catch (Exception ex)
            {

                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        //-------------------------------------------------PastQuestions Region-------------------------------------------------------
        [HttpGet]
        public ActionResult PastQuestions()
        {

            ViewBag.Err = null;
            ViewBag.Msg = null;
            TempData["ExcelERR"] = null;
            TempData["ExcelMSG"] = null;
            Session["ResourceType"] = "PastQuestion";

            try
            {
                //var pid = long.Parse(Session["PID"].ToString());

                //if (AccountController.CheckResourceSubscription(pid, 2) == false)
                //{
                //    ViewBag.Err = "Error!!! You do not have an active subscription for this resource. ";
                //}

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //ViewBag.linktoYearId = GetYears(Year);
                    ViewBag.YearList = nqz.Years.Where(x => x.IsVisible == true).Select(x => new { x.YearID, x.YearName }).ToList();

                    Session["PastQuestionID"] = 0;
                    ViewBag.CourseList = (from fac in nqz.Faculties
                                          join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                          where fac.ResourceTypeID == 2 && crs.IsDeleted == false
                                          orderby fac.FacultyName
                                          select new
                                          {
                                              crs.CourseID,
                                              CourseName = fac.FacultyName + " " + crs.CourseName
                                          });
                }
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();
                Session["CourseList"] = ViewBag.CourseList;
                Session["YearList"] = ViewBag.YearList;
                //DropDown : GetYears() will fill Year DropDown and Return List.  


                AllPastQuestions(0);

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> PastQuestions(PastQuestion model)
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;
            TempData["ExcelERR"] = null;
            TempData["ExcelMSG"] = null;
            //DBQUIZZEntities nqz = new DBQUIZZEntities();

            try
            {
                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                if (ViewBag.YearList == null)
                {
                    ViewBag.YearList = Session["YearList"];
                }
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();

                Session["CourseID"] = model.CourseID;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (model.PastQuestionID <= 0)
                    {
                        if (model.CourseID > 0 && model.YearID == null)
                        {
                            //AllQuestions(model.TopicID.Value);
                            Session["PastQuestionID"] = model.PastQuestionID;
                        }
                        else
                        {
                            PastQuestion tpc = nqz.PastQuestions.FirstOrDefault(x => x.YearID == model.YearID && x.IsDeleted == false && x.CourseID == model.CourseID);
                            if (tpc == null)
                            {
                                if (model.YearID != 0)
                                {
                                    if (model.CourseID == null)
                                    {
                                        TempData["ExcelERR"] = "Error! You Must Select a Subject";
                                    }
                                    else if (model.YearID == null)
                                    {
                                        TempData["ExcelERR"] = "Error! You Must Select The Year ";
                                    }
                                    else if (model.Description == null)
                                    {
                                        TempData["ExcelERR"] = "Error! You Must Enter A Description ";
                                    }
                                    else
                                    {
                                        PastQuestion ncrs = new PastQuestion()
                                        {
                                            CourseID = model.CourseID,
                                            YearID = model.YearID,
                                            linktoYearId = model.YearID,
                                            Description = model.Description,
                                            IsDeleted = false,
                                            Created = DateTime.Now,
                                            Updated = DateTime.Now
                                        };
                                        nqz.PastQuestions.Add(ncrs);
                                        await nqz.SaveChangesAsync();

                                        Session["TopicID"] = ncrs.PastQuestionID;
                                        Session["ResTypeID"] = 2;
                                        TempData["ExcelERR"] = "Record Saved Successfully";

                                        //AllPastQuestions(int.Parse(model.CourseID.ToString()));
                                        //ModelState.Clear();
                                    }
                                }
                                else
                                {
                                    //AllPastQuestions(int.Parse(model.CourseID.ToString()));
                                }
                            }
                            else
                            {
                                TempData["ExcelERR"] = "This PastQuestion Already Exists for the selected Subject, therefore, Save transaction aborted";
                            }
                        }
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            PastQuestion tpc = nqz.PastQuestions.FirstOrDefault(x => x.YearID == model.YearID && x.IsDeleted == false && x.CourseID == model.CourseID);
                            if (tpc == null)
                            {
                                PastQuestion ncrs = nqz.PastQuestions.SingleOrDefault(x => x.PastQuestionID == model.PastQuestionID);

                                //ncrs.TopicID = model.TopicID;
                                ncrs.CourseID = model.CourseID;
                                ncrs.YearID = model.YearID;
                                ncrs.Description = model.Description;
                                ncrs.Updated = DateTime.Now;

                                await nqz.SaveChangesAsync();

                                Session["PastQuestionID"] = model.PastQuestionID;
                                Session["TopicID"] = model.PastQuestionID;
                                Session["ResTypeID"] = 2;
                                TempData["ExcelMSG"] = "Update Completed Successfully";
                            }
                            else
                            {
                                //TempData["ExcelERR"] = "This PastQuestion Already Exists for the selected Subject, therefore, Save transaction aborted";
                            }


                            //ModelState.Clear();
                        }

                    }
                    Session["QYear"] = nqz.Years.FirstOrDefault(x => x.YearID == model.YearID).YearName.Value;
                }
                AllPastQuestions(int.Parse(model.CourseID.ToString()));
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult NewPastQuestion(PastQuestion model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();
                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                if (ViewBag.YearList == null)
                {
                    ViewBag.YearList = Session["YearList"];
                }

                ModelState.Clear();
                AllPastQuestions(0);

                return View("PastQuestions", model);

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        private ActionResult AllPastQuestions(int crsid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (crsid > 0)
                {

                    //var crslist = (from crs in nqz.Courses
                    //               join tpc in nqz.PastQuestions on crs.CourseID equals tpc.CourseID
                    //               where tpc.IsDeleted == false && tpc.CourseID == crsid
                    //               orderby crs.CourseName
                    var crslist = (from fac in nqz.Faculties
                                   join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                   join tpc in nqz.PastQuestions on crs.CourseID equals tpc.CourseID
                                   join yr in nqz.Years on tpc.YearID equals yr.YearID
                                   where fac.ResourceTypeID == 2 && tpc.IsDeleted == false && tpc.CourseID == crsid
                                   orderby crs.CourseName
                                   select new PastQuestionViewModels
                                   {
                                       PastQuestionID = tpc.PastQuestionID,
                                       FacultyName = fac.FacultyName,
                                       CourseID = crs.CourseID,
                                       YearName = yr.YearName ?? 0,
                                       CourseName = crs.CourseName,
                                       Description = tpc.Description
                                   }).ToList();

                    ViewBag.PastQuestionList = crslist;
                }
                else
                {
                    var crslist = (from fac in nqz.Faculties
                                   join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                   join tpc in nqz.PastQuestions on crs.CourseID equals tpc.CourseID
                                   join yr in nqz.Years on tpc.YearID equals yr.YearID
                                   where fac.ResourceTypeID == 2 && tpc.IsDeleted == false
                                   orderby crs.CourseName
                                   select new PastQuestionViewModels
                                   {

                                       PastQuestionID = tpc.PastQuestionID,
                                       FacultyName = fac.FacultyName,
                                       CourseID = crs.CourseID,
                                       YearName = yr.YearName ?? 0,
                                       CourseName = crs.CourseName,
                                       Description = tpc.Description
                                   }).ToList();

                    ViewBag.PastQuestionList = crslist;
                }
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult EditPastQuestion(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            PastQuestion model = new PastQuestion();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                if (ViewBag.YearList == null)
                {
                    ViewBag.YearList = Session["YearList"];
                }
                //ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();

                PastQuestion ncrs = nqz.PastQuestions.SingleOrDefault(x => x.IsDeleted == false && x.PastQuestionID == id);
                model.PastQuestionID = ncrs.PastQuestionID;
                model.CourseID = ncrs.CourseID;
                model.YearID = ncrs.YearID;
                model.Description = ncrs.Description;

                //ViewBag.Msg = "Update Completed Successfully";
                Session["PastQuestionID"] = id.ToString();
                Session["TopicID"] = id.ToString();
                Session["ResTypeID"] = 2;
                Session["QYear"] = nqz.Years.FirstOrDefault(x => x.YearID == model.YearID).YearName.Value;
                AllPastQuestions(ncrs.CourseID ?? 0);
                return View("PastQuestions", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DeletePastQuestion(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (ViewBag.CourseList == null)
                {
                    ViewBag.CourseList = Session["CourseList"];
                }
                if (ViewBag.YearList == null)
                {
                    ViewBag.YearList = Session["YearList"];
                }
                PastQuestion ncrs = nqz.PastQuestions.SingleOrDefault(x => x.PastQuestionID == id);
                if (ncrs != null)
                {
                    sklid = ncrs.CourseID.Value;
                    ncrs.IsDeleted = true;
                    ncrs.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }

                TempData["ExcelERR"] = "Delete Completed Successfully";
                AllTopics(ncrs.CourseID ?? 0);
                return RedirectToAction("PastQuestions", "Quiz");
            }
            catch (Exception ex)
            {

                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }

        }
        //-------------------------------------------------YEAR Region-------------------------------------------------------
        private SelectList GetYears(int? iSelectedYear)
        {
            int CurrentYear = DateTime.Now.Year;

            for (int i = 2010; i <= CurrentYear; i++)
            {
                ddlYears.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }

            //Default It will Select Current Year  
            return new SelectList(ddlYears, "Value", "Text", iSelectedYear);

        }

        //-------------------------------------------------Questions Region-------------------------------------------------------
        [HttpGet]
        public ActionResult Questions(int id)
        {
            try
            {
                Session["EditQuestionModel"] = null;
                int TopID = 0;
                //Session["QueAns"] = null;

                ViewBag.QueID = 0;
                Session["img"] = "0";
                if (blansflag == true)
                {
                    TopID = int.Parse(Session["TopID"].ToString());
                    AllQuestions(TopID, id);
                    if (ViewBag.AnswerList == null)
                    {
                        ViewBag.AnswerList = Session["QueAns"];
                    }
                    AllQuestions(TopID, id);
                }
                //Question model = new Question(); 
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (id == 1)
                    {
                        ViewBag.TopicList = (from fac in nqz.Faculties
                                             join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                             join tpc in nqz.Topics on crs.CourseID equals tpc.CourseID
                                             where crs.IsDeleted == false
                                             orderby fac.FacultyID
                                             select new
                                             {
                                                 tpc.TopicID,
                                                 TopicName = fac.FacultyName + " " + crs.CourseName + " " + tpc.TopicName
                                             }).ToList();
                    }
                    else
                    {
                        ViewBag.TopicList = (from fac in nqz.Faculties
                                             join crs in nqz.Courses on fac.FacultyID equals crs.FacultyID
                                             join psq in nqz.PastQuestions on crs.CourseID equals psq.CourseID
                                             join yrs in nqz.Years on psq.YearID equals yrs.YearID
                                             where crs.IsDeleted == false
                                             orderby fac.FacultyID
                                             select new
                                             {
                                                 TopicID = psq.PastQuestionID,
                                                 TopicName = fac.FacultyName + " " + crs.CourseName + " " + yrs.YearName
                                             }).ToList();
                    }
                    Session["TopicList"] = ViewBag.TopicList;
                }

                if (TempData.ContainsKey("TopicID"))
                {
                    Session["TopID"] = int.Parse(TempData["TopicID"].ToString());
                    TopID = int.Parse(TempData["TopicID"].ToString());
                    AllQuestions(TopID, id);
                }
                else
                {

                    //AllAnswers(TopID);
                }

                //model.ResourceTypeID = id;
                Session["ResTypeID"] = id;
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Questions(QuestionVModels model)
        {
            int QYr = 0;
            int ResTypeid = int.Parse(Session["ResTypeID"].ToString());
            var pid = long.Parse(Session["PID"].ToString());
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (ResTypeid == 2)
                {
                    var QYr1 = (from psq in nqz.PastQuestions join qyr in nqz.Years on psq.YearID equals qyr.YearID where psq.PastQuestionID == model.TopicID select new { Year = qyr.YearName }).FirstOrDefault();
                    QYr = QYr1.Year ?? 0;
                }

                ViewBag.AddAnswers = null;
                if (ViewBag.TopicList == null)
                {
                    ViewBag.TopicList = Session["TopicList"];
                }
                //nqz.Topics.Where(x => x.IsDeleted == false).Select(x => new { x.TopicID, x.TopicName }).ToList();

                if (model.QuestionID <= 0)
                {
                    if (model.TopicID > 0 && model.QuestionString == null)
                    {
                        AllQuestions(int.Parse(model.TopicID.ToString()), ResTypeid);
                        Session["TopID"] = model.TopicID;
                    }
                    else
                    {
                        Question que = nqz.Questions.SingleOrDefault(x => x.TopicID == model.TopicID && x.IsDeleted == false && x.QuestionString.ToLower() == model.QuestionString.ToLower());
                        if (que == null)
                        {

                            if (int.Parse(model.TopicID.ToString()) == 0)
                            {
                                ViewBag.Err = "Error! Select a PastQuestion ID";
                            }
                            //else if (model.Number == null)
                            //{
                            //    ViewBag.Err = "Error! Enter A Number";
                            //}
                            else if (model.QuestionString == null)
                            {
                                ViewBag.Err = "Error! Enter a Question String";
                            }
                            //else if (int.Parse(model.QuestionMarks.ToString()) == 0)
                            //{
                            //    ViewBag.Err = "Error! Enter a Question Marks";
                            //}
                            //else if (int.Parse(model.QuestionDuration.ToString()) == 0)
                            //{
                            //    ViewBag.Err = "Error! Enter a Question Duration";
                            //}
                            else if (model.AnswerExplanation == null)
                            {
                                ViewBag.Err = "Error! Enter an Answer Explanation";
                            }
                            else
                            {

                                //int? qcnt = (from sm in nqz.Questions
                                //             where sm.TopicID == model.TopicID
                                //             select sm.QuestionID).Count();

                                Question ncrs = new Question()
                                {
                                    TopicID = model.TopicID,
                                    //Number = "0",
                                    ResourceTypeID = ResTypeid,
                                    QuestionString = model.QuestionString,
                                    AnswerExplanation = model.AnswerExplanation,
                                    QuestionMarks = 5, // model.QuestionMarks,
                                    QuestionDuration = 2, // model.QuestionDuration,
                                    Instruction = model.Instruction,
                                    QuestionTypeID = 1,
                                    Year = QYr,
                                    IsMock = false,
                                    IsVerified = model.IsVerified,
                                    IsDeleted = false,
                                    InserterID = pid,
                                    UpdaterID = pid,
                                    Created = DateTime.UtcNow,
                                    Updated = DateTime.UtcNow
                                };
                                nqz.Questions.Add(ncrs);
                                await nqz.SaveChangesAsync();

                                var file = model.ImageFile;
                                if (file != null)
                                {
                                    var FileName = Path.GetFileNameWithoutExtension(file.FileName);
                                    var ext = Path.GetExtension(file.FileName);
                                    FileName = "/Content/QuePix/QUE_" + model.TopicID + "_" + ncrs.QuestionID + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                                    model.ImagePath = FileName;
                                    file.SaveAs(Server.MapPath(FileName));

                                    Question question = nqz.Questions.FirstOrDefault(x => x.QuestionID == ncrs.QuestionID);
                                    question.ImagePath = FileName;
                                    await nqz.SaveChangesAsync();
                                }
                                else
                                {
                                    model.ImagePath = null;
                                }
                                //if (Session["img"].ToString() == "1")
                                //{
                                //    if (model.ImageFile.FileName != null)
                                //    {
                                //        string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                                //        string ext = Path.GetExtension(model.ImageFile.FileName);
                                //        FileName = "QUE_" + model.TopicID + "_" + ncrs.QuestionID + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                                //        model.ImagePath = "~/Content/QuePix/" + FileName;
                                //        FileName = Path.Combine("~/Content/QuePix/", FileName);
                                //        model.ImageFile.SaveAs(Server.MapPath(FileName));

                                //        model.ImagePath = FileName;
                                //        //Session["QueImagePath"] = FileName;
                                //Session["img"] = "0";
                                //Question question = nqz.Questions.FirstOrDefault(x => x.QuestionID == ncrs.QuestionID);
                                //question.ImagePath = FileName;
                                //await nqz.SaveChangesAsync();

                                //    }
                                //}

                                ViewBag.QueID = ncrs.QuestionID;

                                ViewBag.Msg = "Record Saved Successfully";

                                AllQuestions(int.Parse(model.TopicID.ToString()), ResTypeid);
                                //ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.Err = "This Question Already Exists for the selected Question, therefore, Save transaction aborted";
                            AllQuestions(int.Parse(model.TopicID.ToString()), ResTypeid);
                            //ModelState.Clear();
                        }

                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Question que = nqz.Questions.SingleOrDefault(x => x.TopicID == model.TopicID && x.IsDeleted == false && x.QuestionString == model.QuestionString);
                        if (que != null)
                        {
                            if (Session["img"].ToString() == "1")
                            {
                                if (model.ImageFile.FileName != null)
                                {
                                    string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                                    string ext = Path.GetExtension(model.ImageFile.FileName);
                                    FileName = "QUE_" + model.TopicID + "_" + model.QuestionID + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                                    model.ImagePath = "~/Content/QuePix/" + FileName;
                                    FileName = Path.Combine("~/Content/QuePix/", FileName);
                                    model.ImageFile.SaveAs(Server.MapPath(FileName));

                                    model.ImagePath = FileName;
                                    //Session["QueImagePath"] = FileName;
                                    Session["img"] = "1";
                                }
                            }
                            Question ncrs = nqz.Questions.SingleOrDefault(x => x.QuestionID == model.QuestionID);

                            //ncrs.QuestionID = model.QuestionID;
                            //ncrs.TopicID = model.TopicID;
                            ncrs.Number = model.Number;
                            ncrs.ResourceTypeID = int.Parse(Session["ResTypeID"].ToString());
                            ncrs.Year = QYr;
                            ncrs.QuestionString = model.QuestionString;
                            ncrs.AnswerExplanation = model.AnswerExplanation;
                            ncrs.QuestionMarks = 5; //model.QuestionMarks;
                            ncrs.QuestionDuration = 2;// model.QuestionDuration;
                            ncrs.Instruction = model.Instruction;
                            ncrs.ImagePath = model.ImagePath;
                            ncrs.IsMock = false;
                            ncrs.UpdaterID = pid;
                            ncrs.IsVerified = model.IsVerified;
                            ncrs.Updated = DateTime.UtcNow;

                            await nqz.SaveChangesAsync();

                            ViewBag.QueID = ncrs.QuestionID;

                            ViewBag.Msg = "Update Completed Successfully";

                            AllQuestions(int.Parse(model.TopicID.ToString()), ResTypeid);
                            //ModelState.Clear();
                        }
                        else
                        {
                            Question ncrs = nqz.Questions.SingleOrDefault(x => x.QuestionID == model.QuestionID);

                            //ncrs.QuestionID = model.QuestionID;
                            //ncrs.TopicID = model.TopicID;
                            ncrs.Number = model.Number;
                            ncrs.ResourceTypeID = int.Parse(Session["ResTypeID"].ToString());
                            ncrs.Year = QYr;
                            ncrs.QuestionString = model.QuestionString;
                            ncrs.AnswerExplanation = model.AnswerExplanation;
                            ncrs.QuestionMarks = 5; //model.QuestionMarks;
                            ncrs.QuestionDuration = 2;// model.QuestionDuration;
                            ncrs.Instruction = model.Instruction;
                            ncrs.ImagePath = model.ImagePath;
                            ncrs.IsMock = false;
                            ncrs.UpdaterID = pid;
                            ncrs.IsVerified = model.IsVerified;
                            ncrs.Updated = DateTime.UtcNow;

                            await nqz.SaveChangesAsync();

                            ViewBag.QueID = ncrs.QuestionID;

                            ViewBag.Msg = "Update Completed Successfully";
                            //ViewBag.Err = "This Question Already Exists for the selected Topic, therefore, Save transaction aborted";
                            AllQuestions(int.Parse(model.TopicID.ToString()), ResTypeid);
                        }
                    }
                    else
                    {
                        ViewBag.ErrMessage = "Invalid Data";
                        return View("Error");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");

            }

        }

        [HttpGet]
        public ActionResult InsertImage(QuestionVModels model)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var question = nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID);
                    return File(question.ImagePath, "");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult RetrieveImage(int imgid)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var question = nqz.Questions.FirstOrDefault(x => x.QuestionID == imgid);
                    return File(question.ImagePath, "");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult NewQuestion(QuestionVModels model)
        {
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Err = null;
                ViewBag.Msg = null;

                if (ViewBag.TopicList == null)
                {
                    ViewBag.TopicList = Session["TopicList"];
                }
                Session["img"] = "0";
                model.TopicID = int.Parse(Session["TopID"].ToString());
                model.ResourceTypeID = int.Parse(Session["ResTypeID"].ToString());
                //ModelState.Clear();
                AllQuestions(model.ResourceTypeID, model.TopicID);

                return View("Questions", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public async Task<JsonResult> UploadQuestionPix(QuestionVModels model)
        {
            try
            {
                //QuestionVModels model = Session["EditQuestionModel"] as QuestionVModels;
                if (model.QuestionID == 0)
                {
                    model.QuestionID = int.Parse(Session["QueID"].ToString());
                }
                if (model.TopicID == 0)
                {
                    model.TopicID = int.Parse(Session["EditTopicID"].ToString());
                }
                var file = model.Image_File;
                if (file != null)
                {
                    var FileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var ext = Path.GetExtension(file.FileName);
                    FileName = "/Content/QuePix/QUE_" + model.TopicID + "_" + model.QuestionID + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                    model.ImagePath = FileName;
                    string strfile = Server.MapPath(FileName);
                    file.SaveAs(strfile);
                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        Question question = nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID);
                        question.ImagePath = FileName;
                        await nqz.SaveChangesAsync();
                    }

                    //return Json(new { su})
                    return Json(new { success = true, responseText = "Image Upload was Successful!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.ImagePath = null;
                    return Json(new { success = false, responseText = "No Image was selected!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                //return View("Error");
                //ViewBag.Err = "Error! There has been an entry error. Please, try again.";
                return Json(new { success = false, responseText = "Error! There has been an upload error. Please, try again." }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Questions");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadQuestionPix1(HttpPostedFileBase postedFile)
        {
            try
            {

                //if (!string.IsNullOrEmpty(postedFile.ToString()))
                ////{
                if (postedFile.FileName != null && postedFile.ContentLength > 0)
                {
                    //if (Request.Files.Count > 0)
                    //{
                    QuestionVModels model = Session["EditQuestionModel"] as QuestionVModels;
                    string FileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    if (FileName.Contains(" "))
                    {
                        FileName = FileName.Replace(" ", "");
                    }
                    string ext = Path.GetExtension(postedFile.FileName);
                    FileName = FileName + "_" + model.TopicID + "_" + model.QuestionID + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                    model.ImagePath = "~/Content/QuePix/" + FileName;
                    FileName = Path.Combine("~/Content/QuePix/", FileName);
                    //model.ImageFile.SaveAs(Server.MapPath(FileName));
                    postedFile.SaveAs(Server.MapPath(FileName));

                    model.ImagePath = FileName;
                    Session["ImagePath"] = FileName;

                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        Question question = nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID);
                        if (question != null)
                        {
                            question.ImagePath = model.ImagePath;
                            await nqz.SaveChangesAsync();
                        }

                        //return View(model);
                    }
                }

                //}
                return RedirectToAction("Questions");
            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                //return View("Error");
                ViewBag.Err = "Error! There has been an entry error. Please, try again.";
                return RedirectToAction("Questions");
            }
        }

        //[HttpPost]
        //public async Task<ActionResult> ImportFile(HttpPostedFileBase importFile)
        //{
        //    if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

        //    try
        //    {
        //        var fileData = GetDataFromCSVFile(importFile.InputStream);

        //        var dtEmployee = fileData.ToDataTable();
        //        var tblEmployeeParameter = new SqlParameter("tblEmployeeTableType", SqlDbType.Structured)
        //        {
        //            TypeName = "dbo.tblTypeEmployee",
        //            Value = dtEmployee
        //        };
        //        await _dbContext.Database.ExecuteSqlCommandAsync("EXEC spBulkImportEmployee @tblEmployeeTableType", tblEmployeeParameter);
        //        return Json(new { Status = 1, Message = "File Imported Successfully " });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Status = 0, Message = ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> ImportFromExcel(HttpPostedFileBase postedFile)
        {

            string strTopic = string.Empty;
            bool blsave = false;

            try
            {
                int QYr = DateTime.Now.Year;

                TempData["ExcelERR"] = null;
                TempData["ExcelMSG"] = null;

                int topicid = 0; //int.Parse(Session["TopicID"].ToString());
                //if (topicid <= 0)
                //{
                //    TempData["ExcelERR"] = "ERROR! You must select a Topic to process.";
                //    if (Session["ResourceType"].ToString() == "Quiz")
                //    {
                //        return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                //    }
                //    else if (Session["ResourceType"].ToString() == "PastQuestion")
                //    {
                //        return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                //    }
                //}

                if (ModelState.IsValid)
                {
                    //if (Session["CourseID"].ToString() == null)
                    //{
                    //    TempData["ExcelERR"] = "ERROR! You must select a topic!";
                    //}
                    //else
                    //{

                    //}
                    if (postedFile == null)
                    {
                        TempData["ExcelERR"] = "ERROR! No File Was Selected!";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                        }
                        //return Json(new { Status = 0, Message = "No File Selected" });
                    }

                    if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                    {
                        //ModelState.AddModelError("postedFile", "Your file is too large. Maximum size allowed is 50MB !");
                        TempData["ExcelERR"] = "ERROR! Your file is too large. Maximum size allowed is 50MB!";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                        }
                        //return Json(new { Status = 0, Message = "Your file is too large. Maximum size allowed is 50MB !" });
                    }
                    else
                    {
                        string filePath = string.Empty;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + Path.GetFileName(postedFile.FileName);
                        //if (verifiedfilepath != filePath)
                        //{
                        //    TempData["ExcelERR"] = "ERROR! The Verification Status of the selected file could not be determined. Please Verify the file and try again.";
                        //    if (Session["ResourceType"].ToString() == "Quiz")
                        //    {
                        //        return RedirectToAction("Topics", "Quiz");
                        //    }
                        //    else if (Session["ResourceTypes"].ToString() == "PastQuestion")
                        //    {
                        //        return RedirectToAction("PastQuestion", "Quiz");
                        //    }

                        //}
                        string extension = Path.GetExtension(postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        string conString = string.Empty;
                        switch (extension)
                        {
                            case ".xls": //For Excel 97-03.  
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx": //For Excel 07 and above.  
                                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                break;
                        }

                        try
                        {
                            System.Data.DataTable dt = new System.Data.DataTable();
                            dt = READExcel(filePath);
                            //conString = string.Format(conString, filePath);

                            //using (OleDbConnection connExcel = new OleDbConnection(conString))
                            //{
                            //    using (OleDbCommand cmdExcel = new OleDbCommand())
                            //    {
                            //        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            //        {
                            //            cmdExcel.Connection = connExcel;

                            //            //Get the name of First Sheet.  
                            //            connExcel.Open();
                            //            System.Data.DataTable dtExcelSchema;
                            //            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            //            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            //            connExcel.Close();

                            //            //Read Data from First Sheet.  
                            //            connExcel.Open();
                            //            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            //            odaExcel.SelectCommand = cmdExcel;
                            //            odaExcel.Fill(dt);
                            //            connExcel.Close();
                            //        }
                            //    }
                            //}

                            int cnt = 0, QueID = 0;
                            int OptionNumber = 0;
                            bool correctans = false;
                            if (Session["ResourceType"].ToString() == "PastQuestion")
                            {
                                QYr = int.Parse(Session["QYear"].ToString());
                            }

                            //QueID = 9308;
                            //QueID = 9386;
                            //QueID = 9646;
                            //QueID = 9269;
                            //string optletter = null;
                            foreach (DataRow objDataRow in dt.Rows)
                            {
                                if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                                cnt++;
                                var questr = objDataRow["QUESTION TEXT"].ToString().Trim();
                                //if (questr.Contains("\'"))
                                //{
                                //    questr = questr.Replace("\'", "\"");
                                //}

                                var ansexplanation = objDataRow["ANSWER EXPLANATION"].ToString().Trim();

                                var pixpath = objDataRow["IMAGE NAME"].ToString().Trim();
                                if (pixpath.ToUpper() != "NIL")
                                {
                                    pixpath = "/Content/QuePix/" + pixpath;
                                }
                                string figanswer = String.Empty;
                                var figstlot = Convert.ToInt32(objDataRow["FIG SLOTS"].ToString().Trim());
                                if (figstlot > 0)
                                {
                                    figanswer = objDataRow["CORRECT OPTION"].ToString().Trim();
                                }

                                bool isfree = false;
                                var restypeid = Convert.ToInt32(objDataRow["RESOURCE TYPE"].ToString().Trim());
                                if (restypeid == 4)
                                {
                                    isfree = true;
                                }

                                strTopic = objDataRow["TOPIC"].ToString().Trim();
                                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                                {
                                    //Check if Category/Topic exists in the system and insert questions and answers
                                    var Crsid = int.Parse(Session["CourseID"].ToString());
                                    Topic topic = nqz.Topics.FirstOrDefault(x => x.CourseID == Crsid && x.TopicName.ToLower() == strTopic.ToLower() && x.IsDeleted == false);
                                    if (topic != null)
                                    {
                                        topicid = topic.TopicID;
                                    }
                                    else
                                    {
                                        Topic ntop = new Topic()
                                        {
                                            CourseID = int.Parse(Session["CourseID"].ToString()),
                                            TopicName = strTopic,
                                            Description = strTopic,
                                            IsDeleted = false,
                                            Created = DateTime.Now,
                                            Updated = DateTime.Now
                                        };
                                        nqz.Topics.Add(ntop);
                                        await nqz.SaveChangesAsync();

                                        topicid = ntop.TopicID;

                                    }

                                    //Save Question Details
                                    var nque1 = nqz.Questions.FirstOrDefault(x => x.TopicID == topicid && x.QuestionString.Trim() == questr.Trim() && x.AnswerExplanation.Trim() == ansexplanation.Trim());
                                    //var nque1 = nqz.Questions.FirstOrDefault(x => x.QuestionID == QueID);
                                    if (nque1 == null)
                                    {
                                        Question nque = new Question()
                                        {
                                            TopicID = topicid,
                                            Number = "(" + cnt.ToString() + ")",
                                            QuestionNumber = cnt,
                                            QuestionString = questr,
                                            ResourceTypeID = restypeid,
                                            QuestionTypeID = Convert.ToInt32(objDataRow["QUESTION TYPE"].ToString().Trim()),
                                            AnswerExplanation = ansexplanation.Trim(),
                                            QuestionMarks = 5,
                                            QuestionDuration = 2,
                                            Instruction = objDataRow["INSTRUCTION"].ToString().Trim(),
                                            Year = QYr,
                                            FIGSlot = figstlot, //Convert.ToInt32(objDataRow["FIG SLOTS"].ToString().Trim()),
                                            Answer = figanswer,
                                            IsMock = false,
                                            IsFree = isfree,
                                            ImagePath = pixpath,
                                            IsDeleted = false,
                                            IsVerified = false,
                                            Created = DateTime.Now,
                                            Updated = DateTime.Now
                                        };
                                        nqz.Questions.Add(nque);
                                        await nqz.SaveChangesAsync();

                                        QueID = nque.QuestionID;

                                        //blsave = true;
                                    }
                                    else
                                    {
                                        QueID = nque1.QuestionID;

                                        Question questions = nqz.Questions.FirstOrDefault(x => x.QuestionID == QueID);
                                        questions.QuestionString = questr;
                                        questions.AnswerExplanation = ansexplanation.Trim();
                                        questions.QuestionTypeID = Convert.ToInt32(objDataRow["QUESTION TYPE"].ToString().Trim());
                                        questions.Instruction = objDataRow["INSTRUCTION"].ToString().Trim();
                                        questions.FIGSlot = figstlot;
                                        questions.Answer = figanswer;
                                        questions.ImagePath = pixpath;
                                        await nqz.SaveChangesAsync();
                                        blsave = false;
                                        QueID++;
                                    }

                                    //Save answer details
                                    //Save answer details

                                    //if (blsave == true)
                                    //{
                                    OptionNumber = int.Parse(objDataRow["OPTIONS NUMBER"].ToString());
                                    //}
                                    if (OptionNumber > 0)
                                    {
                                        for (int i = 1; i <= OptionNumber; i++)
                                        {

                                            if (objDataRow["CORRECT OPTION"].ToString() == ((OptionLetter)i).ToString())
                                            {
                                                correctans = true;
                                            }
                                            else
                                            {
                                                correctans = false;
                                            }

                                            var nans = nqz.Answers.FirstOrDefault(x => x.QuestionID == QueID && x.OptionLetter == ((OptionLetter)i).ToString());
                                            if (nans == null)
                                            {
                                                if (i == 1)
                                                {
                                                    ActionResult x = await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION A"].ToString(), correctans);
                                                }
                                                else if (i == 2)
                                                {
                                                    ActionResult x = await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION B"].ToString(), correctans);
                                                }
                                                else if (i == 3)
                                                {
                                                    ActionResult x = await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION C"].ToString(), correctans);
                                                }
                                                else if (i == 4)
                                                {
                                                    ActionResult x = await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION D"].ToString(), correctans);
                                                }
                                                else if (i == 5)
                                                {
                                                    ActionResult x = await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                            }
                                            else
                                            {
                                                if (i == 1)
                                                {
                                                    ActionResult y = await UpdateAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                                else if (i == 2)
                                                {
                                                    ActionResult y = await UpdateAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                                else if (i == 3)
                                                {
                                                    ActionResult y = await UpdateAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                                else if (i == 4)
                                                {
                                                    ActionResult y = await UpdateAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                                else if (i == 5)
                                                {
                                                    ActionResult y = await UpdateAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }

                                            }

                                        }
                                    }
                                }

                            }
                            dt.Dispose();

                            TempData["ExcelMSG"] = "ALERT! File uploaded successfully! "; //+ strTopic;
                            if (Session["ResourceType"].ToString() == "Quiz")
                            {
                                return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                            }
                            else if (Session["ResourceType"].ToString() == "PastQuestion")
                            {
                                return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                            }

                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                            return View("Error");

                        }
                        finally
                        {

                        }
                    }
                }

                TempData["ExcelERR"] = "ERROR! No file was selected!";

                if (Session["ResourceType"].ToString() == "Quiz")
                {
                    return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                }
                return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                //return Json(new { Status = 0, Message = "no files were selected !" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }
        }

        //public async Task<ActionResult> ExcelUploadWithFormat()
        //{
        //    try
        //    {

        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {

        //    }
        //}

        public System.Data.DataTable READExcel(string path)
        {
            System.Data.DataTable dt;

            try
            {
                string strVal = String.Empty;

                Application objXL = null;
                Workbook objWB = null;
                objXL = new Application();
                objWB = objXL.Workbooks.Open(path);
                Worksheet objSHT = objWB.Worksheets[1];

                int rows = objSHT.UsedRange.Rows.Count;
                int cols = objSHT.UsedRange.Columns.Count;
                dt = new System.Data.DataTable();
                int noofrow = 1;

                for (int c = 1; c <= cols; c++)
                {
                    string colname = objSHT.Cells[1, c].Text;
                    dt.Columns.Add(colname);
                    noofrow = 2;
                }

                for (int r = noofrow; r <= rows; r++)
                {
                    DataRow dr = dt.NewRow();
                    for (int c = 1; c <= cols; c++)
                    {
                        strVal = objSHT.Cells[r, c].Text;
                        //if (c == 2)
                        //{
                        //    strorigques = objSHT.Cells[r, c].Text;
                        //}
                        //if (c == 3)
                        //{
                        //    strorigansexp = objSHT.Cells[r, c].Text;
                        //}

                        if (c == 2 || c == 3)
                        {
                            strVal = FindFirstBold(objSHT.Cells[r, c]);
                        }

                        dr[c - 1] = strVal;
                    }

                    dt.Rows.Add(dr);
                }

                objWB.Close();
                objXL.Quit();

                return dt;
            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                //return View("Error");
                return null;
            }
            finally
            {

            }



        }

        private string FindFirstBold(Range cell)
        {
            bool blfst = false, blsec = false, blthd = false;

            string cellstr = cell.Text;
            try
            {

                for (int index = 1; index <= cell.Text.ToString().Length; index++)
                {
                    Characters ch = cell.get_Characters(index, 1);
                    if (ch.Equals(' '))
                    {
                        bool bold = (bool)ch.Font.Bold;
                        if (bold)
                        {
                            if (blfst == false)
                            {
                                blfst = true;
                                cellstr = cellstr.Insert(index - 1, "<b>");
                            }
                        }
                        else
                        {
                            if (blfst == true)
                            {
                                cellstr = cellstr.Insert(index + 2, "</b>");
                                blfst = false;
                            }
                        }

                        //bool italic = (bool)ch.Font.Italic;
                        //if (italic)
                        //{
                        //    if (blsec == false)
                        //    {
                        //        blsec = true;
                        //        if (blfst == true)
                        //        {
                        //            cellstr = cellstr.Insert(index - 2, "<i>");

                        //        }
                        //        else
                        //        {
                        //            cellstr = cellstr.Insert(index - 1, "<i>");
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (blsec == true)
                        //    {
                        //        cellstr = cellstr.Insert(index + 2, "</i>");
                        //        blsec = false;
                        //    }
                        //}

                        //int undln = (int)ch.Font.Underline;
                        //if (undln == 2)
                        //{
                        //    if (blthd == false)
                        //    {
                        //        blthd = true;
                        //        cellstr = cellstr.Insert(index - 1, "<u>");
                        //    }
                        //}
                        //else
                        //{
                        //    if (blthd == true)
                        //    {
                        //        cellstr = cellstr.Insert(index + 2, "</u>");
                        //        blthd = false;
                        //    }
                        //}

                    }

                }

                return cellstr;
            }
            catch (Exception ex)
            {
                return null;
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                //return View("Error");
            }
            finally
            {

            }

        }

        [HttpPost]
        public async Task<ActionResult> VerifyImport(HttpPostedFileBase postedFiles)
        {
            string strTopic = string.Empty;


            try
            {
                int QYr = DateTime.Now.Year;

                TempData["ExcelERR"] = null;
                TempData["ExcelMSG"] = null;

                Session["ExcelDoc"] = "Verify";

                int topicid = int.Parse(Session["TopicID"].ToString());
                int CourseID = int.Parse(Session["CourseID"].ToString());

                if (ModelState.IsValid)
                {

                    if (postedFiles == null)
                    {
                        TempData["ExcelERR"] = "ERROR! No File Was Selected!";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("Topics", "Quiz");
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("PastQuestions", "Quiz");
                        }
                        //return Json(new { Status = 0, Message = "No File Selected" });
                    }

                    if (postedFiles != null && postedFiles.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                    {
                        //ModelState.AddModelError("postedFile", "Your file is too large. Maximum size allowed is 50MB !");
                        TempData["ExcelERR"] = "ERROR! Your file is too large. Maximum size allowed is 50MB!";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("Topics", "Quiz");
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("PastQuestions", "Quiz", new { id = topicid });
                        }
                        //return Json(new { Status = 0, Message = "Your file is too large. Maximum size allowed is 50MB !" });
                    }
                    else
                    {
                        string filePath = string.Empty;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + Path.GetFileName(postedFiles.FileName);
                        verifiedfilepath = filePath;
                        string extension = Path.GetExtension(postedFiles.FileName);
                        postedFiles.SaveAs(filePath);

                        string conString = string.Empty;
                        switch (extension)
                        {
                            case ".xls": //For Excel 97-03.  
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx": //For Excel 07 and above.  
                                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                break;
                        }

                        try
                        {
                            System.Data.DataTable dt = new System.Data.DataTable();
                            conString = string.Format(conString, filePath);

                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.  
                                        connExcel.Open();
                                        System.Data.DataTable dtExcelSchema;
                                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                        connExcel.Close();

                                        //Read Data from First Sheet.  
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dt);
                                        connExcel.Close();
                                    }
                                }
                            }

                            //int cnt = 0, QueID = 0;
                            //int OptionNumber = 0;
                            //bool correctans = false;
                            //if (Session["ResourceType"].ToString() == "PastQuestion")
                            //{
                            //    QYr = int.Parse(Session["QYear"].ToString());
                            //}

                            //string optletter = null;
                            //var Crsid = int.Parse(Session["CourseID"].ToString());
                            var str1 = string.Empty;
                            var str2 = string.Empty;
                            var strRet = "Error with Columns: ";
                            var strRetR = "Error with Cells: ";
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    str1 = dt.Columns[i].ColumnName;
                                    str2 = dt.Rows[j].ItemArray[i].ToString();
                                    if (i == 0)
                                    {
                                        //if (str1 != "SN")
                                        //{

                                        //}

                                    }
                                    else if (i == 1)
                                    {
                                        if (str1 != "QUESTION TEXT")
                                        {
                                            //if (!strRet.Contains(str1))
                                            //{
                                            //    strRet += str1 + " ";
                                            //}
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 2)
                                    {
                                        if (str1 != "ANSWER EXPLANATION")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 3)
                                    {
                                        if (str1 != "OPTION A")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 4)
                                    {
                                        if (str1 != "OPTION B")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 5)
                                    {
                                        if (str1 != "OPTION C")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 6)
                                    {
                                        if (str1 != "OPTION D")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 7)
                                    {
                                        if (str1 != "OPTION E")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 8)
                                    {
                                        if (str1 != "CORRECT OPTION")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 9)
                                    {
                                        if (str1 != "OPTIONS NUMBER")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 10)
                                    {
                                        if (str1 != "INSTRUCTION")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 11)
                                    {
                                        if (str1 != "QUESTION TYPE")
                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 12)
                                    {
                                        if (str1 != "FIG SLOTS")

                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 13)
                                    {
                                        if (str1 != "RESOURCE TYPE")

                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 14)
                                    {
                                        if (str1 != "IMAGE NAME")

                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }
                                    else if (i == 15)
                                    {
                                        if (str1 != "TOPIC")

                                        {
                                            strRet = CheckExist(strRet, str1);
                                        }
                                    }


                                    if (str2 == "")
                                    {
                                        int j1 = j + 1, i1 = i + 1;
                                        strRetR += "Cell R" + j1 + "C" + i1 + " ";
                                    }
                                }
                                if (strRet == "Error with Columns: ")
                                {
                                    strRet = "";
                                }

                            }
                            if (strRetR == "Error with Cells: ")
                            {
                                strRetR = "";
                            }
                            string strmsg = string.Empty;
                            if (strRet == "" && strRetR == "")
                            {
                                blVerified = true;
                                TempData["ExcelMSG"] = "ALERT! File Verification completed successfully without defined errors. ";
                            }
                            else
                            {
                                TempData["ExcelERR"] = "ERRORS: " + strRet + " : " + strRetR;
                            }

                            dt.Dispose();

                            if (Session["ResourceType"].ToString() == "Quiz")
                            {
                                return RedirectToAction("Topics", "Quiz");
                            }
                            else if (Session["ResourceType"].ToString() == "PastQuestion")
                            {
                                return RedirectToAction("PastQuestions", "Quiz");
                            }

                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                            return View("Error");

                        }
                        finally
                        {

                        }
                    }
                }

                TempData["ExcelERR"] = "ERROR! No file was selected!";

                if (Session["ResourceType"].ToString() == "Quiz")
                {
                    return RedirectToAction("Topics", "Quiz");
                }
                return RedirectToAction("PastQuestions", "Quiz");
                //return Json(new { Status = 0, Message = "no files were selected !" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }
        }

        private string CheckExist(string strRet, string str)
        {
            if (!strRet.Contains(str))
            {
                strRet += str + " ";
            }

            return strRet;
        }

        [HttpPost]
        public async Task<ActionResult> ImportFromExcelX(HttpPostedFileBase postedFile)
        {
            string strTopic = string.Empty;


            try
            {
                int QYr = DateTime.Now.Year;

                TempData["ExcelERR"] = null;
                TempData["ExcelMSG"] = null;

                int topicid = 0; //int.Parse(Session["TopicID"].ToString());
                //if (topicid <= 0)
                //{
                //    TempData["ExcelERR"] = "ERROR! You must select a Topic to process.";
                //    if (Session["ResourceType"].ToString() == "Quiz")
                //    {
                //        return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                //    }
                //    else if (Session["ResourceType"].ToString() == "PastQuestion")
                //    {
                //        return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                //    }
                //}
                if (ModelState.IsValid)
                {
                    if (postedFile == null)
                    {
                        TempData["ExcelERR"] = "ERROR! No File Was Selected !";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                        }
                        //return Json(new { Status = 0, Message = "No File Selected" });
                    }

                    if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                    {
                        //ModelState.AddModelError("postedFile", "Your file is too large. Maximum size allowed is 50MB !");
                        TempData["ExcelERR"] = "ERROR! Your file is too large. Maximum size allowed is 50MB !";
                        if (Session["ResourceType"].ToString() == "Quiz")
                        {
                            return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                        }
                        else if (Session["ResourceType"].ToString() == "PastQuestion")
                        {
                            return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                        }
                        //return Json(new { Status = 0, Message = "Your file is too large. Maximum size allowed is 50MB !" });
                    }
                    else
                    {
                        string filePath = string.Empty;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + Path.GetFileName(postedFile.FileName);
                        string extension = Path.GetExtension(postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        string conString = string.Empty;
                        switch (extension)
                        {
                            case ".xls": //For Excel 97-03.  
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx": //For Excel 07 and above.  
                                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                break;
                        }

                        try
                        {
                            System.Data.DataTable dt = new System.Data.DataTable();
                            conString = string.Format(conString, filePath);

                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.  
                                        connExcel.Open();
                                        System.Data.DataTable dtExcelSchema;
                                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                        connExcel.Close();

                                        //Read Data from First Sheet.  
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dt);
                                        connExcel.Close();
                                    }
                                }
                            }

                            int cnt = 0, QueID = 0;
                            int OptionNumber = 0;
                            bool correctans = false;
                            if (Session["ResourceType"].ToString() == "PastQuestion")
                            {
                                QYr = int.Parse(Session["QYear"].ToString());
                            }


                            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                            {
                                //string optletter = null;
                                foreach (DataRow objDataRow in dt.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                                    cnt++;
                                    var questr = objDataRow["QUESTION TEXT"].ToString().Trim();
                                    //if (questr.Contains("\'"))
                                    //{
                                    //    questr = questr.Replace("\'", "\"");
                                    //}

                                    var ansexplanation = objDataRow["ANSWER EXPLANATION"].ToString();
                                    //if(ansexplanation.Contains("\'"))
                                    //{
                                    //    ansexplanation = ansexplanation.Replace("\'", "\"");
                                    //}
                                    topicid = Convert.ToInt32(objDataRow["TOPIC"].ToString().Trim());
                                    //strTopic = objDataRow["TOPIC"].ToString().Trim();

                                    //Check if Category/Topic exists in the system and insert questions and answers
                                    Topic topic = nqz.Topics.FirstOrDefault(x => x.TopicID == topicid && x.IsDeleted == false);
                                    if (topic != null)
                                    {
                                        //Save Question Details
                                        var nque1 = nqz.Questions.FirstOrDefault(x => x.TopicID == topicid && x.QuestionString.Trim() == questr);
                                        if (nque1 == null)
                                        {
                                            Question nque = new Question()
                                            {
                                                TopicID = topicid,
                                                Number = "(" + cnt.ToString() + ")",
                                                QuestionString = questr,
                                                ResourceTypeID = int.Parse(Session["ResTypeID"].ToString()),
                                                QuestionTypeID = Convert.ToInt32(objDataRow["QUESTION TYPE"].ToString().Trim()),
                                                AnswerExplanation = ansexplanation.Trim(),
                                                QuestionMarks = Convert.ToInt32(objDataRow["MARKS"].ToString()),
                                                QuestionDuration = Convert.ToInt32(objDataRow["TIME ALLOTTED"].ToString()),
                                                Instruction = objDataRow["INSTRUCTION"].ToString().Trim(),

                                                Year = QYr,
                                                IsMock = false,
                                                IsDeleted = false,
                                                IsVerified = false,
                                                Created = DateTime.Now,
                                                Updated = DateTime.Now
                                            };
                                            nqz.Questions.Add(nque);
                                            await nqz.SaveChangesAsync();

                                            QueID = nque.QuestionID;
                                        }
                                        else
                                        {
                                            QueID = nque1.QuestionID;
                                        }

                                        //Save answer details
                                        OptionNumber = int.Parse(objDataRow["OPTIONS NUMBER"].ToString());
                                        for (int i = 1; i <= OptionNumber; i++)
                                        {

                                            if (objDataRow["CORRECT OPTION"].ToString() == ((OptionLetter)i).ToString())
                                            {
                                                correctans = true;
                                            }
                                            else
                                            {
                                                correctans = false;
                                            }

                                            var nans = nqz.Answers.FirstOrDefault(x => x.QuestionID == QueID && x.OptionLetter == ((OptionLetter)i).ToString());
                                            if (nans == null)
                                            {
                                                if (i == 1)
                                                {
                                                    await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION A"].ToString(), correctans);
                                                }
                                                else if (i == 2)
                                                {
                                                    await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION B"].ToString(), correctans);
                                                }
                                                else if (i == 3)
                                                {
                                                    await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION C"].ToString(), correctans);
                                                }
                                                else if (i == 4)
                                                {
                                                    await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION D"].ToString(), correctans);
                                                }
                                                else if (i == 5)
                                                {
                                                    await SaveAnswer(QueID, ((OptionLetter)i).ToString(), objDataRow["OPTION E"].ToString(), correctans);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (strTopic == null)
                                        {
                                            strTopic = " with errors. The following Topic number(s) not found: " + topicid.ToString();
                                        }
                                        else
                                        {
                                            strTopic = strTopic + ", " + topicid.ToString();
                                        }
                                    }
                                }
                                dt.Dispose();
                            }
                            TempData["ExcelMSG"] = "ALERT! File uploaded successfully! " + strTopic;
                            if (Session["ResourceType"].ToString() == "Quiz")
                            {
                                return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                            }
                            else if (Session["ResourceType"].ToString() == "PastQuestion")
                            {
                                return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                            }

                            //return Json(new { Status = 1, Message = "File uploaded successfully" });
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                            return View("Error");
                            //TempData["ExcelERR"] = "ERROR! File uploaded successfully!";
                            //return RedirectToAction("Topics", "Quiz");
                            //return Json("error" + e.Message);
                            //return Json(new { Status = 0, Message = ex.Message });
                        }
                        finally
                        {

                        }
                        //return RedirectToAction("Index");  
                    }
                }

                TempData["ExcelERR"] = "ERROR! No file was selected!";

                if (Session["ResourceType"].ToString() == "Quiz")
                {
                    return RedirectToAction("EditTopic", "Quiz", new { id = topicid });
                }
                return RedirectToAction("EditPastQuestion", "Quiz", new { id = topicid });
                //return Json(new { Status = 0, Message = "no files were selected !" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }
        }

        private async Task<ActionResult> SaveAnswer(int qid, string letter, string answer, bool correct)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                Answer nans = new Answer()
                {
                    QuestionID = qid,
                    OptionLetter = letter,
                    AnswerString = answer,
                    IsCorrect = correct,
                    IsDeleted = false,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                };
                nqz.Answers.Add(nans);
                await nqz.SaveChangesAsync();


                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        private async Task<ActionResult> UpdateAnswer(int qid, string letter, string answer, bool correct)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                var nans = nqz.Answers.FirstOrDefault(x => x.QuestionID == qid && x.OptionLetter == letter);
                if (nans == null)
                {
                    Answer nans1 = nqz.Answers.FirstOrDefault(x => x.AnswerID == nans.AnswerID);
                    {
                        nans1.AnswerString = answer;
                        nans1.IsCorrect = correct;
                        nans1.Updated = DateTime.Now;
                    };
                }

                await nqz.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }
        private ActionResult AllQuestions(int tpcid, int typeid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (typeid == 1)
                {
                    IEnumerable<QuestionViewModels> crslist = (from tpc in nqz.Topics
                                                               join que in nqz.Questions on tpc.TopicID equals que.TopicID
                                                               where que.IsDeleted == false && que.IsVerified == false && tpc.TopicID == tpcid && que.ResourceTypeID == typeid
                                                               orderby que.QuestionID descending //tpc.TopicName
                                                               select new QuestionViewModels
                                                               {
                                                                   QuestionID = que.QuestionID,
                                                                   TopicName = tpc.TopicName,
                                                                   Number = que.Number,
                                                                   QuestionString = que.QuestionString,
                                                                   AnswerExplanation = que.AnswerExplanation,
                                                                   QuestionMarks = que.QuestionMarks ?? 0,
                                                                   QuestionDuration = que.QuestionDuration ?? 0,
                                                                   Instruction = que.Instruction
                                                               }).ToList();

                    ViewBag.QuestionList = crslist;
                }
                else
                {
                    IEnumerable<QuestionViewModels> crslist = (from tpc in nqz.PastQuestions
                                                               join que in nqz.Questions on tpc.PastQuestionID equals que.TopicID
                                                               join qyr in nqz.Years on tpc.YearID equals qyr.YearID
                                                               where que.IsDeleted == false && que.IsVerified == false && tpc.PastQuestionID == tpcid && que.ResourceTypeID == typeid
                                                               orderby que.QuestionID descending
                                                               select new QuestionViewModels
                                                               {
                                                                   QuestionID = que.QuestionID,
                                                                   TopicName = qyr.YearName.ToString(),
                                                                   Number = que.Number,
                                                                   QuestionString = que.QuestionString,
                                                                   QuestionMarks = que.QuestionMarks ?? 0,
                                                                   QuestionDuration = que.QuestionDuration ?? 0,
                                                                   Instruction = que.Instruction
                                                               }).ToList();
                    ViewBag.QuestionList = crslist;
                }
                return null;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult EditQuestion(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();

            QuestionVModels model = new QuestionVModels();
            int ResTypeid = int.Parse(Session["ResTypeID"].ToString());
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (ViewBag.TopicList == null)
                {
                    ViewBag.TopicList = Session["TopicList"];
                }
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Question ncrs = nqz.Questions.SingleOrDefault(x => x.IsDeleted == false && x.QuestionID == id);
                    model.QuestionID = ncrs.QuestionID;
                    Session["QueID"] = ncrs.QuestionID;
                    model.TopicID = ncrs.TopicID ?? 0;
                    Session["EditTopicID"] = ncrs.TopicID;
                    model.Number = ncrs.Number;
                    model.QuestionString = ncrs.QuestionString;
                    model.AnswerExplanation = ncrs.AnswerExplanation;
                    model.QuestionMarks = ncrs.QuestionMarks ?? 0;
                    model.QuestionDuration = ncrs.QuestionDuration ?? 0;
                    model.Instruction = ncrs.Instruction;
                    model.ImagePath = ncrs.ImagePath;
                    //model.Image_File = 
                    model.IsVerified = ncrs.IsVerified ?? false;
                    //ViewBag.Msg = "Update Completed Successfully";
                    ViewBag.QueID = ncrs.QuestionID;

                    ViewBag.AddAnswers = "Add";
                    if (model.ImagePath != null)
                    {
                        Session["img"] = "1";
                    }
                    else
                    {
                        Session["img"] = "0";
                    }

                    AllQuestions(model.TopicID, ResTypeid);
                    AllAnswers(model.QuestionID);
                }
                Session["EditQuestionModel"] = model;
                return View("Questions", model);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpGet]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            int ResTypeid = int.Parse(Session["ResTypeID"].ToString());
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Question ncrs = nqz.Questions.SingleOrDefault(x => x.QuestionID == id);
                if (ncrs != null)
                {
                    sklid = ncrs.TopicID.Value;
                    ncrs.IsDeleted = true;
                    ncrs.Updated = DateTime.Now;
                    await nqz.SaveChangesAsync();
                }


                ViewBag.Msg = "Delete Completed Successfully";

                AllQuestions(sklid, ResTypeid);
                return RedirectToAction("Questions", "Quiz");
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        //-------------------------------------------------Answers Region-------------------------------------------------------
        [HttpGet]
        public ActionResult Answers(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            AnswerViewModels NAnsModel = new AnswerViewModels();
            try
            {
                blansflag = true;

                if (id == 0)
                {
                    if (Session["QueID"] != null)
                    {
                        id = int.Parse(Session["QueID"].ToString());
                        ViewBag.QueID = id;

                    }

                }
                ViewBag.QueID = id;
                //ViewBag.QuestionList = nqz.Questions.Where(x => x.IsDeleted == false).Select(x => new { x.QuestionID, x.QuestionString }).ToList();
                Question nque = nqz.Questions.FirstOrDefault(x => x.QuestionID == id);
                if (nque != null)
                {
                    NAnsModel.QuestionID = nque.QuestionID;
                    NAnsModel.QuestionNumber = nque.QuestionNumber ?? 0;
                    NAnsModel.QuestionString = nque.QuestionString;
                    ViewBag.QueText = nque.QuestionNumber + " " + nque.QuestionString;
                    Session["QueText"] = nque.QuestionNumber + " " + nque.QuestionString;
                    //Session["TopicID"] = nque.TopicID;
                    //TempData["QuesID"] = nque.QuestionID;
                }
                AllAnswers(id);
                //Session["ANSMODEL"] = NAnsModel;
                return View(NAnsModel);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Answers(AnswerViewModels model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //ViewBag.QuestionList = nqz.Questions.Where(x => x.IsDeleted == false).Select(x => new { x.QuestionID, x.QuestionString }).ToList();

                if (model.AnswerID <= 0)
                {
                    //if (ViewBag.AnswerList == null)
                    //{
                    //    ViewBag.AnswerList = Session["QueAns"];
                    //}
                    if (ViewBag.QueText == null)
                    {
                        ViewBag.QueText = Session["QueText"];
                    }
                    model.QuestionID = int.Parse(Session["QueID"].ToString());
                    ViewBag.QueID = model.QuestionID;
                    if (model.QuestionID > 0 && model.AnswerString == null)
                    {
                        Question nque = nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID);
                        if (nque != null)
                        {
                            ViewBag.QueText = nque.QuestionNumber + " " + nque.QuestionString;
                        }

                        AllAnswers(model.QuestionID);
                    }
                    else
                    {
                        if (ViewBag.QueText == null)
                        {
                            ViewBag.QueText = Session["QueText"];
                        }

                        Answer que = nqz.Answers.SingleOrDefault(x => x.QuestionID == model.QuestionID && x.IsDeleted == false && x.AnswerString == model.AnswerString);
                        if (que == null)
                        {
                            //if (model.QuestionID == null)
                            //{
                            //    ViewBag.Err = "Error! Select a Question";
                            //}
                            //else 
                            if (model.OptionLetter == null)
                            {
                                ViewBag.Err = "Error! Enter an Option Letter";
                            }
                            else if (model.AnswerString == null)
                            {
                                ViewBag.Err = "Error! Enter a Answer String";
                            }
                            //else if (model.IsCorrect == false)
                            //{
                            //    ViewBag.Err = "Error! You must Specify";
                            //}
                            else
                            {

                                model.QuestionID = int.Parse(Session["QueID"].ToString());
                                int MaxAns = nqz.AppSettings.SingleOrDefault().MaximumOptions.Value;
                                int AnsCnt = nqz.Answers.Where(x => x.QuestionID == model.QuestionID).Count();

                                if (AnsCnt == MaxAns)
                                {
                                    ViewBag.Err = "The maximum number of Answer options for Questions is " + MaxAns + ", therefore, Save transaction aborted";
                                }
                                else
                                {
                                    Answer ncrs = new Answer()
                                    {
                                        QuestionID = model.QuestionID,
                                        OptionLetter = model.OptionLetter,
                                        AnswerString = model.AnswerString,
                                        IsCorrect = model.IsCorrect,
                                        IsDeleted = false,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now,
                                    };
                                    nqz.Answers.Add(ncrs);
                                    await nqz.SaveChangesAsync();

                                    //model.QuestionID = ncrs.QuestionID ?? 0;
                                    ViewBag.QueID = ncrs.QuestionID ?? 0;
                                    ViewBag.Msg = "Record Saved Successfully";
                                }

                                //AllAnswers(model.QuestionID);
                                //ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.Err = "This Answer Already Exists for the selected Question therefore, Save transaction failed";
                        }
                    }
                }
                else
                {
                    if (ViewBag.QueText == null)
                    {
                        ViewBag.QueText = Session["QueText"];
                    }

                    if (ModelState.IsValid)
                    {
                        Answer que = nqz.Answers.SingleOrDefault(x => x.QuestionID == model.QuestionID && x.IsDeleted == false && x.AnswerString == model.AnswerString);
                        if (que != null)
                        {
                            Answer ncrs = nqz.Answers.SingleOrDefault(x => x.AnswerID == model.AnswerID);
                            //ncrs.AnswerID = model.AnswerID;
                            //ncrs.QuestionID = model.QuestionID;
                            ncrs.OptionLetter = model.OptionLetter;
                            ncrs.AnswerString = model.AnswerString;
                            ncrs.IsCorrect = model.IsCorrect;
                            ncrs.Updated = DateTime.Now;

                            await nqz.SaveChangesAsync();


                        }
                        else
                        {
                            Answer ncrs = nqz.Answers.SingleOrDefault(x => x.AnswerID == model.AnswerID);
                            //ncrs.AnswerID = model.AnswerID;
                            //ncrs.QuestionID = model.QuestionID;
                            ncrs.OptionLetter = model.OptionLetter;
                            ncrs.AnswerString = model.AnswerString;
                            ncrs.IsCorrect = model.IsCorrect;
                            ncrs.Updated = DateTime.Now;

                            await nqz.SaveChangesAsync();

                            ViewBag.Msg = "Update Completed Successfully";
                            //ViewBag.Err = "This Answer Already Exists for the selected Question therefore, Save transaction failed";
                        }
                        ViewBag.Msg = "Update Completed Successfully";
                    }

                }
                ViewBag.QueID = model.QuestionID;
                AllAnswers(model.QuestionID);
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult NewAnswer(AnswerViewModels model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //if (model == null)
                //{
                //    model = Session
                //}
                //ViewBag.QuestionList = nqz.Questions.Where(x => x.IsDeleted == false).Select(x => new { x.QuestionID, x.QuestionString }).ToList();

                //ModelState.Clear();
                if (ViewBag.AnswerList == null)
                {
                    ViewBag.AnswerList = Session["QueAns"];
                }
                //AllAnswers(model.QuestionID);
                if (ViewBag.QueText == null)
                {
                    ViewBag.QueText = Session["ViewBag.QueText"];
                }

                return View("Answers", model);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        private ActionResult AllAnswers(int queid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //if (queid > 0)
                //{
                var crslist = (from que in nqz.Questions
                               join tpc in nqz.Answers on que.QuestionID equals tpc.QuestionID
                               where tpc.IsDeleted == false && que.QuestionID == queid
                               orderby que.Number
                               select new AnswerViewModels
                               {
                                   AnswerID = tpc.AnswerID,
                                   QuestionString = que.QuestionString,
                                   OptionLetter = tpc.OptionLetter,
                                   AnswerString = tpc.AnswerString,
                                   IsCorrect = tpc.IsCorrect ?? false
                               }).ToList();
                Session["QueAns"] = crslist;
                ViewBag.AnswerList = crslist;
                //}
                //else
                //{
                //    var crslist = (from que in nqz.Questions
                //                   join tpc in nqz.Answers on que.QuestionID equals tpc.QuestionID
                //                   where tpc.IsDeleted == false
                //                   orderby que.Number
                //                   select new AnswerViewModels
                //                   {
                //                       AnswerID = tpc.AnswerID,
                //                       QuestionString = que.QuestionString,
                //                       OptionLetter = tpc.OptionLetter,
                //                       AnswerString = tpc.AnswerString,
                //                       IsCorrect = tpc.IsCorrect
                //                   }).ToList();

                //    ViewBag.AnswerList = crslist;
                //}
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditAnswer(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            AnswerViewModels model = new AnswerViewModels();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;
                ViewBag.QueID = model.QuestionID;
                //ViewBag.QuestionList = nqz.Questions.Where(x => x.IsDeleted == false).Select(x => new { x.QuestionID, x.QuestionString }).ToList();

                Answer ncrs = nqz.Answers.SingleOrDefault(x => x.IsDeleted == false && x.AnswerID == id);

                Question nque = nqz.Questions.FirstOrDefault(x => x.QuestionID == ncrs.QuestionID);
                if (nque != null)
                {
                    ViewBag.QueText = nque.QuestionNumber + " " + nque.QuestionString;
                }
                model.AnswerID = ncrs.AnswerID;
                model.QuestionID = ncrs.QuestionID ?? 0;
                model.OptionLetter = ncrs.OptionLetter;
                model.AnswerString = ncrs.AnswerString;
                model.IsCorrect = ncrs.IsCorrect ?? false;

                ViewBag.QueID = model.QuestionID;
                AllAnswers(model.QuestionID);
                return View("Answers", model);

                //ViewBag.Msg = "Update Completed Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> DeleteAnswer(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Answer ncrs = nqz.Answers.SingleOrDefault(x => x.AnswerID == id);
                    if (ncrs != null)
                    {
                        sklid = ncrs.QuestionID.Value;
                        ncrs.IsDeleted = true;
                        ncrs.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();
                    }


                    ViewBag.Msg = "Delete Completed Successfully";

                    AllAnswers(sklid);
                    return RedirectToAction("Answers", "Quiz");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> ClientQuiz(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();

            ClientQuestionViewModel NQuiz = new ClientQuestionViewModel();

            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    int quecnt = Convert.ToInt32(Session["QueCount"].ToString());
                    int quetime = Convert.ToInt32(Session["QuizDuration"].ToString());
                    ClientQuiz nclqz = new ClientQuiz
                    {
                        PersonID = Convert.ToInt64(Session["PID"].ToString()),
                        Token = Guid.NewGuid().ToString(),
                        TopicID = id,
                        QuestionCount = quecnt,
                        LastQuestionID = quecnt,
                        Duration = quetime,
                        IsDeleted = false,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    nqz.ClientQuizs.Add(nclqz);
                    //nqz.SaveChanges();
                    await nqz.SaveChangesAsync();
                }

                return View(NQuiz);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult ClientQuiz(ClientQuestionViewModel model)
        {
            //bool blAnswerFlag = false;
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            long NClientQueID = 0;
            //Session["ClientAns"] = null;
            ViewBag.ErrAnswer = null;
            ViewBag.AnsExplanation = null;
            Session["Marks"] = 0;
            Session["QuestionsAttempted"] = 0;
            try
            {

                return View(model);

            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpGet]
        public ActionResult QuizEval(Guid token, int? qno)
        {
            //TempData["AnsCorrect"] = null;
            //TempData["AnsIncorrect"] = null;
            TempData["QuizExpired"] = null;
            Session["QAnswered"] = "false";
            TempData["skipQueText"] = null;
            TempData["skipQue"] = null;
            TempData["QueEnd"] = null;
            blsubmitflag = false;
            Session["Submitted"] = null;
            try
            {
                if (Session.Count > 0)
                {

                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        if (Session["QuizStatus"].ToString() == "1")
                        {
                            //if (strAnsSel == "")
                            //{
                            //    Session["blsubmitflag"] = null;

                            if (ansflag == false)
                            {
                                TempData["posQue"] = null;
                                TempData["negQue"] = null;
                                TempData["Marks"] = null;
                                TempData["PrevAnswerID"] = null;
                            }

                            QuizInfoViewModels qzmodel = Session["QZINFOMODEL"] as QuizInfoViewModels;

                            //Session["quetime"] = qzmodel.QuizDuration;
                            //TempData["Rem_Time"] = qzmodel.QuizDuration;
                            int QueTime = qzmodel.QuizDuration; //int.Parse(Session["quetime"].ToString());

                            if (Session["Rem_Time"] == null)
                            {
                                Session["Rem_Time"] = DateTime.Now.AddMinutes(QueTime).AddSeconds(15).ToString("dd-MM-yyyy h:mm:ss tt");
                                //Session["Rem_Time"] = DateTime.Now.AddMinutes(0).AddSeconds(20).ToString("dd-MM-yyyy h:mm:ss tt");
                            }
                            TempData["Rem_Time"] = Session["Rem_Time"];



                            Session["TOKEN"] = token;
                            //int quecnt = int.Parse(Session["QueCount"].ToString());
                            long qzid = qzmodel.QuizID; //int.Parse(Session["QUIZID"].ToString());
                            int topid = qzmodel.TopicID;//int.Parse(Session["TopicID"].ToString());
                            if (token == null)
                            {
                                TempData["ErrMessage"] = "You have an invalid token. Please, try again.";
                                return RedirectToAction("Error");
                            }
                            int qcnt = qzmodel.QuestionsCount; //int.Parse(Session["QueCount"].ToString());
                            if (qno.GetValueOrDefault() < 1)
                            {
                                qno = 1;
                            }
                            //int qcnt = (from sm in nqz.Questions
                            //            where sm.TopicID == tid
                            //            select sm.QuestionID).Count();
                            if (qno.GetValueOrDefault() >= qcnt)
                            {
                                TempData["QueEnd"] = "1";
                                if (qno.GetValueOrDefault() > qcnt)
                                {
                                    qno = qcnt;
                                }
                            }

                            var pid = qzmodel.PersonID; // long.Parse(Session["PID"].ToString());

                            //var QuestionId = nqz.Questions.Where(x => x.TopicID == tid).Select(x => x.QuestionID).FirstOrDefault();
                            //var QuestionId = (from que in nqz.Questions
                            //                  join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
                            //                  where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == tid
                            //                  select new { que.QuestionID }).FirstOrDefault();

                            //Session["QueCount"] = qcnt;

                            if (qno > 0)
                            {
                                //var _model = nqz.Questions.Where(x => x.TopicID == tid && x.QuestionNumber == qno).Select(x => new QuestionXModels()
                                var _model = (from que in nqz.Questions
                                              join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
                                              where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == qzid && clque.QuestionNumber == qno
                                              select new QuestionXModels()
                                              {
                                                  QuestionID = que.QuestionID,
                                                  QuestionString = que.QuestionString,
                                                  QuestionNumber = clque.QuestionNumber ?? 0,
                                                  AnswerExplanation = que.AnswerExplanation,
                                                  Number = que.Number,
                                                  Token = clque.Token,
                                                  Instruction = que.Instruction,
                                                  ImagePath = que.ImagePath,
                                                  AnswerID = clque.AnswerID ?? 0,
                                                  Iscorrect = clque.IsCorrect ?? false,
                                                  Options = que.Answers.Where(y => y.IsDeleted == false).Select(y => new AnswerXModels()
                                                  {
                                                      AnswerID = y.AnswerID,
                                                      OptionLetter = y.OptionLetter,
                                                      AnswerString = y.AnswerString,
                                                      IsCorrect = y.IsCorrect ?? false,
                                                      Token = token.ToString(),
                                                      QuestionNumber = qno ?? 0
                                                  }).ToList()
                                              }).FirstOrDefault();

                                if (_model != null)
                                {
                                    if (_model.ImagePath != null)
                                    {
                                        Session["img"] = "1";
                                    }
                                    else
                                    {
                                        Session["img"] = "0";
                                    }
                                }

                                TempData["QuePosition"] = qno + " of " + qcnt;
                                //Session["QUESNum"] = _model.QuestionNumber;
                                Session["QUESANS"] = _model;
                                return View(_model);

                            }
                            else
                            {
                                TempData["QuizExpired"] = "1";
                                return View();
                            }
                        }
                        else
                        {
                            TempData["skipQueText"] = "You must click on Submit to get scored and continue.";
                            return View();
                        }
                        //}
                        //else
                        //{
                        //    ViewBag.ErrMessage = "Quiz Error: Oh! Your Quiz just timed out.";
                        //    return View("Error");
                        //}
                    }


                }
                else
                {
                    ViewBag.ErrMessage = "An unexpected error has occurred and you must log in again.";
                    return View("Error");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpGet]
        public async Task<ActionResult> SkillPracticeQuiz(Guid token, int? qno)
        {
            //TempData["AnsCorrect"] = null;
            //TempData["AnsIncorrect"] = null;
            TempData["QuizExpired"] = null;
            Session["QAnswered"] = "false";
            TempData["skipQueText"] = null;
            TempData["skipQue"] = null;
            TempData["QueEnd"] = null;
            blsubmitflag = false;
            Session["Submitted"] = null;

            TempData["FIGSlot1"] = "0";
            TempData["FIGSlot2"] = "0";

            if (firsttime == false)
            {
                Session["Counter"] = "0";
                firsttime = true;
            }

            try
            {
                if (Session.Count > 0)
                {

                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        if (Session["QuizStatus"].ToString() == "1")
                        {
                            //if (strAnsSel == "")
                            //{
                            //    Session["blsubmitflag"] = null;

                            if (ansflag == false)
                            {
                                TempData["posQue"] = null;
                                TempData["negQue"] = null;
                                TempData["Marks"] = null;
                                TempData["PrevAnswerID"] = null;
                            }

                            QuizInfoViewModels qzmodel = Session["QZINFOMODEL"] as QuizInfoViewModels;

                            Session["quetime"] = qzmodel.QuizDuration;
                            TempData["Rem_Time"] = qzmodel.QuizDuration;
                            //int QueTime = qzmodel.QuizDuration; //int.Parse(Session["quetime"].ToString());

                            //if (Session["Rem_Time"] == null)
                            //{
                            //    Session["Rem_Time"] = DateTime.Now.AddMinutes(QueTime).AddSeconds(15).ToString("dd-MM-yyyy h:mm:ss tt");
                            //    //Session["Rem_Time"] = DateTime.Now.AddMinutes(0).AddSeconds(20).ToString("dd-MM-yyyy h:mm:ss tt");
                            //}
                            //TempData["Rem_Time"] = Session["Rem_Time"];



                            Session["TOKEN"] = token;
                            //int quecnt = int.Parse(Session["QueCount"].ToString());
                            long qzid = qzmodel.QuizID; //int.Parse(Session["QUIZID"].ToString());
                            int topid = qzmodel.TopicID;//int.Parse(Session["TopicID"].ToString());
                            if (token == null)
                            {
                                TempData["ErrMessage"] = "You have an invalid token. Please, try again.";
                                return RedirectToAction("Error");
                            }
                            int qcnt = qzmodel.QuestionsCount; //int.Parse(Session["QueCount"].ToString());
                            if (qno.GetValueOrDefault() < 1)
                            {
                                qno = 1;
                            }
                            //int qcnt = (from sm in nqz.Questions
                            //            where sm.TopicID == tid
                            //            select sm.QuestionID).Count();
                            if (qno.GetValueOrDefault() >= qcnt)
                            {
                                TempData["QueEnd"] = "1";
                                if (qno.GetValueOrDefault() > qcnt)
                                {
                                    qno = qcnt;
                                }
                            }

                            var pid = qzmodel.PersonID; // long.Parse(Session["PID"].ToString());

                            //var QuestionId = nqz.Questions.Where(x => x.TopicID == tid).Select(x => x.QuestionID).FirstOrDefault();
                            //var QuestionId = (from que in nqz.Questions
                            //                  join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
                            //                  where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == tid
                            //                  select new { que.QuestionID }).FirstOrDefault();

                            //Session["QueCount"] = qcnt;

                            if (qno > 0)
                            {
                                //var _model = nqz.Questions.Where(x => x.TopicID == tid && x.QuestionNumber == qno).Select(x => new QuestionXModels()
                                var _model = (from que in nqz.Questions
                                              join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
                                              where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == qzid && clque.QuestionNumber == qno
                                              select new QuestionXModels()
                                              {
                                                  QuestionID = que.QuestionID,
                                                  QuestionTypeID = que.QuestionTypeID ?? 0,
                                                  QuestionString = que.QuestionString,
                                                  QuestionNumber = clque.QuestionNumber ?? 0,
                                                  AnswerExplanation = que.AnswerExplanation,
                                                  Number = que.Number,
                                                  Token = clque.Token,
                                                  Instruction = que.Instruction,
                                                  ImagePath = que.ImagePath,
                                                  FIGSlot = que.FIGSlot ?? 0,
                                                  Answer = que.Answer,
                                                  AnswerID = clque.AnswerID ?? 0,
                                                  Iscorrect = clque.IsCorrect ?? false,
                                                  Options = que.Answers.Where(y => y.IsDeleted == false).Select(y => new AnswerXModels()
                                                  {
                                                      AnswerID = y.AnswerID,
                                                      OptionLetter = y.OptionLetter,
                                                      AnswerString = y.AnswerString,
                                                      IsCorrect = y.IsCorrect ?? false,
                                                      Token = token.ToString(),
                                                      QuestionNumber = qno ?? 0
                                                  }).ToList()
                                              }).FirstOrDefault();

                                if (_model != null)
                                {
                                    if (_model.ImagePath != null)
                                    {
                                        Session["img"] = "1";
                                    }
                                    else
                                    {
                                        Session["img"] = "0";
                                    }
                                }

                                TempData["QuePosition"] = qno + " of " + qcnt;
                                //Session["QUESNum"] = _model.QuestionNumber;
                                Session["QUESANS"] = _model;
                                return View(_model);

                            }
                            else
                            {
                                TempData["QuizExpired"] = "1";
                                return View();
                            }
                        }
                        else
                        {
                            TempData["skipQueText"] = "You must click on Submit to get scored and continue.";
                            return View();
                        }
                        //}
                        //else
                        //{
                        //    ViewBag.ErrMessage = "Quiz Error: Oh! Your Quiz just timed out.";
                        //    return View("Error");
                        //}
                    }


                }
                else
                {
                    ViewBag.ErrMessage = "An unexpected error has occurred and you must log in again.";
                    return View("Error");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }

        }

        [HttpPost]
        public async Task<ActionResult> PostAnswers(QuestionXModels model)
        {
            try
            {
                long psnID = long.Parse(Session["PID"].ToString());
                model = Session["QUESANS"] as QuestionXModels;
                //long clqueid = 0;
                int queid = 0;
                int intansid = 0;
                int intprevansid = 0;
                int marks = 0;
                int quenum = 0;
                bool? ansStatus = false;
                bool queSkip = false;
                //string Direction = string.Empty;

                //Session["blsubmitflag"] = "1";

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var nclqz = nqz.ClientQuizs.FirstOrDefault(x => x.Token == model.Token);
                    if (nclqz == null)
                    {
                        ViewBag.ErrMessage = "The Token is Invalid ";
                        return View("Error");
                    }

                    if (model.QuestionTypeID == 2)
                    {
                        if (model.FIGSlot == 1)
                        {
                            ViewBag.FIGSlot1 = "1";
                        }
                        else if (model.FIGSlot == 2)
                        {
                            ViewBag.FIGSlot1 = "1";
                            ViewBag.FIGSlot2 = "1";
                        }
                    }

                    var rdoAnswer = Request.Form["rdoAnswer"];

                    var hdfDirection = Request.Form["Direction"];
                    if (strAnsSel == "false")
                    {
                        strAnsSel = Request.Form["AnswerSelection"];
                    }
                    //var AnsSel = 
                    quenum = model.QuestionNumber;
                    //quenum = int.Parse(Session["QUESNum"].ToString());
                    if (model != null)
                    {
                        if (model.ImagePath != null)
                        {
                            Session["img"] = "1";
                        }
                        else
                        {
                            Session["img"] = "0";
                        }
                    }

                    if (hdfDirection == "forward")
                    {
                        if (rdoAnswer != null)
                        {
                            //if (model.Answered == false)
                            //{
                            intansid = int.Parse(rdoAnswer.ToString());

                            intSubmit = 1;
                            var nclqz1 = nqz.ClientQuestions.Where(x => x.ClientQuizID == nclqz.ClientQuizID && x.PersonID == psnID && x.QuestionNumber == quenum)
                                    .Select(x => new
                                    {
                                        x.ClientQuestionID,
                                        x.QuestionMarks,
                                        x.QuestionID,
                                        x.AnswerID,
                                        x.Answered
                                    }).FirstOrDefault();
                            if (nclqz1 != null)
                            {
                                if (nclqz1.Answered == false)
                                {
                                    if (intansid != nclqz1.AnswerID)
                                    {
                                        Answer nans = nqz.Answers.FirstOrDefault(x => x.AnswerID == intansid);
                                        if (nans != null)
                                        {
                                            ansStatus = nans.IsCorrect;
                                            TempData["AnsIncorrect"] = nans.OptionLetter + " => " + nans.AnswerString;
                                        }

                                        if (ansStatus == true)
                                        {
                                            totalMarks += nclqz1.QuestionMarks;
                                            posQue++;

                                            int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                            if (expletive >= maxEnumValue)
                                            {
                                                expletive = 0;
                                            }

                                            var expltv = (Expletives)expletive;
                                            TempData["Expletive"] = expltv + "!";
                                            expletive++;

                                            //ViewBag.Marks = totalMarks;
                                            TempData["Marks"] = totalMarks;
                                            //ViewBag.posQue = "1";
                                            TempData["posQue"] = "1";
                                            ViewBag.SuccessAnswer = "true";
                                            bldisplayinterver = true;
                                        }
                                        else
                                        {

                                            Answer nans1 = nqz.Answers.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID && x.IsCorrect == true);
                                            if (nans1 != null)
                                            {
                                                TempData["AnsCorrect"] = nans1.OptionLetter + " => " + nans1.AnswerString;
                                            }
                                            TempData["AnsExplanation"] = nqz.Questions.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID).AnswerExplanation;

                                            totalMarks -= 2;
                                            TempData["Marks"] = totalMarks;
                                            //ViewBag.Marks = totalMarks;
                                            negQue--;
                                            //ViewBag.negQue = "1";
                                            TempData["Expletive"] = "Oooch!!!";
                                            TempData["negQue"] = "1";
                                            expletive = 0;
                                            ViewBag.SuccessAnswer = "false";

                                            //quenum++;
                                        }
                                    }

                                }
                                else
                                {
                                    TempData["Marks"] = totalMarks;
                                    //quenum++;
                                }

                            }

                            ClientQuestion clquestion = nqz.ClientQuestions.FirstOrDefault(x => x.ClientQuestionID == nclqz1.ClientQuestionID);
                            clquestion.AnswerID = intansid;
                            clquestion.IsCorrect = ansStatus;
                            clquestion.Answered = true;
                            clquestion.Updated = DateTime.UtcNow;
                            await nqz.SaveChangesAsync();
                            //}
                            //else
                            //{
                            //quenum++;
                            //}
                            Session["skipQue"] = null;
                            ansflag = true;

                        }
                        else
                        {
                            if (model.FIGSlot != 0)
                            {
                                var nclqz1 = nqz.Questions.Where(x => x.IsFree == true && x.QuestionNumber == quenum)
                                    .Select(x => new
                                    {
                                        x.QuestionMarks,
                                        x.QuestionID,
                                    }).FirstOrDefault();

                                if (model.Answer.Contains(","))
                                {
                                    string[] arrfigans = model.Answer.ToString().Split(',');
                                    if (arrfigans.Length == 2)
                                    {
                                        if (arrfigans[0].ToLower() == model.FIGAnswer1.ToLower() && arrfigans[1].ToLower() == model.FIGAnswer2.ToLower())
                                        {
                                            totalMarks += nclqz1.QuestionMarks;
                                            posQue++;

                                            int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                            if (expletive >= maxEnumValue)
                                            {
                                                expletive = 0;
                                            }

                                            var expltv = (Expletives)expletive;
                                            TempData["Expletive"] = expltv + "!";
                                            expletive++;

                                            TempData["Marks"] = totalMarks;
                                            TempData["posQue"] = "1";
                                            ViewBag.SuccessAnswer = "true";
                                            bldisplayinterver = true;
                                        }
                                        else
                                        {
                                            Answer nans1 = nqz.Answers.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID && x.IsCorrect == true);
                                            if (nans1 != null)
                                            {
                                                TempData["AnsCorrect"] = nans1.OptionLetter + " => " + nans1.AnswerString;
                                            }
                                            TempData["AnsExplanation"] = nqz.Questions.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID).AnswerExplanation;

                                            totalMarks -= 2;
                                            TempData["Marks"] = totalMarks;
                                            negQue--;
                                            TempData["Expletive"] = "Oooch!!!";
                                            TempData["negQue"] = "1";
                                            expletive = 0;
                                            ViewBag.SuccessAnswer = "false";
                                        }
                                    }
                                }
                                else
                                {
                                    if (model.Answer.ToLower() == model.FIGAnswer1.ToLower())
                                    {
                                        totalMarks += nclqz1.QuestionMarks;
                                        posQue++;

                                        int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                        if (expletive >= maxEnumValue)
                                        {
                                            expletive = 0;
                                        }

                                        var expltv = (Expletives)expletive;
                                        TempData["Expletive"] = expltv + "!";
                                        expletive++;

                                        TempData["Marks"] = totalMarks;
                                        TempData["posQue"] = "1";
                                        ViewBag.SuccessAnswer = "true";
                                        bldisplayinterver = true;
                                    }
                                    else
                                    {
                                        Answer nans1 = nqz.Answers.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID && x.IsCorrect == true);
                                        if (nans1 != null)
                                        {
                                            TempData["AnsCorrect"] = nans1.OptionLetter + " => " + nans1.AnswerString;
                                        }
                                        TempData["AnsExplanation"] = nqz.Questions.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID).AnswerExplanation;

                                        totalMarks -= 2;
                                        TempData["Marks"] = totalMarks;
                                        negQue--;
                                        TempData["Expletive"] = "Oooch!!!";
                                        TempData["negQue"] = "1";
                                        expletive = 0;
                                        ViewBag.SuccessAnswer = "false";
                                    }

                                }
                            }
                            else
                            {
                                TempData["Marks"] = totalMarks;
                                TempData["skipQue"] = "1";
                                TempData["skipQueText"] = "You must select an Option to get a score.";
                                Session["skipQue"] = "1";
                                Session["skipQueText"] = "You must select an Option to get a score.";
                                TempData["negQue"] = "1";

                                intSubmit = 0;
                            }
                        }


                    }
                    else
                    {
                        if (rdoAnswer != null)
                        {
                            if (intSubmit == 1)
                            {
                                TempData["Marks"] = totalMarks;

                                quenum++;
                                Session["skipQue"] = null;

                                intSubmit = 0;
                            }
                            else
                            {
                                TempData["Marks"] = totalMarks;
                                Session["skipQue"] = "1";
                                Session["skipQueText"] = "You must click on Submit to get scored.";
                                intSubmit = 0;
                            }
                        }
                        else
                        {
                            TempData["Marks"] = totalMarks;
                            TempData["skipQue"] = "1";
                            TempData["skipQueText"] = "You must select an Option to continue.";
                            Session["skipQue"] = "1";
                            Session["skipQueText"] = "You must select an Option to continue.";
                            intSubmit = 0;
                        }
                        //var nclqz1 = nqz.ClientQuestions.Where(x => x.ClientQuizID == nclqz.ClientQuizID && x.PersonID == psnID && x.QuestionNumber == quenum)
                        //            .Select(x => new
                        //            {
                        //                x.ClientQuestionID,
                        //                x.Answered
                        //            }).FirstOrDefault();
                        //if (nclqz1 != null)
                        //{
                        //    model.Answered = nclqz1.Answered ?? false;
                        //    Session["QAnswered"] = nclqz1.Answered ?? false;
                        //}

                    }
                }
                TempData["QuePosition"] = quenum + " of " + Session["QueCount"].ToString();

                return RedirectToAction("SkillPracticeQuiz", new
                {
                    @token = Session["TOKEN"],
                    @qno = quenum
                });

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        //[HttpGet]
        //public ActionResult QuizEval(Guid token, int? qno)
        //{
        //    //TempData["AnsCorrect"] = null;
        //    //TempData["AnsIncorrect"] = null;
        //    TempData["QuizExpired"] = null;
        //    Session["QAnswered"] = "false";
        //    TempData["skipQueText"] = null;
        //    TempData["skipQue"] = null;
        //    TempData["QueEnd"] = null;
        //    blsubmitflag = false;
        //    try
        //    {
        //        if (Session.Count > 0)
        //        {

        //            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
        //            {
        //                if (Session["QuizStatus"].ToString() == "1")
        //                {
        //                    //if (strAnsSel == "")
        //                    //{
        //                    //    Session["blsubmitflag"] = null;

        //                    if (ansflag == false)
        //                    {
        //                        TempData["posQue"] = null;
        //                        TempData["negQue"] = null;
        //                        TempData["Marks"] = null;
        //                        TempData["PrevAnswerID"] = null;
        //                    }

        //                    QuizInfoViewModels qzmodel = Session["QZINFOMODEL"] as QuizInfoViewModels;

        //                    int QueTime = qzmodel.QuizDuration; //int.Parse(Session["quetime"].ToString());

        //                    if (Session["Rem_Time"] == null)
        //                    {
        //                        Session["Rem_Time"] = DateTime.Now.AddMinutes(QueTime).AddSeconds(15).ToString("dd-MM-yyyy h:mm:ss tt");
        //                        //Session["Rem_Time"] = DateTime.Now.AddMinutes(0).AddSeconds(20).ToString("dd-MM-yyyy h:mm:ss tt");
        //                    }
        //                    TempData["Rem_Time"] = Session["Rem_Time"];

        //                    Session["TOKEN"] = token;
        //                    //int quecnt = int.Parse(Session["QueCount"].ToString());
        //                    long qzid = qzmodel.QuizID; //int.Parse(Session["QUIZID"].ToString());
        //                    int topid = qzmodel.TopicID;//int.Parse(Session["TopicID"].ToString());
        //                    if (token == null)
        //                    {
        //                        TempData["ErrMessage"] = "You have an invalid token. Please, try again.";
        //                        return RedirectToAction("Error");
        //                    }
        //                    int qcnt = qzmodel.QuestionsCount; //int.Parse(Session["QueCount"].ToString());
        //                    if (qno.GetValueOrDefault() < 1)
        //                    {
        //                        qno = 1;
        //                    }
        //                    //int qcnt = (from sm in nqz.Questions
        //                    //            where sm.TopicID == tid
        //                    //            select sm.QuestionID).Count();
        //                    if (qno.GetValueOrDefault() >= qcnt)
        //                    {
        //                        TempData["QueEnd"] = "1";
        //                        if (qno.GetValueOrDefault() > qcnt)
        //                        {
        //                            qno = qcnt;
        //                        }
        //                    }

        //                    var pid = qzmodel.PersonID; // long.Parse(Session["PID"].ToString());

        //                    //var QuestionId = nqz.Questions.Where(x => x.TopicID == tid).Select(x => x.QuestionID).FirstOrDefault();
        //                    //var QuestionId = (from que in nqz.Questions
        //                    //                  join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
        //                    //                  where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == tid
        //                    //                  select new { que.QuestionID }).FirstOrDefault();

        //                    //Session["QueCount"] = qcnt;

        //                    if (qno > 0)
        //                    {
        //                        //var _model = nqz.Questions.Where(x => x.TopicID == tid && x.QuestionNumber == qno).Select(x => new QuestionXModels()
        //                        var _model = (from que in nqz.Questions
        //                                      join clque in nqz.ClientQuestions on que.QuestionID equals clque.QuestionID
        //                                      where que.TopicID == topid && clque.PersonID == pid && clque.ClientQuizID == qzid && clque.QuestionNumber == qno
        //                                      select new QuestionXModels()
        //                                      {
        //                                          QuestionID = que.QuestionID,
        //                                          QuestionString = que.QuestionString,
        //                                          QuestionNumber = clque.QuestionNumber ?? 0,
        //                                          AnswerExplanation = que.AnswerExplanation,
        //                                          Number = que.Number,
        //                                          Instruction = que.Instruction,
        //                                          ImagePath = que.ImagePath,
        //                                          AnswerID = clque.AnswerID ?? 0,
        //                                          Iscorrect = clque.IsCorrect ?? false,
        //                                          Options = que.Answers.Where(y => y.IsDeleted == false).Select(y => new AnswerXModels()
        //                                          {
        //                                              AnswerID = y.AnswerID,
        //                                              OptionLetter = y.OptionLetter,
        //                                              AnswerString = y.AnswerString,
        //                                              IsCorrect = y.IsCorrect ?? false,
        //                                              Token = token.ToString(),
        //                                              QuestionNumber = qno ?? 0
        //                                          }).ToList()
        //                                      }).FirstOrDefault();

        //                        TempData["QuePosition"] = qno + " of " + qcnt;
        //                        Session["QUESNum"] = _model.QuestionNumber;
        //                        Session["QUESANS"] = _model;
        //                        return View(_model);

        //                    }
        //                    else
        //                    {
        //                        TempData["QuizExpired"] = "1";
        //                        return View();
        //                    }
        //                }
        //                else
        //                {
        //                    TempData["skipQueText"] = "You must click on Submit to get scored and continue.";
        //                    return View();
        //                }
        //                //}
        //                //else
        //                //{
        //                //    ViewBag.ErrMessage = "Quiz Error: Oh! Your Quiz just timed out.";
        //                //    return View("Error");
        //                //}
        //            }


        //        }
        //        else
        //        {
        //            ViewBag.ErrMessage = "An unexpected error has occurred and you must log in again.";
        //            return View("Error");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
        //        return View("Error");
        //    }

        //}

        //[HttpPost]
        //public async Task<ActionResult> PostAnswers(QuestionXModels model)
        //{
        //    try
        //    {
        //        long psnID = long.Parse(Session["PID"].ToString());
        //        //model = Session["QUESANS"] as QuestionXModels;
        //        //long clqueid = 0;
        //        int queid = 0;
        //        int intansid = 0;
        //        int intprevansid = 0;
        //        int marks = 0;
        //        int quenum = 0;
        //        bool? ansStatus = false;
        //        bool queSkip = false;
        //        //string Direction = string.Empty;

        //        //Session["blsubmitflag"] = "1";

        //        using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
        //        {
        //            var nclqz = nqz.ClientQuizs.FirstOrDefault(x => x.Token == model.Token);
        //            if (nclqz == null)
        //            {
        //                ViewBag.ErrMessage = "The Token is Invalid ";
        //                return View("Error");
        //            }

        //            var rdoAnswer = Request.Form["rdoAnswer"];
        //            var hdfDirection = Request.Form["Direction"];
        //            if (strAnsSel == "false")
        //            {
        //                strAnsSel = Request.Form["AnswerSelection"];
        //            }
        //            //var AnsSel = 
        //            quenum = int.Parse(Session["QUESNum"].ToString());

        //            if (hdfDirection == "forward")
        //            {
        //                if (rdoAnswer != null)
        //                {
        //                    if (model.Answered == false)
        //                    {
        //                        intansid = int.Parse(rdoAnswer.ToString());

        //                        var nclqz1 = nqz.ClientQuestions.Where(x => x.ClientQuizID == nclqz.ClientQuizID && x.PersonID == psnID && x.QuestionNumber == quenum)
        //                                .Select(x => new
        //                                {
        //                                    x.ClientQuestionID,
        //                                    x.QuestionMarks,
        //                                    x.QuestionID,
        //                                    x.AnswerID,
        //                                    x.Answered
        //                                }).FirstOrDefault();
        //                        if (nclqz1 != null)
        //                        {
        //                            if (nclqz1.Answered == false)
        //                            {
        //                                if (intansid != nclqz1.AnswerID)
        //                                {
        //                                    Answer nans = nqz.Answers.FirstOrDefault(x => x.AnswerID == intansid);
        //                                    if (nans != null)
        //                                    {
        //                                        ansStatus = nans.IsCorrect;
        //                                        TempData["AnsIncorrect"] = nans.OptionLetter + " => " + nans.AnswerString;
        //                                    }

        //                                    if (ansStatus == true)
        //                                    {
        //                                        totalMarks += nclqz1.QuestionMarks;
        //                                        posQue++;
        //                                        var expltv = (Expletives)expletive;
        //                                        TempData["Expletive"] = expltv;
        //                                        expletive++;
        //                                        //ViewBag.Marks = totalMarks;
        //                                        TempData["Marks"] = totalMarks;
        //                                        //Display positive Image
        //                                        //ViewBag.posQue = "1";
        //                                        TempData["posQue"] = "1";
        //                                        ViewBag.SuccessAnswer = "true";
        //                                        bldisplayinterver = true;
        //                                    }
        //                                    else
        //                                    {

        //                                        Answer nans1 = nqz.Answers.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID && x.IsCorrect == true);
        //                                        if (nans1 != null)
        //                                        {
        //                                            TempData["AnsCorrect"] = nans1.OptionLetter + " => " + nans1.AnswerString;
        //                                        }
        //                                        TempData["AnsExplanation"] = nqz.Questions.FirstOrDefault(x => x.QuestionID == nclqz1.QuestionID).AnswerExplanation;

        //                                        totalMarks -= 2;
        //                                        TempData["Marks"] = totalMarks;
        //                                        //ViewBag.Marks = totalMarks;
        //                                        negQue--;
        //                                        //ViewBag.negQue = "1";
        //                                        TempData["Expletive"] = "Oooch!!!";
        //                                        TempData["negQue"] = "1";
        //                                        expletive = 0;
        //                                        ViewBag.SuccessAnswer = "false";

        //                                        //quenum++;
        //                                    }
        //                                }

        //                            }
        //                            else
        //                            {
        //                                TempData["Marks"] = totalMarks;
        //                                //quenum++;
        //                            }

        //                        }

        //                        ClientQuestion clquestion = nqz.ClientQuestions.FirstOrDefault(x => x.ClientQuestionID == nclqz1.ClientQuestionID);
        //                        clquestion.AnswerID = intansid;
        //                        clquestion.IsCorrect = ansStatus;
        //                        clquestion.Answered = true;
        //                        clquestion.Updated = DateTime.UtcNow;
        //                        await nqz.SaveChangesAsync();
        //                    }
        //                    else
        //                    {
        //                        quenum++;
        //                    }
        //                }
        //                else
        //                {
        //                    //queid = int.Parse(Request.Form["QuestionID"].ToString());
        //                    TempData["Marks"] = totalMarks;
        //                    TempData["skipQue"] = "1";
        //                    TempData["skipQueText"] = "You must select an Option to get a score.";
        //                    TempData["negQue"] = "1";
        //                    //skipQue++;
        //                    //quenum++;

        //                }

        //                ansflag = true;
        //            }
        //            else
        //            {
        //                if (rdoAnswer != null)
        //                {
        //                    TempData["Marks"] = totalMarks;

        //                    quenum++;
        //                }
        //                else
        //                {
        //                    TempData["Marks"] = totalMarks;
        //                    TempData["skipQue"] = "1";
        //                    TempData["skipQueText"] = "You must select an Option to continue.";
        //                }
        //                //var nclqz1 = nqz.ClientQuestions.Where(x => x.ClientQuizID == nclqz.ClientQuizID && x.PersonID == psnID && x.QuestionNumber == quenum)
        //                //            .Select(x => new
        //                //            {
        //                //                x.ClientQuestionID,
        //                //                x.Answered
        //                //            }).FirstOrDefault();
        //                //if (nclqz1 != null)
        //                //{
        //                //    model.Answered = nclqz1.Answered ?? false;
        //                //    Session["QAnswered"] = nclqz1.Answered ?? false;
        //                //}

        //            }
        //        }
        //        TempData["QuePosition"] = quenum + " of " + Session["QueCount"].ToString();

        //        return RedirectToAction("QuizEval", new
        //        {
        //            @token = Session["TOKEN"],
        //            @qno = quenum
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
        //        return View("Error");
        //    }
        //}

        [HttpGet]
        //public async Task<ActionResult> QuizResult()
        public ActionResult QuizResult()
        {
            try
            {
                Session["QuizStatus"] = "0";

                //int nqzid = 0;
                ResultViewModel nres = Session["QZResult"] as ResultViewModel;


                //return View("QuizResult", nres);
                //return View("Quiz/Quiz")
                ViewBag.Archives = null;
                return View(nres);
                //return Json(new { success = true, responseText = "Registration Successful!" }, JsonRequestBehavior.AllowGet);
                //return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpGet]
        //public async Task<ActionResult> QuizResult()
        public ActionResult QResult()
        {
            try
            {
                Session["QuizStatus"] = "0";

                //int nqzid = 0;
                QResultViewModel nres = Session["QZResult"] as QResultViewModel;

                totalMarks = 0;
                varCorrectAnswer = "";
                varInCorrectAnswer = "";
                varAnswerExplanation = "";
                //return View("QuizResult", nres);
                //return View("Quiz/Quiz")
                ViewBag.Archives = null;
                return View(nres);
                //return Json(new { success = true, responseText = "Registration Successful!" }, JsonRequestBehavior.AllowGet);
                //return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        //[HttpPost]
        //public ActionResult PostAnswers(AnswerXModels model)
        //{
        //    try
        //    {

        //        ////model = Session["QUESANS"] as AnswerXModels;

        //        int intansid;
        //        int marks = 0;
        //        int quenum = 0;
        //        bool? ansStatus = false;
        //        CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //        var nclqz = nqz.ClientQuizs.FirstOrDefault(x => x.Token == model.Token);
        //        if (nclqz == null)
        //        {
        //            ViewBag.ErrMessage = "The Token is Invalid ";
        //            return View("Error");
        //        }

        //        var rdoAnswer = Request.Form["rdoAnswer"];
        //        //if (rdoAnswer != null)
        //        //{
        //        intansid = int.Parse(rdoAnswer);
        //        //    var SelectedAns = nqz.Answers.Where(x => x.AnswerID == intans)
        //        //        .Select(x => new
        //        //        {
        //        //ansStatus = model.IsCorrect;
        //        //});
        //        //intansid = rdoAnswer
        //        ansStatus = nqz.Answers.FirstOrDefault(x => x.AnswerID == intansid).IsCorrect;
        //        //}

        //        quenum = int.Parse(Session["QUESNum"].ToString());
        //        //var nclqz1 = (from cqz in nqz.ClientQuizs
        //        //              join que in nqz.Questions on cqz.TopicID equals cqz.TopicID
        //        //              where tpc.TopicID == id
        //        //              select new { ClientQuizID = cqz.ClientQuizID, Token = cqz.Token, PersonID = cqz.PersonID, QuestionCount = cqz.QuestionCount, Topic = tpc.TopicName }).FirstOrDefault();
        //        var nclqz1 = nqz.ClientQuestions.Where(x => x.ClientQuizID == nclqz.ClientQuizID && x.QuestionNumber == quenum)
        //                .Select(x => new
        //                {
        //                    CQueID = x.ClientQuestionID,
        //                    Marks = x.QuestionMarks,
        //                    QueID = x.QuestionID
        //                }).FirstOrDefault();
        //        if (nclqz1 != null)
        //        {
        //            marks = nclqz1.Marks ?? 0;

        //            if (ansStatus == true)
        //            {
        //                totalMarks += marks;
        //                posQue++;
        //                ViewBag.Marks = totalMarks;
        //                //Display positive Image
        //                ViewBag.posQue = "1";
        //            }
        //            else
        //            {
        //                ViewBag.Marks = totalMarks;
        //                negQue--;
        //                ViewBag.negQue = "1";
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Marks = totalMarks;
        //            negQue--;
        //            ViewBag.negQue = "1";
        //        }

        //        ClientQuestion clquestion = nqz.ClientQuestions.FirstOrDefault(x => x.ClientQuestionID == nclqz1.CQueID);
        //        clquestion.AnswerID = intansid;
        //        clquestion.IsCorrect = ansStatus;
        //        clquestion.QuestionMarks = marks;
        //        clquestion.Token = model.Token;
        //        clquestion.Updated = DateTime.UtcNow;

        //        //nqz.SaveChanges();

        //        //var nextQuestionNumber = 1;
        //        //if
        //        quenum++;
        //        return RedirectToAction("QuizEval", new
        //        {
        //            @token = Session["TOKEN"],
        //            @qno = quenum
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
        //        return View("Error");
        //    }
        //}

        public System.Data.DataTable LINQToDataTable<t>(IEnumerable<t> varlist)
        {
            System.Data.DataTable dtReturn = new System.Data.DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (t rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        #region QuizTimer
        public ActionResult QuizManager(int id)
        {
            //int dur = int.Parse(Session["QuizDuration"].ToString()) + 2;
            //PostCountdownTimer(dur);
            //TimerViewModel model = new TimerViewModel();
            //TempData["EndTime"] = Session["EndTime"];

            return View();

        }

        //[HttpPost]
        //public void PostCountdownTimer(TimerViewModel model)
        //public void PostCountdownTimer(int dur)
        //{
        //    if (Session["EndTime"] == null)
        //    {
        //        Session["EndTime"] = DateTime.Now.AddHours(0).AddMinutes(0).AddSeconds(dur).ToString("dd-MM-yyyy h:mm:ss tt");
        //        //Session["EndTime"] = DateTime.Now.AddHours(model.Hour).AddMinutes(model.Minute).AddSeconds(model.Second).ToString("dd-MM-yyyy h:mm:ss tt");
        //    }
        //    TempData["EndTime"] = Session["EndTime"];
        //    //Response.Redirect("/Quiz/QuizManager");
        //}

        [HttpGet]
        // Destroys EndTime Session object
        //public JsonResult StopTimer()
        public async Task<ActionResult> StopTimer(string CompTime)
        {
            try
            {
                //Session.Abandon();
                Session["QuizStatus"] = "0";
                long nqzid = 0;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    QResultViewModel nres1 = new QResultViewModel();

                    if (Session["QuizType"].ToString() == "FreeBees")
                    {
                        nqzid = int.Parse(Session["FreeTrialQuizID"].ToString());

                        FreeTrialQuiz trialQuiz = nqz.FreeTrialQuizs.FirstOrDefault(x => x.FreeTrialQuizID == nqzid);
                        trialQuiz.QuizDuration = int.Parse(Session["Duration"].ToString());
                        trialQuiz.QueCount = int.Parse(Session["QueCount"].ToString());
                        trialQuiz.QuizName = Session["QuizName"].ToString();
                        trialQuiz.TimeSpent = CompTime;
                        trialQuiz.Score = totalMarks;
                        trialQuiz.QuestionsAttempted = AttemptedQuestions;
                        trialQuiz.Success = POSQuestions;
                        trialQuiz.Failure = NEGQuestions;
                        trialQuiz.Updated = DateTime.UtcNow;
                        //trialQuiz.
                        await nqz.SaveChangesAsync();

                        trialQuiz.Percentage = totalMarks;

                        var qry = (from ftr in nqz.FreeTrials
                                   join ftrqz in nqz.FreeTrialQuizs
                                    on ftr.FreeTrialID equals ftrqz.FreeTrialID
                                   where ftrqz.FreeTrialQuizID == nqzid
                                   select new
                                   {
                                       FEmail = ftr.Email,
                                       FPhone = ftr.Phone,
                                       FToken = ftrqz.Token

                                   }).FirstOrDefault();

                        if (totalMarks > 100)
                        {
                            totalMarks = 100;
                        }

                        nres1.TimeSpent = CompTime;
                        nres1.Token = qry.FToken;
                        nres1.Client = qry.FEmail + " : " + qry.FPhone;
                        nres1.Topic = Session["QuizName"].ToString();
                        nres1.TotalQuestions = int.Parse(Session["QueCount"].ToString());
                        nres1.QuizScores = totalMarks ?? 0;
                        nres1.Attemped = AttemptedQuestions;
                        nres1.SuccessQuestion = POSQuestions;
                        nres1.FailedQuestions = NEGQuestions;
                        nres1.Percentage = totalMarks ?? 0;
                        nres1.Qualification = Qualification(totalMarks ?? 0);

                        Session["QZResult"] = nres1;
                    }
                    else
                    {
                        ResultViewModel nres = new ResultViewModel();
                        //Save the End Time
                        nqzid = int.Parse(Session["QUIZID"].ToString());
                        string endtime = DateTime.Now.ToShortTimeString().ToString();
                        ClientQuiz nclt = nqz.ClientQuizs.FirstOrDefault(x => x.ClientQuizID == nqzid);
                        nclt.EndTime = Session["Rem_Time"].ToString() + " mins.";
                        nclt.Score = totalMarks;
                        nclt.Percentage = (totalMarks / nclt.TotalScore) * 100;
                        nclt.Updated = DateTime.UtcNow;
                        await nqz.SaveChangesAsync();

                        nres.TopicID = nclt.TopicID ?? 0;
                        nres.Quiz = Session["QzSubject"].ToString();
                        nres.StartTime = nclt.StartTime;
                        nres.EndTime = nclt.EndTime;
                        nres.Percentage = nclt.Percentage ?? 0;
                        nres.Points = nclt.Score ?? 0;
                        nres.Qualification = Qualification(nclt.Score ?? 0);
                        nres.Token = nclt.Token;
                        nres.TotalPoints = nclt.TotalScore ?? 0;
                        nres.Topic = Session["QzTopic"].ToString();

                        Session["QZResult"] = nres;
                    }


                }



                ////return View("QuizResult", nres);
                ////return View(nres);
                //return RedirectToAction("QuizResult");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //return View("Error");
            }
        }




        #endregion
        //public ActionResult =  ClientQuiz(ClientQuestion model)
        //{
        //    //DBQUIZZEntities nqz = new DBQUIZZEntities();
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    try
        //    {
        //        ViewBag.CourseList = nqz.Courses.Where(x => x.IsDeleted == false).Select(x => new { x.CourseID, x.CourseName }).ToList();
        //        ViewBag.TopicList = nqz.Topics.Where(x => x.IsDeleted == false && x.CourseID == model.CourseID).Select(x => new { x.TopicID, x.TopicName }).ToList();

        //        if (model.CourseID > 0)
        //        {
        //            if (model.TopicID <= 0 || model.TopicID == null)
        //            {
        //                ViewBag.TopicList = nqz.Topics.Where(x => x.IsDeleted == false && x.CourseID == model.CourseID).Select(x => new { x.TopicID, x.TopicName }).ToList();
        //            }
        //            else
        //            {
        //                int cnt = 0;
        //                var questionlist = nqz.Questions.Where(x => x.IsDeleted == false && x.TopicID == model.TopicID)
        //                    .Select(x => new { x.QuestionID }).ToList();
        //                //var questionlist = (from cou in nqz.Courses
        //                //                    join top in nqz.Topics on cou.CourseID equals top.CourseID
        //                //                    join que in nqz.Questions on top.TopicID equals que.TopicID
        //                //                    where que.IsDeleted == false && cou.CourseID == model.CourseID && top.TopicID == model.TopicID
        //                //                    select new { que.QuestionID });


        //                foreach (var item in questionlist)
        //                {
        //                    cnt++;
        //                    ClientQuestion ncq = nqz.ClientQuestions.SingleOrDefault(x => x.QuestionID == item.QuestionID && x.QuestionNumber == cnt && x.TopicID == model.TopicID && x.PersonID == 1);
        //                    if (ncq == null)
        //                    {
        //                        ClientQuestion ncque = new ClientQuestion();
        //                        {
        //                            ncque.QuestionID = item.QuestionID;
        //                            ncque.PersonID = 1;
        //                            ncque.CourseID = model.CourseID;
        //                            ncque.TopicID = model.TopicID;
        //                            ncque.QuestionNumber = cnt;
        //                            ncque.AnswerID = 0;
        //                            ncque.IsCorrect = false;
        //                            ncque.Created = DateTime.Now;
        //                            ncque.Updated = DateTime.Now;

        //                            nqz.ClientQuestions.Add(ncque);
        //                            nqz.SaveChanges();
        //                        };
        //                    }
        //                }

        //                ClientQuestion ncqq = nqz.ClientQuestions.SingleOrDefault(x => x.PersonID == 1 && x.CourseID == model.CourseID && x.TopicID == model.TopicID && x.QuestionNumber == 1);
        //                if (ncqq != null)
        //                {
        //                    Question nque = nqz.Questions.SingleOrDefault(x => x.QuestionID == ncqq.QuestionID);
        //                    if (nque != null)
        //                    {
        //                        model.QuestionID = nque.QuestionID;
        //                        model.QuestionString = nque.QuestionString;
        //                        model.QuestionNumber = ncqq.QuestionNumber;
        //                    }

        //                    //ViewBag.AnswerList = nqz.Answers.Where(x => x.QuestionID == nque.QuestionID && x.IsDeleted == false).Select(x => new { x.AnswerID, x.AnswerString }).ToList();

        //                }

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        if (ex.Message.Contains("failed on Open") || ex.Message.Contains("network-related"))
        //        {
        //            //return RedirectToAction("Login", "Account");
        //            ViewBag.Err = "A Network-related error occurred. Check network connectivity ";
        //        }
        //        else
        //        {
        //            ViewBag.Err = "An Error Occurred: " + ex.InnerException;
        //        }
        //    }
        //    //AllAnswers(sklid);

        //    if (ViewBag.AnswerList != null)
        //    {
        //        return View(model);
        //    }
        //    else
        //    {
        //        return View(model);// RedirectToAction("Answers", "Quiz");
        //    }
        //}

        [HttpGet]
        public ActionResult CurrentClass(int id)
        {
            ViewBag.SubscriptionERR = null;
            Session["clscnt"] = "0";
            bool ret = false;
            //try
            //{
            long? currSubID = 0;
            int? prcid = 0;
            int? clsid = 0;
            //DBQUIZZEntities nqz = new DBQUIZZEntities();

            try
            {
                var pid = long.Parse(Session["PID"].ToString());
                //Person nper = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                //if (nper != null)
                //{
                //    currSubID = nper.ActiveSubscriptionID;
                //}
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == id && x.IsActive == true);
                    if (nsub != null)
                    {
                        /*prcid = nsub.PricingID*/

                        if (id == 1)
                        {
                            var nfac = nqz.Faculties.Where(x => x.IsDeleted == false && x.ResourceTypeID == id).Select(x => new { x.FacultyID, x.FacultyName }).FirstOrDefault();

                            if (nfac != null)
                            {
                                clsid = nfac.FacultyID;
                                ViewBag.SubjectTitle = nfac.FacultyName;
                                Session["QzSubject"] = nfac.FacultyName;
                            }
                        }
                        else
                        {
                            var nres = nqz.ResourceTypes.Where(x => x.IsDeleted == false && x.ResourceTypeID == id).Select(x => new { x.ResourceTypeID, x.ResourceTypeName }).FirstOrDefault();
                            if (nres != null)
                            {
                                ViewBag.SubjectTitle = nres.ResourceTypeName;
                                Session["QzSubject"] = nres.ResourceTypeName;
                            }
                        }
                    }
                    else
                    {
                        ret = true;
                        return RedirectToAction("Subscriptions", "Account");
                        //ViewBag.SubscriptionERR = "There is no active Subscription running for Resource. Please, ";
                    }
                    //var nprc = (from prc in nqz.Pricings
                    //            join fac in nqz.Faculties on prc.FacultyID equals fac.FacultyID
                    //            where prc.IsDeleted == false && prc.PricingID == prcid
                    //            select new
                    //            {
                    //                CourseID = fac.FacultyID,
                    //                CourseName = fac.FacultyName
                    //            }).FirstOrDefault();
                    //if (nprc != null)
                    //{
                    //    clsid = nprc.CourseID;
                    //    ViewBag.SubjectTitle = nprc.CourseName;
                    //}

                    if (ret == false)
                    {
                        if (id == 1)
                        {
                            var _model = nqz.Courses.Where(x => x.FacultyID == clsid && x.IsDeleted == false).Select(x => new CurrentClassViewModels()
                            {
                                CourseID = x.CourseID,
                                CourseName = x.CourseName,
                                CurrentTopics = nqz.Topics.Where(y => y.CourseID == x.CourseID && y.IsDeleted == false).Select(y => new ClassTopics()
                                {
                                    TopicID = y.TopicID,
                                    TopicName = y.TopicName
                                }).ToList()
                            }).ToList();

                            Session["clscnt"] = _model.Count();

                            return View(_model);
                        }
                        else if (id == 2)
                        {
                            var _model = nqz.Faculties.Where(x => x.ResourceTypeID == id && x.IsDeleted == false).Select(x => new CurrentClassViewModels()
                            {
                                CourseID = x.FacultyID,
                                CourseName = x.FacultyName,
                                CurrentTopics = (from crs in nqz.Courses
                                                 join pqn in nqz.PastQuestions on crs.CourseID equals pqn.CourseID
                                                 join yrs in nqz.Years on pqn.YearID equals yrs.YearID
                                                 where crs.FacultyID == x.FacultyID && crs.IsDeleted == false
                                                 select new ClassTopics()
                                                 {
                                                     TopicID = pqn.PastQuestionID,
                                                     TopicName = crs.CourseName + " " + yrs.YearName
                                                 }).ToList()

                            }).ToList();

                            Session["clscnt"] = _model.Count();

                            return View(_model);
                        }
                    }

                }
                return View();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        public ActionResult ViewTopics(int id)
        {
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            //var pid = long.Parse(Session["PID"].ToString());
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    ViewBag.SelectedCourse = nqz.Courses.FirstOrDefault(x => x.CourseID == id).CourseName.ToString();

                    IEnumerable<TopicViewModels> ClientTopics = nqz.Topics.Where(x => x.CourseID == id && x.IsDeleted == false).Select(x => new TopicViewModels
                    {
                        TopicID = x.TopicID,
                        TopicName = x.TopicName
                    }).ToList();


                    return View(ClientTopics);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        //[HttpGet]
        //public ActionResult QuizInfo(int id)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    //var pid = long.Parse(Session["PID"].ToString());

        //    Topic ntopic = nqz.Topics.FirstOrDefault(x => x.TopicID == id && x.IsDeleted == false);
        //    if (ntopic != null)
        //    {
        //        //    QuizInfoViewModels nqzm = new QuizInfoViewModels();
        //        //    nqzm.QuizID = id;
        //        //    nqzm.QuizName = ntopic.TopicName;
        //        //    nqzm.Count = 35;
        //        //    nqzm.Duration = 20;
        //        ViewBag.QuizName = ntopic.TopicName;
        //        ViewBag.QuizDescription = "";

        //        ViewBag.QuizDuration = "35";
        //        ViewBag.QuestionCount = "20";

        //        Session["QuizID"] = id;
        //    }


        //    return View();
        //}

        //[HttpGet]
        //public ActionResult QuizInfo(int id)
        //{


        //    try
        //    {
        //        using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
        //        {
        //            var pid = long.Parse(Session["PID"].ToString());
        //            QuizInfoViewModels nqzm = new QuizInfoViewModels();

        //            int? quetime = (from cnt in nqz.Questions
        //                            where cnt.TopicID == id
        //                            select cnt.QuestionDuration).Sum();

        //            quetime = 1;

        //            int? TotalScore = (from cnt in nqz.Questions
        //                               where cnt.TopicID == id
        //                               select cnt.QuestionMarks).Sum();

        //            int Tquecnt = (from cnt in nqz.Questions
        //                           where cnt.TopicID == id
        //                           select cnt.QuestionID).Count();

        //            int quecnt = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).QuestionsBatches.Value;
        //            if (quecnt > Tquecnt)
        //            {
        //                quecnt = Tquecnt;
        //            }

        //            ClientQuiz nclqz = nqz.ClientQuizs.FirstOrDefault(x => x.PersonID == pid && x.TopicID == id);
        //            if (nclqz == null)
        //            {

        //                Topic ntopic = nqz.Topics.FirstOrDefault(x => x.TopicID == id && x.IsDeleted == false);
        //                if (ntopic != null)
        //                {
        //                    nqzm.PersonID = pid;
        //                    nqzm.TopicID = id;
        //                    nqzm.QuizName = ntopic.TopicName;
        //                    nqzm.Count = quecnt;
        //                    nqzm.Duration = quetime ?? 0;
        //                    //nqzm.QuizName = 
        //                    //ViewBag.QuizName = ntopic.TopicName;
        //                    //ViewBag.QuizDescription = "";

        //                    Session["quetime"] = quetime;
        //                    //ViewBag.QuizDuration = "35";
        //                    //ViewBag.QuizCount = "20";
        //                }
        //            }
        //            else
        //            {

        //                Session["quetime"] = quetime;

        //                //ClientQuiz nclqz1 = nqz.ClientQuizs.FirstOrDefault(x => x.ClientQuizID == nclqz.ClientQuizID);
        //                var nclqz1 = (from tpc in nqz.Topics
        //                              join cqz in nqz.ClientQuizs on tpc.TopicID equals cqz.TopicID
        //                              where tpc.TopicID == id && cqz.PersonID == pid
        //                              select new { cqz.ClientQuizID, cqz.Token, cqz.PersonID, cqz.QuestionCount, Topic = tpc.TopicName }).FirstOrDefault();
        //                if (nclqz1 != null)
        //                {
        //                    nqzm.QuizID = nclqz1.ClientQuizID;
        //                    nqzm.Token = nclqz1.Token;
        //                    //Session["QUIZID"] = nclqz1.ClientQuizID;
        //                    nqzm.PersonID = nclqz1.PersonID ?? 0;
        //                    nqzm.TopicID = id;
        //                    nqzm.QuizName = nclqz1.Topic;
        //                    //ViewBag.QuizName = nclqz1.Topic;
        //                    nqzm.Count = nclqz1.QuestionCount ?? 0;
        //                    nqzm.Duration = quetime ?? 0; // nclqz1.Duration ?? 0;

        //                    quecnt = nclqz1.QuestionCount ?? 0;
        //                }
        //            }
        //            Session["QueCount"] = quecnt;
        //            Session["TotalScore"] = TotalScore;
        //            //Session["QUIZID"] = nclqz1;
        //            Session["QZINFOMODEL"] = nqzm;
        //            return View(nqzm);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
        //        return View("Error");
        //    }
        //}
        [HttpGet]
        public ActionResult QuizInfo(int id)
        {
            try
            {
                Session["QZResult"] = null;
                Session["blsubmitflag"] = null;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var pid = long.Parse(Session["PID"].ToString());
                    QuizInfoViewModels nqzm = new QuizInfoViewModels();

                    int TotalScore = 100;
                    //int? TotalScore = (from cnt in nqz.Questions
                    //                   where cnt.TopicID == id
                    //                   select cnt.QuestionMarks).Sum();

                    int Tquecnt = (from cnt in nqz.Questions
                                   where cnt.TopicID == id
                                   select cnt.QuestionID).Count();

                    int questart = 0, questop = 0;

                    //Get Default Quiz duration and number of questions
                    var defvalues = nqz.AppSettings.Where(x => x.ID == 1).Select(x => new { x.QuestionsBatches, x.QuizPeriod }).FirstOrDefault();
                    int quetime = int.Parse(defvalues.QuizPeriod.ToString()); //Quiz Duration

                    int defquecnt = defvalues.QuestionsBatches ?? 0; //Number of questions

                    //Get number of quiz done
                    int quesdone = 0;
                    int qzcnt = (from cnt in nqz.ClientQuizs where cnt.PersonID == pid && cnt.TopicID == id select cnt.ClientQuizID).Count();
                    if (qzcnt == 0)
                    {
                        qzcnt++;

                        quesdone = 0;
                    }
                    else
                    {
                        //Calculate total number of questions attempted
                        if (Tquecnt > defquecnt)
                        {
                            quesdone = defquecnt * qzcnt;
                        }
                        else
                        {
                            quesdone = Tquecnt;
                        }
                    }


                    //Get Serial number of first question for the Topic
                    int quesn = nqz.Questions.FirstOrDefault(x => x.TopicID == id).QuestionID;

                    //Determine which questions to pass to the quiz
                    if (quesdone == 0)
                    {
                        questart = quesn;

                        if (Tquecnt > defquecnt)
                        {
                            questop = questart + defquecnt;
                        }
                        else
                        {
                            questop = questart + Tquecnt;
                        }
                    }
                    else
                    {
                        questart = quesn + quesdone;

                        questop = questart + defquecnt;
                    }

                    if (Tquecnt < defquecnt)
                    {
                        defquecnt = Tquecnt;
                    }

                    questop = GetLastID(questart, id, defquecnt);

                    Topic ntopic = nqz.Topics.FirstOrDefault(x => x.TopicID == id && x.IsDeleted == false);
                    if (ntopic != null)
                    {
                        nqzm.PersonID = pid;
                        nqzm.TopicID = id;
                        nqzm.QuizName = ntopic.TopicName;
                        Session["QzTopic"] = ntopic.TopicName;
                        Session["QzTitle"] = ntopic.TopicName.ToUpper() + " QUIZ " + " " + "1 - " + defquecnt + " : " + DateTime.Now.ToLongDateString().ToString().ToUpper();
                        nqzm.QuestionsCount = defquecnt;
                        nqzm.QuizDuration = quetime;
                        nqzm.TotalMarks = TotalScore;
                        nqzm.StartSN = questart;
                        nqzm.StopSN = questop;
                        //nqzm.QuizName = 
                        //ViewBag.QuizName = ntopic.TopicName;
                        //ViewBag.QuizDescription = "";


                        //ViewBag.QuizDuration = "35";
                        //ViewBag.QuizCount = "20";
                    }

                    Session["quetime"] = quetime;
                    Session["QueCount"] = defquecnt;
                    Session["TotalScore"] = TotalScore;
                    //Session["QUIZID"] = nclqz1;
                    Session["QZINFOMODEL"] = nqzm;
                    return View(nqzm);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpGet]
        private int GetLastID(int firstid, int topicid, int qnum = 0)
        {
            int lastid = 0; //, lid = 0;
            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
            {
                //Topic ntopic = nqz.Topics.FirstOrDefault(x => x.TopicID == topicid && x.IsDeleted == false).;
                var tpcs = (from cnt in nqz.Questions
                            where cnt.TopicID == topicid && cnt.QuestionID >= firstid
                            select new { QueID = cnt.QuestionID }).ToList();
                //lid = tpcs.Max(x => x.QueID);
                int itemcnt = tpcs.Count();
                if (itemcnt > qnum)
                {
                    itemcnt = qnum;

                    for (var i = 1; i <= tpcs.Count(); i++)
                    {
                        if (i == itemcnt)
                        {
                            lastid = tpcs[i].QueID;
                            qnum = i;
                            break;
                        }
                    }
                }
                else
                {
                    lastid = tpcs.Max(x => x.QueID);
                }

            }

            return lastid;
        }

        [HttpPost]
        public async Task<ActionResult> QuizInfo(QuizInfoViewModels models)
        {
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            string strguid = string.Empty;
            Guid nguid;
            try
            {
                totalMarks = 0;
                expletive = 0;

                TempData["posQue"] = null;
                TempData["negQue"] = null;
                TempData["Marks"] = "0";
                TempData["AnsIncorrect"] = null;
                TempData["AnsCorrect"] = null;
                TempData["AnsExplanation"] = null;
                TempData["StartTime"] = null;
                TempData["EndTime"] = null;
                Session["QAnswered"] = "false";
                Session["Rem_Time"] = null;
                TempData["Expletive"] = null;

                Session["blsubmitflag"] = null;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    QuizInfoViewModels model = Session["QZINFOMODEL"] as QuizInfoViewModels;

                    ClientQuiz nclquz = new ClientQuiz
                    {
                        PersonID = model.PersonID,
                        Token = Guid.NewGuid().ToString(),
                        TopicID = model.TopicID,
                        QuestionCount = model.QuestionsCount,
                        Duration = model.QuizDuration,
                        TotalScore = model.TotalMarks,
                        IsDeleted = false,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };
                    nqz.ClientQuizs.Add(nclquz);
                    await nqz.SaveChangesAsync();

                    //model.TopicID = nclquz.TopicID ?? 0;
                    Session["QUIZID"] = nclquz.ClientQuizID;
                    model.QuizID = nclquz.ClientQuizID;
                    //model.QuestionsCount = quecnt;

                    model.Token = nclquz.Token;
                    //nguid = Guid.Parse(nclquz.Token);

                    int cnt = 0;
                    //var questionlist = nqz.Questions.Where(x => x.IsDeleted == false && x.TopicID == model.TopicID && x.QuestionID >= model.StartSN && x.QuestionID <= model.StopSN)
                    //    .Select(x => new { x.QuestionID, x.QuestionMarks }).Take(model.QuestionsCount).ToList();
                    var questionlist = nqz.Questions.Where(x => x.IsDeleted == false && x.TopicID == model.TopicID && x.QuestionID >= model.StartSN && x.QuestionID < model.StopSN)
                        .Select(x => new { x.QuestionID, x.QuestionMarks }).ToList();

                    foreach (var item in questionlist)
                    {
                        cnt++;
                        ClientQuestion ncq = nqz.ClientQuestions.FirstOrDefault(x => x.QuestionID == item.QuestionID && x.Token == model.Token && x.PersonID == model.PersonID);
                        if (ncq == null)
                        {
                            ClientQuestion ncque = new ClientQuestion();
                            {
                                ncque.QuestionID = item.QuestionID;
                                ncque.QuestionNumber = cnt;
                                ncque.PersonID = model.PersonID;
                                //ncque.ClientQuizID = int.Parse(nclquz.ClientQuizID.ToString());
                                ncque.ClientQuizID = model.QuizID;
                                //ncque.Token = nclquz.Token;
                                ncque.Token = model.Token;
                                //ncque.PersonID = ncq.PersonID;
                                ncque.QuestionMarks = item.QuestionMarks;
                                ncque.AnswerID = 0;
                                ncque.Answered = false;
                                ncque.IsCorrect = false;
                                ncque.Created = DateTime.Now;
                                ncque.Updated = DateTime.Now;

                                nqz.ClientQuestions.Add(ncque);
                                await nqz.SaveChangesAsync();


                                //Session["ClientQuestionID"] = ncque.ClientQuestionID;
                                strguid = model.Token;
                            }

                        }
                        //else
                        //{
                        //    Session["QUIZID"] = model.QuizID;
                        //    strguid = nclquz.Token;
                        //}
                    }

                    //int MarkSum = nqz.ClientQuestions.Where(x => x.Token == model.Token && x.PersonID == model.PersonID).Select(x => x.QuestionMarks ?? 0).Sum();
                    ////int mksm = MarkSum.Sum();
                    //Session["MarkSum"] = MarkSum;
                    Session["TopicID"] = model.TopicID;
                    nguid = Guid.Parse(nclquz.Token);
                    Session["QuizStatus"] = "1";
                    Session["QZINFOMODEL"] = model;

                    //Save the Start Time
                    string starttime = DateTime.Now.ToShortTimeString().ToString();

                    ClientQuiz nclt = nqz.ClientQuizs.FirstOrDefault(x => x.ClientQuizID == nclquz.ClientQuizID);
                    nclt.StartTime = starttime;
                    //nclt.TotalScore = int.Parse(Session["TotalScore"].ToString());
                    nclt.Updated = DateTime.UtcNow;
                    await nqz.SaveChangesAsync();

                    //return View("QuizEval", new { token = model.Token });
                    return RedirectToAction("SkillPracticeQuiz", "Quiz", new { token = nguid });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult QuestionArchives()
        {

            try
            {
                var pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    IEnumerable<ClientQuizViewModel> QArchives = (from pers in nqz.Persons
                                                                  join clq in nqz.ClientQuizs on pers.PersonID equals clq.PersonID
                                                                  join res in nqz.ResourceTypes on clq.ResourceTypeID equals res.ResourceTypeID
                                                                  join tpc in nqz.Topics on clq.TopicID equals tpc.TopicID
                                                                  where pers.PersonID == pid
                                                                  orderby clq.Created descending
                                                                  select new ClientQuizViewModel
                                                                  {
                                                                      ClientQuizID = clq.ClientQuizID,
                                                                      PersonID = pers.PersonID,
                                                                      Token = clq.Token,
                                                                      ResourceTypeName = res.ResourceTypeName,
                                                                      TopicName = tpc.TopicName,
                                                                      Created = clq.Created
                                                                  }).ToList();

                    ViewBag.ArchiveList = QArchives;

                    Session["QArchives"] = QArchives;
                }



                return View();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult QuestionArchives(int id)
        {

            try
            {
                var pid = long.Parse(Session["PID"].ToString());
                ResultViewModel nres = new ResultViewModel();
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //Save the End Time

                    //string endtime = DateTime.Now.ToShortTimeString().ToString();
                    ClientQuiz nclt = nqz.ClientQuizs.FirstOrDefault(x => x.ClientQuizID == id);
                    var result = (from clqz in nqz.ClientQuizs
                                  join tpc in nqz.Topics on clqz.TopicID equals tpc.TopicID
                                  join crs in nqz.Courses on tpc.CourseID equals crs.CourseID
                                  //join fac in nqz.Faculties on crs.FacultyID equals fac.FacultyID
                                  where nclt.ClientQuizID == id
                                  select new ResultViewModel
                                  {
                                      Token = clqz.Token,
                                      Quiz = crs.CourseName,
                                      Topic = tpc.TopicName,
                                      StartTime = clqz.StartTime,
                                      Points = clqz.Score ?? 0,
                                      Qualification = Qualification(clqz.Score ?? 0)
                                  }).FirstOrDefault();

                    //nres.TopicID = nclt.TopicID ?? 0;
                    //nres.Quiz = Session["QzSubject"].ToString();
                    //nres.StartTime = nclt.StartTime;
                    //nres.EndTime = nclt.EndTime;
                    //nres.Percentage = nclt.Percentage ?? 0;
                    //nres.Points = nclt.Score ?? 0;
                    //nres.Qualification = Qualification(nclt.Score ?? 0);
                    //nres.Token = nclt.Token;
                    //nres.TotalPoints = nclt.TotalScore ?? 0;
                    //nres.Topic = Session["QzTopic"].ToString();
                    Session["QZResult"] = result;
                }




                return RedirectToAction("QuizResultX");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        public ActionResult QuizResultArchive(int id)
        {
            //ResultViewModel nres = Session["QZResult"] as ResultViewModel;

            //return View(nres);

            try
            {
                //var pid = long.Parse(Session["PID"].ToString());
                //ResultViewModel nres = new ResultViewModel();
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //Save the End Time

                    //string endtime = DateTime.Now.ToShortTimeString().ToString();
                    //ClientQuiz nclt = nqz.ClientQuizs.FirstOrDefault(x => x.ClientQuizID == id);
                    var result = (from clqz in nqz.ClientQuizs
                                  join tpc in nqz.Topics on clqz.TopicID equals tpc.TopicID
                                  join crs in nqz.Courses on tpc.CourseID equals crs.CourseID
                                  where clqz.ClientQuizID == id
                                  select new ResultViewModel
                                  {
                                      Token = clqz.Token,
                                      Quiz = crs.CourseName,
                                      Topic = tpc.TopicName,
                                      StartTime = clqz.StartTime,
                                      Points = clqz.Score ?? 0
                                  }).FirstOrDefault();

                    result.Qualification = Qualification(result.Scores);

                    //,
                    //                                    Qualification = Qualification(clqz.Score ?? 0)
                    //nres.TopicID = nclt.TopicID ?? 0;
                    //nres.Quiz = Session["QzSubject"].ToString();
                    //nres.StartTime = nclt.StartTime;
                    //nres.EndTime = nclt.EndTime;
                    //nres.Percentage = nclt.Percentage ?? 0;
                    //nres.Points = nclt.Score ?? 0;
                    //nres.Qualification = Qualification(nclt.Score ?? 0);
                    //nres.Token = nclt.Token;
                    //nres.TotalPoints = nclt.TotalScore ?? 0;
                    //nres.Topic = Session["QzTopic"].ToString();
                    //Session["QZResult"] = result;
                    ViewBag.Archives = "1";
                    return View("QuizResult", result);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        private string Qualification(double score)
        {
            string sQualification = string.Empty;
            try
            {
                if (score >= 0 && score <= 29)
                {
                    sQualification = "FAIL";
                }
                else if (score >= 30 && score <= 49)
                {
                    sQualification = "FAIR";
                }
                else if (score >= 50 && score <= 59)
                {
                    sQualification = "GOOD";
                }
                else if (score >= 60 && score <= 69)
                {
                    sQualification = "VERY GOOD";
                }
                else if (score >= 70 && score <= 89)
                {
                    sQualification = "EXCELLENT";
                }
                else if (score >= 90 && score <= 100)
                {
                    sQualification = "DISTINCTION";
                }
                return sQualification;
            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return null;
            }
        }

        public ActionResult Test()
        {
            try
            {

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult FreeTrial()
        {
            //bool ret = false;
            //long? currSubID = 0;
            //int? prcid = 0;
            //int? clsid = 0;

            //Session.Clear();

            try
            {

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var _model = nqz.Faculties.Where(x => x.SchoolID == 1 && x.IsDeleted == false).Select(x => new FreeTrialViewModels()
                    {
                        FacultyID = x.FacultyID,
                        FacultyName = x.FacultyName + " : " + x.Description,
                        FreeCourses = nqz.Courses.Where(y => y.FacultyID == x.FacultyID && y.IsDeleted == false).Select(y => new GradeCourses()
                        {
                            CourseID = y.CourseID,
                            CourseName = y.CourseName
                        }).Take(2).ToList()
                    }).Take(12).ToList();

                    Session["grdcnt"] = _model.Count();

                    return View(_model);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult FreeTrials(int id)
        {
            bool ret = false;
            //try
            //{
            //long? currSubID = 0;
            //int? prcid = 0;
            //int? clsid = 0;
            Session["CorrectAns"] = null;
            Session["InCorrectAns"] = null;
            Session["Ans2"] = null;
            Session["Ans3"] = null;
            Session["Ans4"] = null;
            try
            {
                FreeQuizInfoViewModels models = new FreeQuizInfoViewModels();
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Session["FreeTrialCourseID"] = id;
                    Cours cours = nqz.Courses.FirstOrDefault(x => x.CourseID == id);
                    models.QuestionsCount = 20;
                    models.QuizDuration = 10;
                    models.QuizName = cours.Faculty.FacultyName + " " + cours.CourseName;
                    models.SubjectID = id;
                    Session["FQuizInfo"] = models;

                    Session["QuizName"] = models.QuizName;

                    return RedirectToAction("FQuizInfo");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and try again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and try again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View();
            }
        }

        //public ActionResult FreeBees()
        //{
        //    try
        //    {

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
        //        return View("Error");
        //    }
        //}

        [HttpGet]
        public ActionResult FQuizInfo()
        {
            Session["QZResult"] = null;
            Session["blsubmitflag"] = null;

            Session["Ans1"] = null;
            Session["Ans2"] = null;
            Session["Ans3"] = null;
            Session["Ans4"] = null;

            try
            {
                totalMarks = 0;
                varCorrectAnswer = "";
                varInCorrectAnswer = "";
                varAnswerExplanation = "";

                FreeQuizInfoViewModels models = Session["FQuizInfo"] as FreeQuizInfoViewModels;

                return View(models);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and try again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and try again";
                }
                else
                {
                    ViewBag.ErrMessage = ex.Message + " Trace = " + ex.StackTrace;
                }

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FQuizInfo(FreeQuizInfoViewModels model)
        {
            firsttime = false;
            totalMarks = 0;
            blSubmit = false;
            ansflag = false;
            varAnswerExplanation = "";

            if (model.Phone != null)
            {
                if (model.Phone.Length < 9)
                {
                    ViewBag.Err = "You must enter a valid Phone Number.";
                    return View(model);
                }
            }

            string strguid = string.Empty;
            Guid nguid;
            try
            {
                //Reinitialize all tracking variables
                totalMarks = 0;
                expletive = 0;
                POSQuestions = 0;
                NEGQuestions = 0;
                AttemptedQuestions = 0;
                Scored = 0;

                TempData["posQue"] = null;
                TempData["negQue"] = null;
                TempData["Marks"] = "0";
                TempData["AnsIncorrect"] = null;
                TempData["AnsCorrect"] = null;
                TempData["AnsExplanation"] = null;
                TempData["StartTime"] = null;
                TempData["EndTime"] = null;
                Session["QAnswered"] = "false";
                Session["Rem_Time"] = null;
                TempData["Expletive"] = null;

                TempData["skipQueText"] = null;

                Session["blsubmitflag"] = null;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    long FreeTrialID = 0;

                    FreeTrial freeTrial = nqz.FreeTrials.FirstOrDefault(x => x.Email == model.Email);
                    if (freeTrial == null)
                    {
                        FreeTrial freeTrials = new FreeTrial()
                        {
                            Email = model.Email,
                            Phone = model.Phone,
                            Created = DateTime.UtcNow

                        };
                        nqz.FreeTrials.Add(freeTrials);
                        await nqz.SaveChangesAsync();

                        FreeTrialID = freeTrials.FreeTrialID;
                    }
                    else
                    {
                        FreeTrialID = freeTrial.FreeTrialID;
                    }

                    Session["FreeTrialID"] = FreeTrialID;

                    FreeTrialQuiz freeTrialQuiz = new FreeTrialQuiz()
                    {
                        Token = Guid.NewGuid().ToString(),
                        FreeTrialID = FreeTrialID,
                        SubjectID = model.SubjectID,
                        QuizName = model.QuizName,
                        QueCount = model.QuestionsCount,
                        QuizDuration = model.QuizDuration,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };
                    nqz.FreeTrialQuizs.Add(freeTrialQuiz);
                    await nqz.SaveChangesAsync();

                    model.FreeTrialQuizID = freeTrialQuiz.FreeTrialQuizID;
                    Session["FreeTrialQuizID"] = freeTrialQuiz.FreeTrialQuizID;
                    nguid = Guid.Parse(freeTrialQuiz.Token);

                    Session["CourseID"] = model.SubjectID;
                    //nguid = Guid.Parse(freeTrialQuiz.Token);
                    Session["QuizStatus"] = "1";
                    Session["QZINFOMODEL"] = model;
                }
                Session["Score"] = 0;
                Session["Duration"] = model.QuizDuration;
                Session["QueCount"] = model.QuestionsCount;
                Session["QuizType"] = "FreeBees";
                Session["QTOKEN"] = nguid;
                return RedirectToAction("FreeBees", "Quiz", new { token = nguid });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = "The system has encountered an unexpected error. Please, check your coonectivity and try again. ";
                }

                return View("Error");
            }
        }

        [HttpGet]

        //public async Task<ActionResult> FreeBees(Guid token, string bgn)
        public async Task<ActionResult> FreeBees(Guid token, int? qno)
        {
            bool blchkmodel = false;

            Session["QuizExpired"] = null;
            Session["QAnswered"] = "false";
            Session["skipQueText"] = null;
            Session["skipQue"] = null;
            Session["QueEnd"] = null;
            blsubmitflag = false;
            Session["Submitted"] = null;

            Session["FIGSlot1"] = "0";
            Session["FIGSlot2"] = "0";

            if (firsttime == false)
            {
                Session["Counter"] = "0";
                firsttime = true;
            }

            try
            {
                if (Session.Count > 0)
                {
                    //if (bgn != null)
                    //{
                    //    string Decqno = AppResourceController.Decrypt(bgn.ToString());

                    //    qno = int.Parse(Decqno.ToString());
                    //}

                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        if (Session["QuizStatus"].ToString() == "1")
                        {

                            if (ansflag == false)
                            {
                                Session["posQue"] = null;
                                Session["negQue"] = null;
                                Session["Marks"] = "0";
                                Session["PrevAnswerID"] = null;
                            }

                            FreeQuizInfoViewModels qzmodel = Session["QZINFOMODEL"] as FreeQuizInfoViewModels;

                            Session["quetime"] = qzmodel.QuizDuration;
                            Session["Rem_Time"] = qzmodel.QuizDuration;


                            Session["TOKEN"] = token;

                            long qzid = qzmodel.FreeTrialQuizID;
                            //Session["FreeTrialID"] = qzmodel.FreeTrialID;
                            int topid = qzmodel.SubjectID;
                            if (token == null)
                            {
                                Session["ErrMessage"] = "You have an invalid token. Please, try again.";
                                return RedirectToAction("Error");
                            }
                            int qcnt = qzmodel.QuestionsCount;
                            if (qno.GetValueOrDefault() < 1)
                            {
                                qno = 1;
                            }

                            if (qno.GetValueOrDefault() >= qcnt)
                            {
                                Session["QueEnd"] = "1";
                                if (qno.GetValueOrDefault() > qcnt)
                                {
                                    qno = qcnt;
                                }
                            }

                            if (qzmodel.FreeTrialID == 0)
                            {
                                qzmodel.FreeTrialID = long.Parse(Session["FreeTrialID"].ToString());
                            }
                            var pid = qzmodel.FreeTrialID;

                            var subjectid = int.Parse(Session["CourseID"].ToString());
                            var ntoken = Session["TOKEN"].ToString();
                            if (qno > 0)
                            {

                                QuestionXModels _model = new QuestionXModels();

                                //if (!blSubmit)
                                //{
                                _model = (from que in nqz.Questions
                                          join ftque in nqz.FreeTrialQuestions on que.QuestionID equals ftque.QuestionID
                                          where ftque.CourseID == subjectid && ftque.QuestionNumber == qno
                                          select new QuestionXModels()
                                          {
                                              QuestionID = ftque.QuestionID ?? 0,
                                              QuestionTypeID = que.QuestionTypeID ?? 0,
                                              QuestionString = que.QuestionString,
                                              QuestionNumber = ftque.QuestionNumber ?? 0,
                                              QuestionMarks = que.QuestionMarks ?? 0,
                                              AnswerExplanation = que.AnswerExplanation,
                                              Number = que.Number,
                                              Token = ntoken,
                                              Instruction = que.Instruction,
                                              FIGSlot = que.FIGSlot ?? 0,
                                              Answer = que.Answer,
                                              ImagePath = que.ImagePath,
                                              Options = que.Answers.Where(y => y.IsDeleted == false).Select(y => new AnswerXModels()
                                              {
                                                  AnswerID = y.AnswerID,
                                                  OptionLetter = y.OptionLetter,
                                                  AnswerString = y.AnswerString,
                                                  IsCorrect = y.IsCorrect ?? false,
                                                  Token = token.ToString(),
                                                  QuestionNumber = qno ?? 0
                                              }).ToList()
                                          }).FirstOrDefault();
                                //}

                                if (_model.ImagePath == null || _model.ImagePath.ToLower() == "nil")
                                {
                                    Session["img"] = "0";
                                }
                                else
                                {
                                    Session["img"] = "1";
                                }

                                if (_model.FIGSlot > 0)
                                {
                                    if (_model.FIGSlot == 1)
                                    {
                                        Session["FIGSlot1"] = "1";
                                    }
                                    else if (_model.FIGSlot == 2)
                                    {
                                        Session["FIGSlot1"] = "1";
                                        Session["FIGSlot2"] = "1";
                                    }
                                }


                                _model.SMarks = totalMarks ?? 0;

                                if (blSubmit == true)
                                {
                                    _model.AnswerID = AnsID;
                                    if (_model.FIGSlot > 0)
                                    {
                                        _model.FIGAnswer1 = Session["Ans1"].ToString();
                                    }

                                    if (_model.FIGSlot > 1)
                                    {
                                        _model.FIGAnswer2 = Session["Ans2"].ToString();
                                    }
                                    if (_model.FIGSlot > 2)
                                    {
                                        _model.FIGAnswer2 = Session["Ans2"].ToString();
                                        _model.FIGAnswer3 = Session["Ans3"].ToString();
                                    }
                                    if (_model.FIGSlot > 3)
                                    {
                                        _model.FIGAnswer2 = Session["Ans2"].ToString();
                                        _model.FIGAnswer3 = Session["Ans3"].ToString();
                                        _model.FIGAnswer4 = Session["Ans4"].ToString();
                                    }

                                    _model.SExplitive = varExplitive;
                                    _model.SPosQue = posQue;
                                    _model.SNegQue = negQue;
                                    if (negQue > 0)
                                    {
                                        _model.SCorrectAnswer = Session["CorrectAns"].ToString(); // varCorrectAnswer;
                                        _model.SInCorrectAnswer = Session["InCorrectAns"].ToString();
                                        _model.SAnswerExplanation = _model.AnswerExplanation; //varAnswerExplanation;

                                    }

                                    if (qno == qcnt)
                                    {
                                        _model.QLine1 = "Click Next >>|";
                                    }
                                    else
                                    {
                                        _model.QLine1 = "Click Next >>";
                                    }

                                }
                                else
                                {
                                    _model.QLine1 = "Click Submit";
                                }

                                Session["queid"] = _model.QuestionID;
                                Session["quemarks"] = _model.QuestionMarks;
                                Session["quenum"] = _model.QuestionNumber;
                                Session["answer"] = _model.Answer;
                                Session["explanation"] = _model.AnswerExplanation;

                                Session["QuePosition"] = qno + " of " + qcnt;
                                Session["QUESANS"] = _model;
                                return View(_model);

                            }
                            else
                            {
                                Session["QuizExpired"] = "1";
                                return View();
                            }
                        }
                        else
                        {
                            Session["skipQueText"] = "You must click on Submit to get scored and continue.";
                            return View();
                        }
                    }
                }
                else
                {
                    ViewBag.ErrMessage = "An unexpected error has occurred and you must log in again.";
                    return View("Error");
                }

            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("command definition"))
                //{
                //    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                //}
                //else if (ex.Message.Contains("The underlying provider failed on Open"))
                //{
                //    ViewBag.ErrMessage = "Connection Failure. Please, check your network connectivity and try again ";
                //}
                //else if (ex.Message.Contains("An error occurred while updating the entries"))
                //{
                //    ViewBag.ErrMessage = "Connection Failure. Please, check your network connectivity and try again";
                //}
                //else if (ex.Message.Contains("Object reference not set to an instance of an object."))
                //{
                //    ViewBag.ErrMessage = "Connection Failure. Your Current has expired and will need restart the application. In addition please, you may check your network connectivity and try again";
                //}
                //else
                //{
                //    ViewBag.ErrMessage = "The system has encountered an unexpected error. Please, check your coonectivity and try again. ";
                //}
                ViewBag.ErrMessage = ex.StackTrace;
                return View("Error");
            }

        }



        [HttpPost]
        public JsonResult CountUpTimer(int TCounter)
        {
            Session["Counter"] = TCounter;
            bool result = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> FreePostAnswers(QuestionXModels model)
        {
            try
            {
                long psnID = long.Parse(Session["FreeTrialID"].ToString()); //long.Parse(Session["PID"].ToString());
                //model = Session["QUESANS"] as QuestionXModels;

                int intansid = 0;
                int quenum = 0;
                bool? ansStatus = false;

                //model.SMarks = 0;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var nclqz = nqz.FreeTrialQuizs.FirstOrDefault(x => x.Token == model.Token);

                    //if (model.Token == Session["QTOKEN"].ToString())
                    //{
                    //    ViewBag.ErrMessage = "The Token is Invalid ";
                    //    return View("Error");
                    //}


                    var rdoAnswer = Request.Form["rdoAnswer"];
                    var hdfDirection = Request.Form["Direction"];
                    if (strAnsSel == "false")
                    {
                        strAnsSel = Request.Form["AnswerSelection"];
                    }
                    if (model.QuestionNumber == 0)
                    {
                        model.QuestionNumber = int.Parse(Session["quenum"].ToString());
                    }
                    quenum = model.QuestionNumber;
                    if (model != null)
                    {
                        if (model.ImagePath != null)
                        {
                            Session["img"] = "1";
                        }
                        else
                        {
                            Session["img"] = "0";
                        }
                    }
                    if (model.QuestionMarks == 0)
                    {
                        model.QuestionMarks = int.Parse(Session["quemarks"].ToString());
                    }

                    if (hdfDirection == "forward")
                    {
                        if (ansflag == false)
                        {
                            if (rdoAnswer != null)
                            {



                                intansid = int.Parse(rdoAnswer.ToString());

                                //Persist Answer ID
                                AnsID = intansid;

                                var subjectid = int.Parse(Session["CourseID"].ToString());
                                intSubmit = 1;
                                //var nclqz1 = (from crs in nqz.Courses
                                //              join fqz in nqz.FreeTrialQuizs on crs.CourseID equals fqz.SubjectID
                                //              join tpc in nqz.Topics on fqz.SubjectID equals tpc.CourseID
                                //              join que in nqz.Questions on tpc.TopicID equals que.TopicID
                                //              where fqz.SubjectID == subjectid && que.IsFree == true && que.QuestionNumber == quenum
                                //              select new
                                //              {
                                //                  que.QuestionMarks,
                                //                  que.QuestionID,
                                //              }).FirstOrDefault();
                                model.QuestionID = int.Parse(Session["queid"].ToString());
                                var queans = nqz.Answers.FirstOrDefault(x => x.QuestionID == model.QuestionID && x.IsCorrect == true);

                                //if (nclqz1 != null)
                                //{
                                //if (nclqz1.Answered == false)
                                //{
                                if (intansid != queans.AnswerID)
                                {

                                    Answer nans = nqz.Answers.FirstOrDefault(x => x.AnswerID == intansid);
                                    if (nans != null)
                                    {
                                        ansStatus = nans.IsCorrect;
                                        varCorrectAnswer = nans.OptionLetter + " --> " + nans.AnswerString;

                                    }


                                    if (ansStatus == true)
                                    {
                                        //totalMarks += model.QuestionMarks;
                                        //posQue++;

                                        //int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                        //if (expletive >= maxEnumValue)
                                        //{
                                        //    expletive = 0;
                                        //}

                                        //var expltv = (Expletives)expletive;
                                        //Session["Expletive"] = expltv + "!";
                                        //expletive++;

                                        //Session["Marks"] = totalMarks;
                                        //Session["posQue"] = "1";

                                        totalMarks += model.QuestionMarks;
                                        posQue++;
                                        POSQuestions++;
                                        int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                        if (expletive >= maxEnumValue)
                                        {
                                            expletive = 0;
                                        }

                                        var expltv = (Expletives)expletive;
                                        varExplitive = expltv + "!";
                                        expletive++;
                                        model.SMarks = totalMarks ?? 0;
                                        posQue = 1;
                                        //model.SPosQue = 1;
                                        negQue = 0;
                                        //model.SPosQue = 1;

                                        ViewBag.SuccessAnswer = "true";
                                        bldisplayinterver = true;
                                    }
                                    else
                                    {
                                        Session["InCorrectAns"] = varCorrectAnswer;

                                        Answer nans1 = nqz.Answers.FirstOrDefault(x => x.QuestionID == model.QuestionID && x.IsCorrect == true);
                                        if (nans1 != null)
                                        {
                                            varCorrectAnswer = nans1.OptionLetter + " --> " + nans1.AnswerString;
                                            Session["CorrectAns"] = nans1.OptionLetter + " --> " + nans1.AnswerString;
                                        }
                                        //varAnswerExplanation = nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID).AnswerExplanation;

                                        //totalMarks -= 2;
                                        //Session["Marks"] = totalMarks;
                                        //negQue--;
                                        //Session["Expletive"] = "Oooch!!!";
                                        //Session["negQue"] = "1";
                                        //expletive = 0;
                                        //varInCorrectAnswer = model.FIGAnswer1;

                                        //varCorrectAnswer = model.Answer;

                                        //varAnswerExplanation = model.AnswerExplanation; //Session["explanation"].ToString();

                                        totalMarks -= 2;
                                        NEGQuestions++;
                                        //Session["Marks"] = totalMarks;
                                        negQue = 1;
                                        posQue = 0;
                                        varExplitive = "Ouch!!!";
                                        //Session["negQue"] = "1";
                                        expletive = 0;
                                        ViewBag.SuccessAnswer = "false";

                                    }
                                }
                                else
                                {
                                    totalMarks += model.QuestionMarks;
                                    posQue++;
                                    POSQuestions++;
                                    int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                    if (expletive >= maxEnumValue)
                                    {
                                        expletive = 0;
                                    }

                                    var expltv = (Expletives)expletive;
                                    varExplitive = expltv + "!";
                                    expletive++;
                                    model.SMarks = totalMarks ?? 0;
                                    posQue = 1;
                                    //model.SPosQue = 1;
                                    negQue = 0;
                                    //model.SPosQue = 1;

                                    ViewBag.SuccessAnswer = "true";
                                    bldisplayinterver = true;
                                }

                                //}

                                //ClientQuestion clquestion = nqz.ClientQuestions.FirstOrDefault(x => x.ClientQuestionID == nclqz1.ClientQuestionID);
                                //clquestion.AnswerID = intansid;
                                //clquestion.IsCorrect = ansStatus;
                                //clquestion.Answered = true;
                                //clquestion.Updated = DateTime.UtcNow;
                                //await nqz.SaveChangesAsync();
                                blSubmit = true;
                                Session["skipQue"] = null;
                                ansflag = true;
                                Scored = quenum;
                            }
                            else
                            {
                                if (model.FIGSlot > 0)
                                {
                                    var subjectid = int.Parse(Session["CourseID"].ToString());

                                    if (model.Answer == null)
                                    {
                                        model.Answer = Session["answer"].ToString();
                                        model.AnswerExplanation = Session["explanation"].ToString();
                                    }

                                    if (model.FIGSlot > 1)
                                    {
                                        string[] arrfigans = model.Answer.ToString().Split(',');
                                        if (arrfigans.Length == 2)
                                        {
                                            if (arrfigans[0].ToLower().Trim() == model.FIGAnswer1.ToLower().Trim() && arrfigans[1].ToLower().Trim() == model.FIGAnswer2.ToLower().Trim())
                                            {

                                                totalMarks += model.QuestionMarks;
                                                POSQuestions++;
                                                posQue = 1;
                                                negQue = 0;
                                                int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                                if (expletive >= maxEnumValue)
                                                {
                                                    expletive = 0;
                                                }

                                                var expltv = (Expletives)expletive;
                                                varExplitive = expltv + "!";
                                                expletive++;
                                                model.SMarks = totalMarks ?? 0;
                                                //posQue = 1;
                                                //model.SPosQue = 1;
                                                //totalMarks += model.QuestionMarks;
                                                //posQue++;

                                                //int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                                //if (expletive >= maxEnumValue)
                                                //{
                                                //    expletive = 0;
                                                //}

                                                //var expltv = (Expletives)expletive;
                                                //Session["Expletive"] = expltv + "!";
                                                //expletive++;

                                                //model.SMarks = totalMarks ?? 0;
                                                //Session["Marks"] = totalMarks;
                                                //Session["posQue"] = "1";
                                                ViewBag.SuccessAnswer = "true";
                                                bldisplayinterver = true;
                                            }
                                            else
                                            {
                                                //Session["AnsCorrect"] = model.Answer;

                                                //Session["AnsExplanation"] = Session["explanation"].ToString(); //nqz.Questions.FirstOrDefault(x => x.QuestionID == model.QuestionID).AnswerExplanation;

                                                //totalMarks -= 2;
                                                //model.SMarks = totalMarks ?? 0;
                                                //Session["Marks"] = totalMarks;
                                                //negQue--;
                                                //Session["Expletive"] = "Oooch!!!";
                                                //Session["negQue"] = "1";
                                                //expletive = 0;
                                                Session["Ans1"] = model.FIGAnswer1.ToString();
                                                varInCorrectAnswer = model.FIGAnswer1;
                                                Session["InCorrectAns"] = model.FIGAnswer1.ToString() + " " + model.FIGAnswer1.ToString();

                                                varCorrectAnswer = model.Answer;
                                                Session["CorrectAns"] = model.Answer;

                                                varAnswerExplanation = model.AnswerExplanation; //Session["explanation"].ToString();

                                                totalMarks -= 2;
                                                NEGQuestions++;
                                                //Session["Marks"] = totalMarks;
                                                negQue = 1;
                                                posQue = 0;
                                                varExplitive = "Ouch!!!";
                                                //Session["negQue"] = "1";
                                                expletive = 0;
                                                ViewBag.SuccessAnswer = "false";
                                            }

                                            Session["Ans2"] = model.FIGAnswer2;


                                        }
                                        if (arrfigans.Length == 3)
                                        {
                                            if (arrfigans[0].ToLower() == model.FIGAnswer1.ToLower() && arrfigans[1].ToLower() == model.FIGAnswer2.ToLower() && arrfigans[2].ToLower() == model.FIGAnswer3.ToLower())
                                            {

                                                Session["Ans1"] = model.FIGAnswer1;
                                                Session["InCorrectAns"] = model.FIGAnswer1;
                                                Session["Ans2"] = model.FIGAnswer2;
                                                Session["Ans3"] = model.FIGAnswer3;

                                                varCorrectAnswer = model.Answer;
                                                Session["CorrectAns"] = model.Answer;

                                                //Session["Marks"] = totalMarks;
                                                //Session["posQue"] = "1";

                                                totalMarks += model.QuestionMarks;
                                                //posQue++;
                                                POSQuestions++;
                                                int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                                if (expletive >= maxEnumValue)
                                                {
                                                    expletive = 0;
                                                }

                                                var expltv = (Expletives)expletive;
                                                varExplitive = expltv + "!";
                                                expletive++;
                                                model.SMarks = totalMarks ?? 0;
                                                posQue = 1;
                                                negQue = 0;
                                                //model.SPosQue = 1;

                                                ViewBag.SuccessAnswer = "true";
                                                bldisplayinterver = true;
                                            }
                                            else
                                            {

                                                //Session["AnsCorrect"] = model.Answer;

                                                //Session["AnsExplanation"] = Session["explanation"].ToString();

                                                //totalMarks -= 2;
                                                //Session["Marks"] = totalMarks;
                                                //negQue--;
                                                //Session["Expletive"] = "Oooch!!!";
                                                //Session["negQue"] = "1";
                                                //expletive = 0;
                                                varInCorrectAnswer = model.FIGAnswer1;

                                                Session["Ans1"] = model.FIGAnswer1; //model.Answer;

                                                varAnswerExplanation = model.AnswerExplanation; //Session["explanation"].ToString();
                                                NEGQuestions++;
                                                totalMarks -= 2;
                                                //Session["Marks"] = totalMarks;
                                                negQue = 1;
                                                posQue = 0;
                                                varExplitive = "Ouch!!!";
                                                //Session["negQue"] = "1";
                                                expletive = 0;
                                                ViewBag.SuccessAnswer = "false";
                                            }
                                            Session["Ans2"] = model.FIGAnswer2;
                                            Session["Ans3"] = model.FIGAnswer3;
                                        }
                                        if (arrfigans.Length == 4)
                                        {
                                            if (arrfigans[0].ToLower().Trim() == model.FIGAnswer1.ToLower().Trim() && arrfigans[1].ToLower().Trim() == model.FIGAnswer2.ToLower().Trim() && arrfigans[2].ToLower().Trim() == model.FIGAnswer3.ToLower().Trim() && arrfigans[3].ToLower().Trim() == model.FIGAnswer4.ToLower().Trim())
                                            {
                                                totalMarks += model.QuestionMarks;
                                                POSQuestions++;

                                                Session["Ans1"] = model.FIGAnswer1;
                                                //posQue++;
                                                posQue = 1;
                                                negQue = 0;
                                                int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                                if (expletive >= maxEnumValue)
                                                {
                                                    expletive = 0;
                                                }

                                                var expltv = (Expletives)expletive;
                                                Session["Expletive"] = expltv + "!";
                                                expletive++;

                                                //Session["Marks"] = totalMarks;
                                                //Session["posQue"] = "1";
                                                ViewBag.SuccessAnswer = "true";
                                                bldisplayinterver = true;
                                            }
                                            else
                                            {



                                                varInCorrectAnswer = model.FIGAnswer1;

                                                varCorrectAnswer = model.Answer;

                                                varAnswerExplanation = model.AnswerExplanation; //Session["explanation"].ToString();

                                                totalMarks -= 2;
                                                NEGQuestions++;
                                                //Session["Marks"] = totalMarks;
                                                negQue = 1;
                                                posQue = 0;
                                                varExplitive = "Ouch!!!";
                                                //Session["negQue"] = "1";
                                                expletive = 0;

                                                ViewBag.SuccessAnswer = "false";
                                            }

                                            Session["Ans2"] = model.FIGAnswer2.ToString();
                                            Session["Ans3"] = model.FIGAnswer3.ToString();
                                            Session["Ans4"] = model.FIGAnswer3.ToString();
                                        }

                                        blSubmit = true;
                                        Session["skipQue"] = null;
                                        ansflag = true;
                                        Scored = quenum;
                                    }
                                    else
                                    {
                                        if (!blSubmit)
                                        {
                                            if (model.FIGAnswer1 != null)
                                            {
                                                if (model.Answer.ToLower().Trim() == model.FIGAnswer1.ToLower().Trim())
                                                {
                                                    POSQuestions++;
                                                    totalMarks += model.QuestionMarks;
                                                    posQue = 1;
                                                    negQue = 0;
                                                    int maxEnumValue = Convert.ToInt32(Enum.GetValues(typeof(Expletives)).Cast<Expletives>().Last());
                                                    if (expletive >= maxEnumValue)
                                                    {
                                                        expletive = 0;
                                                    }

                                                    var expltv = (Expletives)expletive;
                                                    varExplitive = expltv + "!";
                                                    expletive++;
                                                    model.SMarks = totalMarks ?? 0;
                                                    //posQue = 1;
                                                    //model.SPosQue = 1;
                                                    Session["Ans1"] = model.FIGAnswer1.ToString();
                                                    ViewBag.SuccessAnswer = "true";
                                                    bldisplayinterver = true;
                                                }
                                                else
                                                {
                                                    //Session["AnsCorrect"] = model.Answer;

                                                    //Session["AnsExplanation"] = Session["explanation"].ToString();
                                                    varInCorrectAnswer = model.FIGAnswer1;
                                                    Session["answer"] = varInCorrectAnswer;
                                                    varCorrectAnswer = model.Answer;

                                                    Session["InCorrectAns"] = model.FIGAnswer1.ToString();

                                                    Session["CorrectAns"] = model.Answer;

                                                    //varAnswerExplanation = model.AnswerExplanation; //Session["explanation"].ToString();
                                                    NEGQuestions++;
                                                    totalMarks -= 2;
                                                    //Session["Marks"] = totalMarks;
                                                    negQue = 1;
                                                    posQue = 0;
                                                    varExplitive = "Ouch!!!";
                                                    //Session["negQue"] = "1";
                                                    expletive = 0;
                                                    ViewBag.SuccessAnswer = "false";
                                                }

                                                blSubmit = true;
                                                Session["skipQue"] = null;
                                                ansflag = true;
                                                Scored = quenum;
                                            }
                                            else
                                            {
                                                //Session["skipQue"] = "1";
                                                //Session["skipQueText"] = "You must enter an answer to Continue.";
                                                Session["skipQue"] = "1";
                                                Session["skipQueText"] = "You must enter an answer to Continue.";
                                                //Session["negQue"] = "1";
                                            }


                                        }
                                        else
                                        {

                                        }


                                    }
                                }
                                else
                                {
                                    //Session["Marks"] = totalMarks;
                                    Session["skipQue"] = "1";
                                    Session["skipQueText"] = "You must enter an answer to get a score.";
                                    //Session["skipQue"] = "1";
                                    //Session["skipQueText"] = "You must enter an answer to get a score.";
                                    //Session["negQue"] = "1";

                                    blSubmit = false;
                                }
                            }
                        }
                        else
                        {
                            Session["skipQue"] = "1";
                            Session["skipQueText"] = "You must click on Next >> to move to the next question.";
                        }
                    }
                    else
                    {
                        if (blSubmit == true)
                        {
                            posQue = 0;
                            negQue = 0;
                            blSubmit = false;
                            ansflag = false;
                            Scored = quenum;
                            AttemptedQuestions = quenum;
                            quenum++;
                        }
                        else
                        {
                            //if (rdoAnswer != null)
                            //{
                            //Session["Marks"] = totalMarks;
                            //Session["skipQue"] = "1";
                            Session["skipQueText"] = "You must click on Submit to get scored.";
                            //intSubmit = 0;

                            //}
                            //else
                            //{
                            //    if (model.FIGSlot > 0)
                            //    {
                            //        Session["skipQueText"] = "You must enter an answer to continue.";
                            //    }
                            //    else
                            //    {
                            //        Session["skipQueText"] = "You must select an Option to continue.";
                            //        Session["skipQueText"] = "You must select an Option to continue.";
                            //    }
                            //    Session["Marks"] = totalMarks;
                            //    Session["skipQue"] = "1";
                            //    Session["skipQue"] = "1";
                            //    intSubmit = 0;
                            //}
                        }

                    }
                }
                Session["QuePosition"] = quenum + " of " + Session["QueCount"].ToString();
                AttemptedQuestions = quenum;
                //string Encqno = AppResourceController.Encrypt(quenum.ToString());

                //return RedirectToAction("FreeBees", new
                //{
                //    @token = Session["TOKEN"].ToString(),
                //    @bgn = Encqno
                //});

                return RedirectToAction("FreeBees", new
                {
                    @token = Session["TOKEN"],
                    @qno = quenum
                });

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check your network connectivity and try again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check your network connectivity and try again";
                }
                else if (ex.Message.Contains("Object reference not set to an instance of an object."))
                {
                    ViewBag.ErrMessage = "Connection Failure. Your Current has expired and will need restart the application. In addition please, you may check your network connectivity and try again";
                }
                else
                {
                    ViewBag.ErrMessage = "The system has encountered an unexpected error. Please, check your coonectivity and try again. ";
                }

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FreeTrialMigration(QuestionXModels model)
        {
            try
            {
                //long psnID = long.Parse(Session["FreeTrialID"].ToString()); //long.Parse(Session["PID"].ToString());
                //model = Session["QUESANS"] as QuestionXModels;

                int intansid = 0;
                int quenum = 0;
                bool? ansStatus = false;

                model.SMarks = 0;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //where que.IsFree == true && tpc.TopicID == 482
                    var freemodel = (from que in nqz.Questions
                                     join tpc in nqz.Topics on que.TopicID equals tpc.TopicID
                                     join crs in nqz.Courses on tpc.CourseID equals crs.CourseID
                                     where que.IsFree == true && que.QuestionID >= 9716 && que.QuestionID <= 9735
                                     select new QuestionXModels()
                                     {
                                         QuestionID = que.QuestionID,
                                         CourseID = crs.CourseID,
                                         QuestionTypeID = que.QuestionTypeID ?? 0,
                                         QuestionString = que.QuestionString,
                                         QuestionNumber = que.QuestionNumber ?? 0,
                                         QuestionMarks = que.QuestionMarks ?? 0,
                                         AnswerExplanation = que.AnswerExplanation,
                                         Number = que.Number,
                                         TopicID = que.TopicID ?? 0,
                                     }).ToList();

                    foreach (var item in freemodel)
                    {
                        FreeTrialQuestion freeques = new FreeTrialQuestion();
                        freeques.QuestionID = item.QuestionID;
                        freeques.CourseID = item.CourseID;
                        freeques.QuestionNumber = item.QuestionNumber;
                        freeques.ImagePath = item.ImagePath;
                        freeques.TopicID = item.TopicID;
                        freeques.Created = DateTime.UtcNow;
                        freeques.Updated = DateTime.UtcNow;
                        nqz.FreeTrialQuestions.Add(freeques);
                        await nqz.SaveChangesAsync();
                    }


                }

                return RedirectToAction("Topics", "Quiz");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("command definition"))
                {
                    ViewBag.ErrMessage = "A connectivity error was encountered. Refresh Page to continue with the quiz. ";
                }
                else if (ex.Message.Contains("The underlying provider failed on Open"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again ";
                }
                else if (ex.Message.Contains("An error occurred while updating the entries"))
                {
                    ViewBag.ErrMessage = "Connection Failure. Please, check network connectivity and login again";
                }
                else
                {
                    ViewBag.ErrMessage = "The system has encountered an unexpected error. Please, check your coonectivity and try again. ";
                }

                return View();
            }
        }

    }
}

