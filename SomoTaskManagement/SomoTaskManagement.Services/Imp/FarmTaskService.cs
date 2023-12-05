using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.Internal;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Firebase.Storage;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Interface;
using SomoTaskManagement.Services.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnidecodeSharpFork;
using Notification = SomoTaskManagement.Domain.Entities.Notification;

namespace SomoTaskManagement.Services.Impf
{
    public class FarmTaskService : IFarmTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public FarmTaskService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        private async Task<Dictionary<FarmTask, FarmTaskModel>> MapFarmTasks(IEnumerable<FarmTask> farmTasks)
        {
            var map = new Dictionary<FarmTask, FarmTaskModel>();
            var farmTaskModels = _mapper.Map<IEnumerable<FarmTask>, IEnumerable<FarmTaskModel>>(farmTasks);

            foreach (var pair in farmTasks.Zip(farmTaskModels, (ft, ftModel) => new { ft, ftModel }))
            {
                map.Add(pair.ft, pair.ftModel);
            }

            foreach (var farmTask in farmTasks)
            {
                if (farmTask.SuppervisorId != null)
                {
                    var member = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId.Value);

                    if (map.ContainsKey(farmTask))
                    {
                        if (member != null)
                        {
                            map[farmTask].SupervisorName = member.Name;
                            map[farmTask].AvatarSupervisor = member.Avatar;
                        }
                        else
                        {
                            map[farmTask].SupervisorName = null;
                            map[farmTask].AvatarSupervisor = null;
                        }
                    }
                }

                //if (member != null && map.ContainsKey(farmTask))
                //{
                //    map[farmTask].AvatarSupervisor = member.Avatar;
                //}
                var employeeNames = await ListTaskEmployee(farmTask.Id);

                if (employeeNames != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].EmployeeName = string.Join(", ", employeeNames);
                }

                var employeeIds = await ListTaskEmployeeId(farmTask.Id);

