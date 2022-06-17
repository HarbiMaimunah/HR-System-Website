using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.IRepository
{
    public interface IUserRepo
    {
        public Employee GetEmployee(string email);
        public void Register(User User, Employee Employee);
        public bool isAlreadyRegistered(string email);
        public User AuthenticateUser(UserInfo Login);
        public Employee GetRegisteredEmployee(int UserID);
        public int GetUserId();
        public string GetUserRole();

    }
}
