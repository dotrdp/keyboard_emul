using StardewModdingAPI;
using VirtualKeyboard.Inputs;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Test command to verify virtual key detection at multiple levels
    /// </summary>
    public class LowLevelTestCommand : IConsoleCommand
    {
        public string Name => "test_lowlevel";
        public string Description => "Test virtual key detection at multiple system levels";
        public string Usage => "test_lowlevel <key>";

        public string Execute(string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Usage}";

            if (!System.Enum.TryParse<SButton>(args[0], out SButton key))
                return $"Invalid key: {args[0]}";

            // Activate virtual key
            KeybindManager.HoldKey(key, 3000);
            
            var result = $"Testing virtual {key} key at multiple levels:\n";
            result += $"KeybindManager.IsKeyHeld({key}): {KeybindManager.IsKeyHeld(key)}\n";
            result += $"VirtualInputState.Active: {VirtualInputState.Active}\n";
            result += $"VirtualInputState.GetPressedKeys(): [{string.Join(", ", VirtualInputState.GetPressedKeys())}]\n";
            
            // Test if we can convert to Keys
            if (KeybindManager.TryConvertSButtonToKeys(key, out var keys))
            {
                result += $"Converted to Keys.{keys}\n";
                result += $"VirtualInputState.IsKeyDown({keys}): {VirtualInputState.IsKeyDown(keys)}\n";
                
                // Test framework level
                var virtualKeyboard = VirtualInputState.GetKeyboard();
                result += $"Virtual KeyboardState.IsKeyDown({keys}): {virtualKeyboard.IsKeyDown(keys)}\n";
                
                // Test if patches are working
                try
                {
                    var currentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    result += $"Framework Keyboard.GetState().IsKeyDown({keys}): {currentState.IsKeyDown(keys)}\n";
                }
                catch (System.Exception ex)
                {
                    result += $"Framework test error: {ex.Message}\n";
                }
            }
            else
            {
                result += $"Could not convert {key} to Keys enum\n";
            }
            
            result += "\nVirtual key will be held for 3 seconds - try moving in game!";
            return result;
        }
    }
}
