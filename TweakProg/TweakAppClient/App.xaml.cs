using System.Windows;

namespace TweakAppClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // For now, let's open our new window directly to test it.
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}