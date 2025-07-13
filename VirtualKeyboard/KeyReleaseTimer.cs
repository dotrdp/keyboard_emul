using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StardewModdingAPI;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard
{
    /// <summary>
    /// Simple timer-based key release system that forces keys to be released after specified duration
    /// This is a failsafe to prevent infinite key loops
    /// </summary>
    public static class KeyReleaseTimer
    {
        private static readonly Dictionary<SButton, System.Threading.CancellationTokenSource> _activeTimers = new();
        
        /// <summary>
        /// Start a timer to force-release a key after the specified duration
        /// </summary>
        public static void StartTimer(SButton key, int durationMs)
        {
            // Cancel any existing timer for this key
            if (_activeTimers.TryGetValue(key, out var existingToken))
            {
                existingToken.Cancel();
                _activeTimers.Remove(key);
            }
            
            // Create new timer
            var cancellationSource = new System.Threading.CancellationTokenSource();
            _activeTimers[key] = cancellationSource;
            
            // Start async timer
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(durationMs, cancellationSource.Token);
                    
                    // Time expired - force release the key
                    if (!cancellationSource.Token.IsCancellationRequested)
                    {
                        ForceReleaseKey(key);
                        _activeTimers.Remove(key);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Timer was cancelled - this is normal
                }
                catch (Exception ex)
                {
                    ModEntry.Monitor.Log($"Error in KeyReleaseTimer for {key}: {ex.Message}", LogLevel.Error);
                }
            });
        }
        
        /// <summary>
        /// Cancel the timer for a key (called when key is manually released)
        /// </summary>
        public static void CancelTimer(SButton key)
        {
            if (_activeTimers.TryGetValue(key, out var token))
            {
                token.Cancel();
                _activeTimers.Remove(key);
            }
        }
        
        /// <summary>
        /// Force release a key by clearing it from all systems
        /// </summary>
        private static void ForceReleaseKey(SButton key)
        {
            try
            {
                ModEntry.Monitor.Log($"FORCE RELEASING stuck key: {key}", LogLevel.Warn);
                
                // Check current state before force release
                var wasInHeldKeys = KeybindManager.GetHeldKeys().Contains(key);
                ModEntry.Monitor.Log($"Key {key} was in HeldKeys: {wasInHeldKeys}", LogLevel.Info);
                
                // Clear from KeybindManager
                KeybindManager.ReleaseKey(key);
                
                // Check if it was removed
                var stillInHeldKeys = KeybindManager.GetHeldKeys().Contains(key);
                ModEntry.Monitor.Log($"Key {key} still in HeldKeys after ReleaseKey: {stillInHeldKeys}", LogLevel.Info);
                
                // Clear from VirtualInputSimulator
                if (KeybindManager.TryConvertSButtonToKeys(key, out var xnaKey))
                {
                    VirtualInputSimulator.Instance.SetKeyPressed(xnaKey, false);
                    ModEntry.Monitor.Log($"Called VirtualInputSimulator.SetKeyPressed({xnaKey}, false)", LogLevel.Info);
                    
                    // Force clear the movement flags if it's a movement key
                    VirtualInputSimulator.Instance.ClearAllInputs();
                    ModEntry.Monitor.Log($"Called VirtualInputSimulator.ClearAllInputs()", LogLevel.Info);
                }
                
                // Clear Windows input if enabled - DISABLED FOR DEBUG
                // The Windows input simulation seems to be causing phantom keystrokes
                /*
                if (KeybindManager.UseWindowsInputWhenMinimized && KeybindManager.TryConvertSButtonToKeys(key, out var winKey))
                {
                    WindowsInputSimulator.SendKeyInput(winKey, false);
                    ModEntry.Monitor.Log($"Called WindowsInputSimulator.SendKeyInput({winKey}, false)", LogLevel.Info);
                }
                */
                ModEntry.Monitor.Log($"Skipped Windows input release to prevent phantom keystrokes", LogLevel.Info);
                
                // Force disable VirtualInputSimulator if no keys are held
                // CRITICAL FIX: Don't disable VirtualInputSimulator.Active!
                // Disabling it causes the game to fall back to real keyboard state
                // which might still have stuck keys from Windows input simulation
                /*
                if (!KeybindManager.GetHeldKeys().Any())
                {
                    VirtualInputSimulator.Active = false;
                    ModEntry.Monitor.Log($"Disabled VirtualInputSimulator.Active", LogLevel.Info);
                }
                */
                ModEntry.Monitor.Log($"Keeping VirtualInputSimulator.Active = true to prevent fallback to real keyboard", LogLevel.Info);
                
                ModEntry.Monitor.Log($"Force released key: {key}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                ModEntry.Monitor.Log($"Error force releasing key {key}: {ex.Message}", LogLevel.Error);
            }
        }
        
        /// <summary>
        /// Clear all active timers (emergency stop)
        /// </summary>
        public static void ClearAllTimers()
        {
            foreach (var kvp in _activeTimers)
            {
                kvp.Value.Cancel();
            }
            _activeTimers.Clear();
        }
    }
}
