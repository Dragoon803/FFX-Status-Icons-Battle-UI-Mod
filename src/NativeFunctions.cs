// SPDX-License-Identifier: MIT

using Fahrenheit;
using System;
using System.Runtime.InteropServices;

namespace Fahrenheit.Mods.StatusIcons;

public unsafe partial class StatusIconsMod {
    private static GraphicUiRemapX2 _GraphicUiRemapX2 = null!;
    private static GraphicUiRemapY2 _GraphicUiRemapY2 = null!;
    private static ToMakeBtlEasyEdgeDigitRight _ToMakeBtlEasyEdgeDigitRight = null!;
    private static ToMakeBtlEasyEdgeFont _ToMakeBtlEasyEdgeFont = null!;
    private static TOMkpShapeXYWHUV _TOMkpShapeXYWHUV = null!;
    private static TOMakePktScissor _TOMakePktScissor = null!;
    private static MsGetRamChrName _MsGetRamChrName = null!;
    private static TOGetFFXLang _TOGetFFXLang = null!;
    private static MsGetRamChrHPmax _MsGetRamChrHPmax = null!;
    private static MsGetRamChrMPmax _MsGetRamChrMPmax = null!;
    private static MsMenuGetText _MsMenuGetText = null!;
    private static ToMakeBtlEasyFont _ToMakeBtlEasyFont = null!;
    private static ToGetBtlEasyFontWidth _ToGetBtlEasyFontWidth = null!;
    private static TOMkpCrossEasyStrFontSClut _TOMkpCrossEasyStrFontSClut = null!;
    private static MsGetChr _MsGetChr = null!;
    private static TOBtlDrawStatusLimitGauge _TOBtlDrawStatusLimitGauge = null!;

    // Module-relative offsets for the executable used by the original UI.
    // Check these against your supported FFX build before changing versions.
    private static void load_native_functions() {
        _GraphicUiRemapX2 = get_native_function<GraphicUiRemapX2>(0x244990);
        _GraphicUiRemapY2 = get_native_function<GraphicUiRemapY2>(0x2449D0);
        _ToMakeBtlEasyEdgeDigitRight = get_native_function<ToMakeBtlEasyEdgeDigitRight>(0x505890);
        _ToMakeBtlEasyEdgeFont = get_native_function<ToMakeBtlEasyEdgeFont>(0x505930);
        _TOMkpShapeXYWHUV = get_native_function<TOMkpShapeXYWHUV>(0x503BB0);
        _TOMakePktScissor = get_native_function<TOMakePktScissor>(0x4FDEE0);
        _MsGetRamChrName = get_native_function<MsGetRamChrName>(0x39AF20);
        _TOGetFFXLang = get_native_function<TOGetFFXLang>(0x4AC2A0);
        _MsGetRamChrHPmax = get_native_function<MsGetRamChrHPmax>(0x39AE00);
        _MsGetRamChrMPmax = get_native_function<MsGetRamChrMPmax>(0x39AE80);
        _MsMenuGetText = get_native_function<MsMenuGetText>(0x38FD40);
        _ToMakeBtlEasyFont = get_native_function<ToMakeBtlEasyFont>(0x505AB0);
        _ToGetBtlEasyFontWidth = get_native_function<ToGetBtlEasyFontWidth>(0x505290);
        _TOMkpCrossEasyStrFontSClut = get_native_function<TOMkpCrossEasyStrFontSClut>(0x501660);
        _MsGetChr = get_native_function<MsGetChr>(0x394030);
        _TOBtlDrawStatusLimitGauge = get_native_function<TOBtlDrawStatusLimitGauge>(0x496050);
    }

    private static TDelegate get_native_function<TDelegate>(nint module_offset) where TDelegate : Delegate {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>((nint)FhUtil.ptr_at<byte>(module_offset));
    }
}
