using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.EntityFrameworkCore;

namespace EnviromentCrime.Models
{
    public class ApplicationDbContext : DbContext
    {
        //Constructor
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //DbSet-Properties
        public DbSet<Errand> Errands { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ErrandStatus> ErrandStatuses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
    }
}