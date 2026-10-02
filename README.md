# FFX Status Icons UI Mod

A Fahrenheit module for Final Fantasy X that displays status icons and their remaining duration beside each party member’s battle stats.

The mod also adds status icons to the party status preview and the monster Info window.

## Features

- Displays icons for 19 status effects.
- Shows remaining turns for timed effects.
- Displays ∞ for duration values of 254 or 255.
- Shows only the icon for statuses without a turn counter.
- Automatically rotates between two status pages when both contain active statuses.
- Adds status icons to party previews and the monster Info window.
- Uses FFX’s existing status data without changing how statuses apply or expire.
- Supports layout customization through `src/Battle_UI/Battle_UI_Settings.cs`.

## Requirements

- Final Fantasy X HD Remaster (Steam).
- A compatible Fahrenheit installation.
- .NET 10 SDK if building from source.
- Fahrenheit SDK and reference packages version `1.0.0-alpha12`.

Fahrenheit is available from its [official repository](https://github.com/fahrenheit-crew/fahrenheit).

## Building

This project uses the official **Fahrenheit SDK**. You do not need `Fahrenheit.local.props` or a direct reference to Fahrenheit’s source project.

### Configure the package source

As of October 1, 2026, alpha12 is not available on public NuGet. Obtain these matching packages from your Fahrenheit build or its maintainers:

```text
Fahrenheit.Sdk.1.0.0-alpha12.nupkg
Fahrenheit.1.0.0-alpha12.nupkg
```

Local Release builds of Fahrenheit place them in `artifacts/pkg/rel`.

Register the folder containing both packages as a NuGet source:

```powershell
dotnet nuget add source "C:\Modding\FahrenheitPackages" --name FahrenheitLocalAlpha12
```

Replace the example path with your actual package folder. This is a one-time setup. Keep nuget.org enabled for other dependencies.

**Do not substitute alpha11:** its API differs from the tested alpha12 runtime and causes the monster preview to crash.

### Build with Visual Studio

1. Open `src/FFX.StatusIcons.UI.csproj` in Visual Studio with .NET 10 support.
2. Allow NuGet to restore dependencies.
3. Select **Release**.
4. Choose **Build → Build Solution**.

You do not need to add Fahrenheit’s source project to the solution.

### Build from the command line

From this repository’s root folder, run:

```powershell
dotnet build src/FFX.StatusIcons.UI.csproj -c Release
```

The SDK generates the mod manifest automatically from the project’s metadata.

## Installing

Close FFX before replacing mod files.

After building, open:

```text
src/bin/Release/net10.0/win-x86/publish/
```

Create this folder inside your game installation:

```text
fahrenheit/mods/FFX.StatusIcons.UI/
```

Copy these generated files into it:

```text
FFX.StatusIcons.UI.dll
FFX.StatusIcons.UI.manifest.json
FFX.StatusIcons.UI.deps.json
FFX.StatusIcons.UI.runtimeconfig.json
FFX.StatusIcons.UI.pdb
```

The `.pdb` is optional and helps with debugging.

Add this entry on its own line in Fahrenheit’s `mods/loadorder` file:

```text
FFX.StatusIcons.UI
```

Launch Final Fantasy X through Fahrenheit normally.

## Compatibility

Do not enable this module alongside another mod that replaces `TOBtlDrawStatusWin`.

This module replaces the party battle-status renderer. If another mod modifies that renderer, the implementations need to be merged.

The standalone alpha12 build has been tested in-game, including the monster preview. Future Fahrenheit or FFX updates may require adjustments. Keep the SDK version compatible with the installed Fahrenheit runtime.

## Integrating Into Another Mod

To incorporate the Status Icons UI into your own Fahrenheit project, see [INTEGRATION.md](INTEGRATION.md).

The guide covers the required source files, registration steps, and merging the battle UI renderer.

## License

Released under the MIT License. See `LICENSE` for details.

## Community and More Mods

This mod and other wonderful FFX mods are brought to you by [Cid’s Salvage Ship](https://discord.gg/yAQc3ngwDF).
