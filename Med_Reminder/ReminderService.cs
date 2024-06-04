using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class ReminderService
    {
        private readonly SetReminderPage _setReminderPage;
        private Timer _timer;

        public ReminderService(SetReminderPage setReminderPage)
        {
            _setReminderPage = setReminderPage;
        }

        public void Start(CancellationToken stoppingToken)
        {
            _timer = new Timer(async (e) =>
            {
                await _setReminderPage.CheckAndSendRemindersAsync(stoppingToken);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }
    }
}

