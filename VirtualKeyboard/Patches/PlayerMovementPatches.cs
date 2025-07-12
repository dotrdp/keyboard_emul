using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using VirtualKeyboard.Inputs;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Simplified player movement patches that focus only on existing methods
    /// </summary>
    
    /// <summary>
    /// Patch the main game update loop to inject virtual input
    /// </summary>
    [HarmonyPatch(typeof(Game1), "Update")]
    public class Game1_Update
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (VirtualInputState.Active)
            {
                // At the very beginning of the game update, force our virtual input
                try
                {
                    var currentKBField = AccessTools.Field(typeof(Game1), "currentKBState");
                    var oldKBField = AccessTools.Field(typeof(Game1), "oldKBState");
                    
                    var virtualKeyboard = VirtualInputState.GetKeyboard();
                    currentKBField?.SetValue(null, virtualKeyboard);
                    oldKBField?.SetValue(null, virtualKeyboard);

                    // Also try to override the static Keyboard.GetState call
                    try
                    {
                        var kbStateField = AccessTools.Field(typeof(Keyboard), "_state");
                        kbStateField?.SetValue(null, virtualKeyboard);
                    }
                    catch
                    {
                        // Ignore reflection errors
                    }
                }
                catch { /* ignore reflection errors */ }
            }
        }
    }
}
