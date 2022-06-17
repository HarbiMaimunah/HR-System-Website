using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Entities
{
    public class VacationRequest
    {

        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Vacation Type")]
        public string VacationType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Range(1, 14)]
        [Display(Name = "Vacation Duration")]
        public int? VacationDuration { get; set; }

        #nullable disable
        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        #nullable enable
        [StringLength(255)]
        [Display(Name = "Rejection Reason")]
        public string? RejectionReason { get; set; }

        [StringLength(int.MaxValue)]
        public string? AttachmentName { get; set; }

        [Required]
        public bool IsDraft { get; set; }

        public int EmployeeID { get; set; }
        
        public ICollection<Employee>? Employees { get; set; }

        public VacationRequest()
        {
            Employees = new List<Employee>();
        }
    }
}
