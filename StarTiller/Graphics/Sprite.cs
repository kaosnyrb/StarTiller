using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KNPE
{
    public class Sprite
    {
        //The texture object used when drawing the sprite
        public Texture2D mSpriteTexture;

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
        }
        //Draw the sprite to the screen
        public void Draw(SpriteBatch theSpriteBatch, Vector2 Position)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position, Color.White);
        }
    }

    public class RenderSprite
    {
        public bool Active = false;
        public Vector2 Position = new Vector2(0, 0);
        public int SpriteID = 0;
    }

    public static class SpriteManager
    {
        public static Sprite[] SpriteList;
        static RenderSprite[] RenderSpriteList;
        public static int MAXSPRITES = 27;
        public static int MAXRENDERSPRITES = 300;

        static SpriteManager()
        {
            SpriteList = new Sprite[MAXSPRITES];
            for (int i = 0; i < MAXSPRITES; i++)
            {
                SpriteList[i] = new Sprite();
            }
            RenderSpriteList = new RenderSprite[MAXRENDERSPRITES];
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                RenderSpriteList[i] = new RenderSprite();
            }
        }

        public static void LoadSprite(int Index, ContentManager theContentManager, string theAssetName)
        {
            SpriteList[Index].LoadContent(theContentManager, theAssetName);
        }

        public static void ClearRenderSpriteList()
        {
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                RenderSpriteList[i].Active = false;
            }
        }

        public static void RenderSprite(int SpriteID, Vector2 Position)
        {
            bool SetRenderSprite = false;
            for ( int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++ )
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    SetRenderSprite = true;
                }
            }
        }

        public static void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Begin();
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                if ( RenderSpriteList[i].Active )
                {
                    SpriteList[RenderSpriteList[i].SpriteID].Draw(theSpriteBatch,RenderSpriteList[i].Position);
                }
            }
            theSpriteBatch.End();
        }

        public static Texture2D GetTexture(int i)
        {
            return SpriteList[i].mSpriteTexture;
        }
    }
}
