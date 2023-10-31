using Google;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using SomoTaskManagement.Services.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Notify.HubSignalR
{
    public class NotifyHub :Hub
    {
        private readonly ITaskRepository _taskRepository;
        private readonly SomoTaskManagemnetContext dbContext;
        private readonly TaskRepository taskRepository;

        public NotifyHub(IConfiguration configuration, SomoTaskManagemnetContext _dbContext)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            dbContext = _dbContext;
            taskRepository = new TaskRepository(connectionString, dbContext);
        }


        public async Task SendNotifys()
        {
            var products = taskRepository.GetNotification();
            await Clients.All.SendAsync("ReceivedProducts", products);
        }
    }
}
