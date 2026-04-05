using System.Windows;

namespace UaskusTweaks;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        try
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Critical error during startup:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Uaskus Tweaks – Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Environment.Exit(1);
        }
    }
}
