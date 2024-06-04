using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
namespace Med_Reminder;
using System.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;


public partial class MyRemindersPage : ContentPage
{
    private readonly MyAppDbContext dbContext;

    public ObservableCollection<string> ReminderItems { get; set; } = new ObservableCollection<string>();

    public MyRemindersPage()
    {
        InitializeComponent();
        dbContext = new MyAppDbContext();
        LoadReminders();
    }

    private async void LoadReminders()
    {
        
            var reminders = await dbContext.GetRemindersForTodayAndFuture(App.CurrentUserId);

            ReminderItems.Clear();


        foreach (var reminder in reminders)
        {
            var medication = await dbContext._leki.FirstOrDefaultAsync(l => l.id_leku == reminder.id_leku);
            if (medication != null)
            {
                string reminderInfo = $"{reminder.data_pocz�tkowa?.ToShortDateString()} {(reminder.godzina_przypomnienia != TimeSpan.Zero ? reminder.godzina_przypomnienia.ToString() : "")} - {medication.NazwaLeku} (ID: {medication.id_leku})";
                ReminderItems.Add(reminderInfo);
            }
        }

        RemindersList.ItemsSource = ReminderItems;
        if (ReminderItems.Count == 0)
        {
            await DisplayAlert("B��d","Nie znaleziono przypmnie� dla danego dnia i przysz�ych","OK");
        }
    }
}
       
    

