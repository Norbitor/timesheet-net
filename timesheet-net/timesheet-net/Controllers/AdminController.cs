using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Employees()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CheckUserPermission();
            var entities = new TimesheetDBEntities();
            return View(entities.Employees.ToList());
        }

        [HttpGet]
        public ActionResult Employee()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CheckUserPermission();
            PopulateJobPositionsList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Employee([Bind(Include = "EMail, Password, Name, Surname, Telephone, JobPositionID")] Employees empl)
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CheckUserPermission();
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                {
                    SHA256 sha256 = SHA256.Create();
                    byte[] hashPass = sha256.ComputeHash(Encoding.Default.GetBytes(empl.Password)); //256-bits employee pass
                    string hashPassHex = BitConverter.ToString(hashPass).Replace("-", string.Empty); //64 chars hash pass
                    empl.Password = hashPassHex;
                    empl.EmployeeStateID = 1; // TODO: Eliminate this magic value
                    ctx.Employees.Add(empl);
                    ctx.SaveChanges();
                    return RedirectToAction("Employees", "Admin");
                }
            }
            return View();
        }

        public ActionResult ProjectList()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CheckUserPermission();
            return View();
        }

        public ActionResult ProjectForm()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            CheckUserPermission();
            return View();
        }

        private void CheckUserPermission()
        {
            if ((int)Session["JobPosition"] != 1) // TODO: Replace this magic number with value from DB
            {
                throw new UnauthorizedAccessException("Nie masz wystarczających uprawnień do oglądania tej witryny.");
            }
        }

        private void PopulateJobPositionsList(object selectedJobPosition = null)
        {
            var ctx = new TimesheetDBEntities();
            var jobPositions = from j in ctx.JobPositions
                               orderby j.JobPositionName
                               select j;
            ViewBag.JobPositionID = new SelectList(jobPositions, "JobPositionID", "JobPositionName", selectedJobPosition);
        }
    }
}