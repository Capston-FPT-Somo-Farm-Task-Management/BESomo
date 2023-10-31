using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.EvidenceImage;
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
        private readonly IConfiguration _configuration;

        public EvidenceImageService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

        public async Task<EvidenceImage> UploadEvidenceImage(EvidenceImageModel evidenceImageModel)
        {
            var imageEvidence = new EvidenceImage
            {
                TaskEvidenceId = evidenceImageModel.TaskEvidenceId,
            };

            if (evidenceImageModel.ImageFile != null && evidenceImageModel.ImageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                string fileExtension = Path.GetExtension(evidenceImageModel.ImageFile.FileName);

                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                };

                var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                    .Child("images")
                    .Child(fileName + fileExtension);

                await firebaseStorage.PutAsync(evidenceImageModel.ImageFile.OpenReadStream());

                string imageUrl = await firebaseStorage.GetDownloadUrlAsync();

                imageEvidence.ImageUrl = imageUrl;
            }

            return imageEvidence;
        }
    }
}
