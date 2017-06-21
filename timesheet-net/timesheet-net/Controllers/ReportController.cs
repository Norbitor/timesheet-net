using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class ReportController : Controller
    {
        TimesheetDBEntities ctx = new TimesheetDBEntities();

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

            int uid = (int)Session["EmployeeID"];

            var timesheets = (from t in ctx.Timesheets
                              join p in ctx.Projects on t.ProjectMembers.ProjectID equals p.ProjectID
                              where t.ProjectMembers.EmployeeID == uid
                              select new ShowReportViewModel
                              {
                                  ProjectName = p.Name,
                                  TimesheetID = t.TimesheetID,
                                  Start = t.Start,
                                  Finish = t.Finish,
                                  ProjectMemberID = t.ProjectMemberID,
                                  TimesheetStateID = t.TimesheetStateID,
                                  Comment = t.Comment,
                                  Tasks = t.Tasks,
                                  MondaySum = t.Tasks.Sum(task => (decimal?)task.MondayHours) ?? 0,
                                  TuesdaySum = t.Tasks.Sum(task => (decimal?)task.TuesdayHours) ?? 0,
                                  WednesdaySum = t.Tasks.Sum(task => (decimal?)task.WednesdayHours) ?? 0,
                                  ThursdaySum = t.Tasks.Sum(task => (decimal?)task.ThursdayHours) ?? 0,
                                  FridaySum = t.Tasks.Sum(task => (decimal?)task.FridayHours) ?? 0,
                                  SaturdaySum = t.Tasks.Sum(task => (decimal?)task.SaturdayHours) ?? 0,
                                  SundaySum = t.Tasks.Sum(task => (decimal?)task.SundayHours) ?? 0
                              }).ToList();

            

            return View(timesheets);
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