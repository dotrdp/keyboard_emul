using StardewModdingAPI;
using System.Diagnostics;

namespace VirtualKeyboard
{
    internal class ModConfig
    {
        public VirtualButton vToggle { get; set; } = new VirtualButton((SButton)0, new Rect(36, 12, 64, 64), 0.5f);
        public VirtualButton[] Buttons { get; set; } = new VirtualButton[4]
        {
              new VirtualButton((SButton) 80, new Rect(190, 80, 90, 90), 0.5f),
              new VirtualButton((SButton) 73, new Rect(290, 80, 90, 90), 0.5f),
              new VirtualButton((SButton) 79, new Rect(390, 80, 90, 90), 0.5f),
              new VirtualButton((SButton) 81, new Rect(490, 80, 90, 90), 0.5f)
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

        internal class VirtualButton
        {
            public SButton key { get; set; }
            public Rect rectangle { get; set; }
            public float transparency { get; set; } = 0.5f;
            public string command { get; set; }
            public string? alias { get; set; } = null;

            public VirtualButton(
              SButton key,
              Rect rectangle,
              float transparency,
              string command = "",
              string? alias = null)
            {
                this.key = key;
                this.rectangle = rectangle;
                this.transparency = transparency;
                this.command = command;
                this.alias = alias;
            }
        }
    }
}
