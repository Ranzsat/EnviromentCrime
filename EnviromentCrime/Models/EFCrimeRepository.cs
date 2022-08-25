using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
    /**Repository class for the webbapplication.
     * It holds all the queries and methods for handling
     * the data.
     */
    public class EFCrimeRepository : ICrimeRepository
    {
        private ApplicationDbContext appContext;
        private IHttpContextAccessor contextAcc;

        //Constructor
        public EFCrimeRepository(ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            appContext = ctx;
            contextAcc = cont;
        }

        //Queryables for the needed tables
        public IQueryable<Errand> Errands => appContext.Errands.Include(e => e.Samples).Include(e => e.Pictures);
        public IQueryable<Department> Departments => appContext.Departments;
        public IQueryable<ErrandStatus> ErrandStatuses => appContext.ErrandStatuses;
        public IQueryable<Employee> Employees => appContext.Employees;
        public IQueryable<Sequence> Sequences => appContext.Sequences;

        //Task to get a specific Errand from the DB
        public Task<Errand> GetCrimeDetail(string id)
        {
            return Task.Run(() =>
            {
                var crimeDetail = Errands.Where(ed => ed.RefNumber.Equals(id)).First();
                return crimeDetail;
            });
        }

        /**
         * Method for saving the Errand with the information that the
         * user inputs in the reports
         * 
         * @return the RefNumber of the errand for use in the next view
         */
        public string SaveErrand(Errand errand)
        {
            if(errand.ErrandID == 0)
            {
                //Get the sequence value from the database
                Sequence sequenceValue = appContext.Sequences.FirstOrDefault(sv => sv.Id.Equals(1));
                
                //Uppdate the errand with the sequenceValue and StatusId
                //Also increments the sequence value
                errand.RefNumber = "2020-45-" + sequenceValue.CurrentValue;
                errand.StatusId = "S_A";
                sequenceValue.CurrentValue++;

                //Uppdate the sequenceValue in the table Sequences
                //Add the errand to the table of Errands
                appContext.Sequences.Update(sequenceValue);
                appContext.Errands.Add(errand);

            }
            //Save the changes in the DB
            appContext.SaveChanges();
            return errand.RefNumber;
        }
        
        /**
         * Method for updating an Errand's DepartmentId in the Database
         */
        public void UpdateErrandDepartmentId(Errand errand, string id)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);

                //Checking if id is D00
                if (id.Equals("D00"))
                {
                    Console.WriteLine("Can't add Småstads Kommun as a department");
                }
                //Check if user hasn't chosen a Department
                else if(id == "Välj")
                {
                    Console.WriteLine("Can't add null as a deprtment");
                }
                else if (dbEntry != null)
                {
                    //Replace the departmentId
                    dbEntry.DepartmentId = id;
                    appContext.SaveChanges();
                }   
            }
            catch(Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        /**
         * Method for updating an Errand to add an employee to it
         */
        public void UpdateErrandEmployeeId(Errand errand, string id)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                dbEntry.EmployeeId = id;
                appContext.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        /**
         * Method for updating an errand when the Manager says there should be no need
         * to take an action for said errand
         */
        public void UpdateErrandNoAction(Errand errand, string reason)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);

                dbEntry.StatusId = "S_B";
                dbEntry.InvestigatorInfo = reason;
                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        /**
         * Method for updating the status of an errand to something other
         * than no action required
         */
        public void UpdateErrandStatus(Errand errand, string stausId)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                dbEntry.StatusId = stausId;
                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        public void UpdateErrandEvent(Errand errand, string eventText)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                if(dbEntry.InvestigatorAction == null)
                {
                    dbEntry.InvestigatorAction = eventText;
                }
                else
                {
                    dbEntry.InvestigatorAction = dbEntry.InvestigatorAction + "\n" + eventText;
                }
                
                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        public void UpdateErrandInformation(Errand errand, string information)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                if(dbEntry.InvestigatorInfo == null)
                {
                    dbEntry.InvestigatorInfo = information;
                }
                else
                {
                    dbEntry.InvestigatorInfo =  dbEntry.InvestigatorInfo + "\n" + information;
                }

                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        //Upload picture to an errands collection of pictures
        public void AddErrandPicture(Errand errand, string pictureName)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                
                if (dbEntry != null)
                {
                    Picture picture = new Picture { PictureName = pictureName };
                    appContext.Add(picture);
                    picture.ErrandId = dbEntry.ErrandID;
                }
                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        //Method for uploading a sample
        public void AddErrandSample(Errand errand, string sampleName)
        {
            try
            {
                //See if the errand can be found
                Errand dbEntry = appContext.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);

                if (dbEntry != null)
                {
                    Sample sample = new Sample { SampleName = sampleName };
                    appContext.Add(sample);
                    sample.ErrandId = dbEntry.ErrandID;
                }
                appContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while finding the dbEntry " + e.Message);
            }
        }

        /**
         * Method for querying the Errands in the database and saving them
         * in a new list for use of the Coordinator role
         * 
         * @return sends the errand list back to the controller for use
         */
        public IQueryable<MyErrand> GetAllErrandsCoordinator()
        {
            var errandList = from err in Errands
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId

                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                                into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()

                             join em in Employees on err.EmployeeId equals em.EmployeeId
                                into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()

                             orderby err.RefNumber descending

                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                             };

            return errandList;
        }

        /**
         * Method for queries the Errand table in the database and saving them
         * in a new list for use of the Investigator role
         * Filters out the errands that the investigator is not part of
         * 
         * @return sends the errand list back to the controller for use
         */
        public IQueryable<MyErrand> GetAllErrandsInvestigator(string employeeId)
        {
            var errandList = from err in Errands
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId

                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                                into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()

                             join em in Employees on err.EmployeeId equals em.EmployeeId
                                 into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()

                             where empE.EmployeeId.Equals(employeeId)
                             orderby err.RefNumber descending

                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                             };

            return errandList;
        }

        /**
         * Method for queries the Errand table in the database and saving them
         * in a new list for use of the Manager role
         * Filters out the errands that the manager is not part of
         * 
         * @return sends the errand list back to the controller for use
         */
        public IQueryable<MyErrand> GetAllErrandsManager(string departmentId)
        {
            var errandList = from err in Errands
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId

                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                                into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()
                             where deptE.DepartmentId.Equals(departmentId)

                             join em in Employees on err.EmployeeId equals em.EmployeeId
                                 into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()

                             orderby err.RefNumber descending

                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                             };

            return errandList;
        }

        /**
         * Method that queries the Employee table in the database where and
         * gets all the employees that the manager is in charge of
         * 
         * @return a list of theese employees for later use
         */
        public IQueryable<Employee> GetManagerEmployees()
        {
            var user = contextAcc.HttpContext.User.Identity.Name;
            var managerDetails = Employees.Where(em => em.EmployeeId.Equals(user)).FirstOrDefault();

            var employeeList = from em in Employees
                               where em.DepartmentId.Equals(managerDetails.DepartmentId)
                               orderby em.EmployeeId descending

                               select new Employee
                               {
                                   EmployeeId = em.EmployeeId,
                                   EmployeeName = em.EmployeeName
                               };
            return employeeList;
        }
    }
}
