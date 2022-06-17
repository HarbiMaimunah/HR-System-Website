using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class EmployeeProfile
    {
        public Employee EmployeeInfo { get; set; }
        public Employee ManagerInfo { get; set; }
        public string Layout { get; set; }
    }
}
