using EnviromentCrime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
    /**
     * Interface for the Repository
     */
    public interface ICrimeRepository
    {
        //Queries for getting data for the views
        //Read
        IQueryable<Errand> Errands { get; }
        IQueryable<Department> Departments { get; }
        IQueryable<ErrandStatus> ErrandStatuses { get; }
        IQueryable<Employee> Employees { get; }
        IQueryable<Sequence> Sequences { get; }

        //Task that get the specific Errand for the Default CrimeDetail depending on its ErrandId
        Task<Errand> GetCrimeDetail(string id);

        //Save an errand to the DB
        string SaveErrand(Errand errand);

        void UpdateErrandDepartmentId(Errand errand, string id);
        
        //Add an employee to an errand and update the db
        void UpdateErrandEmployeeId(Errand errand, string id);

        //Change the status to have no action
        void UpdateErrandNoAction(Errand errand, string reason);

        //Update the status of an errand
        void UpdateErrandStatus(Errand errand, string stausId);

        //Update the errand event
        void UpdateErrandEvent(Errand errand, string eventText);

        //Update the errand information
        void UpdateErrandInformation(Errand errand, string information);

        //Add the files to the errand
        void AddErrandPicture(Errand errand, string pictureName);
        void AddErrandSample(Errand errand, string sampleName);

        //Methods for querying the db and getting the different start components for the differnet roles
        IQueryable<MyErrand> GetAllErrandsCoordinator();
        IQueryable<MyErrand> GetAllErrandsInvestigator(string employeeId);
        IQueryable<MyErrand> GetAllErrandsManager(string departmentId);

        //Query for getting the employees under a single manager
        IQueryable<Employee> GetManagerEmployees();
    }
}
