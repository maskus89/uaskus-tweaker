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
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            MessageBox.Show($"Unhandled error:\n{ex?.Message}\n\n{ex?.StackTrace}",
                "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"UI error:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        if (!IsRunningAsAdmin())
        {
            MessageBox.Show(
                "Uaskus Tweaks needs to run as Administrator. Right-click the EXE and choose Run as administrator.",
                "Administrator Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            Shutdown();
            return;
        }

        try
        {
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start the app:\n{ex.Message}\n\n{ex.StackTrace}",
                "Uaskus Tweaks – Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
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
