using System;
using System.IO;
using System.Runtime.InteropServices; // THIS 'using' IS NEW
using System.Threading.Tasks;
using System.Windows;

namespace TweakAppClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ClearTempButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable button to prevent multiple clicks
            ClearTempButton.IsEnabled = false;

            // Provide feedback and log the start of the operation
            FeedbackTextBlock.Text = "Temporary folder cleanup started...\n";
            await Logger.LogAsync("Temp cleanup initiated by user.");

            try
            {
                var tempPath = Path.GetTempPath();
                var files = Directory.GetFiles(tempPath);
                var directories = Directory.GetDirectories(tempPath);

                FeedbackTextBlock.Text += $"Cleaning path: {tempPath}\n";

                // Delete files
                foreach (var file in files)
                {
                    try { File.Delete(file); } catch { /* Ignore errors */ }
                }

                // Delete directories
                foreach (var dir in directories)
                {
                    try { Directory.Delete(dir, true); } catch { /* Ignore errors */ }
                }

                FeedbackTextBlock.Text += $"\nCleanup finished! Deleted {files.Length} files and {directories.Length} directories.";
                await Logger.LogAsync($"Temp cleanup finished. Deleted {files.Length} files and {directories.Length} directories.");
            }
            catch (Exception ex)
            {
                FeedbackTextBlock.Text += $"\nAn unexpected error occurred: {ex.Message}";
                await Logger.LogAsync($"Temp cleanup failed with error: {ex.Message}");
            }
            finally
            {
                // Re-enable the button
                ClearTempButton.IsEnabled = true;
            }
        }

        // --- NEW CODE STARTS HERE ---

        // This block imports the function from the Windows shell (shell32.dll)
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        private async void EmptyRecycleBinButton_Click(object sender, RoutedEventArgs e)
        {
            EmptyRecycleBinButton.IsEnabled = false;
            FeedbackTextBlock.Text += "\nEmptying Recycle Bin...\n";
            await Logger.LogAsync("Recycle Bin cleanup initiated.");

            try
            {
                // Call the Windows function
                // Flags mean: no confirmation dialog, no progress animation, no sound
                uint result = SHEmptyRecycleBin(IntPtr.Zero, null, 0x00000007);

                FeedbackTextBlock.Text += "Recycle Bin has been emptied.\n";
                await Logger.LogAsync("Recycle Bin cleanup finished successfully.");
            }
            catch (Exception ex)
            {
                FeedbackTextBlock.Text += $"Error emptying Recycle Bin: {ex.Message}\n";
                await Logger.LogAsync($"Recycle Bin cleanup failed: {ex.Message}");
            }
            finally
            {
                EmptyRecycleBinButton.IsEnabled = true;
            }
        }
        // --- NEW CODE ENDS HERE ---
    }
}