using StardewValley.Util;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        }

        /// <summary>
        /// Creates a virtual KeyboardState based on current movement inputs
        /// This follows the TASMod pattern for input simulation
        /// </summary>
        public KeyboardState GetKeyboardState()
        {
            var pressedKeys = new List<Keys>();

            // Map movement directions to WASD keys
            if (_moveUpPressed || _moveUpHeld)
                pressedKeys.Add(Keys.W);
            if (_moveDownPressed || _moveDownHeld)
                pressedKeys.Add(Keys.S);
            if (_moveLeftPressed || _moveLeftHeld)
                pressedKeys.Add(Keys.A);
            if (_moveRightPressed || _moveRightHeld)
                pressedKeys.Add(Keys.D);

            return new KeyboardState(pressedKeys.ToArray());
        }
    }
}
