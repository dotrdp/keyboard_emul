using System;
using StardewModdingAPI;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to show comprehensive auto-skip status
    /// </summary>
    public class AutoSkipStatusCommand : IConsoleCommand
    {
        public string Name => "autoskip_status";
        public string Description => "Show detailed auto-skip status and settings";
        public string Usage => "autoskip_status";

        public string Execute(string[] args)
        {
            var settings = AutoSkipCommands.GetSettings();
            
            return $"🚀 VirtualKeyboard Auto-Skip Status 🚀\n" +
                   $"═══════════════════════════════════════\n" +
                   $"Auto-Skip Events:    {(settings.events ? "✅ ENABLED" : "❌ DISABLED")}\n" +
                   $"Auto-Skip Dialogues: {(settings.dialogues ? "✅ ENABLED" : "❌ DISABLED")}\n" +
                   $"Auto-Skip Letters:   {(settings.dialogues ? "✅ ENABLED" : "❌ DISABLED")} (uses dialogue setting)\n" +
                   $"Dialogue Skip Speed: {settings.speed} lines per tick\n" +
                   $"═══════════════════════════════════════\n" +
                   $"Commands:\n" +
                   $"  autoskip on/off - Toggle all auto-skip features\n" +
                   $"  autoskip on dialogues [speed] - Set dialogue skip speed\n" +
                   $"  skip dialogue/event/letter - Manual skip commands\n" +
                   $"\n" +
                   $"Note: Auto-skip is {(settings.events && settings.dialogues ? "FULLY ACTIVE" : "PARTIALLY ACTIVE")} - " +
                   $"all events, dialogues, and letters will be skipped automatically!";
        }
    }
}
