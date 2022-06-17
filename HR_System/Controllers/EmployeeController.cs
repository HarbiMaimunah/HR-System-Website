using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace HR_System.Controllers
{
    public class EmployeeController : Controller
    {
        public static string baseUrl = "https://localhost:44364/api/Employees/";

        public async Task<IActionResult> Profile()
        {
            var employeeProfile = new EmployeeProfile();

            employeeProfile.EmployeeInfo = await EmployeeInfo();
            employeeProfile.ManagerInfo = await ManagerInfo();

            var Role = HttpContext.Session.GetString("Role");
            if(Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            return View(employeeProfile);
        }

        [HttpGet]
        public async Task<Employee> EmployeeInfo()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetEmployee";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);
            return res;
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

        public IActionResult MyVacationsRequests()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            return View();
        }

        [HttpGet]
        public async Task<List<VacationRequest>> GetVacationsRequestsFromApi()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "ListRequestedVacations";
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
                var requestData = (from temprequest in requests select temprequest); //Gets an IQueryable of the DataSource

                if (!string.IsNullOrEmpty(searchValue)) //Searching. Here we will search through each column.
                {
                    requestData = requestData.Where(r => r.VacationType.Contains(searchValue)
                                                || r.StartDate.ToString().Contains(searchValue)
                                                || r.EndDate.ToString().Contains(searchValue)
                                                || r.VacationDuration.ToString().Contains(searchValue)
                                                || r.Status.Contains(searchValue));
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

        public IActionResult RequestsInDraft()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            return View();
        }

        [HttpGet]
        public async Task<List<VacationRequest>> GetDraftRequestsFromApi()
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "ListRequestsInDraft";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<VacationRequest>>(jsonStr);

            return res;
        }

        [HttpPost]
        public async Task<IActionResult> GetRequestsInDraft()
        {
            try
            {
                var requests = await GetDraftRequestsFromApi();

                var draw = Request.Form["draw"].FirstOrDefault(); //The Number of times the API is called for the current datatable.
                var start = Request.Form["start"].FirstOrDefault(); //The count of records to skip. This will be used while Paging in EFCore
                var length = Request.Form["length"].FirstOrDefault(); //The page size. See the Top Dropdown in the Jquery Datatable that says, ‘Showing n entries’.
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); //The Value from the Search Box
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var requestData = (from temprequest in requests select temprequest); //Gets an IQueryable of the DataSource

                if (!string.IsNullOrEmpty(searchValue)) //Searching. Here we will search through each column.
                {
                    requestData = requestData.Where(r => r.VacationType.Contains(searchValue)
                                                || r.StartDate.ToString().Contains(searchValue)
                                                || r.EndDate.ToString().Contains(searchValue)
                                                || r.VacationDuration.ToString().Contains(searchValue)
                                                || r.Status.Contains(searchValue));
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

        public IActionResult NewRequest()
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewRequest(EmployeeRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = baseUrl + "RequestNewVacation";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string uniqueFileName = null;

                if (req.Attachment != null)
                {
                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    dirPath = Path.GetDirectoryName(dirPath);
                    var path = Path.GetFullPath(Path.Combine(dirPath, "\\Users\\maimu\\OneDrive\\سطح المكتب\\C-Sharp\\HR_System\\DataAccess\\Attachments"));

                    string uploadsFolder = Path.Combine(path);
                  
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + req.Attachment.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        req.Attachment.CopyTo(fileStream);
                    }
                }

                req.Request.AttachmentName = uniqueFileName;

                if(req.Request.StartDate == null || req.Request.EndDate == null)
                {
                    TempData["NewRequestError"] = "Set a Duration for the Vacation";
                    return RedirectToAction("NewRequest");
                }
                if(req.Request.EndDate < req.Request.StartDate)
                {
                    TempData["NewRequestError"] = "End Date Cannot be Before Start Date. Please Try Again";
                    return RedirectToAction("NewRequest");
                }
                
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(req.Request), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(url, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["NewRequestError"] = error;
                        return RedirectToAction("NewRequest");
                    }

                }
            }

            return RedirectToAction("MyVacationsRequests");
        }

        [HttpPost]
        public async Task<IActionResult> SaveRequestAsDraft(EmployeeRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = baseUrl + "SaveRequestAsDraft";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string uniqueFileName = null;

                if (req.Attachment != null)
                {
                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    dirPath = Path.GetDirectoryName(dirPath);
                    var path = Path.GetFullPath(Path.Combine(dirPath, "\\Users\\maimu\\OneDrive\\سطح المكتب\\C-Sharp\\HR_System\\DataAccess\\Attachments"));

                    string uploadsFolder = Path.Combine(path);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + req.Attachment.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        req.Attachment.CopyTo(fileStream);
                    }
                }

                req.Request.AttachmentName = uniqueFileName;

                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(req.Request), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(url, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["NewRequestError"] = error;
                        return RedirectToAction("NewRequest");
                    }

                }
            }

            return RedirectToAction("RequestsInDraft");
        }

        public async Task<IActionResult> RequestDetails(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            var request = await GetRequestDetails(id);
            return View(request);
        }

        [HttpGet]
        public async Task<VacationRequest> GetRequestDetails(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetRequest/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<VacationRequest>(jsonStr);
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

        public async Task<IActionResult> EditRequest(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            var empReq = new EmployeeRequest()
            {
                Request = await GetRequestInDraft(id)
            };

            return View(empReq);
        }

        public async Task<IActionResult> DraftDetails(int id)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == "Both")
            {
                ViewBag.Message = "~/Views/Shared/_BothLayout.cshtml";
            }
            else
            {
                ViewBag.Message = "~/Views/Shared/_EmployeeLayout.cshtml";
            }

            var request = await GetRequestInDraft(id);
            return View(request);
        }

        [HttpGet]
        public async Task<VacationRequest> GetRequestInDraft(int id)
        {
            var accessToken = HttpContext.Session.GetString("Token");
            var url = baseUrl + "GetRequestInDraft/" + id.ToString();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<VacationRequest>(jsonStr);
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequest(EmployeeRequest model)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = baseUrl + "UpdateRequest/" + (model.Request.ID).ToString();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string uniqueFileName = null;

                if (model.Attachment != null)
                {
                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    dirPath = Path.GetDirectoryName(dirPath);
                    var path = Path.GetFullPath(Path.Combine(dirPath, "\\Users\\maimu\\OneDrive\\سطح المكتب\\C-Sharp\\HR_System\\DataAccess\\Attachments"));

                    string uploadsFolder = Path.Combine(path);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Attachment.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Attachment.CopyTo(fileStream);
                    }
                }

                model.Request.AttachmentName = uniqueFileName;

                var stringContent = new StringContent(JsonConvert.SerializeObject(model.Request), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync(url, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["DraftUpdateError"] = error;
                    }
                }

            }

            return RedirectToAction("RequestsInDraft");

        }
        /*
        [HttpPost]
        public async Task<IActionResult> SubmitDraft(EmployeeRequest model)
        {
            if(model.Attachment != null)
            {
                if(model.Request.AttachmentName != null)
                {
                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    dirPath = Path.GetDirectoryName(dirPath);
                    var path = Path.GetFullPath(Path.Combine(dirPath, "\\Users\\maimu\\OneDrive\\سطح المكتب\\C-Sharp\\HR_System\\DataAccess\\Attachments"));

                    string filePath = Path.Combine(path, model.Request.AttachmentName);
                    System.IO.File.Delete(filePath);
                }

                model.Request.AttachmentName = DownloadAttachment(model.Request.AttachmentName);

            }

            using (var httpClient = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("Token");
                var url = "https://localhost:44364/api/Employees/UpdateRequest/" + (model.Request.ID).ToString();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

        }
        */
        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
