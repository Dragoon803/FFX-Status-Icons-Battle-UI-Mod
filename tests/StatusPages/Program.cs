using Fahrenheit.Mods.StatusIcons;

var timer = new StatusPageTimer();
int checks = 0;
void Check(int expected, bool first, bool second, long time) {
    int actual = timer.select_page(first, second, time, 3000);
    if (actual != expected) throw new Exception($"At {time}: expected page {expected}, got {actual}");
    checks++;
}
Check(-1, false, false, 0);
Check(0, true, false, 100);
Check(0, true, false, 20000); // A single page never cycles.
Check(1, false, true, 21000);
Check(1, false, true, 40000);
Check(0, true, true, 41000); // Start page 1 when both become occupied.
Check(0, true, true, 43999);
Check(1, true, true, 44000); // Exactly three seconds.
Check(1, true, true, 46999);
Check(0, true, true, 47000);
Check(1, true, true, 50000);
Check(0, true, false, 50001); // Current page empties: switch immediately.
Check(0, true, true, 51000); // Reappearance gives page 1 a full interval.
Check(1, true, true, 54000);
Check(-1, false, false, 54001);
Check(0, true, true, 55000);
Check(1, true, true, 64000); // A delayed draw advances by elapsed intervals.
timer.reset(); // Hidden HUD, party replacement, or new battle.
Check(0, true, true, 65000);
var otherCharacter = new StatusPageTimer();
if (otherCharacter.select_page(true, true, 68000, 3000) != 0) throw new Exception("Independent timer failed");
Check(1, true, true, 68000);
Console.WriteLine($"PASS: {checks + 1} page-selection checks, including independent characters.");
