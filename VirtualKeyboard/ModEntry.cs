using System;
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
        private ModConfig config_ = new ModConfig();
        private List<KeyButton> buttons_ = new List<KeyButton>();
        private ClickableTextureComponent? virtual_toggle_button_;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.config_ = this.Helper.ReadConfig<ModConfig>();
            for (int index = 0; index < this.config_.buttons.Length; ++index)
            {
                this.buttons_.Add(new KeyButton(helper, this.config_.buttons[index]));
            }
            Texture2D texture = Helper.ModContent.Load<Texture2D>("assets/togglebutton.png");
            this.virtual_toggle_button_ = new ClickableTextureComponent(new Rectangle(64, 12, 128, 128), texture, new Rectangle(0, 0, 16, 16), 4f, false);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.Rendered += this.Rendered;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game draws to the sprite batch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Rendered(object? sender, RenderedEventArgs e)
        {
            if (this.virtual_toggle_button_ == null) return;
            ((ClickableComponent)this.virtual_toggle_button_).bounds.X = 200;
            ((ClickableComponent)this.virtual_toggle_button_).bounds.Y = 12;
            ((ClickableComponent)this.virtual_toggle_button_).bounds.X = this.config_.vToggle.rectangle.X;
            ((ClickableComponent)this.virtual_toggle_button_).bounds.Y = this.config_.vToggle.rectangle.Y;

            Game1.spriteBatch.End();
            Game1.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?(Matrix.CreateScale(1f)));
            this.virtual_toggle_button_.draw(Game1.spriteBatch, Color.Multiply(Color.White, 1.0f), 1E-06f, 0);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
    }
}