// SPDX-License-Identifier: MIT

using System.Runtime.InteropServices;

namespace Fahrenheit.Mods.StatusIcons;

internal enum BattleStatusWindowMode : byte {
    HpPreview = 0,
    MpPreview = 1,
    TextPreview = 2,
    Normal = 3
}

// Offsets of the native texture coordinates, not screen positions.
// UV coordinates describe which rectangle to use inside a texture sheet.
internal readonly record struct StatusTextureUvOffsets(int Left, int Top, int Right, int Bottom);

// Native TOBtlCtrlStatusWin owns cached HP/MP/gauge values. The renderer owns
// the text-preview countdown/index, matching the native draw function.
// First entry: VA 0x0133F7A4; subsequent entries have a 0x90-byte stride.
[StructLayout(LayoutKind.Explicit, Size = 0x90)]
internal struct BattleStatusRowDisplay {
    // Color indices select a palette in the native renderer (called CLUT in FFX).
    [FieldOffset(0x04)] public int DisplayedHp;
    [FieldOffset(0x0D)] public byte HpAndNameColorIndex;
    [FieldOffset(0x1C)] public int DisplayedMp;
    [FieldOffset(0x25)] public byte MpColorIndex;
    [FieldOffset(0x2C)] public int DisplayedOverdriveParts;
    [FieldOffset(0x30)] public short TextCycleCountdown;
    [FieldOffset(0x32)] public short TextCycleIndex;
    [FieldOffset(0x34)] public short TextCount;
}

internal static class BattleStatusWindowDefinitions {
    // TOBtlDrawInfoWinStatus (008951B0), verified in InfoStatusLayout.txt.
    public const int MonsterPreviewTextIndexOffset = 0xF3F584;
    public const int MonsterPreviewTextCountOffset = 0xF3F586;
    public const int MonsterPreviewTextIdsOffset = 0xF3F588;
    public const float MonsterPreviewLineWidth = 380f;
    public const float MonsterPreviewTextScale = 0.78f;

    // IMPLEMENTATION DETAILS. For everyday edits, use Battle_UI_Settings.cs.
    // These translate its simple adjustments into native layout coordinates.
    // X: smaller = left, larger = right. Y: smaller = up, larger = down.
    // Values are internal UI units, scaled by the renderer, not screen pixels.
    // Element positions are offsets from each character row. All rows share them.
    // Example: NameY = -20f moves names 16 units above their vanilla position.
    // Rebuild and load the updated DLL after editing. No hot reload is provided.

    // Overall panel. Row Y = PanelY + slot * CharacterSpacing.
    public const float PanelX = 1231f + Battle_UI_Settings.MoveWholePanelRight;
    public const float PanelY = 835f - Battle_UI_Settings.MoveWholePanelUp;
    public const float CharacterSpacing = 52f + Battle_UI_Settings.ExtraSpaceBetweenCharacters;

    // Character name.
    public const float NameX = 21f + Battle_UI_Settings.MoveNamesRight;
    public const float NameY = -4f - Battle_UI_Settings.MoveNamesUp;
    public const float NameScale = 0.68f;
    public const float NameSpacing = 1f;

    // Normal-mode HP/MP labels (graphics) and right-aligned numbers.
    public const float HpLabelX = 240f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float HpNumberRightX = 395f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float MpLabelX = 420f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float MpNumberRightX = 540f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float LabelY = 5f - Battle_UI_Settings.MoveHpAndMpUp;
    public const float HpLabelWidth = 61.5f;
    public const float MpLabelWidth = 49f;
    public const float LabelHeight = 23f;
    public const float NumberY = -2f - Battle_UI_Settings.MoveHpAndMpUp;
    public const float NumberScale = 0.65f;
    public const float NumberSpacing = 0f;

    // Overdrive bar. Independent of the name position.
    public const float OverdriveX = 21f + Battle_UI_Settings.MoveOverdriveBarsRight;
    public const float OverdriveY = 35f - Battle_UI_Settings.MoveOverdriveBarsUp;
    public const float OverdriveWidth = 520f;
    public const float OverdriveHeight = 8f;

