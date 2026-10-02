// SPDX-License-Identifier: MIT

using Fahrenheit;
using static Fahrenheit.Mods.StatusIcons.BattleStatusWindowDefinitions;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    // Port of FFX.exe VA 0x008963C0. Preserve all four modes and draw order.
    // Sources: BattleStatusWindow.txt and StatusDrawingAbi.txt. Assembly supplies
    // texture coordinates and stack arguments omitted by the decompiler.
    // To move the UI, edit Battle_UI_Settings.cs. No renderer edits are needed.
    private void h_TOBtlDrawStatusWin() {
        _TOMakePktScissor(0, 0, ScissorWidth, ScissorHeight);
        ushort* characters = FhUtil.ptr_at<ushort>(DisplayedCharacterIdsOffset);
        BattleStatusRowDisplay* rows = FhUtil.ptr_at<BattleStatusRowDisplay>(RowDisplayDataOffset);

        for (int slot = 0; slot < PartySlotCount; slot++) {
            ushort character = characters[slot];

            if (character == EmptyCharacterId) {
                continue;
            }

            float x = _GraphicUiRemapX2(PanelX);
            float y = _GraphicUiRemapY2(PanelY + slot * CharacterSpacing);
            BattleStatusRowDisplay* row = rows + slot;
            BattleStatusWindowMode mode = (BattleStatusWindowMode)FhUtil.get_at<byte>(DisplayModeOffset);

            switch (mode) {
                case BattleStatusWindowMode.Normal:
                    if (FhUtil.get_at<short>(CurrentCharacterOffset) == character) {
                        draw_native_status_highlight(x, y, false);
                    }

                    // Draw the normal FFX overdrive bar. A host mod can adapt
                    // this call if it also supplies a custom cost preview.
                    _TOBtlDrawStatusLimitGauge(row->DisplayedOverdriveParts,
                        x + _GraphicUiRemapX2(OverdriveX), y + _GraphicUiRemapY2(OverdriveY),
                        _GraphicUiRemapX2(OverdriveWidth), _GraphicUiRemapY2(OverdriveHeight));
                    draw_native_status_name(character, row, x, y);

                    draw_hp_or_mp_label(false, false, x, y);
                    draw_hp_or_mp_number(row->DisplayedHp, row->HpAndNameColorIndex, HpNumberRightX, x, y);

                    draw_hp_or_mp_label(true, false, x, y);
                    draw_hp_or_mp_number(row->DisplayedMp, row->MpColorIndex, MpNumberRightX, x, y);

                    draw_status_durations(slot, character, x, y);
                    break;

                case BattleStatusWindowMode.HpPreview:
                case BattleStatusWindowMode.MpPreview:
                    draw_native_status_preview_highlight(character, x, y);
                    draw_native_status_name(character, row, x, y);
                    bool is_mp = mode == BattleStatusWindowMode.MpPreview;
                    draw_hp_or_mp_label(is_mp, true, x, y);
                    // Native MP preview also uses the HP/name palette.
                    draw_hp_or_mp_number(is_mp ? row->DisplayedMp : row->DisplayedHp,
                        row->HpAndNameColorIndex, PreviewCurrentRightX, x, y);
                    _TOMkpCrossEasyStrFontSClut(FhUtil.ptr_at<byte>(PreviewSlashTextOffset),
                        x + _GraphicUiRemapX2(PreviewSlashX), y + _GraphicUiRemapY2(PreviewSlashY),
                        0, NumberScale, NumberSpacing);
                    draw_hp_or_mp_number(is_mp ? _MsGetRamChrMPmax(character) : _MsGetRamChrHPmax(character),
                        0, PreviewMaximumRightX, x, y);
                    break;

                case BattleStatusWindowMode.TextPreview:
                    draw_native_status_preview_highlight(character, x, y);
                    draw_native_status_name(character, row, x, y);

                    if (row->TextCount != 0) {
                        short previous_countdown = row->TextCycleCountdown;
                        row->TextCycleCountdown = unchecked((short)(previous_countdown - 1));

                        if (previous_countdown < 1) {
                            row->TextCycleIndex = unchecked((short)(row->TextCycleIndex + 1));
                            row->TextCycleCountdown = PreviewTextCycleReset;

                            if (row->TextCycleIndex >= row->TextCount) {
                                row->TextCycleIndex = 0;
                            }
                        }

                        ushort* text_ids = FhUtil.ptr_at<ushort>(TextIdsOffset + slot * RowDisplayStride);
                        ushort text_id = text_ids[row->TextCycleIndex];
                        byte* text = _MsMenuGetText(1, text_id, 1);
                        draw_status_preview(text_id, text, x + _GraphicUiRemapX2(PreviewTextX),
                            y + _GraphicUiRemapY2(PreviewTextY));
                    }

                    break;
            }
        }
    }

    // Texture coordinates remain native data. Layout is supplied separately.
    private static void draw_status_texture(uint shape, float x, float y, float width, float height,
        StatusTextureUvOffsets texture) {
        _TOMkpShapeXYWHUV(shape, x, y, width, height,
            FhUtil.get_at<float>(texture.Left), FhUtil.get_at<float>(texture.Top),
            FhUtil.get_at<float>(texture.Right), FhUtil.get_at<float>(texture.Bottom));
    }

    private static void draw_native_status_preview_highlight(ushort character, float x, float y) {
        if ((FhUtil.get_at<uint>(PreviewSelectionMaskOffset) & (1u << (character & 31))) != 0) {
            draw_native_status_highlight(x, y, true);
        }
    }

    private static void draw_native_status_highlight(float x, float y, bool is_preview) {
        float horizontal_offset = is_preview ? PreviewHighlightX : HighlightX;
        float vertical_offset = is_preview ? PreviewHighlightY : HighlightY;
        float height = is_preview ? Battle_UI_Settings.GreenBarHeight : Battle_UI_Settings.PurpleBarHeight;
        draw_status_texture(ExplicitUvShape, x + _GraphicUiRemapX2(horizontal_offset),
            y + _GraphicUiRemapY2(vertical_offset), _GraphicUiRemapX2(HighlightWidth), _GraphicUiRemapY2(height),
            is_preview ? PreviewHighlightTexture : ActiveHighlightTexture);
    }

    private static void draw_native_status_name(ushort character, BattleStatusRowDisplay* row, float x, float y) {
        _ToMakeBtlEasyEdgeFont(_MsGetRamChrName(character), x + _GraphicUiRemapX2(NameX),
            y + _GraphicUiRemapY2(NameY), row->HpAndNameColorIndex, NameScale, NameSpacing);
    }

    private static void draw_hp_or_mp_number(int value, uint color_index, float number_right_x, float x, float y) {
        _ToMakeBtlEasyEdgeDigitRight(value, x + _GraphicUiRemapX2(number_right_x),
            y + _GraphicUiRemapY2(NumberY), color_index, NumberScale, NumberSpacing);
    }

    private static void draw_hp_or_mp_label(bool is_mp, bool is_preview, float x, float y) {
        bool alternate_language = _TOGetFFXLang() == AlternateLabelLanguage;
        float label_offset_x = is_preview ? PreviewLabelX : is_mp ? MpLabelX : HpLabelX;
        float label_x, label_y, width, height;
        StatusTextureUvOffsets texture;

        if (alternate_language) {
            label_x = (float)(((double)_GraphicUiRemapX2(label_offset_x) + x) +
                (is_mp ? AlternateMpLabelXAdjustment : AlternateHpLabelXAdjustment));
            label_y = (float)(((double)_GraphicUiRemapY2(LabelY) + y) + AlternateLabelYAdjustment);
            width = _GraphicUiRemapX2(is_mp ? AlternateMpLabelWidth : AlternateHpLabelWidth);
            height = _GraphicUiRemapY2(AlternateLabelHeight);
            texture = is_mp ? AlternateMpLabelTexture : AlternateHpLabelTexture;
        }
        else {
            label_x = x + _GraphicUiRemapX2(label_offset_x);
            label_y = y + _GraphicUiRemapY2(LabelY);
            width = _GraphicUiRemapX2(is_mp ? MpLabelWidth : HpLabelWidth);
            height = _GraphicUiRemapY2(LabelHeight);
            texture = is_mp ? MpLabelTexture : HpLabelTexture;
        }

        draw_status_texture(is_mp ? MpLabelShape : HpLabelShape, label_x, label_y, width, height, texture);
    }
}
