using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteFontPlus;

using AttackOnTitan.Components;
using AttackOnTitan.Models;

namespace AttackOnTitan.Scenes
{
    public class Level0Scene : DrawableGameComponent, IScene
    {
        public Dictionary<string, Texture2D> Textures { get; }
        public Dictionary<string, SpriteFont> Fonts { get; }
        public SpriteBatch Sprite { get; private set; }
        public List<IComponent> Components { get; }

        public Dictionary<OutputActionType, Action<UnitInfo, MapCellInfo>> CommandsActions = new();

        public GameModel GameModel;

        private MapComponent _mapComponent;

        private Keys _lastKey = Keys.None;

        private CharacterRange[] _characterRanges = new []
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

            _mapComponent = new MapComponent(this, 40, 35, 185, 160, 60, 60);

            CommandsActions[OutputActionType.AddUnit] = _mapComponent.AddUnit;
            CommandsActions[OutputActionType.MoveUnit] = _mapComponent.MoveUnit;
            CommandsActions[OutputActionType.ChangeUnitOpacity] = _mapComponent.ChangeUnitOpacity;
            CommandsActions[OutputActionType.ChangeCellOpacity] = _mapComponent.ChangeCellOpacity;
            CommandsActions[OutputActionType.StopUnit] = _mapComponent.StopUnit;

            GameModel = new(40, 35);
            GameModel.Run();

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            GameModel.Dispose();
            base.Dispose(disposing);
        }


        protected override void LoadContent()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            var texturesName = new[] { "Hexagon", "Ball", "Scout", "Garrison", 
                "Police", "Builder", "Cadet", "Titan" };

            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                30, 2048, 2048, _characterRanges).CreateSpriteFont(device);

            foreach (var textureName in texturesName)
                Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            UpdateKeyBoardState();
            RunModelActions();

            _mapComponent.Update(gameTime, mouseState);
            foreach (var component in Components)
                component.Update(gameTime, mouseState);

            base.Update(gameTime);
        }

        private void UpdateKeyBoardState()
        {
            var keyboardState = Keyboard.GetState();
            var keyboardPressed = keyboardState.GetPressedKeys();
            var keyPressed = keyboardPressed.Length == 0 ? Keys.None : keyboardPressed[0];

            if (keyPressed != _lastKey && keyPressed != Keys.None)
                GameModel.InputActions.Enqueue(new InputAction(keyPressed));

            _lastKey = keyPressed;
        }

        private void RunModelActions()
        {
            while (GameModel.OutputActions.TryDequeue(out var outputAction))
                CommandsActions[outputAction.ActionType](outputAction.UnitInfo, outputAction.MapCellInfo);
        }

        public override void Draw(GameTime gameTime)
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;

            _mapComponent.Draw(Sprite);


            //Sprite.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //Sprite.Draw(Textures["Background"], device.ScissorRectangle, Color.White);
            //foreach (var component in Components)
            //    component.Draw(Sprite);
            //Sprite.End();

            base.Draw(gameTime);
        }
    }
}
