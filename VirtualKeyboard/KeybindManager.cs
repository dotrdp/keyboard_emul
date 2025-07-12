using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualKeyboard.Inputs;

namespace VirtualKeyboard
{
    /// <summary>
    /// Manages virtual keybind states and timing - TASMod approach
    /// </summary>
    public static class KeybindManager
    {
        private static readonly Dictionary<SButton, DateTime> _heldKeys = new();
        private static readonly Dictionary<SButton, DateTime> _pressedKeys = new();

        /// <summary>
        /// Whether any virtual keybinds are currently active
        /// </summary>
        public static bool HasActiveKeybinds => _heldKeys.Any() || _pressedKeys.Any();

        /// <summary>
        /// Press a key for one frame
        /// </summary>
        public static void PressKey(SButton key)
        {
            _pressedKeys[key] = DateTime.Now.AddMilliseconds(50); // Brief press
            UpdateVirtualInput();
            ModEntry.Logger?.Log($"KeybindManager: Pressing key {key} for 50ms", LogLevel.Debug);
        }

        /// <summary>
        /// Hold a key for a duration
        /// </summary>
        public static void HoldKey(SButton key, int durationMs = 1000)
        {
            _heldKeys[key] = DateTime.Now.AddMilliseconds(durationMs);
            UpdateVirtualInput();
            ModEntry.Logger?.Log($"KeybindManager: Holding key {key} for {durationMs}ms", LogLevel.Debug);
        }

        /// <summary>
        /// Release a specific key
        /// </summary>
        public static void ReleaseKey(SButton key)
        {
            _heldKeys.Remove(key);
            _pressedKeys.Remove(key);
            UpdateVirtualInput();
            ModEntry.Logger?.Log($"KeybindManager: Released key {key}", LogLevel.Debug);
        }

        /// <summary>
        /// Check if a key is currently held virtually
        /// </summary>
        public static bool IsKeyHeld(SButton key)
        {
            var now = DateTime.Now;
            
            // Check if key is in pressed state and still active
            if (_pressedKeys.TryGetValue(key, out DateTime pressedEnd) && now <= pressedEnd)
            {
                return true;
            }
            
            // Check if key is in held state and still active
            if (_heldKeys.TryGetValue(key, out DateTime heldEnd) && now <= heldEnd)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Clear all virtual keybinds
        /// </summary>
        public static void ClearAll()
        {
            _heldKeys.Clear();
            _pressedKeys.Clear();
            VirtualInputState.Reset();
            ModEntry.Logger?.Log("KeybindManager: Cleared all virtual keybinds", LogLevel.Debug);
        }

        /// <summary>
        /// Update and cleanup expired keybinds
        /// </summary>
        public static void Update()
        {
            var now = DateTime.Now;
            var expiredPressed = _pressedKeys.Where(kvp => now > kvp.Value).Select(kvp => kvp.Key).ToList();
            var expiredHeld = _heldKeys.Where(kvp => now > kvp.Value).Select(kvp => kvp.Key).ToList();

            bool hasChanges = false;

            foreach (var key in expiredPressed)
            {
                _pressedKeys.Remove(key);
                hasChanges = true;
                ModEntry.Logger?.Log($"KeybindManager: Expired pressed key {key}", LogLevel.Trace);
            }

            foreach (var key in expiredHeld)
            {
                _heldKeys.Remove(key);
                hasChanges = true;
                ModEntry.Logger?.Log($"KeybindManager: Expired held key {key}", LogLevel.Trace);
            }

            if (hasChanges)
            {
                UpdateVirtualInput();
            }
        }

        /// <summary>
        /// Get all currently active virtual keys
        /// </summary>
        public static List<SButton> GetActiveKeys()
        {
            var now = DateTime.Now;
            var activeKeys = new List<SButton>();

            // Add all pressed keys that are still active
            activeKeys.AddRange(_pressedKeys.Where(kvp => now <= kvp.Value).Select(kvp => kvp.Key));
            
            // Add all held keys that are still active
            activeKeys.AddRange(_heldKeys.Where(kvp => now <= kvp.Value).Select(kvp => kvp.Key));

            return activeKeys.Distinct().ToList();
        }

        /// <summary>
        /// Update the VirtualInputState with current active keys
        /// </summary>
        private static void UpdateVirtualInput()
        {
            var activeKeys = GetActiveKeys();
            
            // Convert SButton to Keys where possible
            var keys = new List<Keys>();
            foreach (var sButton in activeKeys)
            {
                if (TryConvertSButtonToKeys(sButton, out Keys key))
                {
                    keys.Add(key);
                }
            }

            // Update virtual input state
            VirtualInputState.ClearKeys();
            if (keys.Any())
            {
                VirtualInputState.AddKeys(keys);
                ModEntry.Logger?.Log($"VirtualInputState: Updated with keys [{string.Join(", ", keys)}]", LogLevel.Trace);
            }
        }

        /// <summary>
        /// Convert SButton to Keys enum
        /// </summary>
        public static bool TryConvertSButtonToKeys(SButton sButton, out Keys keys)
        {
            keys = default;

            // Handle keyboard keys (most common case)
            if (sButton.ToString().StartsWith("Keys."))
            {
                var keyName = sButton.ToString().Substring(5); // Remove "Keys." prefix
                if (Enum.TryParse<Keys>(keyName, out keys))
                {
                    return true;
                }
            }

            // Direct conversion for common keys
            switch (sButton)
            {
                case SButton.W:
                    keys = Keys.W;
                    return true;
                case SButton.A:
                    keys = Keys.A;
                    return true;
                case SButton.S:
                    keys = Keys.S;
                    return true;
                case SButton.D:
                    keys = Keys.D;
                    return true;
                case SButton.Space:
                    keys = Keys.Space;
                    return true;
                case SButton.Enter:
                    keys = Keys.Enter;
                    return true;
                case SButton.Escape:
                    keys = Keys.Escape;
                    return true;
                case SButton.LeftShift:
                    keys = Keys.LeftShift;
                    return true;
                case SButton.RightShift:
                    keys = Keys.RightShift;
                    return true;
                case SButton.LeftControl:
                    keys = Keys.LeftControl;
                    return true;
                case SButton.RightControl:
                    keys = Keys.RightControl;
                    return true;
                // Add more mappings as needed
                default:
                    // Try parsing the SButton name directly as Keys enum
                    return Enum.TryParse<Keys>(sButton.ToString(), out keys);
            }
        }

        /// <summary>
        /// Get debug information about current state
        /// </summary>
        public static string GetDebugInfo()
        {
            var now = DateTime.Now;
            var activePressed = _pressedKeys.Where(kvp => now <= kvp.Value).ToList();
            var activeHeld = _heldKeys.Where(kvp => now <= kvp.Value).ToList();

            return $"Pressed: [{string.Join(", ", activePressed.Select(kvp => $"{kvp.Key}({(kvp.Value - now).TotalMilliseconds:F0}ms)"))}] " +
                   $"Held: [{string.Join(", ", activeHeld.Select(kvp => $"{kvp.Key}({(kvp.Value - now).TotalMilliseconds:F0}ms)"))}] " +
                   $"VirtualActive: {VirtualInputState.Active}";
        }
    }
}
