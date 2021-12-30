using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Web;
using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using braingainspa.Models;
using System.Threading.Tasks;
//using braingainspa.Pdf;

namespace braingainspa.Controllers
{
    //[Authorize]
    public class UtilityController : Controller
    {
        long CompID = 0, MemID = 0;
        string F_IDstring = null;
        //
        // GET: /Utility/
        public ActionResult Index()
        {
            //VientiDBEntities vdb = new VientiDBEntities();

            //List<BANK> banklist = vdb.BANKS.ToList();
            //ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK");

            return View();
        }

        //GET
        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult> Bank_Info(BankInfoViewModels model)
        {

            try
            {
                ViewBag.BankList = Session["BankList"];
                if (model.BANK_ID <= 0)
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

        [AllowAnonymous]
        public ActionResult AddEditBank()
        //public ActionResult AddEditBank()
        {

            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            BankInfoViewModels model = new BankInfoViewModels();
            try
            {
                List<BANK> banklist = nqz.BANKS.ToList();
                ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");

                //if (bankinfoid > 0)
                //{
                //    var PerID = Session["PersonID"].ToString();

                //    BankInfo nBank = nqz.BankInfos.SingleOrDefault(x => x.BankInfoID == bankinfoid && x.IsDeleted == false);
                //    model.BankInfoID = nBank.BankInfoID;
                //    //nBank.PersonID = long.Parse(PerID);//int.Parse(Session["PersonID"].ToString());
                //    model.BANK_ID = nBank.BankID;
                //    model.BranchName = nBank.BranchName;
                //    model.AccountName = nBank.AccountName;
                //    model.AccountNumber = nBank.AccountNumber;

                //}
                //else
                //{
                //    BankInfo nBank = new BankInfo();
                //    nBank.PersonID = int.Parse(Session["PersonID"].ToString());
                //    nBank.BankID = model.BANK_ID;
                //    nBank.BranchName = model.BranchName;
                //    nBank.AccountName = model.AccountName;
                //    nBank.AccountNumber = model.AccountNumber;
                //    nBank.IsDeleted = false;
                //    nBank.Created = DateTime.Now;
                //    nBank.Updated = DateTime.Now;
                //    nqz.BankInfos.Add(nBank);
                //    nqz.SaveChanges();
                //}

                return PartialView("_AddBankInfoPartial", model);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [AllowAnonymous]
        public ActionResult ShowBankInfo(int bankinfoid)
        {

            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            BankInfoViewModels model = new BankInfoViewModels();
            try
            {
                //List<BankInfoViewModels> bnklist = nqz.BankInfos.Where(x => x.IsDeleted == false && x.BankInfoID == bankinfoid).Select(x => new BankInfoViewModels { BankInfoID = x.BankInfoID, BankName = x.BANK.BANK1, BranchName = x.BranchName, AccountName = x.AccountName, AccountNumber = x.AccountNumber }).ToList();

                //ViewBag.BankInfoList = bnklist;


                return PartialView("_ViewBankInfoPartial", model);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [AllowAnonymous]
        public JsonResult DeleteBankInfo(int bankinfoid)
        {

            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            try
            {
                bool result = false;
                BankInfo nBank = nqz.BankInfos.SingleOrDefault(x => x.BankInfoID == bankinfoid && x.IsDeleted == false);
                if (nBank != null)
                {
                    nBank.IsDeleted = true;
                    nqz.SaveChanges();
                    result = true;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //GET
        //[AllowAnonymous]
        //public ActionResult ManagePackages()
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //    List<Network> nlist = nqz.Networks.ToList();

        //    ViewBag.PkgList = nlist;


        //    return View();
        //}

        //POST
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult ManagePackages(ManagePackageViewModel model)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //    //List<BANK> banklist = nqz.BANKS.ToList();
        //    //ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");

        //    if (model.NetworkID > 0)
        //    {
        //        Network nNetWk = nqz.Networks.SingleOrDefault(x => x.NetworkID == model.NetworkID && x.IsDeleted == false);
        //        nNetWk.NetworkID = model.NetworkID;
        //        nNetWk.Name = model.Name;
        //        nNetWk.StartupAmount = model.StartupAmount;
        //        nNetWk.MaxCycle = model.MaxCycle;
        //        nNetWk.MaxUnderlines = model.MaxUnderlines;
        //        nNetWk.Level1 = model.Level1;
        //        nNetWk.Level2 = model.Level2;
        //        nNetWk.Level3 = model.Level3;
        //        nNetWk.BonusAmount = model.BonusAmount;
        //        nNetWk.CompanyAmount = model.CompanyAmount;
        //        nNetWk.Updated = DateTime.Now.Date;
        //        nqz.SaveChanges();
        //    }
        //    else
        //    {
        //        Network nNetWk = new Network();
        //        nNetWk.Name = model.Name;
        //        nNetWk.StartupAmount = model.StartupAmount;
        //        nNetWk.MaxCycle = model.MaxCycle;
        //        nNetWk.MaxUnderlines = model.MaxUnderlines;
        //        nNetWk.Level1 = model.Level1;
        //        nNetWk.Level2 = model.Level2;
        //        nNetWk.Level3 = model.Level3;
        //        nNetWk.BonusAmount = model.BonusAmount;
        //        nNetWk.CompanyAmount = model.CompanyAmount;
        //        nNetWk.Created = DateTime.Now.Date;
        //        nNetWk.Updated = DateTime.Now.Date;
        //        nNetWk.IsDeleted = false;
        //        nqz.Networks.Add(nNetWk);
        //        nqz.SaveChanges();
        //    }

        //    return View(model);
        //}


        //[AllowAnonymous]
        //public ActionResult AddEditPackage(int networkid)
        //{

        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    ManagePackageViewModel model = new ManagePackageViewModel();
        //    try
        //    {
        //        if (networkid > 0)
        //        {
        //            Network nNetWk = nqz.Networks.SingleOrDefault(x => x.NetworkID == networkid && x.IsDeleted == false);
        //            model.NetworkID = nNetWk.NetworkID;
        //            model.Name = nNetWk.Name;
        //            model.StartupAmount = nNetWk.StartupAmount;
        //            model.MaxCycle = nNetWk.MaxCycle;
        //            model.MaxUnderlines = nNetWk.MaxUnderlines;
        //            model.Level1 = nNetWk.Level1;
        //            model.Level2 = nNetWk.Level2;
        //            model.Level3 = nNetWk.Level3;
        //            model.BonusAmount = nNetWk.BonusAmount;
        //            model.CompanyAmount = nNetWk.CompanyAmount;

        //        }
        //        //List<ManagePackageViewModel> Pkglist = nqz.Networks.Where(x => x.IsDeleted == false).Select(x => new ManagePackageViewModel { NetworkID = x.NetworkID, Name = x.Name, StartupAmount = x.StartupAmount, MaxCycle = x.MaxCycle, MaxUnderlines = x.MaxUnderlines }).ToList();

        //        //ViewBag.PackageList = Pkglist;

        //        return PartialView("_AddPackagePartial", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        //[AllowAnonymous]
        //public ActionResult ShowPackage(int networkid)
        //{

        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    ManagePackageViewModel model = new ManagePackageViewModel();
        //    try
        //    {
        //        List<ManagePackageViewModel> Pkglist = nqz.Networks.Where(x => x.IsDeleted == false && x.NetworkID == networkid).Select(x => new ManagePackageViewModel { NetworkID = x.NetworkID, Name = x.Name, StartupAmount = x.StartupAmount, MaxCycle = x.MaxCycle, MaxUnderlines = x.MaxUnderlines, Level1 = x.Level1, Level2 = x.Level2, Level3 = x.Level3, BonusAmount = x.BonusAmount, CompanyAmount = x.CompanyAmount }).ToList();

        //        ViewBag.PackageList = Pkglist;


        //        return PartialView("_ViewPackagePartial", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        //[AllowAnonymous]
        //public JsonResult DeletePackage(int networkid)
        //{

        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //    try
        //    {
        //        bool result = false;
        //        Network nNetWk = nqz.Networks.SingleOrDefault(x => x.NetworkID == networkid && x.IsDeleted == false);
        //        if (nNetWk != null)
        //        {
        //            nNetWk.IsDeleted = true;
        //            vdb.SaveChanges();
        //            result = true;
        //        }

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}
        ////GET
        //public ActionResult Payments()
        //{
        //    VientiDBEntities vdb = new VientiDBEntities();

        //    List<BANK> banklist = vdb.BANKS.ToList();
        //    ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");



        //    return View();
        //}

        ////POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Payments(PaymentViewModel model)
        //{
        //    VientiDBEntities vdb = new VientiDBEntities();

        //    List<BANK> banklist = vdb.BANKS.ToList();
        //    ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");

        //    //List<BankInfoViewModels> Banklist = vdb.BankInfos.Where(x => x.IsDeleted == false && x.PersonID.Equals(Session["PersonID"].ToString())).Select(x => new BankInfoViewModels { BankInfoID = x.BankInfoID, BankName = x.BANK.BANK1, AccountName = x.AccountName, AccountNumber = x.AccountNumber, BranchName = x.BranchName }).ToList();

        //    //ViewBag.BankList = Banklist;

        //    //BankInfo nbankinfo = new BankInfo();
        //    //nbankinfo.PersonID = int.Parse(Session["PersonID"].ToString());//model.PersonID;
        //    //nbankinfo.BankID = model.BankID;
        //    //nbankinfo.AccountName = model.AccountName;
        //    //nbankinfo.AccountNumber = model.AccountNumber;
        //    //nbankinfo.BranchName = model.BranchName;
        //    //nbankinfo.Created = DateTime.Now.Date;
        //    //nbankinfo.Updated = DateTime.Now.Date;

        //    //vdb.BankInfos.Add(nbankinfo);
        //    //vdb.SaveChanges();






        //    return View();
        //}

        //GET
        public ActionResult Packages()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();





            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Packages(PaymentViewModel model)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();



        //    return View();
        //}

        //GET
        //[AllowAnonymous]
        //public ActionResult DownlineReport()
        //{
        //    int ddlrp = 1;

        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    if (Session.Count <= 0)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }



        //    var PId = long.Parse(Session["PersonID"].ToString());

        //    User nUser = nqz.Users.SingleOrDefault(x => x.PersonID == PId);
        //    if (nUser != null)
        //    {
        //        if (nUser.FirstTimeLogin == 1)
        //        {
        //            return RedirectToLocal("/Account/ManageProfile?retcode=0");
        //        }
        //    }

        //    List<Network> netlist = nqz.Networks.Where(x => x.IsVisible == true).ToList();
        //    ViewBag.NetList = new SelectList(netlist, "NetworkID", "Name");


        //    if (ddlrp == 1)
        //    {
        //        ReferralBoard nrefbd = nqz.ReferralBoards.SingleOrDefault(x => x.PersonID == PId);
        //        nrefbd.MLevel = "0";
        //        nqz.SaveChanges();


        //        GetDownline(PId, ddlrp);

        //        F_IDstring = F_IDstring.Trim();

        //        //////////////var dllist = vdb.DownlineVEReport(F_IDstring, 2); //vdb.GetDownlineRepList1(PId, networkid);
        //        //////////////ViewBag.DLList = dllist;
        //    }
        //    else
        //    {
        //        //var dllist = vdb.GetDownlineRepList2(PId, ddlrp);
        //        //ViewBag.DL1List = dllist;
        //    }



        //    return View();
        //}

        ////POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DownlineReport(PaymentViewModel model)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //    if (Session.Count <= 0)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    //List<BANK> banklist = vdb.BANKS.ToList();
        //    //ViewBag.BankList = new SelectList(banklist, "BANK_ID", "BANK1");

        //    //List<BankInfoViewModels> Banklist = vdb.BankInfos.Where(x => x.IsDeleted == false && x.PersonID.Equals(Session["PersonID"].ToString())).Select(x => new BankInfoViewModels { BankInfoID = x.BankInfoID, BankName = x.BANK.BANK1, AccountName = x.AccountName, AccountNumber = x.AccountNumber, BranchName = x.BranchName }).ToList();

        //    //ViewBag.BankList = Banklist;

        //    //BankInfo nbankinfo = new BankInfo();
        //    //nbankinfo.PersonID = int.Parse(Session["PersonID"].ToString());//model.PersonID;
        //    //nbankinfo.BankID = model.BankID;
        //    //nbankinfo.AccountName = model.AccountName;
        //    //nbankinfo.AccountNumber = model.AccountNumber;
        //    //nbankinfo.BranchName = model.BranchName;
        //    //nbankinfo.Created = DateTime.Now.Date;
        //    //nbankinfo.Updated = DateTime.Now.Date;

        //    //vdb.BankInfos.Add(nbankinfo);
        //    //vdb.SaveChanges();






        //    return View();
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        //public JsonResult ShowDownline(int networkid)
        //{

        //    VientiDBaseEntities vdb = new VientiDBaseEntities();
        //    var PId = long.Parse(Session["PersonID"].ToString());
        //    try
        //    {
        //        if (networkid == 1)
        //        {
        //            //var dllist = vdb.GetDownlineRepList1(PId, networkid);
        //            //ViewBag.DLList = dllist;


        //        }
        //        else
        //        {
        //            //var dllist = vdb.GetDownlineRepList2(PId, networkid);
        //            //ViewBag.DLList = dllist;
        //        }

        //        return Json(new { success = true });

        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }

        //}

        //[HttpPost]
        //////[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        //public JsonResult ShowDownline(int networkid)
        //{
        //    VientiDBaseEntities vdb = new VientiDBaseEntities();

        //    Stream stream = null;

        //    var PId = long.Parse(Session["PersonID"].ToString());
        //    F_IDstring = null;

        //    try
        //    {
        //        if (networkid == 1)
        //        {

        //            ReferralBoard nrefbd = vdb.ReferralBoards.SingleOrDefault(x => x.PersonID == PId);
        //            nrefbd.MLevel = "0";
        //            vdb.SaveChanges();


        //            GetDownline(PId, networkid);

        //            F_IDstring = F_IDstring.Trim();

        //            var dllist = vdb.DownlineVEReport(F_IDstring, 2); //vdb.GetDownlineRepList1(PId, networkid);
        //            ViewBag.DLList = dllist;



        //            ReportDocument rd = new ReportDocument();

        //            string strdl = System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt");
        //            rd.Load(strdl);

        //            //rd.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt"));

        //            rd.SetDataSource(dllist.ToList());

        //            Response.Buffer = false;
        //            Response.ClearContent();
        //            Response.ClearHeaders();

        //            //rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "cryDownlineReport");

        //            stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //            stream.Seek(0, SeekOrigin.Begin);

        //            File(stream, "application/pdf", "DownlinesReport.pdf");

        //            ////return View(new WebFormView("~/path/to/your/webform.aspx"));

        //        }
        //        else
        //        {
        //            var dllist = vdb.GetDownlineRepList2(PId, networkid);
        //            ViewBag.DLList = dllist;
        //        }

        //        return Json(new { success = true });


        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }
        //}

        [HttpGet]
        ////[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ShowDownline(string strDatalist)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            Stream stream = null;

            //int networkid = 1;

            var PId = long.Parse(Session["PersonID"].ToString());
            F_IDstring = null;

            try
            {
                //if (strDatalist != null)
                //{
                F_IDstring = Session["strdata"].ToString();
                //////////var dllist = vdb.DownlineVEReport(F_IDstring, 2); //vdb.GetDownlineRepList1(PId, networkid);
                //////////ViewBag.DLList = dllist;



                ReportDocument rd = new ReportDocument();

                string strdl = System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt");
                rd.Load(strdl);

                //rd.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt"));

                ////////////rd.SetDataSource(dllist.ToList());

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                //rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "cryDownlineReport");

                stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                //return File(stream, "application/pdf", "DownlinesReport.pdf");
                return new FileStreamResult(stream, "application/pdf");


                //}
                //else
                //{
                //    var dllist = vdb.GetDownlineRepList2(PId, networkid);
                //    ViewBag.DLList = dllist;
                //}

                //return Json(new { success = true });


            }
            catch
            {
                return Json(new { success = false });
            }

            return Json(new { success = false });
        }

        //[HttpPost]
        //////[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        //public ActionResult ShowDownline(int networkid)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();

        //    //Stream stream = null;

        //    //int networkid = 1;

        //    var PId = long.Parse(Session["PersonID"].ToString());
        //    F_IDstring = null;

        //    try
        //    {
        //        if (networkid == 1)
        //        {

        //            ReferralBoard nrefbd = vdb.ReferralBoards.SingleOrDefault(x => x.PersonID == PId);
        //            nrefbd.MLevel = "0";
        //            vdb.SaveChanges();


        //            GetDownline(PId, networkid);

        //            F_IDstring = F_IDstring.Trim();

        //            Session["strdata"] = F_IDstring;

        //            //Pdf(F_IDstring);

        //            //var dllist = vdb.DownlineVEReport(F_IDstring, 2); //vdb.GetDownlineRepList1(PId, networkid);
        //            //ViewBag.DLList = dllist;



        //            //ReportDocument rd = new ReportDocument();

        //            //string strdl = System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt");
        //            //rd.Load(strdl);

        //            ////rd.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt"));

        //            //rd.SetDataSource(dllist.ToList());

        //            //Response.Buffer = false;
        //            //Response.ClearContent();
        //            //Response.ClearHeaders();

        //            ////rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "cryDownlineReport");

        //            //stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //            //stream.Seek(0, SeekOrigin.Begin);

        //            //return File(stream, "application/pdf", "DownlinesReport.pdf");

        //            ////return View(new WebFormView("~/path/to/your/webform.aspx"));

        //            //return View(F_IDstring);

        //        }
        //        else
        //        {
        //            ////////////var dllist = vdb.GetDownlineRepList2(PId, networkid);
        //            ////////////ViewBag.DLList = dllist;
        //        }

        //        return Json(new { success = true });


        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }
        //}

        private void GetDownline(long id, int netid)
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            //var PId = long.Parse(Session["PersonID"].ToString());
            int i = 0;
            string[] strArrIDs = null;
            string strCommaSepIDs = id.ToString() + ",0";


            Person nperson = nqz.Persons.SingleOrDefault(x => x.IsCompany == true);
            CompID = nperson.PersonID;

            //Mem1 = int.Parse(RefID.ToString());
            do
            {

                if (i == 0)
                {
                    strArrIDs = strCommaSepIDs.Split(',');
                    strCommaSepIDs = null;
                }

                for (int j = 0; j <= strArrIDs.Length - 1; j++)
                {
                    if (strArrIDs[j] != "0")
                    {
                        var RefIDx = long.Parse(strArrIDs[j]);

                        //List<ReferralBoard> nrbL = nqz.ReferralBoards.Where(x => x.ReferralID == Mem1).ToList();
                        //List<ReferralBoard> nrbL = nqz.ReferralBoards.Where(x => x.ReferralID == RefIDx && x.NetworkID == netid).ToList();
                        //foreach (var rbL in nrbL)
                        //{
                        //    MemID = rbL.PersonID ?? 0;
                        //    //ReferralBoard nrefbd = nqz.ReferralBoards.SingleOrDefault(x => x.PersonID == rbL.ReferralID);

                        //    GetNewLevel(RefIDx, MemID, netid);

                        //    F_IDstring = F_IDstring + MemID + " ";

                        //    if (strCommaSepIDs == null)
                        //    {
                        //        strCommaSepIDs = MemID.ToString();
                        //    }
                        //    else
                        //    {
                        //        strCommaSepIDs = strCommaSepIDs + "," + MemID;
                        //    }
                        //}

                    }
                }

                i++;

                if (i >= strArrIDs.Length - 1)
                {
                    i = 0;

                }


                //Mem1 = Mem2;

            } while (strCommaSepIDs != null);
        }

        //private void GetNewLevel(long refid, long memid, int netid)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    int ML = 0;
        //    ReferralBoard nrefbd = nqz.ReferralBoards.SingleOrDefault(x => x.PersonID == refid);
        //    ML = int.Parse(nrefbd.MLevel);

        //    ML++;

        //    ReferralBoard nrefbd1 = nqz.ReferralBoards.SingleOrDefault(x => x.PersonID == memid);
        //    nrefbd1.MLevel = ML.ToString();
        //    nqz.SaveChanges();
        //}

        //GET
        public List<State> GetStateList()
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();

            List<State> States = nqz.States.ToList();

            return States;
        }
        //GET
        //public List<LGA> GetLGAList(int StateID)
        //{
        //    VientiDBEntities nqz = new VientiDBEntities();


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        //    return View();
        //}
        //GET
        [AllowAnonymous]
        public ActionResult FAQ()
        {


            return View();
        }

        //public CrystalReportPdfResult Pdf(string repdata)
        //{
        //    CArumala_edquizEntities nqz = new CArumala_edquizEntities();
        //    //List<Customer> model = new List<Customer>();
        //    //model.Add(new Customer { CompanyName = "Blah Inc.", ContactName = "Joe Blogs" });
        //    //string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptCustomers.rpt");
        //    ////////////////var dllist = vdb.DownlineVEReport(F_IDstring, 2); //vdb.GetDownlineRepList1(PId, networkid);
        //    ////////////////ViewBag.DLList = dllist;

        //    string reportPath = System.IO.Path.Combine(Server.MapPath("~/Reports"), "cryDownlineReport.rpt");
        //    ////////////////////rd.Load(strdl);

        //    //////////////return new CrystalReportPdfResult(reportPath, dllist.ToList());
        //    return new CrystalReportPdfResult(reportPath, null);
        //}

        [AllowAnonymous]
        private long GetSponsor(int downlinecnt, int totalmembership) //downlinecnt = 5, totalmembership = 781
        {
            CArumala_edquizEntities nqz = new CArumala_edquizEntities();
            long id = 0;
            int SponsorCnt = 0;
            int cnt = nqz.Persons.Select(x => x.PersonID == id).Count();
            if (cnt >= 1)
            {
                do
                {
                    id++;
                    //SponsorCnt = nqz.Persons.Select(x => x.ReferralID == id).Count();

                } while (SponsorCnt >= downlinecnt);
            }

            return id;
        }

        [AllowAnonymous]
        private long GetDownline(int src, string refereecode) //src 1 - firstdownline; 2 - underlines 
        {
            try
            {
                using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
                {
                    int cnt = 0;
                    int SponsorCnt = 0;
                    string ret;

                    var nqry = nqz.AppSettings.Where(x => x.ID == 1).Select(x => new { x.MaxFirstLevel, x.MaxUnderline }).FirstOrDefault();
                    for (int i = 1; i <= nqry.MaxUnderline; i++)
                    {
                        if (i == 1)
                        {
                            int firstlevelcnt = nqz.Persons.Where(x => x.ReferralCode == refereecode).Count();
                            if (firstlevelcnt < nqry.MaxFirstLevel)
                            {
                                // Add firstlevel downline
                                ret = refereecode;
                            }
                            else
                            {
                                var ndnln = nqz.Persons.Where(x => x.ReferralCode == refereecode).Select(x => new { });
                            }
                        }
                    }


                    //cnt = nqz.Persons.Select(x => x.PersonID == id).Count();
                    //if (cnt >= 1)
                    //{
                    //    do
                    //    {
                    //        cnt++;
                    //        //SponsorCnt = nqz.Persons.Select(x => x.ReferralID == id).Count();

                    //    } while (SponsorCnt >= downlinecnt);
                    //}
                }
            }
            catch (Exception ex)
            {
                //ViewBag.ErrMessage = ex.Message + " Stack = " + ex.StackTrace;
                //return View("Error");
            }



            return 0;
        }
    }
}