using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
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

        private readonly Dictionary<OutputActionType, Action<OutputAction>> _commandsActions = new();

        private GameModel _gameModel;

        private MapComponent _mapComponent;
        private TopBarComponent _topBarComponent;

        private Keys _lastKey = Keys.None;

        private readonly CharacterRange[] _characterRanges =
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
            var viewport = SceneManager.GraphicsMgr.GraphicsDevice.Viewport;
            _mapComponent = new MapComponent(this, 40, 35, 
                222, 192, 60, 60);
            _topBarComponent = new TopBarComponent(this, viewport.Width, 35, 24);
            
            _commandsActions[OutputActionType.AddUnit] = action => _mapComponent.AddUnit(action.UnitInfo);
            _commandsActions[OutputActionType.MoveUnit] = action => _mapComponent.MoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.StopUnit] = action => _mapComponent.StopUnit(action.UnitInfo);
            _commandsActions[OutputActionType.ChangeUnitOpacity] = action => _mapComponent.ChangeUnitOpacity(action.UnitInfo);
            _commandsActions[OutputActionType.ChangeCellOpacity] = action => _mapComponent.ChangeCellOpacity(action.MapCellInfo);
            _commandsActions[OutputActionType.AddResource] = action => _topBarComponent.AddResource(action.ResourceInfo);
            _commandsActions[OutputActionType.UpdateResourceCount] = action => _topBarComponent.UpdateResourceCount(action.ResourceInfo);
            
            _gameModel = new GameModel(40, 35);
            _gameModel.Run();

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            _gameModel.Dispose();
            base.Dispose(disposing);
        }


        protected override void LoadContent()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            var texturesName = new[] { "Hexagon", "Ball", 
                "Scout", "Garrison", "Police", "Builder", "Cadet", "Titan", 
                "Grass", 
                "Coin", "Log", "Stone", "TopBarBackground" };

            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                100, 2048, 2048, _characterRanges).CreateSpriteFont(device);
            
            foreach (var textureName in texturesName)
                Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            
            _topBarComponent.SetFont(Fonts["Medium"]);
            _topBarComponent.SetBackgroundTexture(Textures["TopBarBackground"]);
            
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
                _commandsActions[outputAction.ActionType](outputAction);
        }

        public override void Draw(GameTime gameTime)
        {
            _mapComponent.Draw(Sprite);
            _topBarComponent.Draw(Sprite);

            base.Draw(gameTime);
        }
    }
}
