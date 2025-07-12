using StardewModdingAPI;
using VirtualKeyboard.Console;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Simple command to test if SMAPI input helper recognizes virtual keys
    /// </summary>
    public class KeybindDirectTestCommand : IConsoleCommand
    {
        public string Name => "keybind_direct_test";
        public string Description => "Directly test if SMAPI input helper sees virtual keys";
        public string Usage => "keybind_direct_test <key>";

        public string Execute(string[] args)
        {
            if (args.Length < 1)
            {
                return $"Usage: {Usage}\nExample: keybind_direct_test W";
            }

            if (!System.Enum.TryParse<SButton>(args[0], out var key))
            {
                return $"Invalid key: {args[0]}";
            }

            var result = $"Direct test for key: {key}\n";
            
            // Step 1: Set virtual key
            KeybindManager.PressKey(key);
            result += $"1. KeybindManager.PressKey({key}) called\n";
            
            // Step 2: Check our manager
            var isHeldByManager = KeybindManager.IsKeyHeld(key);
            result += $"2. KeybindManager.IsKeyHeld({key}): {isHeldByManager}\n";
            
            // Step 3: Try to access SMAPI's input helper directly
            try
            {
                var modHelper = ModEntry.Helper;
                if (modHelper?.Input != null)
                {
                    var smapiSees = modHelper.Input.IsDown(key);
                    result += $"3. SMAPI Helper.Input.IsDown({key}): {smapiSees}\n";
                    
                    if (smapiSees)
                    {
                        result += "✅ SUCCESS: SMAPI sees the virtual key!\n";
                    }
                    else
                    {
                        result += "❌ PROBLEM: SMAPI does not see the virtual key\n";
                    }
                }
                else
                {
                    result += "3. SMAPI Input Helper not available\n";
                }
            }
            catch (System.Exception ex)
            {
                result += $"3. Error testing SMAPI input: {ex.Message}\n";
            }
            
            // Step 4: Check Game1 input
            try
            {
                var game1Input = StardewValley.Game1.input;
                if (game1Input != null)
                {
                    // Convert to keyboard key if possible
                    if (key.TryGetKeyboard(out var xnaKey))
                    {
                        var game1Sees = StardewValley.Game1.input.GetKeyboardState().IsKeyDown(xnaKey);
                        result += $"4. Game1.input.IsKeyDown({xnaKey}): {game1Sees}\n";
                    }
                    else
                    {
                        result += $"4. {key} is not a keyboard key\n";
                    }
                }
                else
                {
                    result += "4. Game1.input is null\n";
                }
            }
            catch (System.Exception ex)
            {
                result += $"4. Error testing Game1 input: {ex.Message}\n";
            }
            
            // Hold for 1 second then release
            result += $"Holding {key} for 1 second...\n";
            KeybindManager.HoldKey(key, 1000);
            
            return result.TrimEnd();
        }
    }
}
