using System;
using StardewModdingAPI;

namespace VirtualKeyboard
{
    /// <summary>
    /// Configuration for the VirtualKeyboard mod
    /// Note: This configuration is maintained for backwards compatibility
    /// but the mod now uses console-based keybind simulation instead of visual buttons
    /// </summary>
    internal class ModConfig
    {
        /// <summary>
        /// Whether the keybind system is enabled by default
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Default interval between key presses in sequences (milliseconds)
        /// </summary>
        public int DefaultSequenceInterval { get; set; } = 100;

        /// <summary>
        /// Default duration for key holds (milliseconds)
        /// </summary>
        public int DefaultHoldDuration { get; set; } = 50;

        /// <summary>
        /// Whether to log keybind operations for debugging
        /// </summary>
        public bool LogKeybindOperations { get; set; } = true;

        // Legacy configuration (kept for backwards compatibility)
        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        public Toggle vToggle { get; set; } = new Toggle(SButton.None, new Rect(0, 0, 0, 0));

        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        public int AboveMenu { get; set; } = 0;

        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        public float ButtonScale { get; set; } = 1.0f;

        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        public VirtualButton[][] Buttons { get; set; } = Array.Empty<VirtualButton[]>();

        // Legacy classes (kept for backwards compatibility)
        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        internal class Rect
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public Rect(int x, int y, int width, int height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
            }
        }

        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        internal class Toggle
        {
            public SButton key { get; set; }
            public Rect rectangle { get; set; }

            public Toggle(SButton key, Rect rectangle)
            {
                this.key = key;
                this.rectangle = rectangle;
            }
        }

        [System.Obsolete("Visual buttons are no longer supported. Use console commands instead.")]
        internal class VirtualButton
        {
            public SButton key { get; set; }
            public string alias { get; set; }

            public VirtualButton(SButton key, string alias = "")
            {
                this.key = key;
                this.alias = alias;
            }
        }
    }
}
