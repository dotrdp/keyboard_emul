using StardewValley.Util;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace VirtualKeyboard.Simulation
{
    /// <summary>
    /// Input simulator that provides virtual movement and action button simulation
    /// following the IInputSimulator interface pattern from Stardew Valley
    /// </summary>
    public class VirtualInputSimulator : IInputSimulator
    {
        private bool _moveUpPressed;
        private bool _moveRightPressed;
        private bool _moveLeftPressed;
        private bool _moveDownPressed;
        private bool _moveUpReleased;
        private bool _moveRightReleased;
        private bool _moveLeftReleased;
        private bool _moveDownReleased;
        private bool _moveUpHeld;
        private bool _moveRightHeld;
        private bool _moveLeftHeld;
        private bool _moveDownHeld;

        // Generic key tracking for all keys
        private readonly Dictionary<Keys, bool> _heldKeys = new();
        private readonly Dictionary<Keys, bool> _pressedKeys = new();
        private readonly Dictionary<Keys, bool> _releasedKeys = new();

        public static VirtualInputSimulator Instance { get; } = new();
        public static bool Active { get; set; } = false;

        public void SimulateInput(
            ref bool actionButtonPressed,
            ref bool switchToolButtonPressed,
            ref bool useToolButtonPressed,
            ref bool useToolButtonReleased,
            ref bool addItemToInventoryButtonPressed,
            ref bool cancelButtonPressed,
            ref bool moveUpPressed,
            ref bool moveRightPressed,
            ref bool moveLeftPressed,
            ref bool moveDownPressed,
            ref bool moveUpReleased,
            ref bool moveRightReleased,
            ref bool moveLeftReleased,
            ref bool moveDownReleased,
            ref bool moveUpHeld,
            ref bool moveRightHeld,
            ref bool moveLeftHeld,
            ref bool moveDownHeld)
        {
            if (!Active) return;

            // Apply our virtual movement inputs
            if (_moveUpPressed) moveUpPressed = true;
            if (_moveRightPressed) moveRightPressed = true;
            if (_moveLeftPressed) moveLeftPressed = true;
            if (_moveDownPressed) moveDownPressed = true;

            if (_moveUpReleased) moveUpReleased = true;
            if (_moveRightReleased) moveRightReleased = true;
            if (_moveLeftReleased) moveLeftReleased = true;
            if (_moveDownReleased) moveDownReleased = true;

            if (_moveUpHeld) moveUpHeld = true;
            if (_moveRightHeld) moveRightHeld = true;
            if (_moveLeftHeld) moveLeftHeld = true;
            if (_moveDownHeld) moveDownHeld = true;
        }

        public void SetMovementPressed(string direction)
        {
            ClearAllInputs();
            switch (direction.ToLower())
            {
                case "up":
                    _moveUpPressed = true;
                    _moveUpHeld = true;
                    break;
                case "down":
                    _moveDownPressed = true;
                    _moveDownHeld = true;
                    break;
                case "left":
                    _moveLeftPressed = true;
                    _moveLeftHeld = true;
                    break;
                case "right":
                    _moveRightPressed = true;
                    _moveRightHeld = true;
                    break;
            }
        }

        public void SetMovementReleased(string direction)
        {
            switch (direction.ToLower())
            {
                case "up":
                    _moveUpPressed = false;
                    _moveUpHeld = false;
                    _moveUpReleased = true;
                    break;
                case "down":
                    _moveDownPressed = false;
                    _moveDownHeld = false;
                    _moveDownReleased = true;
                    break;
                case "left":
                    _moveLeftPressed = false;
                    _moveLeftHeld = false;
                    _moveLeftReleased = true;
                    break;
                case "right":
                    _moveRightPressed = false;
                    _moveRightHeld = false;
                    _moveRightReleased = true;
                    break;
            }
        }

        public void ClearAllInputs()
        {
            _moveUpPressed = false;
            _moveRightPressed = false;
            _moveLeftPressed = false;
            _moveDownPressed = false;
            _moveUpReleased = false;
            _moveRightReleased = false;
            _moveLeftReleased = false;
            _moveDownReleased = false;
            _moveUpHeld = false;
            _moveRightHeld = false;
            _moveLeftHeld = false;
            _moveDownHeld = false;
            
            // Clear generic key tracking
            _pressedKeys.Clear();
            _releasedKeys.Clear();
            _heldKeys.Clear();
        }

        /// <summary>
        /// Set the state of a specific key for virtual input simulation
        /// </summary>
        public void SetKeyPressed(Keys key, bool pressed)
        {
            if (pressed)
            {
                _heldKeys[key] = true;
                _pressedKeys[key] = true;
                _releasedKeys.Remove(key);
                
                // CRITICAL FIX: Also handle movement-specific flags for movement keys
                switch (key)
                {
                    case Keys.W:
                        _moveUpPressed = true;
                        _moveUpHeld = true;
                        break;
                    case Keys.S:
                        _moveDownPressed = true;
                        _moveDownHeld = true;
                        break;
                    case Keys.A:
                        _moveLeftPressed = true;
                        _moveLeftHeld = true;
                        break;
                    case Keys.D:
                        _moveRightPressed = true;
                        _moveRightHeld = true;
                        break;
                }
            }
            else
            {
                _heldKeys.Remove(key);
                _pressedKeys.Remove(key);
                _releasedKeys[key] = true;
                
                // CRITICAL FIX: Also clear movement-specific flags for movement keys
                switch (key)
                {
                    case Keys.W:
                        _moveUpPressed = false;
                        _moveUpHeld = false;
                        _moveUpReleased = true;
                        break;
                    case Keys.S:
                        _moveDownPressed = false;
                        _moveDownHeld = false;
                        _moveDownReleased = true;
                        break;
                    case Keys.A:
                        _moveLeftPressed = false;
                        _moveLeftHeld = false;
                        _moveLeftReleased = true;
                        break;
                    case Keys.D:
                        _moveRightPressed = false;
                        _moveRightHeld = false;
                        _moveRightReleased = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Check if a specific key is currently being held
        /// </summary>
        public bool IsKeyHeld(Keys key)
        {
            return _heldKeys.ContainsKey(key);
        }

        /// <summary>
        /// Creates a virtual KeyboardState based on current movement inputs and KeybindManager state
        /// This follows the TASMod pattern for input simulation
        /// </summary>
        public KeyboardState GetKeyboardState()
        {
            // If not active, return empty state
            if (!Active)
            {
                return new KeyboardState();
            }
            
            var pressedKeys = new List<Keys>();

            // Add movement keys based on internal state
            if (_moveUpPressed || _moveUpHeld)
                pressedKeys.Add(Keys.W);
            if (_moveDownPressed || _moveDownHeld)
                pressedKeys.Add(Keys.S);
            if (_moveLeftPressed || _moveLeftHeld)
                pressedKeys.Add(Keys.A);
            if (_moveRightPressed || _moveRightHeld)
                pressedKeys.Add(Keys.D);

            // Add generic held keys
            foreach (var key in _heldKeys.Keys)
            {
                if (!pressedKeys.Contains(key))
                {
                    pressedKeys.Add(key);
                }
            }

            // Add keys from KeybindManager
            if (KeybindManager.IsEnabled)
            {
                var heldKeys = KeybindManager.GetHeldKeys();
                foreach (var sButton in heldKeys)
                {
                    // Convert SButton to Keys if possible
                    if (TryConvertSButtonToKeys(sButton, out Keys key))
                    {
                        if (!pressedKeys.Contains(key))
                            pressedKeys.Add(key);
                    }
                }
            }

            return new KeyboardState(pressedKeys.ToArray());
        }

        /// <summary>
        /// Convert SButton to Keys enum for keyboard state generation
        /// </summary>
        private bool TryConvertSButtonToKeys(SButton sButton, out Keys key)
        {
            // Handle movement keys
            switch (sButton)
            {
                case SButton.W: key = Keys.W; return true;
                case SButton.A: key = Keys.A; return true;
                case SButton.S: key = Keys.S; return true;
                case SButton.D: key = Keys.D; return true;
                
                // Action keys
                case SButton.C: key = Keys.C; return true;
                case SButton.X: key = Keys.X; return true;
                case SButton.F: key = Keys.F; return true;
                case SButton.Y: key = Keys.Y; return true;
                case SButton.N: key = Keys.N; return true;
                
                // Inventory keys  
                case SButton.D1: key = Keys.D1; return true;
                case SButton.D2: key = Keys.D2; return true;
                case SButton.D3: key = Keys.D3; return true;
                case SButton.D4: key = Keys.D4; return true;
                case SButton.D5: key = Keys.D5; return true;
                case SButton.D6: key = Keys.D6; return true;
                case SButton.D7: key = Keys.D7; return true;
                case SButton.D8: key = Keys.D8; return true;
                case SButton.D9: key = Keys.D9; return true;
                case SButton.D0: key = Keys.D0; return true;
                case SButton.OemMinus: key = Keys.OemMinus; return true;
                case SButton.OemPlus: key = Keys.OemPlus; return true;
                
                // Menu keys
                case SButton.Escape: key = Keys.Escape; return true;
                case SButton.E: key = Keys.E; return true;
                case SButton.I: key = Keys.I; return true;
                case SButton.M: key = Keys.M; return true;
                case SButton.J: key = Keys.J; return true;
                case SButton.Tab: key = Keys.Tab; return true;
                
                // Modifier keys
                case SButton.LeftShift: key = Keys.LeftShift; return true;
                case SButton.RightShift: key = Keys.RightShift; return true;
                case SButton.LeftControl: key = Keys.LeftControl; return true;
                case SButton.RightControl: key = Keys.RightControl; return true;
                case SButton.LeftAlt: key = Keys.LeftAlt; return true;
                case SButton.RightAlt: key = Keys.RightAlt; return true;
                
                // Space and Enter
                case SButton.Space: key = Keys.Space; return true;
                case SButton.Enter: key = Keys.Enter; return true;
                
                default:
                    key = Keys.None;
                    return false;
            }
        }
    }
}
