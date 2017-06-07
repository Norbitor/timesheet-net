using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class AjaxController : Controller
    {
        TimesheetDBEntities ctx = new TimesheetDBEntities();

        [HttpPost]
        public JsonResult GetEmployeeList(string pattern = null)
        {
            if (pattern == null)
            {
                return Json(ctx.Employees.Select(empl => new
                {
                    EmployeeID = empl.EmployeeID,
                    NameAndSurname = empl.Name + " " + empl.Surname
                }).Take(10));
            }
            var employees = from empl in ctx.Employees
                            where empl.Name.Contains(pattern) || empl.Surname.Contains(pattern)
                            select new
                            {
                                EmployeeID = empl.EmployeeID,
                                NameAndSurname = empl.Name + " " + empl.Surname
                            };
            return Json(employees.Take(10));
        }

        [HttpPost]
        public JsonResult GetAssignedEmplotyeesToProject(int projID)
        {
            var result = from empl in ctx.Employees
                         join pm in ctx.ProjectMembers on empl equals pm.Employees
                         where pm.ProjectID == projID
                         select new
                         {
                             EmployeeID = empl.EmployeeID,
                             NameAndSurname = empl.Name + " " + empl.Surname
                         };
                            
            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}