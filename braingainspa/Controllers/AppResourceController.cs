using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using System.Threading.Tasks;
using Newtonsoft.Json;
using braingainspa.Models;

namespace braingainspa.Controllers
{
    public class AppResourceController : Controller
    {
        private static byte[] key = { };
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string EncryptionKey = "qazwsxedc";
        // GET: AppResource
        public DataTable LINQToDataTable<t>(IEnumerable<t> varlist)
        {
            DataTable dtReturn = new DataTable();

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

        private string PopulateEmailBody(int src, string name, string email = null, string password = null, string refcode = null, string parcode = null, string id = null, string item = null, string mode = null, string amount = null, string url = null)
        {
            //src: 1 == Registration; 2 == Payment
            string body = string.Empty, filePath = string.Empty;
            try
            {
                if (src == 1)
                {
                    filePath = HostingEnvironment.MapPath("~/Content/DOCS/RegDoc.html");
                    //using (StreamReader reader = new StreamReader(VirtualPathUtility.ToAbsolute("~/Content/DOCS/RegDoc.html")))
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{FullName}", name);
                    body = body.Replace("{Date}", DateTime.Now.ToLongDateString());
                    body = body.Replace("{Email}", email);
                    body = body.Replace("{Password}", password);
                    body = body.Replace("{RefCode}", refcode);
                    if (parcode == "0" || parcode == "null")
                    {
                        parcode = "N/A";
                    }
                    body = body.Replace("{ParCode}", parcode);

                    body = body.Replace("{url}", url + Encrypt(id));
                    //string purl = ConfigurationManager.AppSettings["DomainHeader"].ToString() + Request.ServerVariables["HTTP_HOST"] + "/Account/SignupSuccess?key=";
                    //string purl = ConfigurationManager.AppSettings["DomainHeader"].ToString() + Request.ServerVariables["HTTP_HOST"] + "/Account/Activation?key=";

                }
                else if (src == 2)
                {
                    filePath = HostingEnvironment.MapPath("~/Content/DOCS/PayDoc.htm");
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{FullName}", name);
                    body = body.Replace("{Item}", item);
                    body = body.Replace("{PayMode}", mode);
                    body = body.Replace("{Date}", DateTime.Now.ToLongDateString());
                    body = body.Replace("{Amount}", amount);
                }
                else if (src == 3)
                {
                    filePath = HostingEnvironment.MapPath("~/Content/DOCS/PasswordDoc.html");
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{Password}", password);
                    body = body.Replace("{Date}", DateTime.Now.ToLongDateString());
                }


            }
            catch (Exception ex)
            {

            }


            return body;

        }

        public void SendRegistrationMail(string email, string name, string password, string refcode, string parcode, string id, string url = null)
        {
            string body = this.PopulateEmailBody(1, name, email, password, refcode, parcode, id, "", "", "", url);
            SendMail(email.Trim(), "Braingainspa Registration Notification", body);
        }

        public void SendPaymentMail(string customername, string email, string Item, string mode, string amount, string password, int userid)
        {
            string body = this.PopulateEmailBody(2, customername, Item, mode, amount);

            SendMail(email.Trim(), "Braingainspa Payment Notification", body);
        }
        public void SendPasswordMail(string email, string password)
        {
            string body = this.PopulateEmailBody(3, "", email, password);

            SendMail(email.Trim(), "Braingainspa Password Notification", body);
        }
        public static void SendMail(string UserTo, string Subject, string Body)
        {
            try
            {
                //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                //MailMessage email_msg = new MailMessage();
                //email_msg.To.Add(UserTo);
                //email_msg.From = new MailAddress("cozar.microsystems@gmail.com ", "braingainspa");

                //SmtpClient smtp = new SmtpClient("mail.brainixe.com", 8889);
                //MailMessage email_msg = new MailMessage();
                //email_msg.To.Add(UserTo);
                //email_msg.From = new MailAddress("customerservices@brainixe.com", "Web Quiz");

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                MailMessage email_msg = new MailMessage();
                email_msg.To.Add(UserTo);
                email_msg.From = new MailAddress("braingainintl@gmail.com", "Braingainspa");

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                email_msg.AlternateViews.Add(htmlView);
                email_msg.Subject = Subject;
                email_msg.Body = Body;
                email_msg.IsBodyHtml = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new NetworkCredential("braingainintl@gmail.com ", "Captilla1");
                //smtp.Credentials = new NetworkCredential("cozar.microsystems@gmail.com", "coss1222");
                //smtp.Credentials = new NetworkCredential("customerservices@brainixe.com", "brainserv12@*");
                //smtp.EnableSsl = false;
                smtp.EnableSsl = true;
                smtp.Send(email_msg);

            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {

            }
        }

        public static int SendSMS(string cntrycode, string strnumber, string strMsg)
        {
            string RStatus = "";
            string URLResponseString;
            string sendder = cntrycode;

            //if (strnumber.Length == 11)
            //{
            //    strnumber = strnumber.Remove(0, 1);
            //}

            try
            {
                if (strnumber.Substring(0, 1) == "0")
                {
                    string nnum = "234" + strnumber.Substring(1);
                    strnumber = nnum;
                }
                else if (strnumber.Substring(0, 3) == "234")
                {
                    //string nnum = "+" + strnumber.Substring(0);
                    //strnumber = nnum;
                }
                else
                {
                    strnumber = "234" + strnumber;
                }
                //if (strnumber.Substring(0, 1) == "0")
                //{
                //    string nnum = "+234" + strnumber.Substring(1);
                //    strnumber = nnum;
                //}
                //else if (strnumber.Substring(0, 3) == "234")
                //{
                //    string nnum = "+" + strnumber.Substring(0);
                //    strnumber = nnum;
                //}
                //else
                //{
                //    strnumber = "+234" + strnumber;
                //}
                URLResponseString = GetHttpResponse("http://api.ibulky.com/sendsms/?apikey=6af75e4bb4de3a4cd5c28c60-a9a20fd&sender=BGSpa&recipient=" + strnumber + "&message=" + strMsg + "&msgtype=text&delivery=yes");

                //string ff = "https://konnect.dotgo.com/api/v1/Accounts/_qqQN136N4wb9TNrV9kAeQ==/Messages?id=1235&to=" + strnumber + "&body=" + strMsg + "&sender_mask=BGSPa&priority=high&api_token=Z9yOQke2aJ1DSBbmjrhHsgyUse_AnSmlxZD024lx1F4=";
                //URLResponseString = GetHttpResponse(ff);
                //URLResponseString = GetHttpResponse("https://konnect.dotgo.com/api/v1/Accounts/_qqQN136N4wb9TNrV9kAeQ==/Messages?id=1234&to=" + strnumber + "&body=" + strMsg + "&sender_mask=BGSPa&priority=high&api_token=Z9yOQke2aJ1DSBbmjrhHsgyUse_AnSmlxZD024lx1F4=");

                ////URLResponseString = GetHttpResponse("http://portal.nigeriabulksms.com/api/?username=creditchex&password=Nnemere1&message=SMSTEST&sender=CreditChex&mobiles=2348033667378");
                //URLResponseString = GetHttpResponse("https://account.smskit.net/smsAPI?sendsms&apikey=yNaw5DdrJYrnMValFvlgdFKwmfU1ptFr&apitoken=ijU61577539385&type=sms&from=BGSpa&to=" + strnumber + "&text=" + strMsg);
                //RStatus = URLResponseString.Substring(35, 6);
                //if (URLResponseString.Contains("ok"))
                if (URLResponseString.Contains("2501"))
                {
                    //Return "Not Sent"
                    return 1;
                }
                else
                {
                    //Return "Sent"
                    return 2;
                }
                //if (RStatus == "queued")
                //{
                //    //Return "Sent"
                //    return 1;
                //}
                //else
                //{
                //    //Return "Not Sent"
                //    return 2;
                //}
            }
            catch (Exception ex)
            {
                return 2;

            }
        }

        private static string GetHttpResponse(string url)
        {

            //url= HttpUtility.UrlEncode(url);
            //  url = HttpUtility.UrlDecode(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string resp = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return resp;

        }

        #region encrypt-decrypt

        public static string Decrypt(string Input)
        {
            Byte[] inputByteArray = new Byte[Input.Length];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(Input);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;

                return encoding.GetString(ms.ToArray());


            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public static string Encrypt(string Input)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(Input);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());

            }

            catch (Exception ex)
            {
                return "";
            }

        }

