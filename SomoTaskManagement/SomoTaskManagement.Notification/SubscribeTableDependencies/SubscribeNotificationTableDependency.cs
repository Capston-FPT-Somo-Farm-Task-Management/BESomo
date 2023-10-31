using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Notify.HubSignalR;
using TableDependency.SqlClient;

namespace SomoTaskManagement.Notify.SubscribeTableDependencies
{
    public class SubscribeNotificationTableDependency : ISubscribeTableDependency
    {

        SqlTableDependency<Notification> tableDependency;
        NotifyHub taskHub;
        SomoTaskManagemnetContext context;

        public SubscribeNotificationTableDependency(NotifyHub taskhub, SomoTaskManagemnetContext context)
        {
            this.taskHub = taskhub;
            this.context = context;
        }

        public void SubscribeTableDependency(string connectString)
        {
            tableDependency = new SqlTableDependency<Notification>(connectString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Notification> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await taskHub.SendNotifys(); 
            }
        }
        
        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Notification)} SqlTableDependency error: {e.Error.Message}");
        }


    }
}
