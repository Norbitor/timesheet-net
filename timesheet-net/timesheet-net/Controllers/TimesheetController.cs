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
        public ActionResult Current()
        {
            if (Session["EmployeeID"] != null)
            {
                TimesheetDBEntities ctx = new TimesheetDBEntities();
                //person logged in ID
                int loggedIn = (int)Session["EmployeeID"];
                //list of project IDs where person is assigned
                var listOfProjectIDs = ctx.ProjectMembers.Include(x=> x.Projects).Where(x => x.EmployeeID == loggedIn).OrderBy(x => x.ProjectMemberID).Select(x=>x.ProjectID).ToList();
               
                if (listOfProjectIDs!=null) //if user is assigned to somewhere
                {
                    List<SelectListItem> IDNameProject = new List<SelectListItem>();
                    foreach(var item in listOfProjectIDs)
                    {
                        var projectName = ctx.Projects.Where(x => x.ProjectID == item).Select(x => x.Name).FirstOrDefault();
                        if (projectName != null)
                        {
                            IDNameProject.Add(new SelectListItem
                            {
                                Text = projectName.ToString(),
                                Value= item.ToString()
                            });
                        }
                    }

                    return View(IDNameProject);
                }

            }

            return View();
        }

        // GET: Timesheet
        public ActionResult Disapproved()
        {
            return View();
        }


    }
}