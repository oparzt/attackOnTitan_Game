using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class Camera2D
    {
        public Matrix Transform;
        public Vector3 InitMouseDragPos;
        public Vector3 InitDragPos;
        public Vector3 Pos;

        public float RightBorder => -(MapWidthWithZoom - _viewportWidth);
        public float BottomBorder => -(MapHeightWithZoom - _viewportHeight);

        private bool _isDrag;
        private float _zoom = 0.7f;
        private const float ZoomSpeed = 0.00005f;
        private int _lastScroll;

        public float MapWidthWithZoom => _mapWidth * _zoom;
        public float MapHeightWithZoom => _mapHeight * _zoom;

        private readonly float _mapWidth;
        private readonly float _mapHeight;
        
        private readonly float _viewportWidth;
        private readonly float _viewportHeight;

        public Point MousePoint;

        public float Zoom
        {
            get => _zoom;
            set => _zoom = value >= 1f ? 1f : value <= 0.6f ? 0.6f : value;
        }

        public bool MatrixWasUpdated;
        
        public Camera2D(int startPosX, int startPosY, 
            int viewportWidth, int viewportHeight,
            int mapWidth, int mapHeight)
        {
            Pos = new Vector3(startPosX, startPosY, 0);
            
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _lastScroll = Mouse.GetState().ScrollWheelValue;

            MatrixWasUpdated = true;
            UpdateTransformMatrix();
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var curMousePos = new Vector3(mouseState.X, mouseState.Y, 0);

            if (mouseState.ScrollWheelValue != _lastScroll) 
                UpdateZoom(mouseState.ScrollWheelValue);
            if (_isDrag) UpdateMove(curMousePos);

            if (mouseState.LeftButton == ButtonState.Released)
                _isDrag = false;
            else if (!_isDrag)
                StartMove(curMousePos);
            
            MousePoint = new Point((int)((mouseState.X - Pos.X) / _zoom), 
                (int)((mouseState.Y - Pos.Y) / _zoom));
        }

        private void StartMove(Vector3 mousePos)
        {
            _isDrag = true;
            InitDragPos = Pos;
            InitMouseDragPos = mousePos;
        }

        private void UpdateZoom(int scrollValue)
        {
            var zoomDiff = (scrollValue - _lastScroll) * ZoomSpeed;
            _lastScroll = scrollValue;
            Zoom += zoomDiff;
            
            if (Pos.Y < BottomBorder) Pos.Y = BottomBorder;
            if (Pos.X < RightBorder) Pos.X = RightBorder;

            MatrixWasUpdated = true;
            UpdateTransformMatrix();
        }

        private void UpdateMove(Vector3 mousePos)
        {
            var prePos = mousePos - InitMouseDragPos + InitDragPos;
            
            if ((prePos - Pos).Length() > 3)
                MatrixWasUpdated = true;
            
            Pos = prePos;

            if (Pos.Y < BottomBorder) Pos.Y = BottomBorder;
            if (Pos.X < RightBorder) Pos.X = RightBorder;
            if (Pos.Y > 0) Pos.Y = 0;
            if (Pos.X > 0) Pos.X = 0;

            UpdateTransformMatrix();
        }

        private void UpdateTransformMatrix()
        {
            Transform = Matrix.Identity *
                        Matrix.CreateScale(Zoom, Zoom, 0) *
                        Matrix.CreateTranslation(Pos);
        }
    }
}
