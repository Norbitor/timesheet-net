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
    }
}