# FFX Status Icons UI Mod

A Fahrenheit module for Final Fantasy X that displays status icons and remaining
turns beside the party's battle stats. It also adds matching icons to the party
status preview and the monster Info window's cycling status label.

This package is independent of Resolute Path. It includes its own initialization,
native function declarations, manifest and tests. You can build it as a separate
module or integrate its source into an existing Fahrenheit mod.

[GitHub repository](https://github.com/Dragoon803/FFX-Status-Icons-Battle-UI-Mod)

## Features

- Nineteen status icons loaded from the installed FFX-2 texture archive.
- Remaining turns for timed effects; infinity for duration values 254 and 255.
- Icon-only display for Auto-Life and statuses without a turn counter.
- Two pages that rotate only when both contain active statuses.
- Separate icon offsets, spacing, preview settings and ribbon heights.
- Native FFX overdrive bars. No custom combat or overdrive-cost rules are included.

The game controls status application and expiration. This module only reads
those values and displays them.

## Build the module

You need Windows, the .NET 10 SDK, and a Fahrenheit source checkout compatible
with your installed game and Fahrenheit runtime. The code was extracted from a
working UI using the September 2026 Fahrenheit API; later versions may need
adjustments. Native addresses also depend on the FFX executable version.

1. Obtain Fahrenheit from its [official repository](https://github.com/fahrenheit-crew/fahrenheit).
   Follow its build prerequisites for the version you use.
2. Copy `Fahrenheit.local.props.example` to `Fahrenheit.local.props`.
3. Set `FahrenheitRoot` to your Fahrenheit checkout, specifically the directory
   containing `src/core/Fahrenheit.csproj`. This local file is ignored by Git.
4. From this package's root folder, run:

```powershell
dotnet build src/FFX.StatusIcons.UI.csproj -c Release
dotnet run --project tests/StatusPages/StatusPages.csproj
```

Alternatively, put the checkout in `external/fahrenheit`, or pass its path:

```powershell
dotnet build src/FFX.StatusIcons.UI.csproj -c Release "-p:FahrenheitRoot=C:/Modding/fahrenheit"
```

### Using Visual Studio

Complete the `Fahrenheit.local.props` setup above before opening the project.
Save it in the package root, alongside this README, not inside `src`. Make sure
its filename does not end with `.example` or `.txt`.

1. Open `src/FFX.StatusIcons.UI.csproj` in Visual Studio with .NET 10 support.
2. In Solution Explorer, right-click the top **Solution** entry, not the project.
3. Choose **Add → Existing Project**, then select
   `src/core/Fahrenheit.csproj` inside your Fahrenheit checkout.
4. Select **Release**, then choose **Build → Build Solution**.

If NU1104 reports a missing Fahrenheit project, check `FahrenheitRoot` and the
local props filename. If NU1105 reports missing project information, confirm
the Fahrenheit project is loaded in the same solution, then rebuild.

### Install the built module

Close FFX before replacing files or changing the load order.

1. Open the build output: `src/bin/Release/net10.0/win-x86/`.
2. Create `FFX.StatusIcons.UI` inside the game's `fahrenheit/mods` directory.
3. Copy these files directly into that folder:

   ```text
   FFX.StatusIcons.UI.dll
   FFX.StatusIcons.UI.manifest.json
   FFX.StatusIcons.UI.deps.json
   FFX.StatusIcons.UI.runtimeconfig.json
   FFX.StatusIcons.UI.pdb
   ```

   The `.pdb` is optional and helps with debugging. Use the installed runtime's
   dependencies; do not replace Fahrenheit runtime DLLs with build-output copies.

4. Back up `fahrenheit/mods/loadorder`. Add `FFX.StatusIcons.UI` on its own line
   and remove any conflicting battle-UI mod entry for the test. The file has no
   `.txt` extension; the entry has no `.dll` extension.
5. Start FFX through Fahrenheit as usual and enter battle.

Do not enable this module alongside another replacement for `TOBtlDrawStatusWin`
(including a mod that already contains this UI). Merge the implementations
instead. This hook replaces the party renderer, so two replacements do not
automatically combine their changes.

## Change the layout

Start with `src/Battle_UI/Battle_UI_Settings.cs`. The copied layout is:

```text
Character name        HP       MP
Overdrive bar
Status icons and durations
```

| Setting | Purpose |
| --- | --- |
| `MoveWholePanelUp`, `MoveWholePanelRight` | Move the party panel |
| `ExtraSpaceBetweenCharacters` | Change the space between character sections |
| `MoveStatusLineUp`, `MoveStatusLineRight` | Move icons and durations together |
| `StatusIconSize`, `StatusIconSpacing` | Change icon size and distance between icons |
| `StatusDurationGap` | Change the gap before a number or infinity symbol |
| `MoveHasteUp`, `MoveHasteRight`, etc. | Adjust a specific status |
| `StatusPageSeconds` | Set real seconds between populated pages |
| `PurpleBarHeight`, `GreenBarHeight` | Resize each ribbon independently |
| `PreviewStatusIconSize`, `PreviewStatusIconGap` | Adjust party preview icons |
| `MonsterPreviewIconSize`, `MonsterPreviewIconGap` | Adjust monster Info icons |

Positive **Up** raises an element; positive **Right** moves it right. Negative
values reverse the direction. Ribbon height extends downward from its top edge.
Rebuild after editing: these settings are not live controls.

Active icons pack together. Adding Auto-Life may move Haste to the right because
Auto-Life appears first. Individual offsets do not reserve slots for absent statuses.

## Integrate into your own mod

See [INTEGRATION.md](INTEGRATION.md) for the source files, registration steps and
places where an existing renderer must be merged. No files from Resolute Path
are required.

## Assets and compatibility

The module loads `/FFX-2_Data/GameData/PS3Data/menu/D3D11/freetex.dds.phyre` through
Fahrenheit. It expects a 1024-by-768 texture. No game textures or exported PNGs
are bundled. The source uses the icon rectangles from the working UI.

The frame queue passes icon positions from the game thread to Fahrenheit's
rendering thread. Text is drawn by FFX; icons are drawn through ImGui. If OBS
records the text but misses icons, check its Game Capture overlay setting.

Fahrenheit is a work in progress. These integration instructions are best-effort
guidance for this API, not an authoritative framework specification. For framework
support, use the developer Discord linked in the official Fahrenheit README.

## Verification

The included timer tests run without the game. They cover empty pages, one-page
displays, rotation boundaries, resets and independent character timers.

The author confirmed the standalone module works in-game with Resolute Path
disabled on October 1, 2026. Retest after building for a different setup or
changing native bindings. Check three party members, status application/removal, finite and
unlimited durations, page rotation, party previews, monster Info previews and
entering/exiting battle. Passing a build does not validate native addresses.

The source retains its MIT SPDX notices and credits Dragoon803 in the manifest.
Fahrenheit and the game's assets are separate dependencies with their own terms.

## Uploading the source

Use this folder as the repository root. Include the source, tests, documentation,
license, example props file and dotfiles. The `.gitignore` excludes build output,
Visual Studio caches, local Fahrenheit paths and external dependencies.

If uploading through GitHub's web interface, exclude those files yourself:
`.gitignore` does not filter files you manually select for browser upload.
Local Visual Studio solutions can contain paths to your Fahrenheit checkout;
they are excluded as well. Users can open the portable `.csproj` directly.
