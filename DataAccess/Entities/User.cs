using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Entities
{
    public class User
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public ICollection<Employee>? Employees { get; set; }

        public User()
        {
            Employees = new List<Employee>();
        }

    }
}
