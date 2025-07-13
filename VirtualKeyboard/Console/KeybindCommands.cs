using System;
using System.Linq;
using StardewModdingAPI;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to press a key
    /// </summary>
    public class KeybindPressCommand : IConsoleCommand
    {
        public string Name => "keybind_press";
        public string Description => "Press and hold a key indefinitely";
        public string Usage => "keybind_press <key>";

        public string Execute(string[] args)
        {
            if (args.Length == 0)
                return "Error: Please specify a key to press. Usage: keybind_press <key>";

            if (!Enum.TryParse<SButton>(args[0], true, out SButton key))
                return $"Error: Invalid key '{args[0]}'. Use valid SButton names like W, A, S, D, C, X, etc.";

            VirtualInputSimulator.Active = true;
            KeybindManager.PressKey(key);

            return $"Key '{key}' pressed and held";
        }
    }

    /// <summary>
    /// Command to release a key
    /// </summary>
    public class KeybindReleaseCommand : IConsoleCommand
    {
        public string Name => "keybind_release";
        public string Description => "Release a currently held key";
        public string Usage => "keybind_release <key>";

        public string Execute(string[] args)
        {
            if (args.Length == 0)
                return "Error: Please specify a key to release. Usage: keybind_release <key>";

            if (!Enum.TryParse<SButton>(args[0], true, out SButton key))
                return $"Error: Invalid key '{args[0]}'. Use valid SButton names like W, A, S, D, C, X, etc.";

            KeybindManager.ReleaseKey(key);

            if (!KeybindManager.HasActiveKeybinds)
                VirtualInputSimulator.Active = false;

            return $"Key '{key}' released";
        }
    }

    /// <summary>
    /// Command to hold a key for a specific duration
    /// </summary>
    public class KeybindHoldCommand : IConsoleCommand
    {
        public string Name => "keybind_hold";
        public string Description => "Hold a key for specified duration";
        public string Usage => "keybind_hold <key> <duration_ms>";

        public string Execute(string[] args)
        {
            if (args.Length < 2)
                return "Error: Please specify key and duration. Usage: keybind_hold <key> <duration_ms>";

            if (!Enum.TryParse<SButton>(args[0], true, out SButton key))
                return $"Error: Invalid key '{args[0]}'. Use valid SButton names like W, A, S, D, C, X, etc.";

            if (!int.TryParse(args[1], out int duration) || duration < 1)
                return "Error: Duration must be a positive number in milliseconds";

            duration = Math.Min(duration, 30000); // Max 30 seconds

            VirtualInputSimulator.Active = true;
            KeybindManager.HoldKey(key, duration);

            return $"Key '{key}' held for {duration}ms";
        }
    }

    /// <summary>
    /// Command to execute a sequence of keys
    /// </summary>
    public class KeybindSequenceCommand : IConsoleCommand
    {
        public string Name => "keybind_sequence";
        public string Description => "Execute a sequence of key presses";
        public string Usage => "keybind_sequence <key1,key2,key3> [interval_ms]";

        public string Execute(string[] args)
        {
            if (args.Length == 0)
                return "Error: Please specify keys. Usage: keybind_sequence <key1,key2,key3> [interval_ms]";

            var keyStrings = args[0].Split(',');
            var keys = new SButton[keyStrings.Length];

            for (int i = 0; i < keyStrings.Length; i++)
            {
                if (!Enum.TryParse<SButton>(keyStrings[i].Trim(), true, out keys[i]))
                    return $"Error: Invalid key '{keyStrings[i].Trim()}'. Use valid SButton names.";
            }

            int interval = 100; // Default 100ms
            if (args.Length > 1 && int.TryParse(args[1], out int customInterval))
                interval = Math.Max(50, Math.Min(2000, customInterval));

            VirtualInputSimulator.Active = true;
            KeybindManager.ExecuteSequence(keys, interval);

            return $"Executing sequence: {string.Join(" → ", keys)} with {interval}ms intervals";
        }
    }

    /// <summary>
    /// Command to execute a key combination
    /// </summary>
    public class KeybindComboCommand : IConsoleCommand
    {
        public string Name => "keybind_combo";
        public string Description => "Execute a simultaneous key combination";
        public string Usage => "keybind_combo <key1+key2+key3> [hold_duration_ms]";

        public string Execute(string[] args)
        {
            if (args.Length == 0)
                return "Error: Please specify keys. Usage: keybind_combo <key1+key2+key3> [hold_duration_ms]";

            var keyStrings = args[0].Split('+');
            var keys = new SButton[keyStrings.Length];

            for (int i = 0; i < keyStrings.Length; i++)
            {
                if (!Enum.TryParse<SButton>(keyStrings[i].Trim(), true, out keys[i]))
                    return $"Error: Invalid key '{keyStrings[i].Trim()}'. Use valid SButton names.";
            }

            int holdDuration = 100; // Default 100ms
            if (args.Length > 1 && int.TryParse(args[1], out int customDuration))
                holdDuration = Math.Max(50, Math.Min(5000, customDuration));

            VirtualInputSimulator.Active = true;
            KeybindManager.ExecuteCombo(keys, holdDuration);

            return $"Executing combo: {string.Join(" + ", keys)} for {holdDuration}ms";
        }
    }

    /// <summary>
    /// Command to clear all virtual keys
    /// </summary>
    public class KeybindClearCommand : IConsoleCommand
    {
        public string Name => "keybind_clear";
        public string Description => "Clear all virtual key states";
        public string Usage => "keybind_clear";

        public string Execute(string[] args)
        {
            KeybindManager.ClearAllKeys();
            VirtualInputSimulator.Instance.ClearAllInputs();
            VirtualInputSimulator.Active = false;

            return "All virtual key states cleared";
        }
    }

    /// <summary>
    /// Command to show keybind status
    /// </summary>
    public class KeybindStatusCommand : IConsoleCommand
    {
        public string Name => "keybind_status";
        public string Description => "Show current keybind status";
        public string Usage => "keybind_status";

        public string Execute(string[] args)
        {
            var status = KeybindManager.GetStatus();
            var simulatorActive = VirtualInputSimulator.Active ? "Active" : "Inactive";
            
            return $"KeybindManager: {status}\nVirtualInputSimulator: {simulatorActive}";
        }
    }

    /// <summary>
    /// Command to list available keys
    /// </summary>
    public class KeybindListCommand : IConsoleCommand
    {
        public string Name => "keybind_list";
        public string Description => "List available keys for binding";
        public string Usage => "keybind_list [category]";

        public string Execute(string[] args)
        {
            string category = args.Length > 0 ? args[0].ToLower() : "all";

            switch (category)
            {
                case "movement":
                    return "Movement Keys: W (up), A (left), S (down), D (right)";
                
                case "action":
                    return "Action Keys: C (action), X (use tool), F (interact), Y (confirm), N (no)";
                
                case "inventory":
                    return "Inventory Keys: D1-D9, D0, OemMinus (-), OemPlus (+)";
                
                case "menu":
                    return "Menu Keys: Escape, E (inventory), I (items), M (map), J (journal), Tab";
                
                case "modifier":
                    return "Modifier Keys: LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt";
                
                case "all":
                default:
                    return "Key Categories: movement, action, inventory, menu, modifier\n" +
                           "Use 'keybind_list <category>' for specific keys\n" +
                           "Example: keybind_list movement";
            }
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
            if (args.Length == 0)
                return $"Keybind system is currently: {(KeybindManager.IsEnabled ? "Enabled" : "Disabled")}\n" +
                       "Usage: keybind_enable <true|false>";

            if (bool.TryParse(args[0], out bool enable))
            {
                KeybindManager.IsEnabled = enable;
                
                if (!enable)
                {
                    KeybindManager.ClearAllKeys();
                    VirtualInputSimulator.Active = false;
                }
                
                return $"Keybind system {(enable ? "enabled" : "disabled")}";
            }

            return "Error: Please specify 'true' or 'false'";
        }
    }

    /// <summary>
    /// Command to show help for keybind commands
    /// </summary>
    public class KeybindHelpCommand : IConsoleCommand
    {
        public string Name => "keybind_help";
        public string Description => "Show help for keybind commands";
        public string Usage => "keybind_help";

        public string Execute(string[] args)
        {
            return @"Keybind Commands:
=================

Basic Commands:
• keybind_press <key>              - Press and hold a key
• keybind_release <key>            - Release a held key  
• keybind_hold <key> <duration>    - Hold key for specific time
• keybind_clear                    - Clear all keys

Advanced Commands:
• keybind_sequence <k1,k2,k3> [ms] - Execute key sequence
• keybind_combo <k1+k2+k3> [ms]    - Execute simultaneous combo
• keybind_status                   - Show current status
• keybind_list [category]          - List available keys
• keybind_enable <true|false>      - Enable/disable system

Movement Commands:
• move_up [duration]               - Move up
• move_down [duration]             - Move down  
• move_left [duration]             - Move left
• move_right [duration]            - Move right
• stop_movement                    - Stop all movement

Examples:
• keybind_press C                  - Hold action key
• keybind_hold F 500               - Interact for 500ms
• keybind_sequence W,W,D,D 200     - Move sequence
• keybind_combo LeftShift+C 300    - Shift+Action combo";
        }
    }
}
