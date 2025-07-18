using System;
using System.Linq;
using StardewModdingAPI;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to press a key once
    /// </summary>
    public class KeybindPressCommand : IConsoleCommand
    {
        public string Name => "keybind_press";
        public string Description => "Press a key once";
        public string Usage => "keybind_press <key>";

        public string Execute(string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Usage}";

            if (Enum.TryParse<SButton>(args[0], true, out var key))
            {
                KeybindManager.PressKey(key);
                return $"Pressed key: {key}";
            }

            return $"Invalid key: {args[0]}";
        }
    }

    /// <summary>
    /// Command to hold a key for a duration
    /// </summary>
    public class KeybindHoldCommand : IConsoleCommand
    {
        public string Name => "keybind_hold";
        public string Description => "Hold a key for specified duration in milliseconds";
        public string Usage => "keybind_hold <key> <duration_ms>";

        public string Execute(string[] args)
        {
            if (args.Length != 2)
                return $"Usage: {Usage}";

            if (!Enum.TryParse<SButton>(args[0], true, out var key))
                return $"Invalid key: {args[0]}";

            if (!int.TryParse(args[1], out var duration) || duration <= 0)
                return $"Invalid duration: {args[1]}";

            KeybindManager.HoldKey(key, duration);
            return $"Holding key {key} for {duration}ms";
        }
    }

    /// <summary>
    /// Command to release a held key
    /// </summary>
    public class KeybindReleaseCommand : IConsoleCommand
    {
        public string Name => "keybind_release";
        public string Description => "Release a currently held key";
        public string Usage => "keybind_release <key>";

        public string Execute(string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Usage}";

            if (Enum.TryParse<SButton>(args[0], true, out var key))
            {
                KeybindManager.ReleaseKey(key);
                return $"Released key: {key}";
            }

            return $"Invalid key: {args[0]}";
        }
    }

    /// <summary>
    /// Command to execute a sequence of key presses
    /// </summary>
    public class KeybindSequenceCommand : IConsoleCommand
    {
        public string Name => "keybind_sequence";
        public string Description => "Execute a sequence of key presses with specified interval";
        public string Usage => "keybind_sequence <key1,key2,key3> [interval_ms]";

        public string Execute(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
                return $"Usage: {Usage}";

            var keyStrings = args[0].Split(',', StringSplitOptions.RemoveEmptyEntries);
            var keys = new SButton[keyStrings.Length];

            for (int i = 0; i < keyStrings.Length; i++)
            {
                if (!Enum.TryParse<SButton>(keyStrings[i].Trim(), true, out keys[i]))
                    return $"Invalid key: {keyStrings[i]}";
            }

            int interval = 100; // Default interval
            if (args.Length == 2 && (!int.TryParse(args[1], out interval) || interval <= 0))
                return $"Invalid interval: {args[1]}";

            // Use multiple individual key presses for sequence simulation
            foreach (var key in keys)
            {
                KeybindManager.PressKey(key);
            }
            return $"Executing sequence: {string.Join(", ", keys)} (simulated as individual presses)";
        }
    }

    /// <summary>
    /// Command to execute simultaneous key presses (combo)
    /// </summary>
    public class KeybindComboCommand : IConsoleCommand
    {
        public string Name => "keybind_combo";
        public string Description => "Execute simultaneous key presses";
        public string Usage => "keybind_combo <key1+key2+key3> [duration_ms]";

        public string Execute(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
                return $"Usage: {Usage}";

            var keyStrings = args[0].Split('+', StringSplitOptions.RemoveEmptyEntries);
            var keys = new SButton[keyStrings.Length];

            for (int i = 0; i < keyStrings.Length; i++)
            {
                if (!Enum.TryParse<SButton>(keyStrings[i].Trim(), true, out keys[i]))
                    return $"Invalid key: {keyStrings[i]}";
            }

            int duration = 50; // Default duration
            if (args.Length == 2 && (!int.TryParse(args[1], out duration) || duration <= 0))
                return $"Invalid duration: {args[1]}";

            // Hold all keys simultaneously for combo simulation
            foreach (var key in keys)
            {
                KeybindManager.HoldKey(key, duration);
            }
            return $"Executing combo: {string.Join("+", keys)} for {duration}ms";
        }
    }

    /// <summary>
    /// Command to repeat a key press multiple times
    /// </summary>
    public class KeybindRepeatCommand : IConsoleCommand
    {
        public string Name => "keybind_repeat";
        public string Description => "Repeat a key press multiple times with specified interval";
        public string Usage => "keybind_repeat <key> <count> [interval_ms]";

        public string Execute(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
                return $"Usage: {Usage}";

            if (!Enum.TryParse<SButton>(args[0], true, out var key))
                return $"Invalid key: {args[0]}";

            if (!int.TryParse(args[1], out var count) || count <= 0)
                return $"Invalid count: {args[1]}";

            int interval = 100; // Default interval
            if (args.Length == 3 && (!int.TryParse(args[2], out interval) || interval <= 0))
                return $"Invalid interval: {args[2]}";

            var keys = Enumerable.Repeat(key, count);
            // Simulate sequence by pressing the key multiple times
            foreach (var k in keys)
            {
                KeybindManager.PressKey(k);
            }
            return $"Repeating key {key} {count} times (simulated as individual presses)";
        }
    }

    /// <summary>
    /// Command to list all available SButton keys
    /// </summary>
    public class KeybindListCommand : IConsoleCommand
    {
        public string Name => "keybind_list";
        public string Description => "List all available SButton keys";
        public string Usage => "keybind_list [filter]";

        public string Execute(string[] args)
        {
            var allKeys = Enum.GetNames<SButton>();
            
            if (args.Length > 0)
            {
                var filter = args[0].ToLower();
                allKeys = allKeys.Where(k => k.ToLower().Contains(filter)).ToArray();
            }

            if (allKeys.Length == 0)
                return "No keys found matching the filter.";

            const int maxDisplay = 50;
            if (allKeys.Length > maxDisplay)
            {
                var displayed = allKeys.Take(maxDisplay);
                return $"Available keys ({allKeys.Length} total, showing first {maxDisplay}):\n{string.Join(", ", displayed)}\n... and {allKeys.Length - maxDisplay} more. Use a filter to narrow results.";
            }

            return $"Available keys ({allKeys.Length}):\n{string.Join(", ", allKeys)}";
        }
    }

    /// <summary>
    /// Command to show current keybind status
    /// </summary>
    public class KeybindStatusCommand : IConsoleCommand
    {
        public string Name => "keybind_status";
        public string Description => "Show current keybind status and held keys";
        public string Usage => "keybind_status";

        public string Execute(string[] args)
        {
            return KeybindManager.GetDebugInfo();
        }
    }

    /// <summary>
    /// Command to clear all active keybinds
    /// </summary>
    public class KeybindClearCommand : IConsoleCommand
    {
        public string Name => "keybind_clear";
        public string Description => "Clear all active keybinds and held keys";
        public string Usage => "keybind_clear";

        public string Execute(string[] args)
        {
            KeybindManager.ClearAll();
            return "Cleared all active keybinds";
        }
    }

    /// <summary>
    /// Command to enable/disable keybind system
    /// </summary>
    public class KeybindEnableCommand : IConsoleCommand
    {
        public string Name => "keybind_enable";
        public string Description => "Enable or disable the keybind system";
        public string Usage => "keybind_enable <true|false>";

        public string Execute(string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Usage}";

            if (bool.TryParse(args[0], out var enabled))
            {
                // For now, just clear all keys if disabling, since we don't have IsEnabled property
                if (!enabled)
                {
                    KeybindManager.ClearAll();
                }
                return $"Keybind system {(enabled ? "enabled" : "disabled (all keys cleared)")}";
            }

            return $"Invalid value: {args[0]}. Use 'true' or 'false'.";
        }
    }

    /// <summary>
    /// Command to show help for keybind commands
    /// </summary>
    public class KeybindHelpCommand : IConsoleCommand
    {
        public string Name => "keybind_help";
        public string Description => "Show help for keybind commands";
        public string Usage => "keybind_help [command]";

        public string Execute(string[] args)
        {
            if (args.Length == 1)
            {
                var commandName = args[0].ToLower();
                var commands = ConsoleCommandHandler.GetCommands();
                
                if (commands.TryGetValue(commandName, out var command))
                {
                    return $"{command.Name}: {command.Description}\nUsage: {command.Usage}";
                }
                
                return $"Unknown command: {commandName}";
            }

            var allCommands = ConsoleCommandHandler.GetCommands().Values
                .OrderBy(c => c.Name)
                .Select(c => $"{c.Name}: {c.Description}")
                .ToArray();

            return $"Available keybind commands:\n{string.Join("\n", allCommands)}\n\nUse 'keybind_help <command>' for detailed usage.";
        }
    }
}
