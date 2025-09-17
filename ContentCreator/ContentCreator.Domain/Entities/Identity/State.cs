using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCreator.Domain.Entities.Identity
{
    public class State
    {
        public Guid Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
