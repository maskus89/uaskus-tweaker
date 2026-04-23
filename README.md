# Uaskus Tweaker

Windows 10/11 optimization utility with a modern WPF interface for debloating, privacy hardening, and performance tuning.

![Build and Release](https://github.com/maskus89/uaskus-tweaker/workflows/Build%20and%20Release/badge.svg)

Uaskus Tweaker packages common Windows tweaks behind a single GUI, with risk labels, preset bundles, logging, and automatic restore point creation before changes are applied.

## Highlights

- 100+ Windows tweaks grouped by category
- One-click presets for gaming, privacy, and performance
- Clear risk indicators for safer review before applying changes
- Built-in log output and log export
- Self-contained Windows executable for end users

## Quick Start

1. Download the latest `UaskusTweaks.zip` from [GitHub Releases](https://github.com/maskus89/uaskus-tweaker/releases/latest).
2. Extract the archive.
3. Run `UaskusTweaks.exe` as **Administrator**.
4. Review the selected tweaks and their risk level.
5. Apply changes and restart Windows if prompted.

## Safety Notes

- This application changes Windows system settings, services, boot configuration, and registry values.
- Some tweaks require a restart before they take effect.
- A system restore point is created before applying changes.
- Review high-risk and extreme tweaks carefully before using them on production or shared machines.

## Requirements

- Windows 10 or Windows 11, 64-bit
- Administrator privileges
- No separate .NET runtime for the published executable

## Distribution

### Stable Release

Use the latest release artifact from [GitHub Releases](https://github.com/maskus89/uaskus-tweaker/releases/latest).

### CI Artifact

Every push to `main` also produces a portable build artifact:

1. Open [GitHub Actions](https://github.com/maskus89/uaskus-tweaker/actions)
2. Select the latest `Build and Release` run
3. Download the `UaskusTweaks-exe` artifact

## Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Windows, because the project targets WPF
- [Node.js](https://nodejs.org/) for the Vite-powered documentation site

### Build

```powershell
dotnet restore src/UaskusTweaks.csproj
dotnet build src/UaskusTweaks.csproj
```

### Publish

```powershell
dotnet publish src/UaskusTweaks.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o publish
```

Published output:

```text
publish\UaskusTweaks.exe
```

### Site

The repository also includes a Vite + React + TypeScript site:

```powershell
npm install
npm run site:dev
npm run site:typecheck
npm run site:build
```

Site source lives in `site/` and the production build is emitted to `site/dist/`.

## Repository Layout

```text
.github/             CI/CD workflows
site/                Vite + React + TypeScript site source
src/                 WPF application source code
README.md
RELEASES.md
uaskus-tweaker.sln
```

```text
src/
|- Models/           Domain models
|- Resources/        WPF resource dictionaries
|- Services/         Windows integration and tweak execution logic
|- ViewModels/       MVVM view models and commands
|- App.xaml
|- MainWindow.xaml
|- UaskusTweaks.csproj
```

## Documentation

- Site source entrypoint: [site/index.html](site/index.html)
- Release notes: [RELEASES.md](RELEASES.md)
