using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
using System.Text;

namespace TweakAppClient
{
    public partial class MainWindow : Window
    {
        private bool _isOperationInProgress = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadSystemInformation();
            UpdateStatus("Ready");
        }

        private void LoadSystemInformation()
        {
            try
            {
                var systemInfo = new StringBuilder();
                
                // Get computer name
                systemInfo.AppendLine($"Computer: {Environment.MachineName}");
                
                // Get OS information
                systemInfo.AppendLine($"OS: {Environment.OSVersion.VersionString}");
                
                // Get available memory
                var totalMemory = GC.GetTotalMemory(false) / (1024 * 1024);
                systemInfo.AppendLine($"Available Memory: {totalMemory} MB");
                
                // Get disk space
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        var freeSpace = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                        var totalSpace = drive.TotalSize / (1024 * 1024 * 1024);
                        systemInfo.AppendLine($"Drive {drive.Name}: {freeSpace} GB free of {totalSpace} GB");
                    }
                }
                
                SystemInfoTextBlock.Text = systemInfo.ToString();
            }
            catch (Exception ex)
            {
                SystemInfoTextBlock.Text = $"Error loading system information: {ex.Message}";
            }
        }

        private void UpdateStatus(string status)
        {
            StatusTextBlock.Text = status;
        }

        private void ShowProgress(bool show, string message = "")
        {
            ProgressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            ProgressTextBlock.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            ProgressTextBlock.Text = message;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            ClearTempButton.IsEnabled = enabled;
            EmptyRecycleBinButton.IsEnabled = enabled;
            CleanupDiskButton.IsEnabled = enabled;
            _isOperationInProgress = !enabled;
        }

        private async void ClearTempButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isOperationInProgress) return;

            SetButtonsEnabled(false);
            ShowProgress(true, "Cleaning temporary files...");
            UpdateStatus("Cleaning temporary files...");

            var startTime = DateTime.Now;
            FeedbackTextBlock.Text = $"[{startTime:HH:mm:ss}] Temporary folder cleanup started...\n";
            await Logger.LogAsync("Temp cleanup initiated by user.");

            try
            {
                var tempPath = Path.GetTempPath();
                var files = Directory.GetFiles(tempPath);
                var directories = Directory.GetDirectories(tempPath);

                FeedbackTextBlock.Text += $"Cleaning path: {tempPath}\n";
                FeedbackTextBlock.Text += $"Found {files.Length} files and {directories.Length} directories to clean.\n";

                int deletedFiles = 0;
                int deletedDirs = 0;
                int errors = 0;

                // Delete files with progress
                for (int i = 0; i < files.Length; i++)
                {
                    try 
                    { 
                        File.Delete(files[i]); 
                        deletedFiles++;
                    } 
                    catch 
                    { 
                        errors++;
                    }
                    
                    if (i % 10 == 0) // Update progress every 10 files
                    {
                        ProgressBar.Value = (double)i / files.Length * 50; // First 50% for files
                        ShowProgress(true, $"Deleting files... {i + 1}/{files.Length}");
                        await Task.Delay(1); // Allow UI to update
                    }
                }

                // Delete directories with progress
                for (int i = 0; i < directories.Length; i++)
                {
                    try 
                    { 
                        Directory.Delete(directories[i], true); 
                        deletedDirs++;
                    } 
                    catch 
                    { 
                        errors++;
                    }
                    
                    ProgressBar.Value = 50 + (double)i / directories.Length * 50; // Second 50% for directories
                    ShowProgress(true, $"Deleting directories... {i + 1}/{directories.Length}");
                    await Task.Delay(1);
                }

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                FeedbackTextBlock.Text += $"\n[{endTime:HH:mm:ss}] Cleanup completed in {duration.TotalSeconds:F1} seconds!\n";
                FeedbackTextBlock.Text += $"Deleted: {deletedFiles} files, {deletedDirs} directories\n";
                if (errors > 0)
                {
                    FeedbackTextBlock.Text += $"Errors: {errors} items could not be deleted\n";
                }

                await Logger.LogAsync($"Temp cleanup finished. Deleted {deletedFiles} files and {deletedDirs} directories in {duration.TotalSeconds:F1}s.");
                UpdateStatus($"Cleanup completed - {deletedFiles} files, {deletedDirs} directories deleted");
            }
            catch (Exception ex)
            {
                FeedbackTextBlock.Text += $"\n[{DateTime.Now:HH:mm:ss}] Error: {ex.Message}\n";
                await Logger.LogAsync($"Temp cleanup failed with error: {ex.Message}");
                UpdateStatus("Cleanup failed - see log for details");
            }
            finally
            {
                ShowProgress(false);
                SetButtonsEnabled(true);
            }
        }

        // --- NEW CODE STARTS HERE ---

        // This block imports the function from the Windows shell (shell32.dll)
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        private async void EmptyRecycleBinButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isOperationInProgress) return;

            SetButtonsEnabled(false);
            ShowProgress(true, "Emptying Recycle Bin...");
            UpdateStatus("Emptying Recycle Bin...");

            var startTime = DateTime.Now;
            FeedbackTextBlock.Text += $"\n[{startTime:HH:mm:ss}] Emptying Recycle Bin...\n";
            await Logger.LogAsync("Recycle Bin cleanup initiated.");

            try
            {
                // Call the Windows function
                // Flags mean: no confirmation dialog, no progress animation, no sound
                uint result = SHEmptyRecycleBin(IntPtr.Zero, null, 0x00000007);

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                if (result == 0)
                {
                    FeedbackTextBlock.Text += $"[{endTime:HH:mm:ss}] Recycle Bin emptied successfully in {duration.TotalSeconds:F1} seconds.\n";
                    await Logger.LogAsync("Recycle Bin cleanup finished successfully.");
                    UpdateStatus("Recycle Bin emptied successfully");
                }
                else
                {
                    FeedbackTextBlock.Text += $"[{endTime:HH:mm:ss}] Recycle Bin operation completed with result code: {result}\n";
                    await Logger.LogAsync($"Recycle Bin cleanup completed with result code: {result}");
                    UpdateStatus("Recycle Bin operation completed");
                }
            }
            catch (Exception ex)
            {
                FeedbackTextBlock.Text += $"[{DateTime.Now:HH:mm:ss}] Error emptying Recycle Bin: {ex.Message}\n";
                await Logger.LogAsync($"Recycle Bin cleanup failed: {ex.Message}");
                UpdateStatus("Recycle Bin operation failed");
            }
            finally
            {
                ShowProgress(false);
                SetButtonsEnabled(true);
            }
        }

        private async void CleanupDiskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isOperationInProgress) return;

            SetButtonsEnabled(false);
            ShowProgress(true, "Performing disk cleanup...");
            UpdateStatus("Performing comprehensive disk cleanup...");

            var startTime = DateTime.Now;
            FeedbackTextBlock.Text += $"\n[{startTime:HH:mm:ss}] Starting comprehensive disk cleanup...\n";
            await Logger.LogAsync("Comprehensive disk cleanup initiated.");

            try
            {
                long totalBytesFreed = 0;
                int totalItemsDeleted = 0;

                // 1. Clean Windows temp folder
                ShowProgress(true, "Cleaning Windows temp folder...");
                ProgressBar.Value = 10;
                var tempResult = await CleanupDirectory(Path.GetTempPath(), "Windows Temp");
                totalBytesFreed += tempResult.BytesFreed;
                totalItemsDeleted += tempResult.ItemsDeleted;

                // 2. Clean user temp folder
                ShowProgress(true, "Cleaning user temp folder...");
                ProgressBar.Value = 30;
                var userTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
                if (Directory.Exists(userTempPath))
                {
                    var userTempResult = await CleanupDirectory(userTempPath, "User Temp");
                    totalBytesFreed += userTempResult.BytesFreed;
                    totalItemsDeleted += userTempResult.ItemsDeleted;
                }

                // 3. Clean browser cache (Chrome)
                ShowProgress(true, "Cleaning browser cache...");
                ProgressBar.Value = 50;
                var chromeCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                    "Google", "Chrome", "User Data", "Default", "Cache");
                if (Directory.Exists(chromeCachePath))
                {
                    var cacheResult = await CleanupDirectory(chromeCachePath, "Chrome Cache");
                    totalBytesFreed += cacheResult.BytesFreed;
                    totalItemsDeleted += cacheResult.ItemsDeleted;
                }

                // 4. Clean Windows logs
                ShowProgress(true, "Cleaning Windows logs...");
                ProgressBar.Value = 70;
                var logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs");
                if (Directory.Exists(logsPath))
                {
                    var logsResult = await CleanupOldLogFiles(logsPath);
                    totalBytesFreed += logsResult.BytesFreed;
                    totalItemsDeleted += logsResult.ItemsDeleted;
                }

                // 5. Empty Recycle Bin
                ShowProgress(true, "Emptying Recycle Bin...");
                ProgressBar.Value = 90;
                SHEmptyRecycleBin(IntPtr.Zero, null, 0x00000007);

                ProgressBar.Value = 100;
                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                var freedMB = totalBytesFreed / (1024 * 1024);
                FeedbackTextBlock.Text += $"\n[{endTime:HH:mm:ss}] Comprehensive cleanup completed in {duration.TotalSeconds:F1} seconds!\n";
                FeedbackTextBlock.Text += $"Total items deleted: {totalItemsDeleted}\n";
                FeedbackTextBlock.Text += $"Total space freed: {freedMB} MB\n";

                await Logger.LogAsync($"Comprehensive disk cleanup finished. Freed {freedMB} MB, deleted {totalItemsDeleted} items in {duration.TotalSeconds:F1}s.");
                UpdateStatus($"Cleanup completed - {freedMB} MB freed, {totalItemsDeleted} items deleted");
            }
            catch (Exception ex)
            {
                FeedbackTextBlock.Text += $"\n[{DateTime.Now:HH:mm:ss}] Error during disk cleanup: {ex.Message}\n";
                await Logger.LogAsync($"Disk cleanup failed with error: {ex.Message}");
                UpdateStatus("Disk cleanup failed - see log for details");
            }
            finally
            {
                ShowProgress(false);
                SetButtonsEnabled(true);
            }
        }

        private async Task<(long BytesFreed, int ItemsDeleted)> CleanupDirectory(string path, string description)
        {
            try
            {
                if (!Directory.Exists(path)) return (0, 0);

                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                long bytesFreed = 0;
                int itemsDeleted = 0;

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        bytesFreed += fileInfo.Length;
                        File.Delete(file);
                        itemsDeleted++;
                    }
                    catch { /* Ignore errors */ }
                }

                var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                foreach (var dir in directories)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        itemsDeleted++;
                    }
                    catch { /* Ignore errors */ }
                }

                if (itemsDeleted > 0)
                {
                    FeedbackTextBlock.Text += $"Cleaned {description}: {itemsDeleted} items, {bytesFreed / (1024 * 1024)} MB freed\n";
                }

                return (bytesFreed, itemsDeleted);
            }
            catch
            {
                return (0, 0);
            }
        }

        private async Task<(long BytesFreed, int ItemsDeleted)> CleanupOldLogFiles(string logsPath)
        {
            try
            {
                long bytesFreed = 0;
                int itemsDeleted = 0;
                var cutoffDate = DateTime.Now.AddDays(-30); // Delete logs older than 30 days

                var logFiles = Directory.GetFiles(logsPath, "*.log", SearchOption.AllDirectories);
                foreach (var file in logFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            bytesFreed += fileInfo.Length;
                            File.Delete(file);
                            itemsDeleted++;
                        }
                    }
                    catch { /* Ignore errors */ }
                }

                return (bytesFreed, itemsDeleted);
            }
            catch
            {
                return (0, 0);
            }
        }
        // --- NEW CODE ENDS HERE ---
    }
}