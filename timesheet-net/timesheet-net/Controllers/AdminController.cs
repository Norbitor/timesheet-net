using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace timesheet_net.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult UserList()
        {
            if (Session["EmployeeID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ProjectList()
        {
            if (Session["EmployeeID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ProjectForm()
        {
            if (Session["EmployeeID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult UserForm()
        {
            if (Session["EmployeeID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}