using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using VirtualKeyboard.Console;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Patches for automatically skipping events and dialogues when they are created
    /// </summary>
    [HarmonyPatch]
    public class EventSkipPatches
    {
        /// <summary>
        /// Patch the Event constructor to immediately skip events if auto-skip is enabled
        /// </summary>
        [HarmonyPatch(typeof(Event), MethodType.Constructor, new[] { typeof(string) })]
        [HarmonyPostfix]
        public static void Event_Constructor_Postfix(Event __instance)
        {
            try
            {
                ModEntry.Monitor.Log($"Event constructor called with event: {__instance?.id}", LogLevel.Debug);
                var (autoSkipEvents, _, _) = AutoSkipCommands.GetSettings();
                ModEntry.Monitor.Log($"Auto-skip events setting: {autoSkipEvents}", LogLevel.Debug);
                
                if (autoSkipEvents)
                {
                    ModEntry.Monitor.Log($"Auto-skipping event (constructor): {__instance?.id}", LogLevel.Info);
                    
                    // Skip the event immediately
                    Game1.delayedActions.Add(new DelayedAction(50, delegate
                    {
                        try
                        {
                            if (Game1.eventUp && Game1.currentLocation?.currentEvent != null)
                            {
                                ModEntry.Monitor.Log("Executing event skip...", LogLevel.Info);
                                Game1.currentLocation.currentEvent.skipEvent();
                            }
                            else
                            {
                                ModEntry.Monitor.Log("Event skip conditions not met", LogLevel.Debug);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error auto-skipping event: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in Event constructor patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Patch the Event constructor with additional parameters to immediately skip events if auto-skip is enabled
        /// </summary>
        [HarmonyPatch(typeof(Event), MethodType.Constructor, new[] { typeof(string), typeof(string), typeof(string) })]
        [HarmonyPostfix]
        public static void Event_Constructor_Full_Postfix(Event __instance)
        {
            try
            {
                var (autoSkipEvents, _, _) = AutoSkipCommands.GetSettings();
                if (autoSkipEvents)
                {
                    ModEntry.Monitor.Log($"Auto-skipping event (full constructor): {__instance?.id}", LogLevel.Debug);
                    
                    // Skip the event immediately
                    Game1.delayedActions.Add(new DelayedAction(50, delegate
                    {
                        try
                        {
                            if (Game1.eventUp && Game1.currentLocation?.currentEvent != null)
                            {
                                ModEntry.Monitor.Log("Executing event skip...", LogLevel.Debug);
                                Game1.currentLocation.currentEvent.skipEvent();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error auto-skipping event: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in Event full constructor patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Patch when Game1.eventUp is set to true to skip events immediately
        /// </summary>
        [HarmonyPatch(typeof(Game1), "set_eventUp")]
        [HarmonyPostfix]
        public static void Game1_SetEventUp_Postfix(bool value)
        {
            try
            {
                var (autoSkipEvents, _, _) = AutoSkipCommands.GetSettings();
                if (value && autoSkipEvents)
                {
                    ModEntry.Monitor.Log("Game1.eventUp set to true, auto-skipping...", LogLevel.Debug);
                    
                    // Skip the event with a small delay to ensure it's fully initialized
                    Game1.delayedActions.Add(new DelayedAction(100, delegate
                    {
                        try
                        {
                            if (Game1.eventUp && Game1.currentLocation?.currentEvent != null)
                            {
                                ModEntry.Monitor.Log("Executing delayed event skip...", LogLevel.Debug);
                                Game1.currentLocation.currentEvent.skipEvent();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error in delayed event skip: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in Game1.eventUp patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Patch DialogueBox constructor to immediately skip dialogues if auto-skip is enabled
        /// </summary>
        [HarmonyPatch(typeof(DialogueBox), MethodType.Constructor, new[] { typeof(string) })]
        [HarmonyPostfix]
        public static void DialogueBox_Constructor_Postfix(DialogueBox __instance)
        {
            try
            {
                ModEntry.Monitor.Log("DialogueBox constructor (string) called", LogLevel.Debug);
                var (_, autoSkipDialogues, dialogueSpeed) = AutoSkipCommands.GetSettings();
                ModEntry.Monitor.Log($"Auto-skip dialogues setting: {autoSkipDialogues}", LogLevel.Debug);
                
                if (autoSkipDialogues)
                {
                    ModEntry.Monitor.Log("Auto-skipping dialogue (string constructor)", LogLevel.Info);
                    
                    // Skip the dialogue with a small delay
                    Game1.delayedActions.Add(new DelayedAction(dialogueSpeed, delegate
                    {
                        try
                        {
                            if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                            {
                                ModEntry.Monitor.Log("Executing dialogue skip...", LogLevel.Info);
                                // Simulate left click to advance dialogue
                                dialogueBox.receiveLeftClick(Game1.viewport.Width / 2, Game1.viewport.Height - 64);
                            }
                            else
                            {
                                ModEntry.Monitor.Log("Dialogue skip conditions not met", LogLevel.Debug);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error auto-skipping dialogue: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in DialogueBox constructor patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Patch DialogueBox constructor with Dialogue parameter to immediately skip dialogues if auto-skip is enabled
        /// </summary>
        [HarmonyPatch(typeof(DialogueBox), MethodType.Constructor, new[] { typeof(Dialogue) })]
        [HarmonyPostfix]
        public static void DialogueBox_Dialogue_Constructor_Postfix(DialogueBox __instance)
        {
            try
            {
                var (_, autoSkipDialogues, dialogueSpeed) = AutoSkipCommands.GetSettings();
                if (autoSkipDialogues)
                {
                    ModEntry.Monitor.Log("Auto-skipping dialogue (Dialogue constructor)", LogLevel.Debug);
                    
                    // Skip the dialogue with a small delay
                    Game1.delayedActions.Add(new DelayedAction(dialogueSpeed, delegate
                    {
                        try
                        {
                            if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                            {
                                ModEntry.Monitor.Log("Executing dialogue skip...", LogLevel.Debug);
                                // Simulate left click to advance dialogue
                                dialogueBox.receiveLeftClick(Game1.viewport.Width / 2, Game1.viewport.Height - 64);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error auto-skipping dialogue: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in DialogueBox Dialogue constructor patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Patch when Game1.activeClickableMenu is set to a DialogueBox to auto-skip
        /// </summary>
        [HarmonyPatch(typeof(Game1), "set_activeClickableMenu")]
        [HarmonyPostfix]
        public static void Game1_SetActiveClickableMenu_Postfix(IClickableMenu value)
        {
            try
            {
                var (_, autoSkipDialogues, dialogueSpeed) = AutoSkipCommands.GetSettings();
                if (value is DialogueBox && autoSkipDialogues)
                {
                    ModEntry.Monitor.Log("DialogueBox set as activeClickableMenu, auto-skipping...", LogLevel.Debug);
                    
                    // Skip the dialogue with a small delay
                    Game1.delayedActions.Add(new DelayedAction(dialogueSpeed, delegate
                    {
                        try
                        {
                            if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                            {
                                ModEntry.Monitor.Log("Executing delayed dialogue skip...", LogLevel.Debug);
                                // Simulate left click to advance dialogue
                                dialogueBox.receiveLeftClick(Game1.viewport.Width / 2, Game1.viewport.Height - 64);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ModEntry.Monitor.Log($"Error in delayed dialogue skip: {ex.Message}", LogLevel.Error);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                ModEntry.Monitor.Log($"Error in Game1.activeClickableMenu patch: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
