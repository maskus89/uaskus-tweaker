using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UaskusTweaks.ViewModels;

namespace UaskusTweaks;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private Button? _lastSelectedButton;

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

    private void CategoryButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn.Background != (System.Windows.Media.Brush)FindResource("BrushCyan"))
            {
                btn.Background = (System.Windows.Media.Brush)FindResource("BrushBg3");
            }
        }
    }

    private void CategoryButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn.Background != (System.Windows.Media.Brush)FindResource("BrushCyan"))
            {
                btn.Background = (System.Windows.Media.Brush)FindResource("BrushBg2");
            }
        }
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button clickedBtn && clickedBtn.Tag is TweakCategoryViewModel selectedCategory)
        {
            // Reset the previous button
            if (_lastSelectedButton != null && _lastSelectedButton != clickedBtn)
            {
                _lastSelectedButton.Background = (System.Windows.Media.Brush)FindResource("BrushBg2");
                _lastSelectedButton.Foreground = (System.Windows.Media.Brush)FindResource("BrushTextSecondary");
            }
            
            // Highlight the clicked button
            clickedBtn.Background = (System.Windows.Media.Brush)FindResource("BrushCyan");
            clickedBtn.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            
            // Remember this button for next time
            _lastSelectedButton = clickedBtn;
            
            // Update the view model
            _vm.SelectedCategory = selectedCategory;
        }
    }
}
