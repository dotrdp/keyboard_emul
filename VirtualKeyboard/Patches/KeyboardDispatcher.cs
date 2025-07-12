using HarmonyLib;
using StardewValley;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patch to suppress keyboard input when virtual keybinds are active
    /// </summary>
    public class KeyboardDispatcher_ShouldSuppress : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.ShouldSuppress";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
            );
        }

        public static void Postfix(ref bool __result)
        {
            // Only suppress if we have active virtual keybinds
            if (KeybindManager.HasActiveKeybinds)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch to handle text input events
    /// </summary>
    public class KeyboardDispatcher_Event_TextInput : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.Event_TextInput";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
            );
        }

        public static bool Prefix()
        {
            // Skip text input if we have active virtual keybinds
            return !KeybindManager.HasActiveKeybinds;
        }
    }

    /// <summary>
    /// Patch to handle character entered events
    /// </summary>
    public class KeyboardDispatcher_EventInput_CharEntered : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.EventInput_CharEntered";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
            );
        }

        public static bool Prefix()
        {
            // Skip character events if we have active virtual keybinds
            return !KeybindManager.HasActiveKeybinds;
        }
    }

    /// <summary>
    /// Patch to handle key down events from input
    /// </summary>
    public class KeyboardDispatcher_EventInput_KeyDown : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.EventInput_KeyDown";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
            );
        }

        public static bool Prefix()
        {
            // Skip input key events if we have active virtual keybinds
            return !KeybindManager.HasActiveKeybinds;
        }
    }

    /// <summary>
    /// Patch to handle general key down events
    /// </summary>
    public class KeyboardDispatcher_Event_KeyDown : IPatch
    {
        public static string BaseKey = "KeyboardDispatcher.Event_KeyDown";
        public override string Name => BaseKey;

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(KeyboardDispatcher), BaseKey.Split(".")[1]),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
            );
        }

        public static bool Prefix()
        {
            // Skip key down events if we have active virtual keybinds
            return !KeybindManager.HasActiveKeybinds;
        }
    }
}
