using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SomoTaskManagement.Domain.Model.Employee;

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

        public async Task<byte[]> ExportEmployeesToExcel(int farmId)
        {
            using (var package = new ExcelPackage())
            {
                var worksheetMaterial = package.Workbook.Worksheets.Add("Material");
                var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
                worksheetMaterial.Cells["B1:G1"].Merge = true;
                worksheetMaterial.Cells[1, 2].Value = $"Thông tin dụng cụ trong trang trại {farm.Name}";
                worksheetMaterial.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetMaterial.Cells[3, 1].Value = "Mã dụng cụ";
                worksheetMaterial.Cells[3, 2].Value = "Tên";
                worksheetMaterial.Cells[3, 3].Value = "Hình ảnh";

                var materials = await _unitOfWork.RepositoryMaterial.GetData(e => e.FarmId == farmId);

                int row = 4;
                foreach (var material in materials)
                {
                    worksheetMaterial.Cells[row, 1].Value = material.Id;
                    worksheetMaterial.Cells[row, 2].Value = material.Name;
                    worksheetMaterial.Cells[row, 3].Value = material.UrlImage;

                    row++;
                }
                worksheetMaterial.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }


        public async Task ImportMaterialFromExcel(Stream excelFileStream, int farmId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                using (var package = new ExcelPackage(excelFileStream))
                {
                    var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        throw new Exception("Tài liệu không hợp lệ");
                    }
                    for (int row = 4; row <= rowCount; row++)
                    {

                        var material = new MaterialImportExcelModel
                        {
                            MaterialName = worksheet.Cells[row, 2].Value?.ToString(),
                            UrlIMage = worksheet.Cells[row, 3].Value?.ToString(),
                        };
                        object materialId = worksheet.Cells[row, 1].Value;

                        if (materialId != null && int.TryParse(materialId.ToString(), out int materialIdInt))
                        {
                            var existMaterial = await _unitOfWork.RepositoryMaterial.GetSingleByCondition(a => a.Id == materialIdInt);

                            if (existMaterial != null)
                            {
                                existMaterial.Name = material.MaterialName;
                                existMaterial.UrlImage = material.UrlIMage;

                                _unitOfWork.RepositoryMaterial.Update(existMaterial);

                                await _unitOfWork.RepositoryEmployee.Commit();
                            }
                        }
                        else
                        {
                            var materialNew = new Material
                            {
                                Name = material.MaterialName,
                                UrlImage = material.UrlIMage,
                                Status = 1,
                                FarmId = farm.Id
                            };
                           
                            await _unitOfWork.RepositoryMaterial.Add(materialNew);
                        }

                    }

                    await _unitOfWork.RepositoryMaterial.Commit();
                    _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception($"Error during mat import: {ex.Message}");
            }
        }

        public static string GetEnumDescription<T>(T enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
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
