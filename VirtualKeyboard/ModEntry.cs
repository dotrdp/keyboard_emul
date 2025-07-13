using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using VirtualKeyboard.Console;
using VirtualKeyboard.Patches;

namespace VirtualKeyboard
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /// <summary>
        /// Static reference to the monitor for logging
        /// </summary>
        public new static IMonitor Monitor { get; private set; } = null!;

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
            Monitor = base.Monitor;
            Helper = helper;
            IPatch.Monitor = Monitor;

            try
            {
                // Initialize core systems
                InitializeSystems();

                // Apply Harmony patches
                ApplyPatches();

                // Register console commands
                RegisterConsoleCommands(helper);

                // Hook game events
                RegisterEventHandlers(helper);

                Monitor.Log("VirtualKeyboard mod initialized successfully with console-based keybind simulation", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to initialize VirtualKeyboard mod: {ex.Message}", LogLevel.Error);
                Monitor.Log(ex.StackTrace ?? "No stack trace available", LogLevel.Trace);
            }
        }

        /// <summary>
        /// Initialize core systems
        /// </summary>
        private void InitializeSystems()
        {
            // Initialize keybind manager
            KeybindManager.Initialize();

            // Initialize console command system
            ConsoleCommandHandler.Initialize();

            Monitor.Log("Core systems initialized", LogLevel.Trace);
        }

        /// <summary>
        /// Apply Harmony patches
        /// </summary>
        private void ApplyPatches()
        {
            harmony = new Harmony(this.ModManifest.UniqueID);

            // Apply all patches in the Patches namespace
            PatchAll(harmony);

            // Apply additional patches using HarmonyPatchAll for attribute-based patches
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Monitor.Log("Applied attribute-based patches (InputState_Patches, StartupPatches)", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to apply attribute-based patches: {ex.Message}", LogLevel.Error);
            }

            Monitor.Log("Harmony patches applied", LogLevel.Info);
        }

        /// <summary>
        /// Apply all patches found in the Patches namespace
        /// </summary>
        /// <param name="harmony">The Harmony instance</param>
        private void PatchAll(Harmony harmony)
        {
            var patchTypes = new[]
            {
                typeof(KeyboardDispatcher_ShouldSuppress),
                typeof(KeyboardDispatcher_Event_TextInput),
                typeof(KeyboardDispatcher_EventInput_CharEntered),
                typeof(KeyboardDispatcher_EventInput_KeyDown),
                typeof(KeyboardDispatcher_Event_KeyDown),
                typeof(CriticalInputPatch),
                typeof(PlayerMovementPatch),
                typeof(InputHelper_IsDown),
                typeof(SInputState_IsDown),
                typeof(KeyboardState_IsKeyDown),
                typeof(Game1_Input),
                typeof(SMAPI_InputHelper),
                typeof(Game1_InputIsDown)
            };

            foreach (var patchType in patchTypes)
            {
                try
                {
                    if (Activator.CreateInstance(patchType) is IPatch patch)
                    {
                        patch.Patch(harmony);
                        Monitor.Log($"Applied patch: {patch.Name}", LogLevel.Trace);
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to apply patch {patchType.Name}: {ex.Message}", LogLevel.Error);
                }
            }

            // Apply the SInputState keyboard patch separately as it doesn't use IPatch
            try
            {
                SInputState_GetKeyboardState_Patch.Patch(harmony);
                Monitor.Log("Applied SInputState_GetKeyboardState_Patch", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to apply SInputState_GetKeyboardState_Patch: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Register console commands with SMAPI
        /// </summary>
        /// <param name="helper">Mod helper</param>
        private void RegisterConsoleCommands(IModHelper helper)
        {
            // Register each keybind command with SMAPI console
            var commands = ConsoleCommandHandler.GetCommands();
            
            foreach (var command in commands.Values)
            {
                helper.ConsoleCommands.Add(command.Name, command.Description, (name, args) =>
                {
                    var result = ConsoleCommandHandler.ExecuteCommand($"{name} {string.Join(" ", args)}");
                    Monitor.Log(result, LogLevel.Info);
                });
            }

            // Register new movement commands using IInputSimulator
            var movementCommands = new IConsoleCommand[]
            {
                new MoveUpCommand(),
                new MoveDownCommand(),
                new MoveLeftCommand(),
                new MoveRightCommand(),
                new StopMovementCommand()
            };

            foreach (var command in movementCommands)
            {
                helper.ConsoleCommands.Add(command.Name, command.Description, (name, args) =>
                {
                    var result = command.Execute(args);
                    Monitor.Log(result, LogLevel.Info);
                });
            }

            Monitor.Log($"Registered {commands.Count + movementCommands.Length} console commands", LogLevel.Info);
        }

        /// <summary>
        /// Register event handlers
        /// </summary>
        /// <param name="helper">Mod helper</param>
        private void RegisterEventHandlers(IModHelper helper)
        {
            // Update keybind manager each game tick
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            // Log when save is loaded
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            Monitor.Log("Event handlers registered", LogLevel.Trace);
        }

        /// <summary>
        /// Handle game update ticks
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // Update keybind manager every tick
            KeybindManager.Update();
        }

        /// <summary>
        /// Handle save loaded
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Save loaded - keybind system ready", LogLevel.Info);
        }

        /// <summary>
        /// Cleanup when mod is disposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clear all virtual keys
                KeybindManager.ClearAllKeys();

                // Unpatch Harmony
                harmony?.UnpatchAll(this.ModManifest.UniqueID);
                harmony = null;

                Monitor.Log("VirtualKeyboard mod disposed", LogLevel.Trace);
            }

            base.Dispose(disposing);
        }
    }
}