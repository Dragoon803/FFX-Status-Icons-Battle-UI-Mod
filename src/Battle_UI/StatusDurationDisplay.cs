// SPDX-License-Identifier: MIT

using Fahrenheit;
using Fahrenheit.FFX;
using System;
using System.Globalization;
using System.Text;
using static Fahrenheit.Mods.StatusIcons.BattleStatusWindowDefinitions;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    // Each character has an independent page timer. Battle memory supplies the
    // durations; drawing the HUD never advances a status's remaining turns.
    private readonly StatusPageTimer[] _status_page_timers = [new(), new(), new()];
    private readonly ushort[] _status_page_characters = [EmptyCharacterId, EmptyCharacterId, EmptyCharacterId];
    private readonly bool[] _status_rows_drawn = new bool[PartySlotCount];
    private static readonly byte[]?[] _status_number_labels = new byte[254][];
    private static byte[]? _status_infinity_label;
    private static FhLangId? _status_label_language;

    private void draw_status_durations(int slot, ushort character_id, float row_x, float row_y) {
        var character = _MsGetChr(character_id);

        if (character == null) {
            return;
        }

        _status_rows_drawn[slot] = true;

        if (_status_page_characters[slot] != character_id) {
            _status_page_timers[slot].reset();
            _status_page_characters[slot] = character_id;
        }

        var turns = character->ram.status_suffer_turns_left;
        var permanent = character->ram.status_suffer;
        var extra = character->ram.status_suffer_extra;

        // Order matches StatusIconRectangles in StatusIcons.cs.
        // Timed effects: 0 = absent; 1..253 = turns; 254/255 = unlimited.
        // Permanent flags have no turn counter: represent presence as 255.
        Span<byte> statuses = stackalloc byte[] {
            // PAGE 1: positive effects followed by four timed ailments.
            status_presence((extra & StatusExtraFlags.AUTO_LIFE) != 0),
            turns.haste,
            turns.protect,
            turns.shell,
            turns.reflect,
            turns.regen,
            turns.darkness,
            turns.silence,
            turns.sleep,
            turns.slow,

            // PAGE 2: permanent ailments and breaks.
            status_presence((permanent & StatusPermanentFlags.POISON) != 0),
            status_presence((permanent & StatusPermanentFlags.CONFUSION) != 0),
            status_presence((permanent & StatusPermanentFlags.BERSERK) != 0),
            status_presence((permanent & StatusPermanentFlags.PETRIFICATION) != 0),
            status_presence((permanent & StatusPermanentFlags.ZOMBIE) != 0),
            status_presence((permanent & StatusPermanentFlags.POWER_BREAK) != 0),
            status_presence((permanent & StatusPermanentFlags.MAGIC_BREAK) != 0),
            status_presence((permanent & StatusPermanentFlags.ARMOR_BREAK) != 0),
            status_presence((permanent & StatusPermanentFlags.MENTAL_BREAK) != 0)
        };

        int page = _status_page_timers[slot].select_page(has_active_status(statuses[..10]),
            has_active_status(statuses[10..]), Environment.TickCount64,
            (long)(Battle_UI_Settings.StatusPageSeconds * 1000));

        if (page < 0) {
            return;
        }

        float left = row_x + _GraphicUiRemapX2(NameX + Battle_UI_Settings.MoveStatusLineRight);
        float top = row_y + _GraphicUiRemapY2(NameY - 28f - Battle_UI_Settings.MoveStatusLineUp);
        int start = page == 0 ? 0 : 10;
        int end = page == 0 ? 10 : statuses.Length;
        int visible_position = 0;

        for (int index = start; index < end; index++) {
            int remaining = statuses[index];

            if (remaining == 0) {
                continue;
            }

            float icon_left = left + _GraphicUiRemapX2(visible_position * Battle_UI_Settings.StatusIconSpacing);
            var adjustment = Battle_UI_Settings.get_status_adjustment(index);
            icon_left += _GraphicUiRemapX2(adjustment.Right);
            float status_top = top - _GraphicUiRemapY2(adjustment.Up);
            visible_position++;
            bool ready = queue_status_icon(icon_left, status_top, StatusIconRectangles[index],
                Battle_UI_Settings.StatusIconSize, Battle_UI_Settings.MoveStatusIconsRight,
                Battle_UI_Settings.MoveStatusIconsUp);

            // No words or orphaned numbers while the texture is loading.
            // Only Haste through Slow have turn counters. Auto-Life and page 2
            // use presence flags, so they show neither a number nor infinity.
            bool has_turn_counter = index >= 1 && index <= 9;

            if (!ready || !has_turn_counter) {
                continue;
            }

            float number_left = icon_left + _GraphicUiRemapX2(
                Battle_UI_Settings.MoveStatusIconsRight + Battle_UI_Settings.StatusIconSize
                + Battle_UI_Settings.StatusDurationGap);
            draw_status_duration(remaining, number_left, status_top);
        }

        // Never change battle data. The game owns expiration and removal.
    }

    private static byte status_presence(bool active) {
        return active ? (byte)255 : (byte)0;
    }

    private static bool has_active_status(ReadOnlySpan<byte> statuses) {
        foreach (byte status in statuses) {
            if (status != 0) {
                return true;
            }
        }

        return false;
    }

    private static void draw_status_duration(int turns, float left, float top) {
        FhLangId language = FhGlobal.lang_id;

        if (_status_label_language != language) {
            Array.Clear(_status_number_labels);
            _status_infinity_label = null;
            _status_label_language = language;
        }

        bool unlimited = turns == 254 || turns == 255;
        byte[] label = unlimited
            ? (_status_infinity_label ??= encode_status_duration_label("∞", language))
            : (_status_number_labels[turns] ??= encode_status_duration_label(
                turns.ToString(CultureInfo.InvariantCulture), language));
        // Three digits need a smaller font to stay inside the 50-unit slot.
        float text_size = !unlimited && turns >= 100 ? 0.33f : 0.5f;

        fixed (byte* text = label) {
            _ToMakeBtlEasyEdgeFont(text, left, top, 0, text_size, 0f);
        }
    }

    private static byte[] encode_status_duration_label(string text, FhLangId language) {
        // Native FFX text is not a normal C# string. Encode it for the current
        // language and leave one zero byte at the end to terminate the label.
        ReadOnlySpan<byte> source = Encoding.UTF8.GetBytes(text);
        const FhEncodingFlags flags = FhEncodingFlags.IMPLICIT_CJK_EXTENSION;
        int length = FhEncoding.compute_encode_buffer_size(source, language, FhGameId.FFX, flags);
        byte[] encoded = new byte[length + 1];
        FhEncoding.encode(source, encoded, language, FhGameId.FFX, flags);
        return encoded;
    }
}
