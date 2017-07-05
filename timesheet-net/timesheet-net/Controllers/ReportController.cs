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

            int uid = (int)Session["EmployeeID"];

            

            return View();
        }

        public ActionResult Show(string startDate)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }

            int uid = (int)Session["EmployeeID"];
            DateTime start = DateTime.Parse(startDate);
            DateTime finish = start.AddDays(7);

            var timesheets = (from t in ctx.Timesheets
                              join p in ctx.Projects on t.ProjectMembers.ProjectID equals p.ProjectID
                              where t.ProjectMembers.EmployeeID == uid &&
                                    t.Start >= start && t.Finish <= finish
                              select new ShowReportViewModel
                              {
                                  Projects = p,
                                  TimesheetID = t.TimesheetID,
                                  Start = t.Start,
                                  Finish = t.Finish,
                                  ProjectMemberID = t.ProjectMemberID,
                                  TimesheetState = t.TimesheetStates,
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

            if (timesheets == null)
            {
                return HttpNotFound("Nie znaleziono Timesheeta spelniajacego kryteria");
            }

            ViewBag.StartDate = start;
            ViewBag.FinishDate = finish;

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