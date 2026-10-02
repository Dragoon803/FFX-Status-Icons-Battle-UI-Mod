// SPDX-License-Identifier: MIT

using Fahrenheit.Gui;
using static Fahrenheit.Mods.StatusIcons.BattleStatusWindowDefinitions;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    // Keep the game's preview text, then place its matching icon after the word.
    // Measure with the native font: character counts do not give reliable widths.
    private void draw_status_preview(ushort text_id, byte* text, float left, float top) {
        if (text == null) {
            return;
        }

        bool has_icon = try_get_status_preview_icon(text_id, out Rect icon);
        float text_width = 0;

        if (has_icon) {
            _ToGetBtlEasyFontWidth(text, &text_width, 0, PreviewTextScale, PreviewTextSpacing);
        }

        _ToMakeBtlEasyFont(text, left, top, 0, PreviewTextScale, PreviewTextSpacing);

        if (has_icon && float.IsFinite(text_width) && text_width >= 0) {
            queue_status_icon(left + text_width, top, icon,
                Battle_UI_Settings.PreviewStatusIconSize,
                Battle_UI_Settings.PreviewStatusIconGap,
                Battle_UI_Settings.MovePreviewStatusIconUp);
        }
    }

    // Native menu-text IDs, not spell IDs or translated words.
    // TOBtlMakeStatusBuff (008981D0) constructs these from:
    // 1006 + permanent flag bit, 1012 + duration field, 1020 + extra flag bit.
    // See docs/native-reference/StatusPreviewResearch.txt and Fahrenheit's status.cs.
    // Reuse the same rectangles as the main status row so both displays agree.
    private static bool try_get_status_preview_icon(ushort text_id, out Rect icon) {
        switch (text_id) {
            case 0x1029:
                icon = AutoLifeIconRectangle;
                return true;
            case 0x101D:
                icon = HasteIconRectangle;
                return true;
            case 0x1016:
                icon = ProtectIconRectangle;
                return true;
            case 0x1015:
                icon = ShellIconRectangle;
                return true;
            case 0x1017:
                icon = ReflectIconRectangle;
                return true;
            case 0x101C:
                icon = RegenIconRectangle;
                return true;
            case 0x1014:
                icon = DarknessIconRectangle;
                return true;
            case 0x1013:
                icon = SilenceIconRectangle;
                return true;
            case 0x1012:
                icon = SleepIconRectangle;
                return true;
            case 0x101E:
                icon = SlowIconRectangle;
                return true;
            case 0x1009:
                icon = PoisonIconRectangle;
                return true;
            case 0x100E:
                icon = ConfuseIconRectangle;
                return true;
            case 0x100F:
                icon = BerserkIconRectangle;
                return true;
            case 0x1008:
                icon = PetrifyIconRectangle;
                return true;
            case 0x1007:
                icon = ZombieIconRectangle;
                return true;
            case 0x100A:
                icon = PowerBreakIconRectangle;
                return true;
            case 0x100B:
                icon = MagicBreakIconRectangle;
                return true;
            case 0x100C:
                icon = ArmorBreakIconRectangle;
                return true;
            case 0x100D:
                icon = MentalBreakIconRectangle;
                return true;
            default:
                icon = default;
                return false;
        }
    }
}
