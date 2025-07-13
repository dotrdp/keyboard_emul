using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches InputState.GetKeyboardState() to override focus checks when virtual input is active.
    /// This is the core method that blocks input when the game is minimized.
    /// </summary>
    [HarmonyPatch(typeof(StardewValley.InputState))]
    public class InputState_Patches
    {
        /// <summary>
        /// Prefix patch for InputState.GetKeyboardState() to bypass focus checks when virtual input is active.
        /// This allows input simulation to work even when the game is minimized.
        /// </summary>
        [HarmonyPatch("GetKeyboardState")]
        [HarmonyPrefix]
        public static bool GetKeyboardState_Prefix(ref KeyboardState __result, StardewValley.InputState __instance)
        {
            // If virtual input simulator is not active, let the original method run
            if (!VirtualInputSimulator.Active)
                return true;

            // When virtual input is active, we need to call the original method logic 
            // but bypass the focus checks that return default(KeyboardState)
            
            // Access private fields using reflection to replicate the original method logic
            var keyStateField = AccessTools.Field(typeof(StardewValley.InputState), "_keyState");
            var lastKeyStateTickField = AccessTools.Field(typeof(StardewValley.InputState), "_lastKeyStateTick");
            var ignoredKeysField = AccessTools.Field(typeof(StardewValley.InputState), "_ignoredKeys");
            var pressedKeysField = AccessTools.Field(typeof(StardewValley.InputState), "_pressedKeys");
            var currentKeyboardStateField = AccessTools.Field(typeof(StardewValley.InputState), "_currentKeyboardState");

            if (keyStateField == null || lastKeyStateTickField == null || ignoredKeysField == null || 
                pressedKeysField == null || currentKeyboardStateField == null)
            {
                // Fallback if reflection fails - return current keyboard state
                __result = Keyboard.GetState();
                return false;
            }

            var keyState = (KeyboardState?)keyStateField.GetValue(__instance);
            var lastKeyStateTick = (int)(lastKeyStateTickField.GetValue(__instance) ?? -1);
            var ignoredKeys = (System.Collections.Generic.List<Keys>)(ignoredKeysField.GetValue(__instance) ?? new System.Collections.Generic.List<Keys>());
            var pressedKeys = (System.Collections.Generic.List<Keys>)(pressedKeysField.GetValue(__instance) ?? new System.Collections.Generic.List<Keys>());
            var currentKeyboardState = (KeyboardState)(currentKeyboardStateField.GetValue(__instance) ?? default(KeyboardState));

            // Replicate the original method logic but without the focus checks
            if (lastKeyStateTick != Game1.ticks || !keyState.HasValue)
            {
                if (ignoredKeys?.Count == 0)
                {
                    keyState = currentKeyboardState;
                }
                else if (ignoredKeys != null && pressedKeys != null)
                {
                    pressedKeys.Clear();
                    pressedKeys.AddRange(currentKeyboardState.GetPressedKeys());
                    
                    for (int i = 0; i < ignoredKeys.Count; i++)
                    {
                        Keys key = ignoredKeys[i];
                        if (!pressedKeys.Contains(key))
                        {
                            ignoredKeys.RemoveAt(i);
                            i--;
                        }
                    }
                    
                    for (int i = 0; i < pressedKeys.Count; i++)
                    {
                        Keys key = pressedKeys[i];
                        if (ignoredKeys.Contains(key))
                        {
                            pressedKeys.RemoveAt(i);
                            i--;
                        }
                    }
                    
                    keyState = new KeyboardState(pressedKeys.ToArray());
                }
                
                // Update the private fields
                keyStateField.SetValue(__instance, keyState);
                lastKeyStateTickField.SetValue(__instance, Game1.ticks);
            }

            __result = keyState ?? default(KeyboardState);
            return false; // Skip the original method
        }
    }
    
    /// <summary>
    /// Critical patch for Game1.IsActiveNoOverlay to ensure UpdateControlInput is called
    /// even when the game is minimized and virtual input is active.
    /// </summary>
    [HarmonyPatch(typeof(Game1))]
    public class Game1_IsActiveNoOverlay_Patches
    {
        [HarmonyPatch("IsActiveNoOverlay", MethodType.Getter)]
        [HarmonyPostfix]
        public static void IsActiveNoOverlay_Postfix(ref bool __result)
        {
            // When virtual input is active, force the game to think it's active
            // This ensures UpdateControlInput gets called even when minimized
            if (VirtualInputSimulator.Active)
            {
                __result = true;
            }
        }
    }
}