    // Background selection highlight. Height is independent of character spacing.
    public const float HighlightX = -65f + Battle_UI_Settings.MovePurpleBarRight;
    public const float HighlightY = -2f - Battle_UI_Settings.MovePurpleBarUp;
    public const float PreviewHighlightX = -65f + Battle_UI_Settings.MoveGreenBarRight;
    public const float PreviewHighlightY = -2f - Battle_UI_Settings.MoveGreenBarUp;
    public const float HighlightWidth = 680f;

    // HP/MP preview: label, current value, slash, maximum value.
    public const float PreviewLabelX = 240f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float PreviewCurrentRightX = 395f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float PreviewSlashX = 420f + Battle_UI_Settings.MoveHpAndMpRight;
    public const float PreviewSlashY = 6f - Battle_UI_Settings.MoveHpAndMpUp;
    public const float PreviewMaximumRightX = 540f + Battle_UI_Settings.MoveHpAndMpRight;

    // Cycling status text preview; the countdown is draw calls, not battle turns.
    public const float PreviewTextX = 279f;
    public const float PreviewTextY = -21f;
    public const float PreviewTextScale = 0.78f;
    public const float PreviewTextSpacing = 1f;
    public const short PreviewTextCycleReset = 20;

    // Language 0xB has different label graphics. These adjustments are applied
    // AFTER remapping, preserving the native unscaled offsets.
    public const float AlternateHpLabelWidth = 68f;
    public const float AlternateMpLabelWidth = 66.4f;
    public const float AlternateLabelHeight = 24f;
    public const double AlternateHpLabelXAdjustment = -3.0;
    public const double AlternateMpLabelXAdjustment = -5.0;
    public const double AlternateLabelYAdjustment = -2.0;

    // NATIVE DATA / TEXTURES -- not position settings.
    // Memory offsets are relative to FFX.exe. Leave these alone when editing layout.
    public const int DisplayModeOffset = 0xF3D742;
    public const int DisplayedCharacterIdsOffset = 0xF3F76C; // ushort[3]
    public const int RowDisplayDataOffset = 0xF3F7A4;
    public const int PreviewSelectionMaskOffset = 0xF3D138;
    public const int CurrentCharacterOffset = 0x1FCC088; // signed short (toNow)
    public const int TextIdsOffset = 0xF3F7DA;
    public const int PreviewSlashTextOffset = 0x78F87C;
    public const int PartySlotCount = 3;
    public const ushort EmptyCharacterId = 0xFF;
    public const int RowDisplayStride = 0x90;
    public const int AlternateLabelLanguage = 0xB;
    public static readonly StatusTextureUvOffsets ActiveHighlightTexture = new(0x75F010, 0x75EEE4, 0x75F034, 0x75EEFC);
    public static readonly StatusTextureUvOffsets PreviewHighlightTexture = new(0x75F02C, 0x75EF84, 0x75F048, 0x75EF9C);
    public static readonly StatusTextureUvOffsets HpLabelTexture = new(0x75EDA0, 0x75ED60, 0x75EDB4, 0x75ED64);
    public static readonly StatusTextureUvOffsets MpLabelTexture = new(0x75EDCC, 0x75ED60, 0x75EDDC, 0x75ED64);
    public static readonly StatusTextureUvOffsets AlternateHpLabelTexture = new(0x75EDA4, 0x75ED5C, 0x75EDBC, 0x75ED68);
    public static readonly StatusTextureUvOffsets AlternateMpLabelTexture = new(0x75EDC8, 0x75ED5C, 0x75EDE8, 0x75ED68);
    public const uint HpLabelShape = 0xE9;
    public const uint MpLabelShape = 0xEA;
    public const uint ExplicitUvShape = 0xFFFFFFFF;
    public const int ScissorWidth = 0x200;
    public const int ScissorHeight = 0x1A0;
}
