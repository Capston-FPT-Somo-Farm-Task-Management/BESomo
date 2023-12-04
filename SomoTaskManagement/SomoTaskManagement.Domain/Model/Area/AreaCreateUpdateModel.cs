using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Area
{
    public class AreaCreateUpdateModel
    {
        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Diện tích của trang trại phải lớn hơn 0.")]
        public double FArea { get; set; }
        public int FarmId { get; set; }
        public string Code { get; set; }
    }
}
