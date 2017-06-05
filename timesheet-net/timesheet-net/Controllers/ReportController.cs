using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace timesheet_net.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            return View();
        }

        public ActionResult Show()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            return View();
        }

        public ActionResult Review()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            return View();
        }
    }
}