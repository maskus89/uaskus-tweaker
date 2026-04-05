using System.Collections.ObjectModel;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using UaskusTweaks.Models;
using UaskusTweaks.Services;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;
using WpfApplication = System.Windows.Application;

namespace UaskusTweaks.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly TweakExecutorService _executor = new();
    private readonly RestorePointService _restorePoint = new();
    private readonly BackupService _backup = new();

    private TweakCategoryViewModel? _selectedCategory;
    private bool _isApplying;
    private bool _createRestorePoint = true;
    private string _searchText = string.Empty;
    private string _statusText = "Ready";

    public ObservableCollection<TweakCategoryViewModel> Categories { get; } = new();
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public SystemInfoViewModel SystemInfo { get; } = new();

    public bool IsAdmin { get; } = new WindowsPrincipal(WindowsIdentity.GetCurrent())
        .IsInRole(WindowsBuiltInRole.Administrator);

    public TweakCategoryViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public bool IsApplying
    {
        get => _isApplying;
        private set
        {
            SetProperty(ref _isApplying, value);
            OnPropertyChanged(nameof(CanApply));
        }
    }

    public bool CanApply => !_isApplying;

    public bool CreateRestorePoint
    {
        get => _createRestorePoint;
        set => SetProperty(ref _createRestorePoint, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnPropertyChanged(nameof(FilteredTweaks));
        }
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public IEnumerable<TweakViewModel> FilteredTweaks
    {
        get
        {
            if (SelectedCategory == null) return Enumerable.Empty<TweakViewModel>();
            var query = SelectedCategory.Tweaks.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var lower = _searchText.ToLowerInvariant();
                query = query.Where(t =>
                    t.Name.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(lower, StringComparison.OrdinalIgnoreCase));
            }
            return query;
        }
    }

    public int SelectedTweakCount => Categories.SelectMany(c => c.Tweaks).Count(t => t.IsSelected);

    // Commands
    public ICommand ApplySelectedCommand { get; }
    public ICommand PreviewCommand { get; }
    public ICommand ExportLogCommand { get; }
    public ICommand RefreshSystemInfoCommand { get; }
    public ICommand GamingPresetCommand { get; }
    public ICommand PrivacyPresetCommand { get; }
    public ICommand MaxPerformanceCommand { get; }
    public ICommand ExtremePerformanceCommand { get; }

    public MainViewModel()
    {
        // Initialize commands first (ALWAYS, even if tweaks fail to load)
        ApplySelectedCommand = new AsyncRelayCommand(
            async () => await ApplyTweaksAsync(),
            () => CanApply);

        PreviewCommand = new RelayCommand(_ => ShowPreview());
        ExportLogCommand = new RelayCommand(_ => ExportLog());
        RefreshSystemInfoCommand = new AsyncRelayCommand(
            async () => await SystemInfo.RefreshAsync());

        GamingPresetCommand = new RelayCommand(_ => ApplyPreset("Gaming Preset",
            "essential_gamedvr", "game_mode", "game_dvr_bar", "game_fso",
            "game_power", "game_nagle", "game_gpu_sched", "game_cpu_prio",
            "game_xbox_svc", "essential_edge", "perf_cpu_100", "perf_responsiveness"));

        PrivacyPresetCommand = new RelayCommand(_ => ApplyPreset("Privacy Preset",
            "essential_telemetry", "essential_activity", "essential_consumer",
            "priv_telemetry_svc", "priv_activity", "priv_adid", "priv_cortana",
            "priv_feedback", "priv_timeline", "priv_websearch", "priv_tailored",
            "priv_diagdata", "priv_errorreporting"));

        MaxPerformanceCommand = new RelayCommand(_ => ApplyPreset("Max Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_100",
            "perf_fast_startup", "perf_gpu_sched", "perf_animations",
            "perf_transparency", "perf_responsiveness", "perf_cpu_priority",
            "essential_gamedvr", "essential_edge"));

        ExtremePerformanceCommand = new RelayCommand(_ => ApplyPreset("EXTREME Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_throttle",
            "perf_cpu_100", "perf_fast_startup", "perf_gpu_sched",
            "perf_visual_effects", "perf_animations", "perf_transparency",
            "perf_superfetch", "perf_paging_exec", "perf_responsiveness",
            "perf_cpu_priority", "perf_winsearch",
            "ext_hpet", "ext_dynamictick", "ext_timer_res", "ext_mmcss",
            "ext_cpu_sched"));

        // Now try to load tweaks
        try
        {
            var categories = TweakDefinitions.GetAllCategories();
            foreach (var cat in categories)
            {
                foreach (var tweak in cat.Tweaks)
                    tweak.PropertyChanged += (_, _) => OnPropertyChanged(nameof(SelectedTweakCount));
                Categories.Add(cat);
            }
            SelectedCategory = Categories.FirstOrDefault();
            AddLog("Tweaks loaded successfully.", LogLevel.Success);
        }
        catch (Exception ex)
        {
            AddLog($"Error loading tweaks: {ex.Message}", LogLevel.Error);
        }

        try
        {
            _ = SystemInfo.RefreshAsync();
        }
        catch (Exception ex)
        {
            AddLog($"Warning: Could not load system info: {ex.Message}", LogLevel.Warning);
        }

        AddLog("Uaskus Tweaks started. Running as " + (IsAdmin ? "Administrator." : "Standard User."),
            IsAdmin ? LogLevel.Success : LogLevel.Warning);
    }

    private void ApplyPreset(string presetName, params string[] tweakIds)
    {
        // Deselect all first
        foreach (var cat in Categories)
            foreach (var t in cat.Tweaks)
                t.IsSelected = false;

        var ids = new HashSet<string>(tweakIds);
        foreach (var cat in Categories)
            foreach (var t in cat.Tweaks)
                if (ids.Contains(t.Id))
                    t.IsSelected = true;

        AddLog($"Preset '{presetName}' applied — {tweakIds.Length} tweaks selected.", LogLevel.Info);
        OnPropertyChanged(nameof(SelectedTweakCount));
    }

    private async Task ApplyTweaksAsync()
    {
        var selected = Categories.SelectMany(c => c.Tweaks).Where(t => t.IsSelected).ToList();
        if (selected.Count == 0)
        {
            AddLog("No tweaks selected.", LogLevel.Warning);
            return;
        }

        IsApplying = true;
        StatusText = "Applying tweaks…";

        try
        {
            if (CreateRestorePoint)
            {
                AddLog("Creating system restore point…", LogLevel.Info);
                var rpOk = await _restorePoint.CreateRestorePointAsync("Uaskus Tweaks – Before Changes");
                AddLog(rpOk ? "Restore point created." : "Restore point failed (continuing).",
                    rpOk ? LogLevel.Success : LogLevel.Warning);
            }

            AddLog($"Applying {selected.Count} tweak(s)…", LogLevel.Info);
            bool anyRestart = false;
            int ok = 0, fail = 0;

            for (int i = 0; i < selected.Count; i++)
            {
                var tweak = selected[i];
                StatusText = $"[{i + 1}/{selected.Count}] {tweak.Name}";
                AddLog($"Applying: {tweak.Name}", LogLevel.Info);

                bool tweakOk = true;
                foreach (var cmd in tweak.Model.Commands)
                {
                    var (success, message) = await _executor.ExecuteAsync(cmd);
                    if (success)
                        AddLog($"  ✓ {cmd.Description ?? cmd.Command}", LogLevel.Success);
                    else
                    {
                        AddLog($"  ✗ {cmd.Description ?? cmd.Command}: {message}", LogLevel.Error);
                        tweakOk = false;
                    }
                }

                if (tweakOk) ok++; else fail++;
                if (tweak.RequiresRestart) anyRestart = true;
            }

            AddLog($"Done. {ok} succeeded, {fail} failed.", fail > 0 ? LogLevel.Warning : LogLevel.Success);
            StatusText = $"Complete: {ok} succeeded, {fail} failed.";

            if (anyRestart)
            {
                AddLog("One or more tweaks require a system restart to take effect.", LogLevel.Warning);
                var result = MessageBox.Show(
                    "Some tweaks require a restart. Restart now?",
                    "Restart Required", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    System.Diagnostics.Process.Start("shutdown", "/r /t 5 /c \"Uaskus Tweaks restart\"");
            }
        }
        finally
        {
            IsApplying = false;
        }
    }

    private void ShowPreview()
    {
        var selected = Categories.SelectMany(c => c.Tweaks).Where(t => t.IsSelected).ToList();
        if (selected.Count == 0)
        {
            MessageBox.Show("No tweaks selected.", "Preview", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Selected Tweaks: {selected.Count}\n");
        foreach (var t in selected)
        {
            sb.AppendLine($"[{t.RiskText}] {t.Name}");
            sb.AppendLine($"  {t.Description}");
            if (t.RequiresRestart) sb.AppendLine("  ⚠ Requires restart");
            sb.AppendLine();
        }
        MessageBox.Show(sb.ToString(), "Preview – Selected Tweaks", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ExportLog()
    {
        try
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"UaskusTweaks_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            var lines = LogEntries.Select(e =>
                $"[{e.Timestamp:yyyy-MM-dd HH:mm:ss}] [{e.LogLevel}] {e.Message}");

            File.WriteAllLines(path, lines);
            AddLog($"Log exported to: {path}", LogLevel.Success);
        }
        catch (Exception ex)
        {
            AddLog($"Failed to export log: {ex.Message}", LogLevel.Error);
        }
    }

    private void AddLog(string message, LogLevel level = LogLevel.Info)
    {
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            LogEntries.Add(new LogEntry { Message = message, LogLevel = level });
            // Keep log to 1000 entries
            while (LogEntries.Count > 1000)
                LogEntries.RemoveAt(0);
        });
    }
}
