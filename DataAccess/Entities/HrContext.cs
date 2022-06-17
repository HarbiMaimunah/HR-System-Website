using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class HrContext : DbContext
    {
        public HrContext(DbContextOptions<HrContext> options) : base (options)
        {
                
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<VacationRequest> VacationRequests { get; set; }

        public DbSet<User> Users { get; set; }

    }
}