                if (employeeIds != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].EmployeeId = employeeIds;
                }

                var materialName = await ListMaterial(farmTask.Id);

                if (materialName != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].MaterialName = string.Join(", ", materialName);
                }

                var materialId = await ListMaterialId(farmTask.Id);

                if (materialId != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].MaterialId = materialId;
                }
                var evidence = await _unitOfWork.RepositoryTaskEvidence.GetData(f => f.TaskId == farmTask.Id);
                var totalEvidence = evidence.Count();
                if (totalEvidence > 0 && map.ContainsKey(farmTask))
                {
                    map[farmTask].IsHaveEvidence = true;
                }

                var taskRepeate = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == farmTask.Id);
                var dateRepeate = taskRepeate.Select(t => t.StartDate.Value).ToList();
                if (dateRepeate != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].DateRepeate = dateRepeate;
                }

                var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == farmTask.Id && s.Status == true);
                if (subtask.Any() && map.ContainsKey(farmTask))
                {
                    map[farmTask].IsHaveSubtask = true;
                }

            }

            return map;
        }

        public async Task<IEnumerable<FarmTaskModel>> GetList()
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                                        .GetData(expression: null, includes: includes);

            var map = await MapFarmTasks(farmTasks);

            return map.Values;
        }

        public async Task<IEnumerable<TaskCountPerDayModel>> GetTotalTaskOfWeekByMember(int memberId)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Field,
            };
            var member = await _unitOfWork.RepositoryMember.GetById(memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            var allDaysOfWeek = Enumerable.Range(0, 7).Select(day => startOfWeek.AddDays(day)).ToList();

            var role = await CheckRoleMember(memberId);
            var farmTasks = new List<FarmTask>();
            if (role == 1)
            {
                farmTasks = (await _unitOfWork.RepositoryFarmTask.GetData(
                        expression: t => (t.ManagerId == memberId || t.ManagerId == null)
                            && (t.StartDate >= startOfWeek || t.StartDate <= endOfWeek)
                            && t.Status != 0,
                        includes: includes
                    )).ToList();

            }
            else if (role == 3)
            {
                farmTasks = (await _unitOfWork.RepositoryFarmTask.GetData(
                        expression: t => t.SuppervisorId == memberId
                            && (t.StartDate >= startOfWeek || t.StartDate <= endOfWeek)
                            && t.Status != 0,
                        includes: includes
                    )).ToList();

            }

            var taskCounts = allDaysOfWeek
                .Select(day => new TaskCountPerDayModel
                {
                    Date = day.Date,
                    TaskCount = farmTasks.Count(f => f.StartDate.Value.Date == day.Date),
                    TotalTaskOfLivestock = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 1)),
                    TotalTaskOfPlant = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 0)),
                    TotalTaskOfOther = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field == null && !f.FieldId.HasValue)),
                    TotalTaskDoing = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 3)),
                    TotalTaskToDo = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 1)),
                    TotalTaskClose = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 8)),
                    TotalTaskPending = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 5)),
                })
                .ToList();


            return taskCounts;
        }

        public async Task<IEnumerable<TaskCountPerDayModel>> GetTotalTaskOfFarm(int farmId)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Field,
            };
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmId) ?? throw new Exception("Không tìm thấy trang trại");

            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            var allDaysOfWeek = Enumerable.Range(0, 7).Select(day => startOfWeek.AddDays(day)).ToList();

            var memberOfFarm = await _unitOfWork.RepositoryMember.GetData(m => m.FarmId == farmId && m.RoleId == 3);
            var memberIds = memberOfFarm.Select(m => m.Id).ToList();

            var farmTasks = (await _unitOfWork.RepositoryFarmTask.GetData(
                     expression: t => memberIds.Contains(t.SuppervisorId.Value)
                         && (t.StartDate >= startOfWeek || t.StartDate <= endOfWeek)
                         && t.Status != 0,
                     includes: includes
                 )).ToList();


            var taskCounts = allDaysOfWeek
                .Select(day => new TaskCountPerDayModel
                {
                    Date = day.Date,
                    TaskCount = farmTasks.Count(f => f.StartDate.Value.Date == day.Date),
                    TotalTaskOfLivestock = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 1)),
                    TotalTaskOfPlant = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 0)),
                    TotalTaskOfOther = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Field == null && !f.FieldId.HasValue)),
                    TotalTaskDoing = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 3)),
                    TotalTaskToDo = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 1)),
                    TotalTaskClose = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 8)),
                    TotalTaskPending = farmTasks.Count(f => f.StartDate.Value.Date == day.Date && (f.Status == 5)),
                })
                .ToList();


            return taskCounts;
        }


        public async Task<TotalTaskOfMonth> GetTotalTaskOfFarmIncurrentMonth(int farmId, int month)
        {
            if (month < 0 || month > 12)
            {
                throw new Exception("Tháng không hợp lệ ");
            }
            var numberOfDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Field,
            };
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmId) ?? throw new Exception("Không tìm thấy trang trại");

            var memberOfFarm = await _unitOfWork.RepositoryMember.GetData(m => m.FarmId == farmId && m.RoleId == 3);
            var memberIds = memberOfFarm.Select(m => m.Id).ToList();

            var farmTasks = (await _unitOfWork.RepositoryFarmTask.GetData(
                     expression: t => memberIds.Contains(t.SuppervisorId.Value)
                         && (t.StartDate.Value.Month == month)
                         && t.Status != 0,
                     includes: includes
                 )).ToList();

            return new TotalTaskOfMonth
            {
                TaskCount = farmTasks.Count(),
                TotalTaskOfLivestock = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 1)),
                TotalTaskOfPlant = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 0)),
                TotalTaskOfOther = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Field == null && !f.FieldId.HasValue)),
                TotalTaskDoing = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Status == 3)),
                TotalTaskToDo = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Status == 1)),
                TotalTaskClose = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Status == 8)),
                TotalTaskPending = farmTasks.Count(f => f.StartDate.Value.Month == month && (f.Status == 5)),
            };
                
        }


        public List<object> GetStatusDescriptions()
        {
            var results = new List<object>();

            foreach (TaskStatusEnum status in Enum.GetValues(typeof(TaskStatusEnum)))
            {
                results.Add(new
                {
                    Status = (int)status,
                    Description = GetTaskStatusDescription(status)
                });
            }

            return results;
        }
        public static string GetTaskStatusDescription(TaskStatusEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public async Task<GetFarmTaskModel> Get(int id)
        {
            var farmTask = await _unitOfWork.RepositoryFarmTask.GetById(id);

            if (farmTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            var taskRepeate = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == farmTask.Id);
            var dateRepeate = taskRepeate.Select(t => t.StartDate.Value).ToList();

            var member = await _unitOfWork.RepositoryMember.GetById(farmTask.ManagerId);

            var plant = await _unitOfWork.RepositoryPlant.GetById(farmTask.PlantId);
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetById(farmTask.LiveStockId);

            var field = farmTask.FieldId != null ? await _unitOfWork.RepositoryField.GetById(farmTask.FieldId) : null;

            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(farmTask.TaskTypeId);
            var zone = field != null ? await _unitOfWork.RepositoryZone.GetSingleByCondition(h => h.Id == field.ZoneId) : null;
            var area = zone != null ? await _unitOfWork.RepositoryArea.GetById(zone.AreaId) : null;

            var status = (TaskStatusEnum)farmTask.Status;
            var statusString = GetTaskStatusDescription(status);

            var statusFieldString = field != null ? GetHabitantTypeDescription((HabitantTypeStatus)field.Status) : null;

            var priority = (PriorityEnum)farmTask.Priority;
            var priorityString = GetPriorityDescription(priority);

            var employeeNames = await ListTaskEmployee(farmTask.Id);
            var employeeNamesString = String.Join(", ", employeeNames);

            var employeeIds = await ListTaskEmployeeId(farmTask.Id);

            var materialNames = await ListMaterial(farmTask.Id);
            var materialNamesString = String.Join(", ", materialNames);

            var materialIds = await ListMaterialId(farmTask.Id);

            var employeeNameCodes = await ListTaskEmployeeNameCode(farmTask.Id);
            var employeeNameCodesString = string.Join(Environment.NewLine, employeeNameCodes);

            var receiver = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId);
            var supervisorName = receiver != null ? $"{receiver.Code} - {receiver.Name}" : null;
            var supervisorAvatar = receiver != null ? $"{receiver.Avatar} - {receiver.Avatar}" : null;

            var farmTaskModel = new GetFarmTaskModel
            {
                Id = farmTask.Id,
                CreateDate = farmTask.CreateDate,
                StartDate = farmTask.StartDate,
                EndDate = farmTask.EndDate,
                Description = farmTask.Description,
                Priority = priorityString,
                IsRepeat = farmTask.IsRepeat.Value,
                SupervisorName = supervisorName,
                SuppervisorId = receiver != null ? receiver.Id : (int?)null,
                ManagerName = member != null ? member.Name : null,
                PlantName = plant != null ? plant.Name : null,
                liveStockName = liveStock != null ? liveStock.Name : null,
                FieldName = field != null ? $"{field.Code} - {field.Name}" : null,
                TaskTypeName = taskType != null ? taskType.Name : null,
                TaskTypeId = taskType != null ? taskType.Id : 0,
                Remind = farmTask.Remind.HasValue ? farmTask.Remind.Value : 0,
                Name = farmTask.Name,
                Status = statusString,
                ZoneName = zone != null ? $"{zone.Code} - {zone.Name}" : null,
                AreaName = area != null ? $"{area.Code} - {area.Name}" : null,
                ExternalId = liveStock != null ? liveStock.ExternalId : (plant != null ? plant.ExternalId : null),
                EmployeeName = employeeNamesString,
                FieldId = field != null ? field.Id : 0,
                ZoneId = zone != null ? zone.Id : null,
                AreaId = area != null ? area.Id : null,
                FieldStatus = statusFieldString,
                EmployeeId = employeeIds,
                MaterialId = materialIds,
                MaterialName = materialNamesString,
                EmployeeNameCode = employeeNameCodesString,
                OriginalTaskId = farmTask.OriginalTaskId,
                DateRepeate = dateRepeate,
                Code = farmTask.Code,
                UpdateDate = farmTask.UpdateDate,
                OverallEfforMinutes = farmTask.OverallEfforMinutes.HasValue ? farmTask.OverallEfforMinutes.Value : 0,
                OverallEffortHour = farmTask.OverallEffortHour.HasValue ? farmTask.OverallEffortHour.Value : 0,
                AddressDetail = farmTask.AddressDetail,
                IsPlant = farmTask.IsPlant,
                IsSpecific = farmTask.IsSpecific,
                IsExpired = farmTask.IsExpired,
                AvatarSupervisor = supervisorAvatar,
                AvatarManager = member != null ? member.Avatar : null,
            };
            if (farmTask.OriginalTaskId == 0)
            {
                farmTaskModel.IsParent = true;
            }
            var evidence = await _unitOfWork.RepositoryTaskEvidence.GetData(f => f.TaskId == farmTask.Id);
            var totalEvidence = evidence.Count();
            if (totalEvidence > 0)
            {
                farmTaskModel.IsHaveEvidence = true;
            }
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == farmTask.Id && s.Status == true);
            if (subtask.Any())
            {
                farmTaskModel.IsHaveSubtask = true;
            }

            return farmTaskModel;
        }


        public static string GetHabitantTypeDescription(HabitantTypeStatus status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }
        public static string GetPriorityDescription(PriorityEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public async Task<IEnumerable<FarmTaskModel>> GetTaskByDay(DateTime date)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                 expression: task => task.StartDate.Value.Date == date.Date,
                 includes: includes
             );

            farmTasks = farmTasks.OrderByDescending(t => t.Status)
                     .ThenByDescending(t => t.Priority)
                     .ToList();

            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var map = await MapFarmTasks(farmTasks);

            return map.Values;

        }

        // get task theo manager
        public async Task<FarmTaskPageResult> GetTaskByStatusMemberDate(int id, int status, DateTime? date, int pageIndex, int pageSize, int? checkTaskParent, string? taskName)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType
            };
            int skipCount = (pageIndex - 1) * pageSize;

            var farmTasks = (await _unitOfWork.RepositoryFarmTask.GetData(
            expression: t => (t.ManagerId == id || t.ManagerId == null) &&
                             t.Status == status &&
                             (!date.HasValue ||
                              (!t.StartDate.HasValue ||
                               (date.Value.Date >= t.StartDate.Value.Date && date.Value.Date <= t.EndDate.Value.Date))),
            includes: includes)).ToList();

            if (checkTaskParent == 0)
            {
                farmTasks = farmTasks.Where(t => t.OriginalTaskId == 0 && t.IsRepeat == true).ToList();
            }

            if (!string.IsNullOrEmpty(taskName))
            {
                farmTasks = farmTasks.Where(t => t.Name.Unidecode().IndexOf(taskName.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            var totalTaskCount = farmTasks.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);

            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }


            if (status == 8 || status == 7)
            {
                farmTasks = farmTasks.OrderByDescending(t => t.StartDate).ToList()
                    .Skip(skipCount)
                    .Take(pageSize)
                    .ToList(); ;
            }
            else
            {
                farmTasks = farmTasks.OrderBy(t => t.StartDate).ThenBy(t => t.Priority).ThenByDescending(t => t.CreateDate)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            }

            var map = await MapFarmTasks(farmTasks);

            return new FarmTaskPageResult
            {
                FarmTasks = map.Values,
                TotalPages = totalPages,
            };
        }

        public async Task<FarmTaskPageResult> GetAllTaskByMemberDate(int id, DateTime? date, int pageIndex, int pageSize, int? checkTaskParent, string? taskName)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType
            };
            int skipCount = (pageIndex - 1) * pageSize;

            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                 expression: t => (t.ManagerId == id || t.ManagerId == null) &&
                                  (!date.HasValue ||
                                    (!t.StartDate.HasValue ||
                                    (date.Value.Date >= t.StartDate.Value.Date && date.Value.Date <= t.EndDate.Value.Date))),
                 includes: includes);
            if (checkTaskParent == 0)
            {
                farmTasks = farmTasks.Where(t => t.OriginalTaskId == 0 && t.IsRepeat == true);
            }


            if (!string.IsNullOrEmpty(taskName))
            {
                farmTasks = farmTasks.Where(t => t.Name.Unidecode().IndexOf(taskName.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var totalTaskCount = farmTasks.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);


            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            farmTasks = farmTasks.OrderBy(t => t.StartDate).ThenBy(t => t.Priority).ThenByDescending(t => t.CreateDate)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();


            var map = await MapFarmTasks(farmTasks);

            return new FarmTaskPageResult
            {
                FarmTasks = map.Values,
                TotalPages = totalPages,
            };
        }

        public async Task<FarmTaskPageResult> GetTaskByStatusSupervisorDate(int id, int status, DateTime? date, int pageIndex, int pageSize, string? taskName)
        {
            try
            {
                var includes = new Expression<Func<FarmTask, object>>[]
                {
            t => t.Manager,
            t => t.Plant,
            t => t.LiveStrock,
            t => t.Field.Zone,
            t => t.Field.Zone.Area,
            t => t.Field,
            t => t.TaskType
                };

                var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                    expression: t => t.SuppervisorId == id &&
                                    t.Status == status &&
                                    (!date.HasValue ||
                                (!t.StartDate.HasValue ||
                                    (date.Value.Date >= t.StartDate.Value.Date && date.Value.Date <= t.EndDate.Value.Date)))
,
                    includes: includes);

                if (!string.IsNullOrEmpty(taskName))
                {
                    farmTasks = farmTasks.Where(t => t.Name.Unidecode().IndexOf(taskName.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0);
                }


                var totalTaskCount = farmTasks.Count();
                var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);

                if (status == 8 || status == 7)
                {
                    farmTasks = farmTasks.OrderByDescending(t => t.StartDate).ToList()
                        .Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                }
                else
                {
                    farmTasks = farmTasks.OrderBy(t => t.StartDate).ThenBy(t => t.Priority).ThenByDescending(t => t.CreateDate)
                                    .Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                }

                var map = await MapFarmTasks(farmTasks);

                return new FarmTaskPageResult
                {
                    FarmTasks = map.Values,
                    TotalPages = totalPages,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IEnumerable<FarmTaskModel>> GetTaskByMemberId(int id)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
           {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
           };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                    .GetData(expression: t => t.ManagerId == id, includes: includes);
            farmTasks = farmTasks.OrderByDescending(t => t.Status)
                     .ThenByDescending(t => t.Priority)
                     .ToList();
            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var map = await MapFarmTasks(farmTasks);

            return map.Values;

        }
        public async Task<List<int>> GetManagerId()
        {
            var manager = await _unitOfWork.RepositoryMember.GetData(m => m.RoleId == 1);
            var managerId = manager.Select(m => m.Id).ToList();
            return managerId;
        }
        public async Task<List<string>> GetTokenByMemberId(int id)
        {
            var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => h.MemberId == id);

            if (hubConnections == null)
            {
                throw new Exception("Không tìm thấy kết nối");
            }

            var connectionIds = hubConnections.Select(h => h.ConnectionId).ToList();
            return connectionIds;
        }

        private static int taskCounter = 1;
        private string GenerateTaskCode()
        {
            Guid uniqueId = Guid.NewGuid();

            string uniquePart = uniqueId.ToString("N").Substring(0, 8);

            return "CV" + uniquePart;
        }

        public async Task CreateTaskDraft(TaskDraftModel taskDraftModel, List<DateTime>? Dates, List<int>? materialIds)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            string taskCode = GenerateTaskCode();

            var startDate = taskDraftModel.StartDate;
            var endDate = taskDraftModel.EndDate;
            int taskId = 0;
            int originalTaskId = 0;
            var dateList = new List<DateTime?> { startDate };
            dateList.AddRange(Dates.Select(date => (DateTime?)date));

            foreach (var date in dateList)
            {
                if (date < startDate)
                {
                    throw new Exception("Ngày truyền vào không được nhỏ hơn ngày bắt đầu (startDate).");
                }
                if (startDate > endDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                if (endDate < currentTime)
                {
                    throw new Exception("Ngày kết thúc không được bé hơn ngày hiện tại");
                }
                var plant = await _unitOfWork.RepositoryPlant.GetById(taskDraftModel.PlantId);
                if (plant?.Status == 0)
                {
                    throw new Exception($"{plant.Name} đã ở trạng thái inactive");
                }
                if (taskDraftModel.StartDate > taskDraftModel.EndDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                var livestock = await _unitOfWork.RepositoryLiveStock.GetById(taskDraftModel.LiveStockId);
                if (livestock?.Status == 0)
                {
                    throw new Exception($"{livestock.Name} đã ở trạng thái inactive");
                }

                var field = await _unitOfWork.RepositoryField.GetById(taskDraftModel.FieldId);
                if (field?.IsDelete == true)
                {
                    throw new Exception($"{field.Name} đã ở trạng thái inactive");
                }

                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskDraftModel.TaskTypeId);
                if (taskType?.IsDelete == true)
                {
                    throw new Exception($"{taskType.Name} đã ở trạng thái inactive");
                }
                if (taskDraftModel.IsPlant == true && taskType.Status != 0)
                {
                    throw new Exception($"Sai loại công việc");
                }

                var farmTaskNew = new FarmTask
                {
                    CreateDate = currentTime,
                    Description = taskDraftModel.Description?.Trim(),
                    Priority = string.IsNullOrEmpty(taskDraftModel.Priority) ? (int?)null : (int?)ParsePriorityFromString(taskDraftModel.Priority),
                    SuppervisorId = taskDraftModel.SupervisorId == 0 ? (int?)null : taskDraftModel.SupervisorId,
                    FieldId = taskDraftModel.FieldId == 0 ? (int?)null : taskDraftModel.FieldId,
                    TaskTypeId = taskDraftModel.TaskTypeId == 0 ? (int?)null : taskDraftModel.TaskTypeId,
                    IsRepeat = (originalTaskId == 0) ? taskDraftModel.IsRepeat : false,
                    ManagerId = taskDraftModel.ManagerId == 0 ? (int?)null : taskDraftModel.ManagerId,
                    PlantId = taskDraftModel.PlantId == 0 ? (int?)null : taskDraftModel.PlantId,
                    LiveStockId = taskDraftModel.LiveStockId == 0 ? (int?)null : taskDraftModel.LiveStockId,
                    Name = taskDraftModel.Name,
                    Status = 0,
                    Remind = taskDraftModel.Remind,
                    OriginalTaskId = originalTaskId,
                    AddressDetail = taskDraftModel.AddressDetail?.Trim(),
                    OverallEfforMinutes = 0,
                    OverallEffortHour = 0,
                    UpdateDate = null,
                    Code = taskCode,
                    IsPlant = taskDraftModel.IsPlant,
                    IsSpecific = taskDraftModel.IsSpecific,
                    IsExpired = false,
                };
                if (startDate.HasValue && date.HasValue)
                {
                    farmTaskNew.StartDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, startDate.Value.Hour, startDate.Value.Minute, startDate.Value.Second);
                    if (endDate.HasValue)
                    {
                        farmTaskNew.EndDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, endDate.Value.Hour, endDate.Value.Minute, endDate.Value.Second).AddDays((endDate - startDate).Value.Days);
                    }
                }
                else
                {
                    farmTaskNew.StartDate = null;
                    farmTaskNew.EndDate = null;
                }

                await CreateTaskForDate(farmTaskNew, materialIds);
                if (farmTaskNew.OriginalTaskId == 0)
                {
                    taskId = farmTaskNew.Id;

                }
                if (farmTaskNew.IsRepeat.Value)
                {
                    if (originalTaskId == 0)
                    {
                        originalTaskId = farmTaskNew.Id;
                    }
                }
            }
        }

        public async Task AddEmployeeToTaskAsign(int taskId, List<int>? employeeIds, int? overallEfforMinutes, int? overallEffortHour)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("Không tìm thấy nhiệm vụ");
            if (task.Status == 1 || task.Status == 2 || task.Status == 3)
            {
                if (task.StartDate.HasValue && task.EndDate.HasValue)
                {
                    TimeSpan totalDuration = task.EndDate.Value - task.StartDate.Value;
                    double totalHours = totalDuration.TotalHours;

                    var totalEffortHour = overallEffortHour + overallEfforMinutes / 60;

                    if (totalEffortHour > totalHours)
                    {
                        throw new Exception("Giờ dự kiến không được lớn hơn tổng thời gian giữa ngày bắt đầu và ngày kết thúc");
                    }
                }
                else
                {
                    throw new Exception("Ngày bắt đầu hoặc ngày kết thúc không có giá trị");
                }

                if (task.Status == 1 || task.Status == 2)
                {
                    task.Status = 2;
                    task.OverallEfforMinutes = overallEfforMinutes;
                    task.OverallEffortHour = overallEffortHour;
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }

                if (!employeeIds.Any())
                {
                    throw new Exception("Nhân viên không được rỗng");
                }

                foreach (var employeeId in employeeIds)
                {
                    var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                    if (employee == null)
                    {
                        throw new Exception($"Không tìm thấy nhân viên");
                    }

                    var existingEmployeeTask = await _unitOfWork.RepositoryEmployee_Task.GetData(et => et.EmployeeId == employee.Id && et.TaskId == taskId);
                    string taskCode = GenerateSubTaskCode();
                    if (!existingEmployeeTask.Any())
                    {
                        var employeeTask = new Employee_Task
                        {
                            EmployeeId = employee.Id,
                            TaskId = taskId,
                            ActualEfforMinutes = 0,
                            ActualEffortHour = 0,
                            Status = false,
                            Code = taskCode
                        };

                        await _unitOfWork.RepositoryEmployee_Task.Add(employeeTask);
                        await _unitOfWork.RepositoryEmployee_Task.Commit();
                    }
                }

                _unitOfWork.RepositoryEmployee_Task.Delete(et => !employeeIds.Contains(et.EmployeeId) && et.TaskId == taskId);
                await _unitOfWork.RepositoryMaterial_Task.Commit();



            }
            else
            {
                throw new Exception("Chỉ thêm được nhân viên trong trạng thái chuẩn bị và đang thực hiện");
            }
        }

        public async Task UpdateTaskDisagreeAndChangeToToDo(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int> materialIds)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
                if (task != null)
                {
                    if (task.Status == 6)
                    {
                        await UpdateTask(taskId, taskModel, dates, materialIds);
                        task.Status = 1;
                        _unitOfWork.RepositoryFarmTask.Update(task);
                        await _unitOfWork.RepositoryFarmTask.Commit();

                        string message = $"Công việc '{task.Name}' đã được giao lại";
                        List<int> memberIds = new List<int>();
                        if (task.SuppervisorId.HasValue)
                        {
                            memberIds.Add(task.SuppervisorId.Value);
                            var managerTokens = await GetTokenByMemberId(task.SuppervisorId.Value);
                            await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                        }

                    }
                    else
                    {
                        throw new Exception("Không thể từ chối");
                    }
                }
                else
                {
                    throw new Exception("Không tìm thấy nhiệm vụ ");
                }
                var evidences = await _unitOfWork.RepositoryTaskEvidence.GetData(e => e.TaskId == taskId);
                foreach (var evidence in evidences)
                {
                    _unitOfWork.RepositoryTaskEvidence.Delete(evidence);
                    await _unitOfWork.RepositoryTaskEvidence.Commit();
                }
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }

        public async Task UpdateTask(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int> materialIds)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var farmTask = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.Id == taskId);
                if (farmTask == null)
                {
                    throw new Exception("Không tìm thấy nhiệm vụ");
                }

                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                var taskRepeats = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == farmTask.Id);
                var repeatDates = taskRepeats.Select(t => t.StartDate).ToList();

                if (taskModel.EndDate < currentDateTime || taskModel.EndDate < taskModel.StartDate)
                {
                    throw new Exception("Ngày kết thúc không được bé hơn ngày hiện tại và ngày bắt đầu");
                }
                await UpdateFarmTask(farmTask, taskModel.StartDate?.Date, taskModel, materialIds);

                if (farmTask.OriginalTaskId == 0)
                {
                    if (dates != null && dates.Any())
                    {
                        foreach (var date in dates)
                        {
                            if (farmTask.StartDate == null || date == farmTask.StartDate.Value.Date)
                            {
                                continue;
                            }
                            else if (date < currentDateTime)
                            {
                                throw new Exception("Không thể cập nhật nhiệm vụ với ngày bắt đầu trong quá khứ.");
                            }

                            var existingTask = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.OriginalTaskId == farmTask.Id && t.StartDate.HasValue && t.StartDate.Value.Date == date.Date);

                            if (existingTask == null)
                            {
                                var newTask = CreateNewFarmTask(date, farmTask, taskModel);
                                await _unitOfWork.RepositoryFarmTask.Add(newTask);
                                await CreateTaskForDate(newTask, materialIds);
                            }
                            else
                            {
                                await UpdateFarmTask(existingTask, date, taskModel, materialIds);
                                existingTask.IsRepeat = false;
                                await _unitOfWork.RepositoryFarmTask.Commit();
                            }
                        }
                    }

                    _unitOfWork.RepositoryFarmTask.Delete(t => dates == null || !dates.Contains(t.StartDate.Value.Date) && t.OriginalTaskId == farmTask.Id);
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }
                else
                {
                    await UpdateFarmTask(farmTask, farmTask.StartDate?.Date, taskModel, materialIds);
                    farmTask.OriginalTaskId = 0;
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }



        public async Task DeleteTaskTodoDraftAssign(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("Không tìm thấy nhiệm vụ");
            if (task.Status == 1 || task.Status == 0 || task.Status == 2)
            {
                _unitOfWork.RepositoryFarmTask.Delete(t => t.Id == taskId);
                if (task.OriginalTaskId == 0)
                {
                    var subtasks = await _unitOfWork.RepositoryFarmTask.GetData(s => s.OriginalTaskId == taskId);
                    foreach (var subtask in subtasks)
                    {
                        _unitOfWork.RepositoryFarmTask.Delete(subtask);
                    }
                }
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else
            {
                throw new Exception("Chỉ được xóa những nhiệm vụ nháp, chuẩn bị và đã giao");
            }
        }

        public async Task DeleteTaskAssign(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("Không tìm thấy nhiệm vụ");
            if (task.Status == 2)
            {
                _unitOfWork.RepositoryFarmTask.Delete(t => t.Id == taskId && t.ManagerId == null);
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else
            {
                throw new Exception("Chỉ được xóa những nhiệm vụ đã giao");
            }
        }

        private FarmTask CreateNewFarmTask(DateTime date, FarmTask parentTask, TaskDraftModelUpdate taskModel)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var startDate = taskModel.StartDate;
            var endDate = taskModel.EndDate;

            // Check if startDate and endDate are not null
            if (startDate.HasValue && endDate.HasValue)
            {
                var newTask = new FarmTask
                {
                    CreateDate = currentTime,
                    StartDate = new DateTime(date.Year, date.Month, date.Day, startDate.Value.Hour, startDate.Value.Minute, startDate.Value.Second),
                    EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Value.Hour, endDate.Value.Minute, endDate.Value.Second).AddDays((endDate.Value - startDate.Value).Days),
                    Description = taskModel.Description,
                    Priority = string.IsNullOrEmpty(taskModel.Priority) ? (int?)null : (int?)ParsePriorityFromString(taskModel.Priority),
                    SuppervisorId = taskModel.SupervisorId ?? 0,
                    FieldId = taskModel.FieldId == 0 ? (int?)null : taskModel.FieldId,
                    TaskTypeId = taskModel.TaskTypeId == 0 ? (int?)null : taskModel.TaskTypeId,
                    IsRepeat = false,
                    ManagerId = taskModel.ManagerId == 0 ? (int?)null : taskModel.ManagerId,
                    PlantId = taskModel.PlantId == 0 ? (int?)null : taskModel.PlantId,
                    LiveStockId = taskModel.LiveStockId == 0 ? (int?)null : taskModel.LiveStockId,
                    Name = taskModel.Name,
                    Status = parentTask.Status,
                    Remind = taskModel.Remind,
                    OriginalTaskId = parentTask.Id,
                    AddressDetail = taskModel.AddressDetail,
                    OverallEfforMinutes = 0,
                    OverallEffortHour = 0,
                    UpdateDate = currentTime,
                    Code = GenerateTaskCode(),
                    IsPlant = parentTask.IsPlant,
                    IsSpecific = parentTask.IsSpecific,
                    IsExpired = false,
                };

                return newTask;
            }
            else
            {
                // Handle the case when startDate or endDate is null
                // You might throw an exception, log an error, or handle it according to your business logic
                throw new InvalidOperationException("StartDate and EndDate must have values.");
            }
        }

        public async Task UpdateTaskDraftAndToPrePare(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int>? materialIds)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("KhÔng tìm thấy nhiệm vụ)");
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            await UpdateTask(taskId, taskModel, dates, materialIds);
            task.Status = 1;
            task.IsRepeat = true;
            var existingTasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == taskId);

            if (existingTasks.Any())
            {
                foreach (var existingTask in existingTasks)
                {
                    if (existingTask.StartDate <= currentDateTime)
                    {
                        throw new Exception("Cập nhật lại ngày bắt đầu nó không được bé hơn ngày hiện tại");
                    }
                    existingTask.Status = 1;
                    existingTask.IsRepeat = false;
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }

            }

            string message = $"Công việc '{task.Name}' đã được giao";
            List<int> memberIds = new List<int>();
            if (task.SuppervisorId.HasValue)
            {
                memberIds.Add(task.SuppervisorId.Value);
                var managerTokens = await GetTokenByMemberId(task.SuppervisorId.Value);
                await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);

            }
            await _unitOfWork.RepositoryFarmTask.Commit();

        }


        private async Task UpdateFarmTask(FarmTask farmTask, DateTime? date, TaskDraftModelUpdate taskModel, List<int> materialIds)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var startDate = taskModel.StartDate;
            var endDate = taskModel.EndDate;

            farmTask.Name = taskModel.Name;
            farmTask.Description = taskModel.Description?.Trim();
            farmTask.Priority = (int)ParsePriorityFromString(taskModel.Priority);
            farmTask.SuppervisorId = taskModel.SupervisorId == 0 ? (int?)null : taskModel.SupervisorId;
            farmTask.FieldId = taskModel.FieldId == 0 ? (int?)null : taskModel.FieldId;
            farmTask.TaskTypeId = taskModel.TaskTypeId == 0 ? (int?)null : taskModel.TaskTypeId;
            farmTask.PlantId = taskModel.PlantId == 0 ? (int?)null : taskModel.PlantId;
            farmTask.LiveStockId = taskModel.LiveStockId == 0 ? (int?)null : taskModel.LiveStockId;
            //farmTask.StartDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, startDate.Value.Hour, startDate.Value.Minute, startDate.Value.Second);
            //farmTask.EndDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, endDate.Value.Hour, endDate.Value.Minute, endDate.Value.Second).AddDays((endDate - startDate).Value.Days);
            farmTask.Remind = taskModel.Remind;
            farmTask.IsRepeat = taskModel.IsRepeat;
            farmTask.ManagerId = taskModel.ManagerId == 0 ? (int?)null : taskModel.ManagerId;
            farmTask.UpdateDate = currentDateTime;
            farmTask.AddressDetail = taskModel.AddressDetail?.Trim();

            if (startDate.HasValue)
            {
                farmTask.StartDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, startDate.Value.Hour, startDate.Value.Minute, startDate.Value.Second);
                if (endDate.HasValue)
                {
                    DateTime newEndDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, endDate.Value.Hour, endDate.Value.Minute, endDate.Value.Second).AddDays((endDate - startDate).Value.Days);

                    if (farmTask.EndDate != newEndDate)
                    {
                        farmTask.EndDate = newEndDate;
                        farmTask.IsExpired = false;
                    }
                }
            }
            else
            {
                farmTask.StartDate = null;
                farmTask.EndDate = null;
                farmTask.IsExpired = false;
            }


            if (farmTask.Status == 1)
            {
                if (farmTask.Name == null || farmTask.Description == null || farmTask.TaskTypeId == null || farmTask.ManagerId == null || farmTask.SuppervisorId == null)
                {
                    throw new Exception("Các thuộc tính không được null khi trạng thái là 1");
                }
            }
            foreach (var materialId in materialIds)
            {
                var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                if (material == null)
                {
                    throw new Exception($"Không tìm thấy dụng cụ");
                }

                var existingmaterialTask = await _unitOfWork.RepositoryMaterial_Task.GetSingleByCondition(et =>
                    et.MaterialId == material.Id && et.TaskId == farmTask.Id);

                if (existingmaterialTask == null)
                {
                    var materialTask = new Material_Task
                    {
                        MaterialId = material.Id,
                        TaskId = farmTask.Id,
                    };

                    await _unitOfWork.RepositoryMaterial_Task.Add(materialTask);
                    await _unitOfWork.RepositoryMaterial_Task.Commit();
                }
            }

            _unitOfWork.RepositoryMaterial_Task.Delete(et => !materialIds.Contains(et.MaterialId) && et.TaskId == farmTask.Id);
            await _unitOfWork.RepositoryMaterial_Task.Commit();

            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public async Task CreateTaskToDo(TaskToDoModel taskToDoModel, List<DateTime>? Dates, List<int>? materialIds)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var startDate = taskToDoModel.StartDate;
            var endDate = taskToDoModel.EndDate;
            int taskId = 0;
            int originalTaskId = 0;
            var supervisorTokens = await GetTokenByMemberId(taskToDoModel.SupervisorId.Value);
            List<string> deviceTokens = new List<string>();
            deviceTokens.AddRange(supervisorTokens);


            foreach (var date in new List<DateTime> { startDate.Value }.Concat(Dates))
            {
                if (date < startDate)
                {
                    throw new Exception("Ngày truyền vào không được nhỏ hơn ngày bắt đầu (startDate).");
                }
                if (startDate.Value > endDate.Value)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }

                if (endDate < vietnamTime)
                {
                    throw new Exception("Ngày kết thúc không được bé hơn ngày hiện tại");
                }

                var plant = await _unitOfWork.RepositoryPlant.GetById(taskToDoModel.PlantId);
                if (plant?.Status == 0)
                {
                    throw new Exception($"{plant.Name} đã ở trạng thái inactive");
                }
                if (taskToDoModel.StartDate > taskToDoModel.EndDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                var livestock = await _unitOfWork.RepositoryLiveStock.GetById(taskToDoModel.LiveStockId);
                if (livestock?.Status == 0)
                {
                    throw new Exception($"{livestock.Name} đã ở trạng thái inactive");
                }

                var field = await _unitOfWork.RepositoryField.GetById(taskToDoModel.FieldId);
                if (field?.IsDelete == true)
                {
                    throw new Exception($"{field.Name} đã ở trạng thái inactive");
                }

                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskToDoModel.TaskTypeId);
                if (taskType?.IsDelete == true)
                {
                    throw new Exception($"{taskType.Name} đã ở trạng thái inactive");
                }
                if (taskToDoModel.IsPlant == true && taskType.Status != 0)
                {
                    throw new Exception($"Sai loại công việc");
                }

                var supervisor = await _unitOfWork.RepositoryMember.GetById(taskToDoModel.SupervisorId);
                if (supervisor?.Status == 0)
                {
                    throw new Exception($"{supervisor.Name} đã ở trạng thái inactive");
                }

                string taskCode = GenerateTaskCode();
                var farmTaskNew = new FarmTask
                {
                    CreateDate = vietnamTime,
                    StartDate = new DateTime(date.Year, date.Month, date.Day, startDate.Value.Hour, startDate.Value.Minute, startDate.Value.Second),
                    EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Value.Hour, endDate.Value.Minute, endDate.Value.Second).AddDays((endDate - startDate).Value.Days),
                    Description = taskToDoModel.Description?.Trim(),
                    Priority = (int)ParsePriorityFromString(taskToDoModel.Priority),
                    SuppervisorId = taskToDoModel.SupervisorId == 0 ? (int?)null : taskToDoModel.SupervisorId,
                    FieldId = taskToDoModel.FieldId == 0 ? (int?)null : taskToDoModel.FieldId,
                    TaskTypeId = taskToDoModel.TaskTypeId,
                    IsRepeat = (originalTaskId == 0) ? taskToDoModel.IsRepeat : false,
                    ManagerId = taskToDoModel.ManagerId == 0 ? (int?)null : taskToDoModel.ManagerId,
                    PlantId = taskToDoModel.PlantId == 0 ? (int?)null : taskToDoModel.PlantId,
                    LiveStockId = taskToDoModel.LiveStockId == 0 ? (int?)null : taskToDoModel.LiveStockId,
                    Name = taskToDoModel.Name,
                    Status = 1,
                    Remind = taskToDoModel.Remind,
                    OriginalTaskId = originalTaskId,
                    AddressDetail = taskToDoModel.AddressDetail?.Trim(),
                    OverallEfforMinutes = 0,
                    OverallEffortHour = 0,
                    UpdateDate = null,
                    Code = taskCode,
                    IsPlant = taskToDoModel.IsPlant,
                    IsSpecific = taskToDoModel.IsSpecific,
                    IsExpired = false,
                };

                var suppervisor = await _unitOfWork.RepositoryMember.GetById(taskToDoModel.SupervisorId);

                if (suppervisor == null)
                {
                    throw new Exception("Không tìm thấy người dám sát");
                }

                if (supervisorTokens.Count > 0)
                {
                    var remindMessage = new Message
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = "Nhắc nhở",
                            Body = $"Còn {farmTaskNew.Remind} phút tới công việc của bạn"
                        },
                        Data = new Dictionary<string, string>
                        {
                            { "taskId", taskId.ToString() }
                        }
                    };

                    DateTime? remindTime = farmTaskNew.StartDate?.AddMinutes(-farmTaskNew.Remind.Value) ?? DateTime.MinValue;

                    var currentTime = vietnamTime;

                    if (remindTime.HasValue && remindTime.Value > currentTime)
                    {
                        var delayMilliseconds = (int)(remindTime.Value - currentTime).TotalMilliseconds;
                        var timer = new System.Timers.Timer(delayMilliseconds);
                        timer.Elapsed += async (sender, e) =>
                        {
                            await SendNotificationToDevices(supervisorTokens, remindMessage);
                            timer.Dispose();
                        };
                        timer.AutoReset = false;
                        timer.Start();
                    }
                }
                await CreateTaskForDate(farmTaskNew, materialIds);
                if (farmTaskNew.OriginalTaskId == 0)
                {
                    taskId = farmTaskNew.Id;

                }
                if (farmTaskNew.IsRepeat.Value)
                {
                    if (originalTaskId == 0)
                    {
                        originalTaskId = farmTaskNew.Id;
                    }
                }

                if (farmTaskNew.EndDate.HasValue && farmTaskNew.EndDate <= vietnamTime)
                {
                    farmTaskNew.IsExpired = true;
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }
                else
                {
                    if (farmTaskNew.EndDate.HasValue && farmTaskNew.EndDate > vietnamTime)
                    {
                        var remainingMilliseconds = (int)(farmTaskNew.EndDate.Value - vietnamTime).TotalMilliseconds;

                        if (remainingMilliseconds == 0)
                        {
                            farmTaskNew.IsExpired = true;
                            await _unitOfWork.RepositoryFarmTask.Commit();
                        }
                        else if (remainingMilliseconds > 0)
                        {
                            var timer = new System.Timers.Timer(remainingMilliseconds);
                            timer.Elapsed += async (sender, e) =>
                            {
                                farmTaskNew.IsExpired = true;
                                await _unitOfWork.RepositoryFarmTask.Commit();
                                timer.Dispose();
                            };
                            timer.AutoReset = false;
                            timer.Start();
                        }
                    }
                }

            }

            var deviceMessages = deviceTokens.Select(token => new Message
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = $"{taskToDoModel.Name}",
                    Body = $"Bạn đã nhận một công việc '{taskToDoModel.Name}'"
                },
                Data = new Dictionary<string, string>
                {
                    { "taskId", taskId.ToString() }
                }
            }).ToList();


            foreach (var deviceMessage in deviceMessages)
            {
                await SendNotificationToDevices(new List<string> { deviceMessage.Token }, deviceMessage);
            }

            var individualNotification = new Notification
            {
                Message = $"Bạn đã nhận một công việc '{taskToDoModel.Name}'",
                MessageType = "Individual",
                NotificationDateTime = vietnamTime,
                IsRead = false,
                IsNew = true,
                TaskId = taskId
            };
            await _unitOfWork.RepositoryNotifycation.Add(individualNotification);
            await _unitOfWork.RepositoryNotifycation.Commit();

            var member_notify = new Notification_Member
            {
                NotificationId = individualNotification.Id,
                MemberId = taskToDoModel.SupervisorId.Value,
            };
            await _unitOfWork.RepositoryNotifycation_Member.Add(member_notify);
            await _unitOfWork.RepositoryNotifycation_Member.Commit();
        }


        public async Task CreateAsignTask(TaskCreateAsignModel taskModel, List<int>? materialIds, List<int>? employeeIds)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                var plant = await _unitOfWork.RepositoryPlant.GetById(taskModel.PlantId);
                if (plant?.Status == 0)
                {
                    throw new Exception($"{plant.Name} đã ở trạng thái inactive");
                }
                if (taskModel.EndDate > vietnamTime)
                {
                    throw new Exception("Ngày kết thúc không được bé hơn ngày hiện tại");
                }
                if (taskModel.StartDate > taskModel.EndDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                var livestock = await _unitOfWork.RepositoryLiveStock.GetById(taskModel.LiveStockId);
                if (livestock?.Status == 0)
                {
                    throw new Exception($"{livestock.Name} đã ở trạng thái inactive");
                }

                var field = await _unitOfWork.RepositoryField.GetById(taskModel.FieldId);
                if (field?.IsDelete == true)
                {
                    throw new Exception($"{field.Name} đã ở trạng thái inactive");
                }

                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskModel.TaskTypeId);
                if (taskType?.IsDelete == true)
                {
                    throw new Exception($"{taskType.Name} đã ở trạng thái inactive");
                }
                if (taskModel.IsPlant == true && taskType.Status != 0)
                {
                    throw new Exception($"Sai loại công việc");
                }

                var supervisor = await _unitOfWork.RepositoryMember.GetById(taskModel.SuppervisorId);
                if (supervisor?.Status == 0)
                {
                    throw new Exception($"{supervisor.Name} đã ở trạng thái inactive");
                }
                if (taskModel.StartDate > taskModel.EndDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }

                TimeSpan totalDuration = taskModel.EndDate - taskModel.StartDate;
                double totalHours = totalDuration.TotalHours;

                var totalEffortHour = taskModel.OverallEffortHour + taskModel.OverallEfforMinutes / 60;

                if (totalEffortHour > totalHours)
                {
                    throw new Exception("Giờ dự kiến không được lớn hơn tổng thời gian giữa ngày bắt đầu và ngày kết thúc");
                }

                string taskCode = GenerateTaskCode();
                var farmTaskNew = new FarmTask
                {
                    CreateDate = vietnamTime,
                    StartDate = taskModel.StartDate,
                    EndDate = taskModel.EndDate,
                    Description = taskModel.Description,
                    Priority = (int)ParsePriorityFromString(taskModel.Priority),
                    SuppervisorId = taskModel.SuppervisorId,
                    FieldId = taskModel.FieldId == 0 ? (int?)null : taskModel.FieldId,
                    TaskTypeId = taskModel.TaskTypeId,
                    IsRepeat = false,
                    ManagerId = null,
                    PlantId = taskModel.PlantId == 0 ? (int?)null : taskModel.PlantId,
                    LiveStockId = taskModel.LiveStockId == 0 ? (int?)null : taskModel.LiveStockId,
                    Name = taskModel.Name,
                    Status = 2,
                    Remind = 0,
                    OriginalTaskId = 0,
                    AddressDetail = taskModel.AddressDetail,
                    OverallEfforMinutes = taskModel.OverallEfforMinutes,
                    OverallEffortHour = taskModel.OverallEffortHour,
                    UpdateDate = null,
                    Code = taskCode,
                    IsExpired = false,
                    IsPlant = taskModel.IsPlant,
                    IsSpecific = taskModel.IsSpecific,
                };

                await _unitOfWork.RepositoryFarmTask.Add(farmTaskNew);

                var suppervisor = await _unitOfWork.RepositoryMember.GetById(taskModel.SuppervisorId);

                if (suppervisor == null)
                {
                    throw new Exception("Không tìm thấy người dám sát");
                }

                var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => h.MemberId == taskModel.SuppervisorId);

                var supervisorTokens = await GetTokenByMemberId(taskModel.SuppervisorId);

                if (supervisorTokens.Count > 0)
                {
                    var remindMessage = new Message
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = "Nhắc nhở",
                            Body = $"Còn {farmTaskNew.Remind} phút tới nhiệm vụ của bạn"
                        },
                        Data = new Dictionary<string, string>
                        {
                            { "taskId", farmTaskNew.Id.ToString() }
                        }
                    };

                    DateTime? remindTime = farmTaskNew.StartDate?.AddMinutes(-farmTaskNew.Remind.Value) ?? DateTime.MinValue;

                    var currentTime = vietnamTime;

                    if (remindTime.HasValue && remindTime.Value > currentTime)
                    {
                        var delayMilliseconds = (int)(remindTime.Value - currentTime).TotalMilliseconds;
                        var timer = new System.Timers.Timer(delayMilliseconds);
                        timer.Elapsed += async (sender, e) =>
                        {
                            await SendNotificationToDevices(supervisorTokens, remindMessage);
                            timer.Dispose();
                        };
                        timer.AutoReset = false;
                        timer.Start();
                    }

                    string message = $"Công việc '{farmTaskNew.Name}' đã được tạo";
                    List<int> memberIds = new List<int>();
                    if (farmTaskNew.ManagerId.HasValue)
                    {
                        memberIds.Add(farmTaskNew.SuppervisorId.Value);
                        var managerTokens = await GetTokenByMemberId(farmTaskNew.SuppervisorId.Value);
                        await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, farmTaskNew.Id);
                    }
                }
                await CreateEmployeeMaterialForTask(farmTaskNew, materialIds, employeeIds);
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateTaskAsign(int taskId, TaskUpdateAsignModel taskModel, List<int>? materialIds, List<int>? employeeIds)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("Không tìm thấy công việc)");

                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                if (task.Status != 2) throw new Exception("Chỉ được cập nhật công việc đã giao");
                if (taskModel.StartDate > taskModel.EndDate)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }

                if (task.StartDate.HasValue && task.EndDate.HasValue)
                {
                    TimeSpan totalDuration = taskModel.EndDate - taskModel.StartDate;
                    double totalHours = totalDuration.TotalHours;

                    var totalEffortHour = taskModel.OverallEffortHour + taskModel.OverallEfforMinutes / 60;

                    if (totalEffortHour > totalHours)
                    {
                        throw new Exception("Giờ dự kiến không được lớn hơn tổng thời gian giữa ngày bắt đầu và ngày kết thúc");
                    }


                }
                else
                {
                    throw new Exception("Ngày bắt đầu hoặc ngày kết thúc không có giá trị");
                }


                task.Name = taskModel.Name;
                task.Description = taskModel.Description?.Trim();
                task.Priority = (int)ParsePriorityFromString(taskModel.Priority);
                task.FieldId = taskModel.FieldId == 0 ? (int?)null : taskModel.FieldId;
                task.TaskTypeId = taskModel.TaskTypeId == 0 ? (int?)null : taskModel.TaskTypeId;
                task.PlantId = taskModel.PlantId == 0 ? (int?)null : taskModel.PlantId;
                task.LiveStockId = taskModel.LiveStockId == 0 ? (int?)null : taskModel.LiveStockId;
                task.StartDate = taskModel.StartDate;
                task.EndDate = taskModel.EndDate;
                task.AddressDetail = taskModel.AddressDetail?.Trim();
                task.OverallEfforMinutes = taskModel.OverallEfforMinutes;
                task.OverallEffortHour = taskModel.OverallEffortHour;

                if (task.EndDate != taskModel.EndDate)
                {
                    task.IsExpired = false;
                }

                if (!employeeIds.Any())
                {
                    throw new Exception("Nhân viên không được để trống");
                }

                foreach (var employeeId in employeeIds)
                {
                    var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                    if (employee == null)
                    {
                        throw new Exception($"Không tìm thấy nhân viên");
                    }

                    var existingEmployeeTask = await _unitOfWork.RepositoryEmployee_Task.GetData(et => et.EmployeeId == employee.Id && et.TaskId == taskId);

                    if (!existingEmployeeTask.Any())
                    {
                        var employeeTask = new Employee_Task
                        {
                            EmployeeId = employee.Id,
                            TaskId = taskId,
                            ActualEfforMinutes = 0,
                            ActualEffortHour = 0,
                            Status = false,
                        };

                        await _unitOfWork.RepositoryEmployee_Task.Add(employeeTask);
                        await _unitOfWork.RepositoryEmployee_Task.Commit();
                    }
                }

                _unitOfWork.RepositoryEmployee_Task.Delete(et => !employeeIds.Contains(et.EmployeeId) && et.TaskId == taskId);
                await _unitOfWork.RepositoryMaterial_Task.Commit();

                foreach (var materialId in materialIds)
                {
                    var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                    if (material == null)
                    {
                        throw new Exception($"Không tìm thấy dụng cụ");
                    }

                    var existingmaterialTask = await _unitOfWork.RepositoryMaterial_Task.GetSingleByCondition(et =>
                        et.MaterialId == material.Id && et.TaskId == taskId);

                    if (existingmaterialTask == null)
                    {
                        var materialTask = new Material_Task
                        {
                            MaterialId = material.Id,
                            TaskId = taskId,
                            Status = true
                        };

                        await _unitOfWork.RepositoryMaterial_Task.Add(materialTask);
                        await _unitOfWork.RepositoryMaterial_Task.Commit();
                    }
                }

                _unitOfWork.RepositoryMaterial_Task.Delete(et => !materialIds.Contains(et.MaterialId) && et.TaskId == taskId);
                await _unitOfWork.RepositoryMaterial_Task.Commit();

                await _unitOfWork.RepositoryFarmTask.Commit();
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }
        private string GenerateSubTaskCode()
        {
            Guid uniqueId = Guid.NewGuid();

            string uniquePart = uniqueId.ToString("N").Substring(0, 8);

            return "CN" + uniquePart;
        }
        private async Task CreateEmployeeMaterialForTask(FarmTask farmTask, List<int> materialIds, List<int> employeeIds)
        {
            if (!employeeIds.Any()) throw new Exception("Nhân viên không được bỏ trống");
            for (int i = 0; i < employeeIds.Count; i++)
            {
                var employeeId = employeeIds[i];
                var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                if (employee == null)
                {
                    continue;
                }
                string taskCode = GenerateSubTaskCode();
                var employee_task = new Employee_Task
                {
                    EmployeeId = employeeId,
                    TaskId = farmTask.Id,
                    Status = false,
                    Code = taskCode
                };

                farmTask.Employee_Tasks.Add(employee_task);
            }


            for (int i = 0; i < materialIds.Count; i++)
            {
                var materialId = materialIds[i];
                var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                if (material == null)
                {
                    continue;
                }

                var material_Task = new Material_Task
                {
                    MaterialId = material.Id,
                    TaskId = farmTask.Id,
                    Status = true,
                };

                farmTask.Material_Tasks.Add(material_Task);
            }

            await _unitOfWork.RepositoryFarmTask.Add(farmTask);
            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public async Task SendNotificationToDevices(List<string> deviceTokens, Message message)
        {
            foreach (var deviceToken in deviceTokens)
            {
                message.Token = deviceToken;

                var messaging = FirebaseMessaging.DefaultInstance;
                await messaging.SendAsync(message);
            }
        }
        private async Task CreateTaskForDate(FarmTask farmTask, List<int> materialIds)
        {

            for (int i = 0; i < materialIds.Count; i++)
            {
                var materialId = materialIds[i];
                var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                if (material == null)
                {
                    continue;
                }

                var material_Task = new Material_Task
                {
                    MaterialId = material.Id,
                    TaskId = farmTask.Id,
                    Status = true,
                };

                farmTask.Material_Tasks.Add(material_Task);
            }

            await _unitOfWork.RepositoryFarmTask.Add(farmTask);
            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public PriorityEnum ParsePriorityFromString(string priorityString)
        {
            Console.WriteLine($"Attempting to parse priority: '{priorityString}'");

            switch (priorityString.Trim().ToLower())
            {
                case "thấp":
                    return PriorityEnum.Short;
                case "trung bình":
                    return PriorityEnum.Medium;
                case "cao":
                    return PriorityEnum.High;
                default:
                    throw new ArgumentException("Ưu tiên truyền vô không hợp lệ");
            }
        }

        public async Task DeleteTask(int farmTaskId)
        {
            var farmTask = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.Id == farmTaskId);

            if (farmTask == null)
            {
                throw new Exception($"Không tìm thấy nhiệm vụ");
            }

            farmTask.Status = 4;
            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public async Task UpdateStatusFormTodoToDraft(int id)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(f => f.Id == id) ?? throw new Exception("Không tìm thấy nhiệm vụ");
            if (task != null)
            {
                task.Status = 0;
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
        }

        public async Task Delete(FarmTask farmTask)
        {
            _unitOfWork.RepositoryFarmTask.Delete(a => a.Id == farmTask.Id);
            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public async Task<IEnumerable<FarmTaskModel>> GetListActive()
        {
            var includes = new Expression<Func<FarmTask, object>>[]
           {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
           };


            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                expression: t => t.Status == 0 || t.Status == 1 || t.Status == 2 || t.Status == 3,
                includes: includes
            );

            farmTasks = farmTasks.OrderByDescending(t => t.CreateDate).ToList();
            var map = await MapFarmTasks(farmTasks);

            return map.Values;
        }
        public async Task<IEnumerable<string>> ListTaskEmployee(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var employeeIds = employee_task.Select(x => x.EmployeeId).ToList();
            if (employee_task != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));

                return employees.Select(e => e.Name);
            }

            return null;
        }

        public async Task<IEnumerable<string>> ListTaskEmployeeNameCode(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }

            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var employeeIds = employee_task.Select(x => x.EmployeeId).ToList();

            if (employee_task != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));

                var employeeNameCodes = employees.Select(e => $"{e.Code} - {e.Name}");

                return employeeNameCodes;
            }

            return null;
        }


        public async Task<List<int>> ListTaskEmployeeId(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var employeeIds = employee_task.Select(x => x.EmployeeId).ToList();
            if (employee_task != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));

                return employees.Select(e => e.Id).ToList();
            }

            return null;
        }
        public async Task<IEnumerable<string>> ListMaterial(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var material_task = await _unitOfWork.RepositoryMaterial_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var materialIds = material_task.Select(x => x.MaterialId).ToList();
            if (material_task != null)
            {
                var employees = await _unitOfWork.RepositoryMaterial.GetData(expression: e => materialIds.Contains(e.Id));

                return employees.Select(e => e.Name);
            }

            return null;
        }

        public async Task<List<int>> ListMaterialId(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var material_task = await _unitOfWork.RepositoryMaterial_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var materialIds = material_task.Select(x => x.MaterialId).ToList();
            if (material_task != null)
            {
                var employees = await _unitOfWork.RepositoryMaterial.GetData(expression: e => materialIds.Contains(e.Id));

                return employees.Select(e => e.Id).ToList();
            }

            return null;
        }

        public async Task<IEnumerable<FarmTaskModel>> GetListActiveByMemberId(int id)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
           {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
           };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                .GetData(expression: t => t.ManagerId == id && t.Status == 0 || t.Status == 1 || t.Status == 2 || t.Status == 3, includes: includes);
            farmTasks = farmTasks.OrderByDescending(t => t.Status)
                     .ThenByDescending(t => t.Priority)
                     .ToList();
            var map = await MapFarmTasks(farmTasks);

            return map.Values;

        }

        public async Task<IEnumerable<FarmTaskModel>> GetTaskByTotalDay(DateTime date, int id)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
           {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
           };

            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                   expression:
                     t =>
                          date.Year == t.StartDate.Value.Year &&

                          date.Month == t.StartDate.Value.Month &&

                          date.Day == t.StartDate.Value.Day &&

                          t.ManagerId == id,

                   includes: includes

                 );
            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            farmTasks = farmTasks.OrderByDescending(t => t.Status)
                     .ThenByDescending(t => t.Priority)
                     .ToList();

            var map = await MapFarmTasks(farmTasks);

            return map.Values;
        }

        public async Task<IEnumerable<FarmTaskModel>> GetListActiveWithPagging(int pageIndex, int pageSize)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
            };

            int skipCount = (pageIndex - 1) * pageSize;

            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                expression: t => t.Status == 0 || t.Status == 1 || t.Status == 2 || t.Status == 3,
                includes: includes
            );
            var map = await MapFarmTasks(farmTasks);

            return map.Values;
        }

        public async Task<int> CheckRoleMember(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            return member.RoleId;
        }

        public async Task CreateDisagreeTask(int id, string description)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                if (task.Status == 1)
                {
                    task.Status = 6;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                    var taskEvidence = new TaskEvidence
                    {
                        Status = 1,
                        SubmitDate = DateTime.Now,
                        Description = description,
                        TaskId = id,
                        EvidenceType = 1,
                    };

                    await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
                    await _unitOfWork.RepositoryTaskEvidence.Commit();

                    string message = $"Công việc '{task.Name}' đã bị từ chối";
                    List<int> memberIds = new List<int>();
                    if (task.ManagerId.HasValue)
                    {
                        memberIds.Add(task.ManagerId.Value);
                        var managerTokens = await GetTokenByMemberId(task.ManagerId.Value);
                        await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                    }
                }
                else
                {
                    throw new Exception("Không thể từ chối");
                }

            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
        }

        public async Task SendNotificationToDeviceAndMembers(List<string> deviceTokens, string message, List<int> memberIds, int taskId)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            var deviceMessages = deviceTokens.Select(token => new Message
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = $"{task.Name}",
                    Body = message
                },
                Data = new Dictionary<string, string>
                {
                    { "TaskId", taskId.ToString() }
                }
            }).ToList();

            foreach (var deviceMessage in deviceMessages)
            {
                await SendNotificationToDevices(new List<string> { deviceMessage.Token }, deviceMessage);
            }

            foreach (var memberId in memberIds)
            {
                var individualNotification = new Domain.Entities.Notification
                {
                    Message = message,
                    MessageType = "Individual",
                    NotificationDateTime = vietnamTime,
                    IsRead = false,
                    IsNew = true,
                    TaskId = taskId
                };
                await _unitOfWork.RepositoryNotifycation.Add(individualNotification);
                await _unitOfWork.RepositoryNotifycation.Commit();

                var member_notify = new Notification_Member
                {
                    NotificationId = individualNotification.Id,
                    MemberId = memberId,
                };
                await _unitOfWork.RepositoryNotifycation_Member.Add(member_notify);
                await _unitOfWork.RepositoryNotifycation_Member.Commit();
            }
        }


        public async Task ChangeStatusFromDoneToDoing(int id, string description, int managerId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                var task = await _unitOfWork.RepositoryFarmTask.GetById(id);

                if (task != null)
                {
                    if (task.Status == 4)
                    {
                        task.Status = 3;
                        _unitOfWork.RepositoryFarmTask.Update(task);
                        await _unitOfWork.RepositoryFarmTask.Commit();

                        var taskEvidence = new TaskEvidence
                        {
                            Status = 1,
                            SubmitDate = vietnamTime,
                            Description = description,
                            TaskId = id,
                            EvidenceType = 4,
                            ManagerId = managerId
                        };

                        await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
                        await _unitOfWork.RepositoryTaskEvidence.Commit();

                        string message = $"Công việc '{task.Name}' được yêu cầu làm lại";
                        List<int> memberIds = new List<int>();
                        if (task.SuppervisorId.HasValue)
                        {
                            memberIds.Add(task.SuppervisorId.Value);
                            var managerTokens = await GetTokenByMemberId(task.SuppervisorId.Value);
                            await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                        }

                    }
                    else
                    {
                        throw new Exception("Không thể từ chối");
                    }

                }
                else
                {
                    throw new Exception("Không tìm thấy nhiệm vụ");
                }
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception($"Lỗi: {ex.Message}");
            }

        }

        public async Task ChangeStatusToPendingAndCancel(int id, EvidencePendingAndCancel taskEvidence, int status, int? managerId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var task = await _unitOfWork.RepositoryFarmTask.GetById(id);

                if (task != null)
                {
                    if (task.Status == 3 || task.Status == 5 || task.Status == 2)
                    {
                        if (status != 5 && status != 7)
                        {
                            throw new Exception("Trạng thái truyền vào chỉ được tạm hoãn hoặc hủy bỏ");
                        }
                        task.Status = status;
                        _unitOfWork.RepositoryFarmTask.Update(task);
                        await _unitOfWork.RepositoryFarmTask.Commit();

                        var taskEvidenceType = 0;
                        var statusString = "";
                        List<int> memberIds = new List<int>();
                        if (status == 5)
                        {
                            taskEvidenceType = 3;
                            statusString = "tạm hoãn";
                        }
                        else if (status == 7)
                        {
                            taskEvidenceType = 2;
                            statusString = "hủy bỏ";
                        }
                        string message = $"Công việc '{task.Name}' đã bị {statusString}";
                        if (managerId.HasValue)
                        {
                            memberIds.Add(task.SuppervisorId.Value);

                            var member = await _unitOfWork.RepositoryMember.GetById(managerId.Value);
                            if (member.RoleId != 1) throw new Exception("Truyền sai id của manager");

                            await AddTaskEvidenceeWithImage(taskEvidence, taskEvidenceType, managerId.Value, id);

                            var managerTokens = await GetTokenByMemberId(task.SuppervisorId.Value);
                            await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);

                        }
                        else
                        {
                            var managerIds = await GetManagerId();

                            memberIds = new List<int>(managerIds);

                            await AddTaskEvidenceeWithImage(taskEvidence, taskEvidenceType, null, id);

                            var managerTokens = await GetTokenAllManger();

                            await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);

                        }
                    }
                    else
                    {
                        throw new Exception("Không thể từ chối");
                    }
                }
                else
                {
                    throw new Exception("Không tìm thấy nhiệm vụ");
                }
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }


        public async Task<List<string>> GetTokenAllManger()
        {
            var managers = await _unitOfWork.RepositoryMember.GetData(m => m.RoleId == 1);
            foreach (var manager in managers)
            {
                var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => h.MemberId == manager.Id);

                if (hubConnections == null)
                {
                    throw new Exception("Không tìm thấy kết nối");
                }

                var connectionIds = hubConnections.Select(h => h.ConnectionId).ToList();
                return connectionIds;
            }

            return null;
        }

        public async Task AddTaskEvidenceeWithImage(EvidencePendingAndCancel taskEvidence, int evidenceType, int? managerId, int taskId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                if (evidenceType != 2 && evidenceType != 3)
                {
                    throw new Exception("Chỉ được ở trạng thái cancel(2) và pending(3)");
                }

                var taskEvidenceCreate = new TaskEvidence
                {
                    Status = 1,
                    SubmitDate = vietnamTime,
                    Description = taskEvidence.Description,
                    TaskId = taskId,
                    EvidenceType = evidenceType,
                    ManagerId = managerId
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

        public async Task<List<EvidenceImage>> UploadEvidenceImages(int id, EvidencePendingAndCancel evidenceCreateUpdateModel)
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

        public async Task ChangeStatusToDoing(int id)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(e => e.TaskId == id);
            if (employee_task.Any())
            {
                if (task != null)
                {
                    task.Status = 3;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                    string message = $"Công việc '{task.Name}' đã bị chuyển sang đang thực hiện";
                    if (task.ManagerId.HasValue)
                    {
                        var managerIds = await GetManagerId();

                        var memberIds = new List<int>(managerIds);

                        var managerTokens = await GetTokenAllManger();

                        await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                    }
                }
                else
                {
                    throw new Exception("Không tìm thấy nhiệm vụ");
                }
            }
            else
            {
                throw new Exception("Cần phải có nhân viên mới đổi được sang thực hiện");
            }

        }



        public async Task DisDisagreeTask(int id)
        {

            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                if (task.Status == 6)
                {
                    task.Status = 1;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                }
                else
                {
                    throw new Exception("Không thể từ chối");
                }
            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ ");
            }
            var evidences = await _unitOfWork.RepositoryTaskEvidence.GetData(e => e.TaskId == id);
            foreach (var evidence in evidences)
            {
                _unitOfWork.RepositoryTaskEvidence.Delete(evidence);
                await _unitOfWork.RepositoryTaskEvidence.Commit();
            }

        }

        public async Task ChangeStatusToClose(int id)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                task.Status = 8;
                _unitOfWork.RepositoryFarmTask.Update(task);
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ ");
            }
        }

        public async Task ChangeStatusToDone(int id)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == id && (s.ActualEfforMinutes != 0 || s.ActualEffortHour != 0));
                if (subtask.Any())
                {
                    task.Status = 4;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                    string message = $"Công việc '{task.Name}' đã hoàn thành";
                    if (task.ManagerId.HasValue)
                    {
                        var managerIds = await GetManagerId();

                        var memberIds = new List<int>(managerIds);

                        var managerTokens = await GetTokenAllManger();

                        await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, task.Id);
                    }
                }
                else
                {
                    throw new Exception("Phải ghi nhận giờ làm trước khi chuyển đổi trạng thái sang hoàn thành ");
                }
            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ ");
            }
        }

        public async Task<TaskByEmployeeDatesEffort> GetTaskByEmployeeDates(int employeeId, [FromQuery] DateTime? startDay, [FromQuery] DateTime? endDay, int pageIndex, int pageSize, [FromQuery] int? status)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType,
            };
            var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.EmployeeId == employeeId &&
                                                 (!startDay.HasValue || !endDay.HasValue || (
                                                            (startDay.Value.Year <= s.DaySubmit.Value.Year &&
                                                             endDay.Value.Year >= s.DaySubmit.Value.Year) &&
                                                            (startDay.Value.Month <= s.DaySubmit.Value.Month &&
                                                             endDay.Value.Month >= s.DaySubmit.Value.Month) &&
                                                            (startDay.Value.Day <= s.DaySubmit.Value.Day &&
                                                             endDay.Value.Day >= s.DaySubmit.Value.Day)
                                                         )));


            var taskIds = subtasks.Select(s => s.TaskId);

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(expression: t => taskIds.Contains(t.Id) && (t.Status == 8 || t.Status == 7), includes: includes);
            if (status != null)
            {
                tasks = await _unitOfWork.RepositoryFarmTask.GetData(expression: t => taskIds.Contains(t.Id) && (t.Status == status), includes: includes);
            }

            tasks = tasks.OrderByDescending(t => t.StartDate).ToList();

            var totalTaskCount = tasks.Count();
            var taskEffortIds = tasks.Select(t => t.Id);

            var subtaskEffortOfEmployee = await _unitOfWork.RepositoryEmployee_Task.GetData(s => taskEffortIds.Contains(s.TaskId) && s.EmployeeId == employeeId);

            var effortMinutesOfEmployee = subtaskEffortOfEmployee.Sum(s => s.ActualEfforMinutes);
            var effortHoursOfEmployee = subtaskEffortOfEmployee.Sum(s => s.ActualEffortHour);

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);

            var map = new Dictionary<FarmTask, TaskByEmployeeDates>();

            var farmTaskModels = _mapper.Map<IEnumerable<FarmTask>, IEnumerable<TaskByEmployeeDates>>(tasks);

            foreach (var pair in tasks.Zip(farmTaskModels, (ft, ftModel) => new { ft, ftModel }))
            {
                map.Add(pair.ft, pair.ftModel);
            }

            foreach (var farmTask in tasks)
            {
                var member = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId);

                if (member != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].SupervisorName = member.Name;
                }

                var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == farmTask.Id && s.EmployeeId == employeeId);
                var effortMinutes = subtaskEffort.Select(s => s.ActualEfforMinutes).Sum();
                var effortHours = subtaskEffort.Select(s => s.ActualEffortHour).Sum();

                if (effortMinutes != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].ActualEfforMinutes = effortMinutes;
                }
                if (effortHours != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].ActualEffortHour = effortHours;
                }

                var evidence = await _unitOfWork.RepositoryTaskEvidence.GetData(f => f.TaskId == farmTask.Id);
                var totalEvidence = evidence.Count();
                if (totalEvidence > 0 && map.ContainsKey(farmTask))
                {
                    map[farmTask].IsHaveEvidence = true;
                }
                var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == farmTask.Id && s.Status == true);
                if (subtask.Any() && map.ContainsKey(farmTask))
                {
                    map[farmTask].IsHaveSubtask = true;
                }
                var subtaskOfTask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == farmTask.Id);
                var employeeIdOftask = subtaskOfTask.Select(e => e.EmployeeId).Distinct().ToList();


                if (farmTask.OverallEffortHour.HasValue && farmTask.OverallEfforMinutes.HasValue && map.ContainsKey(farmTask))
                {
                    map[farmTask].EffortOfTask = $"{farmTask.OverallEffortHour} giờ {farmTask.OverallEfforMinutes} phút";
                    map[farmTask].TotaslEmployee = $"{employeeIdOftask.Count()} người";
                }
            }

            return new TaskByEmployeeDatesEffort
            {
                TaskByEmployeeDates = map.Values,
                TotalPage = totalPages,
                OverallEfforMinutesOfEmployee = effortMinutesOfEmployee,
                OverallEffortHourOfEmployee = effortHoursOfEmployee
            };
        }

        public async Task<IEnumerable<FarmTaskModel>> GetTaskPrepareAndDoing(int id)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Manager,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t => t.TaskType
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                expression: t => t.ManagerId == id &&
                                 (t.Status == 0 || t.Status == 1),
                includes: includes);


            var totalTaskCount = farmTasks.Count();

            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }


            var map = await MapFarmTasks(farmTasks);

            return map.Values;
        }

        public async Task CreateTaskClone(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId) ?? throw new Exception("Không tìm thấy công việc");
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            string taskCode = GenerateTaskCode();

            var taskClone = new FarmTask
            {
                CreateDate = currentTime,
                Description = task.Description?.Trim(),
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                Priority = task.Priority,
                SuppervisorId = task.SuppervisorId == 0 ? (int?)null : task.SuppervisorId,
                FieldId = task.FieldId == 0 ? (int?)null : task.FieldId,
                TaskTypeId = task.TaskTypeId == 0 ? (int?)null : task.TaskTypeId,
                IsRepeat = false,
                ManagerId = task.ManagerId == 0 ? (int?)null : task.ManagerId,
                PlantId = task.PlantId == 0 ? (int?)null : task.PlantId,
                LiveStockId = task.LiveStockId == 0 ? (int?)null : task.LiveStockId,
                Name = $"{task.Name} (Bản sao)",
                Status = 0,
                Remind = task.Remind,
                OriginalTaskId = 0,
                AddressDetail = task.AddressDetail?.Trim(),
                OverallEfforMinutes = 0,
                OverallEffortHour = 0,
                UpdateDate = null,
                Code = taskCode,
                IsPlant = task.IsPlant,
                IsSpecific = task.IsSpecific,
                IsExpired = false,
            };
            await _unitOfWork.RepositoryFarmTask.Add(taskClone);
            await _unitOfWork.RepositoryFarmTask.Commit();
        }
    }
}