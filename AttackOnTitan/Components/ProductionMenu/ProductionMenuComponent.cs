using System.Collections.Generic;
using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class ProductionMenuComponent
    {
        private const string InfoStr = "Информация";
        private const string ProductionStr = "Производство";
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        
        private Dictionary<ResourceType, Texture2D> _resourceTextures;
        private Texture2D _backgroundTexture;
        private Texture2D _plusIconTexture;
        private Texture2D _minusIconTexture;
        private SpriteFont _font;
        private int _fontSize;
        private Vector2 _textOrigin;

        private Rectangle _infoBackgroundRect;
        private Rectangle _prodBackgroundRect;
        
        private Vector2 _infoStrPos;
        private Vector2 _prodStrPos;

        private (Texture2D, Rectangle, string, Vector2)[] _infoRes;
        private (Texture2D, Rectangle, string, Vector2)[] _prodRes;
        private (Texture2D, Rectangle, string, Vector2) _peopleAtWork;

        private (Rectangle, ResourceType)[] _minusBtns;
        private (Rectangle, ResourceType)[] _plusBtns;
        private bool[] _availableMinusBtns;
        private bool[] _availablePlusBtns;
        private bool[] _minusWasPressed;
        private bool[] _plusWasPressed;

        private MouseBtn _lastMouseBtn;

        private bool _isVisible;
        private UnitInfo _unitInfo;
        private MapCellInfo _mapCellInfo;

        public ProductionMenuComponent(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }
        
        public void Initialize(OutputAction action)
        {
            var productionInfo = action.ProductionInfo;
            _resourceTextures = productionInfo.ResourceTexturesName
                .Select(pair => new KeyValuePair<ResourceType, Texture2D>(pair.Key, SceneManager.Textures[pair.Value]))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            _backgroundTexture = SceneManager.Textures[productionInfo.BackgroundTextureName];
            _plusIconTexture = SceneManager.Textures["PlusIcon"];
            _minusIconTexture = SceneManager.Textures["MinusIcon"];
        }

        public void SetFont(SpriteFont font, int fontSize, Vector2 origin)
        {
            _font = font;
            _fontSize = fontSize;
            _textOrigin = origin;
        }
        
        public void UpdateProductionMenu(OutputAction action)
        {
            var productionInfo = action.ProductionInfo;
            UpdateInfo(productionInfo.ResourceInformation);
            UpdateProduction(productionInfo.CanUpdateProductionResource,
                productionInfo.PeopleAtWork, productionInfo.CanUpdateProduction);
        }

        private void UpdateInfo(Dictionary<ResourceType, string> resourceInfo)
        {
            const int cardWidth = 300;
            var cardHeight = resourceInfo.Count * 35 + 35;

            var startX = _viewportWidth / 2 - 8 - cardWidth;
            var startY = _viewportHeight / 2 - cardHeight / 2;

            _infoBackgroundRect = new Rectangle(startX, startY, cardWidth, cardHeight);
            _infoStrPos = new Vector2(startX + 10, startY + 5);

            _infoRes = resourceInfo
                .Select((pair, pairIndex) => (_resourceTextures[pair.Key],
                    new Rectangle(startX + 10, startY + 35 + 35 * pairIndex, 30, 30), 
                    pair.Value, 
                    new Vector2(startX + 50, startY + 35 + (30 - _fontSize) / 2 + 35 * pairIndex)))
                .ToArray();
        }

        private void UpdateProduction(Dictionary<ResourceType, string> resourceInfo,
            (ResourceType, string) peopleAtWork, (bool, bool)[] canUpdateProduction)
        {
            const int cardWidth = 300;
            var cardHeight = resourceInfo.Count * 35 + 70;

            var startX = _viewportWidth / 2 + 8;
            var startY = _viewportHeight / 2 - cardHeight / 2;

            _prodBackgroundRect = new Rectangle(startX, startY, cardWidth, cardHeight);
            _prodStrPos = new Vector2(startX + 10, startY + 5);
            _peopleAtWork = (_resourceTextures[peopleAtWork.Item1], 
                new Rectangle(startX + 10, startY + 35, 30, 30),
                peopleAtWork.Item2,
                new Vector2(startX + 50, startY + 38));

            _prodRes = resourceInfo
                .Select((pair, pairIndex) => (_resourceTextures[pair.Key],
                    new Rectangle(startX + 10, startY + 70 + 35 * pairIndex, 30, 30), 
                    pair.Value, 
                    new Vector2(startX + 50, startY + 70 + (30 - _fontSize) / 2 + 35 * pairIndex)))
                .ToArray();

            UpdateMinusBtns(resourceInfo, canUpdateProduction, startX, startY, cardWidth);
            UpdatePlusBtns(resourceInfo, canUpdateProduction, startX, startY, cardWidth);
        }

        private void UpdateMinusBtns(Dictionary<ResourceType, string> resourceInfo,
            (bool, bool)[] canUpdateProduction, int startX, int startY, int cardWidth)
        {
            _minusBtns = resourceInfo
                .Select((pair, pairIndex) => (
                    new Rectangle(startX + cardWidth - 80, startY + 70 + 35 * pairIndex, 30, 30),
                    pair.Key))
                .ToArray();
            _availableMinusBtns = canUpdateProduction.Select(pair => pair.Item1).ToArray();
            _minusWasPressed = canUpdateProduction.Select(pair => false).ToArray();
        }
        
        private void UpdatePlusBtns(Dictionary<ResourceType, string> resourceInfo,
            (bool, bool)[] canUpdateProduction, int startX, int startY, int cardWidth)
        {
            _plusBtns = resourceInfo
                .Select((pair, pairIndex) => (
                    new Rectangle(startX + cardWidth - 40, startY + 70 + 35 * pairIndex, 30, 30),
                    pair.Key))
                .ToArray();
            _availablePlusBtns = canUpdateProduction.Select(pair => pair.Item2).ToArray();
            _plusWasPressed = canUpdateProduction.Select(pair => false).ToArray();
        }

        public void OpenMenu(OutputAction action) => SetVisible(true, action.MapCellInfo, action.UnitInfo);
        public void CloseMenu(OutputAction action) => SetVisible(false, action.MapCellInfo, action.UnitInfo);
        
        private void SetVisible(bool isVisible, MapCellInfo cellInfo, UnitInfo unitInfo)
        {
            _isVisible = isVisible;
            _mapCellInfo = cellInfo;
            _unitInfo = unitInfo;
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.BuilderChoose)
                {
                    Zones = new [] 
                    { 
                        isVisible ? 
                        new Rectangle(0,0, _viewportWidth, _viewportHeight) :
                        Rectangle.Empty
                    }
                }
            });
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (!_isVisible) return;
            var pressedNow = mouseState.GetPressedMouseBtn() == MouseBtn.Left;

            CheckPressedBtn(mouseState.Position, _minusBtns, _minusWasPressed, _availableMinusBtns, 
                pressedNow, -1);
            CheckPressedBtn(mouseState.Position, _plusBtns, _plusWasPressed, _availablePlusBtns, 
                pressedNow, 1);

            _lastMouseBtn = mouseState.GetPressedMouseBtn();
        }

        private void CheckPressedBtn(Point mousePos, (Rectangle, ResourceType)[] btns, bool[] btnsWasPressed, 
            bool[] availableBtns, bool pressedNow, int diff)
        {
            for (var i = 0; i < btns.Length; i++)
            {
                if (!availableBtns[i]) continue;

                var contains = btns[i].Item1.Contains(mousePos);
                var wasPressed = btnsWasPressed[i];

                if (wasPressed)
                {
                    if (contains)
                    {
                        if (pressedNow) continue;

                        btnsWasPressed[i] = false;
                        GameModel.InputActions.Enqueue(new InputAction
                        {
                            ActionType = InputActionType.ExecCommand,
                            InputUnitInfo = new InputUnitInfo(_unitInfo.ID),
                            InputCellInfo = new InputCellInfo(_mapCellInfo.X, _mapCellInfo.Y),
                            InputCommandInfo = new InputCommandInfo(CommandType.ChangePeopleAtWork)
                            {
                                PeopleDiff = (btns[i].Item2, diff)
                            }
                        });
                    }
                    else
                        btnsWasPressed[i] = false;
                }
                else
                    btnsWasPressed[i] = contains && pressedNow && _lastMouseBtn == MouseBtn.None;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible) return;
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(_backgroundTexture, _infoBackgroundRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(_backgroundTexture, _prodBackgroundRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.DrawString(_font, InfoStr, _infoStrPos, 
                Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(_font, ProductionStr, _prodStrPos, 
                Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);

            foreach (var infoRes in _infoRes)
            {
                spriteBatch.Draw(infoRes.Item1, infoRes.Item2, null,
                    Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
                spriteBatch.DrawString(_font, infoRes.Item3, infoRes.Item4, 
                    Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);
            }
            
            foreach (var prodRes in _prodRes)
            {
                spriteBatch.Draw(prodRes.Item1, prodRes.Item2, null,
                    Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
                spriteBatch.DrawString(_font, prodRes.Item3, prodRes.Item4, 
                    Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);
            }
            
            spriteBatch.Draw(_peopleAtWork.Item1, _peopleAtWork.Item2, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.DrawString(_font, _peopleAtWork.Item3, _peopleAtWork.Item4, 
                Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);

            DrawBtns(spriteBatch, _minusIconTexture, _minusBtns, _availableMinusBtns);
            DrawBtns(spriteBatch, _plusIconTexture, _plusBtns, _availablePlusBtns);
            
            spriteBatch.End();
        }

        private void DrawBtns(SpriteBatch spriteBatch, Texture2D texture, 
            (Rectangle, ResourceType)[] btnsPos, bool[] availableBtns)
        {
            for (var i = 0; i < btnsPos.Length; i++)
            {
                var btnPos = btnsPos[i].Item1;
                var available = availableBtns[i];
                
                spriteBatch.Draw(texture, btnPos, null,
                    Color.White * (available ? 1f : 0.65f), 0f, 
                    Vector2.Zero, SpriteEffects.None, 1);
            }
        }
    }
}