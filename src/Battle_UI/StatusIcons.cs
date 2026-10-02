// SPDX-License-Identifier: MIT

using Fahrenheit;
using Fahrenheit.Events;
using Fahrenheit.Gui;
using Hexa.NET.ImGui;
using System;
using System.Numerics;
using System.Threading;
using static Fahrenheit.Mods.StatusIcons.BattleStatusWindowDefinitions;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    // This is a path INSIDE the game's archive, not a file beside our DLL.
    // Fahrenheit can load the FFX-2 archive while FFX is running.
    private readonly FhTexture _status_icons = new(
        "/FFX-2_Data/GameData/PS3Data/menu/D3D11/freetex.dds.phyre", FhTextureType.PHYRE);

    private static readonly Vector2 StatusTextureSize = new(1024, 768);

    // Rectangle in the original texture, measured in pixels. The exported PNG
    // has a different orientation. These use Fahrenheit's DDS convention,
    // just like its save-menu example; as_uv also handles the vertical flip.
    private static readonly Rect HasteIconRectangle = new() {
        pos = new(480, 160),
        size = new(80, 80)
    };

    private static readonly Rect ProtectIconRectangle = new() {
        pos = new(160, 160),
        size = new(80, 80)
    };

    private static readonly Rect ShellIconRectangle = new() {
        pos = new(80, 160),
        size = new(80, 80)
    };

    private static readonly Rect ReflectIconRectangle = new() {
        pos = new(240, 160),
        size = new(80, 80)
    };

    private static readonly Rect RegenIconRectangle = new() {
        pos = new(320, 160),
        size = new(80, 80)
    };

    private static readonly Rect AutoLifeIconRectangle = new() {
        pos = new(0, 160),
        size = new(80, 80)
    };

    private static readonly Rect DarknessIconRectangle = new() {
        pos = new(320, 240),
        size = new(80, 80)
    };

    private static readonly Rect SilenceIconRectangle = new() {
        pos = new(240, 240),
        size = new(80, 80)
    };

    private static readonly Rect SleepIconRectangle = new() {
        pos = new(160, 240),
        size = new(80, 80)
    };

    private static readonly Rect SlowIconRectangle = new() {
        pos = new(560, 160),
        size = new(80, 80)
    };

    private static readonly Rect PoisonIconRectangle = new() {
        pos = new(400, 240),
        size = new(80, 80)
    };

    private static readonly Rect PetrifyIconRectangle = new() {
        pos = new(80, 240),
        size = new(80, 80)
    };

    private static readonly Rect ZombieIconRectangle = new() {
        pos = new(240, 80),
        size = new(80, 80)
    };

    private static readonly Rect ConfuseIconRectangle = new() {
        pos = new(480, 240),
        size = new(80, 80)
    };

    private static readonly Rect BerserkIconRectangle = new() {
        pos = new(560, 240),
        size = new(80, 80)
    };

    private static readonly Rect PowerBreakIconRectangle = new() {
        pos = new(560, 80),
        size = new(80, 80)
    };

    private static readonly Rect MagicBreakIconRectangle = new() {
        pos = new(640, 80),
        size = new(80, 80)
    };

    private static readonly Rect ArmorBreakIconRectangle = new() {
        pos = new(720, 80),
        size = new(80, 80)
    };

    private static readonly Rect MentalBreakIconRectangle = new() {
        pos = new(800, 80),
        size = new(80, 80)
    };

    // Same order as the status values in draw_status_durations.
    private static readonly Rect[] StatusIconRectangles = [
        AutoLifeIconRectangle, HasteIconRectangle, ProtectIconRectangle,
        ShellIconRectangle, ReflectIconRectangle, RegenIconRectangle,
        DarknessIconRectangle, SilenceIconRectangle, SleepIconRectangle, SlowIconRectangle,
        PoisonIconRectangle, ConfuseIconRectangle, BerserkIconRectangle,
        PetrifyIconRectangle, ZombieIconRectangle, PowerBreakIconRectangle,
        MagicBreakIconRectangle, ArmorBreakIconRectangle, MentalBreakIconRectangle
    ];

    // FFX builds its HUD on the game thread. Fahrenheit draws ImGui on the
    // render thread. Pass only positions between them, never character pointers.
    private readonly record struct StatusIconDraw(
        Vector2 Position, Vector2 Size, Rect TextureRectangle, bool ShowIcon);

    // Up to 30 party icons plus one monster Info-window preview at the same time.
    private const int StatusIconsPerCharacter = 10;
    private const int MaximumStatusIcons = PartySlotCount * StatusIconsPerCharacter + 1;
    private readonly StatusIconDraw[] _pending_status_icons = new StatusIconDraw[MaximumStatusIcons];
    private readonly StatusIconDraw[] _visible_status_icons = new StatusIconDraw[MaximumStatusIcons];
    private readonly object _status_icon_lock = new();
    private int _pending_status_icon_count;
    private int _visible_status_icon_count;
    private bool _status_icons_ready;
    private bool _reported_status_icon_failure;
    private long _next_status_icon_load_attempt;
    private long _last_status_icon_use;

    private void begin_status_icon_frame(UpdateLoopEventArgs args) {
        // If the native HUD does not draw this update, publish an empty list.
        // This prevents icons from sticking around in menus or after battle.
        _pending_status_icon_count = 0;
        Array.Clear(_status_rows_drawn);
    }

    private void finish_status_icon_frame(UpdateLoopEventArgs args) {
        // A hidden row, preview mode, or battle exit must not retain its timer.
        for (int slot = 0; slot < PartySlotCount; slot++) {
            if (!_status_rows_drawn[slot]) {
                _status_page_timers[slot].reset();
                _status_page_characters[slot] = EmptyCharacterId;
            }
        }

        lock (_status_icon_lock) {
            Array.Copy(_pending_status_icons, _visible_status_icons, _pending_status_icon_count);
            _visible_status_icon_count = _pending_status_icon_count;
        }
    }

    // True means the icon is ready, so its optional turn count can be drawn.
    // Each request remembers its own picture; positioning is shared.
    private bool queue_status_icon(float left, float top, Rect texture_rectangle,
        float icon_size, float move_right, float move_up) {
        if (_pending_status_icon_count >= MaximumStatusIcons) {
            return false;
        }

        // Convert the already-remapped native coordinates back to fractions of
        // the 1920 x 1080 HUD. Ghidra confirms both remap functions are linear.
        float native_width = _GraphicUiRemapX2(1920f);
        float native_height = _GraphicUiRemapY2(1080f);

        if (native_width <= 0 || native_height <= 0) {
            return false;
        }

        Vector2 position = new(
            (left + _GraphicUiRemapX2(move_right)) / native_width,
            (top - _GraphicUiRemapY2(move_up)) / native_height);
        Vector2 size = new(icon_size / 1920f, icon_size / 1080f);
        bool show_icon = Volatile.Read(ref _status_icons_ready);
        _pending_status_icons[_pending_status_icon_count++] = new(position, size, texture_rectangle, show_icon);
        return show_icon;
    }

    public override void render_imgui() {
        // Copy under the lock, then release it before loading or drawing.
        // The fixed-size stack buffer avoids allocating an array every frame.
        Span<StatusIconDraw> icons = stackalloc StatusIconDraw[MaximumStatusIcons];
        int count;

        lock (_status_icon_lock) {
            count = _visible_status_icon_count;
            _visible_status_icons.AsSpan(0, count).CopyTo(icons);
        }

        long now = Environment.TickCount64;

        if (count == 0) {
            // Keep the texture through brief HUD interruptions. Fahrenheit
            // releases queued resources after the render frame has finished.
            if (now - _last_status_icon_use > 5000) {
                unload_status_icons();
            }

            return;
        }

        _last_status_icon_use = now;

        if (!load_status_icons(now)) {
            return;
        }

        if (!_status_icons.try_use(out ImTextureRef texture, out _)) {
            return;
        }

        // Fit the game's 16:9 HUD into the window, including letterbox space.
        Vector2 display_size = ImGui.GetIO().DisplaySize;
        float scale = MathF.Min(display_size.X / 1920f, display_size.Y / 1080f);
        Vector2 hud_size = new Vector2(1920, 1080) * scale;
        Vector2 hud_origin = (display_size - hud_size) / 2f;
        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        for (int i = 0; i < count; i++) {
            // Wait for a game-thread update that also knows the texture is ready,
            // keeping each icon and its native turn number on the same frame.
            if (icons[i].ShowIcon) {
                draw_status_icon(draw, texture, hud_origin + icons[i].Position * hud_size,
                    icons[i].Size * hud_size, icons[i].TextureRectangle);
            }
        }
    }

    private bool load_status_icons(long now) {
        if (Volatile.Read(ref _status_icons_ready)) {
            return true;
        }

        if (now < _next_status_icon_load_attempt) {
            return false;
        }

        _next_status_icon_load_attempt = now + 1000; // Retry without hammering the loader.

        try {
            if (FhApi.Resources.load_game_texture_2d(_status_icons)
                && _status_icons.try_use(out _, out FhTextureMetadata? metadata)
                && metadata.width == 1024 && metadata.height == 768) {
                Volatile.Write(ref _status_icons_ready, true);
                return true;
            }
        }
        catch (Exception error) {
            if (!_reported_status_icon_failure) {
                _logger.Warning($"Status icons could not load: {error.Message}");
            }
        }

        if (!_reported_status_icon_failure) {
            _logger.Warning("Status icons unavailable; hiding status entries and retrying once per second.");
        }

        _reported_status_icon_failure = true;
        return false;
    }

    private static void draw_status_icon(ImDrawListPtr draw, ImTextureRef texture,
        Vector2 position, Vector2 size, Rect texture_rectangle) {
        // UV coordinates are fractions of the whole sheet: 0 = start, 1 = end.
        UV crop = texture_rectangle.as_uv(StatusTextureSize);
        draw.AddImage(texture, position, position + size, crop.p0, crop.p1);
    }

    private void unload_status_icons() {
        Volatile.Write(ref _status_icons_ready, false);
        FhApi.Resources.unload_texture(_status_icons);
    }
}
