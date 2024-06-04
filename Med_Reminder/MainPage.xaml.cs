namespace Med_Reminder
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void MyRemindersButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyRemindersPage());
        }
        private async void MyMedicationsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyMedicationsPage());
        }

        private async void SetReminderButton_Clicked(object sender, EventArgs e)
        {
            var twilio = DependencyService.Resolve<Twilio>();
            await Navigation.PushAsync(new SetReminderPage());
        }

        private async void CalculateDoseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalculateDosePage());
        }

        private async void MyProfileButton_Clicked(object sender, EventArgs e)
        {
            var userProfileRepository = DependencyService.Get<IUserProfileRepository>();
            await Navigation.PushAsync(new MyProfilePage());
        }
        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
            await Navigation.PushAsync(new LoginPage());
        }
    }
}