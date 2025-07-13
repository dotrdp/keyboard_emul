using System;
using HarmonyLib;
using StardewValley;
using VirtualKeyboard.Simulation;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches to ensure virtual input works immediately during game startup and initialization
    /// </summary>
    [HarmonyPatch]
    public class StartupPatches
    {
        /// <summary>
        /// Patch Game1._update to ensure virtual input is processed from the very first frame
        /// </summary>
        [HarmonyPatch(typeof(Game1), "_update")]
        [HarmonyPrefix]
        public static void Update_Prefix()
        {
            // Ensure virtual input is available from the very first update cycle
            if (VirtualInputSimulator.Active && Game1.player != null)
            {
                // Remove any artificial barriers to input processing
                if (Game1.freezeControls && !Game1.eventUp && !Game1.isFestival())
                {
                    Game1.freezeControls = false;
                }
                
                // Ensure player can move if they should be able to
                if (Game1.player.CanMove == false && !Game1.player.UsingTool && 
                    Game1.player.freezePause <= 0 && !Game1.eventUp)
                {
                    Game1.player.CanMove = true;
                }
            }
        }

        /// <summary>
        /// Patch Game1.Initialize to set up virtual input as early as possible
        /// </summary>
        [HarmonyPatch(typeof(Game1), "Initialize")]
        [HarmonyPostfix]
        public static void Initialize_Postfix()
        {
            if (VirtualInputSimulator.Active)
            {
                ModEntry.Monitor.Log("Game1.Initialize completed - virtual input ready", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Patch for when the game loads content - another critical initialization point
        /// </summary>
        [HarmonyPatch(typeof(Game1), "LoadContent")]
        [HarmonyPostfix]
        public static void LoadContent_Postfix()
        {
            if (VirtualInputSimulator.Active)
            {
                ModEntry.Monitor.Log("Game1.LoadContent completed - virtual input ready", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Patch farmer initialization to ensure input works immediately for new characters
        /// </summary>
        [HarmonyPatch(typeof(Farmer), MethodType.Constructor, new Type[] { })]
        [HarmonyPostfix]
        public static void Farmer_Constructor_Postfix(Farmer __instance)
        {
            if (VirtualInputSimulator.Active)
            {
                // Ensure the new farmer can immediately receive input
                __instance.CanMove = true;
                __instance.freezePause = 0;
            }
        }

        /// <summary>
        /// Patch for Day startup to ensure input works at the beginning of each day
        /// </summary>
        [HarmonyPatch(typeof(Game1), "newDayAfterFade")]
        [HarmonyPostfix]
        public static void NewDayAfterFade_Postfix()
        {
            if (VirtualInputSimulator.Active && Game1.player != null)
            {
                // Ensure input works immediately after new day fade
                Game1.delayedActions.Add(new DelayedAction(50, delegate
                {
                    if (VirtualInputSimulator.Active && Game1.player != null)
                    {
                        Game1.player.CanMove = true;
                        Game1.player.freezePause = 0;
                        Game1.freezeControls = false;
                        Game1.player.movementDirections.Clear();
                    }
                }));
            }
        }

        /// <summary>
        /// Patch to bypass focus requirements for input when virtual input is active
        /// This is critical for startup scenarios where focus might not be established yet
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsActiveNoOverlay", MethodType.Getter)]
        [HarmonyPostfix]
        public static void IsActiveNoOverlay_Postfix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                // When virtual input is active, always consider the game "active" for input purposes
                __result = true;
            }
        }

        /// <summary>
        /// Patch HasKeyboardFocus to ensure input is processed when virtual input is active
        /// </summary>
        [HarmonyPatch(typeof(Game1), "HasKeyboardFocus")]
        [HarmonyPostfix]
        public static void HasKeyboardFocus_Postfix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                // When virtual input is active, always report that we have keyboard focus
                __result = true;
            }
        }

        /// <summary>
        /// Patch IsMainInstance to ensure input processing works for virtual input
        /// </summary>
        [HarmonyPatch(typeof(Game1), "IsMainInstance", MethodType.Getter)]
        [HarmonyPostfix]
        public static void IsMainInstance_Postfix(ref bool __result)
        {
            if (VirtualInputSimulator.Active)
            {
                // Ensure virtual input is always treated as the main instance
                __result = true;
            }
        }
    }
}
