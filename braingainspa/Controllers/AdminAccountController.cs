using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using braingainspa.Models;
using braingainspa.Controllers;
using System.Threading.Tasks;

namespace braingainspa.Controllers
{
    public class AdminAccountController : Controller
    {
        private CArumala_edquizEntities db = new CArumala_edquizEntities();

        // GET: AdminAccount
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: AdminAccount/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: AdminAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,PersonID,Purse,TempPurse,Withdrawals,IsCompany,Created,Updated,Admin2")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: AdminAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: AdminAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,PersonID,Purse,TempPurse,Withdrawals,IsCompany,Admin2")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: AdminAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: AdminAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult AppSettings()
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;
            ViewBag.ErrorMessage = null;
            try
            {
                AppSettingsViewModel nappsetting = new AppSettingsViewModel();
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    AppSetting apset = nqz.AppSettings.FirstOrDefault(x => x.ID == 1);
                    nappsetting.RegistrationFee = apset.RegistrationFee ?? 0;
                    nappsetting.DefaultReferralCode = apset.DefaultReferralCode;
                    nappsetting.MaxUnderline = apset.MaxUnderline ?? 0;
                    nappsetting.QuestionsBatches = apset.QuestionsBatches ?? 0;
                    nappsetting.MaximumOptions = apset.MaximumOptions ?? 0;
                    nappsetting.MockPeriod = apset.MockPeriod ?? 0;
                    nappsetting.SupportPhone = apset.SupportPhone;
                    nappsetting.QuizPeriod = apset.QuizPeriod ?? 0;
                    nappsetting.PRYQuizPeriod = apset.PRYQuizPeriod ?? 0;
                    nappsetting.JSSQuizPeriod = apset.JSSQuizPeriod ?? 0;
                    nappsetting.SSSQuizPeriod = apset.SSSQuizPeriod ?? 0;

                    return View(nappsetting);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }

        }

        [HttpPost]
        public async Task<ActionResult> AppSettings(AppSettingsViewModel nappsetting)
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;
            ViewBag.ErrorMessage = null;
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    AppSetting apset = nqz.AppSettings.FirstOrDefault(x => x.ID == 1);
                    apset.RegistrationFee = nappsetting.RegistrationFee;
                    apset.DefaultReferralCode = nappsetting.DefaultReferralCode;
                    apset.MaxUnderline = nappsetting.MaxUnderline;
                    apset.QuestionsBatches = nappsetting.QuestionsBatches;
                    apset.MaximumOptions = nappsetting.MaximumOptions;
                    apset.MockPeriod = nappsetting.MockPeriod;
                    apset.SupportPhone = nappsetting.SupportPhone;
                    apset.QuizPeriod = nappsetting.QuizPeriod;
                    apset.PRYQuizPeriod = nappsetting.PRYQuizPeriod;
                    apset.JSSQuizPeriod = nappsetting.JSSQuizPeriod;
                    apset.SSSQuizPeriod = nappsetting.SSSQuizPeriod;

                    await nqz.SaveChangesAsync();

                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }

        }

        [HttpGet]
        public ActionResult AdminAccounts()
        {
            ViewBag.Err = null;
            ViewBag.Msg = null;
            ViewBag.ErrorMessage = null;

            try
            {
                //var PId = long.Parse(Session["PID"].ToString());

                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    var result = new AccountController().SetAccounts(0);

                    var npers = nqz.Persons.Where(x => x.IsCompany == true).Select( y => new { y.PersonID, y.RefereeCode }).FirstOrDefault();

                    var qry = (from tt in nqz.TransactionTypes
                               join ptrans in nqz.PaymentTransactions
                                on tt.TransactionTypeID equals ptrans.TransactionTypeID
                               where ptrans.ReferralCode == npers.RefereeCode
                               orderby ptrans.PaymentTransactionID descending
                               select new AdminTransactionViewModel
                               {
                                   PaymentTransactionID = ptrans.PaymentTransactionID,
                                   TransactionName = tt.TransactionTypeName,
                                   Amount = ptrans.Amount ?? 0,
                                   Comment = ptrans.Comment,
                                   TransDate = ptrans.Created ?? DateTime.Now
                               }).ToList();
                    //}).Take(30).ToList();
                    ViewBag.AdminPaymentList = qry;

                    Account account = nqz.Accounts.FirstOrDefault(x => x.PersonID == npers.PersonID);
                    if(account != null)
                    {
                        TempData["Company"] = account.Purse;
                        TempData["Rewards"] = account.TempPurse;
                        TempData["Admin1"] = account.Withdrawals;
                        TempData["Admin2"] = account.Admin2;
                    }

                }

                


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrMessage = ex.Message + " Stack " + ex.StackTrace;
                return View("Error");
            }
            finally
            {

            }
            //try
            //{
            //    AdminAccountViewModel adminAccountView = new AdminAccountViewModel();
            //    using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
            //    {
            //        Account account = nqz.Accounts.FirstOrDefault(x => x.IsCompany == true);
            //        adminAccountView.Purse = account.Purse ?? 0;
            //        adminAccountView.TempPurse = account.TempPurse ?? 0;
            //        adminAccountView.Withdrawals = account.Withdrawals ?? 0;
            //        adminAccountView.Admin2 = account.Admin2 ?? 0;

            //        return View(adminAccountView);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
            //    return View("Error");
            //}
            //finally
            //{

            //}

        }
    }
}
