using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace VirtualKeyboard
{
    internal class KeyButton
    {
        public const int ButtonBorderWidth = 4;
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

        public bool CalcBounds(int x, int y)
        {
            this.OutterBounds.X = x;
            this.OutterBounds.Y = y;

            if (Game1.smallFont == null)
            {
                return false;
            }
            Vector2 bounds = Game1.smallFont.MeasureString(this.Alias);
            this.InnerBounds.X = OutterBounds.X + ButtonBorderWidth;
            this.InnerBounds.Y = OutterBounds.Y + ButtonBorderWidth;
            this.InnerBounds.Width = (int)bounds.X + 1;
            this.InnerBounds.Height = (int)bounds.Y + 1;

            this.OutterBounds.Width = InnerBounds.Width + ButtonBorderWidth * 2;
            this.OutterBounds.Height = InnerBounds.Height + ButtonBorderWidth * 2;
            return true;
        }
        private bool ShouldTrigger(Vector2 screenPixels, SButton button)
        {
            if (!this.OutterBounds.Contains(screenPixels.X, screenPixels.Y) || this.Hidden)
                return false;
            return true;
        }

        private void EventInputButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (this.Hidden)
                return;
            Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            if (ShouldTrigger(screenPixels, e.Button))
            {
                MethodInfo? overrideButton = Game1.input.GetType().GetMethod("OverrideButton");
                if (overrideButton != null)
                {
                    overrideButton.Invoke(Game1.input, new object[] { ButtonKey, true });
                    this.Helper.Input.Suppress(SButton.MouseLeft);
                }
            }
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (this.Hidden)
                return;

            float transparency = this.Transparency;

            //e.SpriteBatch.Draw(Game1.menuTexture, OutterBounds, new Rectangle(0, 256, 60, 60), Color.White);
            Vector2 UIScaleOutterBounds = Utility.ModifyCoordinatesFromUIScale(new Vector2(this.OutterBounds.X, this.OutterBounds.Y));
            Rectangle UIScaleOutterBoundsRectangle;
            UIScaleOutterBoundsRectangle.X = (int)UIScaleOutterBounds.X;
            UIScaleOutterBoundsRectangle.Y = (int)UIScaleOutterBounds.Y;
            UIScaleOutterBoundsRectangle.Height = (int)Utility.ModifyCoordinateFromUIScale(OutterBounds.Height);
            UIScaleOutterBoundsRectangle.Width = (int)Utility.ModifyCoordinateFromUIScale(OutterBounds.Width);
            e.SpriteBatch.Draw(Game1.menuTexture, UIScaleOutterBoundsRectangle, new Rectangle(0, 256, 60, 60), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1E-06f);

            //e.SpriteBatch.DrawString(Game1.smallFont, this.Alias, new Vector2(this.InnerBounds.X, this.InnerBounds.Y), Game1.textColor);
            float UIScale = Utility.ModifyCoordinateFromUIScale(1.0f);
            Vector2 UIScaleInnerBounds = Utility.ModifyCoordinatesFromUIScale(new Vector2(this.InnerBounds.X, this.InnerBounds.Y));
            e.SpriteBatch.DrawString(Game1.smallFont, this.Alias, UIScaleInnerBounds, Game1.textColor, 0, new Vector2(0, 0), UIScale, SpriteEffects.None, 1E-06f);
        }
    }
}
