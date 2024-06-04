using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Exceptions;

namespace Med_Reminder
{
    public class Twilio
    {
        private static Twilio instance;
        private readonly string accountSid;
        private readonly string authToken;
        private readonly string fromPhoneNumber;

        public Twilio(string accountSid, string authToken, string fromPhoneNumber)
        {
            this.accountSid = accountSid;
            this.authToken = authToken;
            this.fromPhoneNumber = fromPhoneNumber;
            TwilioClient.Init(accountSid, authToken);
        }

        public static Twilio GetInstance(string accountSid, string authToken, string fromPhoneNumber)
        {
            if (instance == null)
            {
                Console.WriteLine("GetInstance is called.");
                instance = new Twilio(accountSid, authToken, fromPhoneNumber);
            }
            return instance;
        }

        public void SendSms(string toPhoneNumber, string message,int przypomnienieId)
        {
            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
                {
                    From = new PhoneNumber(fromPhoneNumber), 
                    Body = message
                };

                MessageResource.Create(messageOptions);
                Console.WriteLine("Message sent successfully.");

                using (var dbContext = new MyAppDbContext())
                {
                    var przypomnienie = dbContext._przypomnienia.FirstOrDefault(p => p.Id == przypomnienieId);
                    if (przypomnienie != null)
                    {
                        przypomnienie._czywyslano = true;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Twilio API error: {ex.Message}");
            }
            catch (ApiConnectionException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
        }
    }
}


