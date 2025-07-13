using System;
using System.Linq;
using StardewModdingAPI;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to move up using IInputSimulator
    /// </summary>
    public class MoveUpCommand : IConsoleCommand
    {
        public string Name => "move_up";
        public string Description => "Move player up for specified duration (default 2000ms)";
        public string Usage => "move_up [duration_ms]";

        public string Execute(string[] args)
        {
            int duration = 2000;
            if (args.Length > 0 && int.TryParse(args[0], out int customDuration))
            {
                duration = Math.Max(100, Math.Min(10000, customDuration));
            }

            VirtualInputSimulator.Active = true;
            VirtualInputSimulator.Instance.SetMovementPressed("up");

            // Set up timer to stop movement
            var timer = new System.Threading.Timer(_ =>
            {
                VirtualInputSimulator.Instance.SetMovementReleased("up");
                VirtualInputSimulator.Instance.ClearAllInputs();
                VirtualInputSimulator.Active = false;
            }, null, duration, System.Threading.Timeout.Infinite);

            return $"Moving up for {duration}ms using IInputSimulator";
        }
    }

    /// <summary>
    /// Command to move down using IInputSimulator
    /// </summary>
    public class MoveDownCommand : IConsoleCommand
    {
        public string Name => "move_down";
        public string Description => "Move player down for specified duration (default 2000ms)";
        public string Usage => "move_down [duration_ms]";

        public string Execute(string[] args)
        {
            int duration = 2000;
            if (args.Length > 0 && int.TryParse(args[0], out int customDuration))
            {
                duration = Math.Max(100, Math.Min(10000, customDuration));
            }

            VirtualInputSimulator.Active = true;
            VirtualInputSimulator.Instance.SetMovementPressed("down");

            // Set up timer to stop movement
            var timer = new System.Threading.Timer(_ =>
            {
                VirtualInputSimulator.Instance.SetMovementReleased("down");
                VirtualInputSimulator.Instance.ClearAllInputs();
                VirtualInputSimulator.Active = false;
            }, null, duration, System.Threading.Timeout.Infinite);

            return $"Moving down for {duration}ms using IInputSimulator";
        }
    }

    /// <summary>
    /// Command to move left using IInputSimulator
    /// </summary>
    public class MoveLeftCommand : IConsoleCommand
    {
        public string Name => "move_left";
        public string Description => "Move player left for specified duration (default 2000ms)";
        public string Usage => "move_left [duration_ms]";

        public string Execute(string[] args)
        {
            int duration = 2000;
            if (args.Length > 0 && int.TryParse(args[0], out int customDuration))
            {
                duration = Math.Max(100, Math.Min(10000, customDuration));
            }

            VirtualInputSimulator.Active = true;
            VirtualInputSimulator.Instance.SetMovementPressed("left");

            // Set up timer to stop movement
            var timer = new System.Threading.Timer(_ =>
            {
                VirtualInputSimulator.Instance.SetMovementReleased("left");
                VirtualInputSimulator.Instance.ClearAllInputs();
                VirtualInputSimulator.Active = false;
            }, null, duration, System.Threading.Timeout.Infinite);

            return $"Moving left for {duration}ms using IInputSimulator";
        }
    }

    /// <summary>
    /// Command to move right using IInputSimulator
    /// </summary>
    public class MoveRightCommand : IConsoleCommand
    {
        public string Name => "move_right";
        public string Description => "Move player right for specified duration (default 2000ms)";
        public string Usage => "move_right [duration_ms]";

        public string Execute(string[] args)
        {
            int duration = 2000;
            if (args.Length > 0 && int.TryParse(args[0], out int customDuration))
            {
                duration = Math.Max(100, Math.Min(10000, customDuration));
            }

            VirtualInputSimulator.Active = true;
            VirtualInputSimulator.Instance.SetMovementPressed("right");

            // Set up timer to stop movement
            var timer = new System.Threading.Timer(_ =>
            {
                VirtualInputSimulator.Instance.SetMovementReleased("right");
                VirtualInputSimulator.Instance.ClearAllInputs();
                VirtualInputSimulator.Active = false;
            }, null, duration, System.Threading.Timeout.Infinite);

            return $"Moving right for {duration}ms using IInputSimulator";
        }
    }

    /// <summary>
    /// Command to stop all movement
    /// </summary>
    public class StopMovementCommand : IConsoleCommand
    {
        public string Name => "stop_movement";
        public string Description => "Stop all virtual movement";
        public string Usage => "stop_movement";

        public string Execute(string[] args)
        {
            VirtualInputSimulator.Instance.ClearAllInputs();
            VirtualInputSimulator.Active = false;
            return "All virtual movement stopped";
        }
    }

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

            KeybindManager.ExecuteSequence(keys, interval);
            return $"Executing sequence: {string.Join(", ", keys)} with {interval}ms intervals";
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

            KeybindManager.ExecuteCombo(keys, duration);
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
            KeybindManager.ExecuteSequence(keys, interval);
            return $"Repeating key {key} {count} times with {interval}ms intervals";
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
            return KeybindManager.GetStatus();
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
            KeybindManager.ClearAllKeys();
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
                KeybindManager.IsEnabled = enabled;
                return $"Keybind system {(enabled ? "enabled" : "disabled")}";
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
