using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace timesheet_net.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult ProjectForm()
        {
            return View();
        }

        public ActionResult UserForm()
        {
            return View();
        }
    }
}