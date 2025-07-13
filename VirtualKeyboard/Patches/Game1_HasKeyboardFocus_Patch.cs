using HarmonyLib;
using StardewValley;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    [HarmonyPatch(typeof(Game1), "HasKeyboardFocus")]
    public class Game1_HasKeyboardFocus_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            // Always return true when our virtual input simulator is active
            // This tricks the game into thinking it has keyboard focus even when minimized
            if (VirtualInputSimulator.Active)
            {
                __result = true;
                return false; // Skip original method
            }
            
            return true; // Run original method
        }
    }
}
