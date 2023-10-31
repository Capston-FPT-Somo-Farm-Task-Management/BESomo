using Google;
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Repo
{
    public class TaskRepository 
    {
        private readonly SomoTaskManagemnetContext dbContext;
        private readonly string connectionString;

        public TaskRepository(string connectionString,SomoTaskManagemnetContext _dbContext)
        {
            this.dbContext = _dbContext;
            this.connectionString = connectionString;
        }
       

        public List<Notification> GetNotification()
        {
            var prodList = dbContext.Notification.ToList();
            foreach (var emp in prodList)
            {
                dbContext.Entry(emp).Reload();
            }
            return prodList;
        }
    }
}
