using System;
using StardewModdingAPI;
using VirtualKeyboard.Simulation;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Command to move up using KeybindManager
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
            KeybindManager.HoldKey(SButton.W, duration);

            return $"Moving up for {duration}ms";
        }
    }

    /// <summary>
    /// Command to move down using KeybindManager
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
            KeybindManager.HoldKey(SButton.S, duration);

            return $"Moving down for {duration}ms";
        }
    }

    /// <summary>
    /// Command to move left using KeybindManager
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
            KeybindManager.HoldKey(SButton.A, duration);

            return $"Moving left for {duration}ms";
        }
    }

    /// <summary>
    /// Command to move right using KeybindManager
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
            KeybindManager.HoldKey(SButton.D, duration);

            return $"Moving right for {duration}ms";
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
            // Release all movement keys
            KeybindManager.ReleaseKey(SButton.W);
            KeybindManager.ReleaseKey(SButton.A);
            KeybindManager.ReleaseKey(SButton.S);
            KeybindManager.ReleaseKey(SButton.D);

            return "All virtual movement stopped";
        }
    }
}
