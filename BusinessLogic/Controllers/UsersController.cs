using DataAccess.Entities;
using DataAccess.IRepository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace BusinessLogic.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _config;
        private IUserRepo _userRepo;

        public UsersController(IConfiguration config, IUserRepo userRepo)
        {
            _config = config;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] UserInfo RegisterInfo)
        {
            var employee = _userRepo.GetEmployee(RegisterInfo.EmailAddress);
            if (_userRepo.isAlreadyRegistered(RegisterInfo.EmailAddress))
            {
                return BadRequest("You already registered");
            }
            else if(employee != null)
            {
                var User = new User()
                {
                    EmployeeID = employee.ID,
                    Username = RegisterInfo.Username,
                    EmailAddress = employee.EmailAddress,
                    Password = RegisterInfo.Password
                };

                if (employee.IsManager && employee.EmployeeID != null)
                {
                    User.Role = Roles.Both;
                }
                else if (employee.IsManager)
                {
                    User.Role = Roles.Manager;
                }
                else
                {
                    User.Role = Roles.Employee;
                }

                _userRepo.Register(User, employee);
                return Ok();
            }
            else
            {
                return NotFound("Registration failed. There is no employee with the given email address");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserInfo Login)
        {
            IActionResult response = Unauthorized();
            var user = _userRepo.AuthenticateUser(Login);

            if(user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(tokenString);
            }

            return response;
        }

        private string GenerateJSONWebToken(User User)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, User.Username),
                new Claim(JwtRegisteredClaimNames.Email, User.EmailAddress),
                new Claim(ClaimTypes.Role, User.Role),
                new Claim(ClaimTypes.Sid, User.EmployeeID.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet]
        [Route("GetRole")]
        public IActionResult GetRole()
        {
            return Ok(_userRepo.GetUserRole());
        }
    }
}
