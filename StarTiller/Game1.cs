using KNPE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarTiller
{
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        
        Graphics_Core GCore;
        
        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            GCore = new Graphics_Core(this);
        }

        protected override void Initialize()
        {
            GCore.Initialize(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GCore.LoadContent(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GCore.Update(this, gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GCore.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}