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
        private bool RaisingPressed;
        private bool RaisingReleased;

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
            //helper.Events.Display.Rendered += new EventHandler<RenderedEventArgs>(this.OnRendered);
            //helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.EventInputButtonReleased);
            //helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.EventInputButtonPressed);
        }

        //private bool ShouldTrigger(Vector2 screenPixels, SButton button)
        //{
        //    Rectangle buttonRectangle = this.ButtonRectangle;
        //    if (!((Rectangle)ref buttonRectangle).Contains(screenPixels.X, screenPixels.Y) || this.Hidden || button != 1000)
        //        return false;
        //    if (!this.Hidden)
        //        Toolbar.toolbarPressed = true;
        //    return true;
        //}

        //private void EventInputButtonPressed(object sender, ButtonPressedEventArgs e)
        //{
        //    if (this.RaisingPressed)
        //        return;
        //    Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
        //    if (this.ButtonKey == null || !this.ShouldTrigger(screenPixels, e.Button))
        //        return;
        //    this.RaisingPressed = true;
        //    if (Game1.input is SInputState input)
        //        input.OverrideButton(this.ButtonKey, true);
        //    this.RaisingPressed = false;
        //}

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

        //private void OnRendered(object sender, EventArgs e)
        //{
        //    if (this.Hidden)
        //        return;
        //    float transparency = this.Transparency;
        //    int num;
        //    if (!Game1.eventUp)
        //    {
        //        switch (Game1.activeClickableMenu)
        //        {
        //            case GameMenu _:
        //            case ShopMenu _:
        //                break;
        //            default:
        //                num = Game1.activeClickableMenu == null ? 1 : 0;
        //                goto label_5;
        //        }
        //    }
        //    num = 0;
        //label_5:
        //    if (num != 0)
        //        transparency *= 0.5f;
        //    Matrix? nullable = Game1.spriteBatch.GetType().GetField("_spriteEffect", (BindingFlags)36)?.GetValue((object)Game1.spriteBatch) is SpriteEffect spriteEffect ? spriteEffect.TransformMatrix : new Matrix?();
        //    Game1.spriteBatch.End();
        //    Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?(Matrix.CreateScale(1f)));
        //    IClickableMenu.drawTextureBoxWithIconAndText(Game1.spriteBatch, Game1.smallFont, Game1.mouseCursors, new Rectangle(256, 256, 10, 10), (Texture2D)null, new Rectangle(0, 0, 1, 1), this.Alias, this.ButtonRectangle.X, this.ButtonRectangle.Y, this.ButtonRectangle.Width, this.ButtonRectangle.Height, Color.op_Multiply(Color.BurlyWood, transparency), 4f, true, false, true, false, false, false, false);
        //    Game1.spriteBatch.End();
        //    if (nullable.HasValue)
        //        Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?(nullable.Value));
        //    else
        //        Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?());
        //}
    }
}
