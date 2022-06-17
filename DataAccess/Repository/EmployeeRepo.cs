using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repository
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private HrContext _hrContext;

        public EmployeeRepo(HrContext hrContext)
        {
            _hrContext = hrContext;
        }

        //Review his information 
        public Employee GetEmployee(int Id)
        {
            return _hrContext.Employees.FirstOrDefault(e => e.ID == Id);
        }

        //Review his manager information
        public Employee GetManager(int ManagerId)
        {

            var manager = _hrContext.Employees.Where(e => e.IsManager == true).FirstOrDefault(e => e.ID == ManagerId);
            return manager;

        }

        //Request a new vacation 
        public void RequestNewVacation(int EmployeeId, VacationRequest Request)
        {
            Request.IsDraft = false;
            Request.EmployeeID = EmployeeId;
            Request.VacationDuration = (Request.EndDate - Request.StartDate).Days;
            _hrContext.VacationRequests.Add(Request);
            _hrContext.SaveChanges();
        }

        public void SaveRequestAsDraft(int EmployeeId, VacationRequest Request)
        {
            Request.IsDraft = true;
            Request.EmployeeID = EmployeeId;
            Request.VacationDuration = (Request.EndDate - Request.StartDate).Days;
            _hrContext.VacationRequests.Add(Request);
            _hrContext.SaveChanges();
        }

        //Review all the vacations he requested with their statuses
        public List<VacationRequest> ListRequestedVacations(int EmployeeId)
        {
            var requests = _hrContext.VacationRequests.Where(v=> v.EmployeeID == EmployeeId && v.IsDraft == false).ToList();
            return requests;
        }

        public List<VacationRequest> ListRequestsInDraft(int EmployeeId)
        {
            var requests = _hrContext.VacationRequests.Where(v => v.EmployeeID == EmployeeId && v.IsDraft == true).ToList();
            return requests;
        }

        //When he request a vacation he can save it as draft to edit it later or submit it to his manager
        public VacationRequest GetRequest(int EmployeeId, int RequestId)
        {
            var request = ListRequestedVacations(EmployeeId).FirstOrDefault(r => r.ID == RequestId && r.IsDraft == false);
            return request;
        }

        public VacationRequest GetRequestInDraft(int EmployeeId, int RequestId)
        {
            var request = ListRequestsInDraft(EmployeeId).FirstOrDefault(r => r.ID == RequestId && r.IsDraft == true);
            return request;
        }

        public bool UpdateRequestInDraft(VacationRequest Request)
        {
            var req = GetRequestInDraft(Request.EmployeeID, Request.ID);
            if (req != null)
            {
                req.IsDraft = true;
                req.VacationType = Request.VacationType;
                req.StartDate = Request.StartDate;
                req.EndDate = Request.EndDate;
                req.AttachmentName = Request.AttachmentName;
                req.VacationDuration = (req.EndDate - req.StartDate).Days;

                _hrContext.SaveChanges();

                return true;
            }

            return false;
        }

        public bool SubmitRequestInDraft(VacationRequest Request)
        {
            var req = GetRequestInDraft(Request.EmployeeID, Request.ID);
            if (req != null)
            {
                req.IsDraft = false;
                req.VacationType = Request.VacationType;
                req.StartDate = Request.StartDate;
                req.EndDate = Request.EndDate;
                req.AttachmentName = Request.AttachmentName;
                req.VacationDuration = (req.EndDate - req.StartDate).Days;

                _hrContext.SaveChanges();

                return true;
            }

            return false;
        }

        public bool is14LimitExceeded(int EmployeeId)
        {
            var vacationsDurationSum = ListRequestedVacations(EmployeeId).Where(v => v.Status == "Accepted" && v.StartDate.Year == DateTime.Now.Year && v.EndDate.Year == DateTime.Now.Year).Sum(v => v.VacationDuration);
            if(vacationsDurationSum == 14)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isDatesOverlapped(int EmployeeId, VacationRequest Request)
        {
            var isDatesOverlapped = ListRequestedVacations(EmployeeId).Exists(v => (v.Status == "Pending" || v.Status == "Accepted") && (Request.StartDate <= v.StartDate && Request.StartDate >= v.EndDate));
            if (isDatesOverlapped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
