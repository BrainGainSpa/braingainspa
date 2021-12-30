using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using braingainspa.Models;

namespace braingainspa.Controllers
{
    public class TreeViewController : Controller
    {
        // GET: TreeView
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Simple()
        {
            List<SiteMenu> all = new List<SiteMenu>();
            using (CArumala_edquizEntities nqz = new CArumala_edquizEntities())
            {
                all = nqz.SiteMenus.OrderBy(a => a.ParentMenuID).ToList();
            }

            return View(all);
        }
    }
}