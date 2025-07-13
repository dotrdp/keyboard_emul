using System;
using System.Collections.Generic;

namespace VirtualKeyboard.Console
{
    /// <summary>
    /// Interface for console commands
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of what the command does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Usage example for the command
        /// </summary>
        string Usage { get; }

        /// <summary>
        /// Execute the command with given arguments
        /// </summary>
        /// <param name="args">Command arguments</param>
        /// <returns>Result message</returns>
        string Execute(string[] args);
    }

    /// <summary>
    /// Manages and executes console commands for keybind simulation
    /// </summary>
    public static class ConsoleCommandHandler
    {
        private static readonly Dictionary<string, IConsoleCommand> Commands = new();

        /// <summary>
        /// Initialize the console command system
        /// </summary>
        public static void Initialize()
        {
            // Register movement commands using IInputSimulator approach
            RegisterCommand(new MoveUpCommand());
            RegisterCommand(new MoveDownCommand());
            RegisterCommand(new MoveLeftCommand());
            RegisterCommand(new MoveRightCommand());
            RegisterCommand(new StopMovementCommand());
            RegisterCommand(new StopAllCommand()); // Emergency stop for all input

            // Register comprehensive keybind commands
            RegisterCommand(new KeybindPressCommand());
            RegisterCommand(new KeybindReleaseCommand());
            RegisterCommand(new KeybindHoldCommand());
            RegisterCommand(new KeybindSequenceCommand());
            RegisterCommand(new KeybindComboCommand());
            RegisterCommand(new KeybindClearCommand());
            RegisterCommand(new KeybindStatusCommand());
            RegisterCommand(new KeybindListCommand());
            RegisterCommand(new KeybindEnableCommand());
            RegisterCommand(new KeybindHelpCommand());
            RegisterCommand(new KeybindWindowsInputCommand());

            Patches.IPatch.Info("Console command system initialized");
        }

        /// <summary>
        /// Register a new console command
        /// </summary>
        /// <param name="command">The command to register</param>
        public static void RegisterCommand(IConsoleCommand command)
        {
            Commands[command.Name.ToLower()] = command;
            Patches.IPatch.Trace($"Registered command: {command.Name}");
        }

        /// <summary>
        /// Execute a console command
        /// </summary>
        /// <param name="input">The full command input</param>
        /// <returns>Result message</returns>
        public static string ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "No command provided.";

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "No command provided.";

            var commandName = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

            if (!Commands.TryGetValue(commandName, out var command))
            {
                return $"Unknown command: {commandName}. Type 'keybind_help' for available commands.";
            }

            try
            {
                var result = command.Execute(args);
                Patches.IPatch.Info($"Executed command: {commandName} -> {result}");
                return result;
            }
            catch (Exception ex)
            {
                var error = $"Error executing command '{commandName}': {ex.Message}";
                Patches.IPatch.Error(error);
                return error;
            }
        }

        /// <summary>
        /// Get all registered commands
        /// </summary>
        /// <returns>Dictionary of commands</returns>
        public static IReadOnlyDictionary<string, IConsoleCommand> GetCommands()
        {
            return Commands;
        }

        /// <summary>
        /// Check if a command exists
        /// </summary>
        /// <param name="commandName">Name of the command</param>
        /// <returns>True if command exists</returns>
        public static bool HasCommand(string commandName)
        {
            return Commands.ContainsKey(commandName.ToLower());
        }
    }
}
