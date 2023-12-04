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

    public class Farm : BaseEntity
    {
        public Farm()
        {
            Areas = new HashSet<Area>();
            Members = new HashSet<Member>();
            Employees = new HashSet<Employee>();
            HabitantTypes = new HashSet<HabitantType>();
            Materials = new HashSet<Material>();
        }

        [Required(ErrorMessage = "Diện tích phải nhập.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Diện tích phải lớn hơn 0")]
        public double FarmArea { get; set; }

        [Required(ErrorMessage = "Địa chỉ phải nhập")]

        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 kí tự")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Hình ảnh phải nhập.")]
        public string UrlImage { get; set; }
        [Required(ErrorMessage = "Mô tả phải nhập.")]
        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<Area> Areas { get; set; }

        [JsonIgnore]
        public ICollection<Member> Members { get; set; }

        [JsonIgnore]
        public ICollection<HabitantType>? HabitantTypes { get; set; }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; }

        [JsonIgnore]
        public ICollection<Material>? Materials { get; set; }
    }
}
