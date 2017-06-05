using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;
using timesheet_net.Utils.Security;

namespace timesheet_net.Controllers
{
    public class ProjectController : Controller
    {
        TimesheetDBEntities ctx = new TimesheetDBEntities();

        public ActionResult Overview()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            else
            {
                var employeeID = (int)Session["EmployeeID"];
                if ((int)Session["JobPosition"] == 1)
                {
                    var entities = new TimesheetDBEntities();
                    return View(entities.Projects.ToList());
                }
                else
                {
                    //list of projectsIDs Session["EmployeeID"] is assigned
                    var projectsIDs = ctx.ProjectMembers.Where(x => x.EmployeeID == employeeID).Select(x => new { x.ProjectID, x.ProjectMemberID }).ToList();
                    Projects project;
                    string name = string.Empty;
                    List<string> projectOverview = new List<string>();
                    List<int> timesheetIDS;
                    List<Tasks> tasksList;
                    int taskCount = 0;
                    decimal hoursworked = 0M;
                    foreach (var item in projectsIDs)
                    {
                        project = ctx.Projects.Where(x => x.ProjectID == item.ProjectID).FirstOrDefault();
                        if (project!=null)
                        {
                            //Name  |   Start   |   Finish  |   Project state name    |   Superior name |   Numer of people that works in this project  | Your part
                            projectOverview.Add(project.Name);
                            projectOverview.Add(project.Start.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                            name = project.Finish.ToString();
                            name = name == null ? "nieokreślono" : name.Split(' ').ElementAt(0);                             
                            projectOverview.Add(name);
                            name = ctx.ProjectStates.Where(x => x.ProjectStateID == project.ProjectStateID).Select(x => x.ProjectStateName).FirstOrDefault();
                            if (name != null) { projectOverview.Add(name); }
                            name = ctx.Employees.Where(x => x.EmployeeID == project.SuperiorID).Select(x => x.Name).FirstOrDefault();
                            name+=" "+ ctx.Employees.Where(x => x.EmployeeID == project.SuperiorID).Select(x => x.Surname).FirstOrDefault();
                            if (name != null) { projectOverview.Add(name); }
                            name = ctx.ProjectMembers.Where(x => x.ProjectID == project.ProjectID).Count().ToString();
                            if (name != null) { projectOverview.Add(name); }
                            //take employee all timesheetsIDs
                            timesheetIDS = ctx.Timesheets.Where(x => x.ProjectMemberID == item.ProjectMemberID).Select(x => x.TimesheetID).ToList();
                            foreach (var item2 in timesheetIDS)
                            {
                                tasksList = ctx.Tasks.Where(x => x.TimesheetID == item2).ToList();
                                taskCount += tasksList.Count();
                                foreach (var item3 in tasksList)
                                {
                                    hoursworked += item3.MondayHours + item3.TuesdayHours + item3.WednesdayHours + item3.ThursdayHours + item3.FridayHours + item3.SaturdayHours + item3.SundayHours;
                                }                                  
                            }
                            if (taskCount==1)
                            {
                                name = taskCount + " zadanie/";
                            }
                            else if (taskCount>=2 && taskCount <= 4) { name = taskCount + " zadania/"; }
                            else { name = taskCount + " zadań/"; }
                            projectOverview.Add(name+hoursworked.ToString() + "h");
                            taskCount = 0;
                            hoursworked = 0.0M;
                        }                        
                    }
                    ViewBag.projectOverview = projectOverview;
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult New()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                PopulateSuperiorsList();
                return View();
            }
            return RedirectToAction("Overview", "Project");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "Name, SuperiorID, Start, Finish")]Projects project)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                if (project.Finish != null && project.Start > project.Finish)
                {
                    ModelState.AddModelError("Start", "Data rozpoczęcia jest późniejsza niż data zakończenia projektu.");
                    PopulateSuperiorsList();
                    return View();
                }
               
                project.CreatedBy = (int)Session["EmployeeID"];
                project.CreationDate = DateTime.Now;
                project.ProjectStateID = 1; // TODO: Change this magic value
                
                ctx.Projects.Add(project);
                ctx.SaveChanges();
                
                return RedirectToAction("Overview", "Project");
            }
            return RedirectToAction("Overview", "Project");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                var proj = ctx.Projects.Find(id);
                if (proj == null)
                {
                    return HttpNotFound("Projekt o podanym ID nie istnieje!");
                }
                PopulateSuperiorsList(proj.SuperiorID.ToString());
                return View(proj);
            }
            return RedirectToAction("Overview", "Project");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectID, Name, SuperiorID, Start, Finish")]Projects project)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                var projToEdit = ctx.Projects.Find(project.ProjectID);
                if (projToEdit == null)
                {
                    return HttpNotFound("Projekt o podanym ID nie istnieje!");
                }

                projToEdit.Name = project.Name;
                projToEdit.SuperiorID = project.SuperiorID;
                projToEdit.Start = project.Start;
                projToEdit.Finish = project.Finish;

                if (project.Finish != null && project.Start > project.Finish)
                {
                    ModelState.AddModelError("Start", "Data rozpoczęcia jest późniejsza niż data zakończenia projektu.");
                    PopulateSuperiorsList();
                    return View(projToEdit);
                }

                ctx.Entry(projToEdit).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Overview", "Project");
            }
            return RedirectToAction("Overview", "Project");
        }

        [HttpGet]
        public ActionResult Close(int id)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                var projToEdit = ctx.Projects.Find(id);
                if (projToEdit == null)
                {
                    return HttpNotFound("Projekt o podanym ID nie istnieje!");
                }

                projToEdit.LastEditDate = DateTime.Now;
                projToEdit.LastEditedBy = (int)Session["EmployeeID"];
                projToEdit.ProjectStateID = 3;
                ctx.Entry(projToEdit).State = EntityState.Modified;
                ctx.SaveChanges();

                return RedirectToAction("Overview", "Project");
            }
            return RedirectToAction("Overview", "Project");
        }

        [HttpGet]
        public ActionResult Open(int id)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            var permutil = new PermissionUtil();
            if (permutil.IsAdministrator((int)Session["JobPosition"]))
            {
                var projToEdit = ctx.Projects.Find(id);
                if (projToEdit == null)
                {
                    return HttpNotFound("Projekt o podanym ID nie istnieje!");
                }

                projToEdit.LastEditDate = DateTime.Now;
                projToEdit.LastEditedBy = (int)Session["EmployeeID"];
                projToEdit.ProjectStateID = 2;
                ctx.Entry(projToEdit).State = EntityState.Modified;
                ctx.SaveChanges();

                return RedirectToAction("Overview", "Project");
            }
            return RedirectToAction("Overview", "Project");
        }

        private void PopulateSuperiorsList(object selectedEmployee = null)
        {
            var ctx = new TimesheetDBEntities();
            var employees = from j in ctx.Employees
                            where j.EmployeeStateID == 1 || j.EmployeeStateID == 2
                            select new
                            {
                                EmployeeID = j.EmployeeID,
                                Employee = j.Name + " " + j.Surname + " (" + j.EMail + ")"
                            };
            ViewBag.SuperiorID = new SelectList(employees, "EmployeeID", "Employee", selectedEmployee);
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
