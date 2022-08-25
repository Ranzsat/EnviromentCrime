using EnviromentCrime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Components
{
    /**
     * Class for handling the methods that goes into the CrimeList ViewComponent
     */
    public class CrimeListViewComponent : ViewComponent
    {
        private ICrimeRepository repository;

        public CrimeListViewComponent(ICrimeRepository repo)
        {
            repository = repo;
        }

        //Method for invoking the ViewComponent
        public IViewComponentResult Invoke()
        {
            return View("CrimeList", repository);
        }
    }
}
