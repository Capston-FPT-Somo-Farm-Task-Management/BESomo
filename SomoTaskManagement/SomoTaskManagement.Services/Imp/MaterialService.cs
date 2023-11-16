using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MaterialService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<MaterialModel>> ListMaterial()
        {
            var material = await _unitOfWork.RepositoryMaterial.GetData(null);
            material = material.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Material>, IEnumerable<MaterialModel>>(material); ;
        }

        public async Task<IEnumerable<MaterialModel>> ListMaterialByFarm(int farmid)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmid) ?? throw new Exception("KHông tìm thấy trang trại");
            var material = await _unitOfWork.RepositoryMaterial.GetData(m=>m.FarmId == farmid);
            material = material.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Material>, IEnumerable<MaterialModel>>(material); ;
        }

        public async Task<IEnumerable<MaterialModel>> ListMaterialActive(int farmid)
        {
            var material = await _unitOfWork.RepositoryMaterial.GetData(expression: m => m.Status == 1 && m.FarmId == farmid);
            material = material.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Material>, IEnumerable<MaterialModel>>(material); ;
        }

        public Task<Material> GetMaterial(int id)
        {
            return _unitOfWork.RepositoryMaterial.GetById(id);
        }

        public async Task AddMaterial(MaterialCreateUpdateModel material)
        {
            var materialNew = new Material
            {
                Name = material.Name,
                Status = 1,
                FarmId = material.FarmId,
            };

            if (material.ImageFile != null)
            {
                var urlImage = await UploadImageToFirebaseAsync(materialNew, material.ImageFile);
                materialNew.UrlImage = urlImage;
            }
            else
            {
                materialNew.UrlImage = "default_image_url";
            }

            await _unitOfWork.RepositoryMaterial.Add(materialNew);
            await _unitOfWork.RepositoryMaterial.Commit();
        }


        private async Task<string> UploadImageToFirebaseAsync(Material material, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                // Thực hiện tải lên khi ImageFile không null
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                };

                string uniqueIdentifier = Guid.NewGuid().ToString();
                string fileName = $"{material.Id}_{uniqueIdentifier}";
                string fileExtension = Path.GetExtension(imageFile.FileName);

                var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                    .Child("images")
                    .Child("employeeavatar")
                    .Child(fileName + fileExtension);

                using (var stream = imageFile.OpenReadStream())
                {
                    await firebaseStorage.PutAsync(stream);
                }

                var imageUrl = await firebaseStorage.GetDownloadUrlAsync();
                return imageUrl;
            }
            else
            {
                return "default_image_url";
            }
        }



        public async Task UpdateMaterial(int id, MaterialCreateUpdateModel material)
        {
            var materialUpdate = await _unitOfWork.RepositoryMaterial.GetById(id);
            if (materialUpdate != null)
            {
                materialUpdate.Status = 1;
                materialUpdate.Id = id;
                materialUpdate.Name = material.Name;
                materialUpdate.FarmId = material.FarmId;

                var urlImage = material.ImageFile != null
                    ? await UploadImageToFirebaseAsync(materialUpdate, material.ImageFile)
                    : materialUpdate.UrlImage;

                materialUpdate.UrlImage = urlImage;
                await _unitOfWork.RepositoryMaterial.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy vật dụng");
            }
        }

        public async Task DeleteMaterial(Material material)
        {
            _unitOfWork.RepositoryMaterial.Delete(a => a.Id == material.Id);
            await _unitOfWork.RepositoryMaterial.Commit();
        }

        public async Task DeleteByStatus(int id)
        {
            var material = await _unitOfWork.RepositoryMaterial.GetById(id);
            if (material == null)
            {
                throw new Exception("Livestock not found");
            }
            material.Status = material.Status == 1 ? 0 : 1; ;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
