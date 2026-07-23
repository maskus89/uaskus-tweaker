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
    private readonly TweakStateDetector _stateDetector = new();
    private readonly TweakBackupService _backup = new();

    private TweakCategoryViewModel? _selectedCategory;
    private bool _isApplying;
    private bool _createRestorePoint = true;
    private bool _isCheckingStates;
    private bool _hasUndoAvailable;
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
        set
        {
            SetProperty(ref _selectedCategory, value);
            OnPropertyChanged(nameof(FilteredTweaks));
        }
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

    public bool HasUndoAvailable
    {
        get => _hasUndoAvailable;
        private set => SetProperty(ref _hasUndoAvailable, value);
    }

    public bool IsCheckingStates
    {
        get => _isCheckingStates;
        private set => SetProperty(ref _isCheckingStates, value);
    }

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
    public ICommand SelectCategoryCommand { get; }
    public ICommand RefreshTweakStatesCommand { get; }
    public ICommand UndoLastApplyCommand { get; }

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
        RefreshTweakStatesCommand = new AsyncRelayCommand(
            RefreshTweakStatesAsync,
            () => !IsApplying && !IsCheckingStates);
        UndoLastApplyCommand = new AsyncRelayCommand(UndoLastApplyAsync, () => !IsApplying && HasUndoAvailable);
        HasUndoAvailable = _backup.HasBackup;

        GamingPresetCommand = new RelayCommand(_ => ApplyPreset("Gaming Preset",
            "game_mode", "game_dvr_bar", "game_fso", "game_power",
            "perf_gpu_sched", "perf_cpu_priority",
            "game_xbox_svc", "essential_edge", "perf_cpu_100", "perf_responsiveness"));

        PrivacyPresetCommand = new RelayCommand(_ => ApplyPreset("Privacy Preset",
            "essential_telemetry", "essential_activity", "essential_consumer",
            "priv_telemetry_svc", "priv_adid", "priv_cortana", "priv_feedback",
            "priv_websearch", "priv_tailored",
            "priv_diagdata", "priv_errorreporting"));

        MaxPerformanceCommand = new RelayCommand(_ => ApplyPreset("Max Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_100",
            "perf_fast_startup", "perf_gpu_sched", "perf_animations",
            "perf_transparency", "perf_responsiveness", "perf_cpu_priority",
            "game_dvr_bar", "essential_edge"));

        ExtremePerformanceCommand = new RelayCommand(_ => ApplyPreset("EXTREME Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_throttle",
            "perf_cpu_100", "perf_fast_startup", "perf_gpu_sched",
            "perf_visual_effects", "perf_animations", "perf_transparency",
            "perf_superfetch", "perf_paging_exec", "perf_responsiveness",
            "perf_cpu_priority", "perf_winsearch",
            "ext_hpet", "ext_dynamictick", "ext_mmcss"));

        SelectCategoryCommand = new RelayCommand(param =>
        {
            if (param is TweakCategoryViewModel cat)
                SelectedCategory = cat;
        });

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
        _ = RefreshTweakStatesAsync();
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

        var conflicts = FindConflicts(selected);
        if (conflicts.Count > 0)
        {
            AddLog("Conflicting selections blocked: " + string.Join(", ", conflicts), LogLevel.Warning);
            MessageBox.Show("Two or more selected tweaks try to set the same setting to different values:\n\n" +
                            string.Join("\n", conflicts) + "\n\nDeselect one before applying.",
                "Conflicting tweaks", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var risky = selected.Where(t => t.RiskLevel is RiskLevel.High or RiskLevel.Critical).ToList();
        if (risky.Count > 0)
        {
            var result = MessageBox.Show(
                $"You selected {selected.Count} tweak(s), including {risky.Count} high-risk or critical tweak(s):\n\n" +
                string.Join("\n", risky.Select(t => $"• [{t.RiskText}] {t.Name}")) +
                "\n\nReview the Preview first if you are unsure. Continue?",
                "Review high-risk tweaks", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;
        }

        IsApplying = true;
        StatusText = "Applying tweaks…";

        try
        {
            var backup = await _backup.CaptureAsync(selected.Select(t => t.Model));
            _backup.Save(backup);
            HasUndoAvailable = true;
            AddLog($"Saved a rollback snapshot for {backup.RegistryEntries.Count} registry/service setting(s).", LogLevel.Success);
            if (backup.UnsupportedCommandCount > 0)
                AddLog($"{backup.UnsupportedCommandCount} command(s) cannot be automatically undone; the restore point remains the fallback.", LogLevel.Warning);

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
            await RefreshTweakStatesAsync(silent: true);

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

    private async Task UndoLastApplyAsync()
    {
        var result = MessageBox.Show(
            "This restores registry values and service startup settings saved before the most recent apply operation. " +
            "One-time commands and removed apps cannot be restored this way. Continue?",
            "Undo last apply", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes)
            return;

        IsApplying = true;
        StatusText = "Restoring the last saved settings…";
        try
        {
            var (success, restored, message) = await _backup.RestoreLastAsync();
            AddLog(success ? message : $"Undo failed: {message}", success ? LogLevel.Success : LogLevel.Error);
            StatusText = success ? $"Restored {restored} saved setting(s)." : "Undo failed.";
            if (success) await RefreshTweakStatesAsync(silent: true);
        }
        finally
        {
            IsApplying = false;
        }
    }

    private static List<string> FindConflicts(IEnumerable<TweakViewModel> tweaks)
    {
        var valuesByTarget = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var command in tweaks.SelectMany(tweak => tweak.Model.Commands))
        {
            string? target = null;
            string? value = null;
            var parts = command.Command.Split('|');
            if (command.Type == CommandType.Registry && parts.Length >= 4)
            {
                target = $"{parts[0]}\\{parts[1]}";
                value = $"{parts[2]}|{parts[3]}";
            }
            else if (command.Type == CommandType.Service && parts.Length >= 2)
            {
                target = $"Service:{parts[0]}";
                value = parts[1];
            }
            if (target is null || value is null) continue;
            if (!valuesByTarget.TryGetValue(target, out var values))
                valuesByTarget[target] = values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            values.Add(value);
        }
        return valuesByTarget.Where(pair => pair.Value.Count > 1).Select(pair => pair.Key).ToList();
    }

    private async Task RefreshTweakStatesAsync() => await RefreshTweakStatesAsync(silent: false);

    private async Task RefreshTweakStatesAsync(bool silent)
    {
        if (IsCheckingStates)
            return;

        IsCheckingStates = true;
        if (!silent)
        {
            StatusText = "Checking current tweak states…";
            AddLog("Checking the current state of registry and service tweaks…", LogLevel.Info);
        }

        try
        {
            var tweaks = Categories.SelectMany(category => category.Tweaks).ToList();
            foreach (var tweak in tweaks)
                tweak.State = await _stateDetector.CheckAsync(tweak.Model);

            var active = tweaks.Count(tweak => tweak.State == TweakState.Enabled);
            if (!silent)
            {
                StatusText = $"State check complete: {active} active.";
                AddLog($"State check complete: {active} tweak(s) fully active.", LogLevel.Success);
            }
        }
        catch (Exception ex)
        {
            StatusText = "Could not complete state check.";
            AddLog($"State check failed: {ex.Message}", LogLevel.Warning);
        }
        finally
        {
            IsCheckingStates = false;
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
