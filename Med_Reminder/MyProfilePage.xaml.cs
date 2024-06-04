using System;
using System.Text;


namespace Med_Reminder
{
    public partial class MyProfilePage : ContentPage
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public MyProfilePage()
        {
            InitializeComponent();
            _userProfileRepository = new UserProfileRepository(new MyAppDbContext());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (App.IsUserLoggedIn)
            {
                await LoadUserData();
            }
            else
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        private async Task LoadUserData()
        {
            int userId = App.CurrentUserId;

            DaneOsobowe daneOsobowe = await _userProfileRepository.GetDaneOsoboweAsync(userId);

            if (daneOsobowe != null)
            {
                ImieLabel.Text = "Imię: " + daneOsobowe.Imie;
                NazwiskoLabel.Text = "Nazwisko: " + daneOsobowe.Nazwisko;
                EmailLabel.Text = "Email: " + daneOsobowe.AdresEmail;
                PlecLabel.Text = "Płeć: " + daneOsobowe.Plec;
                WiekLabel.Text = "Data urodzenia: " + daneOsobowe.DataUrodzenia.ToString("yyyy-MM-dd");
                WagaLabel.Text = "Waga: " + daneOsobowe.Waga.ToString() + " kg";
                TelefonLabel.Text = "Numer telefonu: " + daneOsobowe._numer_telefonu_;
            }
        }

        private async void SaveChangesButton_Clicked(object sender, EventArgs e)
        {
            string newName = NewNameEntry.Text;
            string newEmail = NewEmailEntry.Text;
            string newPassword = NewPasswordEntry.Text;
            double newWeight = Convert.ToDouble(NewWeightEntry.Text);
            string newPhoneNumber = NewPhoneNumberEntry.Text;

            DaneOsobowe currentUser = await _userProfileRepository.GetDaneOsoboweAsync(App.CurrentUserId);

            if (!string.IsNullOrEmpty(newName))
                currentUser.Nazwisko = newName;

            if (!string.IsNullOrEmpty(newEmail))
                currentUser.AdresEmail = newEmail;

            if (!string.IsNullOrEmpty(newPassword))
                currentUser.HasloSzyfrowane = Encoding.UTF8.GetBytes(newPassword);

            if (!string.IsNullOrEmpty(NewWeightEntry.Text) && double.TryParse(NewWeightEntry.Text, out double newWeightValue))
            {
                currentUser.Waga = newWeightValue;
            }
            if (!string.IsNullOrEmpty(newPhoneNumber))
                currentUser._numer_telefonu_ = newPhoneNumber;

            _userProfileRepository.UpdateDaneOsobowe(currentUser);
            await _userProfileRepository.SaveChanges();

            NewNameEntry.Text = string.Empty;
            NewEmailEntry.Text = string.Empty;
            NewPasswordEntry.Text = string.Empty;
            NewWeightEntry.Text = string.Empty;
            NewPhoneNumberEntry.Text= string.Empty;
            await DisplayAlert("Zapisano zmiany", "Twoje dane zostały zaktualizowane.", "OK");

            await LoadUserData();
        }
    }
}
