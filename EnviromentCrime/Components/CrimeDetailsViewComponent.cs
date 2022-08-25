using EnviromentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Components
{
    public class CrimeDetailsViewComponent : ViewComponent
    {
        private ICrimeRepository repository;
        private ApplicationDbContext context;

        public CrimeDetailsViewComponent(ICrimeRepository repo, ApplicationDbContext ctx)
        {
            repository = repo;
            context = ctx;
        }
        //Async task that loads the Default ViewComponent in CrimeDetails
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var crimeDetail = await repository.GetCrimeDetail(id);

            return View(crimeDetail);
        }
    }
}
