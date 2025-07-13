using System;
using System.Linq;
using StardewModdingAPI;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Emergency debug command to check KeybindManager state
    /// </summary>
    public class KeybindEmergencyDebugCommand : IConsoleCommand
    {
        public string Name => "keybind_emergency";
        public string Description => "Emergency debug for stuck keys";
        public string Usage => "keybind_emergency";

        public string Execute(string[] args)
        {
            var result = "=== EMERGENCY DEBUG ===\n";
            result += $"KeybindManager.IsEnabled: {KeybindManager.IsEnabled}\n";
            result += $"KeybindManager.HasActiveKeybinds: {KeybindManager.HasActiveKeybinds}\n";
            
            var heldKeys = KeybindManager.GetHeldKeys();
            result += $"HeldKeys.Count: {heldKeys.Count()}\n";
            
            result += "\n=== HELD KEYS ===\n";
            if (heldKeys.Any())
            {
                foreach (var key in heldKeys)
                {
                    result += $"- {key}\n";
                }
            }
            else
            {
                result += "No held keys\n";
            }
            
            result += "\n=== VIRTUAL INPUT SIMULATOR ===\n";
            result += $"VirtualInputSimulator.Active: {VirtualInputSimulator.Active}\n";
            
            var virtualState = VirtualInputSimulator.Instance.GetKeyboardState();
            var pressedVirtualKeys = virtualState.GetPressedKeys();
            result += $"Virtual pressed keys count: {pressedVirtualKeys.Length}\n";
            
            if (pressedVirtualKeys.Length > 0)
            {
                result += "Virtual pressed keys:\n";
                foreach (var key in pressedVirtualKeys)
                {
                    result += $"- {key}\n";
                }
            }
            
            result += "\n=== EMERGENCY CLEAR ===\n";
            KeybindManager.ClearAllKeys();
            VirtualInputSimulator.Instance.ClearAllInputs();
            VirtualInputSimulator.Active = false;
            result += "Cleared all keys and disabled virtual input\n";
            
            return result;
        }
    }
}
