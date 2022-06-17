using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repository
{
    public class ManagerRepo : IManagerRepo
    {
        private HrContext _hrContext;

        public ManagerRepo(HrContext hrContext)
        {
            _hrContext = hrContext;

            if (GetManager(1) == null)
            {
                var ceo = new Employee()
                {
                    FirstName = "Noura",
                    LastName = "Mohammed",
                    Gender = "Female",
                    BirthDate = new DateTime(1991, 7, 13),
                    MobileNumber = "055555555",
                    EmailAddress = "ceo@organization.com",
                    JobTitle = "CEO",
                    IsManager = true
                };
  
                _hrContext.Add(ceo);
                _hrContext.SaveChanges();

            }
            

        }

        public Employee GetManager(int ManagerId)
        {
            
            var manager = _hrContext.Employees.Where(e => e.IsManager == true).FirstOrDefault(e => e.ID == ManagerId);
            return manager;

        }

        public Employee GetEmployee(int ManagerId, int EmployeeId)
        {
            var employee = _hrContext.Employees.Where(e => e.EmployeeID == ManagerId).FirstOrDefault(e=> e.ID == EmployeeId);
            return employee;
        }

        //Add a new employee under his management
        public void AddNewEmployee(int ManagerId, Employee emp)
        {
            emp.EmployeeID = ManagerId;
            _hrContext.Employees.Add(emp);
            _hrContext.SaveChanges();

        }

        //not mentioned
        public void DeleteEmployee(int ManagerId, int EmployeeId)
        {
            var employee = GetEmployee(ManagerId, EmployeeId);
            _hrContext.Entry(employee).State = EntityState.Deleted;
            _hrContext.SaveChanges();
        }

        //not mentioned
        public bool UpdateEmployee(int ManagerId, int EmployeeId, Employee emp)
        {
            var employee = GetEmployee(ManagerId, EmployeeId);

            if (employee != null)
            {
                employee.FirstName = emp.FirstName;
                employee.LastName = emp.LastName;
                employee.Gender = emp.Gender;
                employee.BirthDate = emp.BirthDate;
                employee.MobileNumber = emp.MobileNumber;
                employee.EmailAddress = emp.EmailAddress;
                employee.JobTitle = emp.JobTitle;
                employee.IsManager = emp.IsManager;

                _hrContext.SaveChanges();

                return true;
            }

            return false;
        }

        //Review the list of all user under his management
        public List<Employee> ListAllEmployees(int ManagerId)
        {
            var employees = _hrContext.Employees.Where(e => e.EmployeeID == ManagerId).ToList();
            return employees;
        }

        //Review the  list of all vacation’s requests for all employees under his management 
        public List<VacationRequest> ListAllVacationRequests(int ManagerId)
        {
            var employees = ListAllEmployees(ManagerId);
            var vacations = _hrContext.VacationRequests.AsEnumerable().Where(v => employees.Any(e => e.ID == v.EmployeeID) && v.IsDraft == false).ToList();
            return vacations;

        }

        //not mentioned
        public VacationRequest GetVacationRequest(int ManagerId, int RequestId)
        {
            var vacations = ListAllVacationRequests(ManagerId);
            var request = vacations.FirstOrDefault(v => v.ID == RequestId);
            return request;
        }

        public void SetResponse(int ManagerId, int RequestId, VacationRequestResponse Response)
        {
            var request = GetVacationRequest(ManagerId, RequestId);
            if(request != null)
            {
          
                request.Status = Response.Status;
                request.RejectionReason = Response.RejectionReason;

                _hrContext.SaveChanges();

            }

        }

        public Employee GetRequester(int RequestId)
        {
            return _hrContext.Employees.AsEnumerable().Where(e => _hrContext.VacationRequests.Any(v => v.EmployeeID == e.ID)).FirstOrDefault();
        }
    }
}