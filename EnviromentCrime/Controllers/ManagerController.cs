using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnviromentCrime.Controllers
{
    [Authorize(Roles ="Manager")]
    public class ManagerController : Controller
    {
        //Variables
        private ICrimeRepository repository;
        private IHttpContextAccessor contextAcc;

        //Constructor for the class
        public ManagerController(ICrimeRepository repo, IHttpContextAccessor cont)
        {
            repository = repo;
            contextAcc = cont;
        }

        /**
         * Method responisble for the start page of connected to the Manager Role
         * Finds the user and gets thier information in the db to be used in the logic or shown
         * on the page
         * 
         * @return the Razorview StartManager, with the repository
         */
        public ViewResult StartManager()
        {
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Chefer";

            //Get the users identity
            var user = contextAcc.HttpContext.User.Identity.Name;

            //Get the employee information from the database
            Employee employee = repository.Employees.Where(ed => ed.EmployeeId.Contains(user)).FirstOrDefault();

            //Put parts of the employee information in different viewbags for use in the view
            ViewBag.Name = employee.EmployeeName;
            ViewBag.Role = employee.RoleTitle;
            ViewBag.DepartmentId = employee.DepartmentId;

            return View(repository);
        }

        /**
         * Method that handles the crime page connected to the Manager Role
         * Takes the id of the errand and puts it in a viewbag so that
         * the details page gets the right information from the database.
         * 
         * @return the Razorview CrimeManager 
         */
        public ViewResult CrimeManager(string id)
        {
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Brott";

            //RefNumber of the Errand
            ViewBag.ID = id;

            //Gets a list of all employees for use in the razorview
            ViewBag.ListOfEmployees = repository.Employees;
            
            //Save the id for use in the help action
            TempData["RefNumber"] = id;
            return View();
        }

        /**
         * Help action that updates the database with the inputs from the Manager user
         * If nothing is chosen by the Manager user then it nulls and nothing is updated
         */
        public IActionResult AddEmployeeAction(string investigator, bool noAction, string reason)
        {
            //Get the id in temp data
            var errandId = TempData["RefNumber"].ToString();

            //Finds the Errand whos refNumber matches the one from the temp data
            Errand errand = repository.Errands.Where(ed => ed.RefNumber.Contains(errandId)).FirstOrDefault();

            //If statement for the different actions a user can take
            if (noAction == true)
            {
                repository.UpdateErrandNoAction(errand, reason);

            } else if(noAction == false && reason == "Ange motivering")
            {
                Console.WriteLine("Need to make a selection of some sort");
            }
            else
            {
                repository.UpdateErrandEmployeeId(errand, investigator);
            }
            return RedirectToAction("CrimeManager", new { id = errandId });
        }
    }
}
