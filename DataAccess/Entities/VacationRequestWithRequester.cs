using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class VacationRequestWithRequester
    {
        public VacationRequest VacationRequest { get; set; }
        public Employee Requester { get; set; }
    }
}
