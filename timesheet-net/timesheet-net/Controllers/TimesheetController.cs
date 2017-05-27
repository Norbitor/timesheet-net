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
            if (Session["EmployeeID"] != null)
            {
                return CurrentOrDisapproved(1);
            }
                return RedirectToAction("", "Home");
            
        }

        [HttpPost]
        public ActionResult DeleteTask(string deleteData)
        {
            if (Session["EmployeeID"] != null)
            {
                if (deleteData != null)
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
        public ActionResult ChangeTimesheet(string timesheetID)
        {
            if (Session["EmployeeID"] != null)
            {
                //session
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                int timesheetIdent = Int32.Parse(timesheetID);
                Session["timesheetID"] = timesheetIdent;
                int timesheetStateID = ctx.Timesheets.Where(x => x.TimesheetID == timesheetIdent).Select(x => x.TimesheetStateID).FirstOrDefault();
                if (timesheetStateID > 0)
                {
                    if (timesheetStateID == 1)
                    {
                        return RedirectToAction("Current", "Timesheet");
                    }
                    else if (timesheetStateID == 5)
                    {
                        return RedirectToAction("Disapproved", "Timesheet");
                    }
                }           
            }
                return RedirectToAction("", "Home");
        }

        [HttpPost]
        public ActionResult SaveTimesheet(string[] data)
        {
            //projectName is the identyfier of the project!
            //string -> int
            bool properData = false; //data null or proper condition
            if (Session["EmployeeID"] != null)
            {
                if (data == null) { properData = true; }
                else if (data.Length % 10 == 0) { properData = true; }
                if (Session["timesheetID"] != null && properData==true)
                {
                    List<long> taskIDFromTimesheet = new List<long>();
                    //int projectID = Int32.Parse(Session["projectID"].ToString());
                   int employeeID = Int32.Parse(Session["EmployeeID"].ToString());
                    int timesheetID = Int32.Parse(Session["TimesheetID"].ToString());
                    using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                    {
                        //var projectMemberID = ctx.ProjectMembers.Where(x => x.ProjectID == projectID && x.EmployeeID == employeeID).Select(x => x.ProjectMemberID).FirstOrDefault();
                       // if (projectMemberID != null)
                        //{
                            //where start & finish && dateTimeNow beetween
                            var dateTimeNow = DateTime.Now.Date;                           
                            //var timesheetID = ctx.Timesheets.Where(x => x.ProjectMemberID == projectMemberID && x.TimesheetStateID == 1).Select(x => x.TimesheetID).FirstOrDefault();
                            if (timesheetID > 0) //timesheetID
                            {
                                int taskID = 0;
                                Tasks task;
                                var tasks = ctx.Tasks.Where(x => x.TimesheetID == timesheetID);
                            //can be no data
                            if (data != null)
                            {
                                for (int i = 0; i < data.Length; i += 10)
                                {
                                    taskID = Int32.Parse(data[i]);
                                    taskIDFromTimesheet.Add(taskID);
                                    if (taskID == 0) //new task
                                    {
                                        task = new Tasks();

                                        task.TimesheetID = timesheetID;
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
                                        task.CreatedBy = employeeID;
                                        task.CreationDate = DateTime.Now;
                                        ctx.Tasks.Add(task);
                                    }
                                    else //existing task
                                    {
                                        task = tasks.Where(x => x.TaskID == taskID).FirstOrDefault();//ctx.Tasks.Where(x => x.TaskID == taskID).FirstOrDefault();

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
                                //Remove from db tasks which users has deleted
                                //tasks -> list of tasks from DB
                                //taskIDFromTimesheet
                                foreach (var item in tasks)
                                {
                                    if (!taskIDFromTimesheet.Contains(item.TaskID))
                                    {
                                        ctx.Entry(item).State = EntityState.Deleted;
                                    }
                                }
                            }
                        //}

                        ctx.SaveChanges();
                        TempData["SaveChanges"] = "OK";
                        int timesheetStateID = ctx.Timesheets.Where(x => x.TimesheetID == timesheetID).Select(x => x.TimesheetStateID).FirstOrDefault();
                        if (timesheetStateID>0)
                        {
                            if (timesheetStateID==1)
                            {
                                return RedirectToAction("Current", "Timesheet");
                            }
                            else if (timesheetStateID==5)
                            {
                                return RedirectToAction("Disapproved", "Timesheet");
                            }   
                        }
                    }
                }
            }
                return RedirectToAction("", "Home");
            
        }


        // GET: Timesheet
        public ActionResult Disapproved()
        {
            if (Session["EmployeeID"] != null)
            {
                return CurrentOrDisapproved(5);
            }
            else
            {
                return RedirectToAction("", "Home");
            }

        }

        public ActionResult CurrentOrDisapproved(int timesheetStateID)
        {
            Session["tasks"] = null;
            if (Session["CurrentOrDisapproved"]!=null && Session["CurrentOrDisapproved"].ToString()!=timesheetStateID.ToString())
            {
                Session["timesheetID"] = null;
            }
            Session["CurrentOrDisapproved"] = timesheetStateID;
            List<SelectListItem> TimesheetIDNameProject = new List<SelectListItem>();
            if (Session["EmployeeID"] != null)
            {
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                //person logged in ID
                int loggedIn = (int)Session["EmployeeID"];
                //list of project IDs where person is assigned
                var listOfProjectAndProjectMembersIDs = ctx.ProjectMembers.Include(x => x.Projects).Where(x => x.EmployeeID == loggedIn).OrderBy(x => x.ProjectMemberID).Select(x => new { x.ProjectID, x.ProjectMemberID }).ToList();
                if (listOfProjectAndProjectMembersIDs.Count() != 0) //if user is assigned to somewhere
                {
                    foreach (var item in listOfProjectAndProjectMembersIDs)
                    {
                        var projectNames = ctx.Projects.Where(x => x.ProjectID == item.ProjectID).Select(x => x.Name).FirstOrDefault();
                        //if loggedIn is assigned to project...
                        if (projectNames != null)
                        {
                            var timesheets = ctx.Timesheets.Where(x => x.ProjectMemberID == item.ProjectMemberID && x.TimesheetStateID==timesheetStateID);
                            if (timesheets != null)
                            {
                                foreach (var item2 in timesheets)
                                {
                                    if (Session["timesheetID"] != null)
                                    {
                                        if (Session["timesheetID"].ToString() == item2.TimesheetID.ToString())
                                        {
                                            TimesheetIDNameProject.Insert(0, (new SelectListItem
                                            {
                                                Text = projectNames.ToString() + " (" + item2.Start.Date.ToString("yyyy-MM-dd") + " - " + item2.Finish.Date.ToString("yyyy-MM-dd") + ")",  //Name
                                                Value = item2.TimesheetID.ToString()        //ProjectID
                                            }));
                                        }
                                        else
                                        {
                                            TimesheetIDNameProject.Add(new SelectListItem
                                            {
                                                Text = projectNames.ToString() + " (" + item2.Start.Date.ToString("yyyy-MM-dd") + " - " + item2.Finish.Date.ToString("yyyy-MM-dd") + ")", //Name
                                                Value = item2.TimesheetID.ToString()          //ProjectID
                                            });
                                        }
                                    }
                                    else
                                    {
                                        Session["timesheetID"] = item2.TimesheetID.ToString();
                                        TimesheetIDNameProject.Add(new SelectListItem
                                        {
                                            Text = projectNames.ToString() + " (" + item2.Start.Date.ToString("yyyy-MM-dd") + " - " + item2.Finish.Date.ToString("yyyy-MM-dd") + ")", //Name
                                            Value = item2.TimesheetID.ToString()          //ProjectID
                                        });
                                    }
                                }
                            }                           
                        }
                    }
                    if (TimesheetIDNameProject.Count()==0)
                    {
                        return View(TimesheetIDNameProject);
                    }

                    //take first from list (ProjectID)
                    int selectedTimesheetID = Int32.Parse(TimesheetIDNameProject.First().Value);
                    //take proper ProjectMemberID -> ProjectID && (int)Session["EmployeeID"]
                    //var projectMemberID = ctx.ProjectMembers.Where(x => x.ProjectID == selectedprojectID && x.EmployeeID == loggedIn).Select(x => x.ProjectMemberID).FirstOrDefault();
                    //take proper Timesheet having ProjectMemberID
                    //if (projectMemberID > 0)
                    //{
                    //info about the projecy
                    var timesheet = ctx.Timesheets.Where(x => x.TimesheetID == selectedTimesheetID).Select(x => new { x.TimesheetID, x.Start, x.Finish, x.TimesheetStateID , x.Comment}).FirstOrDefault();
                    //var timesheet = ctx.Timesheets.Where(x => x.ProjectMemberID == projectMemberID && x.TimesheetStateID == 1).Select(x => new { x.TimesheetID, x.Start, x.Finish, x.TimesheetStateID }).FirstOrDefault();
                    if (timesheet != null)
                    {
                        //Session["TimesheetID"] = timesheet.TimesheetID;
                        string timesheetStateName = ctx.TimesheetStates.Where(x => x.TimesheetStateID == timesheet.TimesheetStateID).Select(x => x.TimesheetStateName).FirstOrDefault();
                        if (timesheetStateName != null)
                        {
                            ViewBag.projectName = (TimesheetIDNameProject.First().Text).Split(new string[] { " (" }, StringSplitOptions.None)[0];
                            ViewBag.timesheetStart = timesheet.Start.ToString("yyyy-MM-dd");
                            ViewBag.timesheetFinish = timesheet.Finish.Date.ToString("yyyy-MM-dd");
                            ViewBag.timesheetStateName = timesheetStateName;
                            if (timesheet.TimesheetStateID == 1) { ViewBag.timesheetComment = null; }
                            else if (timesheet.TimesheetStateID == 5)
                            {
                                ViewBag.timesheetComment = timesheet.Comment == "" ? "Nie podano powodu odrzucenia" : timesheet.Comment;
                            }
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
                    //}
                    return View(TimesheetIDNameProject);
                }

            }

            return View(TimesheetIDNameProject);
        }

    }
}