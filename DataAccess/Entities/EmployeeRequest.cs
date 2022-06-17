using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class EmployeeRequest
    {
        public VacationRequest Request { get; set; }

        [BindProperty]
        public IFormFile Attachment { get; set; }
    }
}
