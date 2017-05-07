using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class ProjectController : Controller
    {
        public ActionResult Overview()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            var entities = new TimesheetDBEntities();
            return View(entities.Projects.ToList());
        }

        [HttpGet]
        public ActionResult New()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            PopulateSuperiorsList();
            return View();
        }

        private void PopulateSuperiorsList(object selectedEmployee = null)
        {
            var ctx = new TimesheetDBEntities();
            var employees = from j in ctx.Employees
                            select j;
            ViewBag.SuperiorID = new SelectList(employees, "EmployeeID", "Surname", selectedEmployee);
        }
    }
}