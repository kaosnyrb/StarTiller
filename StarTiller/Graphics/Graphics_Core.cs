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

        public Graphics_Core(Game game)
        {
            graphics = new GraphicsDeviceManager(game);
            
            game.Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            gameAccess = game;


        }
        internal void Initialize(Game game)
        {


        }
        internal void LoadContent(Game game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            //ScreenTextManager.LoadFont(game.Content, "CreeperFont");
            LoadSprites(game);
            LoadModels(game);
            RenderEntity3d_Manager.Init(game.Content);
        }

        public static void LoadSprites(Game game)
        {
            SpriteManager.LoadSprite(0, game.Content, "MOTDImage");
        }

        public static void LoadModels(Game game)
        {
            //RenderEntity3d_Manager.LoadModel(0, game.Content, "TestPlayer", 1);
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
        }



        internal void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(ClearOptions.Target,new Vector4(0,0.05f,0,255),1,0);

            SpriteManager.RenderSprite(0, Vector2.Zero);

            RenderEntity3d_Manager.Draw(graphics.GraphicsDevice, gameTime);

            SpriteManager.Draw(spriteBatch);
            
            ScreenTextManager.Draw(spriteBatch);
        }
    }
}
