using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TweakAppClient
{
    public static class Logger
    {
        // IMPORTANT: Make sure this port number matches your backend's port!
        // Your backend is currently running on port 5062.
        private const string ApiBaseUrl = "http://localhost:5062";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string? _currentUser = null;

        /// <summary>
        /// Sets the username for logging after a successful login.
        /// </summary>
        /// <param name="username">The logged-in user's name.</param>
        public static void Init(string username)
        {
            _currentUser = username;
        }

        /// <summary>
        /// Sends a log message to the server.
        /// </summary>
        /// <param name="message">The log message text.</param>
        public static async Task LogAsync(string message)
        {
            if (string.IsNullOrEmpty(_currentUser))
            {
                // If no user is set, do nothing.
                return;
            }

            var logData = new
            {
                Username = _currentUser,
                Message = message
            };

            try
            {
                // This sends the log data to your backend's logging controller.
                await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/logging/log", logData);
            }
            catch
            {
                // If there's a network error, we'll just ignore it for now
                // to prevent the client app from crashing.
            }
        }
    }
}