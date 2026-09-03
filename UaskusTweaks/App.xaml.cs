using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using UaskusTweaks.Models;
using UaskusTweaks.Services;
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
        var fatalErrorShown = false;
        var isSmokeTest = e.Args.Any(arg =>
            string.Equals(arg, "--smoke-test", StringComparison.OrdinalIgnoreCase));
        
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
            // Stop WPF from repeatedly dispatching the same fatal layout error.
            args.Handled = true;
            if (fatalErrorShown)
                return;

            fatalErrorShown = true;
            if (!isSmokeTest)
                MessageBox.Show($"UI error:\n{args.Exception.Message}", "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
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

            if (isSmokeTest)
            {
                RunSetupModeSmokeTests(mainWindow);
                Log("Startup smoke test passed.");
                mainWindow.Close();
                Shutdown(0);
                return;
            }

            _ = CheckForUpdatesAsync(mainWindow, Log);
        }
        catch (Exception ex)
        {
            Log($"MainWindow creation failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            if (!isSmokeTest)
                MessageBox.Show($"Failed to create window:\n{ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        base.OnStartup(e);
    }

    private static void RunSetupModeSmokeTests(MainWindow window)
    {
        if (window.DataContext is not ViewModels.MainViewModel viewModel)
            throw new InvalidOperationException("The main view model was not attached.");

        Validate(viewModel.GamingPresetCommand, SetupMode.Gaming, 10,
            "Essential Tweaks", "Ultimate Performance", "Gaming Tweaks");
        Validate(viewModel.PrivacyPresetCommand, SetupMode.Privacy, 11,
            "Essential Tweaks", "Privacy Tweaks");
        Validate(viewModel.MaxPerformanceCommand, SetupMode.Performance, 11,
            "Essential Tweaks", "Ultimate Performance", "Gaming Tweaks");
        Validate(viewModel.ExtremePerformanceCommand, SetupMode.ExtremePerformance, 17,
            "Ultimate Performance", "EXTREME Performance");

        viewModel.FullAccessCommand.Execute(null);
        window.UpdateLayout();
        if (viewModel.ActiveSetupMode != SetupMode.FullAccess ||
            viewModel.VisibleCategories.Count != viewModel.Categories.Count ||
            viewModel.SelectedTweakCount != 17)
        {
            throw new InvalidOperationException("Full Access did not restore all folders while preserving selections.");
        }

        void Validate(ICommand command, SetupMode expectedMode, int expectedSelections,
            params string[] expectedCategories)
        {
            command.Execute(null);
            window.UpdateLayout();

            var actualCategories = viewModel.VisibleCategories.Select(category => category.Name).ToArray();
            if (viewModel.ActiveSetupMode != expectedMode ||
                viewModel.SelectedTweakCount != expectedSelections ||
                !actualCategories.SequenceEqual(expectedCategories))
            {
                throw new InvalidOperationException($"The {expectedMode} Easy Setup view is not configured correctly.");
            }
        }
    }

    private async Task CheckForUpdatesAsync(MainWindow owner, Action<string> log)
    {
        await Task.Delay(1200);
        var updater = new UpdateService();

        UpdateInfo? update;
        try
        {
            update = await updater.CheckForUpdateAsync();
        }
        catch (Exception ex)
        {
            // An unavailable network should never interrupt normal startup.
            log($"Update check skipped: {ex.Message}");
            return;
        }

        if (update is null)
        {
            log($"No update available. Current version: {UpdateService.CurrentVersion.ToString(3)}");
            return;
        }

        log($"Update available: {update.TagName}");
        var result = MessageBox.Show(owner,
            $"A new version of Uaskus Tweaks is available.\n\n" +
            $"Current version: {UpdateService.CurrentVersion.ToString(3)}\n" +
            $"New version: {update.Version.ToString(3)}\n\n" +
            "Download it now? The app will restart automatically when it is ready.",
            "Update available", MessageBoxButton.YesNo, MessageBoxImage.Information);

        if (result != MessageBoxResult.Yes)
        {
            log("User postponed the update.");
            return;
        }

        owner.SetStatusMessage($"Downloading update {update.Version.ToString(3)}…");
        owner.Cursor = Cursors.Wait;
        try
        {
            var downloadedExecutable = await updater.DownloadAsync(update);
            owner.SetStatusMessage("Update downloaded. Restarting…");
            updater.InstallAndRestart(downloadedExecutable);
            Shutdown();
        }
        catch (Exception ex)
        {
            log($"Update failed: {ex.Message}");
            owner.SetStatusMessage("The update could not be installed. You can keep using the app.");
            MessageBox.Show(owner,
                "The update could not be installed automatically. The current version is still safe to use.\n\n" +
                $"Details: {ex.Message}",
                "Update failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            owner.Cursor = null;
        }
    }

    private static bool IsRunningAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
