using HarmonyLib;
using StardewValley;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    [HarmonyPatch(typeof(Game1), "IsMainInstance", MethodType.Getter)]
    public class Game1_IsMainInstance_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            // Always return true when our virtual input simulator is active
            // This ensures the game processes input even when minimized
            if (VirtualInputSimulator.Active)
            {
                __result = true;
                return false; // Skip original method
            }
            
            return true; // Run original method
        }
    }
}
