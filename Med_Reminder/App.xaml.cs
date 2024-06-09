using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace Med_Reminder
{
    public partial class App : Application
    {
        
        private readonly Timer _timer;
        private IHost _host;
        private ReminderBackgroundService _reminderBackgroundService;
        private static Twilio _twilioInstance;
        public App()
        {
            InitializeComponent();

            string accountSid = "AC198ff1c4cef76cd2eab39987c2e93260";
            string authToken = "7894b05429ded166b5bac3fcd3b602ce";
            string fromPhoneNumber = "+12232676211";

            _twilioInstance = Twilio.GetInstance(accountSid, authToken, fromPhoneNumber);

            MainPage = new AppShell();
            MainPage = new NavigationPage(new LoginPage());


            _host = CreateHostBuilder().Build();
            _host.Start();


        }

        protected override void OnStart()
        {
            base.OnStart();
            string accountSid = GetAccountSidFromConfiguration();
            string authToken = GetAuthTokenFromConfiguration();
            string fromPhoneNumber = GetFromPhoneNumberFromConfiguration();
            Twilio.GetInstance(accountSid, authToken, fromPhoneNumber);
            _host.Start(); 
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            _host.StopAsync().Wait();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _host.Start();
        }

        public static Twilio GetTwilioInstance()
        {
            return _twilioInstance;
        }



        private IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                string accountSid = GetAccountSidFromConfiguration();
                string authToken = GetAuthTokenFromConfiguration();
                services.AddDbContext<MyAppDbContext>();
                string fromPhoneNumber = GetFromPhoneNumberFromConfiguration();
                services.AddSingleton<Twilio>(provider => Twilio.GetInstance(accountSid, authToken, fromPhoneNumber));

                services.AddTransient<SendMessage>();
                services.AddHostedService<ReminderBackgroundService>();

                services.AddHostedService(provider =>
                    new ReminderBackgroundService(
                        provider.GetRequiredService<SendMessage>(),
                        provider.GetRequiredService<MyAppDbContext>()
                    )
                );
            });

        private string GetAccountSidFromConfiguration()
        {
            return "AC198ff1c4cef76cd2eab39987c2e93260";
        }

        private string GetAuthTokenFromConfiguration()
        {
            return "7894b05429ded166b5bac3fcd3b602ce";
        }

        private string GetFromPhoneNumberFromConfiguration()
        {
            return "+12232676211";
        }
    

        public static int CurrentUserId { get; set; }
        public static bool IsUserLoggedIn { get; set; }
    }

}
   

    
