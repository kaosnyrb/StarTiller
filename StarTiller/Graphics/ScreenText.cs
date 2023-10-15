using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KNPE
{
    class ScreenText
    {
        public string TextContent;
        public bool Active;
        public Vector2 Position;
        public Color TextColour;
        public ScreenText(string text)
        {
            TextContent = text;
            Position = new Vector2(0, 0);
            Active = false;
        }
        public void Draw(SpriteBatch batch,SpriteFont Font)
        {
            batch.Begin();
            
            batch.DrawString(Font, TextContent, Position + new Vector2(1, 1), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
            batch.DrawString(Font, TextContent, Position, TextColour, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
            
            batch.End();
        }
    }
    static class ScreenTextManager
    {
        public static ScreenText[] Textlist;
        public static int MAXTEXTCOUNT = 100;
        public static SpriteFont Font;

        static  ScreenTextManager()
        {
            Textlist = new ScreenText[MAXTEXTCOUNT];
            for (int i = 0; i < MAXTEXTCOUNT; i++)
            {
                Textlist[i] = new ScreenText("BLANK");
            }
        }
        public static void RenderText(string text, Vector2 Position, Color Colour)
        {
            bool SetText = false;
            for (int i = 0; i < MAXTEXTCOUNT & !SetText ; i++)
            {
                if (!Textlist[i].Active)
                {
                    Textlist[i].Active = true;
                    Textlist[i].Position = Position;
                    Textlist[i].TextColour = Colour;
                    Textlist[i].TextContent = text;
                    SetText = true;
                }
            }
        }
        public static void ClearText()
        {
            for (int i = 0; i < MAXTEXTCOUNT; i++)
            {
                Textlist[i].Active = false;
            }
        }

        public static void LoadFont(ContentManager theContentManager, string theAssetName)
        {
            Font = theContentManager.Load<SpriteFont>(theAssetName);
        }

        public static void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < MAXTEXTCOUNT; i++)
            {
                if (Textlist[i].Active)
                {
                    Textlist[i].Draw(batch, Font);
                }
            }
        }
    }
}
