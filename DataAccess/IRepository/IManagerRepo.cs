using DataAccess.Entities;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.IRepository
{
    public interface IManagerRepo
    {
        public Employee GetManager(int ManagerId);
        public Employee GetEmployee(int ManagerId, int EmployeeId);
        void AddNewEmployee(int ManagerId, Employee emp);
        public void DeleteEmployee(int ManagerId, int EmployeeId);
        public bool UpdateEmployee(int ManagerId, int EmployeeId, Employee emp);
        List<Employee> ListAllEmployees(int ManagerId); 
        List<VacationRequest> ListAllVacationRequests(int ManagerId);
        public VacationRequest GetVacationRequest(int ManagerId, int RequestId);
        public void SetResponse(int ManagerId, int RequestId, VacationRequestResponse Response);
        public Employee GetRequester(int RequestId);
    }
}
