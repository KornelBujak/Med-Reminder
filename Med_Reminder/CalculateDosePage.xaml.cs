namespace Med_Reminder
{
    public partial class CalculateDosePage : ContentPage
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private DaneOsobowe _currentUser;

        public CalculateDosePage()
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
            _currentUser = await _userProfileRepository.GetDaneOsoboweAsync(userId);

            if (_currentUser != null)
            {
                WagaLabel.Text = "Waga: " + _currentUser.Waga.ToString();
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ danych u¿ytkownika.", "OK");
            }
        }

        private void CalculateDoseButton_Clicked(object sender, EventArgs e)
        {
            if (_currentUser.Wiek < 18)
            {
                DisplayAlert("B³¹d", "Opcja dawkowania leku nie jest dostêpna dla u¿ytkownikow niepe³noletnich", "OK");
                return;
            }
            else
            {
                double weight = double.Parse(WagaLabel.Text.Replace("Waga: ", ""));
                double dosage;
                double concentration;

                if (IsLiquidMedicineSwitch.IsToggled)
                {
                    if (string.IsNullOrWhiteSpace(ConcentrationEntry.Text))
                    {
                        DisplayAlert("B³¹d", "Proszê podaæ wartoœæ stê¿enia.", "OK");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(DosageEntry.Text))
                    {
                        DisplayAlert("B³¹d", "Proszê podaæ wartoœæ dawkowania.", "OK");
                        return;
                    }

                    dosage = double.Parse(DosageEntry.Text);
                    concentration = double.Parse(ConcentrationEntry.Text);

                    double dose = weight * dosage;

                    double liquidDose = Math.Round(dose / concentration, 1);
                    DisplayAlert("Dawka leku", $"Dawka leku p³ynnego: {liquidDose} ml", "OK");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DosageEntry.Text))
                    {
                        DisplayAlert("B³¹d", "Proszê podaæ wartoœæ dawkowania.", "OK");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(ConcentrationEntry.Text))
                    {
                        concentration = 000;
                    }
                    else
                    {
                        DisplayAlert("B³¹d", "Wartoœæ stê¿enia nie jest potrzebna!", "OK");
                        return;
                    }

                    dosage = double.Parse(DosageEntry.Text);

                    double dose = weight * dosage;

                    DisplayAlert("Dawka leku", $"Dawka leku: {dose} mg", "OK");
                }
            }
        }


    }
}
