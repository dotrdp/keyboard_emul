using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using VirtualKeyboard.Inputs;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patch the core framework-level input methods
    /// Focus only on methods that definitely exist
    /// </summary>
    
    /// <summary>
    /// Patch Keyboard.GetState() directly at the framework level
    /// This is the deepest level we can intercept keyboard input
    /// </summary>
    [HarmonyPatch(typeof(Keyboard), nameof(Keyboard.GetState), new System.Type[] { })]
    public class Keyboard_GetState
    {
        [HarmonyPostfix]
        public static void Postfix(ref KeyboardState __result)
        {
            if (VirtualInputState.Active)
            {
                __result = VirtualInputState.GetKeyboard();
                ModEntry.Logger?.Log($"Keyboard.GetState: Injected virtual keyboard state with {VirtualInputState.GetPressedKeys().Length} keys", LogLevel.Trace);
            }
        }
    }

    /// <summary>
    /// Patch KeyboardState.IsKeyDown directly
    /// This ensures individual key checks return true for virtual keys
    /// </summary>
    [HarmonyPatch(typeof(KeyboardState), nameof(KeyboardState.IsKeyDown))]
    public class KeyboardState_IsKeyDown
    {
        [HarmonyPostfix]
        public static void Postfix(Keys key, ref bool __result)
        {
            if (!__result && VirtualInputState.Active && VirtualInputState.IsKeyDown(key))
            {
                __result = true;
                ModEntry.Logger?.Log($"KeyboardState.IsKeyDown: Virtual key {key} detected", LogLevel.Trace);
            }
        }
    }


}
