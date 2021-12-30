using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using braingainspa.Models;
using CaptchaMvc.HtmlHelpers;
using Paystack;
using Paystack.Net;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

using braingainspa.Controllers;
using System.Web.UI;
using System.Configuration;
using Paystack.Net.SDK.Transactions;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;

namespace braingainspa.Controllers
{
    public class AccountController : Controller
    {
        static int stintCnt = 0;
        static long pid = 0;

        static string Email = string.Empty;
        static string FirstName = string.Empty;
        static string LastName = string.Empty;
        static bool blSubscribe = false;
        static string compComment = string.Empty;
        static string PayConfirmation = string.Empty;

        decimal Amount = 0;

        static int EditID = 0;

        static int TotalDownlines = 0;

        string enccode = string.Empty;
        string RefCode = string.Empty;

        private static string WebAPIURL = "http://localhost:58984/";

        //private readonly CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Signup()
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;



            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Signup(SignupViewModel model)
        {

            ViewBag.Err = null;
            ViewBag.Msg = null;
            ViewBag.ErrorMessage = null;

            //string EncPassword = AppResourceController.Encrypt("aud4321it");
            //string EncPassword1 = AppResourceController.Encrypt("adaudit44");
            try
            {
                //string strSMS = "Thank you for Registering with Braingainspa.com. However, to complete your Registration, Here is your Account Activation Code: 23467";
                //AppAsyncController.RunAsync(strSMS, model.Phone).Wait();

                //string murl = ConfigurationManager.AppSettings["DomainHeader"].ToString() + Request.ServerVariables["HTTP_HOST"] + "/Account/SignupResponse?key=";

                //AppResourceController nappRes1 = new AppResourceController();
                //nappRes1.SendRegistrationMail("cozarrs@yahoo.com", "Okoro" + ' ' + "Jane", "43er", "444", "556", Convert.ToString(5), murl);

                //AppResourceController.SendSMS("", model.Phone, strSMS);

                //return null;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {


                    if (model.ReferralCode != null)
                    {
                        if (model.ReferralCode.Length != 5)
                        {
                            ViewBag.Err = "Invalid Refferal Code! The Referral Code must be five digits. If not sure, leave it blank. You can set it later.";
                        }
                    }
                    else if (model.ReferralCode != null)
                    {
                        Person npers = nqz.Persons.FirstOrDefault(x => x.RefereeCode == model.ReferralCode);
                        if (npers == null)
                        {
                            ViewBag.Err = "The Referral Code could not be identified. If not sure, leave it blank. You can set it later.";
                        }
                    }
                    else if (model.EntityType == "1")
                    {
                        if (model.ParentCode == null)
                        {
                            ViewBag.Err = "You must specify a Parent Code could for a student.";
                        }
                    }
                    else if (model.ParentCode != null)
                    {
                        Person npers = nqz.Persons.FirstOrDefault(x => x.ParentCode == model.ParentCode);
                        if (npers == null)
                        {
                            ViewBag.Err = "The Parent Code could not be identified. If not sure, leave it blank. You can set it later.";
                        }
                    }
                    else if (model.Password != model.ConfirmPassword)
                    {
                        ViewBag.Err = "Password Mismatch! The Password must match the Confirm Password.";
                    }


                    User nuser1 = nqz.Users.FirstOrDefault(x => x.UserName == model.Email);
                    if (nuser1 != null)
                    {
                        ViewBag.Err = "Alert! The Email already exist. You can attempt a Signin.";
                    }

                    User nuser2 = nqz.Users.FirstOrDefault(x => x.PhoneNumber == model.Phone);
                    if (nuser2 != null)
                    {
                        ViewBag.Err = "Alert! The Phone Number already exist. You can attempt a Signin.";
                    }

                    if (ViewBag.Err == null)
                    {
                        if (ModelState.IsValid)
                        {
                            if (this.IsCaptchaValid("Validate your captcha"))
                            {

                                Session["RegModel"] = model;

                                //int EntityTypeID = 0;
                                if (model.EntityType == "2")
                                {
                                    GenerateParentCode();
                                    model.ParentCode = ViewBag.ParentCode;
                                    //EntityTypeID = 2;
                                }

                                //Get max downline
                                int? maxdownline = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).MaxFirstLevel;

                                //If Referral Code is null then assign default Referral Code - Company
                                string compRefereeCode = nqz.Persons.FirstOrDefault(x => x.IsCompany == true).RefereeCode;
                                if (model.ReferralCode == null)
                                {
                                    model.ReferralCode = compRefereeCode;
                                }
                                else
                                {
                                    //Get Referral's Max FirstLevel Downlines
                                    int? maxPersonDownline = nqz.Persons.FirstOrDefault(x => x.RefereeCode == model.ReferralCode).MaxFirstLevel;

                                    //Get the number of existing referrals. if max, then assign default Referral Code - Company
                                    int referalCnt = nqz.Persons.Where(x => x.ReferralCode == model.ReferralCode).Count();
                                    if (referalCnt >= maxPersonDownline)
                                    {
                                        model.ReferralCode = compRefereeCode;
                                    }
                                }

                                //Session["SRC"] = "Registration";
                                //Session["TransactionType"] = "Registration";
                                ////Session["REGDETAILS"] = model.FirstName + "|" + model.LastName + "|" + model.Phone + "|" + model.Email + "|" + model.Password + "|" + hdn.Value + "|" + ddlLevel.SelectedValue.ToString() + "|" + ddlSemester.SelectedValue.ToString();


                                GenerateReferralCode();
                                RefCode = ViewBag.ReferralCode;

                                model.ReferreeCode = RefCode;
                                model.MaxFirstLevel = maxdownline ?? 0;

                                Session["RegModel"] = model;


                                return RedirectToAction("Subscriptions", "Account");

                                //Person nperson = new Person()
                                //{
                                //    FirstName = model.FirstName,
                                //    LastName = model.LastName,
                                //    EntityTypeID = int.Parse(model.EntityType),
                                //    ReferralCode = model.ReferralCode,
                                //    RefereeCode = model.ReferreeCode,
                                //    ParentCode = model.ParentCode ?? "0",
                                //    MaxFirstLevel = model.MaxFirstLevel,
                                //    IsCompany = false,
                                //    Created = DateTime.Now,
                                //    Updated = DateTime.Now
                                //};
                                //nqz.Persons.Add(nperson);
                                //await nqz.SaveChangesAsync();

                                //string EncPassword = AppResourceController.Encrypt(model.Password);

                                //User nuser = new Models.User()
                                //{
                                //    PersonID = nperson.PersonID,
                                //    UserName = model.Email,
                                //    PhoneNumber = model.Phone,
                                //    RoleID = 4,
                                //    IsActive = false,
                                //    IsRegistered = false,
                                //    Password = EncPassword,
                                //    PhoneCode = ViewBag.ReferralCode,
                                //    ConfirmPhone = false,
                                //    ConfirmEmail = false,
                                //    Created = DateTime.Now,
                                //    Updated = DateTime.Now
                                //};
                                //nqz.Users.Add(nuser);
                                //await nqz.SaveChangesAsync();

                                //enccode = AppResourceController.Encrypt(model.Phone + "|" + RefCode);
                                //Session["PhoneDetails"] = enccode;

                                ////Save Contact Information
                                //Contact ncont = new Contact()
                                //{
                                //    PersonID = nperson.PersonID,
                                //    Phone = model.Phone,
                                //    Email = model.Email,
                                //    Created = DateTime.Now,
                                //    Updated = DateTime.Now

                                //};
                                //nqz.Contacts.Add(ncont);
                                //await nqz.SaveChangesAsync();

                                ////Setup Accounts for the individual
                                //Account account = new Account()
                                //{
                                //    PersonID = nperson.PersonID,
                                //    Purse = 0,
                                //    TempPurse = 0,
                                //    Withdrawals = 0,
                                //    Admin2 = 0,
                                //    IsCompany = false,
                                //    Created = DateTime.Now,
                                //    Updated = DateTime.Now
                                //};
                                //nqz.Accounts.Add(account);
                                //await nqz.SaveChangesAsync();

                                ////SEND SMS
                                ////string strSMS = "Thank you for Registering with Braingain.com. However, to complete your Registration, Here is your Account Activation (Phone) Code: " + ViewBag.ReferralCode;
                                ////AppAsyncController.RunAsync(strSMS, model.Phone).Wait();

                                ////SEND REGISTRATION MAIL
                                //string RR = ConfigurationManager.AppSettings["DomainHeader"].ToString() + Request.ServerVariables["HTTP_HOST"] + "/Account/SignupResponse?key=";

                                //AppResourceController nappRes = new AppResourceController();
                                //nappRes.SendRegistrationMail(model.Email, model.LastName + ' ' + model.FirstName, model.Password, ViewBag.ReferralCode, ViewBag.ParentCode, Convert.ToString(nuser.UserID), RR);

                                ////SEND SMS
                                //string strSMS = "Thank you for Registering with braingainspa.com. However, to complete your Registration, Here is your Account Activation (Phone) Code: " + ViewBag.ReferralCode;
                                //AppResourceController.SendSMS("", model.Phone, strSMS);
                                ////AppAsyncController.RunAsync("My Test Message", "08033667378").Wait();

                                ////ViewBag.Msg = "Record Saved Successfully";
                                ////ViewBag.Verified = "0";
                                //ViewBag.Msg = "1";


                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Invalid Image text; Try again";
                            }

                        }
                    }

                    if (ViewBag.Msg != null)
                    {
                        ViewBag.Msg = null;
                        Session["ConfirmPhone"] = false;
                        return RedirectToAction("SignupSuccess", "Account", new { key = enccode });
                    }
                    else
                    {
                        return View();
                    }
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
            finally
            {

            }
        }



        public ActionResult ResendPhoneCode(string key)
        {
            string phn = null, cd = null;
            ViewBag.Err = null;
            try
            {
                if (key == null)
                {
                    var pid = long.Parse(Session["PID"].ToString());
                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        //var person = (from pers in nqz.Persons
                        //              join usr in nqz.Users on pers.PersonID equals usr.PersonID
                        //              where pers.PersonID == pid
                        //              select new
                        //              {
                        //                  pers.RefereeCode,
                        //                  usr.PhoneNumber
                        //              }).FirstOrDefault();
                        User principal = nqz.Users.FirstOrDefault(x => x.PersonID == pid);
                        if (principal != null)
                        {
                            phn = "08033667378";//principal.PhoneNumber;
                            cd = principal.PhoneCode;
                        }
                    }
                }
                else
                {
                    string phndet = AppResourceController.Decrypt(key);

                    string[] phnarr = phndet.Split('|');
                    phn = phnarr[0];
                    cd = phnarr[1];
                }

                if (cd != null)
                {
                    string strSMS = "Thank you for Registering with braingainspa.com. However, to complete your Registration, Here is your Account Activation (Phone) Code: " + cd;
                    AppResourceController.SendSMS("", phn, strSMS);

                    //return RedirectToAction("ClientDashboard", "Quiz");
                    ViewBag.Msg = "The Verification Code has been resent to your registered phone number.";
                }
                else
                {
                    ViewBag.Err = "An error was encountered while processing the operation. Please, try again.";


                }
                return View("Verified");
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
            //if (ViewBag.VerifyPhone == null)
            //{
            //    ViewBag.VerifyPhone = Session["VerifyPhone"].ToString();
            //}

        }

