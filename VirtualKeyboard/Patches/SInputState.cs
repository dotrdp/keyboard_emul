using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patch to override SMAPI's keyboard state with virtual keyboard state
    /// </summary>
    public class SInputState_GetKeyboardState : IPatch
    {
        public override string Name => "SInputState.GetKeyboardState";

        public override void Patch(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:GetKeyboardState"),
                    postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
            }
            catch (Exception ex)
            {
                Error($"Failed to patch SInputState.GetKeyboardState: {ex.Message}");
            }
        }

        public static void Postfix(ref KeyboardState __result)
        {
            if (KeybindManager.HasActiveKeybinds)
            {
                __result = VirtualKeyboardState.GetKeyboardState();
            }
        }
    }

    /// <summary>
    /// Patch to override button state for individual button checks
    /// </summary>
    public class SInputState_IsDown : IPatch
    {
        public override string Name => "SInputState.IsDown";

        public override void Patch(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method("StardewModdingAPI.Framework.Input.SInputState:IsDown"),
                    postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
            }
            catch (Exception ex)
            {
                Error($"Failed to patch SInputState.IsDown: {ex.Message}");
            }
        }

        public static void Postfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsEnabled && KeybindManager.IsKeyHeld(button))
            {
                __result = true;
            }
        }
    }
}

namespace VirtualKeyboard
{
    /// <summary>
    /// Manages virtual keyboard state for XNA Framework compatibility
    /// </summary>
    public static class VirtualKeyboardState
    {
        /// <summary>
        /// Get a keyboard state that includes virtual key presses
        /// </summary>
        /// <returns>Modified KeyboardState</returns>
        public static KeyboardState GetKeyboardState()
        {
            var realState = Keyboard.GetState();
            
            if (!KeybindManager.HasActiveKeybinds)
                return realState;

            // Get real pressed keys
            var realKeys = realState.GetPressedKeys();
            var virtualKeys = GetVirtualKeys();
            
            // Combine real and virtual keys
            var allKeys = new System.Collections.Generic.HashSet<Keys>();
            
            foreach (var key in realKeys)
                allKeys.Add(key);
                
            foreach (var key in virtualKeys)
                allKeys.Add(key);

            // Create new keyboard state
            return CreateKeyboardState(allKeys);
        }

        /// <summary>
        /// Get virtual keys that are currently pressed
        /// </summary>
        /// <returns>Array of virtual Keys</returns>
        private static Keys[] GetVirtualKeys()
        {
            var virtualKeys = new System.Collections.Generic.List<Keys>();
            
            foreach (var sButton in KeybindManager.GetHeldKeys())
            {
                if (TryConvertSButtonToKeys(sButton, out var key))
                {
                    virtualKeys.Add(key);
                }
            }
            
            return virtualKeys.ToArray();
        }

        /// <summary>
        /// Convert SButton to XNA Keys
        /// </summary>
        /// <param name="sButton">SMAPI button</param>
        /// <param name="key">Converted XNA key</param>
        /// <returns>True if conversion successful</returns>
        private static bool TryConvertSButtonToKeys(SButton sButton, out Keys key)
        {
            // Try direct enum conversion first
            if (Enum.TryParse<Keys>(sButton.ToString(), out key))
                return true;

            // Handle special cases
            key = sButton switch
            {
                SButton.MouseLeft => Keys.None, // Mouse buttons don't map to Keys
                SButton.MouseRight => Keys.None,
                SButton.MouseMiddle => Keys.None,
                SButton.ControllerA => Keys.None, // Controller buttons don't map to Keys
                SButton.ControllerB => Keys.None,
                _ => Keys.None
            };

            return key != Keys.None;
        }

        /// <summary>
        /// Create a KeyboardState from a collection of pressed keys
        /// </summary>
        /// <param name="pressedKeys">Keys that should be pressed</param>
        /// <returns>New KeyboardState</returns>
        private static KeyboardState CreateKeyboardState(System.Collections.Generic.IEnumerable<Keys> pressedKeys)
        {
            // This is a bit hacky, but we need to create a KeyboardState with specific keys pressed
            // Since KeyboardState constructor is internal, we'll use reflection
            
            try
            {
                var keyArray = new bool[256]; // KeyboardState uses internal array of 256 bools
                
                foreach (var key in pressedKeys)
                {
                    var keyIndex = (int)key;
                    if (keyIndex >= 0 && keyIndex < 256)
                        keyArray[keyIndex] = true;
                }

                // Create KeyboardState using reflection
                var constructor = typeof(KeyboardState).GetConstructor(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(Keys[]) },
                    null);

                if (constructor != null)
                {
                    var pressedKeysArray = pressedKeys.ToArray();
                    return (KeyboardState)constructor.Invoke(new object[] { pressedKeysArray });
                }

                // Fallback: return current state if reflection fails
                Patches.IPatch.Warn("Failed to create custom KeyboardState, falling back to real state");
                return Keyboard.GetState();
            }
            catch (Exception ex)
            {
                Patches.IPatch.Error($"Error creating virtual KeyboardState: {ex.Message}");
                return Keyboard.GetState();
            }
        }
    }
}
