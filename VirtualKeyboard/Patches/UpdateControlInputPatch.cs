using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Harmony patch for SMAPI's SInputState.GetKeyboardState to inject virtual keyboard input
    /// This follows the exact pattern used by TASMod for proper input simulation
    /// </summary>
    public static class SInputState_GetKeyboardState_Patch
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:GetKeyboardState"),
                postfix: new HarmonyMethod(typeof(SInputState_GetKeyboardState_Patch), nameof(Postfix))
            );
        }

        public static void Postfix(ref KeyboardState __result)
        {
            if (VirtualInputSimulator.Active)
            {
                var virtualKeyboard = VirtualInputSimulator.Instance.GetKeyboardState();
                ModEntry.Monitor.Log($"SInputState patch: Injecting virtual keyboard with {virtualKeyboard.GetPressedKeys().Length} keys", StardewModdingAPI.LogLevel.Trace);
                __result = virtualKeyboard;
            }
        }
    }
}