        public static string Encrypt_newCor(string Input)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(Input);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());


            }

            catch (Exception ex)
            {
                return "";
            }

        }

        #endregion


        #region

        [AllowAnonymous]
        private long GetSponsor(int downlinecnt, int totalmembership) //downlinecnt = 5, totalmembership = 781
        {

            long id = 0;
            int SponsorCnt = 0;
            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
            {
                int cnt = nqz.Persons.Select(x => x.PersonID == id).Count();
                if (cnt >= 1)
                {
                    do
                    {
                        id++;
                        //SponsorCnt = nqz.Persons.Select(x => x.ReferralID == id).Count();

                    } while (SponsorCnt >= downlinecnt);
                }
            }
            return id;
        }

        [AllowAnonymous]
        private long GetDownlines(int netid, int downlinecnt, int totalmembership) //downlinecnt = 4, totalmembership = 781
        {

            long id = 0;
            int SponsorCnt = 0;
            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
            {
                int cnt = nqz.Persons.Select(x => x.PersonID == id).Count();
                if (cnt >= 1)
                {
                    do
                    {
                        id++;
                        //SponsorCnt = nqz.Persons.Select(x => x.ReferralCode == id).Count();

                    } while (SponsorCnt >= downlinecnt);
                }
            }
            return id;
        }

        #endregion

    }
}
