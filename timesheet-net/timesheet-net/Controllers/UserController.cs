using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using timesheet_net.Models;
using timesheet_net.Utils.Security;

namespace timesheet_net.Controllers
{
    public class UserController : Controller
    {
        TimesheetDBEntities ctx = new TimesheetDBEntities();

        public ActionResult Index()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            return View(ctx.Employees.OrderBy(e => e.EmployeeStateID).ToList());
        }

        [HttpGet]
        public ActionResult New()
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            PopulateJobPositionsList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "EMail, Password, Name, Surname, Telephone, JobPositionID")] Employees empl)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                if (AddEmployee(empl))
                    return RedirectToAction("", "User");
                ModelState.AddModelError("EMail", "Użytkownik o takim adresie e-mail już istnieje w systemie.");
            }
            PopulateJobPositionsList();
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var employee = (from empl in ctx.Employees
                            where empl.EmployeeID == id
                            select empl).FirstOrDefault();
            PopulateJobPositionsList(employee.JobPositionID);
            employee.Password = "";
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID, EMail, Password, Name, Surname, Telephone, JobPositionID")] Employees empl)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                if(AlterEmployee(empl))
                    return RedirectToAction("", "User");
                ModelState.AddModelError("EMail", "Użytkownik o podanym adresie e-mail już istnieje w systemie.");
            }
            PopulateJobPositionsList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int userId)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var empl = new Employees { EmployeeID = userId };
            ctx.Employees.Attach(empl);
            ctx.Employees.Remove(empl);
            ctx.SaveChanges();      
            return RedirectToAction("", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActivation(int userId)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var inactState = ctx.EmployeeState
                .Where(es => es.EmployeeStateName == "Niezatrudniony").First().EmployeeStateID;
            var actState = ctx.EmployeeState.
                Where(es => es.EmployeeStateName == "Aktywny").First().EmployeeStateID;
            var vacState = ctx.EmployeeState.
                Where(es => es.EmployeeStateName == "Na urlopie").First().EmployeeStateID;

            var empl = ctx.Employees
                .Where(em => em.EmployeeID == userId).First();
            if (empl.EmployeeStateID == actState ||
                empl.EmployeeStateID == vacState)
            {
                empl.EmployeeStateID = inactState;
            } else
            {
                empl.EmployeeStateID = actState;
            }
            ctx.SaveChanges();
            return RedirectToAction("", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleVacation(int userId)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var actState = ctx.EmployeeState.
                Where(es => es.EmployeeStateName == "Aktywny").First().EmployeeStateID;
            var vacState = ctx.EmployeeState.
                Where(es => es.EmployeeStateName == "Na urlopie").First().EmployeeStateID;

            var empl = ctx.Employees
                .Where(em => em.EmployeeID == userId).First();
            if (empl.EmployeeStateID == actState)
            {
                empl.EmployeeStateID = vacState;
            }
            else if (empl.EmployeeStateID == vacState)
            {
                empl.EmployeeStateID = actState;
            }
            ctx.SaveChanges();
            return RedirectToAction("", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unlock(int userId)
        {
            if (Session["EmployeeID"] == null)
            {
                Session["PleaseLogin"] = true;
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var empl = ctx.Employees.Find(userId);
            if (empl != null)
            {
                empl.LoginNo = 0;
                ctx.Entry(empl).State = EntityState.Modified;
                ctx.SaveChanges();
            }
            return RedirectToAction("", "User");
        }

        private bool AddEmployee(Employees empl)
        {
            if (ctx.Employees.Where(em => em.EMail == empl.EMail).Count() == 0) { 
                var hasher = new Sha256PasswordUtil();
                empl.Password = hasher.hash(empl.Password);
                empl.EmployeeStateID = 1;
                ctx.Employees.Add(empl);
                ctx.SaveChanges();
                return true;
            }
            return false;
        }

        private bool AlterEmployee(Employees empl)
        {
            if (ctx.Employees.Where(em => em.EmployeeID != empl.EmployeeID && em.EMail == empl.EMail).Count() == 0) { 
                var hasher = new Sha256PasswordUtil();
                var r = ctx.Employees.FirstOrDefault(e => e.EmployeeID == empl.EmployeeID);
                r.EMail = empl.EMail;
                r.Name = empl.Name;
                r.Surname = empl.Surname;
                r.Telephone = empl.Telephone;
                r.JobPositionID = empl.JobPositionID;
                if (empl.Password != null)
                {
                    r.Password = hasher.hash(empl.Password);
                }
                ctx.Entry(r).State = EntityState.Modified;
                ctx.SaveChanges();
                return true;
            }
            return false;
        }

        private void CheckUserPermission()
        {
            PermissionUtil perm = new PermissionUtil();
            if (!perm.IsAdministrator((int)Session["JobPosition"]))
            {
                throw new UnauthorizedAccessException("Nie masz wystarczających uprawnień do oglądania tej witryny.");
            }
        }

        private void PopulateJobPositionsList(object selectedJobPosition = null)
        {
            var jobPositions = from j in ctx.JobPositions
                               orderby j.JobPositionName
                               select j;
            ViewBag.JobPositionID = new SelectList(jobPositions, "JobPositionID", "JobPositionName", selectedJobPosition);
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
