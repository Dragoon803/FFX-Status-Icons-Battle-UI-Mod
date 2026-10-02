# Extraction validation

Checks performed on October 1, 2026:

- Independent Release build against the locally available September Fahrenheit
  source: **0 errors, 0 warnings**.
- Copied timer tests: **20 checks passed**.
- Compared original project source hashes before and after extraction:
  **no original source files changed**.
- Compared all user-editable Battle UI constants: **values preserved**.
- Confirmed the standalone DLL and manifest appear in the build output.
- Checked package source and documentation for dependencies on the original
  project and machine-specific user paths: none required.

The original integrated UI was confirmed working in-game by its author before
extraction. On October 1, 2026, the author also confirmed that the standalone
module works in-game with Resolute Path disabled. This confirms the tested local
setup, not every executable or Fahrenheit version.
The intentional integration change is that it uses the native overdrive bar
instead of the original mod's custom overdrive-cost preview helper.

The original stable project backup remains separate and unchanged.

## Official SDK packaging migration

On October 1, 2026, the source-project reference was replaced with
`Fahrenheit.Sdk/1.0.0-alpha11`, the latest version returned by NuGet at the time
of the check. This follows the official mod template. The SDK now generates
the manifest from the project metadata and publishes the installable files.

- Release build using the official NuGet SDK: **0 errors, 0 warnings**.
- Timer tests: **20 checks passed**.
- Generated manifest retains the published ID, author and repository link.
- Publish output contains the five mod files, without Fahrenheit runtime DLLs.
- No battle rendering code or original Resolute Path files changed for this migration.

The alpha11 packaging build was superseded by the alpha12 correction below.

## Monster preview crash correction

The alpha11 migration build crashed against the installed alpha12 runtime.
Dump `02102026_005611.dmp` reports `System.MissingFieldException` for
`Fahrenheit.FhMethodHandle<T>.fnptr` while preparing
`StatusIconsMod.h_TOBtlDrawInfoWinStatus`. The runtime exposes a property;
the alpha11 reference expects a field. This is a binary API mismatch.

The project now uses `Fahrenheit.Sdk/1.0.0-alpha12`, restored from locally built
Fahrenheit packages through a user-level NuGet source. No machine-specific
package path is stored in the project. The corrected Release build completed
with zero warnings and errors. The monster preview remains enabled and its
rendering code is unchanged. The author confirmed the corrected build works
again in-game on October 1, 2026, following the monster preview crash fix.
