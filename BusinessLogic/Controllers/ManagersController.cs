using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace BusinessLogic.Controllers
{
    [Authorize(Roles = "Manager, Both")]
    [Route("api/Managers")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private IManagerRepo _managerRepo;
        private IUserRepo _userRepo;
        public ManagersController(IManagerRepo managerRepo, IUserRepo userRepo)
        {
            _managerRepo = managerRepo;
            _userRepo = userRepo;
        }

        [HttpGet]
        [Route("GetManager")]
        public IActionResult GetManager()
        {
            var manager = _managerRepo.GetManager(_userRepo.GetUserId());
            if (manager == null)
            {
                return NotFound("Manager with given ID doesn't exist");
            }

            return Ok(manager);
        }

        //Add a new employee under his management
        [HttpPost]
        [Route("Post")]
        public IActionResult AddNewEmployee([FromBody] Employee emp)
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data.");
                }
                else if (!(emp.Gender == "Female" || emp.Gender == "Male"))
                {
                    return BadRequest("Gender must be 'Female' or 'Male'");
                }
                else if (!(emp.JobTitle == "Database Administrator" || emp.JobTitle == "Software Developer" || emp.JobTitle == "Project Manager" || emp.JobTitle == "Software Engineering Director" || emp.JobTitle == "HR Director" || emp.JobTitle == "HR Specialist" || emp.JobTitle == "CEO"))
                {
                    return BadRequest("Job Title must be 'Database Administrator', 'Software Developer', 'Project Manager', 'Software Engineering Director', 'HR Specialist', 'HR Director', or 'CEO'");
                }
                else
                {
                    _managerRepo.AddNewEmployee(_userRepo.GetUserId(), emp);
                    return Ok();
                }
            
        }

        [HttpDelete]
        [Route("Delete/{EmployeeId}")]
        public IActionResult DeleteEmployee(int EmployeeId)
        {
                if (_userRepo.GetUserId() <= 0 || EmployeeId <= 0)
                {
                    return BadRequest("Not a valid id");
                }
                else if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }
                else if(_managerRepo.GetEmployee(_userRepo.GetUserId(), EmployeeId) == null)
                {
                    return NotFound("Employee with given id doesn't exist");
                }

                _managerRepo.DeleteEmployee(_userRepo.GetUserId(), EmployeeId);
                return Ok();

        }

        [HttpPut]
        [Route("Put/{EmployeeId}")]
        public IActionResult UpdateEmployee(int EmployeeId, [FromBody] Employee emp)
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else if (!(emp.Gender == "Female" || emp.Gender == "Male"))
                {
                    return BadRequest("Gender must be 'Female' or 'Male'");
                }
                else if (!(emp.JobTitle == "Database Administrator" || emp.JobTitle == "Software Developer" || emp.JobTitle == "Project Manager" || emp.JobTitle == "Software Engineering Director" || emp.JobTitle == "HR Director" || emp.JobTitle == "HR Specialist" || emp.JobTitle == "CEO"))
                {
                    return BadRequest("Job Title must be 'Database Administrator', 'Software Developer', 'Project Manager', 'Software Engineering Director', 'HR Specialist', 'HR Director', or 'CEO'");
                }

                var isEmployeeExist = _managerRepo.UpdateEmployee(_userRepo.GetUserId(), EmployeeId, emp);
            
                if (isEmployeeExist == false)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                return Ok();
                
        }

        //Review the list of all user under his management 
        [HttpGet]
        [Route("GetEmployees")]
        public IActionResult ListAllEmployees()
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }

                var employees = _managerRepo.ListAllEmployees(_userRepo.GetUserId());

                if (employees.Count == 0)
                {
                    return NotFound("There are no employees under your managmnet");
                }

                return Ok(employees);     
        }

        [HttpGet]
        [Route("GetEmployee/{EmployeeId}")]
        public IActionResult GetEmployee(int EmployeeId)
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }

                var employee = _managerRepo.GetEmployee(_userRepo.GetUserId(), EmployeeId);

                if (employee == null)
                {
                    return NotFound("Employee with given ID doesn't exist");
                }

                return Ok(employee);
        }

        //Review the  list of all vacation’s requests for all employees under his management 
        [HttpGet]
        [Route("GetRequests")]
        public IActionResult ListAllVacationRequests()
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }

                var requests = _managerRepo.ListAllVacationRequests(_userRepo.GetUserId());

                if (requests.Count == 0)
                {
                    return NotFound("There are no vacation requests sent to manager with given ID");
                }

                return Ok(requests);  
        }

        [HttpGet]
        [Route("GetRequest/{RequestId}")]
        public IActionResult GetRequest(int RequestId)
        {
            var request = _managerRepo.GetVacationRequest(_userRepo.GetUserId(), RequestId);
            if(request == null)
            {
                return NotFound("Request with given ID doesn't exist");
            }

            return Ok(request);

        }

        //Approve or reject the pending requests( he must enter a reason for rejection )
        [HttpPatch]
        [Route("SetResponse/{RequestId}")]
        public IActionResult SetResponse(int RequestId, [FromBody] VacationRequestResponse Response)
        {
                if (_managerRepo.GetManager(_userRepo.GetUserId()) == null)
                {
                    return NotFound("Manager with given id doesn't exist");
                }

                var entity = _managerRepo.GetVacationRequest(_userRepo.GetUserId(), RequestId);

                if(entity == null)
                {
                    return NotFound("Request with given id doesn't exist");
                }
                
                else if (!(Response.Status == "Accepted" || Response.Status == "Rejected"))
                {
                    return BadRequest("Status must be 'Accepted' or 'Rejected'");
                }

                else if(Response.Status == "Rejected" && Response.RejectionReason == null)
                {
                    return BadRequest("Reason for rejection is required");
                }

                else if(entity.Status != "Pending")
                {
                    return BadRequest("The Request is already Responded to");
                }

                else
                {
                    _managerRepo.SetResponse(_userRepo.GetUserId(), RequestId, Response);
                    return Ok();
                }   

                
        }

        [HttpGet]
        [Route("GetRequester/{RequestId}")]
        public IActionResult GetRequester(int RequestId)
        {
            var requester = _managerRepo.GetRequester(RequestId);
            if (requester == null)
            {
                return NotFound("Request or Employee with given ID doesn't exist");
            }

            return Ok(requester);
        }

    }
}
