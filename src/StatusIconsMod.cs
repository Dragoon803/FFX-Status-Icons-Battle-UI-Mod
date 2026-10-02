// SPDX-License-Identifier: MIT

using Fahrenheit;
using System.IO;

namespace Fahrenheit.Mods.StatusIcons;

[FhLoad(FhGameId.FFX)]
public unsafe partial class StatusIconsMod : FhModule {
    private FhMethodHandle<TOBtlDrawStatusWin> _TOBtlDrawStatusWin {
        get { return new(new FhMethodLocation("FFX.exe", 0x4963C0)); }
    }

    private FhMethodHandle<TOBtlDrawInfoWinStatus> _TOBtlDrawInfoWinStatus {
        get { return new(new FhMethodLocation("FFX.exe", 0x4951B0)); }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        load_native_functions();

        if (!_TOBtlDrawStatusWin.hook(this, h_TOBtlDrawStatusWin)) {
            return false;
        }

        if (!_TOBtlDrawInfoWinStatus.hook(this, h_TOBtlDrawInfoWinStatus)) {
            return false;
        }

        if (!FhApi.Events.Common.GameLoop.PreUpdate.subscribe(begin_status_icon_frame)) {
            return false;
        }

        return FhApi.Events.Common.GameLoop.PostUpdate.subscribe(finish_status_icon_frame);
    }
}
