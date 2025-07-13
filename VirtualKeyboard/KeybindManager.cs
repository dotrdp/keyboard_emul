using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard
{
    /// <summary>
    /// Manages virtual keyboard input state and keybind simulation
    /// Enhanced with Windows API input for minimized game support
    /// </summary>
    public static class KeybindManager
    {
        /// <summary>
        /// Currently held virtual keys
        /// </summary>
        private static readonly Dictionary<SButton, DateTime> HeldKeys = new();

        /// <summary>
        /// Keys pressed this frame
        /// </summary>
        private static readonly HashSet<SButton> PressedKeys = new();

        /// <summary>
        /// Keys released this frame
        /// </summary>
        private static readonly HashSet<SButton> ReleasedKeys = new();

        /// <summary>
        /// Active key sequences being executed
        /// </summary>
        private static readonly List<KeySequence> ActiveSequences = new();

        /// <summary>
        /// Whether virtual keybinds are currently active
        /// </summary>
        public static bool HasActiveKeybinds => HeldKeys.Count > 0 || PressedKeys.Count > 0 || ActiveSequences.Count > 0;

        /// <summary>
        /// Whether virtual input is enabled
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether to use low-level Windows input when game is minimized
        /// </summary>
        public static bool UseWindowsInputWhenMinimized { get; set; } = true;

        /// <summary>
        /// Event fired when virtual key state changes
        /// </summary>
        public static event Action<SButton, bool>? KeyStateChanged;

        /// <summary>
        /// Initialize the keybind manager
        /// </summary>
        public static void Initialize()
        {
            // Clear any existing state
            HeldKeys.Clear();
            PressedKeys.Clear();
            ReleasedKeys.Clear();
            ActiveSequences.Clear();
            
            // Initialize Windows input simulator for minimized game support
            if (UseWindowsInputWhenMinimized)
            {
                WindowsInputSimulator.Initialize();
            }
        }

        /// <summary>
        /// Update the keybind manager each frame
        /// </summary>
        public static void Update()
        {
            if (!IsEnabled) return;

            // Clear frame-specific collections
            PressedKeys.Clear();
            ReleasedKeys.Clear();

            // Update active sequences
            for (int i = ActiveSequences.Count - 1; i >= 0; i--)
            {
                var sequence = ActiveSequences[i];
                sequence.Update();
                
                if (sequence.IsComplete)
                {
                    ActiveSequences.RemoveAt(i);
                }
            }

            // Check for expired held keys
            var expiredKeys = HeldKeys.Where(kvp => 
                kvp.Value != DateTime.MaxValue && 
                DateTime.Now > kvp.Value).ToList();

            foreach (var kvp in expiredKeys)
            {
                ReleaseKey(kvp.Key);
            }
        }

        /// <summary>
        /// Simulate a key press
        /// </summary>
        /// <param name="key">The key to press</param>
        public static void PressKey(SButton key)
        {
            if (!IsEnabled) return;

            PressedKeys.Add(key);
            HeldKeys[key] = DateTime.MaxValue; // Hold indefinitely until released
            KeyStateChanged?.Invoke(key, true);
            
            // Also send to Windows input simulator for minimized game support
            if (UseWindowsInputWhenMinimized && TryConvertSButtonToKeys(key, out var xnaKey))
            {
                WindowsInputSimulator.SendKeyInput(xnaKey, true);
            }
            
            // Silent operation - only log on demand via status commands
        }

        /// <summary>
        /// Simulate a key release
        /// </summary>
        /// <param name="key">The key to release</param>
        public static void ReleaseKey(SButton key)
        {
            if (!IsEnabled) return;

            if (HeldKeys.ContainsKey(key))
            {
                HeldKeys.Remove(key);
                ReleasedKeys.Add(key);
                KeyStateChanged?.Invoke(key, false);
                
                // Also send to Windows input simulator for minimized game support
                if (UseWindowsInputWhenMinimized && TryConvertSButtonToKeys(key, out var xnaKey))
                {
                    WindowsInputSimulator.SendKeyInput(xnaKey, false);
                }
                
                // Silent operation - only log on demand via status commands
            }
        }

        /// <summary>
        /// Hold a key for a specific duration
        /// </summary>
        /// <param name="key">The key to hold</param>
        /// <param name="durationMs">Duration in milliseconds</param>
        public static void HoldKey(SButton key, int durationMs)
        {
            if (!IsEnabled) return;

            PressedKeys.Add(key);
            HeldKeys[key] = DateTime.Now.AddMilliseconds(durationMs);
            KeyStateChanged?.Invoke(key, true);
            
            // Also send to Windows input simulator for minimized game support
            if (UseWindowsInputWhenMinimized && TryConvertSButtonToKeys(key, out var xnaKey))
            {
                // Fire and forget for Windows input
                System.Threading.Tasks.Task.Run(async () => await WindowsInputSimulator.SendKeyInputAsync(xnaKey, durationMs));
            }
            
            // Silent operation - only log on demand via status commands
        }

        /// <summary>
        /// Execute a sequence of key presses
        /// </summary>
        /// <param name="keys">The keys to press in sequence</param>
        /// <param name="intervalMs">Interval between presses in milliseconds</param>
        public static void ExecuteSequence(IEnumerable<SButton> keys, int intervalMs = 100)
        {
            if (!IsEnabled) return;

            var sequence = new KeySequence(keys, intervalMs);
            ActiveSequences.Add(sequence);
            
            // Only log sequence start - no individual key logs
            Patches.IPatch.Info($"Started key sequence: {string.Join(", ", keys)} with {intervalMs}ms intervals");
        }

        /// <summary>
        /// Execute a key combination (simultaneous press)
        /// </summary>
        /// <param name="keys">The keys to press simultaneously</param>
        /// <param name="holdDurationMs">How long to hold the combination</param>
        public static void ExecuteCombo(IEnumerable<SButton> keys, int holdDurationMs = 50)
        {
            if (!IsEnabled) return;

            var keyList = keys.ToList();
            
            // Press all keys simultaneously
            foreach (var key in keyList)
            {
                PressedKeys.Add(key);
                HeldKeys[key] = DateTime.Now.AddMilliseconds(holdDurationMs);
                KeyStateChanged?.Invoke(key, true);
            }
            
            // Only log combo execution - no individual key logs
            Patches.IPatch.Info($"Executed key combo: {string.Join("+", keyList)} for {holdDurationMs}ms");
        }

        /// <summary>
        /// Check if a virtual key is currently held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is being held virtually</returns>
        public static bool IsKeyHeld(SButton key)
        {
            return HeldKeys.ContainsKey(key);
        }

        /// <summary>
        /// Check if a virtual key was pressed this frame
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was pressed this frame</returns>
        public static bool IsKeyPressed(SButton key)
        {
            return PressedKeys.Contains(key);
        }

        /// <summary>
        /// Check if a virtual key was released this frame
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was released this frame</returns>
        public static bool IsKeyReleased(SButton key)
        {
            return ReleasedKeys.Contains(key);
        }

        /// <summary>
        /// Get all currently held keys
        /// </summary>
        /// <returns>Collection of held keys</returns>
        public static IEnumerable<SButton> GetHeldKeys()
        {
            return HeldKeys.Keys.ToList();
        }

        /// <summary>
        /// Clear all virtual key states
        /// </summary>
        public static void ClearAllKeys()
        {
            var heldKeys = HeldKeys.Keys.ToList();
            foreach (var key in heldKeys)
            {
                ReleaseKey(key);
            }
            
            PressedKeys.Clear();
            ReleasedKeys.Clear();
            ActiveSequences.Clear();
            
            Patches.IPatch.Info("Cleared all virtual key states");
        }

        /// <summary>
        /// Get status information about current keybind state
        /// </summary>
        /// <returns>Status string</returns>
        public static string GetStatus()
        {
            var held = HeldKeys.Keys.ToList();
            var sequences = ActiveSequences.Count;
            
            return $"Held Keys: [{string.Join(", ", held)}], Active Sequences: {sequences}, Enabled: {IsEnabled}";
        }

        /// <summary>
        /// Convert SMAPI SButton to XNA Keys for Windows input simulation
        /// </summary>
        private static bool TryConvertSButtonToKeys(SButton sButton, out Keys key)
        {
            key = sButton switch
            {
                // Movement keys
                SButton.W => Keys.W,
                SButton.A => Keys.A,
                SButton.S => Keys.S,
                SButton.D => Keys.D,
                
                // Action keys
                SButton.C => Keys.C,
                SButton.X => Keys.X,
                SButton.F => Keys.F,
                SButton.Y => Keys.Y,
                SButton.N => Keys.N,
                
                // Inventory keys  
                SButton.D1 => Keys.D1,
                SButton.D2 => Keys.D2,
                SButton.D3 => Keys.D3,
                SButton.D4 => Keys.D4,
                SButton.D5 => Keys.D5,
                SButton.D6 => Keys.D6,
                SButton.D7 => Keys.D7,
                SButton.D8 => Keys.D8,
                SButton.D9 => Keys.D9,
                SButton.D0 => Keys.D0,
                SButton.OemMinus => Keys.OemMinus,
                SButton.OemPlus => Keys.OemPlus,
                
                // Menu keys
                SButton.E => Keys.E,
                SButton.M => Keys.M,
                SButton.J => Keys.J,
                SButton.I => Keys.I,
                SButton.Tab => Keys.Tab,
                SButton.Escape => Keys.Escape,
                
                // Modifier keys
                SButton.LeftShift => Keys.LeftShift,
                SButton.RightShift => Keys.RightShift,
                SButton.LeftControl => Keys.LeftControl,
                SButton.RightControl => Keys.RightControl,
                SButton.LeftAlt => Keys.LeftAlt,
                SButton.RightAlt => Keys.RightAlt,
                SButton.Space => Keys.Space,
                SButton.Enter => Keys.Enter,
                
                _ => Keys.None
            };
            
            return key != Keys.None;
        }
    }

    /// <summary>
    /// Represents a sequence of key presses with timing
    /// </summary>
    public class KeySequence
    {
        private readonly List<SButton> keys;
        private readonly int intervalMs;
        private int currentIndex = 0;
        private DateTime nextActionTime;

        public bool IsComplete => currentIndex >= keys.Count;

        public KeySequence(IEnumerable<SButton> keys, int intervalMs)
        {
            this.keys = keys.ToList();
            this.intervalMs = intervalMs;
            this.nextActionTime = DateTime.Now;
        }

        public void Update()
        {
            if (IsComplete || DateTime.Now < nextActionTime) return;

            var key = keys[currentIndex];
            
            // Press and immediately schedule release
            KeybindManager.PressKey(key);
            KeybindManager.HoldKey(key, 50); // Brief hold
            
            currentIndex++;
            nextActionTime = DateTime.Now.AddMilliseconds(intervalMs);
        }
    }
}
