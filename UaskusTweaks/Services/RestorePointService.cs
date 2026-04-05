namespace UaskusTweaks.Services;

public class RestorePointService
{
    private readonly PowerShellService _ps = new();

    public async Task<bool> CreateRestorePointAsync(string description)
    {
        // Enable system restore on C: first (in case it was disabled)
        await _ps.ExecuteAsync("Enable-ComputerRestore -Drive 'C:\\'");

        // Use double-quoted description; since PowerShellService uses -EncodedCommand,
        // no additional escaping of the description string is required.
        var cmd = $"Checkpoint-Computer -Description \"{description}\" -RestorePointType MODIFY_SETTINGS";
        var (success, _) = await _ps.ExecuteAsync(cmd);
        return success;
    }
}
