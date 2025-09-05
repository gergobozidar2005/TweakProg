using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace TweakAppClient
{
    public partial class RegisterWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Regisztráció folyamatban...";
            RegisterButton.IsEnabled = false;

            var hardwareId = HardwareInfo.GetMotherboardId();

            MessageBox.Show(hardwareId);

            var requestData = new
            {
                Username = UsernameTextBox.Text,
                LicenseKey = LicenseKeyTextBox.Text
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{Configuration.ApiBaseUrl}/api/registration/validate", requestData);

                if (response.IsSuccessStatusCode)
                {
                    StatusTextBlock.Text = "Sikeres regisztráció!";

                    ActivationService.SaveActivationKey(LicenseKeyTextBox.Text);

                    // Létrehozzuk és megnyitjuk a fő ablakot
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Bezárjuk a regisztrációs ablakot
                    this.Close();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    StatusTextBlock.Text = $"Hiba: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Hálózati hiba: {ex.Message}";
            }
            finally
            {
                RegisterButton.IsEnabled = true;
            }
        }
    }
}