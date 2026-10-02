// SPDX-License-Identifier: MIT

using Fahrenheit.FFX;
using Fahrenheit.FFX.Battle;
using System.Runtime.InteropServices;

namespace Fahrenheit.Mods.StatusIcons;

// Native FFX names are retained to match Fahrenheit and Ghidra.
// Calling conventions and parameter order must match the game executable.

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate float GraphicUiRemapX2(float x);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate float GraphicUiRemapY2(float y);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ToMakeBtlEasyEdgeDigitRight(int value, float right_x, float y, uint clut, float scale, float spacing);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ToMakeBtlEasyEdgeFont(byte* text, float x, float y, uint clut, float scale, float spacing);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void TOMkpShapeXYWHUV(uint shape_id, float x, float y, float width, float height,
    float u0, float v0, float u1, float v1);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void TOMakePktScissor(int x, int y, int width, int height);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate byte* MsGetRamChrName(int chr_id);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int TOGetFFXLang();

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int MsGetRamChrHPmax(int chr_id);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int MsGetRamChrMPmax(int chr_id);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate byte* MsMenuGetText(int bank, uint text_id, uint variant);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ToMakeBtlEasyFont(byte* text, float x, float y, uint clut, float scale, float spacing);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ToGetBtlEasyFontWidth(byte* text, float* width, uint clut, float scale, float spacing);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void TOMkpCrossEasyStrFontSClut(byte* text, float x, float y, uint clut, float scale, float spacing);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate Chr* MsGetChr(int chr_id);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void TOBtlDrawStatusWin();

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void TOBtlDrawInfoWinStatus(int character_id, int left, int top);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void TOBtlDrawStatusLimitGauge(int current_parts, float left, float top, float width, float height);
