using System;
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
}
