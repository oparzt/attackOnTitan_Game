using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using AttackOnTitan.Scenes;

namespace AttackOnTitan
{
    public class SceneManager : Game
    {
        public static GraphicsDeviceManager GraphicsMgr;

        public SceneManager()
        {
            GraphicsMgr = new GraphicsDeviceManager(this);
            GraphicsMgr.GraphicsProfile = GraphicsProfile.HiDef;

            Window.AllowUserResizing = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "Attack On Titan - Game";
            Window.IsBorderless = true;
            //Window.BeginScreenDeviceChange(true);
            GraphicsMgr.PreferMultiSampling = true;
            GraphicsMgr.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            GraphicsMgr.PreferredBackBufferWidth = GraphicsMgr.GraphicsDevice.DisplayMode.Width;
            GraphicsMgr.PreferredBackBufferHeight = GraphicsMgr.GraphicsDevice.DisplayMode.Height;
            GraphicsMgr.ApplyChanges();
            //Window.EndScreenDeviceChange("Attack On Titan - Game",
            //    _graphics.GraphicsDevice.DisplayMode.Width,
            //    _graphics.GraphicsDevice.DisplayMode.Height);


            // Components.Add(new StartScene(this));
            Components.Add(new Level0Scene(this));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            var song = Content.Load<Song>("Songs/BackgroundSong");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
