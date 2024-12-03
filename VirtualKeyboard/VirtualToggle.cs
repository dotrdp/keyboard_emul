using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System.Reflection;

namespace VirtualKeyboard
{
    internal class VirtualToggle
    {
        private IModHelper? Helper = null;
        private IMonitor? Monitor = null;
        private int EnabledStage = 0;
        private bool AutoHidden = true;
        private bool IsDefault = true;
        private ClickableTextureComponent VirtualToggleButton;
        private List<KeyButton> Keyboard = new List<KeyButton>();
        private List<KeyButton> KeyboardExtend = new List<KeyButton>();
        private ModConfig ModConfig;
        private Texture2D Texture;
        private int LastPressTick = 0;

        public void Init(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.Helper = helper;
            this.Texture = this.Helper.ModContent.Load<Texture2D>("assets/togglebutton.png");
            this.ModConfig = helper.ReadConfig<ModConfig>();
            for (int index = 0; index < this.ModConfig.buttons.Length; ++index)
                this.Keyboard.Add(new KeyButton(helper, this.ModConfig.buttons[index], this.Monitor));
            for (int index = 0; index < this.ModConfig.buttonsExtend.Length; ++index)
                this.KeyboardExtend.Add(new KeyButton(helper, this.ModConfig.buttonsExtend[index], this.Monitor));
            if (this.ModConfig.vToggle.rectangle.X != 36 || this.ModConfig.vToggle.rectangle.Y != 12)
                this.IsDefault = false;
            this.AutoHidden = this.ModConfig.vToggle.autoHidden;
            this.VirtualToggleButton = new ClickableTextureComponent(new Rectangle(Game1.toolbarPaddingX + 64, 12, 128, 128), this.Texture, new Rectangle(0, 0, 16, 16), 4f, false);
            helper.WriteConfig<ModConfig>(this.ModConfig);
            this.Helper.Events.Display.Rendered += new EventHandler<RenderedEventArgs>(this.OnRendered);
            this.Helper.Events.Display.MenuChanged += new EventHandler<MenuChangedEventArgs>(this.OnMenuChanged);
            this.Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.VirtualToggleButtonPressed);
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!this.AutoHidden || e.NewMenu == null)
                return;
            foreach (KeyButton keyButton in this.Keyboard)
                keyButton.Hidden = true;
            foreach (KeyButton keyButton in this.KeyboardExtend)
                keyButton.Hidden = true;
            this.EnabledStage = 0;
        }

        private void VirtualToggleButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            if (this.ModConfig.vToggle.key != null && e.Button == this.ModConfig.vToggle.key)
            {
                this.ToggleLogic();
            }
            else
            {
                if (e.Button != 1000 || !this.ShouldTrigger(screenPixels))
                    return;
                this.ToggleLogic();
            }
        }

        private void ToggleLogic()
        {
            switch (this.EnabledStage)
            {
                case 0:
                    foreach (KeyButton keyButton in this.Keyboard)
                        keyButton.Hidden = false;
                    foreach (KeyButton keyButton in this.KeyboardExtend)
                        keyButton.Hidden = true;
                    this.EnabledStage = 1;
                    return;
                case 1:
                    if (this.KeyboardExtend.Count > 0)
                    {
                        foreach (KeyButton keyButton in this.KeyboardExtend)
                            keyButton.Hidden = false;
                        this.EnabledStage = 2;
                        return;
                    }
                    break;
            }
            foreach (KeyButton keyButton in this.Keyboard)
                keyButton.Hidden = true;
            foreach (KeyButton keyButton in this.KeyboardExtend)
                keyButton.Hidden = true;
            this.EnabledStage = 0;
            IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
            if (activeClickableMenu == null || Game1.activeClickableMenu is DialogueBox)
                return;
            activeClickableMenu.exitThisMenu(true);
            Toolbar.toolbarPressed = true;
        }

        private bool ShouldTrigger(Vector2 screenPixels)
        {
            int ticks = Game1.ticks;
            if (ticks - this.LastPressTick <= 6 || !((ClickableComponent)this.VirtualToggleButton).containsPoint((int)screenPixels.X, (int)screenPixels.Y))
                return false;
            this.LastPressTick = ticks;
            Toolbar.toolbarPressed = true;
            return true;
        }

        private void OnRendered(object sender, EventArgs e)
        {
            if (this.IsDefault)
            {
                if (Game1.options.verticalToolbar)
                    ((ClickableComponent)this.VirtualToggleButton).bounds.X = Game1.toolbarPaddingX + Game1.toolbar.itemSlotSize + 200;
                else
                    ((ClickableComponent)this.VirtualToggleButton).bounds.X = Game1.toolbarPaddingX + Game1.toolbar.itemSlotSize + 50;
                if (Game1.toolbar.alignTop && !Game1.options.verticalToolbar)
                    ((ClickableComponent)this.VirtualToggleButton).bounds.Y = this.Helper.Reflection.GetField<int>((object)Game1.toolbar, "toolbarHeight", true).GetValue() + 50;
                else
                    ((ClickableComponent)this.VirtualToggleButton).bounds.Y = 12;
            }
            else
            {
                ((ClickableComponent)this.VirtualToggleButton).bounds.X = this.ModConfig.vToggle.rectangle.X;
                ((ClickableComponent)this.VirtualToggleButton).bounds.Y = this.ModConfig.vToggle.rectangle.Y;
            }
            float num = 1f;
            if (this.EnabledStage == 0)
                num = 0.5f;
            if (!Game1.eventUp && !(Game1.activeClickableMenu is GameMenu) && !(Game1.activeClickableMenu is ShopMenu))
                num = 0.25f;
            Matrix? nullable = Game1.spriteBatch.GetType().GetField("_spriteEffect", (BindingFlags)36)?.GetValue((object)Game1.spriteBatch) is SpriteEffect spriteEffect ? spriteEffect.TransformMatrix : new Matrix?();
            Game1.spriteBatch.End();
            Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?(Matrix.CreateScale(1f)));
            this.VirtualToggleButton.draw(Game1.spriteBatch, Color.op_Multiply(Color.White, num), 1E-06f, 0);
            Game1.spriteBatch.End();
            if (nullable.HasValue)
                Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?(nullable.Value));
            else
                Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?());
        }
    }
}
