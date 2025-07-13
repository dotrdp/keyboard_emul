using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace VirtualKeyboard.Simulation
{
    /// <summary>
    /// Low-level Windows input simulator that works even when game is minimized
    /// Uses Windows API to inject keyboard input directly into the game process
    /// </summary>
    public static class WindowsInputSimulator
    {
        #region Windows API Constants
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        #endregion

        #region Windows API Functions
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        #endregion

        private static IntPtr? _gameWindowHandle;
        private static bool _initialized = false;

        /// <summary>
        /// Whether low-level input simulation is enabled
        /// </summary>
        public static bool Enabled { get; set; } = true;

        /// <summary>
        /// Initialize the Windows input simulator
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                // Try to find Stardew Valley window
                _gameWindowHandle = FindStardewValleyWindow();
                
                if (_gameWindowHandle.HasValue && _gameWindowHandle.Value != IntPtr.Zero)
                {
                    _initialized = true;
                    Patches.IPatch.Info($"Windows input simulator initialized for window handle: {_gameWindowHandle.Value}");
                }
                else
                {
                    Patches.IPatch.Warn("Could not find Stardew Valley window for low-level input simulation");
                }
            }
            catch (Exception ex)
            {
                Patches.IPatch.Error($"Failed to initialize Windows input simulator: {ex.Message}");
            }
        }

        /// <summary>
        /// Find the Stardew Valley game window
        /// </summary>
        private static IntPtr FindStardewValleyWindow()
        {
            var currentProcessId = GetCurrentProcessId();
            IntPtr foundWindow = IntPtr.Zero;

            // First try specific window titles
            string[] possibleTitles = {
                "Stardew Valley",
                "StardewValley", 
                "Stardew Valley v1.6",
                "Stardew Valley - SMAPI"
            };

            foreach (var title in possibleTitles)
            {
                var handle = FindWindow(null!, title);
                if (handle != IntPtr.Zero)
                {
                    GetWindowThreadProcessId(handle, out uint processId);
                    if (processId == currentProcessId)
                    {
                        return handle;
                    }
                }
            }

            // If specific titles don't work, enumerate all windows and find one from our process
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    GetWindowThreadProcessId(hWnd, out uint processId);
                    if (processId == currentProcessId)
                    {
                        var length = GetWindowTextLength(hWnd);
                        if (length > 0)
                        {
                            var sb = new System.Text.StringBuilder(length + 1);
                            GetWindowText(hWnd, sb, sb.Capacity);
                            var title = sb.ToString();
                            
                            // Look for any window that might be Stardew Valley
                            if (title.Contains("Stardew", StringComparison.OrdinalIgnoreCase) ||
                                title.Contains("Valley", StringComparison.OrdinalIgnoreCase) ||
                                title.Contains("SMAPI", StringComparison.OrdinalIgnoreCase) ||
                                title.Length > 10) // Any substantial window title from our process
                            {
                                foundWindow = hWnd;
                                return false; // Stop enumeration
                            }
                        }
                    }
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return foundWindow;
        }

        /// <summary>
        /// Send a key press to the game window
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code to press</param>
        /// <param name="hold">Whether to hold the key (true) or release it (false)</param>
        public static bool SendKeyInput(Keys key, bool pressed)
        {
            if (!Enabled || !_initialized || !_gameWindowHandle.HasValue) 
                return false;

            try
            {
                var virtualKeyCode = KeysToVirtualKey(key);
                if (virtualKeyCode == 0) return false;

                var message = pressed ? WM_KEYDOWN : WM_KEYUP;
                var result = PostMessage(_gameWindowHandle.Value, (uint)message, new IntPtr(virtualKeyCode), IntPtr.Zero);
                
                return result;
            }
            catch (Exception ex)
            {
                Patches.IPatch.Error($"Failed to send key input: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send key input asynchronously with duration
        /// </summary>
        /// <param name="key">The key to press</param>
        /// <param name="durationMs">How long to hold the key</param>
        public static async Task SendKeyInputAsync(Keys key, int durationMs = 50)
        {
            if (!Enabled || !_initialized) return;

            // Press key
            SendKeyInput(key, true);
            
            // Hold for duration
            if (durationMs > 0)
            {
                await Task.Delay(durationMs);
            }
            
            // Release key
            SendKeyInput(key, false);
        }

        /// <summary>
        /// Convert XNA Keys to Windows virtual key codes
        /// </summary>
        private static int KeysToVirtualKey(Keys key)
        {
            return key switch
            {
                // Movement keys
                Keys.W => 0x57, // W
                Keys.A => 0x41, // A  
                Keys.S => 0x53, // S
                Keys.D => 0x44, // D
                
                // Action keys
                Keys.C => 0x43, // C
                Keys.X => 0x58, // X
                Keys.F => 0x46, // F
                Keys.Y => 0x59, // Y
                Keys.N => 0x4E, // N
                
                // Inventory keys
                Keys.D1 => 0x31, // 1
                Keys.D2 => 0x32, // 2
                Keys.D3 => 0x33, // 3
                Keys.D4 => 0x34, // 4
                Keys.D5 => 0x35, // 5
                Keys.D6 => 0x36, // 6
                Keys.D7 => 0x37, // 7
                Keys.D8 => 0x38, // 8
                Keys.D9 => 0x39, // 9
                Keys.D0 => 0x30, // 0
                Keys.OemMinus => 0xBD, // -
                Keys.OemPlus => 0xBB,  // +
                
                // Menu keys
                Keys.E => 0x45, // E
                Keys.M => 0x4D, // M
                Keys.J => 0x4A, // J
                Keys.I => 0x49, // I
                Keys.Tab => 0x09, // Tab
                Keys.Escape => 0x1B, // Escape
                
                // Modifier keys
                Keys.LeftShift => 0xA0, // Left Shift
                Keys.RightShift => 0xA1, // Right Shift
                Keys.LeftControl => 0xA2, // Left Ctrl
                Keys.RightControl => 0xA3, // Right Ctrl
                Keys.LeftAlt => 0xA4, // Left Alt
                Keys.RightAlt => 0xA5, // Right Alt
                Keys.Space => 0x20, // Space
                Keys.Enter => 0x0D, // Enter
                
                _ => 0 // Unknown key
            };
        }

        /// <summary>
        /// Check if the simulator is properly initialized
        /// </summary>
        public static bool IsInitialized => _initialized && _gameWindowHandle.HasValue;

        /// <summary>
        /// Get status information
        /// </summary>
        public static string GetStatus()
        {
            return $"Windows Input Simulator - Initialized: {_initialized}, Window Handle: {_gameWindowHandle?.ToString() ?? "None"}, Enabled: {Enabled}";
        }

        /// <summary>
        /// Re-initialize the simulator (useful if window handle changes)
        /// </summary>
        public static void Reinitialize()
        {
            _initialized = false;
            _gameWindowHandle = null;
            Initialize();
        }
    }
}
