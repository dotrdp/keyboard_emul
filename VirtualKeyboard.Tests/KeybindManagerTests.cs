using System;
using System.Threading.Tasks;
using Xunit;

namespace VirtualKeyboard.Tests
{
    /// <summary>
    /// Tests for basic KeybindManager functionality
    /// </summary>
    public class KeybindManagerTests
    {
        [Fact]
        public void KeybindManager_Initialize_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(() => KeybindManager.Initialize());
            Assert.Null(exception);
        }

        [Fact]
        public void KeybindManager_InitialState_ShouldHaveNoActiveKeybinds()
        {
            // Arrange
            KeybindManager.Initialize();
            
            // Act & Assert
            Assert.False(KeybindManager.HasActiveKeybinds);
        }

        [Fact]
        public void KeybindManager_IsEnabled_ShouldDefaultToTrue()
        {
            // Arrange & Act & Assert
            Assert.True(KeybindManager.IsEnabled);
        }

        [Fact]
        public void KeybindManager_ClearAllKeys_ShouldNotThrow()
        {
            // Arrange
            KeybindManager.Initialize();
            
            // Act & Assert
            var exception = Record.Exception(() => KeybindManager.ClearAllKeys());
            Assert.Null(exception);
        }

        [Fact]
        public void KeybindManager_GetStatus_ShouldReturnString()
        {
            // Arrange
            KeybindManager.Initialize();
            
            // Act
            var status = KeybindManager.GetStatus();
            
            // Assert
            Assert.NotNull(status);
            Assert.True(status.Length > 0);
        }
    }
}
