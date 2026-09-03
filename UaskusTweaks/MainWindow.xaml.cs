using System.Windows;
using System.Windows.Input;
using UaskusTweaks.ViewModels;

namespace UaskusTweaks;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        try
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            // Auto-scroll log viewer when new entries arrive
            _vm.LogEntries.CollectionChanged += (_, _) =>
            {
                try
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
                    {
                        LogScrollViewer?.ScrollToEnd();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error scrolling log:\n{ex.Message}", "Scroll Error");
                }
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"MainWindow initialization failed:\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}", 
                "Startup Error");
            throw;
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaximizeButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;

    private void CloseButton_Click(object sender, RoutedEventArgs e)
        => Close();

    private void ClearLog_Click(object sender, RoutedEventArgs e)
        => _vm.LogEntries.Clear();

    public void SetStatusMessage(string message)
        => _vm.SetStatusMessage(message);

}
