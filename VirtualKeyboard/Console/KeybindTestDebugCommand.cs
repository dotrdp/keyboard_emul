using System;
using StardewModdingAPI;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Debug command to test timing of key holds
    /// </summary>
    public class KeybindTestDebugCommand : IConsoleCommand
    {
        public string Name => "keybind_debug";
        public string Description => "Debug key timing behavior";
        public string Usage => "keybind_debug";

        public string Execute(string[] args)
        {
            var now = DateTime.Now;
            var future = now.AddMilliseconds(1000); // 1 second from now
            
            var result = $"Current time: {now:HH:mm:ss.fff}\n";
            result += $"Future time: {future:HH:mm:ss.fff}\n";
            result += $"Now > Future: {now > future}\n";
            result += $"Now >= Future: {now >= future}\n";
            result += $"Future > Now: {future > now}\n";
            result += $"Future >= Now: {future >= now}\n";
            
            // Test with KeybindManager state
            result += $"\nKeybindManager.IsEnabled: {KeybindManager.IsEnabled}\n";
            result += $"Current held keys: {string.Join(", ", KeybindManager.GetHeldKeys())}\n";
            
            return result;
        }
    }
}
