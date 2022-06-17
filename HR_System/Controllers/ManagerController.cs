using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HR_System.Controllers
{
    public class ManagerController : Controller
    {
        public static string baseUrl = "https://localhost:44364/api/Managers/";

        public async Task<IActionResult> Profile()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if(Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            var managerInfo = await ManagerInfo();
            return View(managerInfo);
        }

        [HttpGet]
        public async Task<Employee> ManagerInfo()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetManager";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);
            return res;
        }

        public async Task<IActionResult> Employees()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            return View();
        }

        [HttpGet]
        public async Task<List<Employee>> GetEmployeesFromApi()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetEmployees";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<Employee>>(jsonStr);

            return res;
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await GetEmployeesFromApi();

                var draw = Request.Form["draw"].FirstOrDefault(); //The Number of times the API is called for the current datatable.
                var start = Request.Form["start"].FirstOrDefault(); //The count of records to skip. This will be used while Paging in EFCore
                var length = Request.Form["length"].FirstOrDefault(); //The page size. See the Top Dropdown in the Jquery Datatable that says, ‘Showing n entries’.
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); //The Value from the Search Box
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var employeeData = (from tempemployee in employees select tempemployee); //Gets an IQueryable of the DataSource
                
                if (!string.IsNullOrEmpty(searchValue)) //Searching. Here we will search through each column.
                {
                    employeeData = employeeData.Where(e => e.FirstName.Contains(searchValue)
                                                || e.LastName.Contains(searchValue)
                                                || e.MobileNumber.Contains(searchValue)
                                                || e.EmailAddress.Contains(searchValue)
                                                || e.JobTitle.Contains(searchValue));
                } //Searching. Here we will search through each column.
                recordsTotal = employeeData.Count(); //Gets the total count of the Records.
                var data = employeeData.Skip(skip).Take(pageSize).ToList(); //Performs paging using EFCore
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }; //Sets the data in the required format and returns it.
                return Ok(jsonData); //Sets the data in the required format and returns it.

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult CreateEmployee()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee emp)
        {
            using(var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = baseUrl + "Post";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                emp.User = null;
                if (emp.JobTitle == "Software Engineering Director" || emp.JobTitle == "HR Director" || emp.JobTitle == "CEO")
                {
                    emp.IsManager = true;
                }
                else
                {
                    emp.IsManager = false;
                }
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(emp), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(url, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["EmployeeRecordCreationError"] = error;
                        return RedirectToAction("CreateEmployee");
                    }
                        
                }
            }

                return RedirectToAction("Employees");
        }

        public async Task<IActionResult> EditEmployee(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            var emp = await GetEmployee(id);
            return View(emp);
        }

        [HttpGet]
        public async Task<Employee> GetEmployee(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetEmployee/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee emp)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = baseUrl + "Put/" + (emp.ID).ToString();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
                emp.User = null;

                if (emp.JobTitle == "Software Engineering Director" || emp.JobTitle == "HR Director" || emp.JobTitle == "CEO")
                {
                    emp.IsManager = true;
                }
                else
                {
                    emp.IsManager = false;
                }

                var stringContent = new StringContent(JsonConvert.SerializeObject(emp), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync(url, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["EmployeeRecordUpdateError"] = error;
                    }
                }

            }

            return RedirectToAction("Employees");
            
        }

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "Delete/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            await client.DeleteAsync(url);
            return RedirectToAction("Employees");
        }

        public IActionResult VacationsRequests()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            return View();
        }

        [HttpGet]
        public async Task<List<VacationRequest>> GetVacationsRequestsFromApi()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetRequests";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<VacationRequest>>(jsonStr);

            return res;
        }

        [HttpPost]
        public async Task<IActionResult> GetVacationsRequests()
        {
            try
            {
                var requests = await GetVacationsRequestsFromApi();

                var draw = Request.Form["draw"].FirstOrDefault(); //The Number of times the API is called for the current datatable.
                var start = Request.Form["start"].FirstOrDefault(); //The count of records to skip. This will be used while Paging in EFCore
                var length = Request.Form["length"].FirstOrDefault(); //The page size. See the Top Dropdown in the Jquery Datatable that says, ‘Showing n entries’.
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); //The Value from the Search Box
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var requestData = (from tempemployee in requests select tempemployee); //Gets an IQueryable of the DataSource

                if (!string.IsNullOrEmpty(searchValue)) //Searching. Here we will search through each column.
                {
                    requestData = requestData.Where(r => r.VacationType.Contains(searchValue)
                                                || r.StartDate.ToString().Contains(searchValue)
                                                || r.EndDate.ToString().Contains(searchValue)
                                                || r.VacationDuration.ToString().Contains(searchValue)
                                                || r.Status.Contains(searchValue)
                                                || r.RejectionReason.Contains(searchValue));
                } //Searching. Here we will search through each column.
                recordsTotal = requestData.Count(); //Gets the total count of the Records.
                var data = requestData.Skip(skip).Take(pageSize).ToList(); //Performs paging using EFCore
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }; //Sets the data in the required format and returns it.
                return Ok(jsonData); //Sets the data in the required format and returns it.

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> VacationRequestDetails(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            var RequestWithRequester = new VacationRequestWithRequester();
            RequestWithRequester.VacationRequest = await RequestDetails(id);
            RequestWithRequester.Requester = await RequesterDetails(id);
            return View(RequestWithRequester);
        }

        [HttpGet]
        public async Task<VacationRequest> RequestDetails(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetRequest/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<VacationRequest>(jsonStr);
            return res;
        }

        [HttpGet]
        public async Task<Employee> RequesterDetails(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetRequester/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);
            return res;
        }

        public FileResult DownloadAttachment(string fileDownloadName)
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = Path.GetDirectoryName(dirPath);
            var path = Path.GetFullPath(Path.Combine(dirPath, "\\Users\\maimu\\OneDrive\\سطح المكتب\\C-Sharp\\HR_System\\DataAccess\\Attachments"));

            string uploadsFolder = Path.Combine(path);

            var file = uploadsFolder + "\\" + fileDownloadName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(file);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName);
        }

        public async Task<IActionResult> VacationRequestResponse(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Manager")
            {
                ViewBag.Message = "~/Views/Shared/_ManagerLayout.cshtml";
            }
            else if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }

            var res = new VacationRequestResponse()
            {
                ID = id
            };

            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> RespondToRequest(VacationRequestResponse res)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "SetResponse/" +(res.ID).ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if(res.Status == "Rejected" && res.RejectionReason == null)
            {
                TempData["VacationRequestResponseError"] = "Rejection Reason is Required";
                return RedirectToAction("VacationRequestResponse", new { id = res.ID });
            }
            else if(res.Status == "Accepted" && res.RejectionReason != null)
            {
                TempData["VacationRequestResponseError"] = "Response with Accepted Status Must Not Contain Rejection Reason";
                return RedirectToAction("VacationRequestResponse", new { id = res.ID });
            }

            var stringContent = new StringContent(JsonConvert.SerializeObject(res), Encoding.UTF8, "application/json");
            using(var response = await client.PatchAsync(url, stringContent))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    TempData["VacationRequestResponseError"] = error;
                    return RedirectToAction("VacationRequestResponse");
                }
            }
            
            return RedirectToAction("VacationsRequests");
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
