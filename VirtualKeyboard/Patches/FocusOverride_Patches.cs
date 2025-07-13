using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches to override game focus detection when virtual input is active
    /// This ensures input works even when the game is minimized
    /// </summary>
    [HarmonyPatch]
    public class FocusOverride_Patches
    {
        /// <summary>
        /// Override HasKeyboardFocus to always return true when virtual input is active
        /// </summary>
        [HarmonyPatch(typeof(Game1), "HasKeyboardFocus")]
        [HarmonyPrefix]
        public static bool HasKeyboardFocus_Prefix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                __result = true;
                return false; // Skip original method
            }
            return true; // Run original method
        }

        /// <summary>
        /// Override IsMainInstance to always return true when virtual input is active
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsMainInstance", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool IsMainInstance_Prefix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                __result = true;
                return false; // Skip original method
            }
            return true; // Run original method
        }

        /// <summary>
        /// Override IsActive to always return true when virtual input is active
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsActive", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool IsActive_Prefix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                __result = true;
                return false; // Skip original method
            }
            return true; // Run original method
        }
    }
}
