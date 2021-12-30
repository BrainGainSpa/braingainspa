using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Paystack.Net.SDK.Transactions;
using braingainspa.Models;
using System.Web.UI;
using System.Net.Http;

namespace braingainspa.Controllers
{
    public class AppAsyncController : AsyncController
    {
        // GET: AppAsync
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SubscriptionsPreview(SubscriptionPreviewViewModel model)
        {
            //DBQUIZZEntities nqz = new DBQUIZZEntities();
            //CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            try
            {
                //if (ModelState.IsValid)
                //{

                //}

                TempData["Subscription"] = model;

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

            return View();
        }


        [HttpPost]
        public ActionResult SubscriptionsPreview(SubscriptionPreviewViewModel model, int num = 0)
        {

            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            var nprc = (from prc in nqz.Pricings
                        join grd in nqz.Faculties on prc.FacultyID equals grd.FacultyID
                        join dur in nqz.DurationInMonths on prc.DurationID equals dur.DurationID
                        where prc.IsDeleted == false && prc.SchoolID == 1 && prc.PricingID == model.PricingID
                        select new
                        {

                            Grade = grd.FacultyName,
                            PriceID = prc.PricingID,
                            Price = prc.Amount,
                            Period = dur.Duration,
                            Duration = dur.Description
                        }).FirstOrDefault();
            if (nprc != null)
            {
                //model.PersonID = pid;
                model.PricingID = nprc.PriceID;
                model.Class = nprc.Grade;
                model.Amount = nprc.Price ?? 0;
                model.Duration = nprc.Duration;
                model.StartDate = DateTime.UtcNow;
                model.EndDate = DateTime.UtcNow.AddDays(double.Parse(nprc.Period.ToString()) * 30);

            }

            //model = TempData["Subscription"] as SubscriptionPreviewViewModel;


            try
            {
                Subscription nsubscriptn = new Subscription
                {
                    PersonID = model.PersonID,
                    PricingID = model.PricingID,
                    Amount = model.Amount,
                    IsActive = false,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow

                };

                //nqz.Subscriptions.Add(nsubscriptn);
                //nqz.SaveChanges();

                string email = TempData["Email"].ToString();

                //var result = PayStackManager.MakePayStackPayment(model.Amount.ToString(), email, "1");
                //new PageAsyncTask(InitTransaction);
                //return Redirect(result);


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



            return null;
        }

        #region PayStack
        //private async Task InitTransaction()
        //{
        //var paystackAPI = new Paystack.Net.SDK.Transactions.PaystackTransaction("sk_live_3cee2f0fd8e9300d73bfa2669097b8234f22008f");//LIVE
        ////var paystackAPI = new Paystack.Net.SDK.Transactions.PaystackTransaction("sk_test_10a20081439e9418fa0ffe8772dbda4518ca747e");//TEST
        //Session["Referal"] = RefEmail.Text;
        //Session["NoRefCode"] = "F";
        ////if (chkrefcode.Checked)
        ////{
        ////    Session["NoRefCode"] = "T";
        ////}
        //string[] regarr = Session["REGDETAILS"].ToString().Split('|');
        //string stramt = lblAmount.Text;

        ////Response.Redirect("../Investors/PaymentResponse");

        //if (lblAmount.Text.Contains(","))
        //{
        //    stramt = lblAmount.Text.Replace(",", "");
        //}
        //int intamt = int.Parse(stramt.ToString().Replace(".", ""));
        ////var response = await paystackAPI.InitializeTransaction(regarr[3], intamt, regarr[0], regarr[1], "http://localhost:3821/Investors/PaymentResponse.aspx", "http://localhost:3821/Investors/PaymentResponse.aspx");
        //    var response = await paystackAPI.InitializeTransaction(regarr[3], intamt, regarr[0], regarr[1], "https://www.brainixe.com/Investors/PaymentResponse.aspx", "https://www.brainixe.com/Investors/PaymentResponse.aspx");

        //    if (response.status)
        //    {
        //        Response.AddHeader("Access-Control-Allow-Origin", "*");
        //        Response.AppendHeader("Access-Control-Allow-Origin", "*");
        //        Response.Redirect(response.data.authorization_url);
        //    }
        //}

        #endregion

        #region NEWSMS SETTINGS
        static HttpClient client = new HttpClient();
        static async Task<String> GetSendSmsAsync(string path)
        {
            String responseStr = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    responseStr = await response.Content.ReadAsStringAsync();
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                return responseStr;
            }

        }

        static void ShowResult(String result)
        {
            Console.WriteLine(result);
        }

        public static async Task<string> RunAsync(string strmsg, string strno)
        {
            try
            {
                
            // Make a get request
            String apiresponse = await GetSendSmsAsync("http://api.ebulksms.com:8080/sendsms?username=tijeboi@yahoo.com&apikey=f4defa8a453f6c1248f800a66ca67c75b60431da&sender=BGSPA&messagetext=" + strmsg + "&flash=0&recipients=" + strno);
                //ShowResult(apiresponse);
                return apiresponse;
            }
            catch (Exception ex)
            {
                return ex.StackTrace;
            }
        }

        #endregion
    }
}