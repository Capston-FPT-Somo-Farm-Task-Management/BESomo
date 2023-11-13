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
            int taskId = int.Parse(context.JobDetail.Description);

            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(f => f.Id == taskId);
            if (task != null)
            {
                task.Status = 2;
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
        }
    }
}
