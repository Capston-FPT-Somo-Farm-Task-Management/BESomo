using AutoMapper;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.EvidenceImage;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class TaskEvidenceService : ITaskEvidenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TaskEvidenceService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public Task<IEnumerable<TaskEvidence>> ListTaskEvidence()
        {
            return _unitOfWork.RepositoryTaskEvidence.GetData(null);
        }

        public async Task<TaskEvidenceModel> GetTaskEvidence(int id)
        {
            var evidence = await _unitOfWork.RepositoryTaskEvidence.GetById(id);
            if (evidence == null)
            {
                throw new Exception("Không tìm tháy bằng chứng");
            }
            var evidenceImage = await ListImage(id);

            var evidenceModel = new TaskEvidenceModel
            {
                Id = id,
                SubmitDate = evidence.SubmitDate,
                Status = evidence.Status,
                Description = evidence.Description,
                TaskId = evidence.TaskId,
                UrlImage = evidenceImage,
                Time = FormatTimeDifference(evidence.SubmitDate)
            };

            return evidenceModel;
        }
        public async Task<List<string>> ListImage(int id)
        {
            var imageEvidence = await _unitOfWork.RepositoryEvidenceImage.GetData(expression: i => i.TaskEvidenceId == id);
            return imageEvidence.Select(i => i.ImageUrl).ToList();
        }
        public async Task AddTaskEvidenceeWithImage(EvidenceCreateUpdateModel taskEvidence)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                var taskEvidenceCreate = new TaskEvidence
                {
                    Status = 1,
                    SubmitDate = vietnamTime,
                    Description = taskEvidence.Description,
                    TaskId = taskEvidence.TaskId,
                };
                await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidenceCreate);
                await _unitOfWork.RepositoryTaskEvidence.Commit();

                var uploadedImages = await UploadEvidenceImages(taskEvidenceCreate.Id, taskEvidence);

                foreach (var uploadedImage in uploadedImages)
                {
                    uploadedImage.TaskEvidenceId = taskEvidenceCreate.Id;
                }
                _unitOfWork.CommitTransaction();
                await _unitOfWork.RepositoryEvidenceImage.Add(uploadedImages);
                await _unitOfWork.RepositoryEvidenceImage.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }


        public async Task<List<EvidenceImage>> UploadEvidenceImages(int id, EvidenceCreateUpdateModel evidenceCreateUpdateModel)
        {
            var uploadedImages = new List<EvidenceImage>();

            if (evidenceCreateUpdateModel.ImageFile != null)
            {
                foreach (var imageFile in evidenceCreateUpdateModel.ImageFile)
                {
                    var imageEvidence = new EvidenceImage
                    {
                        TaskEvidenceId = id,
                    };

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        string fileExtension = Path.GetExtension(imageFile.FileName);

                        var options = new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                        };

                        var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                            .Child("images")
                            .Child(fileName + fileExtension);

                        await firebaseStorage.PutAsync(imageFile.OpenReadStream());

                        string imageUrl = await firebaseStorage.GetDownloadUrlAsync();

                        imageEvidence.ImageUrl = imageUrl;
                    }
                    else
                    {
                        imageEvidence.ImageUrl = null;
                    }

                    uploadedImages.Add(imageEvidence);
                }
            }

            return uploadedImages;
        }


        public async Task AddTaskEvidencee(TaskEvidence taskEvidence)
        {

            taskEvidence.Status = 1;
            taskEvidence.SubmitDate = DateTime.Now;


            await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }

        public async Task UpdateTaskEvidence(int id, EvidenceCreateUpdateModel taskEvidence, List<string> oldUrlImages)
        {
            var taskEvidenceUpdate = await _unitOfWork.RepositoryTaskEvidence.GetById(id);
            if (taskEvidenceUpdate == null)
            {
                throw new Exception("Không tìm thấy bằng chứng");
            }
            taskEvidenceUpdate.Description = taskEvidence.Description;

            _unitOfWork.RepositoryEvidenceImage.Delete(expression: i => i.TaskEvidenceId == id);

            var uploadedImages = await UploadEvidenceImages(taskEvidenceUpdate.Id, taskEvidence);

            foreach (var uploadedImage in uploadedImages)
            {
                uploadedImage.TaskEvidenceId = taskEvidenceUpdate.Id;
            }

            await _unitOfWork.RepositoryEvidenceImage.Add(uploadedImages);
            await _unitOfWork.RepositoryEvidenceImage.Commit();
            if (oldUrlImages != null)
            {
                foreach (var oldUrlImage in oldUrlImages)
                {
                    var evidenceImage = new EvidenceImage
                    {
                        TaskEvidenceId = id,
                        ImageUrl = oldUrlImage,
                    };
                    await _unitOfWork.RepositoryEvidenceImage.Add(evidenceImage);
                    await _unitOfWork.RepositoryEvidenceImage.Commit();
                }
            }
            await _unitOfWork.RepositoryTaskEvidence.Commit();

        }



        public async Task DeleteTaskEvidence(int id)
        {
            var taskEvidence = await _unitOfWork.RepositoryTaskEvidence.GetById(id);
            var relatedImages = await _unitOfWork.RepositoryEvidenceImage.GetData(i => i.TaskEvidenceId == taskEvidence.Id);

            foreach (var image in relatedImages)
            {
                _unitOfWork.RepositoryEvidenceImage.Delete(image);
                await _unitOfWork.RepositoryEvidenceImage.Commit();
            }

            _unitOfWork.RepositoryTaskEvidence.Delete(a => a.Id == id);

            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }

        public async Task<IEnumerable<TaskEvidenceModel>> GetEvidenceByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var evidences = await _unitOfWork.RepositoryTaskEvidence.GetData(expression: e => e.TaskId == taskId);
            evidences = evidences.OrderByDescending(e => e.Id).ToList();
            var evidenceIds = evidences.Select(e => e.Id).ToList();

            var image = await _unitOfWork.RepositoryEvidenceImage.GetData(i => evidenceIds.Contains(i.TaskEvidenceId));
            var urlImageMap = image.GroupBy(i => i.TaskEvidenceId)
                                  .ToDictionary(group => group.Key, group => group.Select(i => i.ImageUrl).ToList());

            var map = new Dictionary<TaskEvidence, TaskEvidenceModel>();
            var evidenceModel = _mapper.Map<IEnumerable<TaskEvidence>, IEnumerable<TaskEvidenceModel>>(evidences);

            foreach (var pair in evidences.Zip(evidenceModel, (ft, ftModel) => new { ft, ftModel }))
            {
                map.Add(pair.ft, pair.ftModel);
            }

            foreach (var evidence in evidences)
            {
                if (urlImageMap.ContainsKey(evidence.Id) && map.ContainsKey(evidence))
                {
                    map[evidence].UrlImage = urlImageMap[evidence.Id];
                }

                var time = FormatTimeDifference(evidence.SubmitDate);
                if (time != null)
                {
                    map[evidence].Time = time;
                }
            }

            return map.Values;
        }

        public string FormatTimeDifference(DateTime startTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            TimeSpan timeDifference = vietnamTime - startTime;
            if (timeDifference.TotalDays >= 1)
            {
                int days = (int)timeDifference.TotalDays;
                return $"{days} ngày trước";
            }
            if (timeDifference.TotalHours >= 1)
            {
                int hours = (int)timeDifference.TotalHours;
                return $"{hours} giờ trước";
            }
            if (timeDifference.TotalMinutes >= 1)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                return $"{minutes} phút trước";
            }
            int seconds = (int)timeDifference.TotalSeconds;
            return $"{seconds} giây trước";
        }

        public async Task CreateDisagreeTask(int id, string description)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                task.Status = 5;
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ ");
            }
            var taskEvidence = new TaskEvidence
            {
                Status = 1,
                SubmitDate = DateTime.Now,
                Description = description,
                TaskId = id,
            };

            await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }
    }
}
