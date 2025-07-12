using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using VirtualKeyboard.Console;

namespace VirtualKeyboard
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /// <summary>
        /// Static reference to the monitor for logging
        /// </summary>
        public static IMonitor? Logger { get; private set; }

        /// <summary>
        /// Static reference to the helper for accessing SMAPI APIs
        /// </summary>
        public new static IModHelper Helper { get; private set; } = null!;

        /// <summary>
        /// The Harmony instance for applying patches
        /// </summary>
        private Harmony? harmony;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Initialize static references
            Logger = Monitor;
            Helper = helper;

            try
            {
                // Initialize core systems
                InitializeSystems();

                // Apply Harmony patches
                ApplyPatches();

                // Register console commands
                RegisterConsoleCommands(helper);

                Logger?.Log("Virtual Keyboard mod loaded successfully", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger?.Log($"Failed to initialize mod: {ex}", LogLevel.Error);
                throw;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Initialize core systems
        /// </summary>
        private void InitializeSystems()
        {
            // Setup update loop for keybind management
            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            Logger?.Log("Core systems initialized", LogLevel.Debug);
        }

        /// <summary>
        /// Apply Harmony patches for input interception
        /// </summary>
        private void ApplyPatches()
        {
            // Create Harmony instance
            harmony = new Harmony(ModManifest.UniqueID);

            try
            {
                // Apply all patches from the assembly automatically
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                
                Logger?.Log("All Harmony patches applied successfully", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger?.Log($"Failed to apply patches: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Register console commands
        /// </summary>
        private void RegisterConsoleCommands(IModHelper helper)
        {
            // Initialize console command system
            ConsoleCommandHandler.Initialize();

            // Register core commands
            helper.ConsoleCommands.Add("keybind_press", "Press a virtual key briefly", HandleKeybindPress);
            helper.ConsoleCommands.Add("keybind_hold", "Hold a virtual key for duration", HandleKeybindHold);
            helper.ConsoleCommands.Add("keybind_release", "Release a virtual key", HandleKeybindRelease);
            helper.ConsoleCommands.Add("keybind_clear", "Clear all virtual keybinds", HandleKeybindClear);
            helper.ConsoleCommands.Add("keybind_status", "Show keybind manager status", HandleKeybindStatus);
            helper.ConsoleCommands.Add("keybind_test", "Test virtual key press", HandleKeybindTest);
            helper.ConsoleCommands.Add("test_direct_movement", "Test direct movement patch", HandleDirectMovementTest);

            Logger?.Log("Console commands registered", LogLevel.Debug);
        }

        /// <summary>
        /// Update loop for keybind management
        /// </summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // Update keybind manager to handle timeouts
            KeybindManager.Update();
        }

        /// <summary>
        /// Handle keybind press command
        /// </summary>
        private void HandleKeybindPress(string command, string[] args)
        {
            if (args.Length < 1)
            {
                Logger?.Log("Usage: keybind_press <key>", LogLevel.Info);
                return;
            }

            if (Enum.TryParse<SButton>(args[0], true, out SButton key))
            {
                KeybindManager.PressKey(key);
                Logger?.Log($"Pressed virtual key: {key}", LogLevel.Info);
            }
            else
            {
                Logger?.Log($"Invalid key: {args[0]}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Handle keybind hold command
        /// </summary>
        private void HandleKeybindHold(string command, string[] args)
        {
            if (args.Length < 1)
            {
                Logger?.Log("Usage: keybind_hold <key> [duration_ms]", LogLevel.Info);
                return;
            }

            if (Enum.TryParse<SButton>(args[0], true, out SButton key))
            {
                int duration = 1000; // Default 1 second
                if (args.Length > 1 && int.TryParse(args[1], out int customDuration))
                {
                    duration = customDuration;
                }

                KeybindManager.HoldKey(key, duration);
                Logger?.Log($"Holding virtual key: {key} for {duration}ms", LogLevel.Info);
            }
            else
            {
                Logger?.Log($"Invalid key: {args[0]}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Handle keybind release command
        /// </summary>
        private void HandleKeybindRelease(string command, string[] args)
        {
            if (args.Length < 1)
            {
                Logger?.Log("Usage: keybind_release <key>", LogLevel.Info);
                return;
            }

            if (Enum.TryParse<SButton>(args[0], true, out SButton key))
            {
                KeybindManager.ReleaseKey(key);
                Logger?.Log($"Released virtual key: {key}", LogLevel.Info);
            }
            else
            {
                Logger?.Log($"Invalid key: {args[0]}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Handle keybind clear command
        /// </summary>
        private void HandleKeybindClear(string command, string[] args)
        {
            KeybindManager.ClearAll();
            Logger?.Log("Cleared all virtual keybinds", LogLevel.Info);
        }

        /// <summary>
        /// Handle keybind status command
        /// </summary>
        private void HandleKeybindStatus(string command, string[] args)
        {
            var status = KeybindManager.GetDebugInfo();
            Logger?.Log($"Keybind Status: {status}", LogLevel.Info);
        }

        /// <summary>
        /// Handle keybind test command
        /// </summary>
        private void HandleKeybindTest(string command, string[] args)
        {
            Logger?.Log("Testing virtual key press with 'W' key for movement...", LogLevel.Info);
            KeybindManager.HoldKey(SButton.W, 2000); // Hold W for 2 seconds
            Logger?.Log("Virtual W key should now be held for 2 seconds", LogLevel.Info);
        }

        /// <summary>
        /// Handle direct movement test command
        /// </summary>
        private void HandleDirectMovementTest(string command, string[] args)
        {
            var testCommand = new Console.DirectMovementTestCommand(Logger!);
            testCommand.Execute(command, args);
        }

        /// <summary>
        /// Clean up resources when the mod is disposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unapply patches
                harmony?.UnpatchAll(ModManifest.UniqueID);
                
                // Clean up keybinds
                KeybindManager.ClearAll();
            }
            
            base.Dispose(disposing);
        }
    }
}
