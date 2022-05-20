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
        private SpriteBatch Sprite { get; set; }

        private readonly Dictionary<OutputActionType, Action<OutputAction>> _commandsActions = new();

        private GameModel _gameModel;

        private MapComponent _mapComponent;
        private TopBarComponent _topBarComponent;
        private StepBtnComponent _stepBtnComponent;
        private CommandBarComponent _commandBarComponent;
        private BuilderChooseComponent _builderChooseComponent;

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
        }

        public override void Initialize()
        {
            var viewport = SceneManager.GraphicsMgr.GraphicsDevice.Viewport;
            _mapComponent = new MapComponent(this,222, 192, 60, 60);
            _topBarComponent = new TopBarComponent(this, viewport.Width, 35, 24);
            _stepBtnComponent = new StepBtnComponent(this, viewport.Width, viewport.Height, 24);
            _commandBarComponent = new CommandBarComponent(this, viewport.Width, viewport.Height);
            _builderChooseComponent = new BuilderChooseComponent(this, viewport.Width, viewport.Height);
            
            _commandsActions[OutputActionType.AddUnit] = action => _mapComponent.AddUnit(action.UnitInfo);
            _commandsActions[OutputActionType.MoveUnit] = action => _mapComponent.MoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.RemoveUnit] = action => _mapComponent.RemoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.StopUnit] = action => _mapComponent.StopUnit(action.UnitInfo);
            _commandsActions[OutputActionType.ChangeUnitOpacity] = action => _mapComponent.ChangeUnitOpacity(action.UnitInfo);
            _commandsActions[OutputActionType.InitializeMap] = action => _mapComponent.InitializeMap(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeTextureIntoCell] = action => _mapComponent.ChangeTextureIntoCell(action.MapCellInfo);
            _commandsActions[OutputActionType.ClearTextureIntoCell] = action => _mapComponent.ClearTextureIntoCell(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeCellOpacity] = action => _mapComponent.ChangeCellOpacity(action.MapCellInfo);
            _commandsActions[OutputActionType.UpdateNoServicedZoneForMap] = action => _mapComponent.UpdateNoServicedZone(action.NoServicedZone);
            _commandsActions[OutputActionType.AddResource] = action => _topBarComponent.AddResource(action.ResourceInfo);
            _commandsActions[OutputActionType.UpdateResourceCount] = action => _topBarComponent.UpdateResourceCount(action.ResourceInfo);
            _commandsActions[OutputActionType.ChangeStepBtnState] = _ => _stepBtnComponent.ChangeState();
            _commandsActions[OutputActionType.UpdateCommandsBar] = action => _commandBarComponent.UpdateCommands(action.CommandInfos);
            _commandsActions[OutputActionType.UpdateBuilderChoose] = action => _builderChooseComponent.UpdateBuildings(action.OutputBuildingInfo); 
            
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

            var texturesName = new[] { "Hexagon", "Ball", "Scout", "Garrison", 
                "Police", "Builder", "Cadet", "Titan", "Grass", "Coin", 
                "Log", "Stone", "TopBarBackground", "Step",
                "AttackIcon", "AttackIconHalf", "BuildingIcon", "BuildingIconHalf",
                "GasIcon", "GasIconHalf", "RefuelingIcon", "RefuelingIconHalf",
                "Barracks", "Centre", "House1", "House2", "House3", "Warehouse",
                "BuilderCard", "ExitIcon", "ExitIconHalf"
            };

            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                100, 2048, 2048, _characterRanges).CreateSpriteFont(device);
            foreach (var textureName in texturesName)
                Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            
            _topBarComponent.SetFont(Fonts["Medium"]);
            _stepBtnComponent.SetFont(Fonts["Medium"]);
            _topBarComponent.SetBackgroundTexture(Textures["TopBarBackground"]);
            _stepBtnComponent.SetBackgroundTexture(Textures["Step"]);
            
            RunModelActions();
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            UpdateKeyBoardState();
            RunModelActions();

            _stepBtnComponent.Update(gameTime, mouseState);
            _commandBarComponent.Update(gameTime, mouseState);
            _builderChooseComponent.Update(gameTime, mouseState);
            _mapComponent.Update(gameTime, mouseState);
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateWasEnd
            });

            base.Update(gameTime);
        }

        private void UpdateKeyBoardState()
        {
            var keyboardState = Keyboard.GetState();
            var keyboardPressed = keyboardState.GetPressedKeys();
            var keyPressed = keyboardPressed.Length == 0 ? Keys.None : keyboardPressed[0];

            if (keyPressed != _lastKey && keyPressed != Keys.None)
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.KeyPressed,
                    Key = keyPressed
                });

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
            _stepBtnComponent.Draw(Sprite);
            _commandBarComponent.Draw(Sprite);
            _builderChooseComponent.Draw(Sprite);

            base.Draw(gameTime);
        }
    }
}
