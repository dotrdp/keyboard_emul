using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System.Reflection;

namespace VirtualKeyboard
{
    internal class KeyButton
    {
        private readonly Rectangle ButtonRectangle;
        private readonly SButton ButtonKey;
        private readonly float Transparency;
        private readonly string Alias;
        private readonly string Command;
        public bool Hidden;
        private TextEntryMenu Menu;

        public KeyButton(IModHelper helper, ModConfig.VirtualButton buttonDefine)
        {
            this.Hidden = true;
            this.ButtonRectangle = new Rectangle(buttonDefine.rectangle.X, buttonDefine.rectangle.Y, buttonDefine.rectangle.Width, buttonDefine.rectangle.Height);
            this.ButtonKey = buttonDefine.key;
            this.Alias = buttonDefine.alias != null ? buttonDefine.alias : this.ButtonKey.ToString();
            this.Command = buttonDefine.command;
            if ((double)buttonDefine.transparency <= 0.0099999997764825821 || (double)buttonDefine.transparency > 1.0)
                buttonDefine.transparency = 0.5f;
            this.Transparency = buttonDefine.transparency;

            helper.Events.Display.Rendered += this.OnRendered;
            //helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.EventInputButtonReleased);
            helper.Events.Input.ButtonPressed += this.EventInputButtonPressed;
        }
        private bool ShouldTrigger(Vector2 screenPixels, SButton button)
        {
            Rectangle buttonRectangle = this.ButtonRectangle;
            if (!((Rectangle)ref buttonRectangle).Contains(screenPixels.X, screenPixels.Y) || this.Hidden || button != 1000)
                return false;
            if (!this.Hidden)
                Toolbar.toolbarPressed = true;
            return true;
        }

        private void EventInputButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            if (ShouldTrigger(screenPixels, e.Button))
            {
                if (Game1.input is SInputState input)
                    input.OverrideButton(this.ButtonKey, true);
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
            e.SpriteBatch.Draw(Game1.menuTexture, this.ButtonRectangle, Color.White);
            e.SpriteBatch.DrawString(Game1.smallFont, this.Alias, new Vector2(this.ButtonRectangle.X, this.ButtonRectangle.Y), Color.BurlyWood);
        }
    }
}
