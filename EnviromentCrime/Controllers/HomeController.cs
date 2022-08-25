using System;
using System.Collections.Generic;
using System.Linq;
using EnviromentCrime.Infrastructure;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnviromentCrime.Controllers
{
    /**
     * The Home Contoroller class handles all calls to the
     * Views that are in the Home Folder
     * It also has a connection to the repository if Data needs
     * to be transfered to or from it.
     */
    public class HomeController : Controller
    {
        private ICrimeRepository repository;

        public HomeController(ICrimeRepository repo)
        {
            repository = repo;
        }

        /**
         * Controler for the Index View
         * Creates a new session where the input recorded by the
         * user is saved for later use.
         * If the session already exists then the view shows the last 
         * input data in the form
         */
        public ViewResult Index()
        {
            ViewBag.Title = "Småstads Kommun";

            var savedErrand = HttpContext.Session.GetJson<Errand>("IndexErrand");
            if(savedErrand == null)
            {
                return View();
            }
            else
            {
                return View(savedErrand);
            }
        }

        /**
         * Method that uses HttpPost to send the informaiton needed 
         * in the validate view
         */
        [HttpPost]
        public ViewResult Index(Errand errand)
        {
            HttpContext.Session.SetJson("IndexErrand", errand);
            return View("Validate", errand);
        }
        public ViewResult Services()
        {
            ViewBag.Title = "Småstads Kommun: Tjänster";
            return View();
        }
        public ViewResult FAQ()
        {
            ViewBag.Title = "Småstads Kommun: FAQ";
            return View();
        }
        public ViewResult Contact()
        {
            ViewBag.Title = "Småstads Kommun: Kontakt";
            return View();
        }
        /*public ViewResult Login()
        {
            ViewBag.Title = "Småstads Kommun: Logga In";
            return View();
        }*/

        /**
         * Controler for the Validate View
         * Shows the reported crime for validation by the user
         */
        public ViewResult Validate()
        {
            ViewBag.Title = "Småstads Kommun: Validering";
            return View();
        }

        /**
         * Controller for the ThankYou View
         * Uses the data from the session to show the ReferenceNumber of the Errand 
         * that the user added to the DB
         */
        public ViewResult ThankYou()
        {
            ViewBag.Title = "Småstads Kommun: Tackar";
            //Saves the errand from the Session and sets the RefNumber for later use
            ViewBag.RefNumber = repository.SaveErrand(HttpContext.Session.GetJson<Errand>("IndexErrand"));

            //Removes the session
            HttpContext.Session.Remove("IndexErrand");
            return View();
        }

    }
}
