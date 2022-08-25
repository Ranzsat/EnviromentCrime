using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EnviromentCrime.Controllers
{
    [Authorize(Roles ="Investigator")]
    public class InvestigatorController : Controller
    {
        private ICrimeRepository repository;
        private IWebHostEnvironment environment;
        private ApplicationDbContext context;
        private IHttpContextAccessor contextAcc;

        public InvestigatorController(ICrimeRepository repo, IWebHostEnvironment envo, ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            repository = repo;
            environment = envo;
            context = ctx;
            contextAcc = cont;
        }

        /**
         * Method responisble for the start page of connected to the Investigator Role
         * Finds the user and gets thier information in the db to be used in the logic or shown
         * on the page
         * 
         * @return the Razorview StartInvestigator, with the repository
         */
        public ViewResult StartInvestigator()
        {
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Utredare";

            //Get the users identity
            var user = contextAcc.HttpContext.User.Identity.Name;

            //Get the employee information from the database
            Employee employee = repository.Employees.Where(ed => ed.EmployeeId.Contains(user)).FirstOrDefault();

            //Put parts of the employee information in different viewbags for use in the view
            ViewBag.Name = employee.EmployeeName;
            ViewBag.Role = employee.RoleTitle;
            ViewBag.EmployeeId = employee.EmployeeId;

            return View(repository);
        }

        /**
         * Method that handles the crime page connected to the Investigator Role
         * Takes the id of the errand and puts it in a viewbag so that
         * the details page gets the right information from the database.
         * 
         * @return the Razorview CrimeInvestigator 
         */
        public ViewResult CrimeInvestigator(string id)
        {
            //Title of the page
            ViewBag.Title = "Småstads Kommun: Brott";

            //RefNumber of the Errand
            ViewBag.ID = id;

            //Gets a list of all employees for use in the razorview
            ViewBag.ListOfStatues = repository.ErrandStatuses;

            //Save the id for use in the help action
            TempData["RefNumber"] = id;
            return View();
        }

        /**
         * Help action that updates the database with the inputs from the investigator user
         * If nothing is chosen by the Investigator user then it nulls and nothing is updated
         */
        public async Task<IActionResult> UpdateErrand(string status, string events, string information, IFormFile loadImage, IFormFile loadSample)
        {
            var errandId = TempData["RefNumber"].ToString();
            Errand errand = repository.Errands.Where(ed => ed.RefNumber.Contains(errandId)).FirstOrDefault();

            //Temporary path for the file
            var tempPathImg = Path.GetTempFileName();
            var tempPathSample = Path.GetTempFileName();

            //If the user uploads an image file
            if(loadImage != null)
            {
                using (var stream = new FileStream(tempPathImg, FileMode.Create))
                {
                    await loadImage.CopyToAsync(stream);
                }

                //Create a unique filename for the image, incase of duplicate names
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + loadImage.FileName;

                //Create a new path for the file
                var path = Path.Combine(environment.WebRootPath, "Upload_Images", uniqueFileName);

                //Move the temp file to the correct directory
                System.IO.File.Move(tempPathImg, path);

                repository.AddErrandPicture(errand, uniqueFileName);
            }

            if(loadSample.Length > 0)
            {
                using (var stream = new FileStream(tempPathSample, FileMode.Create))
                {
                    await loadSample.CopyToAsync(stream);
                }

                //Create a unique filename for the image, incase of duplicate names
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + loadSample.FileName;

                //Create a new path for the file
                var path = Path.Combine(environment.WebRootPath, "Upload_Samples", uniqueFileName);

                //Move the temp file to the correct directory
                System.IO.File.Move(tempPathSample, path);

                repository.AddErrandSample(errand, uniqueFileName);
            }

            //If cases for checking if the different types of data exists
            if (events != null) //If the user wants to add events
            {
                repository.UpdateErrandEvent(errand, events);
            }
            if(information != null) //If the user wants to add informations
            {
                repository.UpdateErrandInformation(errand, information);
            }
            if(status == "Välj") //Chaning nothing if the user doesn't change the status
            {
                Console.WriteLine("Can't have a blank status");
            }
            else if(status != null) //If the user wants to change the status of the errand
            {
                repository.UpdateErrandStatus(errand, status);
            }
            return RedirectToAction("CrimeInvestigator", new { id = errandId });
        }
    }
}
