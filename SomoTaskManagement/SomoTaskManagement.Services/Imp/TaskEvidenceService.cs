﻿using AutoMapper;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.EvidenceImage;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Common;
using SomoTaskManagement.Services.Interface;
using SomoTaskManagement.Socket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly NotificationFCM _notificationFCM;
        private readonly UploadImageToFirebase _uploadImageToFirebase;

        public TaskEvidenceService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IServiceProvider serviceProvider, NotificationFCM notificationFCM, UploadImageToFirebase uploadImageToFirebase)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _notificationFCM = notificationFCM;
            _uploadImageToFirebase  = uploadImageToFirebase;
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
                    EvidenceType = 0,
                };
                await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidenceCreate);
                await _unitOfWork.RepositoryTaskEvidence.Commit();

                var uploadedImages = await _uploadImageToFirebase.UploadEvidenceImages(taskEvidenceCreate.Id, taskEvidence);

                foreach (var uploadedImage in uploadedImages)
                {
                    uploadedImage.TaskEvidenceId = taskEvidenceCreate.Id;
                }
                _unitOfWork.CommitTransaction();
                await _unitOfWork.RepositoryEvidenceImage.Add(uploadedImages);
                await _unitOfWork.RepositoryEvidenceImage.Commit();
                //var evidenceCount = await CountEvidenceOfTask(taskEvidence.TaskId);

                var task = await _unitOfWork.RepositoryFarmTask.GetById(taskEvidenceCreate.TaskId);
                string message = $"Công việc '{task.Name}' có một báo cáo";
                List<int> memberIds = new List<int>();
                if (task.ManagerId.HasValue)
                {
                    memberIds.Add(task.ManagerId.Value);
                    var managerTokens = await _notificationFCM.GetTokenByMemberIds(memberIds);
                    await _notificationFCM.SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }
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

            var uploadedImages = await _uploadImageToFirebase.UploadEvidenceImages(taskEvidenceUpdate.Id, taskEvidence);

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
            
            var taskEvidence = await _unitOfWork.RepositoryTaskEvidence.GetById(id)?? throw new Exception("Không tìm thấy báo cáo");
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

                if (evidence.ManagerId.HasValue)
                {
                    var manager = await _unitOfWork.RepositoryMember.GetById(evidence.ManagerId.Value);

                    if (manager != null && map.ContainsKey(evidence))
                    {
                        map[evidence].ManagerName = manager.Name;
                        map[evidence].AvatarManager = manager.Avatar;
                    }
                }
                else
                {
                    if (map.ContainsKey(evidence))
                    {
                        map[evidence].ManagerName = "";
                        map[evidence].AvatarManager = "";
                    }
                }

            }

            return map.Values;
        }

        public async Task<int> CountEvidenceOfTask()
        {
            var taskEvidence = await _unitOfWork.RepositoryTaskEvidence.GetData(null);
            return taskEvidence.Count();
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
                EvidenceType =1
            };

            await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }

        public List<object> GetStatusEvidenceDescriptions()
        {
            var results = new List<object>();

            foreach (EvidenceTypeEnum status in Enum.GetValues(typeof(EvidenceTypeEnum)))
            {
                results.Add(new
                {
                    Status = (int)status,
                    Description = GetTaskStatusDescription(status)
                });
            }

            return results;
        }

        public static string GetTaskStatusDescription(EvidenceTypeEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

    }
}
