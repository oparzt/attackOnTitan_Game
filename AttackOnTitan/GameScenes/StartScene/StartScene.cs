using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteFontPlus;

using AttackOnTitan.GameComponents;


namespace AttackOnTitan.GameScenes.StartScene
{
   

    public class StartScene : DrawableGameComponent, IScene
    {
        public Dictionary<string, Texture2D> Textures { get; }
        public Dictionary<string, SpriteFont> Fonts { get; }
        public SpriteBatch Sprite { get; private set; }
        public List<IComponent> Components { get; }

        private CharacterRange[] _characterRanges = new CharacterRange[]
        {
            CharacterRange.BasicLatin,
            CharacterRange.Cyrillic
        };


        public StartScene(Game game) : base(game)
        {
            Textures = new();
            Fonts = new();
            Components = new();
        }

        public override void Initialize()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;

            var startBtn = new SimpleButton(this, "Medium", "Играть",
                new Vector2(50, device.Viewport.Height - 250),
                new Rectangle(45, device.Viewport.Height - 260, 90, 50),
                Color.White);
            var settingsBtn = new SimpleButton(this, "Medium", "Настройки",
                new Vector2(50, device.Viewport.Height - 200),
                new Rectangle(45, device.Viewport.Height - 210, 90, 50),
                Color.White);
            var exitBtn = new SimpleButton(this, "Medium", "Выход",
                new Vector2(50, device.Viewport.Height - 150),
                new Rectangle(45, device.Viewport.Height - 160, 90, 50),
                Color.White);

            startBtn.OnClick += () =>
            {
                Game.Components.Add(new StartScene(Game));
                Game.Components.Remove(this);
            };
            exitBtn.OnClick += () => Game.Exit();

            Components.Add(startBtn);
            Components.Add(settingsBtn);
            Components.Add(exitBtn);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            Textures["Background"] = Game.Content.Load<Texture2D>("Textures/startBackground");
            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                30, 2048, 2048, _characterRanges).CreateSpriteFont(device);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            foreach (var component in Components)
                component.Update(gameTime, mouseState);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            Sprite.Draw(Textures["Background"], device.ScissorRectangle, Color.White);
            foreach (var component in Components)
                component.Draw(Sprite);
            Sprite.End();

            base.Draw(gameTime);
        }
    }
}
