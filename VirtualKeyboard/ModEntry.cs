using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace VirtualKeyboard
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig ModConfig = new ModConfig();
        private List<List<KeyButton>> Buttons = new List<List<KeyButton>>();
        private ClickableTextureComponent? VirtualToggleButton;
        private int EnabledStage = 0;
        private int LastPressTick = 0;
        private bool FirstRender = true;
        private bool ToolbarAlignTop = false;
        private bool ToolbarVertical = false;
        private int ToolbarItemSlotSize = 0;
        private int ToolbarHeight = 0;
        private Vector2 VirtualToggleButtonPosition = new Vector2(0, 0);

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.ModConfig = Helper.ReadConfig<ModConfig>();
            int buttonsLineNumber = this.ModConfig.Buttons.Length;
            for (int line = 0; line < buttonsLineNumber; line++)
            {
                this.Buttons.Add(new List<KeyButton>());
                for (int index = 0; index < this.ModConfig.Buttons[line].Length; ++index)
                {
                    this.Buttons[line].Add(new KeyButton(helper, this.ModConfig.Buttons[line][index]));
                }
            }

            Texture2D texture = helper.ModContent.Load<Texture2D>("assets/togglebutton.png");
            this.VirtualToggleButton = new ClickableTextureComponent(new Rectangle(this.ModConfig.vToggle.rectangle.X, this.ModConfig.vToggle.rectangle.Y, this.ModConfig.vToggle.rectangle.Width, this.ModConfig.vToggle.rectangle.Height), texture, new Rectangle(0, 0, 16, 16), 4f, false);

            helper.WriteConfig<ModConfig>(this.ModConfig);
            helper.Events.Display.Rendered += this.Rendered;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.VirtualToggleButtonPressed;
        }

        private void VirtualToggleButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            //Vector2 screenPixels = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
            Vector2 screenPixels = e.Cursor.ScreenPixels;
            if (e.Button == this.ModConfig.vToggle.key || ShouldTrigger(screenPixels))
            {
                foreach (List<KeyButton> keyButtonList in this.Buttons)
                    foreach (KeyButton keyButton in keyButtonList)
                        keyButton.Hidden = Convert.ToBoolean(this.EnabledStage);
                this.EnabledStage = 1 - this.EnabledStage;
                this.Helper.Input.Suppress(SButton.MouseLeft);
            }
        }
        private bool ShouldTrigger(Vector2 screenPixels)
        {
            if (this.VirtualToggleButton == null) return false;
            int ticks = Game1.ticks;
            if (ticks - this.LastPressTick <= 6 || !((ClickableComponent)this.VirtualToggleButton).containsPoint((int)screenPixels.X, (int)screenPixels.Y))
                return false;
            this.LastPressTick = ticks;
            return true;
        }
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            foreach (List<KeyButton> keyButtonList in this.Buttons)
                foreach (KeyButton keyButton in keyButtonList)
                    keyButton.Hidden = true;
            this.EnabledStage = 0;
        }

        private void CalVirtualToggleButtonPosition()
        {
            bool RecalButtonPosition = FirstRender;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                Type Game1OptionType = Game1.options.GetType();
                FieldInfo? verticalToolbarField = Game1OptionType.GetField("verticalToolbar");

                if (verticalToolbarField != null)
                {
                    bool currentToolbarVertical = Convert.ToBoolean(verticalToolbarField.GetValue(Game1.options));
                    RecalButtonPosition |= (ToolbarVertical != currentToolbarVertical);
                    ToolbarVertical = currentToolbarVertical;
                }

                foreach (IClickableMenu onScreenMenu in Game1.onScreenMenus)
                {
                    if (onScreenMenu is Toolbar)
                    {
                        Toolbar toolbar = (Toolbar)onScreenMenu;
                        Type ToolbarType = toolbar.GetType();
                        FieldInfo? alignTopField = ToolbarType.GetField("alignTop");
                        if (alignTopField != null)
                        {
                            bool currentAlignTop = Convert.ToBoolean(alignTopField.GetValue(toolbar));
                            RecalButtonPosition |= (ToolbarAlignTop != currentAlignTop);
                            ToolbarAlignTop = currentAlignTop;
                        }

                        PropertyInfo? itemSlotSizeProperty = ToolbarType.GetProperty("itemSlotSize");
                        if (itemSlotSizeProperty != null)
                        {
                            int currentItemSlotSize = Convert.ToInt32(itemSlotSizeProperty.GetValue(toolbar));
                            RecalButtonPosition |= (ToolbarItemSlotSize != currentItemSlotSize);
                            ToolbarItemSlotSize = currentItemSlotSize;
                        }

                        //FieldInfo? toolbarHeightField = ToolbarType.GetField("toolbarHeight");
                        //if (toolbarHeightField != null)
                        //{
                        //    int currentToolbarHeight = Convert.ToInt32(toolbarHeightField.GetValue(toolbar));
                        //    RecalButtonPosition |= (ToolbarHeight != currentToolbarHeight);
                        //    ToolbarHeight = currentToolbarHeight;
                        //}
                        ToolbarHeight = this.Helper.Reflection.GetField<int>(toolbar, "toolbarHeight").GetValue();

                        break;
                    }
                }
            }

            if (RecalButtonPosition)
            {
                int currentToolbarPaddingX = 0;
                Type Game1Type = typeof(Game1);
                FieldInfo? toolbarPaddingXField = Game1Type.GetField("toolbarPaddingX", BindingFlags.Public | BindingFlags.Static);
                if (toolbarPaddingXField != null)
                {
                    currentToolbarPaddingX = Convert.ToInt32(toolbarPaddingXField.GetValue(null));
                }

                int OffsetX = this.ModConfig.vToggle.rectangle.X;
                if (ToolbarVertical)
                {
                    OffsetX += currentToolbarPaddingX + ToolbarItemSlotSize + 20;
                }
                VirtualToggleButtonPosition.X = OffsetX;

                int OffsetY = this.ModConfig.vToggle.rectangle.Y;
                if (ToolbarAlignTop && !ToolbarVertical)
                {
                    OffsetY += ToolbarHeight + 16;
                }
                VirtualToggleButtonPosition.Y = OffsetY;
                OffsetY += this.ModConfig.vToggle.rectangle.Height + 4;

                bool all_calc = true;
                for (int line = 0; line < this.Buttons.Count; ++line)
                {
                    int LineOffsetX = OffsetX;
                    for (int index = 0; index < this.Buttons[line].Count; ++index)
                    {
                        if (!Buttons[line][index].CalcBounds(LineOffsetX, OffsetY))
                        {
                            all_calc = false;
                            break;
                        }
                        LineOffsetX = Buttons[line][index].OutterBounds.X + Buttons[line][index].OutterBounds.Width + 10;
                    }
                    OffsetY = Buttons[line][0].OutterBounds.Y + Buttons[line][0].OutterBounds.Height + 10;
                }
                FirstRender = !all_calc;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game draws to the sprite batch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Rendered(object? sender, RenderedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.VirtualToggleButton == null)
                return;

            CalVirtualToggleButtonPosition();

            Vector2 UIScalePos = Utility.ModifyCoordinatesFromUIScale(VirtualToggleButtonPosition);
            this.VirtualToggleButton.bounds.X = (int)UIScalePos.X;
            this.VirtualToggleButton.bounds.Y = (int)UIScalePos.Y;
            this.VirtualToggleButton.bounds.Height = (int)Utility.ModifyCoordinateFromUIScale(this.ModConfig.vToggle.rectangle.Height);
            this.VirtualToggleButton.bounds.Width = (int)Utility.ModifyCoordinateFromUIScale(this.ModConfig.vToggle.rectangle.Width);
            this.VirtualToggleButton.scale = Utility.ModifyCoordinateFromUIScale(4.0f);
            this.VirtualToggleButton.baseScale = this.VirtualToggleButton.scale;

            float scale = 0.5f + this.EnabledStage * 0.5f;
            this.VirtualToggleButton.draw(e.SpriteBatch, Color.Multiply(Color.White, scale), 1E-06f, 0);
        }
    }
}