using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _twilioPhoneNumber;

    public TwilioService(string accountSid, string authToken, string twilioPhoneNumber)
    {
        _accountSid = accountSid;
        _authToken = authToken;
        _twilioPhoneNumber = twilioPhoneNumber;

        // Khởi tạo TwilioClient ở đây để tránh khởi tạo nhiều lần
        TwilioClient.Init(_accountSid, _authToken);
    }

    public void SendSms(string toPhoneNumber, string messageBody)
    {
        try
        {
            var from = new PhoneNumber(_twilioPhoneNumber);
            var to = new PhoneNumber(toPhoneNumber);

            var message = MessageResource.Create(
                body: messageBody,
                from: from,
                to: to
            );

            Console.WriteLine($"Message sent with SID: {message.Sid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }
}
