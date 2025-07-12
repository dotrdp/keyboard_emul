using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Critical patch that directly intercepts SMAPI's input helper methods.
    /// This is the most likely place where virtual keys need to be injected.
    /// </summary>
    public class CriticalInputPatch : IPatch
    {
        public override string Name => "Critical SMAPI Input Override";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Try to find and patch the actual input helper that mods use
                var smapiModHelper = typeof(StardewModdingAPI.IModHelper);
                var inputHelperType = Type.GetType("StardewModdingAPI.Framework.ModHelpers.InputHelper, StardewModdingAPI");
                
                if (inputHelperType != null)
                {
                    // Patch the GetCursorPosition method
                    var getCursorMethod = AccessTools.Method(inputHelperType, "GetCursorPosition");
                    if (getCursorMethod != null)
                    {
                        harmony.Patch(
                            original: getCursorMethod,
                            postfix: new HarmonyMethod(this.GetType(), nameof(GetCursorPostfix))
                        );
                        Trace("Patched InputHelper.GetCursorPosition");
                    }

                    // More importantly, patch the IsDown method if it exists
                    var isDownMethod = AccessTools.Method(inputHelperType, "IsDown", new[] { typeof(SButton) });
                    if (isDownMethod != null)
                    {
                        harmony.Patch(
                            original: isDownMethod,
                            postfix: new HarmonyMethod(this.GetType(), nameof(IsDownPostfix))
                        );
                        Trace("✅ Successfully patched InputHelper.IsDown - This should make virtual keys work!");
                    }

                    // Also try IsSuppressed method
                    var isSuppressedMethod = AccessTools.Method(inputHelperType, "IsSuppressed", new[] { typeof(SButton) });
                    if (isSuppressedMethod != null)
                    {
                        harmony.Patch(
                            original: isSuppressedMethod,
                            prefix: new HarmonyMethod(this.GetType(), nameof(IsSuppressedPrefix))
                        );
                        Trace("Patched InputHelper.IsSuppressed");
                    }
                }

                // Also try to patch the Game1.IsActive property which affects input
                var isActiveProperty = AccessTools.Property(typeof(Game1), "IsActive");
                if (isActiveProperty?.GetMethod != null)
                {
                    harmony.Patch(
                        original: isActiveProperty.GetMethod,
                        postfix: new HarmonyMethod(this.GetType(), nameof(IsActivePostfix))
                    );
                    Trace("Patched Game1.IsActive");
                }

                Info("Critical input patches applied");
            }
            catch (Exception ex)
            {
                Error($"Failed to apply critical input patches: {ex.Message}");
            }
        }

        public static void GetCursorPostfix()
        {
            // Just a marker that we're in the input system
        }

        public static void IsDownPostfix(SButton button, ref bool __result)
        {
            // This is the critical patch - override IsDown for virtual keys
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                Info($"🎯 CRITICAL: Virtual key override successful - {button} is DOWN");
            }
        }

        public static bool IsSuppressedPrefix(SButton button, ref bool __result)
        {
            // Don't suppress virtual keys
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = false;
                return false; // Skip original method
            }
            return true; // Continue with original method
        }

        public static void IsActivePostfix(ref bool __result)
        {
            // Ensure the game thinks it's active when we have virtual keys
            if (KeybindManager.HasActiveKeybinds)
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Patch to intercept player movement specifically.
    /// This targets the player's actual movement logic.
    /// </summary>
    public class PlayerMovementPatch : IPatch
    {
        public override string Name => "Player Movement Override";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Find the Farmer class movement methods
                var farmerType = typeof(StardewValley.Farmer);
                
                // Patch the MovePosition method if it exists
                var movePositionMethod = AccessTools.Method(farmerType, "MovePosition");
                if (movePositionMethod != null)
                {
                    harmony.Patch(
                        original: movePositionMethod,
                        prefix: new HarmonyMethod(this.GetType(), nameof(MovePositionPrefix))
                    );
                    Trace("Patched Farmer.MovePosition");
                }

                // Patch the getMovementSpeed method
                var getMovementSpeedMethod = AccessTools.Method(farmerType, "getMovementSpeed");
                if (getMovementSpeedMethod != null)
                {
                    harmony.Patch(
                        original: getMovementSpeedMethod,
                        postfix: new HarmonyMethod(this.GetType(), nameof(GetMovementSpeedPostfix))
                    );
                    Trace("Patched Farmer.getMovementSpeed");
                }

                Info("Player movement patches applied");
            }
            catch (Exception ex)
            {
                Warn($"Could not patch player movement: {ex.Message}");
            }
        }

        public static bool MovePositionPrefix(StardewValley.Farmer __instance)
        {
            // Let virtual movement through
            if (KeybindManager.HasActiveKeybinds)
            {
                Info("Virtual keybinds active during player movement");
            }
            return true; // Continue with original method
        }

        public static void GetMovementSpeedPostfix(ref float __result)
        {
            // Ensure movement speed is available for virtual movement
            if (KeybindManager.HasActiveKeybinds)
            {
                // Don't modify speed, just log that we're in movement code
                Trace($"Movement speed: {__result} (virtual keys active)");
            }
        }
    }
}
