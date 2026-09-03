using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UaskusTweaks.Services;

public sealed record UpdateInfo(
    Version Version,
    string TagName,
    string DownloadUrl,
    string? ReleaseNotes,
    long Size,
    string? Digest);

public sealed class UpdateService
{
    private const string LatestReleaseUrl =
        "https://api.github.com/repos/maskus89/uaskus-tweaker/releases/latest";
    private const string ExecutableAssetName = "UaskusTweaks.exe";

    private static readonly HttpClient Client = CreateClient();

    public static Version CurrentVersion => NormalizeVersion(
        Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0));

    public async Task<UpdateInfo?> CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        using var response = await Client.GetAsync(LatestReleaseUrl, timeout.Token);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
        var release = await JsonSerializer.DeserializeAsync<GitHubRelease>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, timeout.Token);

        if (release is null || !TryParseVersion(release.TagName, out var latestVersion))
            return null;

        var asset = release.Assets.FirstOrDefault(item =>
            string.Equals(item.Name, ExecutableAssetName, StringComparison.OrdinalIgnoreCase));

        if (asset is null || string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl))
            return null;

        latestVersion = NormalizeVersion(latestVersion);
        if (latestVersion <= CurrentVersion)
            return null;

        return new UpdateInfo(latestVersion, release.TagName, asset.BrowserDownloadUrl,
            release.Body, asset.Size, asset.Digest);
    }

    public async Task<string> DownloadAsync(UpdateInfo update,
        CancellationToken cancellationToken = default)
    {
        var updateDirectory = Path.Combine(Path.GetTempPath(), "UaskusTweaks", "updates");
        Directory.CreateDirectory(updateDirectory);
        var downloadPath = Path.Combine(updateDirectory, $"UaskusTweaks-update-{Guid.NewGuid():N}.exe");

        try
        {
            using var response = await Client.GetAsync(update.DownloadUrl,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using (var source = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var destination = new FileStream(downloadPath, FileMode.CreateNew,
                FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await source.CopyToAsync(destination, cancellationToken);
            }

            var fileInfo = new FileInfo(downloadPath);
            if (fileInfo.Length < 2 || (update.Size > 0 && fileInfo.Length != update.Size))
                throw new InvalidDataException("The downloaded update is incomplete.");

            await using (var executable = File.OpenRead(downloadPath))
            {
                if (executable.ReadByte() != 'M' || executable.ReadByte() != 'Z')
                    throw new InvalidDataException("The downloaded file is not a Windows executable.");
            }

            await VerifyDigestAsync(downloadPath, update.Digest, cancellationToken);
            return downloadPath;
        }
        catch
        {
            TryDelete(downloadPath);
            throw;
        }
    }

    public void InstallAndRestart(string downloadedExecutable)
    {
        var currentExecutable = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(currentExecutable))
            throw new InvalidOperationException("The current app location could not be found.");

        var scriptDirectory = Path.Combine(Path.GetTempPath(), "UaskusTweaks", "updates");
        Directory.CreateDirectory(scriptDirectory);
        var scriptPath = Path.Combine(scriptDirectory, $"install-{Guid.NewGuid():N}.ps1");

        const string script = """
            param(
                [Parameter(Mandatory=$true)][int]$ProcessId,
                [Parameter(Mandatory=$true)][string]$CurrentExe,
                [Parameter(Mandatory=$true)][string]$DownloadedExe
            )

            $ErrorActionPreference = 'Stop'
            $backup = "$CurrentExe.old"
            $staged = "$CurrentExe.new"
            $log = Join-Path $env:TEMP 'UaskusTweaks\update.log'

            try {
                Wait-Process -Id $ProcessId -ErrorAction SilentlyContinue
                Remove-Item -LiteralPath $backup -Force -ErrorAction SilentlyContinue
                Remove-Item -LiteralPath $staged -Force -ErrorAction SilentlyContinue
                Copy-Item -LiteralPath $DownloadedExe -Destination $staged -Force
                Move-Item -LiteralPath $CurrentExe -Destination $backup -Force
                Move-Item -LiteralPath $staged -Destination $CurrentExe -Force
                Start-Process -FilePath $CurrentExe
                Remove-Item -LiteralPath $backup -Force -ErrorAction SilentlyContinue
                Remove-Item -LiteralPath $DownloadedExe -Force -ErrorAction SilentlyContinue
            }
            catch {
                $_ | Out-String | Set-Content -LiteralPath $log
                if (Test-Path -LiteralPath $backup) {
                    Remove-Item -LiteralPath $CurrentExe -Force -ErrorAction SilentlyContinue
                    Move-Item -LiteralPath $backup -Destination $CurrentExe -Force
                }
            }
            finally {
                Remove-Item -LiteralPath $staged -Force -ErrorAction SilentlyContinue
                Remove-Item -LiteralPath $PSCommandPath -Force -ErrorAction SilentlyContinue
            }
            """;

        File.WriteAllText(scriptPath, script);

        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-WindowStyle");
        startInfo.ArgumentList.Add("Hidden");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        startInfo.ArgumentList.Add("-ProcessId");
        startInfo.ArgumentList.Add(Environment.ProcessId.ToString());
        startInfo.ArgumentList.Add("-CurrentExe");
        startInfo.ArgumentList.Add(currentExecutable);
        startInfo.ArgumentList.Add("-DownloadedExe");
        startInfo.ArgumentList.Add(downloadedExecutable);

        if (Process.Start(startInfo) is null)
            throw new InvalidOperationException("The update installer could not be started.");
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("UaskusTweaks-Updater");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return client;
    }

    private static bool TryParseVersion(string tagName, out Version version)
    {
        var value = tagName.Trim();
        if (value.StartsWith('v') || value.StartsWith('V'))
            value = value[1..];
        value = value.Split('-', 2)[0];
        return Version.TryParse(value, out version!);
    }

    private static Version NormalizeVersion(Version version) => new(
        version.Major,
        version.Minor,
        Math.Max(version.Build, 0),
        Math.Max(version.Revision, 0));

    private static async Task VerifyDigestAsync(string filePath, string? digest,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(digest) ||
            !digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
            return;

        var expected = digest["sha256:".Length..].Trim();
        await using var stream = File.OpenRead(filePath);
        var actual = Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken));
        if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("The downloaded update failed its security check.");
    }

    private static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch { }
    }

    private sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; init; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; init; }

        [JsonPropertyName("assets")]
        public List<GitHubAsset> Assets { get; init; } = new();
    }

    private sealed class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; init; } = string.Empty;

        [JsonPropertyName("size")]
        public long Size { get; init; }

        [JsonPropertyName("digest")]
        public string? Digest { get; init; }
    }
}
