using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace timesheet_net.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Overview()
        {
            return View();
        }
    }
}