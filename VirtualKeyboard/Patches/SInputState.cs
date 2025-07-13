using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches SMAPI's input helper to include virtual keybinds.
    /// This targets the IInputHelper.IsDown method that mods typically use.
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
                // Only log key changes, not every frame check
            }
        }
    }

    /// <summary>
    /// Patches SMAPI's input state directly to include virtual keybinds.
    /// This is a deeper level patch for the internal input state.
    /// </summary>
    public class SInputState_IsDown : IPatch
    {
        public override string Name => "SInputState.IsDown";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Try multiple possible SMAPI input state types
                var inputStateTypes = new[]
                {
                    "StardewModdingAPI.Framework.Input.SInputState, StardewModdingAPI",
                    "StardewModdingAPI.Framework.Input.InputState, StardewModdingAPI",
                    "StardewModdingAPI.Framework.SCore+InputState, StardewModdingAPI"
                };

                foreach (var typeName in inputStateTypes)
                {
                    var inputStateType = Type.GetType(typeName);
                    if (inputStateType != null)
                    {
                        var method = AccessTools.Method(inputStateType, "IsDown", new[] { typeof(SButton) });
                        if (method != null)
                        {
                            harmony.Patch(
                                original: method,
                                postfix: new HarmonyMethod(this.GetType(), nameof(Postfix))
                            );
                            Trace($"Successfully patched {inputStateType.Name}.IsDown");
                            return;
                        }
                    }
                }

                // Try patching the SCore.Input property getter
                var sCoreType = Type.GetType("StardewModdingAPI.Framework.SCore, StardewModdingAPI");
                if (sCoreType != null)
                {
                    var inputProperty = AccessTools.Property(sCoreType, "Input");
                    if (inputProperty?.GetMethod != null)
                    {
                        harmony.Patch(
                            original: inputProperty.GetMethod,
                            postfix: new HarmonyMethod(this.GetType(), nameof(InputPropertyPostfix))
                        );
                        Trace("Patched SCore.Input property as fallback");
                    }
                }

                Warn("Could not find specific IsDown method, using fallback patches");
            }
            catch (Exception ex)
            {
                Error($"Failed to patch SInputState methods: {ex.Message}");
            }
        }

        public static void Postfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                // Removed verbose logging to clean up console
            }
        }

        public static void InputPropertyPostfix(ref object __result)
        {
            if (KeybindManager.HasActiveKeybinds)
            {
                // Silent operation - input interception active
            }
        }
    }

    /// <summary>
    /// Patches Game1's input checking methods directly.
    /// This ensures virtual keys work with the game's internal input checks.
    /// </summary>
    public class Game1_Input : IPatch
    {
        public override string Name => "Game1 Input Methods";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Patch Game1's input field access
                var inputField = AccessTools.Field(typeof(Game1), "input");
                if (inputField != null)
                {
                    Trace("Found Game1.input field");
                }

                // Try to patch any Game1 methods that check input
                var getMouseStateMethod = AccessTools.Method(typeof(Game1), "GetMouseState");
                var getKeyboardStateMethod = AccessTools.Method(typeof(Game1), "GetKeyboardState");
                
                if (getKeyboardStateMethod != null)
                {
                    harmony.Patch(
                        original: getKeyboardStateMethod,
                        postfix: new HarmonyMethod(this.GetType(), nameof(KeyboardStatePostfix))
                    );
                    Trace("Patched Game1.GetKeyboardState");
                }

                // Also patch the oldKBState property
                var oldKBStateProperty = AccessTools.Property(typeof(Game1), "oldKBState");
                if (oldKBStateProperty?.GetMethod != null)
                {
                    harmony.Patch(
                        original: oldKBStateProperty.GetMethod,
                        postfix: new HarmonyMethod(this.GetType(), nameof(OldKBStatePostfix))
                    );
                    Trace("Patched Game1.oldKBState property");
                }
            }
            catch (Exception ex)
            {
                Warn($"Could not patch Game1 input methods: {ex.Message}");
            }
        }

        public static void KeyboardStatePostfix(ref KeyboardState __result)
        {
            if (KeybindManager.HasActiveKeybinds)
            {
                // Get the virtual keyboard state from our simulator
                var virtualState = VirtualKeyboard.Simulation.VirtualInputSimulator.Instance.GetKeyboardState();
                
                // Combine the current state with our virtual state
                var currentKeys = __result.GetPressedKeys().ToList();
                var virtualKeys = virtualState.GetPressedKeys().ToList();
                
                // Add virtual keys that aren't already pressed
                foreach (var virtualKey in virtualKeys)
                {
                    if (!currentKeys.Contains(virtualKey))
                    {
                        currentKeys.Add(virtualKey);
                    }
                }
                
                // Create new keyboard state with combined keys
                __result = new KeyboardState(currentKeys.ToArray());
            }
        }

        private static DateTime _lastKeyboardStateLog = DateTime.MinValue;

        public static void OldKBStatePostfix(ref KeyboardState __result)
        {
            if (KeybindManager.HasActiveKeybinds)
            {
                // Silent operation - old keyboard state interception active
            }
        }
    }

    /// <summary>
    /// Patches XNA Framework's KeyboardState.IsKeyDown to include virtual keys.
    /// This provides fallback coverage for direct XNA keyboard access.
    /// </summary>
    public class KeyboardState_IsKeyDown : IPatch
    {
        public override string Name => "KeyboardState.IsKeyDown";

        public override void Patch(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(KeyboardState), "IsKeyDown", new[] { typeof(Keys) }),
                    postfix: new HarmonyMethod(this.GetType(), nameof(Postfix))
                );
                Trace("Successfully patched KeyboardState.IsKeyDown");
            }
            catch (Exception ex)
            {
                Error($"Failed to patch KeyboardState.IsKeyDown: {ex.Message}");
            }
        }

        public static void Postfix(Keys key, ref bool __result)
        {
            // Convert XNA Keys to SButton and check virtual state
            if (TryConvertKeysToSButton(key, out var sButton))
            {
                if (KeybindManager.IsKeyHeld(sButton))
                {
                    __result = true;
                    // Removed verbose logging to clean up console
                }
            }
        }

        /// <summary>
        /// Convert XNA Keys enum to SMAPI SButton
        /// </summary>
        private static bool TryConvertKeysToSButton(Keys key, out SButton sButton)
        {
            try
            {
                // Direct enum name conversion
                if (Enum.TryParse<SButton>(key.ToString(), out sButton))
                {
                    return true;
                }

                // Handle special cases if needed
                sButton = key switch
                {
                    Keys.None => SButton.None,
                    _ => SButton.None
                };

                return sButton != SButton.None;
            }
            catch
            {
                sButton = SButton.None;
                return false;
            }
        }
    }

}
