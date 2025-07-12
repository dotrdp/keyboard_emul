using StardewModdingAPI;
using VirtualKeyboard.Console;
using System.Linq;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to test if virtual key simulation is working properly
    /// </summary>
    public class KeybindTestCommand : IConsoleCommand
    {
        public string Name => "keybind_test";
        public string Description => "Test virtual key simulation to debug input issues";
        public string Usage => "keybind_test <key>";

        public string Execute(string[] args)
        {
            if (args.Length < 1)
            {
                return $"Usage: {Usage}\nExample: keybind_test W";
            }

            if (!System.Enum.TryParse<SButton>(args[0], out var key))
            {
                return $"Invalid key: {args[0]}";
            }

            var result = $"Testing virtual key simulation for: {key}\n";
            
            // Test 1: Check if KeybindManager recognizes the key
            KeybindManager.PressKey(key);
            var isHeld = KeybindManager.IsKeyHeld(key);
            result += $"KeybindManager.IsKeyHeld({key}): {isHeld}\n";
            
            // Test 2: Check current state
            var status = KeybindManager.GetStatus();
            result += $"KeybindManager status: {status}\n";
            
            // Test 3: Check Game1 input state
            try
            {
                var game1Input = StardewValley.Game1.input;
                if (game1Input != null)
                {
                    result += "Game1.input is available\n";
                }
                else
                {
                    result += "Game1.input is null\n";
                }
            }
            catch (System.Exception ex)
            {
                result += $"Error accessing Game1.input: {ex.Message}\n";
            }
            
            // Hold for 2 seconds then release
            result += $"Holding {key} for 2 seconds...\n";
            KeybindManager.HoldKey(key, 2000);
            
            return result.TrimEnd();
        }
    }
}
