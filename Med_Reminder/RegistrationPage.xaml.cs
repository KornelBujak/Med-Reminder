namespace Med_Reminder;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using Med_Reminder;
using System;
using System.Text;
public partial class RegistrationPage : ContentPage
{

    private MyAppDbContext dbContext;
    public RegistrationPage()
	{
		InitializeComponent();
        dbContext = new MyAppDbContext();
    }

    private void RegisterButton_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string firstName = FirstNameEntry.Text;
        string lastName = LastNameEntry.Text;
        string gender = GenderPicker.SelectedItem?.ToString();
        DateTime dateOfBirth = DatePicker.Date;
        double weight = Convert.ToDouble(WeightEntry.Text);
        string phoneNumber = PhoneNumberEntry.Text;
       

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
       string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
       gender == null)
        {
            DisplayAlert("B³¹d", "Proszê wype³niæ wszystkie wymagane pola.", "OK");
            return;
        }

        if (!email.Contains("@"))
        {
            DisplayAlert("B³¹d", "Nieprawid³owy adres e-mail.", "OK");
            return;
        }

        if (!double.TryParse(WeightEntry.Text, out weight))
        {
            DisplayAlert("B³¹d", "Nieprawid³owa wartoœæ wagi.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            DisplayAlert("B³¹d", "Proszê podaæ numer telefonu.", "OK");
            return;
        }

        var newUser = new DaneOsobowe()
        {
            AdresEmail = email,
            HasloSzyfrowane = Encoding.UTF8.GetBytes(password),
            Imie = firstName,
            Nazwisko = lastName,
            Plec = gender,
            DataUrodzenia = dateOfBirth,
            Waga = weight,
            _numer_telefonu_ = phoneNumber
        };

        dbContext._dane_osobowe.Add(newUser);
        dbContext.SaveChanges();

        DisplayAlert("Rejestracja", "Rejestracja zakoñczona sukcesem!", "OK");
        Navigation.PushAsync(new LoginPage());
    }
}