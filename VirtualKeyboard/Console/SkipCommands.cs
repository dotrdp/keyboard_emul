using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Console commands for skipping events and dialogues
    /// </summary>
    public class SkipCommands : IConsoleCommand
    {
        public string Name => "skip";
        public string Description => "Skip events and dialogues";
        public string Usage => "skip <event|dialogue|letter> [speed]";

        public string Execute(string[] args)
        {
            if (args.Length == 0)
            {
                return "Usage: skip <event|dialogue|letter> [speed]\n" +
                       "  event - Skip current event if skippable\n" +
                       "  dialogue - Skip current dialogue box\n" +
                       "  letter - Skip current letter/mail\n" +
                       "  speed (optional) - Number of dialogue lines to skip (default: 1)";
            }

            if (!Context.IsWorldReady)
            {
                return "Error: No save file loaded";
            }

            string command = args[0].ToLower();
            int speed = 1;

            if (args.Length > 1 && int.TryParse(args[1], out int parsedSpeed))
            {
                speed = Math.Max(1, parsedSpeed);
            }

            switch (command)
            {
                case "event":
                    return SkipEvent();

                case "dialogue":
                    return SkipDialogue(speed);

                case "letter":
                    return SkipLetter(speed);

                default:
                    return $"Unknown command: {command}. Use 'skip' for usage info.";
            }
        }

        private string SkipEvent()
        {
            if (Game1.CurrentEvent != null && Game1.CurrentEvent.skippable)
            {
                Game1.CurrentEvent.skipEvent();
                return "Event skipped successfully";
            }
            else if (Game1.CurrentEvent != null)
            {
                return "Current event is not skippable";
            }
            else
            {
                return "No event is currently active";
            }
        }

        private string SkipDialogue(int speed)
        {
            if (!(Game1.activeClickableMenu is DialogueBox box))
            {
                return "No dialogue box is currently active";
            }

            int runs = 0;
            box.characterIndexInDialogue = box.getCurrentString().Length - 1;

            while (speed > runs && Game1.activeClickableMenu is DialogueBox box2 && !box2.isQuestion)
            {
                runs++;
                box2.transitioning = false;
                box2.safetyTimer = 0;
                box2.receiveLeftClick(0, 0, true);
                box2.characterIndexInDialogue = box2.getCurrentString().Length - 1;
            }

            return $"Skipped {runs} dialogue line(s)";
        }

        private string SkipLetter(int speed)
        {
            if (!(Game1.activeClickableMenu is LetterViewerMenu m))
            {
                return "No letter is currently open";
            }

            int runs = 0;

            // Skip through pages
            while (speed + 1 > runs && m.page < m.mailMessage.Count - 1)
            {
                runs++;
                m.page++;
                Game1.playSound("shwip");
                m.OnPageChange();
            }

            // If we've gone through all pages and there's no interactable content, close the letter
            if (Game1.activeClickableMenu is LetterViewerMenu m2 && !m2.HasInteractable())
            {
                if (speed > runs)
                {
                    Game1.playSound("bigDeSelect");
                    if (!m2.isFromCollection)
                    {
                        m2.exitThisMenu(m2.ShouldPlayExitSound());
                        return $"Skipped {runs} page(s) and closed letter";
                    }
                    else
                    {
                        m2.destroy = true;
                        return $"Skipped {runs} page(s) and destroyed collection letter";
                    }
                }
            }

            return $"Skipped {runs} page(s) in letter";
        }
    }

    /// <summary>
    /// Console commands for automatic skipping functionality
    /// </summary>
    public class AutoSkipCommands : IConsoleCommand
    {
        public string Name => "autoskip";
        public string Description => "Enable/disable automatic skipping of events and dialogues";
        public string Usage => "autoskip <on|off|status> [type]";

        private static bool AutoSkipEvents = true;  // Auto-enabled
        private static bool AutoSkipDialogues = true;  // Auto-enabled
        private static int DialogueSpeed = 10;  // Skip fast by default

        public string Execute(string[] args)
        {
            if (args.Length == 0)
            {
                return "Usage: autoskip <on|off|status> [type] [speed]\n" +
                       "  on/off - Enable or disable auto-skipping\n" +
                       "  type - 'events', 'dialogues', or 'all' (default: all)\n" +
                       "  speed - Dialogue skip speed (default: 1)\n" +
                       "Examples:\n" +
                       "  autoskip on - Enable auto-skip for everything\n" +
                       "  autoskip on dialogues 3 - Enable dialogue auto-skip with speed 3\n" +
                       "  autoskip off events - Disable event auto-skip only\n" +
                       "  autoskip status - Show current settings";
            }

            string command = args[0].ToLower();

            if (command == "status")
            {
                return $"Auto-skip Status:\n" +
                       $"  Events: {(AutoSkipEvents ? "ON" : "OFF")}\n" +
                       $"  Dialogues: {(AutoSkipDialogues ? "ON" : "OFF")}\n" +
                       $"  Dialogue Speed: {DialogueSpeed}";
            }

            bool enable = command == "on";
            string type = args.Length > 1 ? args[1].ToLower() : "all";
            int speed = DialogueSpeed;

            if (args.Length > 2 && int.TryParse(args[2], out int parsedSpeed))
            {
                speed = Math.Max(1, parsedSpeed);
                DialogueSpeed = speed;
            }

            switch (type)
            {
                case "events":
                    AutoSkipEvents = enable;
                    return $"Auto-skip events: {(enable ? "ENABLED" : "DISABLED")}";

                case "dialogues":
                    AutoSkipDialogues = enable;
                    DialogueSpeed = speed;
                    return $"Auto-skip dialogues: {(enable ? "ENABLED" : "DISABLED")} (speed: {speed})";

                case "all":
                    AutoSkipEvents = enable;
                    AutoSkipDialogues = enable;
                    DialogueSpeed = speed;
                    return $"Auto-skip ALL: {(enable ? "ENABLED" : "DISABLED")} (dialogue speed: {speed})";

                default:
                    return $"Unknown type: {type}. Use 'events', 'dialogues', or 'all'";
            }
        }

        /// <summary>
        /// Check and auto-skip events if enabled
        /// </summary>
        public static void CheckAutoSkipEvent()
        {
            try
            {
                var (autoSkipEvents, _, _) = GetSettings();
                
                // Add periodic debug logging to show we're checking
                if (autoSkipEvents && Game1.eventUp)
                {
                    ModEntry.Monitor.Log($"CheckAutoSkipEvent: eventUp={Game1.eventUp}, CurrentEvent={Game1.CurrentEvent?.id}, currentEvent={Game1.currentLocation?.currentEvent?.id}", LogLevel.Debug);
                }
                
                if (autoSkipEvents && Game1.CurrentEvent != null && Game1.CurrentEvent.skippable)
                {
                    ModEntry.Monitor.Log("Auto-skipping event via CurrentEvent...", LogLevel.Info);
                    Game1.CurrentEvent.skipEvent();
                    ModEntry.Monitor.Log("Auto-skipped event via CurrentEvent", LogLevel.Info);
                }
                else if (autoSkipEvents && Game1.eventUp && Game1.currentLocation?.currentEvent != null)
                {
                    ModEntry.Monitor.Log("Auto-skipping event via currentEvent...", LogLevel.Info);
                    Game1.currentLocation.currentEvent.skipEvent();
                    ModEntry.Monitor.Log("Auto-skipped event via currentEvent", LogLevel.Info);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Monitor.Log($"Error auto-skipping event: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Check and auto-skip dialogues if enabled
        /// </summary>
        public static void CheckAutoSkipDialogue()
        {
            try
            {
                var (_, autoSkipDialogues, dialogueSpeed) = GetSettings();
                if (!autoSkipDialogues) return;

                // Add periodic debug logging to show we're checking
                if (Game1.activeClickableMenu is DialogueBox)
                {
                    ModEntry.Monitor.Log($"CheckAutoSkipDialogue: Found DialogueBox, isQuestion={(Game1.activeClickableMenu as DialogueBox)?.isQuestion}", LogLevel.Debug);
                }

                // Handle dialogue boxes
                if (Game1.activeClickableMenu is DialogueBox box && !box.isQuestion)
                {
                    ModEntry.Monitor.Log("Auto-skipping dialogue via polling...", LogLevel.Info);
                    
                    // Immediately show full text
                    box.characterIndexInDialogue = box.getCurrentString().Length - 1;
                    
                    // Skip multiple dialogue lines rapidly
                    int runs = 0;
                    while (dialogueSpeed > runs && Game1.activeClickableMenu is DialogueBox box2 && !box2.isQuestion)
                    {
                        runs++;
                        box2.transitioning = false;
                        box2.safetyTimer = 0;
                        box2.receiveLeftClick(Game1.viewport.Width / 2, Game1.viewport.Height - 64, true);
                        
                        // Ensure text is fully displayed
                        if (Game1.activeClickableMenu is DialogueBox box3)
                        {
                            box3.characterIndexInDialogue = box3.getCurrentString().Length - 1;
                        }
                    }

                    if (runs > 0)
                    {
                        ModEntry.Monitor.Log($"Auto-skipped {runs} dialogue line(s) via polling", LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Monitor.Log($"Error auto-skipping dialogue: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Check and auto-skip letters if enabled
        /// </summary>
        public static void CheckAutoSkipLetter()
        {
            if (!AutoSkipDialogues) return; // Use same setting as dialogues

            if (Game1.activeClickableMenu is LetterViewerMenu m)
            {
                int runs = 0;

                // Skip through all pages rapidly
                while (m.page < m.mailMessage.Count - 1)
                {
                    runs++;
                    m.page++;
                    Game1.playSound("shwip");
                    m.OnPageChange();
                }

                // Auto-close letter if no interactable content
                if (Game1.activeClickableMenu is LetterViewerMenu m2 && !m2.HasInteractable())
                {
                    Game1.playSound("bigDeSelect");
                    if (!m2.isFromCollection)
                    {
                        m2.exitThisMenu(m2.ShouldPlayExitSound());
                    }
                    else
                    {
                        m2.destroy = true;
                    }
                    runs++;
                }

                if (runs > 0)
                {
                    ModEntry.Monitor.Log($"Auto-skipped letter with {runs} action(s)", LogLevel.Debug);
                }
            }
        }

        /// <summary>
        /// Get current auto-skip settings for status commands
        /// </summary>
        public static (bool events, bool dialogues, int speed) GetSettings()
        {
            return (AutoSkipEvents, AutoSkipDialogues, DialogueSpeed);
        }
    }
}
