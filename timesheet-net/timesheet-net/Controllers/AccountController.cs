using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using timesheet_net.Models;

namespace timesheet_net.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult Login(string email, string passwd)
        {
            using (TimesheetDBEntities ctx = new TimesheetDBEntities())
            {
                byte[] pass = Encoding.Default.GetBytes(passwd); //employee pass in bytes
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashPass = sha256.ComputeHash(pass); //256-bits employee pass
                    string hashPassHex = BitConverter.ToString(hashPass).Replace("-", string.Empty); //64 chars hash pass

                    //get login and pass from DB
                    var empl = ctx.Employees.Where(e => e.EMail == email).FirstOrDefault();
                    if (empl != null)
                    {
                        if (empl.Password == hashPassHex) //user typed proper data
                        {
                            if (empl.LoginNo < 1)
                            {
                                Session["EmployeeID"] = empl.EmployeeID;
                                Session["JobPosition"] = empl.JobPositionID;
                                Session["NameSurname"] = empl.Name.ToString() + " " + empl.Surname.ToString();
                                empl.LastLogin = DateTime.Now;
                                empl.LoginNo = 0;
                                Session["Login"] = null;
                            }
                            else
                            {
                                Session["Login"] = "Blocked";
                                return RedirectToAction("", "Home");
                            }
                        }
                        else //user typed incorrect password
                        {
                            if (empl.LoginNo < 1)
                            {
                                empl.LoginNo += 1;//add one because of failed login attempt
                            }
                            else
                            {
                                Session["Login"] = "Blocked";
                                return RedirectToAction("", "Home");
                            }
                        }
                        ctx.Entry(empl).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
                
            }
            
            return RedirectToAction("", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            if (Session["EmployeeID"] != null)
            {
                Session["EmployeeID"] = null;
            }
            return RedirectToAction("", "Home");
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (Session["EmployeeID"] != null) //user is logged in
            {
                using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                {
                    int employeeID = (int)Session["EmployeeID"];
                    Employees empl = ctx.Employees.Where(x => x.EmployeeID == employeeID).FirstOrDefault();
                    if (empl == null)
                    {
                        return HttpNotFound();
                    }
                    return View(empl);
                }
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID, EMail, Password, Name, Surname, Telephone, JobPositionID, LastLogin, EmployeeState")] Employees empl)
        {
            if (empl.EMail!=null && empl.Name != null && empl.Surname != null && empl.Telephone != null)
            {
                using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                {
                    int employeeID = (int)Session["EmployeeID"];
                    var foundEmpl = ctx.Employees.Where(x => x.EmployeeID == employeeID).FirstOrDefault();
                    string typedEmail = foundEmpl.EMail;
                    if (typedEmail == ctx.Employees.Where(x => x.EMail == typedEmail && x.EmployeeID!=employeeID).Select(x => x.EMail).FirstOrDefault())
                    {
                        ModelState.AddModelError("", "Podany e-mail jest już zajęty");
                    }
                    else
                    {
                        foundEmpl.Name = empl.Name;
                        foundEmpl.Surname = empl.Surname;
                        foundEmpl.Telephone = empl.Telephone;
                        ctx.Entry(foundEmpl).State = EntityState.Modified;
                        ctx.SaveChanges();
                        ViewData["Message"] = "OK";
                    }
                }
            }
            return View(empl);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["EmployeeID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string[] pass) //table of passwords
        {
            if (pass[0] != "" && pass[1] != "" && pass[2] != "")
            {
                if (Session["EmployeeID"] != null)
                {
                    using (TimesheetDBEntities ctx = new TimesheetDBEntities())
                    {
                        int employeeID = (int)Session["EmployeeID"];
                        var foundEmployee = ctx.Employees.Where(x => x.EmployeeID == employeeID).FirstOrDefault(); //employee

                        byte[] oldPassword = Encoding.Default.GetBytes(pass[0]); //employee old pass
                        using (var sha256 = SHA256.Create())
                        {
                            byte[] hashOldPass = sha256.ComputeHash(oldPassword); //256-bits employee pass
                            string hashOldPassHex = BitConverter.ToString(hashOldPass).Replace("-", string.Empty); //64 chars hash pass

                            if (hashOldPassHex == foundEmployee.Password) //user typed proper old pass
                            {
                                if (pass[1] == pass[2]) //user typed twice the same new pass
                                {
                                    byte[] newPass = Encoding.Default.GetBytes(pass[1]);
                                    byte[] hashNewPass = sha256.ComputeHash(newPass);
                                    string hashNewPassHex = BitConverter.ToString(hashNewPass).Replace("-", string.Empty);

                                    foundEmployee.Password = hashNewPassHex;
                                    ctx.Entry(foundEmployee).State = EntityState.Modified;
                                    ctx.SaveChanges();
                                    ViewData["Message"] = "OK";
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Podane hasła nie zgadzają się!");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Podane stare hasło jest nieprawidłowe!");
                            }

                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Przynajmniej jedno z wymaganych pól jest nieuzupełnione!");
                
            }
            return View();
        }
    }
}