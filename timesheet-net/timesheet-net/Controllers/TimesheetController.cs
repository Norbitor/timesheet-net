using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;
using System.Data;
using System.Data.Entity;

namespace timesheet_net.Controllers
{
    public class TimesheetController : Controller
    {

        [HttpGet]
        public ActionResult Current()
        {
            List<SelectListItem> IDNameProject = new List<SelectListItem>();
            if (Session["EmployeeID"] != null)
            {
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                //person logged in ID
                int loggedIn = (int)Session["EmployeeID"];
                //list of project IDs where person is assigned
                var listOfProjectIDs = ctx.ProjectMembers.Include(x => x.Projects).Where(x => x.EmployeeID == loggedIn).OrderBy(x => x.ProjectMemberID).Select(x => x.ProjectID).ToList();
                if (listOfProjectIDs.Count() != 0) //if user is assigned to somewhere
                {
                    foreach (var item in listOfProjectIDs)
                    {
                        var projectNames = ctx.Projects.Where(x => x.ProjectID == item).Select(x => x.Name).FirstOrDefault();
                        //if loggedIn is assigned to project...
                        if (projectNames != null)
                        {
                            if (Session["projectName"] != null)
                            {
                                if (Session["projectName"].ToString() == projectNames.ToString())
                                {
                                    IDNameProject.Insert(0, (new SelectListItem
                                    {
                                        Text = projectNames.ToString(),  //Name
                                        Value = item.ToString()          //ProjectID
                                    }));
                                }
                                else
                                {
                                    IDNameProject.Add(new SelectListItem
                                    {
                                        Text = projectNames.ToString(), //Name
                                        Value = item.ToString()          //ProjectID
                                    });
                                }
                            }
                            else
                            {
                                IDNameProject.Add(new SelectListItem
                                {
                                    Text = projectNames.ToString(), //Name
                                    Value = item.ToString()          //ProjectID
                                });
                            }
                        }
                    }

                    //take first from list (ProjectID)
                    int selectedprojectID = Int32.Parse(IDNameProject.First().Value);
                    //take proper ProjectMemberID -> ProjectID && (int)Session["EmployeeID"]
                    var projectMemberID = ctx.ProjectMembers.Where(x => x.ProjectID == selectedprojectID && x.EmployeeID == loggedIn).Select(x => x.ProjectMemberID).FirstOrDefault();
                    //take proper Timesheet having ProjectMemberID
                    if (projectMemberID > 0)
                    {
                        //info about the projecy
                        var timesheet = ctx.Timesheets.Where(x => x.ProjectMemberID == projectMemberID).Select(x => new {x.TimesheetID, x.Start, x.Finish, x.TimesheetStateID }).FirstOrDefault();
                        if (timesheet != null)
                        {
                            string timesheetStateName = ctx.TimesheetStates.Where(x => x.TimesheetStateID == timesheet.TimesheetStateID).Select(x => x.TimesheetStateName).FirstOrDefault();
                            if (timesheetStateName != null)
                            {
                                ViewBag.projectName = IDNameProject.First().Text;
                                ViewBag.timesheetStart = timesheet.Start.ToString("yyyy-MM-dd");
                                ViewBag.timesheetFinish = timesheet.Finish.Date.ToString("yyyy-MM-dd");
                                ViewBag.timesheetStateName = timesheetStateName;
                            }
                            //list of tasks
                            List<Tasks> tasks = (List<Tasks>)Session["tasks"];
                            if (tasks == null)
                            {
                                tasks = ctx.Tasks.Where(x => x.TimesheetID == timesheet.TimesheetID).ToList();
                            }
                            if (tasks.Count()!=0)
                            {
                                Session["tasks"] = tasks;
                                //general hours summary
                                decimal MH = 0;
                                decimal TuH = 0;
                                decimal WH = 0;
                                decimal ThH = 0;
                                decimal FH = 0;
                                decimal SaH = 0;
                                decimal SuH = 0;
                                decimal allH = 0;
                                foreach (var item in tasks)
                                {
                                    MH += item.MondayHours;
                                    TuH += item.TuesdayHours;
                                    WH += item.WednesdayHours;
                                    ThH += item.ThursdayHours;
                                    FH += item.FridayHours;
                                    SaH += item.SaturdayHours;
                                    SuH += item.SundayHours;
                                }
                                allH = MH + TuH + WH + ThH + FH + SaH + SuH;
                                ViewData["0"] = MH.ToString();
                                ViewData["1"] = TuH.ToString();
                                ViewData["2"] = WH.ToString();
                                ViewData["3"] = ThH.ToString();
                                ViewData["4"] = FH.ToString();
                                ViewData["5"] = SaH.ToString();
                                ViewData["6"] = SuH.ToString();
                                ViewBag.allH = allH.ToString();
                            }
                        }
                    }
                    return View(IDNameProject);
                }

            }

            return View(IDNameProject);
        }
        [HttpPost]
        public ActionResult AddTask(string data)
        {
            if (Session["EmployeeID"] != null)
            {
                List<Tasks> tasks = (List<Tasks>)Session["tasks"];
                if (tasks.Count!=0)
                {
                    //new Task
                    Tasks newTask = new Tasks();
                    newTask.TaskName = data;
                    newTask.MondayHours=0.00M;
                    newTask.TuesdayHours = 0.00M;
                    newTask.WednesdayHours = 0.00M;
                    newTask.ThursdayHours = 0.00M;
                    newTask.FridayHours = 0.00M;
                    newTask.SaturdayHours = 0.00M;
                    newTask.SundayHours = 0.00M;
                    newTask.Comment = "";
                    newTask.LastEditedBy = null;
                    newTask.LastEditDate = null;
                    newTask.CreatedBy = (int)Session["EmployeeID"];
                    newTask.CreationDate = DateTime.Now;

                    tasks.Add(newTask);
                    Session["tasks"] = tasks;
                }
                return RedirectToAction("Current", "Timesheet");
            }
            else
            {
                return RedirectToAction("Current", "Timesheet");
            }
        }
        [HttpPost]
        public ActionResult DeleteTask(string deleteData)
        {
            if (Session["EmployeeID"] != null)
            {
                if (deleteData!=null)
                {
                    List<Tasks> tasks = (List<Tasks>)Session["tasks"];
                    tasks.RemoveAt(Int32.Parse(deleteData));
                    Session["tasks"] = tasks;
                }
                return RedirectToAction("Current", "Timesheet");
            }
            else
            {
                return RedirectToAction("Current", "Timesheet");
            }
        }

        [HttpPost]
        public ActionResult ChangeTimesheet(string projectID)
        {
            if (Session["EmployeeID"] != null)
            {
                //session
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                int projectIdent = Int32.Parse(projectID);
                var projectNames = ctx.Projects.Where(x => x.ProjectID == projectIdent).Select(x => x.Name).FirstOrDefault();
                if (projectNames != null)
                {
                    Session["projectName"] = projectNames;
                    Session["tasks"] = null;
                }
                return RedirectToAction("Current", "Timesheet");
            }
            else
            {
                return RedirectToAction("Current", "Timesheet");
            }
        }

        [HttpPost]
        public ActionResult SaveTimesheet(string projectName)
        {
            //projectName is the identyfier of the project!
            //string -> int
            if (Session["EmployeeID"] != null)
            {
                //session
                return RedirectToAction("Current", "Timesheet");
            }
            else
            {
                return RedirectToAction("Current", "Timesheet");
            }
        }


        // GET: Timesheet
        public ActionResult Disapproved()
        {
            return View();
        }



    }
}