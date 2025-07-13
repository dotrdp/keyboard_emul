@echo off
echo Testing keybind_hold with trace logging...
echo Run this in SMAPI console:
echo.
echo keybind_hold a 200
echo.
echo Look for:
echo - [TRACE] KeybindManager.Update() called - HeldKeys.Count: X
echo - [DEBUG] Update called - HeldKeys: X, ExpiredKeys: X
echo - [DEBUG] Key A expires at XX:XX:XX.XXX, expired: false/true
echo - [DEBUG] Normal timer releasing expired key: A
echo.
echo If you don't see Update trace logs, the game update loop isn't calling KeybindManager.Update()
echo If you see phantom keystrokes like "1234567890eimj", we need to find where they're coming from
pause
