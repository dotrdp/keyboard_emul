using StardewModdingAPI;
using System.Diagnostics;

namespace VirtualKeyboard
{
    internal class ModConfig
    {
        public Toggle vToggle { get; set; } = new Toggle((SButton)0, new Rect(36, 12, 64, 64));

        public Rect ButtonsOffset = new Rect(36, 80, 0, 0);
        public VirtualButton[] Buttons { get; set; } = new VirtualButton[4]
        {
              new VirtualButton((SButton) 80, 0.5f),
              new VirtualButton((SButton) 73, 0.5f),
              new VirtualButton((SButton) 79, 0.5f),
              new VirtualButton((SButton) 81, 0.5f)
        };
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
        internal class VirtualButton
        {
            public SButton key { get; set; }
            public float transparency { get; set; } = 0.5f;
            public string alias { get; set; }
            public VirtualButton(
              SButton key,
              float transparency,
              string alias = "")
            {
                this.key = key;
                this.transparency = transparency;
                this.alias = alias;
            }
        }
    }
}
