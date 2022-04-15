using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteFontPlus;

using AttackOnTitan.GameComponents;

namespace AttackOnTitan.GameScenes
{
    public class Level0Scene : DrawableGameComponent, IScene
    {
        public Dictionary<string, Texture2D> Textures { get; }
        public Dictionary<string, SpriteFont> Fonts { get; }
        public SpriteBatch Sprite { get; private set; }
        public List<IComponent> Components { get; }

        private Map _map;

        private CharacterRange[] _characterRanges = new CharacterRange[]
        {
            CharacterRange.BasicLatin,
            CharacterRange.Cyrillic
        };


        public Level0Scene(Game game) : base(game)
        {
            Textures = new();
            Fonts = new();
            Components = new();
        }

        public override void Initialize()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;

            _map = new Map(this, 40, 35, 111, 96);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            Textures["Hexagon"] = Game.Content.Load<Texture2D>("Textures/hexagon");
            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                30, 2048, 2048, _characterRanges).CreateSpriteFont(device);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            _map.Update(gameTime, mouseState);
            foreach (var component in Components)
                component.Update(gameTime, mouseState);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;

            _map.Draw(Sprite);


            //Sprite.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //Sprite.Draw(Textures["Background"], device.ScissorRectangle, Color.White);
            //foreach (var component in Components)
            //    component.Draw(Sprite);
            //Sprite.End();

            base.Draw(gameTime);
        }
    }
}
