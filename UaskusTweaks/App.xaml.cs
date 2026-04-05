using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace UaskusTweaks;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "uaskus_debug.log");
        
        void Log(string msg)
        {
            try
            {
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n");
            }
            catch { }
        }

        Log("=== App Started ===");
        Log($"IsAdmin: {IsRunningAsAdmin()}");

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Log($"AppDomain Exception: {ex?.GetType().Name}: {ex?.Message}");
            MessageBox.Show($"Unhandled error:\n{ex?.Message}", "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (s, args) =>
        {
            Log($"Dispatcher Exception: {args.Exception.GetType().Name}: {args.Exception.Message}");
            MessageBox.Show($"UI error:\n{args.Exception.Message}", "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = false;
        };

        if (!IsRunningAsAdmin())
        {
            Log("Not admin, attempting elevation...");
            // Restart as admin
            var exePath = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? "";
            if (string.IsNullOrEmpty(exePath))
            {
                Log("ERROR: Could not determine exe path");
                MessageBox.Show("Unable to determine executable path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            Log($"Exe path: {exePath}");
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Log("Starting elevated process...");
                Process.Start(psi);
                Log("Elevated process started, shutting down non-admin instance");
            }
            catch (Exception ex)
            {
                Log($"Elevation failed: {ex.Message}");
                MessageBox.Show("Admin elevation failed. Please run as administrator manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Shutdown();
            return;
        }

        Log("Running as admin, creating main window...");
        try
        {
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            Log("MainWindow created, calling Show()");
            mainWindow.Show();
            Log("MainWindow shown");
        }
        catch (Exception ex)
        {
            Log($"MainWindow creation failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            MessageBox.Show($"Failed to create window:\n{ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        base.OnStartup(e);
    }

    private static bool IsRunningAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
