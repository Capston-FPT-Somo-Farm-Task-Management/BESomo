using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Farm;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class FarmService : IFarmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FarmService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Farm>> ListFarm()
        {
            return await _unitOfWork.RepositoryFarm.GetData(null);
        }

        public async Task<Farm> GetFarmById(int id)
        {

            var farm = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy trang trại");
            }
            return farm;
        }


        public async Task CreateFarm(FarmCreateUpdateModel farm)
        {
            var farmCreate = new Farm
            {
                Name = farm.Name,
                Description = farm.Description,
                FarmArea = farm.FarmArea,
                Address = farm.Address,
                Status = 1
            };
            var urlImage = await UploadImageToFirebaseAsync(farmCreate, farm.ImageFile);
            farmCreate.UrlImage = urlImage;
            await _unitOfWork.RepositoryFarm.Add(farmCreate);
            await _unitOfWork.RepositoryFarm.Commit();
        }

        private async Task<string> UploadImageToFirebaseAsync(Farm farm, IFormFile imageFile)
        {
            var options = new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
            };

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string fileName = $"{farm.Id}_{uniqueIdentifier}";
            string fileExtension = Path.GetExtension(imageFile.FileName);

            var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
               .Child("images")
               .Child("FarmImage")
               .Child(fileName + fileExtension);

            using (var stream = imageFile.OpenReadStream())
            {
                await firebaseStorage.PutAsync(stream);
            }

            var imageUrl = await firebaseStorage.GetDownloadUrlAsync();

            return imageUrl;
        }


        public async Task UpdateFarm(int id, FarmCreateUpdateModel farm)
        {
            var farmUpdate = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farmUpdate == null)
            {
                throw new Exception("Không tìm thấy trang trại");
            }

            farmUpdate.Name = farm.Name;
            farmUpdate.Description = farm.Description;
            farmUpdate.Address = farm.Address;
            farmUpdate.FarmArea = farm.FarmArea;

            _unitOfWork.RepositoryFarm.Update(farmUpdate);
            await _unitOfWork.RepositoryFarm.Commit();

            farmUpdate.UrlImage = await UploadImageToFirebaseAsync(farmUpdate, farm.ImageFile);

            _unitOfWork.RepositoryFarm.Update(farmUpdate);
            await _unitOfWork.RepositoryFarm.Commit();
        }


        public async Task DeleteFarm(Farm farm)
        {
            _unitOfWork.RepositoryFarm.Delete(f => f.Id == farm.Id);
            await _unitOfWork.RepositoryFarm.Commit();
        }

    }
}
