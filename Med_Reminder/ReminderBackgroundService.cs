using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class ReminderBackgroundService:BackgroundService
    {
        private readonly SendMessage sendMessage;
        private readonly MyAppDbContext dbContext;
        private readonly Twilio _twilio;

        public ReminderBackgroundService(SendMessage sendMessage, MyAppDbContext dbContext)
        {
            this.sendMessage = sendMessage;
            this.dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.Now.TimeOfDay;

                if (IsReminderTime(currentTime))
                {
                    await CheckAndSendRemindersAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private bool IsReminderTime(TimeSpan currentTime)
        {
            var reminders = dbContext._przypomnienia
                .Where(r => r.godzina_przypomnienia == currentTime)
                .ToList();

            return reminders.Any();
        }

        private async Task CheckAndSendRemindersAsync(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;

            var reminders = dbContext._przypomnienia
                .Where(r => r.data_początkowa <= now && r.godzina_przypomnienia <= now.TimeOfDay &&
                            !r._czywyslano &&
                            (r.data_końcowa == null || r.data_końcowa >= now))
                .ToList();

            foreach (var reminder in reminders)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                var medication = dbContext._leki.FirstOrDefault(l => l.id_leku == reminder.id_leku);
                if (medication != null)
                {
                    string message = $"Proszę zażyć lek {medication.NazwaLeku}.";
                    string userPhoneNumber = dbContext.GetUserPhoneNumber(reminder.dane_osobowe_id);

                    if (!string.IsNullOrEmpty(userPhoneNumber))
                    {
                        if (!userPhoneNumber.StartsWith("+48"))
                        {
                            userPhoneNumber = "+48" + userPhoneNumber;
                        }

                        _twilio.SendSms(userPhoneNumber, message, reminder.Id);

                        reminder._czywyslano = true;
                        dbContext._przypomnienia.Update(reminder);
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
        }
    }
}


