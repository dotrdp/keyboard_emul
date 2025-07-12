using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewValley;
using VirtualKeyboard.Inputs;
using StardewModdingAPI;
using System.Reflection;
using System.Linq;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Ultra-aggressive input override patches
    /// These patches override input at every possible level to ensure virtual keys work
    /// </summary>
    
    /// <summary>
    /// Override the Game1 input fields directly every frame
    /// </summary>
    [HarmonyPatch(typeof(Game1), "Update")]
    [HarmonyPriority(Priority.First)]
    public class Game1_Update_InputOverride
    {
        private static readonly FieldInfo? CurrentKBField = AccessTools.Field(typeof(Game1), "currentKBState");
        private static readonly FieldInfo? OldKBField = AccessTools.Field(typeof(Game1), "oldKBState");
        private static readonly FieldInfo? InputField = AccessTools.Field(typeof(Game1), "input");

        [HarmonyPrefix]
        public static void Prefix()
        {
            if (!VirtualInputState.Active) return;

            var virtualKeyboard = VirtualInputState.GetKeyboard();
            
            try
            {
                // Override Game1 static keyboard states
                CurrentKBField?.SetValue(null, virtualKeyboard);
                OldKBField?.SetValue(null, virtualKeyboard);
                
                // Override Game1.input if it exists
                if (InputField?.GetValue(null) is InputState inputState)
                {
                    OverrideInputState(inputState, virtualKeyboard);
                }
                
                ModEntry.Logger?.Log($"Game1.Update: Aggressively overrode all keyboard states", LogLevel.Trace);
            }
            catch (System.Exception ex)
            {
                ModEntry.Logger?.Log($"Game1.Update override error: {ex.Message}", LogLevel.Error);
            }
        }

        private static void OverrideInputState(InputState inputState, KeyboardState virtualKeyboard)
        {
            var currentKBField = AccessTools.Field(typeof(InputState), "currentKeyboardState");
            var lastKBField = AccessTools.Field(typeof(InputState), "lastKeyboardState");
            
            currentKBField?.SetValue(inputState, virtualKeyboard);
            lastKBField?.SetValue(inputState, virtualKeyboard);
        }
    }

    /// <summary>
    /// Override all player input checking methods
    /// Only patch methods that actually exist
    /// </summary>
    [HarmonyPatch(typeof(Farmer))]
    public class Farmer_InputOverrides
    {
        // Remove the moveUp, moveDown, etc. patches since those methods don't exist
        // Focus on the methods that do exist
        
        [HarmonyPatch("getMovementSpeed")]
        [HarmonyPrefix]
        public static void GetMovementSpeed_Prefix()
        {
            if (VirtualInputState.Active)
            {
                // Force virtual keyboard state before movement speed calculation
                try
                {
                    var currentKBField = AccessTools.Field(typeof(Game1), "currentKBState");
                    currentKBField?.SetValue(null, VirtualInputState.GetKeyboard());
                }
                catch { /* ignore reflection errors */ }
            }
        }
    }

    /// <summary>
    /// Emergency override - patch the actual movement logic
    /// </summary>
    [HarmonyPatch(typeof(Farmer), "MovePosition")]
    [HarmonyPriority(Priority.First)]
    public class Farmer_MovePosition_Emergency
    {
        [HarmonyPrefix]
        public static bool Prefix(Farmer __instance, GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (!VirtualInputState.Active) return true;

            // Manually handle movement for virtual keys
            bool moved = false;
            
            if (VirtualInputState.IsKeyDown(Keys.W))
            {
                // Set moving up using reflection or direct field access
                try 
                {
                    var movingUpField = AccessTools.Field(typeof(Farmer), "movingUp");
                    movingUpField?.SetValue(__instance, true);
                    moved = true;
                    ModEntry.Logger?.Log("Emergency: Set movingUp=true via virtual W", LogLevel.Debug);
                }
                catch { /* ignore reflection errors */ }
            }
            if (VirtualInputState.IsKeyDown(Keys.S))
            {
                try 
                {
                    var movingDownField = AccessTools.Field(typeof(Farmer), "movingDown");
                    movingDownField?.SetValue(__instance, true);
                    moved = true;
                    ModEntry.Logger?.Log("Emergency: Set movingDown=true via virtual S", LogLevel.Debug);
                }
                catch { /* ignore reflection errors */ }
            }
            if (VirtualInputState.IsKeyDown(Keys.A))
            {
                try 
                {
                    var movingLeftField = AccessTools.Field(typeof(Farmer), "movingLeft");
                    movingLeftField?.SetValue(__instance, true);
                    moved = true;
                    ModEntry.Logger?.Log("Emergency: Set movingLeft=true via virtual A", LogLevel.Debug);
                }
                catch { /* ignore reflection errors */ }
            }
            if (VirtualInputState.IsKeyDown(Keys.D))
            {
                try 
                {
                    var movingRightField = AccessTools.Field(typeof(Farmer), "movingRight");
                    movingRightField?.SetValue(__instance, true);
                    moved = true;
                    ModEntry.Logger?.Log("Emergency: Set movingRight=true via virtual D", LogLevel.Debug);
                }
                catch { /* ignore reflection errors */ }
            }

            if (moved)
            {
                // Let the original method run with our movement flags set
                return true;
            }

            return true; // Always let original method run
        }
    }
}
