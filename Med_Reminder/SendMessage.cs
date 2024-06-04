using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class SendMessage
    {
        private readonly MyAppDbContext dbContext;
        private readonly Twilio _twilio;

        public SendMessage(MyAppDbContext dbContext, Twilio twilio)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "DbContext cannot be null");
            }

            if (twilio == null)
            {
                throw new ArgumentNullException(nameof(twilio), "Twilio instance cannot be null");
            }

            this.dbContext = dbContext;
            _twilio = twilio;
        }

        public async Task Send(string toPhoneNumber, string message, int przypomnienieId)
        {
             _twilio.SendSms(toPhoneNumber, message, przypomnienieId);
        }

        public async Task CheckRemindersAsync()
        {
            var now = DateTime.Now;
            var reminders = dbContext._przypomnienia
                .Where(r => r.data_początkowa <= now && r.godzina_przypomnienia <= now.TimeOfDay &&
                    (r.data_końcowa == null || r.data_końcowa >= now))
                .ToList();

            foreach (var reminder in reminders)
            {
                var medication = dbContext._leki.FirstOrDefault(l => l.id_leku == reminder.id_leku);
                if (medication != null)
                {

                    string medicationName = medication.NazwaLeku;
                    string body = $"Przypomnienie o zażyciu leku {medicationName}.";


                    try
                    {
                        var userPhoneNumber = GetUserPhoneNumber(reminder.dane_osobowe_id);
                        if (userPhoneNumber != null)
                        {
                            _twilio.SendSms(userPhoneNumber, body, reminder.Id);

                        }
                        else
                        {
                            Console.WriteLine("User phone number is null.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while sending SMS: {ex.Message}");
                    }
                }
            }
        }
        public string GetUserPhoneNumber(int userId)
        {
            var user = dbContext._dane_osobowe.FirstOrDefault(u => u.Id == userId);
            if (user != null && !string.IsNullOrEmpty(user._numer_telefonu_))
            {
                return user._numer_telefonu_;
            }
            else
            {
                return null;
            }
        }
    }

    
}
