using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Livestock
{
    public class LivestockCreateModel
    {

        [Required(ErrorMessage ="Tên bắt buộc nhập")]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mã động vật bắt buộc nhập")]
        [StringLength(20, ErrorMessage = "ExternalId cannot exceed 20 characters.")]
        public string ExternalId { set; get; }

        [Required(ErrorMessage = "Cân nặng bắt buộc nhập")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public int Weight { set; get; }

        [Required(ErrorMessage = "Giới tính bắt buộc nhập")]
        public bool Gender { set; get; }

        public int HabitantTypeId { set; get; }

        public int FieldId { set; get; }

    }
}
