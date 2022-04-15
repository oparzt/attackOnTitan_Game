using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.GameComponents
{
    public class Camera2D : IComponent
    {
        public Matrix Transform;
        public Vector3 InitMouseDragPos;
        public Vector3 InitDragPos;
        public Vector3 Pos;
        public Vector3 Center;

        public float RightBorder;
        public float BottomBorder;

        public bool IsDrag = false;

        public Camera2D(int startPosX = 0, int startPosY = 0, float rightBorder = 0, float bottomBorder = 0)
        {
            Pos = new Vector3(startPosX, startPosY, 0);
            Center = new Vector3(SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width * 0.5f,
                SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height * 0.5f, 0);

            RightBorder = -rightBorder;
            BottomBorder = -bottomBorder;

            UpdateTransformMatrix();
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var curMousePos = new Vector3(mouseState.X, mouseState.Y, 0);

            if (IsDrag) UpdateMove(curMousePos);

            if (mouseState.LeftButton == ButtonState.Released)
                IsDrag = false;
            else if (!IsDrag)
                StartMove(curMousePos);
        }

        public void StartMove(Vector3 mousePos)
        {
            IsDrag = true;
            InitDragPos = Pos;
            InitMouseDragPos = mousePos;
        }

        public void UpdateMove(Vector3 mousePos)
        {
            Pos = mousePos - InitMouseDragPos + InitDragPos;

            if (Pos.Y < BottomBorder) Pos.Y = BottomBorder;
            if (Pos.X < RightBorder) Pos.X = RightBorder;
            if (Pos.Y > 0) Pos.Y = 0;
            if (Pos.X > 0) Pos.X = 0;

            UpdateTransformMatrix();
        }

        public void UpdateTransformMatrix()
        {
            Transform = Matrix.Identity * Matrix.CreateTranslation(Pos);
        }

        public void Draw(SpriteBatch spriteBatch) {}

        public bool IsComponentOnPosition(Point point)
        {
            throw new NotImplementedException();
        }
    }
}
