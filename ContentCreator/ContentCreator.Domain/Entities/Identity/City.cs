using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCreator.Domain.Entities.Identity
{    
    public class City
    {
        public Guid Id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
