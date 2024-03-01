using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KNPE
{
    public class Graphics_Core
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Camera Position

        public static float ViewDistance = 80000;
        public static Matrix view;
        public static Matrix projection;
        public static Game gameAccess;
        public static bool Quit = false;

        public static Vector3 LightDirection = Vector3.Forward;

        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;

        public Graphics_Core(Game game)
        {
            graphics = new GraphicsDeviceManager(game);
            
            game.Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            gameAccess = game;


        }
        internal void Initialize(Game game)
        {


        }
        internal void LoadContent(Game game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            ScreenTextManager.LoadFont(game.Content, "Arial");
            LoadSprites(game);
            LoadModels(game);
            RenderEntity3d_Manager.Init(game.Content);
        }

        public static void LoadSprites(Game game)
        {
            SpriteManager.LoadSprite(0, game.Content, "MOTDImage");
            SpriteManager.LoadSprite(1, game.Content, "button");
        }

        public static void LoadModels(Game game)
        {
            RenderEntity3d_Manager.LoadModel(0, game.Content, "box", 1);
        }

        internal void UnloadContent(Game game)
        {
            
        }

        internal void Update(Game game, GameTime gameTime)
        {
            if (Quit)
            {
               game.Exit();
            }

            RenderEntity3d_Manager.ClearRenderList();
            SpriteManager.ClearRenderSpriteList();
            ScreenTextManager.ClearText();
        }



        internal void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(ClearOptions.Target,new Vector4(0,0.05f,0,255),1,0);
            RenderEntity3d_Manager.Draw(graphics.GraphicsDevice, gameTime);
            SpriteManager.Draw(spriteBatch);
            ScreenTextManager.Draw(spriteBatch);
        }
    }
}
