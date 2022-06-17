using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace DataAccess.Repository
{
    public class UserRepo : IUserRepo
    {
        private HrContext _hrContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepo(HrContext hrContext, IHttpContextAccessor httpContextAccessor)
        {
            _hrContext = hrContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Employee GetEmployee(string email)
        {
            var employee = _hrContext.Employees.FirstOrDefault(e => e.EmailAddress == email);
            return employee;
        }

        public void Register(User User, Employee Employee)
        {
            _hrContext.Users.Add(User);
            _hrContext.SaveChanges();
            User.EmployeeID = Employee.ID;
            _hrContext.SaveChanges();

        }

        public bool isAlreadyRegistered(string email)
        {
            if(_hrContext.Users.FirstOrDefault(r => r.EmailAddress == email) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public User AuthenticateUser(UserInfo Login)
        {
            var userRequest = _hrContext.Users.FirstOrDefault(l => l.Username == Login.Username && l.Password == Login.Password);
            
            if(userRequest != null)
            {
                return userRequest;
            }
            else
            {
                return null;
            }
        }

        public Employee GetRegisteredEmployee(int UserID)
        {
            return _hrContext.Employees.FirstOrDefault(e => e.ID == UserID);
        }

        public int GetUserId()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;        
            var stringClaimValue = currentUser.FindFirst(ClaimTypes.Sid).Value;
            var IdNumber = Convert.ToInt32(stringClaimValue);
            return IdNumber;
        }

        public string GetUserRole()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            var stringClaimValue = currentUser.FindFirst(ClaimTypes.Role).Value;
            return stringClaimValue;
        }
    }
}
