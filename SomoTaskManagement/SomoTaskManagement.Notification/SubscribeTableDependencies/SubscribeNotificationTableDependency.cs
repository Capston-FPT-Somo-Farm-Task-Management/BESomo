
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Notify.HubSignalR;
using TableDependency.SqlClient;

namespace SomoTaskManagement.Notify.SubscribeTableDependencies
{
    public class SubscribeNotificationTableDependency : ISubscribeTableDependency
    {

        SqlTableDependency<FarmTask> tableDependency;
        TaskHub taskHub;
        SomoTaskManagemnetContext context;

        public SubscribeNotificationTableDependency(TaskHub taskhub, SomoTaskManagemnetContext context)
        {
            this.taskHub = taskhub;
            this.context = context;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<FarmTask>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<FarmTask> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await taskHub.SendTasks();

                var farmTask = e.Entity;
                var notification = new Notification
                {
                    Message = "There is a change in your task",
                    NotificationDateTime = DateTime.Now,
                    MessageType = "Personal"
                };
                context.Notification.Add(notification);
                context.SaveChanges();

                var message = notification.Message;
                await taskHub.SendNotificationToClient(message, farmTask.MemberId);
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(FarmTask)} SqlTableDependency error: {e.Error.Message}");
        }


    }
}
