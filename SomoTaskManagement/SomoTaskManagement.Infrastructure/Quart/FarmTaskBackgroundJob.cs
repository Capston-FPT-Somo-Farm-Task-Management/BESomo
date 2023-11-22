using Quartz;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Infrastructure.Quart
{
    public class FarmTaskBackgroundJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public FarmTaskBackgroundJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.IsExpired == false && t.EndDate <= currentDate);
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    task.IsExpired = true;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();
                }
            }

        }



    }
}
