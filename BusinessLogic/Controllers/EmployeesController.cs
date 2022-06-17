using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Controllers
{
    [Authorize(Roles = "Employee, Both")]
    [Route("api/Employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeRepo _employeeRepo;
        private IUserRepo _userRepo;
        public EmployeesController(IEmployeeRepo employeeRepo, IUserRepo userRepo)
        {
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
        }

        //Review his information
        [HttpGet]
        [Route("GetEmployee")]
        public IActionResult GetEmployee()
        {
            
                var emp = _employeeRepo.GetEmployee(_userRepo.GetUserId());
                if (emp == null)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                return Ok(emp);
            
        }

        [HttpGet]
        [Route("GetManager")]
        public IActionResult GetManager()
        {
            var emp = _employeeRepo.GetEmployee(_userRepo.GetUserId());
            var managerId = (int)emp.EmployeeID;
            var manager = _employeeRepo.GetManager(managerId);
            if(manager == null)
            {
                return NotFound("Manager with given ID doesn't exist");
            }

            return Ok(manager);
        }

        //Request a new vacation 
        [HttpPost]
        [Route("RequestNewVacation")]
        public IActionResult RequestNewVacation([FromBody] VacationRequest Request)
        {
                if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }
                else if (!(Request.VacationType == "Annual" || Request.VacationType == "Sick" || Request.VacationType == "Leave" || Request.VacationType == "Exceptional"))
                {
                    return BadRequest("Vacation Type must be 'Annual', 'Sick', 'Leave', or 'Exceptional'");
                }
                else if (_employeeRepo.isDatesOverlapped(_userRepo.GetUserId(), Request))
                {
                    return BadRequest("Your request is invalid. Its period is overlapped with an existing request");
                }
                else if (Request.VacationDuration < 1 || Request.VacationDuration > 14)
                {
                    return BadRequest("Vacation Duration must be in between 1 and 14 days");
                }
                else
                {
                    if (!(_employeeRepo.is14LimitExceeded(_userRepo.GetUserId())))
                    {
                        _employeeRepo.RequestNewVacation(_userRepo.GetUserId(), Request);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("You cannot request a vacation, you have exceeded the annual 14-day limit");
                    }
                }
            }                
        
        [HttpPost]
        [Route("SaveRequestAsDraft")]
        public IActionResult SaveRequestAsDraft([FromBody] VacationRequest Request)
        {
            if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
            {
                return NotFound("Employee with given ID doesn't exist");
            }
            else if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            else if (!(Request.VacationType == "Annual" || Request.VacationType == "Sick" || Request.VacationType == "Leave" || Request.VacationType == "Exceptional"))
            {
                return BadRequest("Vacation Type must be 'Annual', 'Sick', 'Leave', or 'Exceptional'");
            }
            else if (_employeeRepo.isDatesOverlapped(_userRepo.GetUserId(), Request))
            {
                return BadRequest("Your request is invalid. Its period is overlapped with an existing request");
            }
            else if (Request.VacationDuration < 1 || Request.VacationDuration > 14)
            {
                return BadRequest("Vacation Duration must be in between 1 and 14 days");
            }
            else
            {
                if (!(_employeeRepo.is14LimitExceeded(_userRepo.GetUserId())))
                {
                    _employeeRepo.SaveRequestAsDraft(_userRepo.GetUserId(), Request);
                    return Ok();
                }
                else
                {
                    return BadRequest("You cannot request a vacation, you have exceeded the annual 14-day limit");
                }
            }
        }

        [HttpPut]
        [Route("UpdateRequest/{RequestId}")]
        public IActionResult UpdateRequest(int RequestId, VacationRequest Request)
        {
                Request.ID = RequestId;
                Request.EmployeeID = _userRepo.GetUserId();

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else if (!(Request.VacationType == "Annual" || Request.VacationType == "Sick" || Request.VacationType == "Leave" || Request.VacationType == "Exceptional"))
                {
                    return BadRequest("Vacation Type must be 'Annual', 'Sick', 'Leave', or 'Exceptional'");
                }
                else if (_employeeRepo.isDatesOverlapped(_userRepo.GetUserId(), Request))
                {
                    return BadRequest("Your request is invalid. Its period is overlapped with an existing request");
                }
                else if (Request.VacationDuration < 1 || Request.VacationDuration > 14)
                {
                    return BadRequest("Vacation Duration must be in between 1 and 14 days");
                }

                if (!(_employeeRepo.is14LimitExceeded(_userRepo.GetUserId())))
                {

                    var isRequestExist = _employeeRepo.UpdateRequestInDraft(Request);

                    if (isRequestExist == false)
                    {
                        return NotFound("Employee with given ID doesn't exist");
                    }

                    return Ok();
                }
                else
                {
                    return BadRequest("You cannot request a vacation, you have exceeded the annual 14-day limit");
                }
               
        }

        [HttpPut]
        [Route("SubmitDraft/{RequestId}")]
        public IActionResult SubmitDraft(int RequestId, VacationRequest Request)
        {
            Request.ID = RequestId;
            Request.EmployeeID = _userRepo.GetUserId();

            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }
            else if (!(Request.VacationType == "Annual" || Request.VacationType == "Sick" || Request.VacationType == "Leave" || Request.VacationType == "Exceptional"))
            {
                return BadRequest("Vacation Type must be 'Annual', 'Sick', 'Leave', or 'Exceptional'");
            }
            else if (_employeeRepo.isDatesOverlapped(_userRepo.GetUserId(), Request))
            {
                return BadRequest("Your request is invalid. Its period is overlapped with an existing request");
            }
            else if (Request.VacationDuration < 1 || Request.VacationDuration > 14)
            {
                return BadRequest("Vacation Duration must be in between 1 and 14 days");
            }

            if (!(_employeeRepo.is14LimitExceeded(_userRepo.GetUserId())))
            {

                var isRequestExist = _employeeRepo.SubmitRequestInDraft(Request);

                if (isRequestExist == false)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                return Ok();
            }
            else
            {
                return BadRequest("You cannot request a vacation, you have exceeded the annual 14-day limit");
            }

        }

        //Review all the vacations he requested with their statuses
        [HttpGet]
        [Route("ListRequestedVacations")]
        public IActionResult ListRequestedVacations()
        {
            
                if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                var requests = _employeeRepo.ListRequestedVacations(_userRepo.GetUserId());

                if (requests.Count == 0)
                {
                    return NotFound("There are no requested vacations");
                }

                return Ok(requests);
              
        }

        [HttpGet]
        [Route("ListRequestsInDraft")]
        public IActionResult ListRequestsInDraft()
        {
            if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
            {
                return NotFound("Employee with given ID doesn't exist");
            }

            var requests = _employeeRepo.ListRequestsInDraft(_userRepo.GetUserId());

            if (requests.Count == 0)
            {
                return NotFound("There are no requested vacations");
            }

            return Ok(requests);
        }

        //When he request a vacation he can save it as draft to edit it later or submit it to his manager
        [HttpGet]
        [Route("GetRequest/{RequestId}")]
        public IActionResult GetRequest(int RequestId)
        {
                if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                var request = _employeeRepo.GetRequest(_userRepo.GetUserId(), RequestId);

                if (request == null)
                {
                    return NotFound("There is no request with given ID");
                }

                return Ok(request);
 
        }

        [HttpGet]
        [Route("GetRequestInDraft/{RequestId}")]
        public IActionResult GetRequestInDraft(int RequestId)
        {
            if (_employeeRepo.GetEmployee(_userRepo.GetUserId()) == null)
            {
                return NotFound("Employee with given ID doesn't exist");
            }

            var request = _employeeRepo.GetRequestInDraft(_userRepo.GetUserId(), RequestId);

            if (request == null)
            {
                return NotFound("There is no request with given ID");
            }

            return Ok(request);
        }
    }
}
