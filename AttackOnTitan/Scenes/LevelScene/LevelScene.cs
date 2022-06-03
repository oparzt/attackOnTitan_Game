using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Components;
using AttackOnTitan.Models;

namespace AttackOnTitan.Scenes
{
    public class LevelScene : DrawableGameComponent
    {
        private readonly Dictionary<OutputActionType, Action<OutputAction>> _commandsActions = new();

        private GameModel _gameModel;
        private SpriteBatch _sprite;

        private MapComponent _mapComponent;
        private TopBarComponent _topBarComponent;
        private StepBtnComponent _stepBtnComponent;
        private CommandBarComponent _commandBarComponent;
        private CreatingChooseComponent _creatingChooseComponent;
        private ProductionMenuComponent _productionMenuComponent;
        private UnitStatusBarComponent _unitStatusBarComponent;

        private Keys _lastKey = Keys.None;

        public LevelScene(Game game) : base(game) {}

        public override void Initialize()
        {
            var viewport = SceneManager.GraphicsMgr.GraphicsDevice.Viewport;
            _mapComponent = new MapComponent(222, 192, 60, 60);
            _topBarComponent = new TopBarComponent(viewport.Width, 35);
            _stepBtnComponent = new StepBtnComponent(viewport.Width, viewport.Height);
            _commandBarComponent = new CommandBarComponent(viewport.Width, viewport.Height);
            _creatingChooseComponent = new CreatingChooseComponent(viewport.Width, viewport.Height);
            _productionMenuComponent = new ProductionMenuComponent(viewport.Width, viewport.Height);
            _unitStatusBarComponent = new UnitStatusBarComponent(viewport.Width, viewport.Height);
            
            _commandsActions[OutputActionType.AddUnit] = action => _mapComponent.AddUnit(action.UnitInfo);
            _commandsActions[OutputActionType.MoveUnit] = action => _mapComponent.MoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.RemoveUnit] = action => _mapComponent.RemoveUnit(action.UnitInfo);
            _commandsActions[OutputActionType.StopUnit] = action => _mapComponent.StopUnit(action.UnitInfo);
            _commandsActions[OutputActionType.ChangeUnitOpacity] = action => _mapComponent.ChangeUnitOpacity(action.UnitInfo);
            _commandsActions[OutputActionType.InitializeMap] = action => _mapComponent.InitializeMap(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeTextureIntoCell] = action => _mapComponent.ChangeTextureIntoCell(action.MapCellInfo);
            _commandsActions[OutputActionType.ChangeTextureOverCell] = action => _mapComponent.ChangeTextureOverCell(action.MapCellInfo);
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

            _gameModel = new GameModel(23, 16);
            _gameModel.InitMapBuildings();
            _gameModel.EconomyModel.UpdateResourceSettings();
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
            const string fontName14 = "Medium-14";
            const string fontName18 = "Medium-18";
            _sprite = new SpriteBatch(Game.GraphicsDevice);

            // UnitTextures
            // CommandTextures
            // MapTextures
            
            var texturesName = new[] {
                "TopBarBackground", "Step", "UnitStatusBar", 
                "Wall", "Grass", "PlusIcon", "MinusIcon",
                "BuilderCard", 
            };

            if (!SceneManager.Fonts.ContainsKey(fontName18))
            {
                SceneManager.Fonts[fontName18] = Game.Content.Load<SpriteFont>($"Fonts/OpenSans-{fontName18}");
                SceneManager.FontSizes[SceneManager.Fonts[fontName18]] = 18;
            }
            
            if (!SceneManager.Fonts.ContainsKey(fontName14))
            {
                SceneManager.Fonts[fontName14] = Game.Content.Load<SpriteFont>($"Fonts/OpenSans-{fontName14}");
                SceneManager.FontSizes[SceneManager.Fonts[fontName14]] = 14;
            }
            
            foreach (var textureName in texturesName.Where(textureName => !SceneManager.Textures.ContainsKey(textureName)))
                SceneManager.Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            foreach (var textureName in _gameModel.GetTexturesNames().Where(textureName => !SceneManager.Textures.ContainsKey(textureName)))
                SceneManager.Textures[textureName] = Game.Content.Load<Texture2D>("Textures/" + textureName);
            
            var font18 = SceneManager.Fonts[fontName18];
            var font18Size = SceneManager.FontSizes[font18];
            var font18LineHeight = font18.LineSpacing;
            var font18Origin = new Vector2(0, (font18LineHeight - font18Size) / 2f);
            _stepBtnComponent.SetFont(font18, font18Size, font18Origin);

            var font14 = SceneManager.Fonts[fontName14];
            var font14Size = SceneManager.FontSizes[font14];
            var font14LineHeight = font14.LineSpacing;
            var font14Origin = new Vector2(0, (font14LineHeight - font14Size) / 2f);
            _topBarComponent.SetFont(font14, font14Size, font14Origin);
            _creatingChooseComponent.SetFont(font14, font14Size, font14Origin);
            _productionMenuComponent.SetFont(font14, font14Size, font14Origin);
            _unitStatusBarComponent.SetFont(font14, font14Size, font14Origin);
            _topBarComponent.SetBackgroundTexture(SceneManager.Textures["TopBarBackground"]);
            _stepBtnComponent.SetBackgroundTexture(SceneManager.Textures["Step"]);
            _unitStatusBarComponent.SetBackgroundTexture(SceneManager.Textures["UnitStatusBar"]);
            
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
            _mapComponent.Draw(_sprite);
            
            _topBarComponent.Draw(_sprite);
            _stepBtnComponent.Draw(_sprite);
            _commandBarComponent.Draw(_sprite);
            _creatingChooseComponent.Draw(_sprite);
            _productionMenuComponent.Draw(_sprite);
            _unitStatusBarComponent.Draw(_sprite);

            base.Draw(gameTime);
        }
    }
}
