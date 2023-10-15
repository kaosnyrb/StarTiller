using KNPE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarTiller
{
    class Ui_Core
    {
        internal void Update(Game game, GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Microsoft.Xna.Framework.Point(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                ScreenTextManager.RenderText("Mouse state: " + mouseState.X + ":" + mouseState.Y, new Vector2(500, 500), Color.White);
            }
        }

        internal void Draw(GameTime gameTime)
        {
            SpriteManager.RenderSprite(0, Vector2.Zero);            
            SpriteManager.RenderSprite(1, new Vector2(Graphics_Core.ScreenWidth - 128, 0));

        }
    }
}
