using KNPE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ssf;
using ssf.Generation;
using ssf.IO;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace StarTiller
{
    class Ui_Core
    {
        String Console = "";
        bool running = false;
        public SeedStarfieldSettings settings;

        public Dictionary<string, Button> buttons;
        
        public Ui_Core() 
        {
            settings = YamlImporter.getObjectFromFile<SeedStarfieldSettings>("settings.yaml");
            SSFEventLog.EventLogs = new Queue<string>();
            BlockLib.Instance = new BlockLib
            {
                blocks = Utils.LoadBlockLib(settings.ContentPath)
            };

            //Buttons
            buttons = new Dictionary<string, Button>
            {
                { "Build", new Button(new Vector2(Graphics_Core.ScreenWidth - 128, 16), new Vector2(128, 64), 1, "Build") },
                { "Export", new Button(new Vector2(Graphics_Core.ScreenWidth - 128, 128), new Vector2(128, 64), 1, "Export") }
            };
        }

        public async Task ButtonHandlerAsync(Vector2 mousePosition)
        {
            if (running == false && buttons["Build"].Clicked(mousePosition))
            {
                running = true;
                Console = "";
                await Task.Run(() => RunGeneration());
            }
            if (running == false && buttons["Export"].Clicked(mousePosition))
            {
                running = true;
                Console = "";
                Process Spritggitcli = new Process();

                Spritggitcli.StartInfo.UseShellExecute = false;
                Spritggitcli.StartInfo.RedirectStandardOutput = true;
                Spritggitcli.StartInfo.RedirectStandardError = true;

                Spritggitcli.StartInfo.FileName = settings.SpriggitCli;
                Spritggitcli.StartInfo.Arguments = " deserialize --InputPath \"" + settings.GitModPath + "\" --OutputPath \"" + settings.DataFolder + settings.EspName + "\"";

                SSFEventLog.EventLogs.Enqueue("Running SpriggitCli with : " + Spritggitcli.StartInfo.Arguments);
                Spritggitcli.Start();
                while (!Spritggitcli.HasExited)
                {
                    Thread.Sleep(1000);
                }
                running = false;
                SSFEventLog.EventLogs.Enqueue(Spritggitcli.StandardOutput.ReadToEnd());
                SSFEventLog.EventLogs.Enqueue(Spritggitcli.StandardError.ReadToEnd());

            }
        }

        public async void Update(Game game, GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                await ButtonHandlerAsync(mousePosition);
            }

            while (SSFEventLog.EventLogs.Count > 0)
            {
                Console += SSFEventLog.EventLogs.Dequeue() + Environment.NewLine;
            }
            ScreenTextManager.RenderText(Console, new Vector2(32, 32), Color.White);
        }

        public void Draw(GameTime gameTime)
        {
            //SpriteManager.RenderSprite(0, Vector2.Zero);            
            foreach(var button in buttons)
            {
                button.Value.Draw(gameTime);
            }
        }

        public async void RunGeneration()
        {
            Mundus generator = new Mundus();
            List<Block> blocks = new List<Block>();
            do
            {
                generator.Setup(BlockLib.Instance);
                blocks = generator.Generate(settings.GenLength);
            } while (blocks.Count < settings.MinBlocks);
            BlockExporter.Export(blocks, settings);
            running = false;
        }

    }
}