        [HttpGet]
        public ActionResult SignupSuccess(string key)
        {
            try
            {
                ViewBag.VerifyPhone = "0";
                ViewBag.Verified = "0";
                ViewBag.Err = null;
                VerifyPhoneEmailViewModel nvmd = new VerifyPhoneEmailViewModel();
                nvmd.PhoneDetails = key;

                return View(nvmd);
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
        public async Task<ActionResult> SignupSuccess(VerifyPhoneEmailViewModel model)
        {

            ViewBag.Err = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        if (model.PhoneDetails != null)
                        {
                            string phndet = AppResourceController.Decrypt(model.PhoneDetails);

                            string[] phnarr = phndet.Split('|');
                            //string phn = phnarr[0];
                            string cd = phnarr[1];

                            User nuser = nqz.Users.FirstOrDefault(x => x.PhoneCode == cd);
                            if (nuser != null)
                            {
                                //nuser.IsActive = true;
                                nuser.ConfirmPhone = true;
                                nuser.Updated = DateTime.Now;
                                await nqz.SaveChangesAsync();

                                Session["ConfirmPhone"] = true;
                                ViewBag.VerifyPhone = "1";
                                Session["VerifyPhone"] = "1";
                                TempData["Verified"] = true;
                                //ViewBag.IsVerified = true;
                            }
                            else
                            {
                                ViewBag.VerifyPhone = "0";
                                TempData["Verified"] = false;
                                //Session["ConfirmPhone"] = false;
                                ViewBag.Err = "Invalid Code. An attempt to verify your Phone Number has Failed. The Code could not be verified. Please, try again.";
                            }
                        }
                        else
                        {
                            User nuser = nqz.Users.FirstOrDefault(x => x.PhoneCode == model.PhoneCode);
                            if (nuser != null)
                            {
                                //nuser.IsActive = true;
                                nuser.ConfirmPhone = true;
                                nuser.Updated = DateTime.Now;
                                await nqz.SaveChangesAsync();

                                Session["ConfirmPhone"] = true;
                                ViewBag.VerifyPhone = "1";
                                Session["VerifyPhone"] = "1";
                                TempData["Verified"] = true;
                                //ViewBag.IsVerified = true;
                            }
                            else
                            {
                                ViewBag.VerifyPhone = "0";
                                TempData["Verified"] = false;
                                //Session["ConfirmPhone"] = false;
                                ViewBag.Err = "Invalid Code. An attempt to verify your Phone Number has Failed. The Code could not be verified. Please, try again.";
                            }
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
            //if (ViewBag.VerifyPhone == null)
            //{
            //    ViewBag.VerifyPhone = Session["VerifyPhone"].ToString();
            //}

        }

        // GET: Account/Delete/5
        public async Task<ActionResult> SignupResponse(string key)
        {
            try
            {
                var uid = int.Parse(AppResourceController.Decrypt(key));
                //Session["sUserID"] = uid; // sign up user id
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    User nuser = nqz.Users.FirstOrDefault(x => x.UserID == uid);
                    if (nuser != null)
                    {
                        //nuser.IsActive = true;
                        nuser.ConfirmEmail = true;
                        nuser.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();

                        Session["ConfirmPhone"] = true;
                        ViewBag.VerifyPhone = "1";
                        Session["VerifyPhone"] = "1";
                        TempData["Verified"] = true;
                        //ViewBag.IsVerified = true;
                    }
                    else
                    {
                        ViewBag.VerifyEmail = "0";
                        TempData["Verified"] = false;
                        ViewBag.Err = "Invalid Code. An attempt to verify your Email has Failed. The Code could not be verified.";
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

        // GET: Account/Delete/5
        public ActionResult Signin()
        {
            SigninViewModel model = new SigninViewModel();
            if (Request.Cookies["UserLogin"] != null)
            {
                model.Email = Request.Cookies["UserLogin"].Values["Email"];
                string decPassword = AppResourceController.Decrypt(Request.Cookies["UserLogin"].Values["Password"]);
                model.Password = decPassword;
                string TOS = Request.Cookies["UserLogin"].Values["TOS"];
                if (TOS.ToString() == "True")
                {
                    model.TOS = true;
                }
                model.RememberMe = true;
            }
            Session["Subscription"] = null;
            Session["ImagePath"] = null;
            Session["VErr"] = null;
            Session["QAnswered"] = "false";
            Session["QImagePath"] = null;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Signin(SigninViewModel model)
        {
            Session["Subscription"] = null;
            Session["ImagePath"] = null;
            Session["VErr"] = "0";
            Session["QAnswered"] = "false";
            Session["QImagePath"] = null;
            Session["ExcelDoc"] = "0";
            //Session["Rem_Time"] = "0";
            string EncPassword = string.Empty;
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (ModelState.IsValid)
                    {
                        if (model.TOS == true)
                        {
                            //string DecPassword1 = AppResourceController.Decrypt("TTlN8pvyAykp6bEKkRsxkA==");
                            //string DecPassword2 = AppResourceController.Decrypt("/FTq1mhe7DAm8corkWNp4A==");



                            EncPassword = AppResourceController.Encrypt(model.Password);



                            if (model.Email.Contains("@"))
                            {
                                User nuser = nqz.Users.FirstOrDefault(x => x.UserName == model.Email && x.Password == EncPassword);
                                if (nuser != null)
                                {


                                    TempData["PID"] = nuser.PersonID;
                                    TempData["RoleID"] = nuser.RoleID;
                                    TempData["Verified"] = nuser.IsActive;
                                    Session["PID"] = nuser.PersonID;
                                    Session["ConfirmPhone"] = nuser.ConfirmPhone;
                                    Session["ConfirmEmail"] = nuser.ConfirmEmail;

                                    //Person nperson = nqz.Persons.SingleOrDefault(x => x.PersonID == nuser.PersonID);
                                    var perqry = (from per in nqz.Persons
                                                  join ent in nqz.EntityTypes on per.EntityTypeID equals ent.EntityTypeID
                                                  where per.PersonID == nuser.PersonID
                                                  select new
                                                  {
                                                      per.FirstName,
                                                      per.LastName,
                                                      ent.EntityTypeID,
                                                      Entity = ent.Name,
                                                      ReferreeCode = per.RefereeCode,
                                                      per.ImagePath
                                                  }).FirstOrDefault();
                                    if (perqry != null)
                                    {


                                        Session["FN"] = perqry.FirstName;
                                        Session["LN"] = perqry.LastName;
                                        Session["EntityTypeID"] = perqry.EntityTypeID;
                                        Session["EntityType"] = perqry.Entity;
                                        Session["ReferreeCode"] = perqry.ReferreeCode;
                                        TempData["ReferreeCode"] = perqry.ReferreeCode;
                                        Session["Email"] = model.Email;
                                        Session["ImagePath"] = perqry.ImagePath;

                                    }

                                    nuser.LastLoginDate = DateTime.Now;
                                    nuser.Updated = DateTime.Now;
                                    nqz.SaveChanges();

                                    //Disable All Expired Active Subscriptions
                                    await DisableExpiredSubscriptions(nuser.PersonID.Value);


                                }
                                else
                                {
                                    ViewBag.Err = "Error!!! Invalid username or passwords. Please! try again. [You might also check your Internet Connectivity]";
                                }
                            }
                            else if (model.Email.Substring(0, 1) == "0" || model.Email.Substring(3) == "234")
                            {
                                User nuser = nqz.Users.FirstOrDefault(x => x.PhoneNumber == model.Email && x.Password == EncPassword);
                                if (nuser != null)
                                {
                                    TempData["PID"] = nuser.PersonID;
                                    TempData["RoleID"] = nuser.RoleID;
                                    TempData["Verified"] = nuser.IsActive;
                                    Session["PID"] = nuser.PersonID;
                                    Session["ConfirmPhone"] = nuser.ConfirmPhone;
                                    Session["ConfirmEmail"] = nuser.ConfirmEmail;

                                    var perqry = (from per in nqz.Persons
                                                  join ent in nqz.EntityTypes on per.EntityTypeID equals ent.EntityTypeID
                                                  where per.PersonID == nuser.PersonID
                                                  select new
                                                  {
                                                      per.FirstName,
                                                      per.LastName,
                                                      ent.EntityTypeID,
                                                      Entity = ent.Name,
                                                      ReferreeCode = per.RefereeCode,
                                                      per.ImagePath
                                                  }).FirstOrDefault();
                                    if (perqry != null)
                                    {

                                        Session["FN"] = perqry.FirstName;
                                        Session["LN"] = perqry.LastName;
                                        Session["EntityTypeID"] = perqry.EntityTypeID;
                                        Session["EntityType"] = perqry.Entity;
                                        Session["ReferreeCode"] = perqry.ReferreeCode;
                                        TempData["ReferreeCode"] = perqry.ReferreeCode;
                                        Session["Email"] = model.Email;
                                        Session["ImagePath"] = perqry.ImagePath;
                                    }

                                    nuser.LastLoginDate = DateTime.Now;
                                    nuser.Updated = DateTime.Now;
                                    nqz.SaveChanges();

                                    //Disable All Expired Active Subscriptions
                                    await DisableExpiredSubscriptions(nuser.PersonID.Value);
                                }
                                else
                                {
                                    ViewBag.Err = "Error!!! Invalid username or passwords. Please! try again. [You might also check your Internet Connectivity]";
                                }
                            }
                            else
                            {
                                ViewBag.Err = "Error!!! Error during login. Please! try again. [You might also check your Internet Connectivity]";
                            }
                        }
                        else
                        {
                            ViewBag.Err = "Error!!! Error during login. You must select the Terms of Service check box. Then try again.";

                        }

                    }

                    if (ViewBag.Err == null)
                    {
                        if (model.RememberMe == true)
                        {
                            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

                            HttpCookie cookie = new HttpCookie("UserLogin");
                            cookie.Values.Add("Email", model.Email);
                            cookie.Values.Add("Password", EncPassword);
                            cookie.Values.Add("TOS", model.TOS.ToString());
                            cookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(cookie);
                        }

                        //Set Accounts for the affiliate
                        await SetAccounts(long.Parse(Session["PID"].ToString()));

                        if (TempData["RoleID"].ToString() == "4")
                        {
                            if (Session["EntityTypeID"].ToString() == "1")
                            {
                                if (bool.Parse(Session["ConfirmPhone"].ToString()) == true && (bool.Parse(Session["ConfirmEmail"].ToString()) == true))
                                {
                                    return RedirectToAction("UserInfo", "Account");
                                }
                                else
                                {
                                    if (bool.Parse(Session["ConfirmEmail"].ToString()) == true)
                                    {
                                        return RedirectToAction("UserInfo", "Account");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Verified", "Account");
                                    }

                                }
                            }
                            else if (Session["EntityTypeID"].ToString() == "2")
                            {
                                if (bool.Parse(Session["ConfirmPhone"].ToString()) == true && (bool.Parse(Session["ConfirmEmail"].ToString()) == true))
                                {
                                    return RedirectToAction("UserInfo", "Account");
                                }
                                else
                                {
                                    if (bool.Parse(Session["ConfirmEmail"].ToString()) == true)
                                    {
                                        return RedirectToAction("UserInfo", "Account");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Verified", "Account");
                                    }
                                }
                            }
                            else if (Session["EntityTypeID"].ToString() == "3")
                            {
                                if (bool.Parse(Session["ConfirmPhone"].ToString()) == true && (bool.Parse(Session["ConfirmEmail"].ToString()) == true))
                                {
                                    return RedirectToAction("UserInfo", "Account");
                                }
                                else
                                {
                                    if (bool.Parse(Session["ConfirmEmail"].ToString()) == true)
                                    {
                                        return RedirectToAction("UserInfo", "Account");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Verified", "Account");
                                    }
                                }
                            }
                            else
                            {
                                return View();
                            }
                        }
                        else if (TempData["RoleID"].ToString() == "3")
                        {

                            return RedirectToAction("AdminDashboard", "Quiz");
                            //return RedirectToAction("Questions", "Quiz");
                        }
                        else if (TempData["RoleID"].ToString() == "2")
                        {

                            return RedirectToAction("AdminDashboard", "Quiz");
                            //return RedirectToAction("Questions", "Quiz");
                        }
                        else if (TempData["RoleID"].ToString() == "1")
                        {
                            return RedirectToAction("AdminDashboard", "Quiz");
                            //return RedirectToAction("Questions", "Quiz");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
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
            finally
            {

            }

        }

        public async Task<String> AuthenticateUser()
        {
            string ReturnMessage = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIURL);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                    parameter: Session["SessionNumber"].ToString() + ":" + Session["UserName"].ToString());

                //var responseMessage = await client.GetAsync(requestUri: "Account/GetEmployee");
                var responseMessage = await client.GetAsync(requestUri: "Account/AuthenticateUser");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    ReturnMessage = JsonConvert.DeserializeObject<string>(resultMessage);

                }
            }

            return ReturnMessage;
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> FreeSignin(SigninViewModel model)
        {
            
            string EncPassword = string.Empty;
            try
            {

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    

                    EncPassword = AppResourceController.Encrypt("coss1222");

                    string Email = "cozarrsx@yahoo.com";

                    User nuser = nqz.Users.FirstOrDefault(x => x.UserName == Email && x.Password == EncPassword);
                    if (nuser != null)
                    {
                        FormsAuthentication.SetAuthCookie(nuser.UserName, false);
                     

                    }

                }


                //}

                return RedirectToAction("FreeTrial", "Quiz");

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
            finally
            {

            }

        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> FreeSignout(SigninViewModel model)
        {

            try
            {
                Session.Abandon();
                Session.Clear();
                FormsAuthentication.SignOut();

                return RedirectToAction("FreeSignin", "Quiz");

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
            finally
            {

            }

        }

        //public void GetAccountDetails(long id)
        //{
        //    try
        //    {
        //        using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
        //        {
        //            Account account = nqz.Accounts.FirstOrDefault(x => x.PersonID == id);
        //            if (account != null)
        //            {
        //                TempData["Withdrawals"] = account.Withdrawals;
        //                TempData["Purse"] = account.Purse;
        //                TempData["Earnings"] = account.TempPurse;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        //ViewBag.Err = "Error!!! Invalid username or passwords. Please! try again. [You might also check your Internet Connectivity]";
        //        //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
        //        //return View("Error");
        //    }
        //    finally
        //    {

        //    }
        //}

        public async Task<bool> DisableExpiredSubscriptions(long id)
        {
            bool ret = false;
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {

                    var allsubs = nqz.Subscriptions.Where(x => x.PersonID == id && x.IsActive == true).ToList();
                    if (allsubs.Count > 0 && allsubs != null)
                    {
                        foreach (var item in allsubs)
                        {
                            if (item.EndDate < DateTime.UtcNow)
                            {
                                Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.SubscriptionID == item.SubscriptionID);
                                nsub.IsActive = false;
                                //nsub.Status = false;
                                await nqz.SaveChangesAsync();
                                Session["Subscription"] = "false";
                                ret = false;
                            }
                            else
                            {
                                Session["Subscription"] = "true";
                                ret = true;
                            }

                        }
                    }
                    else
                    {
                        Session["Subscription"] = "false";
                        ret = false;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                return ret;
                //ViewBag.Err = "Error!!! Invalid username or passwords. Please! try again. [You might also check your Internet Connectivity]";
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                //return View("Error");
            }
            finally
            {

            }

        }

        ////////////////////////////////////////

        public ActionResult GenerateReferralCode()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            string refcode = null;
            try
            {
                bool tr = false;
                do
                {
                    GetReferralCode();
                    refcode = ViewBag.RefCode;
                    if (refcode.Length < 5)
                    {
                        tr = true;
                    }
                    else
                    {
                        Person nperson = nqz.Persons.SingleOrDefault(x => x.RefereeCode == refcode);
                        if (nperson == null)
                        {
                            tr = false;
                        }
                        else
                        {

                            tr = true;
                        }
                    }
                } while (tr == true);

                //return refcode;
                ViewBag.ReferralCode = refcode;
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        public ActionResult GenerateParentCode()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            string refcode = null;
            try
            {
                bool tr = false;
                do
                {
                    GetParentCode();
                    refcode = ViewBag.ParCode;
                    if (refcode.Length < 6)
                    {
                        tr = true;
                    }
                    else
                    {
                        Person nperson = nqz.Persons.SingleOrDefault(x => x.ParentCode == refcode);
                        if (nperson == null)
                        {
                            tr = false;
                        }
                        else
                        {

                            tr = true;
                        }
                    }
                } while (tr == true);

                //return refcode;
                ViewBag.ParentCode = refcode;
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        private ActionResult GetReferralCode()
        {

            string number = "";
            Random random = new Random();

            for (int i = 0; i < Convert.ToInt32(1); i++)
            {
                int n = random.Next(0, 100000);
                number = n.ToString();
                //number += n.ToString("D5") + "<br/>";
            }
            ViewBag.RefCode = number;
            return null;
        }

        private ActionResult GetParentCode()
        {

            string number = "";
            Random random = new Random();

            for (int i = 0; i < Convert.ToInt32(1); i++)
            {
                int n = random.Next(0, 1000000);
                number = n.ToString();
                //number += n.ToString("D5") + "<br/>";
            }
            ViewBag.ParCode = number;
            return null;
        }

        [HttpGet]
        public ActionResult Verified()
        {
            if (Session["VErr"].ToString() != "0")
            {
                ViewBag.Err = Session["VErr"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Verified(VerifyPhoneEmailViewModel model)
        {

            ViewBag.Err = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                    {
                        User nuser = nqz.Users.FirstOrDefault(x => x.PhoneCode == model.PhoneCode);
                        if (nuser != null)
                        {
                            nuser.ConfirmPhone = true;
                            nuser.Updated = DateTime.Now;
                            await nqz.SaveChangesAsync();
                            Session["ConfirmPhone"] = true;
                            ViewBag.IsVerified = true;
                        }
                        else
                        {
                            TempData["Verified"] = false;
                            Session["ConfirmPhone"] = false;
                            ViewBag.Err = "Invalid Code. An attempt to verify your Phone Number has Failed. The Code could not be verified. Please, try again.";
                        }
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

        #region Bank Information
        [HttpGet]
        //[AllowAnonymous]
        public ActionResult Bank_Info()
        {
            if (Session.Count <= 0)
            {
                return RedirectToAction("Signin", "Account");
            }

            try
            {


                //List<BANK> banklist = nqz.BANKS.ToList();
                //ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");
                //Session["BankList"] = ViewBag.BankList;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var qry = nqz.BANKS.Where(x => x.IsDeleted == false).Select(x => new { x.BANK_ID, x.BANK1 }).ToList();
                    ViewBag.BankList = qry;
                    Session["BankList"] = ViewBag.BankList;

                    var PId = long.Parse(Session["PID"].ToString());
                    BankInfoViewModels bankInfoView = new BankInfoViewModels();
                    BankInfo nbank = nqz.BankInfos.SingleOrDefault(x => x.PersonID == PId);
                    if (nbank != null)
                    {
                        bankInfoView.BankInfoID = nbank.BankInfoID;
                        bankInfoView.BANK_ID = nbank.BankID;
                        bankInfoView.AccountName = nbank.AccountName;
                        bankInfoView.BranchName = nbank.BranchName;
                        bankInfoView.AccountNumber = nbank.AccountNumber;

                        return View(bankInfoView);
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        //POST
        //[ValidateAntiForgeryToken]
        [HttpPost]
        //[AllowAnonymous]
        public async Task<ActionResult> Bank_Info(BankInfoViewModels model)
        {

            try
            {
                ViewBag.BankList = Session["BankList"];
                if (model.BankInfoID <= 0)
                {
                    ViewBag.Err = "You must select a Bank to proceed.";
                    return View(model);
                }

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {

                    var PerID = long.Parse(Session["PID"].ToString());

                    if (model.BankInfoID > 0)
                    {

                        BankInfo nBank = nqz.BankInfos.SingleOrDefault(x => x.BankInfoID == model.BankInfoID && x.IsDeleted == false);

                        nBank.PersonID = PerID;
                        nBank.BankID = model.BANK_ID;
                        nBank.BranchName = model.BranchName;
                        nBank.AccountName = model.AccountName;
                        nBank.AccountNumber = model.AccountNumber;
                        nBank.Updated = DateTime.Now;
                        await nqz.SaveChangesAsync();

                        ViewBag.Msg = "Specified changes saved successfully";
                    }
                    else
                    {
                        BankInfo nBank = new BankInfo();
                        nBank.PersonID = PerID;
                        nBank.BankID = model.BANK_ID;
                        nBank.BranchName = model.BranchName;
                        nBank.AccountName = model.AccountName;
                        nBank.AccountNumber = model.AccountNumber;
                        nBank.IsDeleted = false;
                        nBank.Created = DateTime.Now;
                        nBank.Updated = DateTime.Now;
                        nqz.BankInfos.Add(nBank);
                        await nqz.SaveChangesAsync();

                        model.BankInfoID = nBank.BankInfoID;

                        ViewBag.Msg = "Record saved successfully";
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        #endregion


        #region ADMIN SECTION

        //----------------------------------------------PRICING REGION---------------------------------------------------------------//
        [HttpGet]
        public ActionResult Pricings()
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            PricingViewModel nmodel = new PricingViewModel();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                ViewBag.DurationList = nqz.DurationInMonths.Where(x => x.IsDeleted == false).Select(x => new { x.DurationID, x.Description }).ToList();

                EditID = 0;

                AllPricing(0);
                return View(nmodel);
            }
            catch (Exception ex)
            {

                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public async Task<ActionResult> Pricings(PricingViewModel model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                ViewBag.DurationList = nqz.DurationInMonths.Where(x => x.IsDeleted == false).Select(x => new { x.DurationID, x.Description }).ToList();

                if (model.SchoolID == 0)
                {
                    model.SchoolID = 1;
                }

                ViewBag.Msg = null;
                ViewBag.Err = null;

                if (model.PricingID <= 0)
                {
                    if (model.FacultyID > 0 && model.Amount == 0)
                    {
                        AllPricing(int.Parse(model.FacultyID.ToString()));
                    }
                    else
                    {
                        if (model.FacultyID == 0)
                        {
                            ViewBag.Err = "Error! you must select a Class to Save";
                        }
                        else if (model.DurationID == 0)
                        {
                            ViewBag.Err = "Error! you must select a Duration to Save";
                        }
                        else if (model.Amount == 0)
                        {
                            ViewBag.Err = "Error! you must enter an Amount. ";
                        }
                        else
                        {
                            Pricing nfac1 = nqz.Pricings.FirstOrDefault(x => x.IsDeleted == false && x.FacultyID == model.FacultyID && x.DurationID == model.DurationID && x.Amount == model.Amount);
                            if (nfac1 == null)
                            {
                                Pricing nfac = new Pricing()
                                {
                                    FacultyID = model.FacultyID,
                                    DurationID = model.DurationID,
                                    Amount = model.Amount,
                                    SchoolID = model.SchoolID,
                                    //Description = model.Description,
                                    IsDeleted = false,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now
                                };
                                nqz.Pricings.Add(nfac);
                                await nqz.SaveChangesAsync();

                                ViewBag.Msg = "Record Saved Successfully";

                                AllPricing(int.Parse(model.FacultyID.ToString()));
                                ModelState.Clear();
                            }
                            else
                            {
                                ViewBag.Err = "An Error Occurred: This class and Duration already exists. Please, try again";
                            }
                        }
                    }

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (model.DurationID != EditID)
                        {
                            EditID = 0;
                            //AllPricing(int.Parse(model.FacultyID.ToString()));
                        }
                        else
                        {
                            Pricing nfac = nqz.Pricings.FirstOrDefault(x => x.IsDeleted == false && x.PricingID == model.PricingID);

                            nfac.FacultyID = model.FacultyID;
                            nfac.DurationID = model.DurationID;
                            nfac.Amount = model.Amount;
                            nfac.SchoolID = model.SchoolID;
                            //Description = model.Description,
                            nfac.Updated = DateTime.Now;
                            //nqz.Pricings.Add(nfac);
                            await nqz.SaveChangesAsync();

                            ViewBag.Msg = "Record Saved Successfully";
                        }
                        AllPricing(int.Parse(model.FacultyID.ToString()));
                        //ModelState.Clear();

                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult NewPricing(Pricing model)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                int grdid = model.FacultyID ?? 0;

                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                ViewBag.DurationList = nqz.DurationInMonths.Where(x => x.IsDeleted == false).Select(x => new { x.DurationID, x.Description }).ToList();
                ModelState.Clear();
                AllPricing(grdid);



            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

            return View("Pricings", model);
        }

        private ActionResult AllPricing(int sklid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                if (sklid > 0)
                {
                    var pricelist = (from fac in nqz.Faculties
                                     join prc in nqz.Pricings on fac.FacultyID equals prc.FacultyID
                                     join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                     where prc.IsDeleted == false && prc.SchoolID == 1 && fac.FacultyID == sklid
                                     select new PricingViewModel
                                     {
                                         PricingID = prc.PricingID,
                                         FacultyID = fac.FacultyID,
                                         ClassName = fac.FacultyName,
                                         SchoolID = prc.SchoolID ?? 0,
                                         DurationID = dur.DurationID,
                                         Duration = dur.Description,
                                         Amount = prc.Amount ?? 0
                                         //FacultyName = fac.FacultyName,
                                         //Description = fac.Description
                                     }).ToList();
                    ViewBag.PriceList = pricelist.OrderBy(x => x.ClassName);
                }
                else
                {
                    var pricelist = (from fac in nqz.Faculties
                                     join prc in nqz.Pricings on fac.FacultyID equals prc.FacultyID
                                     join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                     where prc.IsDeleted == false && prc.SchoolID == 1
                                     select new PricingViewModel
                                     {
                                         PricingID = prc.PricingID,
                                         FacultyID = fac.FacultyID,
                                         ClassName = fac.FacultyName,
                                         SchoolID = prc.SchoolID ?? 0,
                                         DurationID = dur.DurationID,
                                         Duration = dur.Description,
                                         Amount = prc.Amount ?? 0
                                         //FacultyName = fac.FacultyName,
                                         //Description = fac.Description
                                     }).ToList();
                    ViewBag.PriceList = pricelist.OrderBy(x => x.ClassName);
                }
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult EditPricing(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            PricingViewModel model = new PricingViewModel();
            int FID = 0;
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                ViewBag.DurationList = nqz.DurationInMonths.Where(x => x.IsDeleted == false).Select(x => new { x.DurationID, x.Description }).ToList();

                Pricing nfac = nqz.Pricings.SingleOrDefault(x => x.IsDeleted == false && x.PricingID == id);

                FID = nfac.FacultyID ?? 0;
                model.FacultyID = nfac.FacultyID ?? 0;
                model.DurationID = nfac.DurationID ?? 0;
                model.Amount = nfac.Amount ?? 0;
                model.PricingID = nfac.PricingID;
                model.SchoolID = nfac.SchoolID ?? 0;

                EditID = nfac.DurationID ?? 0;

                AllPricing(FID);
                return View("Pricings", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");

            }


        }


        [HttpGet]
        public ActionResult DeletePricing(int id)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;


                Pricing nfac = nqz.Pricings.SingleOrDefault(x => x.PricingID == id);
                if (nfac != null)
                {
                    id = nfac.PricingID;
                    nfac.IsDeleted = true;
                    nfac.Updated = DateTime.Now;
                    nqz.SaveChanges();
                }


                ViewBag.Msg = "Delete Completed Successfully";

                AllPricing(id);
                return RedirectToAction("Pricings", "Account");
            }
            catch (Exception ex)
            {

                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        #endregion

        #region USER SECTION

        //----------------------------------------------PROFILE REGION---------------------------------------------------------------//

        [HttpGet]
        public ActionResult Profiles()
        {

            try
            {


                var pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Person person = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                    ProfileViewModel profileViewModel = new ProfileViewModel()
                    {
                        PersonID = person.PersonID,
                        FirstName = person.FirstName,
                        MiddleName = person.MiddleName,
                        LastName = person.LastName,
                        ReferralCode = person.ReferralCode,
                        ParentCode = person.ParentCode,
                        ImagePath = person.ImagePath

                    };

                    return View(profileViewModel);

                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Profiles(ProfileViewModel model)
        {

            try
            {

                var pid = long.Parse(Session["PID"].ToString());

                //Person person = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);

                if (model.ImageFile.FileName != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string ext = Path.GetExtension(model.ImageFile.FileName);
                    FileName = model.FirstName + "_" + model.LastName + "_" + pid + "_" + DateTime.Now.ToString("yymmddfff") + ext;
                    model.ImagePath = "~/Content/UserPix/" + FileName;
                    FileName = Path.Combine("~/Content/UserPix/", FileName);
                    model.ImageFile.SaveAs(Server.MapPath(FileName));

                    model.ImagePath = FileName;
                    Session["ImagePath"] = FileName;
                }

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Person person = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                    if (person != null)
                    {
                        //PersonID = person.PersonID,
                        person.FirstName = model.FirstName;
                        person.MiddleName = model.MiddleName;
                        person.LastName = model.LastName;
                        person.ReferralCode = model.ReferralCode;
                        person.ParentCode = model.ParentCode;
                        person.ImagePath = model.ImagePath;
                        nqz.SaveChanges();
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        //----------------------------------------------SUBSCRIPTION REGION---------------------------------------------------------------//
        [HttpGet]
        public ActionResult Subscriptions()
        {
            ViewBag.Msg = null;
            ViewBag.Err = null;
            Session["ActiveQuiz"] = " ";
            Session["ActivePQ"] = " ";
            Session["ActiveLib"] = " ";


            //DBQUIZZEntities nqz = new DBQUIZZEntities();

            try
            {
                pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {

                    if (bool.Parse(Session["ConfirmEmail"].ToString()) == true)
                    {
                        //Quiz  Skill Practice
                        var qryQPrice = (from prc in nqz.Pricings
                                         join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                         join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                         where prc.IsDeleted == false && prc.SchoolID == 1 && grd.ResourceTypeID == 1
                                         select new
                                         {
                                             PriceID = prc.PricingID,
                                             Price = grd.FacultyName + " [" + dur.Description + " - " + prc.Amount.ToString() + "]"
                                         }).ToList();
                        ViewBag.QuizPriceList = qryQPrice;
                        Session["QuizPriceList"] = qryQPrice;

                        Subscription nsubq = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 1 && x.IsActive == true);
                        if (nsubq != null)
                        {
                            Session["ActiveQuiz"] = "Active Skill Practice Subscription";
                        }
                        Subscription nsubqz = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 1 && x.Status == false);
                        if (nsubqz != null)
                        {
                            Session["ActiveQuiz"] = "Pending Skill Practice Subscription";

                        }
                        //Past Qustions
                        var qryPPrice = (from prc in nqz.Pricings
                                         join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                         join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                         where prc.IsDeleted == false && prc.SchoolID == 1 && grd.ResourceTypeID == 2
                                         select new
                                         {
                                             PriceID = prc.PricingID,
                                             Price = grd.FacultyName + " [" + dur.Description + " - " + prc.Amount.ToString() + "]"
                                         }).ToList();
                        ViewBag.PastQuestionPriceList = qryPPrice;
                        Session["PastQuestionPriceList"] = qryPPrice;

                        Subscription nsubpq = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 2 && x.IsActive == true);
                        if (nsubpq != null)
                        {
                            Session["ActivePQ"] = "Active Past Question Subscription";
                        }
                        Subscription nsubpqz = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 2 && x.Status == false);
                        if (nsubpqz != null)
                        {
                            Session["ActivePQ"] = "Pending Past Question Subscription";

                        }

                        //E-Library
                        var qryLPrice = (from prc in nqz.Pricings
                                         join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                         join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                         where prc.IsDeleted == false && prc.SchoolID == 1 && grd.ResourceTypeID == 3
                                         select new
                                         {
                                             PriceID = prc.PricingID,
                                             Price = grd.FacultyName + " [" + dur.Description + " - " + prc.Amount.ToString() + "]"
                                         }).ToList();
                        ViewBag.LibraryPriceList = qryLPrice;
                        Session["LibraryPriceList"] = qryLPrice;

                        Subscription nsublb = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 3 && x.IsActive == true);
                        if (nsubpq != null)
                        {
                            Session["ActiveLib"] = "Active E-Library Subscription";
                        }
                        Subscription nsublbz = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.ResourceTypeID == 3 && x.Status == false);
                        if (nsubpqz != null)
                        {
                            Session["ActiveLib"] = "Pending E-Library Subscription";

                        }
                    }
                    else
                    {
                        Session["VErr"] = "You must Verify your registered Email and or Phone Number and then proceed to Subscription.";
                        return RedirectToAction("Verified", "Account");
                    }
                }

                stintCnt = 0;
                //AllSubscription(0);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult Subscriptions(PreSubscriptionViewModel model)
        {
            ViewBag.Msg = null;
            ViewBag.Err = null;

            bool QuizErr = false;
            bool PastQuestionErr = false;
            bool LibraryErr = false;


            try
            {
                pid = long.Parse(Session["PID"].ToString());
                //DBQUIZZEntities nqz = new DBQUIZZEntities();
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (model.QuizPricingID <= 0)
                    {
                        if (ViewBag.QuizPriceList == null)
                        {
                            var qryPrice = Session["QuizPriceList"];
                            ViewBag.QuizPriceList = qryPrice;
                        }
                        if (!Session["ActiveQuiz"].ToString().Contains("Pending"))
                        {
                            QuizErr = true;
                        }

                    }

                    if (model.PastQuestionPricingID <= 0)
                    {
                        if (ViewBag.PastQuestionPriceList == null)
                        {
                            var qryPrice = Session["PastQuestionPriceList"];
                            ViewBag.PastQuestionPriceList = qryPrice;
                        }
                        if (!Session["ActivePQ"].ToString().Contains("Pending"))
                        {
                            PastQuestionErr = true;
                        }

                    }

                    if (model.LibraryPricingID <= 0)
                    {
                        if (ViewBag.LibraryPriceList == null)
                        {
                            var qryPrice = Session["LibraryPriceList"];
                            ViewBag.LibraryPriceList = qryPrice;
                        }
                        if (!Session["ActiveLib"].ToString().Contains("Pending"))
                        {
                            LibraryErr = true;
                        }

                    }

                    if (QuizErr == true && PastQuestionErr == true && LibraryErr == true)
                    {
                        ViewBag.Err = "ERROR!!! Subscription must be made to at least, one Resource. ";
                    }
                    else
                    {
                        Subscription nsubq = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.PricingID == model.QuizPricingID && x.IsActive == false && x.Status == false);
                        if (nsubq == null)
                        {
                            if (model.QuizPricingID > 0)
                            {
                                var nprc = (from prc in nqz.Pricings
                                            join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                            join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                            where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == model.QuizPricingID
                                            select new
                                            {
                                                grd.ResourceTypeID,
                                                PriceID = prc.PricingID,
                                                Price = prc.Amount,
                                                Period = dur.Duration,
                                                dur.Duration
                                            }).FirstOrDefault();

                                if (nprc != null)
                                {
                                    Subscription nsub = new Subscription()
                                    {
                                        PersonID = pid, //long.Parse(Session["PID"].ToString()),
                                        PricingID = nprc.PriceID,
                                        ResourceTypeID = nprc.ResourceTypeID,
                                        Amount = nprc.Price ?? 0,
                                        Duration = nprc.Duration,
                                        StartDate = DateTime.UtcNow,
                                        EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30),
                                        IsActive = false,
                                        Status = false,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow
                                    };
                                    nqz.Subscriptions.Add(nsub);
                                    nqz.SaveChanges();

                                }
                            }
                        }

                        Subscription nsubp = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.PricingID == model.PastQuestionPricingID && x.IsActive == false && x.Status == false);
                        if (nsubq == null)
                        {
                            if (model.PastQuestionPricingID > 0)
                            {

                                var nprc = (from prc in nqz.Pricings
                                            join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                            join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                            where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == model.PastQuestionPricingID
                                            select new
                                            {
                                                grd.ResourceTypeID,
                                                PriceID = prc.PricingID,
                                                Price = prc.Amount,
                                                Period = dur.Duration,
                                                dur.Duration
                                            }).FirstOrDefault();

                                if (nprc != null)
                                {
                                    Subscription nsub = new Subscription()
                                    {
                                        PersonID = pid, //long.Parse(Session["PID"].ToString()),
                                        PricingID = nprc.PriceID,
                                        ResourceTypeID = nprc.ResourceTypeID,
                                        Amount = nprc.Price ?? 0,
                                        Duration = nprc.Duration,
                                        StartDate = DateTime.UtcNow,
                                        EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30),
                                        IsActive = false,
                                        Status = false,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow
                                    };
                                    nqz.Subscriptions.Add(nsub);
                                    nqz.SaveChanges();
                                }
                            }
                        }

                        Subscription nsubl = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == pid && x.PricingID == model.LibraryPricingID && x.IsActive == false && x.Status == false);
                        if (nsubq == null)
                        {
                            if (model.LibraryPricingID > 0)
                            {
                                var nprc = (from prc in nqz.Pricings
                                            join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                                            join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                            where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == model.LibraryPricingID
                                            select new
                                            {
                                                grd.ResourceTypeID,
                                                PriceID = prc.PricingID,
                                                Price = prc.Amount,
                                                Period = dur.Duration,
                                                dur.Duration
                                            }).FirstOrDefault();
                                pid = long.Parse(Session["PID"].ToString());
                                if (nprc != null)
                                {
                                    Subscription nsub = new Subscription()
                                    {
                                        PersonID = pid,//long.Parse(Session["PID"].ToString()),
                                        PricingID = nprc.PriceID,
                                        ResourceTypeID = nprc.ResourceTypeID,
                                        Amount = nprc.Price ?? 0,
                                        Duration = nprc.Duration,
                                        StartDate = DateTime.UtcNow,
                                        EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30),
                                        IsActive = false,
                                        Status = false,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow
                                    };
                                    nqz.Subscriptions.Add(nsub);
                                    nqz.SaveChanges();
                                }
                            }
                        }
                    }
                }




                //if (blSubscribe == false)
                //{
                //    stintCnt = model.PricingID;
                //    //stintCnt++;
                //    //AllSubscription(int.Parse(model.SubscriptionID.ToString()));
                //    //Pricing nprc = nqz.Pricings.FirstOrDefault(x => x.PricingID == model.PricingID && x.IsDeleted == false);
                //    var nprc = (from prc in nqz.Pricings
                //                join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                //                join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                //                where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == model.PricingID
                //                select new
                //                {

                //                    Grade = grd.FacultyName,
                //                    PriceID = prc.PricingID,
                //                    Price = prc.Amount,
                //                    Period = dur.Duration,
                //                    Duration = dur.Description
                //                }).FirstOrDefault();
                //    pid = long.Parse(Session["PID"].ToString());
                //    if (nprc != null)
                //    {
                //        model.PersonID = long.Parse(Session["PID"].ToString());
                //        model.PricingID = nprc.PriceID;
                //        model.Grade = nprc.Grade;
                //        model.Amount = nprc.Price ?? 0;
                //        model.Period = nprc.Duration;
                //        model.StartDate = DateTime.UtcNow;
                //        model.EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30);

                //    }
                //    Session["NSUB"] = model;
                //    //Session["Subscription"] = true;

                //}
                //else
                //{
                //    //ViewBag.Msg = "1";

                //}

                if (ViewBag.Err == null)
                {

                    //return View("SubscriptionsPreview");
                    return RedirectToAction("SubscriptionsPreview");

                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }


        [HttpGet]
        //public ActionResult SubscriptionsPreview(SubscriptionPreviewViewModel model)
        public ActionResult SubscriptionsPreview(SubscriptionViewModel model)
        {
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                decimal? RegFees = 0;
                pid = long.Parse(Session["PID"].ToString());

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    User nuser = nqz.Users.FirstOrDefault(x => x.PersonID == pid && x.IsRegistered == false);
                    if (nuser != null)
                    {
                        RegFees = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).RegistrationFee;
                        ViewBag.RegFees = RegFees;
                    }

                    var nSub = (from prc in nqz.Pricings
                                join fac in nqz.Faculties on prc.FacultyID equals fac.FacultyID
                                join res in nqz.ResourceTypes on fac.ResourceTypeID equals res.ResourceTypeID
                                join sub in nqz.Subscriptions on prc.PricingID equals sub.PricingID
                                where sub.PersonID == pid && sub.Status == false && sub.IsActive == false && sub.Duration > 0
                                select new SubscriptionPreviewViewModel
                                {
                                    SubscriptionID = sub.SubscriptionID,
                                    ResourceType = res.ResourceTypeName,
                                    Class = fac.FacultyName,
                                    Duration = sub.Duration + " month(s)",
                                    Amount = sub.Amount,
                                    StartDate = sub.StartDate,
                                    EndDate = sub.EndDate
                                }).ToList();

                    ViewBag.PreviewList = nSub;
                }


                //var nprc = (from prc in nqz.Pricings
                //            join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                //            join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                //            where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == stintCnt
                //            select new
                //            {

                //                Grade = grd.FacultyName,
                //                PriceID = prc.PricingID,
                //                Price = prc.Amount,
                //                Period = dur.Duration,
                //                Duration = dur.Description
                //            }).FirstOrDefault();
                //if (nprc != null)
                //{
                //    model.PersonID = pid;
                //    model.PricingID = nprc.PriceID;
                //    model.Grade = nprc.Grade;
                //    model.Amount = nprc.Price ?? 0;
                //    model.Period = nprc.Duration;
                //    model.StartDate = DateTime.UtcNow;
                //    model.EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30);

                //}

                //model = TempData["Subscription"] as SubscriptionPreviewViewModel;





                //var result = PayStackManager.MakePayStackPayment(model.Amount.ToString(), email, "1");
                //Task.Factory.StartNew(new PageAsyncTask(InitTransaction));
                //Task new PageAsyncTask(InitTransaction)
                //return Redirect(result);;


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult SubscriptionsPreview()
        {
            ViewBag.Msg = null;
            ViewBag.Err = null;
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //if (ModelState.IsValid)
                //{

                //}

                //TempData["Subscription"] = model;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        //public ActionResult PayWithPayStack()
        //{

        //    var result = PayStackManager.MakePayStackPayment();
        //    return Redirect(result);
        //}

        public async Task<ActionResult> PaymentResponse(string reference)
        {
            decimal? Amt = 0, RegFees = 0;
            //var result = PayStackManager.VerifyPayStackPayment(reference);
            //return Redirect(result);

            ViewBag.Msg = null;
            ViewBag.Err = null;
            string enccode = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(reference))
                {
                    if (PayConfirmation != reference)
                    {

                        //var VerifyStatus = PayStackManager.VerifyPayStackPayment(reference);
                        var VerifyStatus = reference;
                        //if (VerifyStatus == "Success")
                        if (VerifyStatus != null)
                        {
                            string SRC = Session["SRC"].ToString();

                            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                            {

                                //var mySubscrtn = long.Parse(Session["SubscriptionID"].ToString());
                                if (SRC == "Registration")
                                {
                                    SignupViewModel model = Session["RegModel"] as SignupViewModel;

                                    Person nperson = new Person()
                                    {
                                        FirstName = model.FirstName,
                                        LastName = model.LastName,
                                        EntityTypeID = int.Parse(model.EntityType),
                                        ReferralCode = model.ReferralCode,
                                        RefereeCode = model.ReferreeCode,
                                        ParentCode = model.ParentCode ?? "0",
                                        MaxFirstLevel = model.MaxFirstLevel,
                                        IsCompany = false,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now
                                    };
                                    nqz.Persons.Add(nperson);
                                    await nqz.SaveChangesAsync();

                                    string EncPassword = AppResourceController.Encrypt(model.Password);

                                    User nuser = new Models.User()
                                    {
                                        PersonID = nperson.PersonID,
                                        UserName = model.Email,
                                        PhoneNumber = model.Phone,
                                        RoleID = 4,
                                        IsActive = false,
                                        IsRegistered = false,
                                        Password = EncPassword,
                                        PhoneCode = ViewBag.ReferralCode,
                                        ConfirmPhone = false,
                                        ConfirmEmail = false,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now
                                    };
                                    nqz.Users.Add(nuser);
                                    await nqz.SaveChangesAsync();

                                    enccode = AppResourceController.Encrypt(model.Phone + "|" + RefCode);
                                    Session["PhoneDetails"] = enccode;

                                    //Save Contact Information
                                    Contact ncont = new Contact()
                                    {
                                        PersonID = nperson.PersonID,
                                        Phone = model.Phone,
                                        Email = model.Email,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now

                                    };
                                    nqz.Contacts.Add(ncont);
                                    await nqz.SaveChangesAsync();

                                    //Setup Accounts for the individual
                                    Account account = new Account()
                                    {
                                        PersonID = nperson.PersonID,
                                        Purse = 0,
                                        TempPurse = 0,
                                        Withdrawals = 0,
                                        Admin2 = 0,
                                        IsCompany = false,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now
                                    };
                                    nqz.Accounts.Add(account);
                                    await nqz.SaveChangesAsync();

                                    //SEND SMS
                                    //string strSMS = "Thank you for Registering with Braingain.com. However, to complete your Registration, Here is your Account Activation (Phone) Code: " + ViewBag.ReferralCode;
                                    //AppAsyncController.RunAsync(strSMS, model.Phone).Wait();

                                    //SEND REGISTRATION MAIL
                                    string RR = ConfigurationManager.AppSettings["DomainHeader"].ToString() + Request.ServerVariables["HTTP_HOST"] + "/Account/SignupResponse?key=";
                                    AppResourceController nappRes = new AppResourceController();
                                    nappRes.SendRegistrationMail(model.Email, model.LastName + ' ' + model.FirstName, model.Password, model.ReferralCode, model.ParentCode, Convert.ToString(nuser.UserID), RR);

                                    //SEND SMS
                                    string strSMS = "Thank you for Registering with braingainspa.com. However, to complete your Registration, Here is your Account Activation (Phone) Code: " + model.ReferralCode;
                                    AppResourceController.SendSMS("", model.Phone, strSMS);
                                    ViewBag.Msg = "1";

                                    var myID = long.Parse(Session["PID"].ToString());

                                    //Activate All Pending Subscriptions
                                    var allsubs = nqz.Subscriptions.Where(x => x.PersonID == myID && x.Status == false && x.Duration > 0).ToList();
                                    if (allsubs != null)
                                    {
                                        foreach (var item in allsubs)
                                        {
                                            Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.SubscriptionID == item.SubscriptionID);
                                            nsub.IsActive = true;
                                            nsub.Status = true;
                                            nsub.Updated = DateTime.UtcNow;
                                            nqz.SaveChanges();

                                            Amt += item.Amount;
                                        }
                                    }

                                    //Set User Registration
                                    User nuserx = nqz.Users.FirstOrDefault(x => x.PersonID == myID);
                                    if (nuserx != null)
                                    {
                                        User nuser1 = nqz.Users.FirstOrDefault(x => x.UserID == nuser.UserID);
                                        nuser1.IsRegistered = true;
                                        nuser1.IsActive = true;
                                        nuser1.Updated = DateTime.UtcNow;
                                        nqz.SaveChanges();

                                        RegFees = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).RegistrationFee;

                                        //Insert Registration Amount into Payments
                                        ClientPayment nclpayR = new ClientPayment()
                                        {
                                            PersonID = myID,
                                            PaymentModeID = 1,
                                            Amount = RegFees,
                                            Status = true,
                                            Description = "Registration Payment",
                                            IsDeleted = false,
                                            Created = DateTime.UtcNow,
                                            Updated = DateTime.UtcNow
                                        };
                                        nqz.ClientPayments.Add(nclpayR);
                                        nqz.SaveChanges();

                                    }

                                    //Get User Referree Code
                                    string myreferral = nqz.Persons.FirstOrDefault(x => x.PersonID == myID).RefereeCode;

                                    //Update Pending Payments or Earnings
                                    //TempFlag determines Payments that were not received
                                    //AcctFlag is used to resolve or lock in payments received by earners
                                    PaymentTransaction npaytrans = nqz.PaymentTransactions.FirstOrDefault(x => x.ReferralCode == myreferral && x.TempFlag == true && x.AcctFlag == false);
                                    if (npaytrans != null)
                                    {
                                        npaytrans.TempFlag = false;
                                        nqz.SaveChanges();
                                    }

                                    //Insert Details into Payments
                                    ClientPayment nclpay = new ClientPayment()
                                    {
                                        PersonID = myID,
                                        PaymentModeID = 1,
                                        Amount = Amt,
                                        Status = true,
                                        Description = "Subscription Payment",
                                        IsDeleted = false,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow
                                    };
                                    nqz.ClientPayments.Add(nclpay);
                                    nqz.SaveChanges();

                                    //Pass through MLM Routine
                                    if (RegFees > 0)
                                    {
                                        //Subscription + Registration Payment
                                        MLM(myID, RegFees ?? 0, Amt ?? 0, 1);
                                    }
                                    else
                                    {
                                        //Subscription Payment Only
                                        MLM(myID, RegFees ?? 0, Amt ?? 0, 2);
                                    }
                                }
                                else //Subscription
                                {

                                    var myID = long.Parse(Session["PID"].ToString());

                                    //Activate All Pending Subscriptions
                                    var allsubs = nqz.Subscriptions.Where(x => x.PersonID == myID && x.Status == false && x.Duration > 0).ToList();
                                    if (allsubs != null)
                                    {
                                        foreach (var item in allsubs)
                                        {
                                            Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.SubscriptionID == item.SubscriptionID);
                                            nsub.IsActive = true;
                                            nsub.Status = true;
                                            nsub.Updated = DateTime.UtcNow;
                                            nqz.SaveChanges();

                                            Amt += item.Amount;
                                        }
                                    }

                                    //Set User Registration
                                    //User nuser = nqz.Users.FirstOrDefault(x => x.PersonID == myID && x.IsRegistered == false);
                                    //if (nuser != null)
                                    //{
                                    //    User nuser1 = nqz.Users.FirstOrDefault(x => x.UserID == nuser.UserID);
                                    //    nuser1.IsRegistered = true;
                                    //    nuser1.IsActive = true;
                                    //    nuser1.Updated = DateTime.UtcNow;
                                    //    nqz.SaveChanges();

                                    //    RegFees = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).RegistrationFee;

                                    //    //Insert Registration Amount into Payments
                                    //    ClientPayment nclpayR = new ClientPayment()
                                    //    {
                                    //        PersonID = myID,
                                    //        PaymentModeID = 1,
                                    //        Amount = RegFees,
                                    //        Status = true,
                                    //        Description = "Registration Payment",
                                    //        IsDeleted = false,
                                    //        Created = DateTime.UtcNow,
                                    //        Updated = DateTime.UtcNow
                                    //    };
                                    //    nqz.ClientPayments.Add(nclpayR);
                                    //    nqz.SaveChanges();

                                    //}

                                    //Get User Referree Code
                                    string myreferral = nqz.Persons.FirstOrDefault(x => x.PersonID == myID).RefereeCode;

                                    //Update Pending Payments or Earnings
                                    //TempFlag determines Payments that were not received
                                    //AcctFlag is used to resolve or lock in payments received by earners
                                    PaymentTransaction npaytrans = nqz.PaymentTransactions.FirstOrDefault(x => x.ReferralCode == myreferral && x.TempFlag == true && x.AcctFlag == false);
                                    if (npaytrans != null)
                                    {
                                        npaytrans.TempFlag = false;
                                        nqz.SaveChanges();
                                    }

                                    //Insert Details into Payments
                                    ClientPayment nclpay = new ClientPayment()
                                    {
                                        PersonID = myID,
                                        PaymentModeID = 1,
                                        Amount = Amt,
                                        Status = true,
                                        Description = "Subscription Payment",
                                        IsDeleted = false,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow
                                    };
                                    nqz.ClientPayments.Add(nclpay);
                                    nqz.SaveChanges();
                                }
                            }


                            PayConfirmation = reference;

                            ViewBag.Msg = "Your Payment was verified and the transaction has been completed successfully.";
                            //return View("SuccessView");
                        }
                        else
                        {
                            ViewBag.Err = "Your Payment could not be verified by the Payment System Server, and therefore, the transaction failed!";
                            //return View("FailureView");
                        }
                    }
                }
                else
                {
                    ViewBag.Err = "Your Payment could not be verified by the Payment System Server, and therefore, the transaction failed!";
                    //return View("FailureView");
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        public ActionResult MLM(long id, decimal regamt, decimal subamt, int src) //src: 1 = Registration + Subscription, 2 = Subscription
        {
            decimal totamt = 0;

            decimal[] regAmts = new decimal[4], subAmts = new decimal[4];
            float[] regPercent = new float[4], subPercent = new float[4];
            string[] arrLevel = new string[4];

            float regCompPercent = 0, subCompPercent = 0, regRwdPercent = 0, subRwdPercent = 0, regAdm1Percent = 0, subAdm1Percent, regAdm2Percent = 0, subAdm2Percent;
            decimal regCompAmts = 0, subCompAmts = 0, regRwdAmts = 0, subRwdAmts = 0, regAdm1Amts = 0, subAdm1Amts = 0, regAdm2Amts = 0, subAdm2Amts = 0;

            string compRefereeCode = null;
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //Get Company Referee Code
                    compRefereeCode = nqz.Persons.FirstOrDefault(x => x.IsCompany == true).RefereeCode;
                    int maxdownlines = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).MaxUnderline ?? 0;

                    //Get Percentages
                    int k = 0;
                    var nitems = nqz.Percentages.Where(x => x.IsDeleted == false).Select(x => new { x.PercentName, x.PType, x.Registration, x.Subscription });
                    foreach (var item in nitems)
                    {
                        if (item.PType.Trim() == "LEVEL")
                        {
                            //Get the percentages
                            arrLevel[k] = item.PercentName.ToString();
                            regPercent[k] = float.Parse(item.Registration.ToString());
                            subPercent[k] = float.Parse(item.Subscription.ToString());

                            //Calculate the amounts
                            regAmts[k] = (decimal.Parse(regPercent[k].ToString()) / 100) * regamt;
                            subAmts[k] = (decimal.Parse(subPercent[k].ToString()) / 100) * subamt;
                        }
                        else if (item.PType.Trim() == "Company")
                        {
                            //Get the percentages
                            regCompPercent = float.Parse(item.Registration.ToString());
                            subCompPercent = float.Parse(item.Subscription.ToString());

                            //Calculate the Company amounts
                            regCompAmts = (decimal.Parse(regCompPercent.ToString()) / 100) * regamt;
                            subCompAmts = (decimal.Parse(subCompPercent.ToString()) / 100) * subamt;
                        }
                        else if (item.PType.Trim() == "Rewards")
                        {
                            //Get the percentages
                            regRwdPercent = float.Parse(item.Registration.ToString());
                            subRwdPercent = float.Parse(item.Subscription.ToString());

                            //Calculate the Rewards amounts
                            regRwdAmts = (decimal.Parse(regRwdPercent.ToString()) / 100) * regamt;
                            subRwdAmts = (decimal.Parse(subRwdPercent.ToString()) / 100) * subamt;
                        }
                        else if (item.PType.Trim() == "Admin1")
                        {
                            //Get the percentages
                            regAdm1Percent = float.Parse(item.Registration.ToString());
                            subAdm1Percent = float.Parse(item.Subscription.ToString());

                            //Calculate the Admin1 amounts
                            regAdm1Amts = (decimal.Parse(regAdm1Percent.ToString()) / 100) * regamt;
                            subAdm1Amts = (decimal.Parse(subAdm1Percent.ToString()) / 100) * subamt;

                        }
                        else if (item.PType.Trim() == "Admin2")
                        {
                            //Get the percentages
                            regAdm2Percent = float.Parse(item.Registration.ToString());
                            subAdm2Percent = float.Parse(item.Subscription.ToString());

                            //Calculate the Admin2 amounts
                            regAdm2Amts = (decimal.Parse(regAdm2Percent.ToString()) / 100) * regamt;
                            subAdm2Amts = (decimal.Parse(subAdm2Percent.ToString()) / 100) * subamt;
                        }
                        k++;
                    }

                    int z = 0;
                    long levelid = id;
                    int i = 0;
                    string myrefereecode = nqz.Persons.FirstOrDefault(x => x.PersonID == id).RefereeCode;

                    if (src == 1) //Registration And Subscription
                    {
                        ///////////////////////////////////Make Payments for Registration//////////////////////////////
                        compComment = "Company";
                        //Pay Company
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regCompAmts, "Company", 1, false, false);

                        //Pay Rewards
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regRwdAmts, "Rewards", 1, false, false);

                        //Pay Admin1
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regAdm1Amts, "Admin1", 1, false, false);

                        //Pay Admin2
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regAdm2Amts, "Admin2", 1, false, false);

                        //Pay Users
                        do
                        {
                            //Get Referral Code
                            var varcodes = nqz.Persons.Where(x => x.PersonID == levelid).Select(x => new { x.PersonID, x.ReferralCode }).FirstOrDefault();

                            if (varcodes.ReferralCode == compRefereeCode)
                            {
                                string comm = "reg_downline_Level_";
                                string lev = "";
                                totamt = 0;
                                for (int j = i; j <= maxdownlines - 1; j++)
                                {
                                    z++;
                                    totamt += regAmts[j];
                                    lev = lev + z + "_";
                                }

                                //Save Upline Payment To Company
                                compComment = "Company";
                                SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, totamt, comm + lev, 1, false, false);
                                break;
                            }
                            else
                            {
                                compComment = "Member";
                                long uplineid = nqz.Persons.FirstOrDefault(x => x.RefereeCode == varcodes.ReferralCode).PersonID;
                                //Check for current subscription
                                if (CheckCurrentSubscription(uplineid))
                                {
                                    //Save Upline Payment
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, regAmts[i], "reg_downline_level_", 1, false, false);

                                }
                                else
                                {
                                    //Save Unclaimed Payment to Temporary Purse

                                    //SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regAmts[i], "reg_unclaimed_level_", 1);
                                    //Save Upline Payment
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, regAmts[i], "reg_downline_level_", 1, true, false);
                                }
                                //Assign Upline id
                                levelid = uplineid;
                            }
                            i++;
                        } while (i <= maxdownlines - 1);


                        ///////////////////////////////////Make Payment for Subscription/////////////////////////////////////////
                        compComment = "Company";
                        //Pay Company
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subCompAmts, "Company", 2, false, false);

                        //Pay Rewards
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subRwdAmts, "Rewards", 2, false, false);

                        //Pay Admin1
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subAdm1Amts, "Admin1", 2, false, false);

                        //Pay Admin2
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subAdm2Amts, "Admin2", 2, false, false);

                        //Pay Users

                        i = 0;
                        levelid = id;
                        do
                        {
                            //Get Referral Code
                            var varcodes = nqz.Persons.Where(x => x.PersonID == levelid).Select(x => new { x.PersonID, x.ReferralCode }).FirstOrDefault();

                            if (varcodes.ReferralCode == compRefereeCode)
                            {
                                string comm = "sub_downline_Level_";
                                string lev = "";
                                totamt = 0;
                                z = 0;
                                for (int j = i; j <= maxdownlines - 1; j++)
                                {
                                    z++;
                                    totamt += subAmts[j];
                                    lev = lev + z + "_";
                                }


                                //Save Upline Payment(s) To Company
                                compComment = "Company";
                                SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, totamt, comm + lev, 2, false, false);
                                break;
                            }
                            else
                            {
                                compComment = "Member";
                                long uplineid = nqz.Persons.FirstOrDefault(x => x.RefereeCode == varcodes.ReferralCode).PersonID;
                                //Check for current subscription
                                if (CheckCurrentSubscription(uplineid))
                                {
                                    //Save Upline Payment
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, subAmts[i], "sub_downline_level_", 2, false, false);

                                }
                                else
                                {
                                    //Save Unclaimed Payment to Company

                                    //compComment = "Company";
                                    //SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regAmts[i], "sub_unclaimed_level_", 2);
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, subAmts[i], "sub_downline_level_", 2, true, false);
                                }
                                //Assign Upline id
                                levelid = uplineid;
                            }
                            i++;
                        } while (i <= maxdownlines - 1);
                    }
                    else //Subscription Only
                    {
                        //Pay Subscriptions
                        compComment = "Company";
                        //Pay Company
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subCompAmts, "Company", 2, false, false);

                        //Pay Rewards
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subRwdAmts, "Rewards", 2, false, false);

                        //Pay Admin1
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subAdm1Amts, "Admin1", 2, false, false);

                        //Pay Admin2
                        SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, subAdm2Amts, "Admin2", 2, false, false);

                        //Pay Users
                        do
                        {
                            //Get Referral Code
                            var varcodes = nqz.Persons.Where(x => x.PersonID == levelid).Select(x => new { x.PersonID, x.RefereeCode, x.ReferralCode }).FirstOrDefault();

                            if (varcodes.ReferralCode == compRefereeCode)
                            {
                                string comm = "sub_downline_Level_";
                                string lev = "";
                                totamt = 0;
                                z = 0;
                                for (int j = i; j <= maxdownlines - 1; j++)
                                {
                                    z++;
                                    totamt += subAmts[j];
                                    lev = lev + z + "_";
                                }

                                compComment = "Company";
                                //Save Upline Payment(s) To Company
                                SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, totamt, comm + lev, 2, false, false);
                                break;
                            }
                            else
                            {
                                compComment = "Member";
                                long uplineid = nqz.Persons.FirstOrDefault(x => x.RefereeCode == varcodes.ReferralCode).PersonID;
                                //Check for current subscription
                                if (CheckCurrentSubscription(uplineid) == true)
                                {
                                    //Save Upline Payment
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, subAmts[i], "sub_downline_level_", 2, false, false);

                                }
                                else
                                {
                                    //Save Unclaimed Payment to Company
                                    //SavePaymentTransaction(id, myrefereecode, compRefereeCode, i, regAmts[i], "sub_unclaimed_level_", 2);
                                    SavePaymentTransaction(id, myrefereecode, varcodes.ReferralCode, i, subAmts[i], "sub_downline_level_", 2, true, false);
                                }
                                //Assign Upline id
                                levelid = uplineid;
                            }
                            i++;
                        } while (i <= maxdownlines - 1);
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        public static bool CheckCurrentSubscription(long personid)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == personid && x.IsActive == true);
                    if (nsub != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return false;
                //return View("Error");
            }
        }

        //Check for Resource Subscription
        public static bool CheckResourceSubscription(long personid, int resourcetypeid)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Subscription nsub = nqz.Subscriptions.FirstOrDefault(x => x.PersonID == personid && x.ResourceTypeID == resourcetypeid && x.IsActive == true);
                    if (nsub != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return false;
                //return View("Error");
            }
        }

        public ActionResult SavePaymentTransaction(long perid, string referee, string referal, int level, decimal amt, string comment, int transtypeid, bool tempflag, bool toCompany)
        {
            try
            {
                //Set Member Level comment
                int Tmplevel = 0;
                if (compComment != "Company")
                {
                    Tmplevel = level++;
                    comment = comment + level++;
                }


                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    PaymentTransaction npaytrans = new PaymentTransaction()
                    {
                        PersonID = perid,
                        RefereeCode = referee,
                        ReferralCode = referal,
                        Amount = amt,
                        NLevel = Tmplevel,
                        TransactionTypeID = transtypeid,
                        Comment = comment,
                        TempFlag = tempflag,
                        AcctFlag = false,
                        ToCompanyFlag = toCompany,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };
                    nqz.PaymentTransactions.Add(npaytrans);
                    nqz.SaveChanges();

                    //Update Member's account
                    //Account account = nqz.Accounts.FirstOrDefault(x => x.PersonID == perid);
                    //account.Purse = account.Purse + amt;
                    //account.Updated = DateTime.UtcNow;
                }

                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        public ActionResult VerifyPayStackPayment(string reference)
        {
            if (!string.IsNullOrEmpty(reference))
            {
                var VerifyStatus = PayStackManager.VerifyPayStackPayment(reference);
                if (VerifyStatus == "Success")
                {
                    ViewBag.Message = "Your Payment has been successfully made and the shipper has also been notified";
                    return View("SuccessView");
                }
                else
                {
                    ViewBag.Message = "Payment not verified by PayStack Server!";
                    return View("FailureView");
                }
            }
            else
            {
                ViewBag.Message = "Payment not verified by PayStack Server!";
                return View("FailureView");
            }

        }

        [HttpGet]
        public ActionResult NewSubscription(SubscriptionViewModel model)
        {

            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    long grdid = model.SubscriptionID;

                    ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                    ModelState.Clear();
                    AllSubscription(grdid);
                }
                return View("Pricings", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        private ActionResult AllSubscription(long sklid)
        {

            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (sklid > 0)
                    {
                        var subscriptionlist = (from fac in nqz.Faculties
                                                join prc in nqz.Pricings on fac.FacultyID equals prc.FacultyID
                                                join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                                where prc.IsDeleted == false && prc.SchoolID == 1 && fac.FacultyID == sklid
                                                select new PricingViewModel
                                                {
                                                    PricingID = prc.PricingID,
                                                    FacultyID = fac.FacultyID,
                                                    ClassName = fac.FacultyName,
                                                    SchoolID = prc.SchoolID ?? 0,
                                                    DurationID = dur.DurationID,
                                                    Duration = dur.Description,
                                                    Amount = prc.Amount ?? 0
                                                    //FacultyName = fac.FacultyName,
                                                    //Description = fac.Description
                                                }).ToList();
                        ViewBag.SubscriptionList = subscriptionlist.OrderBy(x => x.ClassName);
                    }
                    else
                    {
                        var subscriptionlist = (from fac in nqz.Faculties
                                                join prc in nqz.Pricings on fac.FacultyID equals prc.FacultyID
                                                join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                                                where prc.IsDeleted == false && prc.SchoolID == 1
                                                select new PricingViewModel
                                                {
                                                    PricingID = prc.PricingID,
                                                    FacultyID = fac.FacultyID,
                                                    ClassName = fac.FacultyName,
                                                    SchoolID = prc.SchoolID ?? 0,
                                                    DurationID = dur.DurationID,
                                                    Duration = dur.Description,
                                                    Amount = prc.Amount ?? 0
                                                    //FacultyName = fac.FacultyName,
                                                    //Description = fac.Description
                                                }).ToList();
                        ViewBag.SubscriptionList = subscriptionlist.OrderBy(x => x.ClassName);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult EditSubscription(int id)
        {
            PricingViewModel model = new PricingViewModel();

            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    ViewBag.FacultyList = nqz.Faculties.Where(x => x.IsDeleted == false && x.SchoolID == 1).Select(x => new { x.FacultyID, x.FacultyName }).ToList();

                    Pricing nfac = nqz.Pricings.SingleOrDefault(x => x.IsDeleted == false && x.PricingID == id);

                    model.FacultyID = nfac.FacultyID ?? 0;
                    model.DurationID = nfac.DurationID ?? 0;
                    model.Amount = nfac.Amount ?? 0;
                }
                AllPricing(id);
                return View("Pricings", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DeleteSubscription(int id)
        {
            try
            {
                ViewBag.Msg = null;
                ViewBag.Err = null;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Pricing nfac = nqz.Pricings.SingleOrDefault(x => x.PricingID == id);
                    if (nfac != null)
                    {
                        id = nfac.PricingID;
                        nfac.IsDeleted = true;
                        nfac.Updated = DateTime.Now;
                        nqz.SaveChanges();
                    }
                }

                ViewBag.Msg = "Delete Completed Successfully";
                AllPricing(id);
                return RedirectToAction("Pricings", "Account");

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        #endregion



        #region Payment
        //E-Purse
        [HttpGet]
        public ActionResult PayByEPurse()
        {
            try
            {
                pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //Get Payment Amount
                    Amount = decimal.Parse(Session["PayAmt"].ToString());

                    //Get Purse Amount
                    decimal purse = nqz.Accounts.FirstOrDefault(x => x.PersonID == pid).Purse ?? 0;

                    if (purse < Amount)
                    {
                        ViewBag.Err = "Error! Sorry, there are no enough funds in your Purse to complete this Transaction. Try using another Payment Option.";
                    }
                    else
                    {
                        return RedirectToAction("PaymentResponse", new { reference = "Succcess" });
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

        //PayStack Payment
        [HttpGet]
        public async Task<ActionResult> InitializePayment(SubscriptionViewModel model)
        {
            try
            {
                pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var nper = (from per in nqz.Persons
                                join user in nqz.Users on per.PersonID equals pid
                                where per.PersonID == pid
                                select new
                                {
                                    FName = per.FirstName,
                                    LName = per.LastName,
                                    Email = user.UserName
                                }).FirstOrDefault();
                    if (nper != null)
                    {
                        FirstName = nper.FName;
                        LastName = nper.LName;
                        Email = nper.Email;
                    }
                }

                Amount = decimal.Parse(Session["PayAmt"].ToString());
                //int Amt = int.Parse(Session["Amt"].ToString());
                int Amt = Convert.ToInt32(Amount);
                var vAmt = Amt.ToString() + "00";

                string secretKey = ConfigurationManager.AppSettings["PaystackSecret"];
                var paystackTransactionAPI = new PaystackTransaction(secretKey);
                //var response = await paystackTransactionAPI.InitializeTransaction(Email, int.Parse(vAmt), FirstName, LastName, "http://localhost:56080/Account/PaymentResponse");
                var response = await paystackTransactionAPI.InitializeTransaction(Email, int.Parse(vAmt), FirstName, LastName, "https://braingainspa.com/Account/PaymentResponse");
                //Note that callback url is optional
                if (response.status == true)
                {
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    Response.Redirect(response.data.authorization_url);
                    //return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
                }
                return null;//Json(new { error = true, result = response }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("PaymentResponse", new { reference = "Success" });

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }

        //public async Task<JsonResult> InitializePayment(PaystackCustomerModel model)
        //{
        //    string secretKey = ConfigurationManager.AppSettings["PaystackSecret"];
        //    var paystackTransactionAPI = new PaystackTransaction(secretKey);
        //    var response = await paystackTransactionAPI.InitializeTransaction(model.email, model.amount, model.firstName, model.lastName, "http://localhost:17869/order/callback");
        //    //Note that callback url is optional
        //    if (response.status == true)
        //    {
        //        return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { error = true, result = response }, JsonRequestBehavior.AllowGet);

        //}

        [HttpGet]
        public ActionResult ChangePassword()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var pid = long.Parse(Session["PID"].ToString());
                    User nuser = nqz.Users.FirstOrDefault(x => x.PersonID == pid);
                    if (nuser != null)
                    {
                        if (model.NewPassword == model.ConfirmPassword)
                        {
                            string EncPassword = AppResourceController.Encrypt(model.NewPassword);

                            nuser.Password = EncPassword;
                            nuser.Updated = DateTime.UtcNow;
                            nqz.SaveChanges();

                            ViewBag.Msg = "Alert! Password Change completed successfully. New Password shall reflect at your next login.";
                        }
                        else
                        {
                            ViewBag.Err = "Error! Password mismatch. Please, correct it and try again.";
                        }
                        //}
                        //else
                        //{
                        //    ViewBag.Err = "Error! Password Mismatch. Please, correct it and try again.";
                        //}
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
        public ActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                //var pid = long.Parse(Session["PID"].ToString());
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    User nuser = nqz.Users.FirstOrDefault(x => x.UserName == model.Email);
                    if (nuser != null)
                    {
                        //Get Password
                        string decpass = AppResourceController.Decrypt(nuser.Password);

                        // Send Email
                        AppResourceController nappRes = new AppResourceController();
                        nappRes.SendPasswordMail(model.Email, decpass);

                        ViewBag.Msg = "The password has been sent to the specified Email. Please, login to your Email Account.";
                    }
                    else
                    {
                        ViewBag.Err = "Error! Email does not exist with us. Please, correct it and try again.";
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
        public ActionResult MyCodes()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //var pid = long.Parse(Session["PID"].ToString());
                //Person nper = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                //if (nper != null)
                //{
                //    model.ReferreeCode = nper.RefereeCode;
                //    model.ReferralCode = nper.ReferralCode;
                //    model.ParentCode = nper.ParentCode;
                //}
                //else
                //{
                //    ViewBag.Err = "Error! Password mismatch. Please, correct it and try again.";
                //}
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult MyCodes(MyCodesViewModel model)
        {
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var pid = long.Parse(Session["PID"].ToString());
                    Person nper = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                    if (nper != null)
                    {
                        model.ReferreeCode = nper.RefereeCode;
                        model.ReferralCode = nper.ReferralCode;
                        model.ParentCode = nper.ParentCode;
                    }
                    else
                    {
                        ViewBag.Err = "Error! Password mismatch. Please, correct it and try again.";
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }


        }

        public static class PayStackManager
        {
            public static string MakePayStackPayment(string amt, string email, string refno)
            {
                PayStackResponseModel modal = new PayStackResponseModel();
                PayStackRequestModel reqModel = new PayStackRequestModel();
                //reqModel.amount = "120";
                //var SecKey = string.Format("Bearer {0}", "Paystack secret key");
                //reqModel.email = "customer email";

                //reqModel.callback_url = "Pass url that will be call backed after payment processing";
                //reqModel.reference = "1212"; // Pass unique no or Invoice no
                reqModel.amount = amt;
                var SecKey = string.Format("Bearer {0}", "sk_test_10a20081439e9418fa0ffe8772dbda4518ca747e");
                reqModel.email = email;
                //reqModel.callback_url = "Pass url that will be call backed after payment processing";
                reqModel.callback_url = "http://localhost:56080/Account/PaymentResponse";
                reqModel.reference = refno; // Pass unique no or Invoice no
                var baseAddress = "https://api.paystack.co/transaction/initialize";
                //var baseAddress = "http://localhost:56080/Account/PaymentResponse/";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Headers.Add("Authorization", SecKey);
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                string parsedContent = JsonConvert.SerializeObject(reqModel);
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                var cc = JsonConvert.DeserializeObject(content);
                var cd = JsonConvert.DeserializeObject(cc.ToString());
                modal = JsonConvert.DeserializeObject<PayStackResponseModel>(content);
                return modal.data.authorization_url;
            }

            //string vv = "sk_live_81014ace1ae4bf39a4b01dddfd5a57d6438a6044";
            public static string VerifyPayStackPayment(string RefrenceCode)
            {
                if (!string.IsNullOrEmpty(RefrenceCode))
                {
                    VerifyPayStackResponseModel ResponseModel = new VerifyPayStackResponseModel();
                    var baseAddress = "https://api.paystack.co/transaction/verify/" + RefrenceCode;

                    //var SecKey = string.Format("Bearer {0}", "Your Secret Key");
                    //var SecKey = string.Format("Bearer {0}", "sk_test_10a20081439e9418fa0ffe8772dbda4518ca747e");
                    var SecKey = string.Format("Bearer {0}", ConfigurationManager.AppSettings["PaystackSecret"].ToString());
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                    http.Headers.Add("Authorization", SecKey);
                    http.Accept = "application/json";
                    http.ContentType = "application/json";
                    http.Method = "GET";

                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    ResponseModel = JsonConvert.DeserializeObject<VerifyPayStackResponseModel>(content);
                    if (ResponseModel.data.status.ToLower() == "success")
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Payment verification failed";
                    }
                }
                else
                {
                    return "No Reference code passed";
                }
            }
        }

        #endregion

        #region Downlines Reports
        public ActionResult Downlines()
        {
            try
            {
                TotalDownlines = 0;
                int pid1 = 0, pid2 = 0, pid3 = 0;
                var id = long.Parse(Session["PID"].ToString());
                long levelid = id;
                //int level2 = 0, level23 = 0, level4 = 0;
                //int memcnt = 0;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    //Delete existing records
                    //////////nqz.Downlines.RemoveRange(nqz.Downlines.Where(x => x.PersonID == id));
                    //////////nqz.SaveChanges();

                    //int maxdownlines = nqz.AppSettings.FirstOrDefault(x => x.ID == 1).MaxUnderline ?? 0;
                    var varcodes = nqz.Persons.Where(x => x.PersonID == levelid).Select(x => new { x.PersonID, x.MaxFirstLevel, x.RefereeCode, x.ReferralCode }).FirstOrDefault();

                    //First Level
                    //var firstlevel = nqz.Persons.Where(x => x.ReferralCode == varcodes.RefereeCode).Select(x => new { x.PersonID, x.RefereeCode, Name = x.FirstName + " " + x.LastName }).ToList();
                    var firstlevel = (from per in nqz.Persons
                                      join usr in nqz.Users on per.PersonID equals usr.PersonID
                                      where per.ReferralCode == varcodes.RefereeCode && usr.IsActive == true
                                      select new
                                      {
                                          per.PersonID,
                                          per.RefereeCode,
                                          Name = per.FirstName + " " + per.LastName
                                      }).ToList();
                    TotalDownlines = firstlevel.Count;
                    if (firstlevel.Count > 0)
                    {
                        foreach (var item1 in firstlevel)
                        {
                            string Phone = nqz.Users.FirstOrDefault(x => x.PersonID == item1.PersonID).PhoneNumber;
                            //Save first level members
                            pid1 = SaveDownline(id, item1.Name, 0, Phone);

                            //Second Level
                            var secondlevel = (from per in nqz.Persons
                                               join usr in nqz.Users on per.PersonID equals usr.PersonID
                                               where per.ReferralCode == item1.RefereeCode && usr.IsActive == true
                                               select new
                                               {
                                                   per.PersonID,
                                                   per.RefereeCode,
                                                   Name = per.FirstName + " " + per.LastName
                                               }).ToList();
                            TotalDownlines += secondlevel.Count();
                            if (secondlevel.Count > 0)
                            {
                                foreach (var item2 in secondlevel)
                                {
                                    //Save second level members
                                    pid2 = SaveDownline(id, item2.Name, pid1, null);

                                    //Third Level
                                    var thirdlevel = (from per in nqz.Persons
                                                      join usr in nqz.Users on per.PersonID equals usr.PersonID
                                                      where per.ReferralCode == item2.RefereeCode && usr.IsActive == true
                                                      select new
                                                      {
                                                          per.PersonID,
                                                          per.RefereeCode,
                                                          Name = per.FirstName + " " + per.LastName
                                                      }).ToList();
                                    TotalDownlines += thirdlevel.Count();
                                    if (thirdlevel.Count > 0)
                                    {
                                        foreach (var item3 in thirdlevel)
                                        {
                                            //Save third level members
                                            pid3 = SaveDownline(id, item3.Name, pid2, null);

                                            //Fourth Level
                                            var fourthlevel = (from per in nqz.Persons
                                                               join usr in nqz.Users on per.PersonID equals usr.PersonID
                                                               where per.ReferralCode == item3.RefereeCode && usr.IsActive == true
                                                               select new
                                                               {
                                                                   per.PersonID,
                                                                   per.RefereeCode,
                                                                   Name = per.FirstName + " " + per.LastName
                                                               }).ToList();
                                            TotalDownlines += fourthlevel.Count();
                                            if (fourthlevel.Count > 0)
                                            {
                                                foreach (var item4 in fourthlevel)
                                                {
                                                    //Save fourth level members
                                                    SaveDownline(id, item4.Name, pid3, null);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                    List<Downline> all = new List<Downline>();

                    all = nqz.Downlines.Where(x => x.PersonID == id).OrderBy(a => a.Pid).ToList();
                    ViewBag.TotalDownlines = TotalDownlines;
                    return View(all);
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        private int SaveDownline(long personid, string name, int? id, string description)
        {

            try
            {
                //long levelid = personid;
                //int i = 0;

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    Downline downline = nqz.Downlines.FirstOrDefault(x => x.PersonID == personid && x.Name == name && x.Pid == id);
                    if (downline == null)
                    {
                        Downline ndownline = new Downline()
                        {
                            PersonID = personid,
                            Name = name,
                            Pid = id,
                            Description = description
                        };
                        nqz.Downlines.Add(ndownline);
                        nqz.SaveChanges();

                        return ndownline.ID;
                    }
                    else
                    {
                        return downline.ID;
                    }


                }


            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return 0;//View("Error");
            }

        }

        #endregion

        public async Task<int> SetAccounts(long personid)
        {
            try
            {
                decimal Withdrawals = 0, Earnings = 0, Purse = 0, Admin2 = 0, CompEarnings = 0;
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    if (personid > 0)
                    {
                        var refcode = nqz.Persons.FirstOrDefault(x => x.PersonID == personid).RefereeCode;
                        Earnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.TempFlag == true && x.ToCompanyFlag == false).Select(a => a.Amount).Sum() ?? 0;
                        Purse = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.TempFlag == false && x.ToCompanyFlag == false).Select(a => a.Amount).Sum() ?? 0;
                        Withdrawals = Earnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.Comment == "Withdrawal").Select(a => a.Amount).Sum() ?? 0;

                        if (Withdrawals > 0)
                        {
                            Purse -= Withdrawals;
                        }


                        Session["Withdrawals"] = Withdrawals.ToString("#,##0.00");
                        Session["Earnings"] = Earnings.ToString("#,##0.00");
                        Session["Purse"] = Purse.ToString("#,##0.00");

                        Account account = nqz.Accounts.FirstOrDefault(x => x.PersonID == personid);
                        if (account == null)
                        {
                            Account acct = new Account()
                            {
                                PersonID = personid,
                                Purse = Purse,
                                TempPurse = Earnings,
                                Withdrawals = Withdrawals,
                                Admin2 = 0,
                                IsCompany = false,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                            };
                            nqz.Accounts.Add(acct);
                            await nqz.SaveChangesAsync();

                        }
                        else
                        {
                            account.Purse = Purse;
                            account.TempPurse = Earnings;
                            account.Withdrawals = Withdrawals;
                            account.Admin2 = 0;
                            account.IsCompany = false;
                            account.Updated = DateTime.UtcNow;
                            await nqz.SaveChangesAsync();
                        }


                    }
                    else
                    {
                        var refcode = nqz.Persons.FirstOrDefault(x => x.IsCompany == true).RefereeCode;
                        Purse = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.TempFlag == false && x.Comment == "Company").Select(a => a.Amount).Sum() ?? 0;
                        Earnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.TempFlag == true && x.Comment == "Rewards").Select(a => a.Amount).Sum() ?? 0;
                        Withdrawals = Earnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.Comment == "Admin1").Select(a => a.Amount).Sum() ?? 0;
                        Admin2 = Earnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.Comment == "Admin2").Select(a => a.Amount).Sum() ?? 0;
                        CompEarnings = nqz.PaymentTransactions.Where(x => x.ReferralCode == refcode && x.Comment != "Company" && x.Comment != "Rewards" && x.Comment != "Admin1" && x.Comment != "Admin2").Select(a => a.Amount).Sum() ?? 0;

                        Purse += CompEarnings;

                        Account account = nqz.Accounts.FirstOrDefault(x => x.PersonID == personid);
                        account.Purse = Purse; //Company
                        account.TempPurse = Earnings; //Reward
                        account.Withdrawals = Withdrawals; //Admin1
                        account.Admin2 = Admin2; //Admin2
                        account.Updated = DateTime.UtcNow;

                        await nqz.SaveChangesAsync();


                    }


                }

                return 1;
            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return 0;//View("Error");
            }

        }

        [HttpGet]
        public ActionResult UserInfo()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //var pid = long.Parse(Session["PID"].ToString());
                //Person nper = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                //if (nper != null)
                //{
                //    model.ReferreeCode = nper.RefereeCode;
                //    model.ReferralCode = nper.ReferralCode;
                //    model.ParentCode = nper.ParentCode;
                //}
                //else
                //{
                //    ViewBag.Err = "Error! Password mismatch. Please, correct it and try again.";
                //}
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Income()
        {
            //if (Session.Count <= 0)
            //{
            //    return RedirectToAction("Signin", "Account");
            //}

            try
            {
                var PId = long.Parse(Session["PID"].ToString());

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    string refereecode = nqz.Persons.FirstOrDefault(x => x.PersonID == PId).RefereeCode;

                    var qry = (from per in nqz.Persons
                               join pmt in nqz.PaymentTransactions
                                on per.PersonID equals pmt.PersonID
                               join ttp in nqz.TransactionTypes
                               on pmt.TransactionTypeID equals ttp.TransactionTypeID
                               where pmt.ReferralCode == refereecode
                               orderby pmt.PaymentTransactionID descending
                               select new IncomeViewModel
                               {
                                   PaymentTransactionID = pmt.PaymentTransactionID,
                                   Payer = per.LastName + " " + per.FirstName,
                                   TransactionTypeName = ttp.TransactionTypeName,
                                   Amount = pmt.Amount ?? 0,
                                   Comment = pmt.Comment,
                                   Created = pmt.Created ?? DateTime.Now
                               }).Take(10).ToList();

                    ViewBag.IncomeList = qry.ToList();
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Income(long id)
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {

                    //var qry = (from mod in nqz.PaymentModes
                    //           join clt in nqz.ClientPayments
                    //            on mod.PaymentModeID equals clt.PaymentModeID
                    //           where clt.PersonID == id
                    //           orderby clt.ClientPaymentID descending
                    //           select new
                    //           {
                    //               clt.ClientPaymentID,
                    //               PayMode = mod.PaymentModeName,
                    //               clt.Amount,
                    //               clt.Description,
                    //               clt.Created
                    //           }).Take(10).ToList();

                    //ViewBag.PaymentList = qry;
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
        public ActionResult Payments()
        {

            try
            {
                var PId = long.Parse(Session["PID"].ToString());

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {


                    var qry = (from mod in nqz.PaymentModes
                               join clt in nqz.ClientPayments
                                on mod.PaymentModeID equals clt.PaymentModeID
                               where clt.PersonID == PId
                               orderby clt.ClientPaymentID descending
                               select new ClientPaymentViewModel
                               {
                                   ClientPaymentID = clt.ClientPaymentID,
                                   PayMode = mod.PaymentModeName,
                                   Amount = clt.Amount ?? 0,
                                   Description = clt.Description,
                                   Created = clt.Created ?? DateTime.Now
                               }).Take(10).ToList();

                    ViewBag.PaymentList = qry;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Payments(int id)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //var pid = long.Parse(Session["PID"].ToString());
                //Person nper = nqz.Persons.FirstOrDefault(x => x.PersonID == pid);
                //if (nper != null)
                //{
                //    model.ReferreeCode = nper.RefereeCode;
                //    model.ReferralCode = nper.ReferralCode;
                //    model.ParentCode = nper.ParentCode;
                //}
                //else
                //{
                //    ViewBag.Err = "Error! Password mismatch. Please, correct it and try again.";
                //}
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }

        }
    }

}
