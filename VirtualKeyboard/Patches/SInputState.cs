using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using VirtualKeyboard.Inputs;
using StardewModdingAPI;
using System;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patch SMAPI's SInputState to inject virtual keyboard state
    /// This is the critical patch that makes virtual input actually work
    /// Based on TASMod's SInputState patches
    /// </summary>
    [HarmonyPatch]
    public class SInputState_GetKeyboardState
    {
        [HarmonyTargetMethod]
        public static System.Reflection.MethodInfo? TargetMethod()
        {
            try
            {
                // Find SMAPI's SInputState.GetKeyboardState method
                var type = AccessTools.TypeByName("StardewModdingAPI.Framework.Input.SInputState");
                if (type != null)
                {
                    var method = AccessTools.Method(type, "GetKeyboardState");
                    if (method != null)
                    {
                        ModEntry.Logger?.Log($"Found SInputState.GetKeyboardState method", LogLevel.Debug);
                        return method;
                    }
                }
                ModEntry.Logger?.Log("Could not find SInputState.GetKeyboardState method", LogLevel.Warn);
            }
            catch (Exception ex)
            {
                ModEntry.Logger?.Log($"Error finding SInputState.GetKeyboardState: {ex.Message}", LogLevel.Error);
            }
            return null;
        }

        [HarmonyPostfix]
        public static void Postfix(ref KeyboardState __result)
        {
            try
            {
                // If virtual input is active, replace the keyboard state
                if (VirtualInputState.Active)
                {
                    var virtualKeys = VirtualInputState.GetPressedKeys();
                    __result = VirtualInputState.GetKeyboard();
                    ModEntry.Logger?.Log($"SInputState: Injecting virtual keyboard state with keys: [{string.Join(", ", virtualKeys)}]", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger?.Log($"Error in SInputState postfix: {ex.Message}", LogLevel.Error);
            }
        }
    }

    /// <summary>
    /// Fallback: Patch SMAPI's InputHelper directly as well
    /// </summary>
    public class InputHelper_IsDown : IPatch
    {
        public override string Name => "IInputHelper.IsDown";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Find SMAPI's InputHelper class
                var inputHelperType = Type.GetType("StardewModdingAPI.Framework.Input.InputHelper, StardewModdingAPI");
                if (inputHelperType != null)
                {
                    var isDownMethod = AccessTools.Method(inputHelperType, "IsDown", new[] { typeof(SButton) });
                    if (isDownMethod != null)
                    {
                        harmony.Patch(
                            original: isDownMethod,
                            postfix: new HarmonyMethod(this.GetType(), nameof(Postfix))
                        );
                        Trace($"Successfully patched {inputHelperType.Name}.IsDown");
                        return;
                    }
                }

                Warn("Could not find SMAPI's InputHelper.IsDown method");
            }
            catch (Exception ex)
            {
                Error($"Failed to patch InputHelper.IsDown: {ex.Message}");
            }
        }

        public static void Postfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                ModEntry.Logger?.Log($"InputHelper: Virtual key {button} detected as down", LogLevel.Trace);
            }
        }
    }
}
