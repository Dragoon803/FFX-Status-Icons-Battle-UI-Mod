// SPDX-License-Identifier: MIT

using Fahrenheit;
using static Fahrenheit.Mods.StatusIcons.BattleStatusWindowDefinitions;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    // Sensor's Info window cycles its own status label. Let the native function
    // advance and draw it first, then attach the icon for that exact label.
    private void h_TOBtlDrawInfoWinStatus(int character_id, int left, int top) {
        _TOBtlDrawInfoWinStatus.chain_from(h_TOBtlDrawInfoWinStatus).fnptr!(character_id, left, top);

        int index = FhUtil.get_at<short>(MonsterPreviewTextIndexOffset);
        int count = FhUtil.get_at<short>(MonsterPreviewTextCountOffset);

        if (index < 0 || index >= count) {
            return;
        }

        ushort text_id = FhUtil.get_at<ushort>(MonsterPreviewTextIdsOffset + index * sizeof(ushort));

        if (!try_get_status_preview_icon(text_id, out var icon)) {
            return;
        }

        byte* text = _MsMenuGetText(1, text_id, 1);

        if (text == null) {
            return;
        }

        float text_width = 0;
        _ToGetBtlEasyFontWidth(text, &text_width, 0, MonsterPreviewTextScale, 1f);

        if (!float.IsFinite(text_width) || text_width < 0) {
            return;
        }

        // Match native centering. Width is already in remapped screen units.
        float text_left = left + (_GraphicUiRemapX2(MonsterPreviewLineWidth) - text_width) * 0.5f;
        queue_status_icon(text_left + text_width, top, icon,
            Battle_UI_Settings.MonsterPreviewIconSize,
            Battle_UI_Settings.MonsterPreviewIconGap,
            Battle_UI_Settings.MoveMonsterPreviewIconUp);
    }
}
