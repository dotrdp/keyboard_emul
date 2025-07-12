using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace VirtualKeyboard.Inputs
{
    /// <summary>
    /// Manages virtual input state for keyboard emulation
    /// Based on TASMod's TASInputState pattern
    /// </summary>
    public static class VirtualInputState
    {
        /// <summary>
        /// Whether virtual input is currently active
        /// </summary>
        public static bool Active { get; set; } = false;

        /// <summary>
        /// Current virtual keyboard state
        /// </summary>
        private static VirtualKeyboardState _keyboardState = new VirtualKeyboardState();

        /// <summary>
        /// Reset the virtual input state
        /// </summary>
        public static void Reset()
        {
            Active = false;
            _keyboardState.Clear();
        }

        /// <summary>
        /// Get the current virtual keyboard state as a KeyboardState
        /// </summary>
        public static KeyboardState GetKeyboard()
        {
            return _keyboardState.GetKeyboardState();
        }

        /// <summary>
        /// Add a key to the virtual keyboard state
        /// </summary>
        public static void AddKey(Keys key)
        {
            _keyboardState.Add(key);
            Active = true;
        }

        /// <summary>
        /// Add multiple keys to the virtual keyboard state
        /// </summary>
        public static void AddKeys(IEnumerable<Keys> keys)
        {
            foreach (var key in keys)
            {
                _keyboardState.Add(key);
            }
            if (keys.Any())
            {
                Active = true;
            }
        }

        /// <summary>
        /// Remove a key from the virtual keyboard state
        /// </summary>
        public static void RemoveKey(Keys key)
        {
            _keyboardState.Remove(key);
            if (!_keyboardState.Any())
            {
                Active = false;
            }
        }

        /// <summary>
        /// Clear all virtual keys
        /// </summary>
        public static void ClearKeys()
        {
            _keyboardState.Clear();
            Active = false;
        }

        /// <summary>
        /// Set the complete virtual keyboard state
        /// </summary>
        public static void SetKeyboard(VirtualKeyboardState state)
        {
            _keyboardState.Clear();
            if (state != null && state.Any())
            {
                foreach (var key in state)
                {
                    _keyboardState.Add(key);
                }
                Active = true;
            }
            else
            {
                Active = false;
            }
        }

        /// <summary>
        /// Check if a specific key is currently pressed virtually
        /// </summary>
        public static bool IsKeyDown(Keys key)
        {
            return _keyboardState.Contains(key);
        }

        /// <summary>
        /// Get all currently pressed virtual keys
        /// </summary>
        public static Keys[] GetPressedKeys()
        {
            return _keyboardState.ToArray();
        }
    }

    /// <summary>
    /// Virtual keyboard state implementation
    /// Based on TASMod's TASKeyboardState pattern
    /// </summary>
    public class VirtualKeyboardState : HashSet<Keys>
    {
        public VirtualKeyboardState() : base() { }

        public VirtualKeyboardState(IEnumerable<Keys> keys) : base()
        {
            foreach (var key in keys)
            {
                Add(key);
            }
        }

        public VirtualKeyboardState(KeyboardState state) : this(state.GetPressedKeys()) { }

        public VirtualKeyboardState(string key) : base()
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (System.Enum.TryParse<Keys>(key, out Keys parsedKey))
                {
                    Add(parsedKey);
                }
            }
        }

        public VirtualKeyboardState(string[] keys) : base()
        {
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                
                if (System.Enum.TryParse<Keys>(key, out Keys parsedKey))
                {
                    Add(parsedKey);
                }
            }
        }

        public bool Add(string key)
        {
            if (System.Enum.TryParse<Keys>(key, out Keys parsedKey))
            {
                return Add(parsedKey);
            }
            return false;
        }

        public KeyboardState GetKeyboardState()
        {
            return new KeyboardState(this.ToArray());
        }
    }
}
