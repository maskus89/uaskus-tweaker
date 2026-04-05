using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UaskusTweaks.ViewModels;

namespace UaskusTweaks;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainViewModel();
        DataContext = _vm;

        // Auto-scroll log viewer when new entries arrive
        _vm.LogEntries.CollectionChanged += (_, _) =>
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
            {
                LogScrollViewer.ScrollToEnd();
            });
        };
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
}
