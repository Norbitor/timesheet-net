using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace timesheet_net.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Current()
        {
            return View();
        }

        // GET: Timesheet
        public ActionResult Disapproved()
        {
            return View();
        }


    }
}