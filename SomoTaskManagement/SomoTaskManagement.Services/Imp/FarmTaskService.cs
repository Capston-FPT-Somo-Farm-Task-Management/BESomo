using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
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
using SomoTaskManagement.Services.Interface;
using SomoTaskManagement.Services.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnidecodeSharpFork;
using Notification = SomoTaskManagement.Domain.Entities.Notification;

namespace SomoTaskManagement.Services.Imp
{
    public class FarmTaskService : IFarmTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FarmTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                var member = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId);

                if (member != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].SupervisorName = member.Name;
                }

                if (member != null && map.ContainsKey(farmTask) && farmTask.ManagerId == null)
                {
                    map[farmTask].Avatar = member.Avatar;
                }
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
                var dateRepeate = taskRepeate.Select(t => t.StartDate).ToList();
                if (dateRepeate != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].DateRepeate = dateRepeate;
                }

                //if(farmTask.OriginalTaskId == 0)
                //{
                //    map[farmTask].IsRepeat = false;
                //}
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

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                                        .GetData(
                                            expression: t => t.ManagerId == memberId && (t.StartDate >= startOfWeek || t.StartDate <= endOfWeek), includes: includes
                                        );

            var taskCounts = allDaysOfWeek
                .Select(day => new TaskCountPerDayModel
                {
                    Date = day.Date,
                    TaskCount = farmTasks.Count(f => f.StartDate.Date == day.Date),
                    TotalTaskOfLivestock = farmTasks.Count(f => f.StartDate.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 1)),
                    TotalTaskOfPlant = farmTasks.Count(f => f.StartDate.Date == day.Date && (f.Field != null && f.FieldId.HasValue && f.Field.Status == 0)),
                    TotalTaskOfOther = farmTasks.Count(f => f.StartDate.Date == day.Date && (f.Field == null && !f.FieldId.HasValue))
                })
                .ToList();

            return taskCounts;
        }

        public async Task<TotalTypeOfTaskInWeek> GetTotalTypeOfTaskInWeekByMember(int memberId)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t => t.Field,
            };
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                                       .GetData(
                                           expression: t => t.ManagerId == memberId && t.StartDate >= startOfWeek && t.EndDate <= endOfWeek,
                                            includes: includes
                                       );

            var totalTaskOfLivestock = farmTasks.Count(f => (f.Field != null && f.FieldId.HasValue && f.Field.Status == 1));
            var totalTaskOfPlant = farmTasks.Count(f => (f.Field != null && f.FieldId.HasValue && f.Field.Status == 0));
            int totalTaskOfOther = farmTasks.Count(f => (f.Field == null && !f.FieldId.HasValue));

            return new TotalTypeOfTaskInWeek
            {
                TotalTaskOfLivestock = totalTaskOfLivestock,
                TotalTaskOfPlant = totalTaskOfPlant,
                TotalTaskOfOther = totalTaskOfOther,
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
            var dateRepeate = taskRepeate.Select(t => t.StartDate).ToList();

            var member = await _unitOfWork.RepositoryMember.GetById(farmTask.ManagerId);
            var receiver = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId);
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

            var supervisorName = receiver != null ? $"{receiver.Code} - {receiver.Name}" : null;

            var farmTaskModel = new GetFarmTaskModel
            {
                Id = farmTask.Id,
                CreateDate = farmTask.CreateDate,
                StartDate = farmTask.StartDate,
                EndDate = farmTask.EndDate,
                Description = farmTask.Description,
                Priority = priorityString,
                IsRepeat = farmTask.IsRepeat,
                SupervisorName = supervisorName,
                SuppervisorId = receiver.Id,
                ManagerName = member != null ? member.Name : null,
                PlantName = plant != null ? plant.Name : null,
                liveStockName = liveStock != null ? liveStock.Name : null,
                FieldName = field != null ? $"{field.Code} - {field.Name}" : null,
                TaskTypeName = taskType != null ? taskType.Name : null,
                TaskTypeId = taskType != null ? taskType.Id : 0,
                Remind = farmTask.Remind,
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
                OverallEfforMinutes = farmTask.OverallEfforMinutes,
                OverallEffortHour = farmTask.OverallEffortHour,
                AddressDetail = farmTask.AddressDetail,
            };
            if (farmTask.OriginalTaskId == 0)
            {
                farmTaskModel.IsParent = true;
            }
            else
            {
                farmTaskModel.IsRepeat = false;
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

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                    .GetData(expression: task => task.StartDate.Date == date.Date,
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
            var farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(
                expression: t => (t.ManagerId == id || t.ManagerId == null) &&
                                 t.Status == status &&
                                 (!date.HasValue ||
                                  (date.Value.Date >= t.StartDate.Date && date.Value.Date <= t.EndDate.Date)),
                includes: includes);

            

            if (checkTaskParent == 0)
            {
                farmTasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == 0 && t.IsRepeat == true && t.Status != 4 && (t.ManagerId == id || t.ManagerId == null) &&
                                 t.Status == status &&
                                 (!date.HasValue ||
                                  (date.Value.Date >= t.StartDate.Date && date.Value.Date <= t.EndDate.Date)));
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

            farmTasks = farmTasks.OrderByDescending(t => t.Priority).ThenByDescending(t => t.StartDate)
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
                                  (date.Value.Date >= t.StartDate.Date && date.Value.Date <= t.EndDate.Date)),
                includes: includes);

            if (!string.IsNullOrEmpty(taskName))
            {
                farmTasks = farmTasks.Where(t => t.Name.Unidecode().IndexOf(taskName.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (checkTaskParent != 0)
            {
                var farmTaskParent = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == 0 && t.IsRepeat == true);
            }

            var totalTaskCount = farmTasks.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);


            if (farmTasks == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            farmTasks = farmTasks.OrderByDescending(t => t.Priority).ThenByDescending(t => t.StartDate)
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
               expression: t => t.SuppervisorId == id &&
                                    t.Status == status &&
                                (!date.HasValue ||
                                 (date.Value.Date >= t.StartDate.Date && date.Value.Date <= t.EndDate.Date)),
               includes: includes);



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

            farmTasks = farmTasks.OrderByDescending(t => t.Priority).ThenByDescending(t => t.StartDate)
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
        private string GenerateTaskCode(DateTime vietnamTime, int counter)
        {
            return "CV" + vietnamTime.Year.ToString().Substring(2) + counter.ToString("D5");
        }

        public async Task ProcessTaskCreation(int memberId, TaskRequestModel taskModel)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                var member = await _unitOfWork.RepositoryMember.GetById(memberId) ?? throw new Exception("Không tìm thấy người dùng");
                var startDate = taskModel.FarmTask.StartDate;
                var endDate = taskModel.FarmTask.EndDate;
                int taskId = 0;
                int originalTaskId = 0;

                foreach (var date in new List<DateTime> { startDate }.Concat(taskModel.Dates))
                {
                    string taskCode = GenerateTaskCode(vietnamTime, taskCounter);
                    var farmTaskNew = new FarmTask
                    {
                        CreateDate = vietnamTime,
                        StartDate = new DateTime(date.Year, date.Month, date.Day, startDate.Hour, startDate.Minute, startDate.Second),
                        EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Hour, endDate.Minute, endDate.Second).AddDays((endDate - startDate).Days),
                        Description = taskModel.FarmTask.Description,
                        Priority = (int)ParsePriorityFromString(taskModel.FarmTask.Priority),
                        SuppervisorId = taskModel.FarmTask.SuppervisorId,
                        FieldId = taskModel.FarmTask.FieldId == 0 ? (int?)null : taskModel.FarmTask.FieldId,
                        TaskTypeId = taskModel.FarmTask.TaskTypeId,
                        IsRepeat = (originalTaskId == 0) ? taskModel.FarmTask.IsRepeat : false,
                        ManagerId = taskModel.FarmTask.ManagerId == 0 ? (int?)null : taskModel.FarmTask.ManagerId,
                        PlantId = taskModel.FarmTask.PlantId == 0 ? (int?)null : taskModel.FarmTask.PlantId,
                        LiveStockId = taskModel.FarmTask.LiveStockId == 0 ? (int?)null : taskModel.FarmTask.LiveStockId,
                        Name = taskModel.FarmTask.Name,
                        Status = 0,
                        Remind = taskModel.FarmTask.Remind,
                        OriginalTaskId = originalTaskId,
                        AddressDetail = taskModel.FarmTask.AddressDetail,
                        OverallEfforMinutes = taskModel.FarmTask.OverallEfforMinutes,
                        OverallEffortHour = taskModel.FarmTask.OverallEffortHour,
                        UpdateDate = null,
                        Code = taskCode,
                    };
                    taskCounter++;
                    var suppervisor = await _unitOfWork.RepositoryMember.GetById(taskModel.FarmTask.SuppervisorId);

                    if (suppervisor == null)
                    {
                        throw new Exception("Không tìm thấy người dám sát");
                    }

                    var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => h.MemberId == taskModel.FarmTask.SuppervisorId);

                    var supervisorTokens = await GetTokenByMemberId(taskModel.FarmTask.SuppervisorId);

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
                            { "taskId", taskId.ToString() }
                        }
                        };

                        var remindTime = farmTaskNew.StartDate.AddMinutes(-farmTaskNew.Remind);
                        var currentTime = vietnamTime;

                        if (remindTime > currentTime)
                        {
                            var delayMilliseconds = (int)(remindTime - currentTime).TotalMilliseconds;
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
                    await CreateTaskForDate(farmTaskNew, taskModel.EmployeeIds, taskModel.MaterialIds);
                    taskId = farmTaskNew.Id;
                    if (taskModel.FarmTask.IsRepeat)
                    {
                        if (originalTaskId == 0)
                        {
                            originalTaskId = farmTaskNew.Id;
                        }
                    }
                    await ScheduleAndChangeStatus(farmTaskNew.Id);
                }

                var roleMember = await CheckRoleMember(memberId);

                List<string> deviceTokens = new List<string>();

                var listManager = await GetManagerId();

                List<int> listMemberNotify = new List<int>();

                if (roleMember == 4)
                {
                    foreach (var manager in listManager)
                    {
                        var managerTokens = await GetTokenByMemberId(manager);
                        deviceTokens.AddRange(managerTokens);
                        listMemberNotify.Add(manager);
                    }
                }
                else if (roleMember == 2)
                {
                    var supervisorTokens = await GetTokenByMemberId(taskModel.FarmTask.SuppervisorId);
                    deviceTokens.AddRange(supervisorTokens);

                    listMemberNotify.Add(taskModel.FarmTask.SuppervisorId);
                }

                var deviceMessages = deviceTokens.Select(token => new Message
                {
                    Token = token,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = $"{taskModel.FarmTask.Name}",
                        Body = $"Bạn đã nhận một nhiệm vụ '{taskModel.FarmTask.Name}'"
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
                    Message = $"Bạn đã nhận một nhiệm vụ '{taskModel.FarmTask.Name}'",
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
                    NotificationId = individualNotification.Id
                };

                foreach (var memberNotify in listMemberNotify)
                {
                    member_notify.MemberId = memberNotify;
                    await _unitOfWork.RepositoryNotifycation_Member.Add(member_notify);
                    await _unitOfWork.RepositoryNotifycation_Member.Commit();
                }

                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }

        }

        private async Task ScheduleAndChangeStatus(int taskId)
        {
            Console.WriteLine($"ScheduleAndChangeStatus called for taskId: {taskId}");

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var farmTask = await _unitOfWork.RepositoryFarmTask.GetById(taskId);

            if (farmTask != null && farmTask.StartDate == currentTime)
            {
                Console.WriteLine($"Updating status for taskId: {taskId}");
                farmTask.Status = 2;
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else if (farmTask != null && farmTask.StartDate > currentTime)
            {
                var remindTime = farmTask.StartDate;
                var timer = new System.Timers.Timer((remindTime - currentTime).TotalMilliseconds);
                timer.Elapsed += async (sender, e) =>
                {
                    Console.WriteLine($"Updating status for taskId: {taskId}");
                    farmTask.Status = 2;
                    await _unitOfWork.RepositoryFarmTask.Commit();
                    timer.Dispose();
                };
                timer.AutoReset = false;
                timer.Start();
            }
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
        private async Task CreateTaskForDate(FarmTask farmTask, List<int> employeeIds, List<int> materialIds)
        {
            for (int i = 0; i < employeeIds.Count; i++)
            {
                var employeeId = employeeIds[i];
                var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                if (employee == null)
                {
                    throw new Exception($"Không tìm thấy nhân viên");
                }

                var employee_Task = new Employee_Task
                {
                    EmployeeId = employee.Id,
                    TaskId = farmTask.Id,
                    ActualEfforMinutes = 0,
                    Status = false,
                };

                farmTask.Employee_Tasks.Add(employee_Task);
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

        public async Task Update(int farmTaskId, TaskRequestModel taskModel)
        {
            var farmTask = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.Id == farmTaskId);
            if (farmTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var taskRepeate = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == farmTask.Id);
            var dateRepeate = taskRepeate.Select(t => t.StartDate).ToList();
            var errorResponse = new
            {
                ErrorMessage = "Không thể bỏ ngày lặp lại khi có nhiệm vụ lặp lại đã hoàn thành hoặc đã qua thời gian bắt đầu.",
                RepeatDates = dateRepeate.Where(date => date <= currentDateTime).ToList()
            };
            var jsonResponse = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);

            var datesToKeep = taskModel.Dates.Select(d => d.Date).ToList();
            if (farmTask.OriginalTaskId == 0)
            {
                if (taskModel.Dates.Any())
                {
                    foreach (var date in taskModel.Dates)
                    {
                        if (date == farmTask.StartDate.Date)
                        {
                            continue;
                        }
                        else if (date < currentDateTime)
                        {
                            throw new Exception("Không thể cập nhật nhiệm vụ với ngày bắt đầu trong quá khứ.");
                        }

                        var existingTask = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.OriginalTaskId == farmTask.Id && t.StartDate.Date == date.Date);

                        if (existingTask == null)
                        {
                            var newTask = CreateNewFarmTask(date, farmTask, taskModel);
                            await _unitOfWork.RepositoryFarmTask.Add(newTask);
                            await CreateTaskForDate(newTask, taskModel.EmployeeIds, taskModel.MaterialIds);
                        }
                        else
                        {
                            await UpdateFarmTask(existingTask, date, taskModel);
                        }
                    }
                }

                await UpdateFarmTask(farmTask, farmTask.StartDate.Date, taskModel);

                _unitOfWork.RepositoryFarmTask.Delete(t => !taskModel.Dates.Contains(t.StartDate.Date) && t.OriginalTaskId == farmTask.Id);
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
            else
            {
                await UpdateFarmTask(farmTask, farmTask.StartDate.Date, taskModel);
                await _unitOfWork.RepositoryFarmTask.Commit();
            }

            if (farmTask.Status == 2 || farmTask.Status == 3)
            {
                var childTasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.OriginalTaskId == farmTask.Id);

                foreach (var childTask in childTasks)
                {
                    await UpdateFarmTask(childTask, childTask.StartDate.Date, taskModel);
                }

                await _unitOfWork.RepositoryFarmTask.Commit();
            }
        }

        private FarmTask CreateNewFarmTask(DateTime date, FarmTask parentTask, TaskRequestModel taskModel)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var newTask = new FarmTask
            {
                CreateDate = currentDateTime,
                Name = taskModel.FarmTask.Name,
                Description = taskModel.FarmTask.Description,
                Priority = (int)ParsePriorityFromString(taskModel.FarmTask.Priority),
                SuppervisorId = taskModel.FarmTask.SuppervisorId,
                FieldId = taskModel.FarmTask.FieldId,
                TaskTypeId = taskModel.FarmTask.TaskTypeId,
                PlantId = taskModel.FarmTask.PlantId == 0 ? (int?)null : taskModel.FarmTask.PlantId,
                LiveStockId = taskModel.FarmTask.LiveStockId == 0 ? (int?)null : taskModel.FarmTask.LiveStockId,
                StartDate = new DateTime(date.Year, date.Month, date.Day, parentTask.StartDate.Hour, parentTask.StartDate.Minute, parentTask.StartDate.Second),
                EndDate = new DateTime(date.Year, date.Month, date.Day, parentTask.EndDate.Hour, parentTask.EndDate.Minute, parentTask.EndDate.Second).AddDays((parentTask.EndDate - parentTask.StartDate).Days),
                Status = 0,
                Remind = taskModel.FarmTask.Remind,
                IsRepeat = false,
                ManagerId = taskModel.FarmTask.ManagerId,
                OriginalTaskId = parentTask.Id,
                Code = "CV2300001",
                OverallEfforMinutes = taskModel.FarmTask.OverallEfforMinutes,
                OverallEffortHour = taskModel.FarmTask.OverallEffortHour,
                UpdateDate = null,
                AddressDetail = taskModel.FarmTask.AddressDetail
            };

            return newTask;
        }

        private async Task UpdateFarmTask(FarmTask farmTask, DateTime date, TaskRequestModel taskModel)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var startDate = taskModel.FarmTask.StartDate;
            var endDate = taskModel.FarmTask.EndDate;

            farmTask.Name = taskModel.FarmTask.Name;
            farmTask.Description = taskModel.FarmTask.Description;
            farmTask.Priority = (int)ParsePriorityFromString(taskModel.FarmTask.Priority);
            farmTask.SuppervisorId = taskModel.FarmTask.SuppervisorId;
            farmTask.FieldId = taskModel.FarmTask.FieldId;
            farmTask.TaskTypeId = taskModel.FarmTask.TaskTypeId;
            farmTask.PlantId = taskModel.FarmTask.PlantId == 0 ? (int?)null : taskModel.FarmTask.PlantId;
            farmTask.LiveStockId = taskModel.FarmTask.LiveStockId == 0 ? (int?)null : taskModel.FarmTask.LiveStockId;
            farmTask.StartDate = new DateTime(date.Year, date.Month, date.Day, startDate.Hour, startDate.Minute, startDate.Second);
            farmTask.EndDate = new DateTime(date.Year, date.Month, date.Day, endDate.Hour, endDate.Minute, endDate.Second).AddDays((endDate - startDate).Days);
            //farmTask.Status = taskModel.FarmTask.Status;
            farmTask.Remind = taskModel.FarmTask.Remind;
            farmTask.IsRepeat = taskModel.FarmTask.IsRepeat;
            farmTask.ManagerId = taskModel.FarmTask.ManagerId;
            farmTask.OverallEfforMinutes = taskModel.FarmTask.OverallEfforMinutes;
            farmTask.OverallEffortHour = taskModel.FarmTask.OverallEffortHour;
            farmTask.UpdateDate = currentDateTime;
            farmTask.AddressDetail = taskModel.FarmTask.AddressDetail;

            if(farmTask.OriginalTaskId != 0)
            {
                farmTask.IsRepeat = false;
            }
            //update list employee
            if (taskModel.EmployeeIds == null || !taskModel.EmployeeIds.Any()) throw new Exception("Danh sách nhân viên không được null");
            foreach (var employeeId in taskModel.EmployeeIds)
            {

                var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                if (employee == null)
                {
                    throw new Exception($"Không tìm thấy nhân viên với ID {employeeId}");
                }

                var existingEmployeeTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(et =>
                    et.EmployeeId == employee.Id && et.TaskId == farmTask.Id);

                if (existingEmployeeTask == null)
                {
                    var employeeTask = new Employee_Task
                    {
                        EmployeeId = employee.Id,
                        TaskId = farmTask.Id,
                        Status =false
                    };

                    await _unitOfWork.RepositoryEmployee_Task.Add(employeeTask);
                    await _unitOfWork.RepositoryEmployee_Task.Commit();
                }
            }
            _unitOfWork.RepositoryEmployee_Task.Delete(et => !taskModel.EmployeeIds.Contains(et.EmployeeId) && et.TaskId == farmTask.Id);
            await _unitOfWork.RepositoryEmployee_Task.Commit();

            //update list material
            foreach (var materialId in taskModel.MaterialIds)
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

            _unitOfWork.RepositoryMaterial_Task.Delete(et => !taskModel.MaterialIds.Contains(et.MaterialId) && et.TaskId == farmTask.Id);
            await _unitOfWork.RepositoryMaterial_Task.Commit();

            await _unitOfWork.RepositoryFarmTask.Commit();
        }

        public async Task UpdateStatus(int id, int status)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(f => f.Id == id);
            if (status != 1)
            {
                throw new Exception("Chỉ được đổi trạng thái sang đang thực hiện");
            }
            if (task != null)
            {
                task.Status = status;
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
                          date.Year == t.StartDate.Year &&

                          date.Month == t.StartDate.Month &&

                          date.Day == t.StartDate.Day &&

                          t.ManagerId == id ,

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
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                DateTime taskCreationTime = task.StartDate;

                TimeSpan timeElapsed = taskCreationTime - currentTime;

                if (timeElapsed.TotalMinutes >= 30)
                {
                    task.Status = 5;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

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
                else
                {
                    throw new Exception("Không thể từ chối nhiệm vụ chuẩn bị bắt đầu trong 30 phút nữa.");
                }
            }
            else
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
        }


        public async Task DisDisagreeTask(int id)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(id);
            if (task != null)
            {
                task.Status = 0;
                _unitOfWork.RepositoryFarmTask.Update(task);
                await _unitOfWork.RepositoryFarmTask.Commit();
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

        public async Task<TaskByEmployeeDatesEffort> GetTaskByEmployeeDates(int employeeId, DateTime? startDay, DateTime? endDay, int pageIndex, int pageSize, int? status)
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
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.EmployeeId == employeeId);
            var taskIds = subtask.Select(s => s.TaskId);
            var task = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) &&
                                                                        (!startDay.HasValue || !endDay.HasValue || (
                                                               (startDay.Value.Year <= t.StartDate.Year &&
                                                                endDay.Value.Year >= t.StartDate.Year) &&
                                                               (startDay.Value.Month <= t.StartDate.Month &&
                                                                endDay.Value.Month >= t.StartDate.Month) &&
                                                               (startDay.Value.Day <= t.StartDate.Day &&
                                                                endDay.Value.Day >= t.StartDate.Day)
                                                                        )), includes: includes);

            if (status.HasValue)
            {
                task = task.Where(t => t.Status == status.Value);
            }

            var totalTaskCount = task.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);



            var map = new Dictionary<FarmTask, TaskByEmployeeDates>();

            var farmTaskModels = _mapper.Map<IEnumerable<FarmTask>, IEnumerable<TaskByEmployeeDates>>(task);

            foreach (var pair in task.Zip(farmTaskModels, (ft, ftModel) => new { ft, ftModel }))
            {
                map.Add(pair.ft, pair.ftModel);
            }

            foreach (var farmTask in task)
            {
                var member = await _unitOfWork.RepositoryMember.GetById(farmTask.SuppervisorId);

                if (member != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].SupervisorName = member.Name;
                }

                var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == farmTask.Id && s.EmployeeId == employeeId);
                var effortMinutes = subtaskEffort.ActualEfforMinutes;
                var effortHours = subtaskEffort.ActualEffortHour;
                if (effortMinutes != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].ActualEfforMinutes = effortMinutes;
                }
                if (effortHours != null && map.ContainsKey(farmTask))
                {
                    map[farmTask].ActualEffortHour = effortHours;
                }
            }

            return new TaskByEmployeeDatesEffort
            {
                TaskByEmployeeDates = map.Values,
                TotalPage = totalPages
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
                expression: t => t.ManagerId == id  &&
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
    }
}
