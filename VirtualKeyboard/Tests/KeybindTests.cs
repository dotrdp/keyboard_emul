using System;
using System.Collections.Generic;
using StardewModdingAPI;
using Xunit;
using VirtualKeyboard;
using VirtualKeyboard.Console;

namespace VirtualKeyboard.Tests
{
    /// <summary>
    /// Test suite for VirtualKeyboard console commands and keybind simulation
    /// </summary>
    public class KeybindTests
    {
        /// <summary>
        /// Test key press simulation
        /// </summary>
        [Fact]
        public void TestKeyPress()
        {
            // Arrange
            KeybindManager.Initialize();
            var key = SButton.W;
            
            // Act
            KeybindManager.PressKey(key);
            
            // Assert
            Assert.True(KeybindManager.IsKeyHeld(key));
            Assert.True(KeybindManager.HasActiveKeybinds);
        }

        /// <summary>
        /// Test key release simulation
        /// </summary>
        [Fact]
        public void TestKeyRelease()
        {
            // Arrange
            KeybindManager.Initialize();
            var key = SButton.A;
            KeybindManager.PressKey(key);
            
            // Act
            KeybindManager.ReleaseKey(key);
            
            // Assert
            Assert.False(KeybindManager.IsKeyHeld(key));
        }

        /// <summary>
        /// Test key hold with duration
        /// </summary>
        [Fact]
        public void TestKeyHold()
        {
            // Arrange
            KeybindManager.Initialize();
            var key = SButton.S;
            
            // Act
            KeybindManager.HoldKey(key, 100);
            
            // Assert
            Assert.True(KeybindManager.IsKeyHeld(key));
            
            // Simulate time passing
            System.Threading.Thread.Sleep(150);
            KeybindManager.Update();
            
            // Key should be automatically released
            Assert.False(KeybindManager.IsKeyHeld(key));
        }

        /// <summary>
        /// Test key sequence execution
        /// </summary>
        [Fact]
        public void TestKeySequence()
        {
            // Arrange
            KeybindManager.Initialize();
            var keys = new[] { SButton.A, SButton.B, SButton.C };
            
            // Act
            KeybindManager.ExecuteSequence(keys, 50);
            
            // Assert
            Assert.True(KeybindManager.HasActiveKeybinds);
        }

        /// <summary>
        /// Test key combination execution
        /// </summary>
        [Fact]
        public void TestKeyCombo()
        {
            // Arrange
            KeybindManager.Initialize();
            var keys = new[] { SButton.LeftControl, SButton.C };
            
            // Act
            KeybindManager.ExecuteCombo(keys, 100);
            
            // Assert
            Assert.True(KeybindManager.IsKeyHeld(SButton.LeftControl));
            Assert.True(KeybindManager.IsKeyHeld(SButton.C));
        }

        /// <summary>
        /// Test clearing all keys
        /// </summary>
        [Fact]
        public void TestClearAllKeys()
        {
            // Arrange
            KeybindManager.Initialize();
            KeybindManager.PressKey(SButton.A);
            KeybindManager.PressKey(SButton.B);
            
            // Act
            KeybindManager.ClearAllKeys();
            
            // Assert
            Assert.False(KeybindManager.HasActiveKeybinds);
            Assert.False(KeybindManager.IsKeyHeld(SButton.A));
            Assert.False(KeybindManager.IsKeyHeld(SButton.B));
        }
    }

