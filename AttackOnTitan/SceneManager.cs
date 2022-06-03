using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using AttackOnTitan.Scenes;

namespace AttackOnTitan
{
    public class SceneManager : Game
    {
        public static GraphicsDeviceManager GraphicsMgr;
        public static readonly Dictionary<string, Texture2D> Textures = new ();
        public static readonly Dictionary<string, SpriteFont> Fonts = new ();
        public static readonly Dictionary<SpriteFont, int> FontSizes = new ();

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
            Window.Title = "Attack On Titan - Rebirth of Humanity";
            Window.IsBorderless = true;
            GraphicsMgr.PreferMultiSampling = true;
            GraphicsMgr.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            GraphicsMgr.PreferredBackBufferWidth = GraphicsMgr.GraphicsDevice.DisplayMode.Width;
            GraphicsMgr.PreferredBackBufferHeight = GraphicsMgr.GraphicsDevice.DisplayMode.Height;
            GraphicsMgr.ApplyChanges();

            Components.Add(new StartScene(this));
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            Content.Unload();
            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            var song = Content.Load<Song>("Songs/BackgroundSong");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.15f;

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
