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
                        var projectNames = ctx.Projects.Where(x => x.ProjectID == item).Select(x => new { x.Name, x.ProjectID }).FirstOrDefault();
                        //if loggedIn is assigned to project...
                        if (projectNames != null)
                        {
                            if (Session["projectID"] != null)
                            {
                                if (Session["projectID"].ToString() == projectNames.ProjectID.ToString())
                                {
                                    IDNameProject.Insert(0, (new SelectListItem
                                    {
                                        Text = projectNames.Name.ToString(),  //Name
                                        Value = item.ToString()          //ProjectID
                                    }));
                                }
                                else
                                {
                                    IDNameProject.Add(new SelectListItem
                                    {
                                        Text = projectNames.Name.ToString(), //Name
                                        Value = item.ToString()          //ProjectID
                                    });
                                }
                            }
                            else
                            {
                                Session["projectID"] = item.ToString();
                                IDNameProject.Add(new SelectListItem
                                {
                                    Text = projectNames.Name.ToString(), //Name
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
                            List<Tasks> tasks = ctx.Tasks.Where(x => x.TimesheetID == timesheet.TimesheetID).ToList();

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
                    return View(IDNameProject);
                }

            }

            return View(IDNameProject);
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
                Session["projectID"] = projectIdent;
                Session["tasks"] = null;
                return RedirectToAction("Current", "Timesheet");
            }
            else
            {
                return RedirectToAction("Current", "Timesheet");
            }
        }

        [HttpPost]
        public ActionResult SaveTimesheet(string []data)
        {
            //projectName is the identyfier of the project!
            //string -> int
            if (Session["EmployeeID"] != null)
            {
                if (Session["projectID"]!=null && data.Length%10==0)
                {
                    int projectID = Int32.Parse(Session["projectID"].ToString());
                    int employeeID=Int32.Parse(Session["EmployeeID"].ToString());

                    using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                    {
                        var projectMemberID = ctx.ProjectMembers.Where(x => x.ProjectID == projectID && x.EmployeeID == employeeID).Select(x => x.ProjectMemberID).FirstOrDefault();
                        if (projectMemberID!=null)
                        {
                            //where start & finish && dateTimeNow beetween
                            var dateTimeNow = DateTime.Now.Date;
                            var timesheetID = ctx.Timesheets.Where(x => x.ProjectMemberID == projectMemberID && dateTimeNow>=x.Start && dateTimeNow<=x.Finish).Select(x => x.TimesheetID).FirstOrDefault();
                            if (timesheetID!=null) //timesheetID
                            {                          
                                int taskID = 0;
                                Tasks task;                               
                                for (int i = 0; i < data.Length; i += 10)
                                {
                                    taskID = Int32.Parse(data[i]);                                          
                                    if (taskID == 0) //new task
                                    {
                                        task = new Tasks();

                                        task.TimesheetID = timesheetID;
                                        task.TaskName= data[i + 1];
                                        task.MondayHours = Decimal.Parse(data[i + 2]);
                                        task.TuesdayHours= Decimal.Parse(data[i + 3]);
                                        task.WednesdayHours= Decimal.Parse(data[i + 4]);
                                        task.ThursdayHours= Decimal.Parse(data[i + 5]);
                                        task.FridayHours= Decimal.Parse(data[i + 6]);
                                        task.SaturdayHours= Decimal.Parse(data[i + 7]);
                                        task.SundayHours= Decimal.Parse(data[i + 8]);
                                        task.Comment= data[i + 9];
                                        task.LastEditedBy = employeeID;
                                        task.LastEditDate = DateTime.Now;
                                        task.CreatedBy = employeeID;
                                        task.CreationDate= DateTime.Now;
                                        ctx.Tasks.Add(task);
                                    }
                                    else //existing task
                                    {
                                        task = ctx.Tasks.Where(x => x.TaskID == taskID).FirstOrDefault();

                                        task.TaskName = data[i + 1];
                                        task.MondayHours = Decimal.Parse(data[i + 2]);
                                        task.TuesdayHours = Decimal.Parse(data[i + 3]);
                                        task.WednesdayHours = Decimal.Parse(data[i + 4]);
                                        task.ThursdayHours = Decimal.Parse(data[i + 5]);
                                        task.FridayHours = Decimal.Parse(data[i + 6]);
                                        task.SaturdayHours = Decimal.Parse(data[i + 7]);
                                        task.SundayHours = Decimal.Parse(data[i + 8]);
                                        task.Comment = data[i + 9];
                                        task.LastEditedBy = employeeID;
                                        task.LastEditDate = DateTime.Now;

                                        ctx.Entry(task).State = EntityState.Modified;                                 
                                    }                                 
                                }                       
                            }
                        }
                        ctx.SaveChanges();
                    }
                }
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