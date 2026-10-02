// SPDX-License-Identifier: MIT
namespace Fahrenheit.Mods.StatusIcons;

// START HERE to move things in the battle panel.
// Change only the number after = and keep the semicolon.
// Movement settings use 0 for no extra movement. Size and timing are actual values.
// Try small changes, such as 10, then rebuild and load the new DLL in the game.
// These settings affect ALL characters. They do not change anyone's stats.
internal static class Battle_UI_Settings {
    // WHOLE PANEL: moves names, HP/MP, bars, and highlights together.
    // Up: 10 moves up; -10 moves down. Right: 10 moves right; -10 moves left.
    public const float MoveWholePanelUp = 15;
    public const float MoveWholePanelRight = 0;

    // CHARACTER NAMES: moves only the names, leaving HP/MP and bars in place.
    // Increasing MoveNamesUp raises every name; decreasing it lowers them.
    public const float MoveNamesUp = 15;
    public const float MoveNamesRight = 0;

    // HP AND MP: moves both labels and numbers together, including item previews.
    // Positive Up moves up. Positive Right moves right. Negative reverses it.
    public const float MoveHpAndMpUp = 15;
    public const float MoveHpAndMpRight = 0;

    // OVERDRIVE BARS: moves only the bars and their command-cost overlays.
    public const float MoveOverdriveBarsUp = 15;
    public const float MoveOverdriveBarsRight = 0;

    // PURPLE SELECTION BAR: the background behind the selected character.
    // Positive Up raises it; positive Right moves it right. Negative reverses it.
    // Height extends downward from the top without moving the top edge.
    public const float MovePurpleBarUp = 20;
    public const float MovePurpleBarRight = 0;
    public const float PurpleBarHeight = 90;

    // GREEN TARGET BAR: independent of the purple bar.
    // Its position and height can be adjusted separately from purple.
    // Increase Up to raise it further; reduce Up to lower it.
    public const float MoveGreenBarUp = 30;
    public const float MoveGreenBarRight = 0;
    public const float GreenBarHeight = 52;

    // PREVIEW ICON: shown to the right of words such as Protect or Auto-Life.
    // Gap is the distance from the END of the text to the icon's left edge.
    // Positive Up raises only this icon; negative Up lowers it.
    public const float PreviewStatusIconSize = 26;
    public const float PreviewStatusIconGap = 6;
    public const float MovePreviewStatusIconUp = 0;

    // MONSTER INFO WINDOW: icon beside its cycling status word (e.g. Sleep).
    // Gap moves the icon farther right; positive Up raises it.
    public const float MonsterPreviewIconSize = 26;
    public const float MonsterPreviewIconGap = 6;
    public const float MoveMonsterPreviewIconUp = 0;

    // STATUS LINE: icons and turns below each character's overdrive bar.
    // -82 moves the original above-name position below the existing bar.
    // Increase toward -77 to raise it; decrease toward -87 to lower it.
    // Negative numbers move it down or left. This does not move the name or HP/MP.
    public const float MoveStatusLineUp = -82;
    public const float MoveStatusLineRight = 0;

    // STATUS ICONS: applies to every icon on both pages.
    // Spacing measures from one icon's left edge to the next (not empty space).
    public const float StatusIconSize = 26;
    public const float StatusIconSpacing = 50;
    public const float MoveStatusIconsUp = 2;
    public const float MoveStatusIconsRight = 0;
    public const float StatusDurationGap = -2;

    // INDIVIDUAL STATUSES: move an icon AND its duration together.
    // These are extra adjustments after the shared spacing above.
    // Right: positive moves right, negative moves left.
    // Up: positive moves up, negative moves down. Zero makes no adjustment.
    // Example: MoveProtectRight = 5 adds 5 units to Protect's position.
    // Active icons still pack together; these settings do not reserve empty slots.
    public const float MoveAutoLifeRight = 0;
    public const float MoveAutoLifeUp = 0;
    public const float MoveHasteRight = 0;
    public const float MoveHasteUp = 0;
    public const float MoveProtectRight = 0;
    public const float MoveProtectUp = 0;
    public const float MoveShellRight = 0;
    public const float MoveShellUp = 0;
    public const float MoveReflectRight = 0;
    public const float MoveReflectUp = 0;
    public const float MoveRegenRight = 0;
    public const float MoveRegenUp = 0;
    public const float MoveDarknessRight = 0;
    public const float MoveDarknessUp = 0;
    public const float MoveSilenceRight = 0;
    public const float MoveSilenceUp = 0;
    public const float MoveSleepRight = 0;
    public const float MoveSleepUp = 0;
    public const float MoveSlowRight = 0;
    public const float MoveSlowUp = 0;
    public const float MovePoisonRight = 0;
    public const float MovePoisonUp = 0;
    public const float MoveConfuseRight = 0;
    public const float MoveConfuseUp = 0;
    public const float MoveBerserkRight = 0;
    public const float MoveBerserkUp = 0;
    public const float MovePetrifyRight = 0;
    public const float MovePetrifyUp = 0;
    public const float MoveZombieRight = 0;
    public const float MoveZombieUp = 0;
    public const float MovePowerBreakRight = 0;
    public const float MovePowerBreakUp = 0;
    public const float MoveMagicBreakRight = 0;
    public const float MoveMagicBreakUp = 0;
    public const float MoveArmorBreakRight = 0;
    public const float MoveArmorBreakUp = 0;
    public const float MoveMentalBreakRight = 0;
    public const float MoveMentalBreakUp = 0;

    // Real seconds between pages, only when BOTH pages contain active statuses.
    public const float StatusPageSeconds = 3;
    // SPACE BETWEEN CHARACTERS: try 10 for a little more space; -10 for less.
    // The first row stays put. The second and third rows move down as space grows.
    // Use MoveWholePanelUp above if the bottom row gets too close to the edge.
    public const float ExtraSpaceBetweenCharacters = 38;

    // Wiring for the renderer. Edit the settings above, not this list.
    // Order matches the two pages in StatusDurationDisplay.cs.
    internal static (float Right, float Up) get_status_adjustment(int index) {
        return index switch {
            0 => (MoveAutoLifeRight, MoveAutoLifeUp),
            1 => (MoveHasteRight, MoveHasteUp),
            2 => (MoveProtectRight, MoveProtectUp),
            3 => (MoveShellRight, MoveShellUp),
            4 => (MoveReflectRight, MoveReflectUp),
            5 => (MoveRegenRight, MoveRegenUp),
            6 => (MoveDarknessRight, MoveDarknessUp),
            7 => (MoveSilenceRight, MoveSilenceUp),
            8 => (MoveSleepRight, MoveSleepUp),
            9 => (MoveSlowRight, MoveSlowUp),
            10 => (MovePoisonRight, MovePoisonUp),
            11 => (MoveConfuseRight, MoveConfuseUp),
            12 => (MoveBerserkRight, MoveBerserkUp),
            13 => (MovePetrifyRight, MovePetrifyUp),
            14 => (MoveZombieRight, MoveZombieUp),
            15 => (MovePowerBreakRight, MovePowerBreakUp),
            16 => (MoveMagicBreakRight, MoveMagicBreakUp),
            17 => (MoveArmorBreakRight, MoveArmorBreakUp),
            18 => (MoveMentalBreakRight, MoveMentalBreakUp),
            _ => (0, 0)
        };
    }
}
