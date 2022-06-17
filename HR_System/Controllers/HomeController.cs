using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Controllers
{
    public class HomeController : Controller
    {
        public static string baseUrl = "https://localhost:44364/api/Users/";

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Register(UserInfo RegisterInfo)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(RegisterInfo), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(baseUrl + "Register", stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["RegistrationError"] = error;
                        return RedirectToAction("Index");
                    }
                }

                return RedirectToAction("Index");

            }
        }

        public async Task<IActionResult> SignIn(UserInfo Login)
        {
            using(var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(Login), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(baseUrl + "Login", stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync(); 

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["LoginError"] = "Incorrect Username or Password";
                        return RedirectToAction("Index");
                    }

                    HttpContext.Session.SetString("Token", token);

                    var url = baseUrl + "GetRole";
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string jsonStr = await client.GetStringAsync(url);

                    HttpContext.Session.SetString("Role", jsonStr);
                    var Role = HttpContext.Session.GetString("Role");

                    if (Role == "Manager")
                    {
                        return RedirectToAction("Profile", "Manager");
                    }

                    else
                    {
                        return RedirectToAction("Profile", "Employee");
                    }
                }  
            }
            
        }

    }
}
