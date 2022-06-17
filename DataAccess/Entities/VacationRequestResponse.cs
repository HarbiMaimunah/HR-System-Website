using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Entities
{
    public class VacationRequestResponse
    {
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(255)]
        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }

    }
}
