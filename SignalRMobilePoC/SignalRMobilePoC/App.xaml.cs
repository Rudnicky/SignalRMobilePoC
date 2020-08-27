using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRMobilePoC.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SignalRMobilePoC
{
    public partial class App : Application
    {
        private HubConnection _hubConnection;
        
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            
            // Regular SignalR expression
            // var hubConnection = new HubConnectionBuilder()
            //     .WithUrl("https://10.0.2.2:5001/hubService")
            //     .Build();
            
            // SignalR expression that ignores SSL (when testing locally)
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://10.0.2.2:5001/hub", options => 
                    {
                        options.HttpMessageHandlerFactory = handler => GetInsecureHandler();
                    }
                )
                .Build();
            
            await _hubConnection.StartAsync();
            
            _hubConnection.On(Messages.LogoutMessage, () =>
            {
                var msg = "";
            });
            
            #region Simple Http request test
            //////////////////////////////////////////////////
            /// REGULAR HTTP CALL OF LOCALLY HOSTED SERVER ///
            //////////////////////////////////////////////////
            // Bypass the certificate security check
            //var insecureHandler = GetInsecureHandler();
            //var httpClient = new HttpClient(insecureHandler);
            
            // Applications running in the Android emulator can connect to local HTTP web services via the 10.0.2.2 address,
            // which is an alias to your host loopback interface (127.0.0.1 on your development machine).
            // For example, given a local HTTP web service that exposes a GET operation via the /api/todoitems/ relative URI,
            // an application running in the Android emulator can consume the operation by sending a GET request to http://10.0.2.2:<port>/api/todoitems/.
            //var response = await httpClient.GetAsync("https://10.0.2.2:5001/weatherforecast");
            #endregion
        }
        
        // Method that helps us to ignore ssl locally
        private HttpClientHandler GetInsecureHandler()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                }
            };
            return handler;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}