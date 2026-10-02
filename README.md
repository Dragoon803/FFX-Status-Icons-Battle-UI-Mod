# FFX Status Icons UI Mod

A Fahrenheit module for Final Fantasy X that improves the battle UI by displaying status icons and their remaining duration directly beside each party member's battle stats.

The mod also adds status icons to the party status preview and the monster Info window.

## Features

- Displays icons for 19 status effects.
- Shows remaining turns for timed status effects.
- Displays unlimited-duration effects without a turn counter.
- Automatically rotates between two status pages when necessary.
- Adds status icons to party status previews.
- Adds status icons to the monster Info window.
- Uses FFX's existing status data and does not change how statuses are applied or expire.
- Layout and icon positioning can be customized in `Battle_UI_Settings.cs`.

## Requirements

- Final Fantasy X HD Remaster (Steam)
- Fahrenheit
- .NET 10 SDK if building from source

Fahrenheit can be obtained from its official repository:

https://github.com/fahrenheit-crew/fahrenheit

## Building

Copy:

`Fahrenheit.local.props.example`

and rename the copy to:

`Fahrenheit.local.props`

Open the new file and set `FahrenheitRoot` to your local Fahrenheit source directory. This should be the directory containing:

`src/core/Fahrenheit.csproj`

Then build the project:

## Installing

After building, open:

`src/bin/Release/net10.0/win-x86/`

Create the following folder inside your Fahrenheit installation:

```text
fahrenheit/mods/FFX.StatusIcons.UI/
```

Copy the generated module files into that folder.

Then add:

```text
FFX.StatusIcons.UI
```

to Fahrenheit's `mods/loadorder` file.

Launch Final Fantasy X through Fahrenheit normally.

## Compatibility

Do not enable this module alongside another mod that replaces `TOBtlDrawStatusWin`.

This module replaces the party battle-status renderer, so multiple mods replacing the same function will conflict. If another mod modifies this renderer, the implementations will need to be merged.

The module was built using the September 2026 Fahrenheit API. Future Fahrenheit or FFX updates may require adjustments.

## Integrating Into Another Mod

If you want to incorporate the Status Icons UI directly into your own Fahrenheit project instead of loading it as a separate module, see:

[INTEGRATION.md](INTEGRATION.md)

The integration guide contains the required source files, registration steps, and information about merging the battle UI renderer.

## License

Released under the MIT License. See `LICENSE` for details.

## Community and More Mods
This mod and other wonderful FFX mods brought to you by [Cid's Salvage Ship](https://discord.gg/yAQc3ngwDF).
