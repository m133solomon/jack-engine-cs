using Jack;
using Jack.Graphics;
using Jack.Core;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace Jack
{
    public class TestScene : Scene
    {
        Camera _camera;
        public TestScene(JackApp app) : base(app)
        {
            _camera = new Camera(app, app.WindowSize.Width, app.WindowSize.Height);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            MoveCamera();
        }

        public override void Draw()
        {
            base.Draw();

            int squareWidth = 1000;
            int step = 50;
            int quadCount = 0;

            App.SpriteBatch.Begin(_camera);

            for (int i = 0; i < squareWidth; i += step)
            {
                for (int j = 0; j < squareWidth; j += step)
                {
                    int r = (int)(((float)i / (float)squareWidth) * 255);
                    int g = 255 - (int)(((float)j / (float)squareWidth) * 255);
                    int b = 255;
                    Color color = Color.FromArgb(r, g, b);
                    App.SpriteBatch.FillQuad(new Vector2(i, j), new Vector2(step, step), 0, color);
                    quadCount++;
                }
            }

            App.SpriteBatch.DrawLine(new Vector2(300, 300), new Vector2(700, 700), 10, Color.Purple);

            App.SpriteBatch.End();
        }

        private void MoveCamera()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.W))
            {
                _camera.Position += new Vector2(0, -0.01f);
            }
            if (state.IsKeyDown(Key.S))
            {
                _camera.Position += new Vector2(0, 0.01f);
            }
            if (state.IsKeyDown(Key.A))
            {
                _camera.Position += new Vector2(0.01f, 0);
            }
            if (state.IsKeyDown(Key.D))
            {
                _camera.Position += new Vector2(-0.01f, 0);
            }

            if (state.IsKeyDown(Key.Q))
            {
                _camera.Scale *= 1.01f;
            }
            else if (state.IsKeyDown(Key.E))
            {
                _camera.Scale *= 0.99f;
            }
        }
    }
}