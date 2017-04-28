using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Employees()
        {
            if (Session["EmployeeID"] != null)
            {
                CheckUserPermission();
                var entities = new TimesheetDBEntities();
                return View(entities.Employees.ToList());
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
                CheckUserPermission();
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
                CheckUserPermission();
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
                CheckUserPermission();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        private void CheckUserPermission()
        {
            if ((int)Session["JobPosition"] != 1)
            {
                throw new UnauthorizedAccessException("Nie masz wystarczających uprawnień do oglądania tej witryny.");
            }
        }
    }
}