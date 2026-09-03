# Uaskus Tweaker

A modern WPF GUI application for Windows 10/11 optimization — a professional replacement for the original `uaskustweaker.bat` script.

![Build and Release](https://github.com/maskus89/uaskus-tweaker/workflows/Build%20and%20Release/badge.svg)

## Features

- Dark-themed graphical interface
- 100+ Windows tweaks organized by category (Performance, Privacy, Gaming, and more)
- Risk indicators so you know which tweaks are safe vs. advanced
- One-click presets: Gaming, Privacy, Max Performance, Extreme Performance
- Automatic system restore point before applying any changes
- Built-in log viewer and log export
- No installation required — single self-contained `.exe`

## Download

### Latest Release (Recommended)

Download [**UaskusTweaks.exe**](https://github.com/maskus89/uaskus-tweaker/releases/latest/download/UaskusTweaks.exe) and run it as **Administrator**. No installation or extraction is needed.

The app checks the latest stable GitHub release when it starts and offers to download and restart when an update is available.

### CI Build Artifact

Every push to `main` also produces a build artifact:

1. Go to [**Actions**](https://github.com/maskus89/uaskus-tweaker/actions)
2. Click the latest **Build and Release** run
3. Scroll to **Artifacts** and download `UaskusTweaks-exe`

> CI artifacts expire after 90 days. Use the Releases page for permanent downloads.

## Requirements

- Windows 10 or Windows 11 (64-bit)
- Administrator privileges (required to apply system tweaks)
- No .NET runtime needed — the `.exe` is fully self-contained

## How to Use

1. Download [UaskusTweaks.exe](https://github.com/maskus89/uaskus-tweaker/releases/latest/download/UaskusTweaks.exe)
2. Right-click → **Run as administrator**
3. Choose a category across the top, or use an Easy Setup preset on the left
4. Turn on **SELECT** for the changes you want
5. Click the large **Apply selected changes** button (a restore point is created automatically)

## Build Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Windows (required — this is a WPF/Windows-only application)

### Steps

```bash
# Clone the repo
git clone https://github.com/maskus89/uaskus-tweaker.git
cd uaskus-tweaker

# Restore packages
dotnet restore UaskusTweaks/UaskusTweaks.csproj

# Build (debug)
dotnet build UaskusTweaks/UaskusTweaks.csproj

# Publish a self-contained single-file executable
dotnet publish UaskusTweaks/UaskusTweaks.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o publish
```

The finished executable will be at `publish\UaskusTweaks.exe`.

## Publishing a New Release

Push a version tag to trigger the release workflow automatically:

```bash
git tag v2.2.1
git push origin v2.2.1
```

The workflow builds `UaskusTweaks.exe` with the tag version and attaches that executable to the GitHub Release. The tag must use a numeric version such as `v2.2.0` so installed apps can compare versions correctly.

## Project Structure

```
UaskusTweaks/
├── Models/          # Data models (Tweak, TweakCommand, RiskLevel, …)
├── ViewModels/      # MVVM view models
├── Services/        # Business logic (PowerShell, Registry, RestorePoint, …)
├── Resources/       # WPF resource dictionaries (colors, styles)
├── MainWindow.xaml  # Main application window
└── App.xaml         # Application entry point
```
