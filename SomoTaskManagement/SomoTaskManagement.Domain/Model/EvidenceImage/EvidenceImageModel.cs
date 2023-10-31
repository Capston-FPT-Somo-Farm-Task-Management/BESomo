using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SomoTaskManagement.Domain.Model.EvidenceImage
{
    public class EvidenceImageModel
    {
        [FromForm]
        public IFormFile ImageFile { get; set; }
        public int TaskEvidenceId { set; get; }
    }
}
