using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Twilio.Rest.Api.V2010.Account;
using FirebaseAdmin.Messaging;

namespace SomoTaskManagement.Services.Imp
{
    public class SubTaskService : ISubTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private static int taskCounter = 1;
        public SubTaskService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }


        public async Task<IEnumerable<SubTaskModel>> SubtaskByTask(int taskId, int? employeeId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Nhiệm vụ không tìm thấy");
            }

            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };

            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId && s.Status == true, includes: includes);


            if (employeeId.HasValue)
            {
                subtask = subtask.Where(s => s.EmployeeId == employeeId.Value).ToList();
            }
            subtask = subtask.OrderBy(s => s.DaySubmit).ToList();

            if (!subtask.Any())
            {
                throw new Exception("Nhiệm vụ con rỗng");
            }

            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubTaskModel>>(subtask);
        }


        public async Task<IEnumerable<SubTaskModel>> NonSubtaskByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Nhiệm vụ không tìm thấy");
            }
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId && s.Status == false, includes: includes);
            if (!subtask.Any())
            {
                throw new Exception("Nhiệm vụ con rỗng");
            }
            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubTaskModel>>(subtask);
        }

        public async Task UpdateSubTasks(int subtaskId, SubTaskUpdateModel subTask)
        {

            var subtaskUpdate = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);

            if (subtaskUpdate == null)
            {
                throw new Exception("Không tìm thấy công việc con");
            }
            var employee = await _unitOfWork.RepositoryEmployee.GetById(subTask.EmployeeId);

            var efffortOfEmployeeInSubmitDay = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.EmployeeId == subTask.EmployeeId && s.DaySubmit == subTask.DaySubmit);
            var tottalEffortHour = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEffortHour);
            var tottalEffortHourAfter = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEffortHour) + subTask.OverallEffortHour;

            var tottalEffortMinutes = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEfforMinutes);
            var tottalEffortMinutesAfter = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEfforMinutes) + subTask.OverallEfforMinutes;

            if (tottalEffortMinutesAfter >= 60)
            {
                tottalEffortHourAfter += tottalEffortMinutesAfter / 60;

                tottalEffortMinutesAfter %= 60;
            }
            if (tottalEffortMinutes >= 60)
            {
                tottalEffortHour += tottalEffortMinutes / 60;

                tottalEffortMinutes %= 60;
            }

            if (tottalEffortHourAfter >= 24)
                throw new Exception($"Nhân viên {employee.Name} trong ngày {subTask.DaySubmit.Value.ToString("dd/MM/yyyy")} đã làm {tottalEffortHour} giờ {tottalEffortMinutes} phút");

            subtaskUpdate.Description = subTask.Description;
            subtaskUpdate.Name = subTask.Name;
            subtaskUpdate.Status = true;
            subtaskUpdate.DaySubmit = subTask.DaySubmit;
            subtaskUpdate.ActualEffortHour = subTask.OverallEffortHour;
            subtaskUpdate.ActualEfforMinutes = subTask.OverallEfforMinutes;


            taskCounter++;
            _unitOfWork.RepositoryEmployee_Task.Update(subtaskUpdate);
            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }

        public async Task CreateSubTasks(SubTaskCreateModel subTask)
        {
            string taskCode = GenerateTaskCode();
            var subTaskNew = new Employee_Task
            {
                TaskId = subTask.TaskId,
                EmployeeId = subTask.EmployeeId,
                Description = subTask.Description,
                Name = subTask.Name,
                DaySubmit = subTask.DaySubmit,
                Code = taskCode,
                Status = true,
                ActualEfforMinutes = subTask.OverallEfforMinutes,
                ActualEffortHour = subTask.OverallEffortHour
            };

            var task = await _unitOfWork.RepositoryEmployee_Task.GetData(t => t.TaskId == subTask.TaskId);
            var employeeOfTask = task.Select(t => t.EmployeeId).ToList();
            if (!employeeOfTask.Contains(subTask.EmployeeId))
            {
                throw new Exception("EmployeeId không nằm trong danh sách của task");
            }

            await _unitOfWork.RepositoryEmployee_Task.Add(subTaskNew);
            await _unitOfWork.RepositoryEmployee_Task.Commit();

            //var toPhoneNumbers = new List<string>
            //{
            //    "+84394044324",
            //    "+84777767130"
            //};

            //foreach (var toPhoneNumber in toPhoneNumbers)
            //{
            //    try
            //    {
            //        var twilioAccountSid = _configuration["Twilio:AccountSid"];
            //        var twilioAuthToken = _configuration["Twilio:AuthToken"];
            //        var twilioPhoneNumber = _configuration["Twilio:PhoneNumber"];

            //        TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            //        var messageBody = $"Công việc mới đã được tạo: {subTask.Name}";

            //        var from = new PhoneNumber(twilioPhoneNumber);
            //        var to = new PhoneNumber(toPhoneNumber);

            //        var message = MessageResource.Create(
            //            body: messageBody,
            //            from: from,
            //            to: to
            //        );

            //        Console.WriteLine($"Message sent with SID: {message.Sid}");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error sending message to {toPhoneNumber}: {ex.Message}");
            //    }
            //}
        }



        private string GenerateTaskCode()
        {
            Guid uniqueId = Guid.NewGuid();

            string uniquePart = uniqueId.ToString("N").Substring(0, 8);

            return "CN" + uniquePart;
        }
        public async Task DeleteSubTasks(int subtaskId)
        {
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);
            if (subtask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ con ");
            }
            _unitOfWork.RepositoryEmployee_Task.Delete(s => s.SubtaskId == subtaskId);

            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }


        public async Task<IEnumerable<EmployeeListModel>> GetEmployeesNoSubtask(int taskId)
        {
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.Status == false && s.TaskId == taskId);
            var employeeIds = employee_task.Select(s => s.EmployeeId).ToList();
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));
            if (employees == null)
            {
                throw new Exception("không tìm thấy nhân viên");
            }

            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }

        public async Task UpdateEffortTime(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes)
        {
            var existingTask = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            var subtaskOfTask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == taskId && s.Status == true);
            if (subtaskOfTask.Count() > 0)
            {
                throw new Exception("Không cập nhật được chấm công bởi vì nhiệm vụ đó có nhiệm vụ con");
            }
            if (existingTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            foreach (var employeeEffortTime in employeeEffortTimes)
            {
                var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == taskId && s.EmployeeId == employeeEffortTime.EmployeeId);

                if (existingSubTask == null)
                {
                    throw new Exception($"Không tìm thấy subtask cho employee có ID {employeeEffortTime.EmployeeId}");
                }

                existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
                existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;
                existingSubTask.DaySubmit = employeeEffortTime.DaySubmit;
                existingSubTask.Name = employeeEffortTime.Name;
                existingSubTask.Description = employeeEffortTime.Description;

                _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
                await _unitOfWork.RepositoryEmployee_Task.Commit();
            }
        }

        public async Task UpdateEffortOfSubtask(int subtaskId, EmployeeEffortUpdate employeeEffortTime)
        {
            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);
            if (existingSubTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.Id == existingSubTask.TaskId);
            if (task.Status == 0 || task.Status == 4 || task.Status == 5)
            {
                throw new Exception("Chỉ những nhiệm vụ đang thực hiện, hoàn thành và không hoàn thành mới được chấm công");
            }

            existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
            existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;
            //existingSubTask.TaskId = existingSubTask.TaskId;

            _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }


        public async Task UpdateEffortTimeAndStatusTask(int taskId, List<EmployeeEffortUpdateAndChangeStatus> employeeEffortTimes)
        {
            try
            {

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var existingTask = await _unitOfWork.RepositoryFarmTask.GetById(taskId);

                    if (existingTask == null)
                    {
                        throw new Exception("Không tìm thấy nhiệm vụ");
                    }
                    var subtaskOfTask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == taskId && s.Status == true);
                    if (subtaskOfTask.Count() > 0)
                    {
                        existingTask.Status = 4;
                        await _unitOfWork.RepositoryFarmTask.Commit();
                    }
                    else
                    {
                        existingTask.Status = 4;
                        await _unitOfWork.RepositoryFarmTask.Commit();

                        foreach (var employeeEffortTime in employeeEffortTimes)
                        {
                            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == taskId && s.EmployeeId == employeeEffortTime.EmployeeId && s.Status == false);

                            if (existingSubTask == null)
                            {
                                throw new Exception($"Không tìm thấy subtask cho employee có ID {employeeEffortTime.EmployeeId}");
                            }
                            var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeEffortTime.EmployeeId);

                            var efffortOfEmployeeInSubmitDay = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.EmployeeId == employeeEffortTime.EmployeeId && s.DaySubmit == employeeEffortTime.DaySubmit);
                            var tottalEffortHour = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEffortHour);
                            var tottalEffortHourAfter = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEffortHour) + employeeEffortTime.ActualEffortHour;

                            var tottalEffortMinutes = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEfforMinutes);
                            var tottalEffortMinutesAfter = efffortOfEmployeeInSubmitDay.Sum(s => s.ActualEfforMinutes) + employeeEffortTime.ActualEfforMinutes;

                            if (tottalEffortMinutesAfter >= 60)
                            {
                                tottalEffortHourAfter += tottalEffortMinutesAfter / 60;

                                tottalEffortMinutesAfter %= 60;
                            }
                            if (tottalEffortMinutes >= 60) 
                            {
                                tottalEffortHour += tottalEffortMinutes / 60;

                                tottalEffortMinutes %= 60;
                            }

                            if (tottalEffortHourAfter >= 24)
                                throw new Exception($"Nhân viên {employee.Name} trong ngày {employeeEffortTime.DaySubmit.ToString("dd/MM/yyyy")} đã làm {tottalEffortHour} giờ {tottalEffortMinutes} phút");

                            existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
                            existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;
                            existingSubTask.DaySubmit = employeeEffortTime.DaySubmit;

                            _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
                            await _unitOfWork.RepositoryEmployee_Task.Commit();
                        }
                    }

                    string message = $"Công việc '{existingTask.Name}' đã hoàn thành";
                    if (existingTask.ManagerId.HasValue)
                    {
                        var managerIds = await GetManagerId();

                        var memberIds = new List<int>(managerIds);

                        var managerTokens = await GetTokenAllManger();

                        await SendNotificationToDeviceAndMembers(managerTokens, message, memberIds, existingTask.Id);
                    }


                    _unitOfWork.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int>> GetManagerId()
        {
            var manager = await _unitOfWork.RepositoryMember.GetData(m => m.RoleId == 1);
            var managerId = manager.Select(m => m.Id).ToList();
            return managerId;
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
                    { "taskId", taskId.ToString() }
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
        public async Task SendNotificationToDevices(List<string> deviceTokens, Message message)
        {
            foreach (var deviceToken in deviceTokens)
            {
                message.Token = deviceToken;

                var messaging = FirebaseMessaging.DefaultInstance;
                await messaging.SendAsync(message);
            }
        }
        public async Task<GetEffortByTaskModel> GetEffortByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId, includes: includes);

            bool checkSubtask = false;
            var nonSubtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == taskId && s.Status == true);
            if (nonSubtask.Count() > 0)
            {
                checkSubtask = true;
            }
            var groupedSubtasks = subtasks.GroupBy(s => new { s.TaskId, s.EmployeeId });

            var subtaskEffortModels = groupedSubtasks.Select(group =>
            {
                var subtaskModel = _mapper.Map<Employee_Task, SubtaskEffortModel>(group.First());
                var effortMinutes = group.Sum(s => s.ActualEfforMinutes);
                var effortHours = group.Sum(s => s.ActualEffortHour);

                subtaskModel.TotalActualEfforMinutes = effortMinutes;
                subtaskModel.TotalActualEffortHour = effortHours;

                if (subtaskModel.TotalActualEfforMinutes >= 60)
                {
                    int additionalHours = subtaskModel.TotalActualEfforMinutes / 60;

                    subtaskModel.TotalActualEffortHour += additionalHours;

                    subtaskModel.TotalActualEfforMinutes %= 60;
                }
                return subtaskModel;
            });

            return new GetEffortByTaskModel
            {
                Subtasks = subtaskEffortModels,
                IsHaveSubtask = checkSubtask
            };
        }

        public async Task<TotalEffortModel> GetTotalEffortEmployee(int id, DateTime? startDay, DateTime? endDay)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }

            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.EmployeeId == id &&
                                                    (!startDay.HasValue || !endDay.HasValue || (
                                                               (startDay.Value.Year <= s.DaySubmit.Value.Year &&
                                                                endDay.Value.Year >= s.DaySubmit.Value.Year) &&
                                                               (startDay.Value.Month <= s.DaySubmit.Value.Month &&
                                                                endDay.Value.Month >= s.DaySubmit.Value.Month) &&
                                                               (startDay.Value.Day <= s.DaySubmit.Value.Day &&
                                                                endDay.Value.Day >= s.DaySubmit.Value.Day)
                                                            )));

            var taskIds = subtasks.Select(s => s.TaskId);

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) && (t.Status == 8 || t.Status == 7));

            var totalTask = tasks.Count();

            var taskEffortIds = tasks.Select(t => t.Id);

            var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetData(s => taskEffortIds.Contains(s.TaskId) && s.EmployeeId == id);

            var effortMinutes = subtaskEffort.Sum(s => s.ActualEfforMinutes);
            var effortHours = subtaskEffort.Sum(s => s.ActualEffortHour);

            var totalEffortEmployee = new TotalEffortModel
            {
                EmployeeId = id,
                EmployeeName = employee.Name,
                EmployeeCode = employee.Code,
                TotalTask = totalTask,
                ActualEfforMinutes = effortMinutes,
                ActualEffortHour = effortHours
            };

            return totalEffortEmployee;
        }



    }
}