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
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var entities = new TimesheetDBEntities();
            return View(entities.Employees.ToList());
        }

        [HttpGet]
        public ActionResult Employee(int? id)
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            if (id == null)
            {
                PopulateJobPositionsList();
                return View();
            }
            using (TimesheetDBEntities ctx = new TimesheetDBEntities())
            {
                var employee = (from empl in ctx.Employees
                               where empl.EmployeeID == id
                               select empl).FirstOrDefault();
                PopulateJobPositionsList(employee.JobPositionID);
                return View(employee);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Employee([Bind(Include = "EmployeeID, EMail, Password, Name, Surname, Telephone, JobPositionID")] Employees empl)
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                {
                    if (empl.EmployeeID == 0)
                    {
                        SHA256 sha256 = SHA256.Create();
                        byte[] hashPass = sha256.ComputeHash(Encoding.Default.GetBytes(empl.Password)); //256-bits employee pass
                        string hashPassHex = BitConverter.ToString(hashPass).Replace("-", string.Empty); //64 chars hash pass
                        empl.Password = hashPassHex;
                        empl.EmployeeStateID = 1; // TODO: Eliminate this magic value
                        ctx.Employees.Add(empl);
                    } else
                    {
                        var r = ctx.Employees.FirstOrDefault(e => e.EmployeeID == empl.EmployeeID);
                        r.EMail = empl.EMail;
                        r.Name = empl.Name;
                        r.Surname = empl.Surname;
                        r.Telephone = empl.Telephone;
                        r.JobPositionID = empl.JobPositionID;
                        if (r.Password != empl.Password)
                        {
                            SHA256 sha256 = SHA256.Create();
                            byte[] hashPass = sha256.ComputeHash(Encoding.Default.GetBytes(empl.Password)); //256-bits employee pass
                            string hashPassHex = BitConverter.ToString(hashPass).Replace("-", string.Empty); //64 chars hash pass
                            r.Password = hashPassHex;
                        }
                        ctx.Entry(r).State = EntityState.Modified;
                    }                    
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
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            return View();
        }

        public ActionResult ProjectForm()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
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