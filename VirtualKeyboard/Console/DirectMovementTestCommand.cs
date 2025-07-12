using StardewModdingAPI;
using VirtualKeyboard.Inputs;
using Microsoft.Xna.Framework.Input;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Console command for testing direct movement
    /// </summary>
    internal class DirectMovementTestCommand
    {
        private readonly IMonitor monitor;

        public DirectMovementTestCommand(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        /// <summary>
        /// Execute direct movement test command
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="args">Command arguments</param>
        public void Execute(string command, string[] args)
        {
            if (args.Length < 1)
            {
                monitor.Log("Usage: test_direct_movement <direction>", LogLevel.Info);
                monitor.Log("Directions: up, down, left, right", LogLevel.Info);
                return;
            }

            var direction = args[0].ToLower();
            Keys keyToPress;

            switch (direction)
            {
                case "up":
                    keyToPress = Keys.W;
                    break;
                case "down":
                    keyToPress = Keys.S;
                    break;
                case "left":
                    keyToPress = Keys.A;
                    break;
                case "right":
                    keyToPress = Keys.D;
                    break;
                default:
                    monitor.Log($"Invalid direction: {direction}. Use up, down, left, right", LogLevel.Error);
                    return;
            }

            monitor.Log($"Testing direct movement: {direction} (key: {keyToPress})", LogLevel.Info);
            
            // Store direction for continuous movement
            currentDirection = direction;
            
            // Activate virtual input and press the key for 2 seconds
            VirtualInputState.Active = true;
            VirtualInputState.AddKeys(new[] { keyToPress });
            
            monitor.Log($"Virtual {direction} movement activated for 2 seconds", LogLevel.Info);
            monitor.Log("Watch the player character - they should move immediately", LogLevel.Info);
            
            // Apply immediate movement using direct player manipulation
            ApplyDirectMovement(direction);
            
            // Schedule key release after 2 seconds
            ModEntry.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            updateCount = 0;
        }

        private int updateCount = 0;
        private string currentDirection = "";
        private const int TICKS_FOR_2_SECONDS = 120; // Approximately 2 seconds at 60 FPS

        private void OnUpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            updateCount++;
            
            // Continue applying movement every few ticks
            if (updateCount % 10 == 0 && !string.IsNullOrEmpty(currentDirection))
            {
                ApplyDirectMovement(currentDirection);
            }
            
            if (updateCount >= TICKS_FOR_2_SECONDS)
            {
                // Release all virtual keys and deactivate
                VirtualInputState.ClearKeys();
                VirtualInputState.Active = false;
                currentDirection = "";
                
                monitor.Log("Direct movement test completed - virtual keys released", LogLevel.Info);
                
                // Unsubscribe from update event
                ModEntry.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }

        /// <summary>
        /// Apply direct movement to the player character
        /// </summary>
        /// <param name="direction">Direction to move</param>
        private void ApplyDirectMovement(string direction)
        {
            try
            {
                var player = StardewValley.Game1.player;
                if (player == null || !player.CanMove)
                {
                    monitor.Log("Player cannot move right now", LogLevel.Warn);
                    return;
                }

                // Apply movement directly using setMoving
                switch (direction)
                {
                    case "up":
                        player.setMoving(1); // Up
                        monitor.Log("Applied direct UP movement to player", LogLevel.Info);
                        break;
                    case "right":
                        player.setMoving(2); // Right  
                        monitor.Log("Applied direct RIGHT movement to player", LogLevel.Info);
                        break;
                    case "down":
                        player.setMoving(4); // Down
                        monitor.Log("Applied direct DOWN movement to player", LogLevel.Info);
                        break;
                    case "left":
                        player.setMoving(8); // Left
                        monitor.Log("Applied direct LEFT movement to player", LogLevel.Info);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                monitor.Log($"Error applying direct movement: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
