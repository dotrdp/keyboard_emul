using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Graphics.PackedVector;

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
        public bool Hidden;

        public KeyButton(IModHelper helper, ModConfig.VirtualButton buttonDefine)
        {
            this.Hidden = true;
            this.ButtonKey = buttonDefine.key;
            this.Alias = buttonDefine.alias != "" ? buttonDefine.alias : this.ButtonKey.ToString();
            if ((double)buttonDefine.transparency <= 0.0099999997764825821 || (double)buttonDefine.transparency > 1.0)
                buttonDefine.transparency = 0.5f;
            this.Transparency = buttonDefine.transparency;

            helper.Events.Display.Rendered += this.OnRendered;
            //helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.EventInputButtonReleased);
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
            Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            if (ShouldTrigger(screenPixels, e.Button))
            {
                //if (Game1.input is SInputState input)
                //    input.OverrideButton(this.ButtonKey, true);
            }
        }

        //private void EventInputButtonReleased(object sender, ButtonReleasedEventArgs e)
        //{
        //    if (this.RaisingReleased || !this.ShouldTrigger(Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels), e.Button))
        //        return;
        //    if (this.ButtonKey == 92)
        //        KeyboardInput.Show("Command", "", "", false).ContinueWith<string>((Func<Task<string>, string>)(s =>
        //        {
        //            string result = s.Result;
        //            if (result.Length > 0)
        //                this.SendCommand(result);
        //            return result;
        //        }));
        //    else if (this.ButtonKey == 163)
        //        SGameConsole.Instance.Show();
        //    else if (!string.IsNullOrEmpty(this.Command))
        //    {
        //        this.SendCommand(this.Command);
        //    }
        //    else
        //    {
        //        this.RaisingReleased = true;
        //        if (Game1.input is SInputState input)
        //            input.OverrideButton(this.ButtonKey, false);
        //        this.RaisingReleased = false;
        //    }
        //}

        //private void SendCommand(string command)
        //{
        //    SMainActivity.Instance.core.RawCommandQueue?.Add(command);
        //}

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            if (this.Hidden)
                return;
            float transparency = this.Transparency;
            //e.SpriteBatch.Draw(Game1.menuTexture, this.ButtonRectangle, Color.White);
            //IClickableMenu.drawTextureBox(e.SpriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
            e.SpriteBatch.Draw(Game1.menuTexture, OutterBounds, new Rectangle(0, 256, 60, 60), Color.White);
            e.SpriteBatch.DrawString(Game1.smallFont, this.Alias, new Vector2(this.InnerBounds.X, this.InnerBounds.Y), Game1.textColor);
        }
    }
}
