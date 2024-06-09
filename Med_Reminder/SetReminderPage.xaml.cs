using Microsoft.EntityFrameworkCore;
using System.Globalization;
//using Windows.UI;
//using Xamarin.Forms;
using Autofac;
using System.Data.SqlTypes;

namespace Med_Reminder;

public partial class SetReminderPage : ContentPage
{
    private MyAppDbContext dbContext;
    private readonly Twilio _twilio;
    public TimeSpan CurrentTime { get; set; }

    public SetReminderPage()
    {
        InitializeComponent();
        dbContext = new MyAppDbContext();
        LoadMedications();
        CurrentTime = DateTime.Now.TimeOfDay;
        _twilio = App.GetTwilioInstance();
    }

    private void LoadMedications()
    {
        var medications = dbContext._leki.Where(l => l.DaneOsoboweId == App.CurrentUserId).ToList();
        medicationPicker.ItemsSource = medications;
        medicationPicker.ItemDisplayBinding = new Binding("NazwaLeku");
    }

    private async void SetReminderButton_Clicked(object sender, EventArgs e)
    {
        var selectedMedication = (Lek)medicationPicker.SelectedItem;
        if (selectedMedication == null)
        {
            await DisplayAlert("B³¹d", "Wybierz lek.", "OK");
            return;
        }

        string selectedMedicationName = selectedMedication.NazwaLeku;
        int medicationId = selectedMedication.id_leku;
        DateTime startDate = startDatePicker.Date;
        DateTime endDate = endDatePicker.Date;
        TimeSpan selectedTime = timeReminderPicker.Time;

        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            Przypomnienie reminder = new Przypomnienie()
            {
                dane_osobowe_id = App.CurrentUserId,
                godzina_przypomnienia = selectedTime,
                data_pocz¹tkowa = startDate,
                data_koñcowa = endDate,
                _czywyslano = false,
                nazwa_leku = selectedMedicationName,
                id_leku = medicationId
            };

            dbContext._przypomnienia.Add(reminder);
            await dbContext.SaveChangesAsync();
        }

        await DisplayAlert("Sukces", "Przypomnienie zosta³o ustawione.", "OK");
    }

    public async Task CheckAndSendRemindersAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var reminders = dbContext._przypomnienia
            .Where(r => r.data_pocz¹tkowa <= now && r.godzina_przypomnienia <= now.TimeOfDay &&
                        !r._czywyslano &&
                        (r.data_koñcowa == null || r.data_koñcowa >= now))
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
                string message = $"Przypomnienie o za¿yciu leku {medication.NazwaLeku}.";
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
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    private async Task ScheduleSmsAsync(int reminderId, TimeSpan reminderTime, string userPhoneNumber, string message)
    {
        DateTime now = DateTime.Now;
        DateTime reminderDateTime = DateTime.Today.Add(reminderTime);

        if (reminderDateTime < now)
        {
            reminderDateTime = reminderDateTime.AddDays(1);
        }

        TimeSpan delay = reminderDateTime - now;

        await Task.Delay(delay);

        _twilio.SendSms(userPhoneNumber, message, reminderId);
        await DisplayAlert("Sukces", "Powiadomienie SMS zosta³o wys³ane.", "OK");
    }

    private async void SendSmsButton_Clicked(object sender, EventArgs e)
    {
        TimeSpan selectedTime = timeReminderPicker.Time;
        var selectedMedication = (Lek)medicationPicker.SelectedItem;

        if (selectedMedication != null)
        {
            string selectedMedicationName = selectedMedication.NazwaLeku;
            string message = $"Przypomnienie o za¿yciu leku {selectedMedicationName}.";
            int userId = App.CurrentUserId;
            string userPhoneNumber = dbContext.GetUserPhoneNumber(userId);

            if (!userPhoneNumber.StartsWith("+48"))
            {
                userPhoneNumber = "+48" + userPhoneNumber;
            }

            if (_twilio != null)
            {
                var reminder = dbContext._przypomnienia
                    .Where(r => r.dane_osobowe_id == userId && r.godzina_przypomnienia == selectedTime && r.nazwa_leku == selectedMedicationName)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();

                if (reminder != null)
                {
                    await ScheduleSmsAsync(reminder.Id, selectedTime, userPhoneNumber, message);
                }
                else
                {
                    await DisplayAlert("B³¹d", "Nie znaleziono przypomnienia dla wybranego leku.", "OK");
                }
            }
        }
    }
}

