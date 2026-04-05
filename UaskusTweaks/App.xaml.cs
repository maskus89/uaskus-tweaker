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
        if (!IsRunningAsAdmin())
        {
            try
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Environment.ProcessPath ?? string.Empty;
                var psi = new ProcessStartInfo(exePath)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
            }
            catch
            {
                // User cancelled UAC or elevation failed; continue without admin
            }
            Shutdown();
            return;
        }

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

        base.OnStartup(e);
    }

    private static bool IsRunningAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
