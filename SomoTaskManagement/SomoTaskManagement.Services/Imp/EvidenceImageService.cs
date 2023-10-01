using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class EvidenceImageService : IEvidenceImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvidenceImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<EvidenceImage>> ListEvidenceImage()
        {
            return _unitOfWork.RepositoryEvidenceImage.GetData(null);
        }
        public Task<EvidenceImage> GetEvidenceImage(int id)
        {
            return _unitOfWork.RepositoryEvidenceImage.GetById(id);
        }
        public async Task AddEvidenceImage(EvidenceImage evidenceImage)
        {
            evidenceImage.Status = 1;
            await _unitOfWork.RepositoryEvidenceImage.Add(evidenceImage);
            await _unitOfWork.RepositoryEvidenceImage.Commit();
        }
        public async Task UpdateEvidenceImage(EvidenceImage evidenceImage)
        {
            _unitOfWork.RepositoryEvidenceImage.Update(evidenceImage);
            await _unitOfWork.RepositoryEvidenceImage.Commit();
        }
        public async Task DeleteEvidenceImage(EvidenceImage evidenceImage)
        {
            _unitOfWork.RepositoryEvidenceImage.Delete(a => a.Id == evidenceImage.Id);
            await _unitOfWork.RepositoryEvidenceImage.Commit();
        }
    }
}
