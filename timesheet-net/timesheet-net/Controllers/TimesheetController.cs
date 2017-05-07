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

        // GET: Timesheet
        public ActionResult Current(string projectName=null)
        {
            List<SelectListItem> IDNameProject = new List<SelectListItem>();
            if (Session["EmployeeID"] != null)
            {
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                //person logged in ID
                int loggedIn = (int)Session["EmployeeID"];
                //list of project IDs where person is assigned
                var listOfProjectIDs = ctx.ProjectMembers.Include(x=> x.Projects).Where(x => x.EmployeeID == loggedIn).OrderBy(x => x.ProjectMemberID).Select(x=>x.ProjectID).ToList();
               
                if (listOfProjectIDs.Count()!=0) //if user is assigned to somewhere
                {
                    foreach(var item in listOfProjectIDs)
                    {
                        var projectNames = ctx.Projects.Where(x => x.ProjectID == item).Select(x => x.Name).FirstOrDefault();
                        //if loggedIn is assigned to project...
                        if (projectNames != null)
                        {
                            IDNameProject.Add(new SelectListItem
                            {
                                Text = projectNames.ToString(), //Name
                                Value= item.ToString()          //ProjectID
                            });
                        }
                    }
                    //if someone go to this section first time
                    if (projectName==null)
                    {
                        //take first from list (ProjectID)
                        int selectedprojectID = Int32.Parse(IDNameProject.First().Value);
                        //take proper ProjectMemberID -> ProjectID && (int)Session["EmployeeID"]
                        var projectMemberID = ctx.ProjectMembers.Where(x => x.ProjectID == selectedprojectID && x.EmployeeID == loggedIn).Select(x => x.ProjectMemberID).FirstOrDefault();
                        //take proper Timesheet having ProjectMemberID
                        if (projectMemberID > 0)
                        {
                            var timesheet = ctx.Timesheets.Where(x => x.ProjectMemberID == projectMemberID).Select(x => new { x.Start,x.Finish,x.TimesheetStateID }).FirstOrDefault();
                            if (timesheet != null)
                            {
                                string timesheetStateName = ctx.TimesheetStates.Where(x => x.TimesheetStateID == timesheet.TimesheetStateID).Select(x => x.TimesheetStateName).FirstOrDefault();
                                if (timesheetStateName!=null)
                                {
                                    ViewBag.projectName = IDNameProject.First().Text;
                                    ViewBag.timesheetStart=timesheet.Start.ToString("yyyy-MM-dd");
                                    ViewBag.timesheetFinish=timesheet.Finish.Date.ToString("yyyy-MM-dd");
                                    ViewBag.timesheetStateName=timesheetStateName;
                                }
                            }

                        }

                    }
                    else //TODO
                    { }

                    return View(IDNameProject);
                }

            }

            return View(IDNameProject);
        }

        // GET: Timesheet
        public ActionResult Disapproved()
        {
            return View();
        }


    }
}