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
