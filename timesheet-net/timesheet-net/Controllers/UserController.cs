﻿using System;
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
        public ActionResult Index()
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            var entities = new TimesheetDBEntities();
            return View(entities.Employees.OrderBy(e => e.EmployeeStateID).ToList());
        }

        [HttpGet]
        public ActionResult New()
        {
            if (Session["EmployeeID"] == null)
            {
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
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                AddEmployee(empl);
                return RedirectToAction("", "User");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            using (TimesheetDBEntities ctx = new TimesheetDBEntities())
            {
                var employee = (from empl in ctx.Employees
                               where empl.EmployeeID == id
                               select empl).FirstOrDefault();
                PopulateJobPositionsList(employee.JobPositionID);
                employee.Password = "";
                return View(employee);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID, EMail, Password, Name, Surname, Telephone, JobPositionID")] Employees empl)
        {
            if (Session["EmployeeID"] == null)
            {
                return RedirectToAction("", "Home");
            }
            CheckUserPermission();
            if (empl.EMail != null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                AlterEmployee(empl);
                return RedirectToAction("", "User");
            }
            return View();
        }

        private void AddEmployee(Employees empl)
        {
            using (TimesheetDBEntities ctx = new TimesheetDBEntities())
            {
                var hasher = new Sha256PasswordUtil();
                empl.Password = hasher.hash(empl.Password);
                empl.EmployeeStateID = 1;
                ctx.Employees.Add(empl);
                ctx.SaveChanges();
            }
        }

        private void AlterEmployee(Employees empl)
        {
            using (TimesheetDBEntities ctx = new TimesheetDBEntities())
            {
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
            }
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
            var ctx = new TimesheetDBEntities();
            var jobPositions = from j in ctx.JobPositions
                               orderby j.JobPositionName
                               select j;
            ViewBag.JobPositionID = new SelectList(jobPositions, "JobPositionID", "JobPositionName", selectedJobPosition);
        }
    }
}