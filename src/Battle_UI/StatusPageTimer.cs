// SPDX-License-Identifier: MIT
namespace Fahrenheit.Mods.StatusIcons;

// No game-memory access: the timer can be tested without FFX.
internal sealed class StatusPageTimer {
    private int _page;
    private bool _cycling;
    private long _page_started_at_ms;

    public void reset() {
        _page = 0;
        _cycling = false;
        _page_started_at_ms = 0;
    }

    // -1 = nothing; 0 = page 1; 1 = page 2.
    public int select_page(bool first_page_has_statuses, bool second_page_has_statuses,
        long current_time_ms, long page_interval_ms) {
        if (!first_page_has_statuses || !second_page_has_statuses) {
            reset();
            return first_page_has_statuses ? 0 : second_page_has_statuses ? 1 : -1;
        }

        if (!_cycling || current_time_ms < _page_started_at_ms) {
            _cycling = true;
            _page_started_at_ms = current_time_ms;
            _page = 0;
        }

        page_interval_ms = System.Math.Max(1, page_interval_ms);
        long elapsed_intervals = (current_time_ms - _page_started_at_ms) / page_interval_ms;

        if (elapsed_intervals > 0) {
            // An odd number of intervals switches pages; an even number returns
            // to the same page. This also handles a delayed frame.
            if ((elapsed_intervals & 1) != 0) {
                _page = 1 - _page;
            }

            _page_started_at_ms += elapsed_intervals * page_interval_ms;
        }

        return _page;
    }
}
