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

        [HttpPost]
        public JsonResult AssignEmployeeToProject(int projID, int emplID)
        {
            var projectMember = new ProjectMembers();
            projectMember.EmployeeID = emplID;
            projectMember.ProjectID = projID;

            projectMember = ctx.ProjectMembers.Add(projectMember);
            int recordsWritten = ctx.SaveChanges();
            if (recordsWritten != 0)
            {
                return Json(new
                {
                    Error = 0
                });
            }

            return Json(new
            {
                Error = 1
            });
        }

        [HttpPost]
        public JsonResult UnassignEmployeeFromProject(int projID, int emplID)
        {
            var projectMember = (from pm in ctx.ProjectMembers
                                where pm.ProjectID == projID &&
                                      pm.EmployeeID == emplID
                                select pm).FirstOrDefault();

            ctx.ProjectMembers.Remove(projectMember);

            int result = ctx.SaveChanges();
            if (result != 0)
            {
                return Json(new
                {
                    Error = 0
                });
            }

            return Json(new
            {
                Error = 1
            });
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