# Integrating the status UI

The simplest option is to build this package as its own Fahrenheit module.
Use the steps below if you want the settings and rendering inside your mod.

## Copy the feature

1. Copy `src/Battle_UI`, `src/NativeDelegates.cs`, and `src/NativeFunctions.cs`
   into your mod's source directory.
2. Change `Fahrenheit.Mods.StatusIcons` to your namespace in all copied files,
   including `using static` directives.
3. Change each `partial class StatusIconsMod` to the name of your existing
   `FhModule` class. That class must also be `partial` and allow unsafe code.
4. Keep the helper types such as `Battle_UI_Settings` and `StatusPageTimer`.
   Rename them consistently if your mod already defines the same names.

Use the official Fahrenheit SDK in your own project, as shown in
`src/FFX.StatusIcons.UI.csproj`. It supplies unsafe-code support, the matching
Fahrenheit reference and its transitive ImGui dependency. Keep your own mod
metadata: the SDK generates its manifest from your project properties. Do not
copy a second module entry point when merging into your existing module.

## Register the hooks and frame events

Use `src/StatusIconsMod.cs` as the integration example:

1. Copy its `_TOBtlDrawStatusWin` and `_TOBtlDrawInfoWinStatus` method handles into
   your module, unless equivalent handles already exist.
2. Call `load_native_functions()` from your existing `init` before using the UI.
3. Register `h_TOBtlDrawStatusWin` and `h_TOBtlDrawInfoWinStatus` with those handles.
4. Subscribe `begin_status_icon_frame` to `GameLoop.PreUpdate` and
   `finish_status_icon_frame` to `GameLoop.PostUpdate` once each. Preserve your
   other initialization and event subscriptions.
5. `StatusIcons.cs` supplies `render_imgui`. If you already override it, move the
   copied method's body into a separate helper and call that helper from your
   existing override. Do not keep two overrides on the same class.

The party hook is a replacement renderer. If your mod already changes that
function, merge the drawing changes in `TOBtlDrawStatusWin.cs`; adding another
hook alone can hide one implementation. The monster preview hook calls the next
function in the hook chain, then queues the icon for the label just drawn.

## Keep the data and drawing connected

`StatusDurationDisplay.cs` reads `Chr.ram` status data using `_MsGetChr` and queues
the currently visible page. `StatusIcons.cs` publishes those requests after the
game update and draws them during `render_imgui`. Keep the queue and lock: the
game update and rendering may run on different threads.

The standalone renderer calls the native `_TOBtlDrawStatusLimitGauge` directly.
If your mod needs an overdrive-cost overlay, adapt that call in
`TOBtlDrawStatusWin.cs` to your existing drawing helper. No MP-cost or combat
calculation changes are part of this package.

## Maintain the mappings

- `StatusDurationDisplay.cs`: status values in page order.
- `StatusIcons.cs`: matching icon rectangles in the same order.
- `Battle_UI_Settings.cs`: matching per-status adjustment mapping.
- `StatusPreviewIcons.cs`: native preview text IDs mapped to those rectangles.

When adding a status, update all applicable mappings together. The first page
currently has ten entries; update page boundaries, turn-counter classification
and queue capacity if you change that structure.

Names and preview labels still come from FFX. Status IDs, rather than English
strings, choose preview icons. This lets translated labels use the same mapping.

## Check the native bindings

`NativeFunctions.cs` holds callable function offsets, `StatusIconsMod.cs` holds
hook offsets, and `BattleStatusUI_Definitions.cs` holds data offsets and layout
constants. Offsets are relative to `FFX.exe`, not absolute process addresses.

Verify these against the executable version supported by your mod. Preserve
delegate calling conventions and parameter order when updating bindings.
The research notes copied into `docs/native-reference` describe the baseline
functions; they do not certify compatibility with a newer executable.

Run the timer tests after changes and test the renderer in-game before release.
