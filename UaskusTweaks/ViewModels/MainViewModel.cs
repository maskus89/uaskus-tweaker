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
    private readonly AppliedTweaksStateService _appliedTweaksState = new();

    private TweakCategoryViewModel? _selectedCategory;
    private bool _isApplying;
    private bool _createRestorePoint = true;
    private bool _isCheckingStates;
    private bool _hasUndoAvailable;
    private SetupMode _activeSetupMode = SetupMode.FullAccess;
    private string _statusText = "Choose a category, then select the changes you want.";

    public ObservableCollection<TweakCategoryViewModel> Categories { get; } = new();
    public ObservableCollection<TweakCategoryViewModel> VisibleCategories { get; } = new();
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public SystemInfoViewModel SystemInfo { get; } = new();

    public bool IsAdmin { get; } = new WindowsPrincipal(WindowsIdentity.GetCurrent())
        .IsInRole(WindowsBuiltInRole.Administrator);
    public string DisplayVersion => $"v{UpdateService.CurrentVersion.ToString(3)}";

    public SetupMode ActiveSetupMode
    {
        get => _activeSetupMode;
        private set
        {
            if (!SetProperty(ref _activeSetupMode, value))
                return;

            OnPropertyChanged(nameof(IsFullAccessMode));
            OnPropertyChanged(nameof(IsGamingMode));
            OnPropertyChanged(nameof(IsPrivacyMode));
            OnPropertyChanged(nameof(IsPerformanceMode));
            OnPropertyChanged(nameof(IsExtremePerformanceMode));
            OnPropertyChanged(nameof(ActiveSetupTitle));
            OnPropertyChanged(nameof(ActiveSetupDescription));
            OnPropertyChanged(nameof(CategoryCardWidth));
        }
    }

    public bool IsFullAccessMode => ActiveSetupMode == SetupMode.FullAccess;
    public bool IsGamingMode => ActiveSetupMode == SetupMode.Gaming;
    public bool IsPrivacyMode => ActiveSetupMode == SetupMode.Privacy;
    public bool IsPerformanceMode => ActiveSetupMode == SetupMode.Performance;
    public bool IsExtremePerformanceMode => ActiveSetupMode == SetupMode.ExtremePerformance;
    public double CategoryCardWidth => IsFullAccessMode ? 122 : 190;
    public string ActiveSetupTitle => ActiveSetupMode switch
    {
        SetupMode.Gaming => "Gaming folders",
        SetupMode.Privacy => "Privacy folders",
        SetupMode.Performance => "Performance folders",
        SetupMode.ExtremePerformance => "Extreme performance folders",
        _ => "Full access — all folders"
    };
    public string ActiveSetupDescription => IsFullAccessMode
        ? "Browse every category and build your own setup."
        : "Only folders used by this Easy Setup preset are shown.";

    public TweakCategoryViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            OnPropertyChanged(nameof(CurrentTweaks));
        }
    }

    public bool IsApplying
    {
        get => _isApplying;
        private set
        {
            SetProperty(ref _isApplying, value);
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyButtonText));
        }
    }

    public bool CanApply => !_isApplying && SelectedTweakCount > 0;

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

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public void SetStatusMessage(string message) => StatusText = message;

    public IEnumerable<TweakViewModel> CurrentTweaks =>
        SelectedCategory?.Tweaks ?? Enumerable.Empty<TweakViewModel>();

    public int SelectedTweakCount => Categories.SelectMany(c => c.Tweaks).Count(t => t.IsSelected);
    public string ApplyButtonText => IsApplying
        ? "Applying changes…"
        : SelectedTweakCount == 0
            ? "Select changes to continue"
            : $"Apply {SelectedTweakCount} selected change{(SelectedTweakCount == 1 ? "" : "s")}";

    // Commands
    public ICommand ApplySelectedCommand { get; }
    public ICommand PreviewCommand { get; }
    public ICommand ExportLogCommand { get; }
    public ICommand RefreshSystemInfoCommand { get; }
    public ICommand GamingPresetCommand { get; }
    public ICommand PrivacyPresetCommand { get; }
    public ICommand MaxPerformanceCommand { get; }
    public ICommand ExtremePerformanceCommand { get; }
    public ICommand FullAccessCommand { get; }
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

        GamingPresetCommand = new RelayCommand(_ => ApplyPreset(SetupMode.Gaming, "Gaming Preset",
            "game_mode", "game_dvr_bar", "game_fso", "game_power",
            "perf_gpu_sched", "perf_cpu_priority",
            "game_xbox_svc", "essential_edge", "perf_cpu_100", "perf_responsiveness"));

        PrivacyPresetCommand = new RelayCommand(_ => ApplyPreset(SetupMode.Privacy, "Privacy Preset",
            "essential_telemetry", "essential_activity", "essential_consumer",
            "priv_telemetry_svc", "priv_adid", "priv_cortana", "priv_feedback",
            "priv_websearch", "priv_tailored",
            "priv_diagdata", "priv_errorreporting"));

        MaxPerformanceCommand = new RelayCommand(_ => ApplyPreset(SetupMode.Performance, "Max Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_100",
            "perf_fast_startup", "perf_gpu_sched", "perf_animations",
            "perf_transparency", "perf_responsiveness", "perf_cpu_priority",
            "game_dvr_bar", "essential_edge"));

        ExtremePerformanceCommand = new RelayCommand(_ => ApplyPreset(SetupMode.ExtremePerformance, "EXTREME Performance Preset",
            "perf_power_ultimate", "perf_core_parking", "perf_cpu_throttle",
            "perf_cpu_100", "perf_fast_startup", "perf_gpu_sched",
            "perf_visual_effects", "perf_animations", "perf_transparency",
            "perf_superfetch", "perf_paging_exec", "perf_responsiveness",
            "perf_cpu_priority", "perf_winsearch",
            "ext_hpet", "ext_dynamictick", "ext_mmcss"));

        FullAccessCommand = new RelayCommand(ShowFullAccess);

        // Now try to load tweaks
        try
        {
            var categories = TweakDefinitions.GetAllCategories();
            foreach (var cat in categories)
            {
                foreach (var tweak in cat.Tweaks)
                {
                    tweak.PropertyChanged += (_, args) =>
                    {
                        if (args.PropertyName != nameof(TweakViewModel.IsSelected))
                            return;

                        cat.NotifyCounts();
                        OnPropertyChanged(nameof(SelectedTweakCount));
                        OnPropertyChanged(nameof(CanApply));
                        OnPropertyChanged(nameof(ApplyButtonText));
                        CommandManager.InvalidateRequerySuggested();
                    };
                }
                Categories.Add(cat);
                VisibleCategories.Add(cat);
            }

            RestoreAppliedSelections();
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

    private void ApplyPreset(SetupMode setupMode, string presetName, params string[] tweakIds)
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

        SetVisibleCategories(Categories.Where(category =>
            category.Tweaks.Any(tweak => ids.Contains(tweak.Id))));
        ActiveSetupMode = setupMode;
        SelectedCategory = VisibleCategories.FirstOrDefault();

        AddLog($"Preset '{presetName}' selected — {tweakIds.Length} changes ready for review. Nothing has been applied yet.", LogLevel.Info);
        StatusText = $"{presetName} selected. Review it, then press Apply to make changes.";
        OnPropertyChanged(nameof(SelectedTweakCount));
    }

    private void ShowFullAccess()
    {
        var previousCategory = SelectedCategory;
        SetVisibleCategories(Categories);
        ActiveSetupMode = SetupMode.FullAccess;
        SelectedCategory = previousCategory is not null && VisibleCategories.Contains(previousCategory)
            ? previousCategory
            : VisibleCategories.FirstOrDefault();

        StatusText = SelectedTweakCount == 0
            ? "Full Access opened. Browse every folder and choose your changes."
            : $"Full Access opened. Your {SelectedTweakCount} selected changes were kept; press Apply when ready.";
        AddLog("Full Access opened. Existing selections were kept.", LogLevel.Info);
    }

    private void SetVisibleCategories(IEnumerable<TweakCategoryViewModel> categories)
    {
        VisibleCategories.Clear();
        foreach (var category in categories)
            VisibleCategories.Add(category);
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
            var appliedIds = new List<string>();

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
                if (tweakOk) appliedIds.Add(tweak.Id);
                if (tweak.RequiresRestart) anyRestart = true;
            }

            if (appliedIds.Count > 0)
            {
                var persistedIds = _appliedTweaksState.LoadAppliedTweakIds();
                foreach (var appliedId in appliedIds)
                    persistedIds.Add(appliedId);
                _appliedTweaksState.SaveAppliedTweakIds(persistedIds);
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
            MessageBox.Show(
                "Nothing is selected yet.\n\nChoose a category, then turn on SELECT for the changes you want.",
                "Review selected changes", MessageBoxButton.OK, MessageBoxImage.Information);
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
        MessageBox.Show(sb.ToString(), "Review selected changes", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private void RestoreAppliedSelections()
    {
        var ids = _appliedTweaksState.LoadAppliedTweakIds();
        if (ids.Count == 0)
            return;

        var restored = 0;
        foreach (var tweak in Categories.SelectMany(c => c.Tweaks))
        {
            if (!ids.Contains(tweak.Id))
                continue;

            tweak.IsSelected = true;
            restored++;
        }

        if (restored > 0)
        {
            foreach (var category in Categories)
                category.NotifyCounts();
            OnPropertyChanged(nameof(SelectedTweakCount));
            AddLog($"Restored {restored} previously applied tweak selection(s).", LogLevel.Info);
        }
    }
}