    /// <summary>
    /// Test suite for console commands
    /// </summary>
    public class ConsoleCommandTests
    {
        /// <summary>
        /// Test keybind_press command
        /// </summary>
        [Fact]
        public void TestKeybindPressCommand()
        {
            // Arrange
            var command = new KeybindPressCommand();
            var args = new[] { "W" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Pressed key: W", result);
        }

        /// <summary>
        /// Test keybind_hold command
        /// </summary>
        [Fact]
        public void TestKeybindHoldCommand()
        {
            // Arrange
            var command = new KeybindHoldCommand();
            var args = new[] { "Space", "500" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Holding key Space for 500ms", result);
        }

        /// <summary>
        /// Test keybind_sequence command
        /// </summary>
        [Fact]
        public void TestKeybindSequenceCommand()
        {
            // Arrange
            var command = new KeybindSequenceCommand();
            var args = new[] { "W,A,S,D", "100" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Executing sequence", result);
            Assert.Contains("100ms intervals", result);
        }

        /// <summary>
        /// Test keybind_combo command
        /// </summary>
        [Fact]
        public void TestKeybindComboCommand()
        {
            // Arrange
            var command = new KeybindComboCommand();
            var args = new[] { "LeftControl+C", "50" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Executing combo", result);
            Assert.Contains("LeftControl+C", result);
        }

        /// <summary>
        /// Test keybind_list command
        /// </summary>
        [Fact]
        public void TestKeybindListCommand()
        {
            // Arrange
            var command = new KeybindListCommand();
            var args = new[] { "A" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Available keys", result);
        }

        /// <summary>
        /// Test keybind_help command
        /// </summary>
        [Fact]
        public void TestKeybindHelpCommand()
        {
            // Arrange
            var command = new KeybindHelpCommand();
            var args = Array.Empty<string>();
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Available keybind commands", result);
        }

        /// <summary>
        /// Test invalid command arguments
        /// </summary>
        [Fact]
        public void TestInvalidCommandArguments()
        {
            // Arrange
            var command = new KeybindPressCommand();
            var args = Array.Empty<string>();
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Usage:", result);
        }

        /// <summary>
        /// Test invalid key name
        /// </summary>
        [Fact]
        public void TestInvalidKeyName()
        {
            // Arrange
            var command = new KeybindPressCommand();
            var args = new[] { "InvalidKey" };
            
            // Act
            var result = command.Execute(args);
            
            // Assert
            Assert.Contains("Invalid key", result);
        }
    }

    /// <summary>
    /// Integration tests for the complete system
    /// </summary>
    public class IntegrationTests
    {
        /// <summary>
        /// Test complete workflow: press, hold, release
        /// </summary>
        [Fact]
        public void TestCompleteWorkflow()
        {
            // Arrange
            KeybindManager.Initialize();
            
            // Act & Assert - Press key
            KeybindManager.PressKey(SButton.W);
            Assert.True(KeybindManager.IsKeyHeld(SButton.W));
            
            // Hold for duration
            KeybindManager.HoldKey(SButton.A, 100);
            Assert.True(KeybindManager.IsKeyHeld(SButton.A));
            
            // Release specific key
            KeybindManager.ReleaseKey(SButton.W);
            Assert.False(KeybindManager.IsKeyHeld(SButton.W));
            Assert.True(KeybindManager.IsKeyHeld(SButton.A)); // Should still be held
            
            // Clear all
            KeybindManager.ClearAllKeys();
            Assert.False(KeybindManager.HasActiveKeybinds);
        }

        /// <summary>
        /// Test console command handler integration
        /// </summary>
        [Fact]
        public void TestConsoleCommandHandlerIntegration()
        {
            // Arrange
            ConsoleCommandHandler.Initialize();
            
            // Test various commands
            var commands = new[]
            {
                "keybind_press W",
                "keybind_hold Space 500",
                "keybind_sequence W,A,S,D 100",
                "keybind_combo LeftControl+C",
                "keybind_status",
                "keybind_clear"
            };
            
            foreach (var commandInput in commands)
            {
                // Act
                var result = ConsoleCommandHandler.ExecuteCommand(commandInput);
                
                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.DoesNotContain("Unknown command", result);
            }
        }
    }
}

/// <summary>
/// Example usage scenarios for documentation
/// </summary>
public static class UsageExamples
{
    /// <summary>
    /// Example: Basic movement simulation
    /// </summary>
    public static void ExampleBasicMovement()
    {
        // Move forward for 1 second
        KeybindManager.HoldKey(SButton.W, 1000);
        
        // Turn left briefly
        KeybindManager.HoldKey(SButton.A, 500);
        
        // Move backward
        KeybindManager.HoldKey(SButton.S, 500);
    }

    /// <summary>
    /// Example: Farming sequence
    /// </summary>
    public static void ExampleFarmingSequence()
    {
        // Use tool, move right, use tool, move right (repeat pattern)
        var farmingSequence = new[]
        {
            SButton.LeftAlt,  // Use tool
            SButton.D,        // Move right
            SButton.LeftAlt,  // Use tool  
            SButton.D,        // Move right
            SButton.LeftAlt,  // Use tool
            SButton.D         // Move right
        };
        
        KeybindManager.ExecuteSequence(farmingSequence, 300);
    }

    /// <summary>
    /// Example: Copy-paste simulation
    /// </summary>
    public static void ExampleCopyPaste()
    {
        // Select all
        KeybindManager.ExecuteCombo(new[] { SButton.LeftControl, SButton.A }, 100);
        
        // Copy
        KeybindManager.ExecuteCombo(new[] { SButton.LeftControl, SButton.C }, 100);
        
        // Paste
        KeybindManager.ExecuteCombo(new[] { SButton.LeftControl, SButton.V }, 100);
    }

    /// <summary>
    /// Example: Using console commands
    /// </summary>
    public static void ExampleConsoleCommands()
    {
        // These would be typed in the SMAPI console:
        
        // Press W key once
        // keybind_press W
        
        // Hold space for 2 seconds
        // keybind_hold Space 2000
        
        // Execute WASD sequence
        // keybind_sequence W,A,S,D 200
        
        // Ctrl+C combo
        // keybind_combo LeftControl+C
        
        // Check status
        // keybind_status
        
        // Clear all active keys
        // keybind_clear
    }
}
