using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TweakAppClient
{
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var hardwareId = HardwareInfo.GetMotherboardId();
            var loginData = new
            {
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password,
                HardwareId = hardwareId
            };

            using (var client = new HttpClient())
            {
                var json = System.Text.Json.JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{Configuration.ApiBaseUrl}/api/accountapi/login", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Login successful!");
                    // Itt nyithatod meg a főablakot
                }
                else
                {
                    MessageBox.Show("Login failed. Please check your credentials and hardware ID.");
                }
            }
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // A regisztrációs URL a webes felületen
            string registerUrl = $"{Configuration.WebBaseUrl}/Identity/Account/Register";

            // A link megnyitása az alapértelmezett böngészőben
            Process.Start(new ProcessStartInfo(registerUrl) { UseShellExecute = true });
        }
    }
}