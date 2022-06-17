using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.IRepository
{
    public interface IEmployeeRepo
    {
        public Employee GetEmployee(int Id);
        public Employee GetManager(int ManagerId);
        public void RequestNewVacation(int EmployeeId, VacationRequest Request);
        public void SaveRequestAsDraft(int EmployeeId, VacationRequest Request);
        public VacationRequest GetRequest(int EmployeeId, int RequestId);
        public VacationRequest GetRequestInDraft(int EmployeeId, int RequestId);
        public List<VacationRequest> ListRequestedVacations(int EmployeeId);
        public List<VacationRequest> ListRequestsInDraft(int EmployeeId);
        public bool UpdateRequestInDraft(VacationRequest Request);
        public bool SubmitRequestInDraft(VacationRequest Request);
        public bool is14LimitExceeded(int EmployeeId);
        public bool isDatesOverlapped(int EmployeeId, VacationRequest Request);
    }
}
