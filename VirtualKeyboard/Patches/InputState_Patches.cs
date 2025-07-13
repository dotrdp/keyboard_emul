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
        /// CRITICAL: Also prevents cached keyboard state from persisting sticky keys
        /// </summary>
        [HarmonyPatch(typeof(StardewValley.InputState), "GetKeyboardState")]
        public class InputState_GetKeyboardState_Patch
        {
            [HarmonyPrefix]
            public static bool GetKeyboardState_Prefix(ref KeyboardState __result)
            {
                // Always override when virtual input is active - this prevents cached state issues
                if (VirtualInputSimulator.Active)
                {
                    // Return ONLY our virtual keyboard state - ignore any cached real keyboard state
                    __result = VirtualInputSimulator.Instance.GetKeyboardState();
                    return false; // Skip original method completely
                }
                
                return true; // Use original method when virtual input is not active
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

        /// <summary>
        /// PART 4: Patch Game1.HasKeyboardFocus() to return true when virtual input is active
        /// This is CRITICAL for unfocused window input - InputState.GetKeyboardState() checks this
        /// From decompiled source: if (!Game1.game1.IsMainInstance || !Game1.game1.HasKeyboardFocus()) return default(KeyboardState);
        /// </summary>
        [HarmonyPatch(typeof(Game1), "HasKeyboardFocus")]
        public class Game1_HasKeyboardFocus_Patches
        {
            [HarmonyPostfix]
            public static void HasKeyboardFocus_Postfix(ref bool __result)
            {
                // Force HasKeyboardFocus to return true when virtual input is active
                // This is essential for InputState.GetKeyboardState() to work when window is unfocused
                if (VirtualInputSimulator.Active)
                {
                    __result = true;
                }
            }
        }
    }
}