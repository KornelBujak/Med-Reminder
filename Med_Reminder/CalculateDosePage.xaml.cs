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
                await DisplayAlert("B��d", "Nie uda�o si� pobra� danych u�ytkownika.", "OK");
            }
        }

        private void CalculateDoseButton_Clicked(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - _currentUser.DataUrodzenia.Year;
            if (currentDate < _currentUser.DataUrodzenia.AddYears(age))
            {
                age--;
            }

            if (age < 18)
            {
                DisplayAlert("B��d", "Opcja dawkowania leku nie jest dost�pna dla u�ytkownik�w niepe�noletnich", "OK");
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
                        DisplayAlert("B��d", "Prosz� poda� warto�� st�enia.", "OK");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(DosageEntry.Text))
                    {
                        DisplayAlert("B��d", "Prosz� poda� warto�� dawkowania.", "OK");
                        return;
                    }

                    dosage = double.Parse(DosageEntry.Text);
                    concentration = double.Parse(ConcentrationEntry.Text);

                    double dose = weight * dosage;

                    double liquidDose = Math.Round(dose / concentration, 1);
                    DisplayAlert("Dawka leku", $"Dawka leku p�ynnego: {liquidDose} ml", "OK");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DosageEntry.Text))
                    {
                        DisplayAlert("B��d", "Prosz� poda� warto�� dawkowania.", "OK");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(ConcentrationEntry.Text))
                    {
                        concentration = 000;
                    }
                    else
                    {
                        DisplayAlert("B��d", "Warto�� st�enia nie jest potrzebna!", "OK");
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
