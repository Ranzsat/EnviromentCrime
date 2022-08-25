using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Infrastructure;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnviromentCrime.Controllers
{
    [Authorize(Roles ="Coordinator")]
    public class CoordinatorController : Controller
    {
        private ICrimeRepository repository;
        private IHttpContextAccessor contextAcc;

        public CoordinatorController(ICrimeRepository repo, IHttpContextAccessor cont)
        {
            repository = repo;
            contextAcc = cont;
        }

        /**
         * Method responisble for the start page of connected to the Coordinator Role
         * Finds the user and gets thier information in the db to be used in the logic or shown
         * on the page
         * 
         * @return the Razorview StartCoordinator, with the repository
         */
        public ViewResult StartCoordinator()
        {
            //Get the users identity
            var user = contextAcc.HttpContext.User.Identity.Name;

            //Get the employee information from the database
            Employee employee = repository.Employees.Where(ed => ed.EmployeeId.Contains(user)).FirstOrDefault();

            //Put parts of the employee information in different viewbags for use in the view
            ViewBag.Name = employee.EmployeeName;
            ViewBag.Role = employee.RoleTitle;
            
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Samordnare";
            return View(repository);
        }


        /**
         * Method that handles the crime page connected to the Coordinator Role
         * Takes the id of the errand and puts it in a viewbag so that
         * the details page gets the right information from the database.
         * 
         * @return the Razorview CrimeCooridinator 
         */
        public ViewResult CrimeCoordinator(string id)
        {
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Brott";

            //RefNumber of the Errand
            ViewBag.ID = id;

            //Gets a list of all employees for use in the razorview
            ViewBag.ListOfDepartments = repository.Departments;

            //Save the id for use in the help action
            TempData["RefNumber"] = id;
            return View();
        }

        /**
         * Help action that updates the database with the inputs from the Coordinator user
         * If nothing is chosen by the Coordinator user then it nulls and nothing is updated
         */
        public IActionResult AddDepartmentAction(string department)
        {
            var errandId = TempData["RefNumber"].ToString();
            Errand errand = repository.Errands.Where(ed => ed.RefNumber.Contains(errandId)).FirstOrDefault();
            repository.UpdateErrandDepartmentId(errand, department);
            return RedirectToAction("CrimeCoordinator", new {id = errandId });
        }

        /**
         * Controler for the ReportCrime View
         * Creates a new session where the input recorded by the
         * user is saved for later use.
         * If the session already exists then the view shows the last 
         * input data in the form
         */
        public ViewResult ReportCrime()
        {
            ViewBag.Title = "Småstads Kommun: Rapportera";

            var coordErrand = HttpContext.Session.GetJson<Errand>("CoordErrand");
            if (coordErrand == null)
            {
                return View();
            }
            else
            {
                return View(coordErrand);
            }
        }

        /**
         * Method that uses HttpPost to send the informaiton needed 
         * in the validate view
         */
        [HttpPost]
        public ViewResult ReportCrime(Errand errand)
        {
            HttpContext.Session.SetJson("CoordErrand", errand);
            return View("Validate", errand);
        }

        /**
         * Controler for the Validate View
         * Shows the reported crime for validation by the user
         */
        public ViewResult Validate()
        {
            ViewBag.Title = "Småstads Kommun: Validera";
            return View();
        }

        /**
         * Controller for the Thanks View
         * Uses the data from the session to show the ReferenceNumber of the Errand 
         * that the user added to the DB
         */
        public ViewResult Thanks()
        {
            ViewBag.Title = "Småstads Kommun: Tackar";
            //Saves the errand from the Session and sets the RefNumber for later use
            ViewBag.RefNumber = repository.SaveErrand(HttpContext.Session.GetJson<Errand>("CoordErrand"));
            
            //Removes the session
            HttpContext.Session.Remove("CoordErrand");
            return View();
        }
    }
}
