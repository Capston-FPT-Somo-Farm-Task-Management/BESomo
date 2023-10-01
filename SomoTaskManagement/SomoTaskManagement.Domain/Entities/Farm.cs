using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    
    public class Farm :BaseEntity
    {
        public Farm()
        {
            Areas = new HashSet<Area>();
            Members = new HashSet<Member>();
            Employees = new HashSet<Employee>();
        }

        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FarmArea { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [JsonIgnore]
        public ICollection<Area> Areas { get; set; }

        [JsonIgnore]
        public ICollection<Member> Members { get; set; }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; }
    }
}
