using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Components;

namespace AttackOnTitan.Scenes
{
    public class StartScene : DrawableGameComponent
    {
        private SpriteBatch _sprite;
        private readonly List<IComponent> _components = new();

        public StartScene(Game game) : base(game) {}

        public override void Initialize()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;

            var startBtn = new SimpleButton("Играть",
                new Vector2(50, device.Viewport.Height - 200),
                Color.White);
            // var settingsBtn = new SimpleButton("Настройки",
            //     new Vector2(50, device.Viewport.Height - 200),
            //     Color.White);
            var exitBtn = new SimpleButton("Выход",
                new Vector2(50, device.Viewport.Height - 150),
                Color.White);

            startBtn.OnClick += () =>
            {
                Game.Components.Add(new StoryScene(Game));
                Game.Components.Remove(this);
            };
            exitBtn.OnClick += () => Game.Exit();

            _components.Add(startBtn);
            // _components.Add(settingsBtn);
            _components.Add(exitBtn);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string fontName = "Medium-18";
            _sprite = new SpriteBatch(Game.GraphicsDevice);

            if (!SceneManager.Textures.ContainsKey("Background"))
                SceneManager.Textures["Background"] = Game.Content.Load<Texture2D>("Textures/startBackground");
            if (!SceneManager.Fonts.ContainsKey(fontName))
            {
                SceneManager.Fonts[fontName] = Game.Content.Load<SpriteFont>("Fonts/OpenSans-Medium-18");
                SceneManager.FontSizes[SceneManager.Fonts[fontName]] = 18;
            }

            foreach (var component in _components)
            {
                var font = SceneManager.Fonts[fontName];
                var fontSize = SceneManager.FontSizes[font];
                var lineHeight = font.LineSpacing;
                var origin = new Vector2(0, (lineHeight - fontSize) / 2f);
                
                component.SetFont(font, origin);
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            foreach (var component in _components)
                component.Update(gameTime, mouseState);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            _sprite.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            _sprite.Draw(SceneManager.Textures["Background"], device.ScissorRectangle, Color.White);
            foreach (var component in _components)
                component.Draw(_sprite);
            _sprite.End();

            base.Draw(gameTime);
        }
    }
}
