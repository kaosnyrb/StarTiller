using KNPE;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTiller
{
    public class Button
    {
        public Vector2 Position;
        public Vector2 Size;
        public int sprite;
        public string text;
        public Button(Vector2 Pos, Vector2 size, int sprite = 0, string text = "button")
        {
            Position = Pos;
            Size = size;
            this.sprite = sprite;
            this.text = text;   
        }
        public void Draw(GameTime gameTime)
        {
            SpriteManager.RenderSprite(sprite, Position);
            ScreenTextManager.RenderText(text, new Vector2(Position.X + 16, Position.Y + (Size.Y/2) - 8), Color.White);
        }

        public bool Clicked(Vector2 MouseClick)
        {
            if (MouseClick.X > Position.X
               && MouseClick.X < Position.X + Size.X
               && MouseClick.Y > Position.Y
               && MouseClick.Y < Position.Y + Size.Y)
                return true;
            else
                return false;

        }
    }
}
