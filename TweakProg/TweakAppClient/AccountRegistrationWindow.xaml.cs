using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace TweakAppClient
{
    public partial class AccountRegistrationWindow : Window
    {
        // Make sure this URL and port are correct for your backend!
        private const string ApiBaseUrl = "http://localhost:5062";
        private readonly HttpClient _httpClient = new();

        public AccountRegistrationWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // --- 1. Basic Validation ---
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                StatusTextBlock.Text = "Email and password are required.";
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                StatusTextBlock.Text = "Passwords do not match.";
                return;
            }

            StatusTextBlock.Text = "Registering...";
            RegisterButton.IsEnabled = false;

            // --- 2. Get Hardware ID and Prepare Data ---
            var hardwareId = HardwareInfo.GetMotherboardId();
            var requestData = new
            {
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password,
                HardwareId = hardwareId
            };

            // --- 3. Send Data to the New API Endpoint ---
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/accountapi/register", requestData);

                if (response.IsSuccessStatusCode)
                {
                    StatusTextBlock.Text = "Registration successful! Please check your email to confirm your account.";
                }
                else
                {
                    // Try to read the error message from the backend
                    var errorContent = await response.Content.ReadAsStringAsync();
                    StatusTextBlock.Text = $"Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Network error: {ex.Message}";
            }
            finally
            {
                RegisterButton.IsEnabled = true;
            }
        }
    }
}