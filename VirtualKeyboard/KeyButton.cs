using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;

namespace VirtualKeyboard
{
    internal class KeyButton
    {
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;
        public Rectangle OutterBounds;
        private Rectangle InnerBounds;
        private readonly SButton ButtonKey;
        private readonly float Transparency;
        private readonly string Alias;
        private readonly IModHelper Helper;
        public bool Hidden;

        public KeyButton(IModHelper helper, ModConfig.VirtualButton buttonDefine)
        {
            this.Hidden = true;
            this.ButtonKey = buttonDefine.key;
            this.Alias = buttonDefine.alias != "" ? buttonDefine.alias : this.ButtonKey.ToString();
            if ((double)buttonDefine.transparency <= 0.0099999997764825821 || (double)buttonDefine.transparency > 1.0)
                buttonDefine.transparency = 0.5f;
            this.Transparency = buttonDefine.transparency;
            this.Helper = helper;

            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Input.ButtonPressed += this.EventInputButtonPressed;
        }
        
        public void CalcBounds(int x, int y)
        {
            this.OutterBounds.X = x;
            this.OutterBounds.Y = y;

            Vector2 bounds = Game1.smallFont.MeasureString(this.Alias);
            this.InnerBounds.X = OutterBounds.X + ButtonBorderWidth;
            this.InnerBounds.Y = OutterBounds.Y + ButtonBorderWidth;
            this.InnerBounds.Width = (int)bounds.X + 1;
            this.InnerBounds.Height = (int)bounds.Y + 1;
            
            this.OutterBounds.Width = InnerBounds.Width + ButtonBorderWidth * 2;
            this.OutterBounds.Height = InnerBounds.Height + ButtonBorderWidth * 2;
        }
        private bool ShouldTrigger(Vector2 screenPixels, SButton button)
        {
            if (!this.OutterBounds.Contains(screenPixels.X, screenPixels.Y) || this.Hidden)
                return false;
            return true;
        }

        private void EventInputButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (this.Hidden)
                return;
            Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            if (ShouldTrigger(screenPixels, e.Button))
            {
                MethodInfo? overrideButton = Game1.input.GetType().GetMethod("OverrideButton");
                if (overrideButton != null)
                {
                    overrideButton.Invoke(Game1.input, new object[] { ButtonKey, true });
                }
            }
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            if (this.Hidden)
                return;
            float transparency = this.Transparency;
            e.SpriteBatch.Draw(Game1.menuTexture, OutterBounds, new Rectangle(0, 256, 60, 60), Color.White);
            e.SpriteBatch.DrawString(Game1.smallFont, this.Alias, new Vector2(this.InnerBounds.X, this.InnerBounds.Y), Game1.textColor);
        }
    }
}
