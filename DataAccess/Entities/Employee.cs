using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Entities
{
    public class Employee
    {

        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        #nullable enable
        [StringLength(6)]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        #nullable disable
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required]
        public bool IsManager { get; set; }

        #nullable enable
        [ForeignKey("EmployeeID")]
        public ICollection<Employee>? Employees { get; set; }
        public int? EmployeeID { get; set; }
        public User? User { get; set; }
        public ICollection<VacationRequest>? VacationRequest { get; set; }
        public Employee()
        {
            VacationRequest = new List<VacationRequest>();
            Employees = new List<Employee>();
            User = new User();
        }
    }
}
