using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Zone
{
    public class ZoneCreateUpdateModel
    {
        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Tên chỉ được chứa chữ cái")]
        [StringLength(100, ErrorMessage = "Tên không vượt quá 100 kí tự")]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Diện tích bắt buôc nhập")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FarmArea { get; set; }
        [Required(ErrorMessage = "Loại vùng bắt buộc nhập")]
        public int ZoneTypeId { get; set; }
        [Required(ErrorMessage = "Khu vực bắt buộc nhập")]
        public int AreaId { get; set; }
    }
}
