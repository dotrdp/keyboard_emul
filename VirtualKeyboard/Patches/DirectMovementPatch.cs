using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using VirtualKeyboard.Inputs;
using System;
using System.Reflection;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Diagnostic patch to verify method existence and test direct player movement
    /// </summary>
    [HarmonyPatch]
    internal class VirtualMovementPatch
    {
        /// <summary>
        /// Manual method targeting for UpdateControlInput
        /// </summary>
        static MethodBase TargetMethod()
        {
            // Try to find UpdateControlInput method
            var method = typeof(Game1).GetMethod("UpdateControlInput", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (method != null)
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Found UpdateControlInput method for patching", StardewModdingAPI.LogLevel.Info);
                return method;
            }
            
            ModEntry.Logger?.Log("VirtualMovementPatch: UpdateControlInput method not found, falling back to _update", StardewModdingAPI.LogLevel.Warn);
            
            // Fallback to _update method which is more accessible
            method = typeof(Game1).GetMethod("_update", BindingFlags.NonPublic | BindingFlags.Instance);
            return method;
        }

        /// <summary>
        /// Postfix to inject direct movement control
        /// </summary>
        [HarmonyPostfix]
        private static void Postfix(Microsoft.Xna.Framework.GameTime time)
        {
            if (!VirtualInputState.Active)
                return;

            var player = Game1.player;
            if (player == null || !player.CanMove)
                return;

            // Get virtual keyboard state
            var virtualState = VirtualInputState.GetKeyboard();
            bool hasMovement = false;

            ModEntry.Logger?.Log("VirtualMovementPatch: Checking virtual movement keys", StardewModdingAPI.LogLevel.Trace);

            // Direct movement injection - bypass all input processing
            if (virtualState.IsKeyDown(Keys.W) || virtualState.IsKeyDown(Keys.Up))
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Applying UP movement directly", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(1); // Up
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.D) || virtualState.IsKeyDown(Keys.Right))
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Applying RIGHT movement directly", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(2); // Right
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.S) || virtualState.IsKeyDown(Keys.Down))
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Applying DOWN movement directly", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(4); // Down
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.A) || virtualState.IsKeyDown(Keys.Left))
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Applying LEFT movement directly", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(8); // Left
                hasMovement = true;
            }

            if (hasMovement)
            {
                ModEntry.Logger?.Log("VirtualMovementPatch: Virtual movement applied successfully", StardewModdingAPI.LogLevel.Info);
            }
        }
    }

    /// <summary>
    /// Alternative approach: Patch the public _update method
    /// </summary>
    [HarmonyPatch(typeof(Game1), "_update")]
    internal class Game1UpdatePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!VirtualInputState.Active)
                return;

            // Only apply movement if we're in game mode 3 (playing)
            if (Game1.gameMode != 3)
                return;

            var player = Game1.player;
            if (player == null || !player.CanMove || Game1.activeClickableMenu != null)
                return;

            var virtualState = VirtualInputState.GetKeyboard();
            bool hasMovement = false;

            // Apply virtual movement
            if (virtualState.IsKeyDown(Keys.W) || virtualState.IsKeyDown(Keys.Up))
            {
                ModEntry.Logger?.Log("Game1UpdatePatch: Virtual UP movement", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(1);
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.D) || virtualState.IsKeyDown(Keys.Right))
            {
                ModEntry.Logger?.Log("Game1UpdatePatch: Virtual RIGHT movement", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(2);
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.S) || virtualState.IsKeyDown(Keys.Down))
            {
                ModEntry.Logger?.Log("Game1UpdatePatch: Virtual DOWN movement", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(4);
                hasMovement = true;
            }

            if (virtualState.IsKeyDown(Keys.A) || virtualState.IsKeyDown(Keys.Left))
            {
                ModEntry.Logger?.Log("Game1UpdatePatch: Virtual LEFT movement", StardewModdingAPI.LogLevel.Debug);
                player.setMoving(8);
                hasMovement = true;
            }

            if (hasMovement)
            {
                ModEntry.Logger?.Log("Game1UpdatePatch: Applied virtual movement to player", StardewModdingAPI.LogLevel.Info);
            }
        }
    }
}
