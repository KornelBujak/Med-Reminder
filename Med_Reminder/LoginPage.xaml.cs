namespace Med_Reminder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using Med_Reminder;
using System;
using System.Text;

public partial class LoginPage : ContentPage
{
    private MyAppDbContext dbContext;
    public LoginPage()
    {
        InitializeComponent();
        dbContext = new MyAppDbContext();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        var user = dbContext._dane_osobowe
                    .AsEnumerable()
                    .FirstOrDefault(u => u.AdresEmail == email && Encoding.UTF8.GetString(u.HasloSzyfrowane) == password);

        if (user != null)
        {
            App.IsUserLoggedIn = true;
            App.CurrentUserId = user.Id;
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("B³¹d logowania", "Nieprawid³owy adres e-mail lub has³o", "OK");
        }
    }

    private void RegisterButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegistrationPage());
    }
}
