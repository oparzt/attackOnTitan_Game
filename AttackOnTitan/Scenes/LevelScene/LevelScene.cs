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
    public class LevelScene : DrawableGameComponent, IScene
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
        private CreatingChooseComponent _creatingChooseComponent;
        private ProductionMenuComponent _productionMenuComponent;
        private UnitStatusBarComponent _unitStatusBarComponent;

        private Keys _lastKey = Keys.None;

        private readonly CharacterRange[] _characterRanges =
        {
            CharacterRange.BasicLatin,
            CharacterRange.Cyrillic
        };


        public LevelScene(Game game) : base(game)
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
            _creatingChooseComponent = new CreatingChooseComponent(this, viewport.Width, viewport.Height);
            _productionMenuComponent = new ProductionMenuComponent(this, viewport.Width, viewport.Height);
            _unitStatusBarComponent = new UnitStatusBarComponent(this, viewport.Width, viewport.Height, 24);
            
            _commandsActions[OutputActionType.AddUnit] = action => _mapComponent.AddUnit(action.UnitInfo);
            _commandsActions[OutputActionType.MoveUnit] = action => _mapComponent.MoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.RemoveUnit] = action => _mapComponent.RemoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.StopUnit] = action => _mapComponent.StopUnit(action.UnitInfo);
            _commandsActions[OutputActionType.ChangeUnitOpacity] = action => _mapComponent.ChangeUnitOpacity(action.UnitInfo);
            _commandsActions[OutputActionType.InitializeMap] = action => _mapComponent.InitializeMap(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeTextureIntoCell] = action => _mapComponent.ChangeTextureIntoCell(action.MapCellInfo);
            _commandsActions[OutputActionType.ClearTextureIntoCell] = action => _mapComponent.ClearTextureIntoCell(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeCellOpacity] = action => _mapComponent.ChangeCellOpacity(action.MapCellInfo);
            _commandsActions[OutputActionType.SetCellHidden] = action => _mapComponent.SetCellHidden(action.MapCellInfo);
            _commandsActions[OutputActionType.UpdateNoServicedZoneForMap] = action => _mapComponent.UpdateNoServicedZone(action.NoServicedZone);
            _commandsActions[OutputActionType.AddResource] = action => _topBarComponent.AddResource(action.ResourceInfo);
            _commandsActions[OutputActionType.UpdateResourceCount] = action => _topBarComponent.UpdateResourceCount(action.ResourceInfo);
            _commandsActions[OutputActionType.ChangeStepBtnState] = _ => _stepBtnComponent.ChangeState();
            _commandsActions[OutputActionType.ClearCommandsBar] = _commandBarComponent.ClearCommands;
            _commandsActions[OutputActionType.UpdateCommandsBar] = _commandBarComponent.UpdateCommands;
            _commandsActions[OutputActionType.InitializeCreatingChoose] = _creatingChooseComponent.InitializeCreatingChoose;
            _commandsActions[OutputActionType.UpdateCreatingChoose] = _creatingChooseComponent.UpdateBuildings;
            _commandsActions[OutputActionType.ClearCreatingChoose] = _creatingChooseComponent.ClearCreatingChoose;
            _commandsActions[OutputActionType.InitializeProductionMenu] = _productionMenuComponent.Initialize;
            _commandsActions[OutputActionType.UpdateProductionMenu] = _productionMenuComponent.UpdateProductionMenu;
            _commandsActions[OutputActionType.OpenProductionMenu] = _productionMenuComponent.OpenMenu;
            _commandsActions[OutputActionType.CloseProductionMenu] = _productionMenuComponent.CloseMenu;
            _commandsActions[OutputActionType.UpdateUnitStatusBar] = _unitStatusBarComponent.UpdateStatusBar;
            _commandsActions[OutputActionType.GameOver] = action =>
            {
                Game.Components.Add(new EndScene(Game, action.Win));
                Game.Components.Remove(this);
            };
            _commandsActions[OutputActionType.UpdateGameStepCount] = _topBarComponent.UpdateStepCount;

            _gameModel = new GameModel(23, 11);
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
                "BuilderCard", "ExitIcon", "ExitIconHalf", "WalkIcon", "WalkIconHalf", "People",
                "PlusIcon", "MinusIcon", "Wall", "OuterGates", "UnitStatusBar"
            };

            Fonts["Medium"] = TtfFontBaker.Bake(File.OpenRead("TTFFonts/OpenSans-Medium.ttf"),
                100, 2048, 2048, _characterRanges).CreateSpriteFont(device);
            foreach (var textureName in texturesName)
                Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            
            _topBarComponent.SetFont(Fonts["Medium"]);
            _stepBtnComponent.SetFont(Fonts["Medium"]);
            _unitStatusBarComponent.SetFont(Fonts["Medium"]);
            _topBarComponent.SetBackgroundTexture(Textures["TopBarBackground"]);
            _stepBtnComponent.SetBackgroundTexture(Textures["Step"]);
            _unitStatusBarComponent.SetBackgroundTexture(Textures["UnitStatusBar"]);
            
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
            _creatingChooseComponent.Update(gameTime, mouseState);
            _productionMenuComponent.Update(gameTime, mouseState);
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
            _creatingChooseComponent.Draw(Sprite);
            _productionMenuComponent.Draw(Sprite);
            _unitStatusBarComponent.Draw(Sprite);

            base.Draw(gameTime);
        }
    }
}
