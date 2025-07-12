using HarmonyLib;
using StardewValley;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patch to suppress keyboard input when virtual keybinds are active
    /// Based on TASMod's KeyboardDispatcher patches
    /// </summary>
    [HarmonyPatch(typeof(KeyboardDispatcher), "ShouldSuppress")]
    public class KeyboardDispatcher_ShouldSuppress
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            // Always suppress real input to allow virtual input to take control
            __result = true;
        }
    }

    [HarmonyPatch(typeof(KeyboardDispatcher), "Event_TextInput")]
    public class KeyboardDispatcher_Event_TextInput
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            // Suppress all text input events
            return false;
        }
    }

    [HarmonyPatch(typeof(KeyboardDispatcher), "EventInput_CharEntered")]
    public class KeyboardDispatcher_EventInput_CharEntered
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            // Suppress all char input events
            return false;
        }
    }

    [HarmonyPatch(typeof(KeyboardDispatcher), "EventInput_KeyDown")]
    public class KeyboardDispatcher_EventInput_KeyDown
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            // Suppress all key down events
            return false;
        }
    }

    [HarmonyPatch(typeof(KeyboardDispatcher), "Event_KeyDown")]
    public class KeyboardDispatcher_Event_KeyDown
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            // Suppress all key down events
            return false;
        }
    }
}
