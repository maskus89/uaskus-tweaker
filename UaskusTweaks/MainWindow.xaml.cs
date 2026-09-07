using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using UaskusTweaks.ViewModels;

namespace UaskusTweaks;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    internal void ValidateResponsiveCategories()
    {
        var originalWidth = Width;
        var originalHeight = Height;
        try
        {
            foreach (var size in new[] { new Size(920, 620), new Size(1220, 820), new Size(1600, 900) })
            {
                Width = size.Width;
                Height = size.Height;
                UpdateLayout();
                double? firstWidth = null;
                for (var i = 0; i < CategorySelector.Items.Count; i++)
                {
                    var card = (FrameworkElement)CategorySelector.ItemContainerGenerator.ContainerFromIndex(i);
                    var origin = card.TranslatePoint(new Point(), CategorySelector);
                    if (card.ActualWidth <= 0 || origin.X < -1 ||
                        origin.X + card.ActualWidth > CategorySelector.ActualWidth + 1 ||
                        (firstWidth.HasValue && Math.Abs(firstWidth.Value - card.ActualWidth) > 2))
                        throw new InvalidOperationException($"Category cards do not fit equally at {size}.");
                    firstWidth ??= card.ActualWidth;
                }
                if (firstWidth < CategorySelector.ActualWidth / CategorySelector.Items.Count - 16)
                    throw new InvalidOperationException("Category cards are not filling their available row.");
            }
        }
        finally { Width = originalWidth; Height = originalHeight; }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var handle = new WindowInteropHelper(this).Handle;
        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfo(MonitorFromWindow(handle, 2), ref info)) return;
        var source = HwndSource.FromHwnd(handle);
        if (source?.CompositionTarget is null) return;
        var transform = source.CompositionTarget.TransformFromDevice;
        var available = transform.Transform(new Vector(info.Work.Right - info.Work.Left,
            info.Work.Bottom - info.Work.Top));
        MinWidth = Math.Min(920, available.X);
        MinHeight = Math.Min(620, available.Y);
        Width = Math.Max(MinWidth, Math.Min(1220, available.X * 0.94));
        Height = Math.Max(MinHeight, Math.Min(820, available.Y * 0.94));
        var origin = transform.Transform(new Point(info.Work.Left, info.Work.Top));
        Left = origin.X + (available.X - Width) / 2;
        Top = origin.Y + (available.Y - Height) / 2;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo { public int Size; public NativeRect Monitor, Work; public uint Flags; }
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr window, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

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
