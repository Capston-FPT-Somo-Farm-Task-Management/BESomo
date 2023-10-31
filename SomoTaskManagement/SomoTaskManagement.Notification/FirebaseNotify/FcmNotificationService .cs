using FirebaseAdmin.Messaging;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency.SqlClient.Base.Messages;

namespace SomoTaskManagement.Notify.FirebaseNotify
{
    public class FcmNotificationService
    {
        private FirebaseMessaging _fcm;

        public FcmNotificationService()
        {
            _fcm = FirebaseMessaging.DefaultInstance;
        }

        public async Task SendNotification(string token, string title, string body)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            await _fcm.SendAsync(message);
        }
    }
}
