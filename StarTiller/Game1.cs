using KNPE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarTiller
{
    public class Game1 : Game
    {
        Graphics_Core GCore;
        Ui_Core ui_Core;
        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            GCore = new Graphics_Core(this);
            ui_Core = new Ui_Core();
        }

        protected override void Initialize()
        {
            GCore.Initialize(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GCore.LoadContent(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            GCore.Update(this, gameTime);
            base.Update(gameTime);
            ui_Core.Update(this,gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            ui_Core.Draw(gameTime);
            GCore.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}