namespace UaskusTweaks.Services;

public class RestorePointService
{
    private readonly PowerShellService _ps = new();

    public async Task<bool> CreateRestorePointAsync(string description)
    {
        // Enable system restore on C: first (in case it was disabled)
        await _ps.ExecuteAsync("Enable-ComputerRestore -Drive 'C:\\'");

        var cmd = $"Checkpoint-Computer -Description '{description.Replace("'", "\\'")}' -RestorePointType MODIFY_SETTINGS";
        var (success, output) = await _ps.ExecuteAsync(cmd);
        return success;
    }
}
