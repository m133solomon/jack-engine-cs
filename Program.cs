using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

using Jack;
using Jack.Graphics;
using Jack.Core;

namespace CS_Jack
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var app = new Application())
            {
                app.Run();
            }
        }

        public class Application : JackApp
        {
            public Application() : base()
            {
                WindowTitle = "Jack Sandbox";
                WindowVsync = VSyncMode.On;
            }

            private SpriteBatch _spriteBatch;
            private Camera _camera;
            private Camera _uiCamera;

            Texture _texture;
            Texture _texture2;

            SpriteFont _font;

            protected override void Load()
            {
                _spriteBatch = new SpriteBatch(this);
                _camera = new Camera(this, WindowSize.Width, WindowSize.Height);
                _uiCamera = new Camera(this, WindowSize.Width, WindowSize.Height);
                _texture = new Texture("res/avatar.png");
                _texture2 = new Texture("res/glazing_1.png");

                _font = new SpriteFont("Courier", 37);
                _font.CharSpacing = (int)(_font.FontSize * 0.25f);
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

            private float _deltaTime = 0;
            protected override void Update(float deltaTime)
            {
                _deltaTime = deltaTime;
                MoveCamera();
            }

            // NOTE: start working on render target thingy

            protected override void Draw()
            {
                Clear(Color.FromArgb(20, 20, 20));

                int squareWidth = 1000;
                int step = 50;
                int quadCount = 0;

                _spriteBatch.Begin(_camera);

                for (int i = 0; i < squareWidth; i += step)
                {
                    for (int j = 0; j < squareWidth; j += step)
                    {
                        int r = (int)(((float)i / (float)squareWidth) * 255);
                        int g = 255 - (int)(((float)j / (float)squareWidth) * 255);
                        int b = 255;
                        Color color = Color.FromArgb(r, g, b);
                        _spriteBatch.FillQuad(new Vector2(i, j), new Vector2(step, step), 0, color);
                        quadCount++;
                    }
                }

                _spriteBatch.DrawLine(new Vector2(300, 300), new Vector2(700, 700), 10, Color.Purple);

                _spriteBatch.End();

                _spriteBatch.Begin(_uiCamera);
                _spriteBatch.DrawString("FPS: " + MathF.Ceiling(1 / _deltaTime), new Vector2(50, 50), new Vector2(2), Color.White, _font);
                _spriteBatch.DrawString("Quad Count: " + quadCount, new Vector2(50, 100), new Vector2(2), Color.White, _font);
                _spriteBatch.End();
            }

            protected override void Exit()
            {
            }
        }
    }
}
