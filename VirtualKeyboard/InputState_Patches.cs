using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Reflection;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Patches
{
    [HarmonyPatch]
    public class InputState_Patches
    {
        /// <summary>
        /// Prefix patch for InputState.GetKeyboardState() to bypass focus checks when virtual input is active.
        /// This allows input simulation to work even when the game is minimized.
        /// </summary>
        [HarmonyPatch(typeof(StardewValley.InputState), "GetKeyboardState")]
        [HarmonyPrefix]
        public static bool GetKeyboardState_Prefix(ref KeyboardState __result, StardewValley.InputState __instance)
        {
            // If virtual input simulator is not active, let the original method run
            if (!VirtualInputSimulator.Active)
                return true;

            // When virtual input is active, we need to call the original method logic 
            // but bypass the focus checks that return default(KeyboardState)
            
            // Access private fields using reflection to replicate the original method logic
            var keyStateField = AccessTools.Field(typeof(StardewValley.InputState), "_keyState");
            var lastKeyStateTickField = AccessTools.Field(typeof(StardewValley.InputState), "_lastKeyStateTick");
            var ignoredKeysField = AccessTools.Field(typeof(StardewValley.InputState), "_ignoredKeys");
            var pressedKeysField = AccessTools.Field(typeof(StardewValley.InputState), "_pressedKeys");
            var currentKeyboardStateField = AccessTools.Field(typeof(StardewValley.InputState), "_currentKeyboardState");

            if (keyStateField == null || lastKeyStateTickField == null || ignoredKeysField == null || 
                pressedKeysField == null || currentKeyboardStateField == null)
            {
                // Fallback if reflection fails - return current keyboard state
                __result = Keyboard.GetState();
                return false;
            }

            var keyState = (KeyboardState?)keyStateField.GetValue(__instance);
            var lastKeyStateTick = (int)(lastKeyStateTickField.GetValue(__instance) ?? -1);
            var ignoredKeys = (System.Collections.Generic.List<Keys>)(ignoredKeysField.GetValue(__instance) ?? new System.Collections.Generic.List<Keys>());
            var pressedKeys = (System.Collections.Generic.List<Keys>)(pressedKeysField.GetValue(__instance) ?? new System.Collections.Generic.List<Keys>());
            var currentKeyboardState = (KeyboardState)(currentKeyboardStateField.GetValue(__instance) ?? default(KeyboardState));

            // Replicate the original method logic but without the focus checks
            if (lastKeyStateTick != Game1.ticks || !keyState.HasValue)
            {
                if (ignoredKeys?.Count == 0)
                {
                    keyState = currentKeyboardState;
                }
                else if (ignoredKeys != null && pressedKeys != null)
                {
                    pressedKeys.Clear();
                    pressedKeys.AddRange(currentKeyboardState.GetPressedKeys());
                    
                    for (int i = 0; i < ignoredKeys.Count; i++)
                    {
                        Keys key = ignoredKeys[i];
                        if (!pressedKeys.Contains(key))
                        {
                            ignoredKeys.RemoveAt(i);
                            i--;
                        }
                    }
                    
                    for (int i = 0; i < pressedKeys.Count; i++)
                    {
                        Keys key = pressedKeys[i];
                        if (ignoredKeys.Contains(key))
                        {
                            pressedKeys.RemoveAt(i);
                            i--;
                        }
                    }
                    
                    keyState = new KeyboardState(pressedKeys.ToArray());
                }
                
                // Update the private fields
                keyStateField.SetValue(__instance, keyState);
                lastKeyStateTickField.SetValue(__instance, Game1.ticks);
            }

            __result = keyState ?? default(KeyboardState);
            return false; // Skip the original method
        }
    }
    
    /// <summary>
    /// Critical patch for Game1.IsActiveNoOverlay to ensure UpdateControlInput is called
    /// even when the game is minimized and virtual input is active.
    /// </summary>
    [HarmonyPatch]
    public class Game1_IsActiveNoOverlay_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game1), "IsActiveNoOverlay", MethodType.Getter)]
        public static void IsActiveNoOverlay_Postfix(ref bool __result)
        {
            // When virtual input is active, force the game to think it's active
            // This ensures UpdateControlInput gets called even when minimized
            if (VirtualInputSimulator.Active)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patches for Farmer.Halt() to prevent virtual input from being blocked during warps.
    /// When virtual input is active, we preserve movement capabilities during warp transitions.
    /// </summary>
    [HarmonyPatch]
    public class Farmer_Halt_Patches
    {
        [HarmonyPatch(typeof(Farmer), "Halt")]
        [HarmonyPostfix]
        public static void Halt_Postfix(Farmer __instance)
        {
            // If virtual input is active, restore movement capabilities after Halt()
            if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
            {
                // Restore CanMove - this is critical for virtual input to function
                __instance.CanMove = true;
                
                // Clear any movement freezing that might block virtual input
                __instance.freezePause = 0;
                
                // Ensure global freeze controls don't interfere with virtual input
                Game1.freezeControls = false;
            }
        }
    }

    /// <summary>
    /// Patches for Farmer.warpFarmer() to ensure virtual input continues working after warps.
    /// This handles both the Farmer instance method and ensures proper state restoration.
    /// </summary>
    [HarmonyPatch]
    public class Farmer_WarpFarmer_Patches
    {
        [HarmonyPatch(typeof(Farmer), "warpFarmer", new[] { typeof(Warp), typeof(int) })]
        [HarmonyPostfix]
        public static void WarpFarmer_Postfix(Farmer __instance, Warp w, int warp_collide_direction)
        {
            // If virtual input is active, ensure movement is restored after warp
            if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
            {
                // Use a delayed action to restore movement after the warp completes
                Game1.delayedActions.Add(new DelayedAction(100, delegate
                {
                    if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
                    {
                        __instance.CanMove = true;
                        __instance.freezePause = 0;
                        Game1.freezeControls = false;
                        
                        // Clear any movement directions that might have been set during halt
                        __instance.movementDirections.Clear();
                    }
                }));
            }
        }

        [HarmonyPatch(typeof(Farmer), "warpFarmer", new[] { typeof(Warp) })]
        [HarmonyPostfix]
        public static void WarpFarmer_Simple_Postfix(Farmer __instance, Warp w)
        {
            // Handle the simpler warpFarmer overload
            if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
            {
                Game1.delayedActions.Add(new DelayedAction(100, delegate
                {
                    if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
                    {
                        __instance.CanMove = true;
                        __instance.freezePause = 0;
                        Game1.freezeControls = false;
                        __instance.movementDirections.Clear();
                    }
                }));
            }
        }
    }

    /// <summary>
    /// Patches for Farmer.OnWarp() to handle virtual input state after warp completion.
    /// This ensures virtual input remains functional after the warp event chain completes.
    /// </summary>
    [HarmonyPatch]
    public class Farmer_OnWarp_Patches
    {
        [HarmonyPatch(typeof(Farmer), "OnWarp")]
        [HarmonyPostfix]
        public static void OnWarp_Postfix(Farmer __instance)
        {
            // Ensure virtual input works after warp completion
            if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
            {
                // Add a slightly longer delay to ensure all warp processes have completed
                Game1.delayedActions.Add(new DelayedAction(200, delegate
                {
                    if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
                    {
                        __instance.CanMove = true;
                        __instance.freezePause = 0;
                        Game1.freezeControls = false;
                        __instance.movementDirections.Clear();
                        
                        // Ensure the farmer is not stuck in any animation state
                        __instance.FarmerSprite.PauseForSingleAnimation = false;
                        __instance.UsingTool = false;
                        __instance.forceTimePass = false;
                    }
                }));
            }
        }
    }
}
