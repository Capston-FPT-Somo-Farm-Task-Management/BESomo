using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Interface;
using SomoTaskManagement.Services.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Notify.HubSignalR
{
    public class TaskHub :Hub
    {
        private readonly SomoTaskManagemnetContext dbContext;
        private readonly TaskRepository taskRepository;

        public TaskHub(IConfiguration configuration, SomoTaskManagemnetContext _dbContext)
        {
            var connectionString = configuration.GetConnectionString("MyDB");
            dbContext = _dbContext;
            taskRepository = new TaskRepository(connectionString, dbContext);
        }

        public async Task SendTasks()
        {
            var tasks = taskRepository.GetTasks();
            await Clients.All.SendAsync("ReceivedTask", tasks);
        }

        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceivedNotification", message);
        }

        public async Task SendNotificationToClient(string message, int memberId)
        {
            var hubConnections = dbContext.HubConnection.Where(con => con.MemberId == memberId).ToList();
            foreach (var hubConnection in hubConnections)
            {
                await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, memberId);
            }
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected");
            return base.OnConnectedAsync();
        }

        public async Task SaveUserConnection(int memberId)
        {
            var connectionId = Context.ConnectionId;
            HubConnection hubConnection = new HubConnection
            {
                ConnectionId = connectionId,
                MemberId = memberId
            };

            dbContext.HubConnection.Add(hubConnection);
            await dbContext.SaveChangesAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var hubConnection = dbContext.HubConnection.FirstOrDefault(con => con.ConnectionId == Context.ConnectionId);
            if (hubConnection != null)
            {
                dbContext.HubConnection.Remove(hubConnection);
                dbContext.SaveChangesAsync();
            }

            return base.OnDisconnectedAsync(exception);
        }

    }
}
