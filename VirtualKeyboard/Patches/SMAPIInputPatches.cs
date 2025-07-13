using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Linq;
using System.Reflection;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches SMAPI's input helper directly to ensure our virtual keys are recognized.
    /// This targets the actual IInputHelper interface that mods use.
    /// </summary>
    public class SMAPI_InputHelper : IPatch
    {
        public override string Name => "SMAPI InputHelper Comprehensive";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Find all SMAPI input-related types
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name?.Contains("StardewModdingAPI") == true)
                    {
                        PatchSMAPIInputMethods(harmony, assembly);
                    }
                }
            }
            catch (Exception ex)
            {
                Error($"Failed to patch SMAPI input methods: {ex.Message}");
            }
        }

        private void PatchSMAPIInputMethods(Harmony harmony, Assembly assembly)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    // Look for input-related classes
                    if (type.Name.Contains("Input") || type.Name.Contains("Helper"))
                    {
                        PatchInputMethodsInType(harmony, type);
                    }
                }
            }
            catch (Exception ex)
            {
                Warn($"Error scanning assembly {assembly.GetName().Name}: {ex.Message}");
            }
        }

        private void PatchInputMethodsInType(Harmony harmony, Type type)
        {
            try
            {
                // Look for IsDown methods
                var isDownMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => m.Name == "IsDown" && m.GetParameters().Length == 1 && 
                               m.GetParameters()[0].ParameterType == typeof(SButton));

                foreach (var method in isDownMethods)
                {
                    try
                    {
                        harmony.Patch(
                            original: method,
                            postfix: new HarmonyMethod(this.GetType(), nameof(IsDownPostfix))
                        );
                        Trace($"Patched {type.Name}.{method.Name}");
                    }
                    catch (Exception ex)
                    {
                        Warn($"Failed to patch {type.Name}.{method.Name}: {ex.Message}");
                    }
                }

                // Look for GetState methods
                var getStateMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => m.Name.Contains("GetState") || m.Name.Contains("State"));

                foreach (var method in getStateMethods)
                {
                    try
                    {
                        harmony.Patch(
                            original: method,
                            postfix: new HarmonyMethod(this.GetType(), nameof(GetStatePostfix))
                        );
                        Trace($"Patched {type.Name}.{method.Name}");
                    }
                    catch
                    {
                        // Don't warn for state methods as they might not be patchable
                    }
                }
            }
            catch
            {
                // Ignore reflection errors
            }
        }

        public static void IsDownPostfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                // Virtual key active (logging reduced to avoid spam)
            }
        }

        public static void GetStatePostfix(ref object __result)
        {
            if (KeybindManager.HasActiveKeybinds)
            {
                // Silent operation - SMAPI GetState interception active
            }
        }

        private static DateTime _lastGetStateLog = DateTime.MinValue;
    }

    /// <summary>
    /// Direct patch of the Game1.input.IsDown method which is what the game actually uses.
    /// This is the most direct approach to intercept input checks.
    /// </summary>
    public class Game1_InputIsDown : IPatch
    {
        public override string Name => "Game1.input.IsDown";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Patch Game1.input property access first
                var inputProperty = AccessTools.Property(typeof(StardewValley.Game1), "input");
                if (inputProperty?.GetMethod != null)
                {
                    harmony.Patch(
                        original: inputProperty.GetMethod,
                        postfix: new HarmonyMethod(this.GetType(), nameof(InputPropertyPostfix))
                    );
                    Trace("Patched Game1.input property getter");
                }

                // Try to find the InputState type used by Game1
                var inputStateType = StardewValley.Game1.input?.GetType();
                if (inputStateType != null)
                {
                    var isDownMethod = AccessTools.Method(inputStateType, "IsDown", new[] { typeof(SButton) });
                    if (isDownMethod != null)
                    {
                        harmony.Patch(
                            original: isDownMethod,
                            postfix: new HarmonyMethod(this.GetType(), nameof(IsDownPostfix))
                        );
                        Trace($"Patched {inputStateType.Name}.IsDown");
                    }
                }
            }
            catch (Exception ex)
            {
                Error($"Failed to patch Game1.input.IsDown: {ex.Message}");
            }
        }

        public static void InputPropertyPostfix(ref object __result)
        {
            if (KeybindManager.HasActiveKeybinds && __result != null)
            {
                // Only log once per second to avoid spam
                var now = DateTime.Now;
                if (now.Subtract(_lastInputPropertyLog).TotalSeconds >= 1.0)
                {
                    IPatch.Trace("Game1.input accessed with virtual keys active");
                    _lastInputPropertyLog = now;
                }
            }
        }

        private static DateTime _lastInputPropertyLog = DateTime.MinValue;

        public static void IsDownPostfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                // Virtual key active (logging reduced)
            }
        }
    }
}
