using Microsoft.Extensions.Configuration;
using Quartz;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SomoTaskManagement.Infrastructure.Quart
{
    public class SubTaskBackGroundJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public SubTaskBackGroundJob(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                var employees = await _unitOfWork.RepositoryEmployee.GetData(e => e.Status == 1);

                foreach (var employee in employees)
                {
                    var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.EmployeeId == employee.Id && s.DaySubmit.HasValue && s.DaySubmit.Value.Date == currentDay.Date);

                    var taskIds = subtasks.Select(s => s.TaskId).ToList();
                    var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) && t.Status == 8);

                    foreach (var subTask in subtasks)
                    {
                        var task = tasks.FirstOrDefault(t => t.Id == subTask.TaskId);

                        if (task != null)
                        {
                            var twilioAccountSid = _configuration["Twilio:AccountSid"];
                            var twilioAuthToken = _configuration["Twilio:AuthToken"];
                            var twilioPhoneNumber = _configuration["Twilio:PhoneNumber"];

                            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

                            foreach (var phoneNumber in employee.PhoneNumber)
                            {
                                var toPhoneNumber = phoneNumber.ToString();
                                var messageBody = $"Công việc mới đã được tạo: {subTask.Name}";

                                var from = new PhoneNumber(twilioPhoneNumber);
                                var to = new PhoneNumber(toPhoneNumber);

                                var message = MessageResource.Create(
                                    body: messageBody,
                                    from: from,
                                    to: to
                                );

                                Console.WriteLine($"Message sent with SID: {message.Sid}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in background job: {ex.Message}");
            }
        }


    }
}
