using Microsoft.AspNetCore.Http;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.EvidenceImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IEvidenceImageService
    {
        Task AddEvidenceImage(EvidenceImage evidenceImage);
        Task DeleteEvidenceImage(EvidenceImage evidenceImage);
        Task<EvidenceImage> GetEvidenceImage(int id);
        Task<IEnumerable<EvidenceImage>> ListEvidenceImage();
        Task UpdateEvidenceImage(EvidenceImage evidenceImage);
        Task<EvidenceImage> UploadEvidenceImage(EvidenceImageModel evidenceImageModel);
    }
}
