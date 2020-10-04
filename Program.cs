using System;
using System.Drawing;
using Jack;
using Jack.Graphics;
using OpenTK;
using OpenTK.Input;

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
                ClearColor = Color.FromArgb(30, 30, 30);
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

                _font = new SpriteFont("Arial", 37);
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

            protected override void Draw()
            {
                _spriteBatch.Begin(_camera);

                int squareWidth = 3700;
                int step = 30;
                int quadCount = 0;

                for (int i = -squareWidth / 2; i < squareWidth / 2; i += step)
                {
                    for (int j = -squareWidth / 2; j < squareWidth / 2; j += step)
                    {
                        float pt = (float)Math.Sin(DateTime.Now.Millisecond / 1000.0f * Math.PI);

                        float px = (float)((i + squareWidth / 2.0f) / squareWidth);
                        float py = (float)((j + squareWidth / 2.0f) / squareWidth);

                        int r = (int)((pt / 2 + px / 2) * 255);
                        int g = 255 - (int)((pt / 2 + px / 2) * 255);
                        int b = 255 - (int)((pt / 2 + px / 2) * 255);

                        Color color = Color.FromArgb(r, g, b);

                        _spriteBatch.DrawQuad(new Vector2(i, j), new Vector2(step - 3, step - 3), 0, color);
                        quadCount++;
                    }
                }

                _spriteBatch.End();

                _spriteBatch.Begin(_uiCamera);
                _spriteBatch.DrawString("FPS: " + MathF.Ceiling(1 / _deltaTime), new Vector2(-WindowSize.Width / 2 + 50, WindowSize.Height / 2 - 50), new Vector2(2), Color.White, _font);
                _spriteBatch.End();
            }

            protected override void Exit()
            {
                Console.WriteLine("hello");
                Environment.Exit(0);
            }
        }
    }
}
