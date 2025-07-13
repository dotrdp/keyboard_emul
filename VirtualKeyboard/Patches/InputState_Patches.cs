using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Comprehensive patches for input state when game is minimized
    /// Three-part solution:
    /// 1. InputState.GetKeyboardState() - returns virtual keyboard state when minimized
    /// 2. Game1.IsActiveNoOverlay - ensures UpdateControlInput() gets called when minimized  
    /// 3. Game1.IsActive (base property) - ensures IsActiveNoOverlay works when minimized
    /// </summary>
    [HarmonyPatch]
    public class ComprehensiveInputState_Patches
    {
        /// <summary>
        /// PART 1: Patch InputState.GetKeyboardState() to return virtual keyboard state when minimized
        /// This bypasses the focus check that returns empty KeyboardState when the game window isn't focused
        /// </summary>
        [HarmonyPatch(typeof(StardewValley.InputState), "GetKeyboardState")]
        public class InputState_GetKeyboardState_Patch
        {
            [HarmonyPrefix]
            public static bool GetKeyboardState_Prefix(ref KeyboardState __result)
            {
                // Only override when virtual input is active
                if (VirtualInputSimulator.Active)
                {
                    // Use reflection to get the real KeyboardState, bypassing focus checks
                    var keyboardStateField = typeof(InputState).GetField("_keyboardState", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (keyboardStateField != null)
                    {
                        // Get current real keyboard state - handle null properly
                        var fieldValue = keyboardStateField.GetValue(Game1.input);
                        if (fieldValue != null)
                        {
                            var realState = (KeyboardState)fieldValue;
                            
                            // Enhance it with virtual keys
                            __result = VirtualInputSimulator.Instance.GetKeyboardState();
                            return false; // Skip original method
                        }
                    }
                }
                return true; // Use original method
            }
        }

        /// <summary>
        /// PART 2: Patch Game1.IsActiveNoOverlay to return true when virtual input is active
        /// This ensures UpdateControlInput() gets called even when the window is minimized
        /// From decompiled source: if (!globalFade && !freezeControls && activeClickableMenu == null && (IsActiveNoOverlay || inputSimulator != null))
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsActiveNoOverlay", MethodType.Getter)]
        public class Game1_IsActiveNoOverlay_Patches
        {
            [HarmonyPostfix]
            public static void IsActiveNoOverlay_Postfix(ref bool __result)
            {
                // Force IsActiveNoOverlay to return true when virtual input is active
                // This ensures UpdateControlInput() gets called even when minimized
                if (VirtualInputSimulator.Active)
                {
                    __result = true;
                }
            }
        }

        /// <summary>
        /// PART 3: Patch Game1.IsActive (base property) to return true when virtual input is active
        /// This ensures IsActiveNoOverlay works properly when minimized
        /// From decompiled source: IsActiveNoOverlay checks "if (!base.IsActive) return false;"
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsActive", MethodType.Getter)]
        public class Game1_IsActive_Patches
        {
            [HarmonyPostfix]
            public static void IsActive_Postfix(ref bool __result)
            {
                // Force IsActive to return true when virtual input is active
                // This ensures IsActiveNoOverlay doesn't return false due to base.IsActive being false
                if (VirtualInputSimulator.Active)
                {
                    __result = true;
                }
            }
        }
    }
}