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
        static void Main(string[] args)
        {
            using (var app = new Application())
            {
                app.Run();
            }
        }

        public class Application : JackApp
        {
            public static SpriteBatch SpriteBatch { get; private set; }

            public Application() : base()
            {
                WindowTitle = "Jack Sandbox";
                ClearColor = Color.Black;
                WindowVsync = VSyncMode.On;
            }

            Texture _texture;
            Texture _texture2;
            RenderTarget _renderTareget;

            protected override void Load()
            {
                SpriteBatch = new SpriteBatch(this);
                _texture = new Texture("./avatar.png");
                _texture2 = new Texture("./glazing_1.png");

                _renderTareget = new RenderTarget(3000, 3000);

                _renderTareget.Bind();
                SpriteBatch.Begin();

                int squareWidth = 3000;

                int quadCount = 0;
                for (int i = -squareWidth / 2; i < squareWidth / 2; i += 30)
                {
                    for (int j = -squareWidth / 2; j < squareWidth / 2; j += 30)
                    {
                        Texture t = i < j ? _texture : _texture2;
                        SpriteBatch.DrawQuad(new Vector2(i, j), new Vector2(25, 25), 0, t, Color.White);
                        quadCount++;
                    }
                }

                SpriteBatch.End();
                _renderTareget.Unbind();
            }
            
            private void MoveCamera()
            {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Key.W))
                {
                    SpriteBatch.TranslateViewMatrix(new Vector2(0, -0.01f));
                }
                if (state.IsKeyDown(Key.S))
                {
                    SpriteBatch.TranslateViewMatrix(new Vector2(0, 0.01f));
                }
                if (state.IsKeyDown(Key.A))
                {
                    SpriteBatch.TranslateViewMatrix(new Vector2(0.01f, 0));
                }
                if (state.IsKeyDown(Key.D))
                {
                    SpriteBatch.TranslateViewMatrix(new Vector2(-0.01f, 0));
                }

                if (state.IsKeyDown(Key.Q))
                {
                    SpriteBatch.ScaleViewMatrix(new Vector2(1.01f));
                }
                else if (state.IsKeyDown(Key.E))
                {
                    SpriteBatch.ScaleViewMatrix(new Vector2(0.99f));
                }
            }

            protected override void Draw(float deltaTime)
            {
                MoveCamera();
                
                SpriteBatch.Begin();
                SpriteBatch.DrawQuad(Vector2.Zero, new Vector2(300, 300), 0, _renderTareget.Texture, Color.White);
                SpriteBatch.DrawQuad(new Vector2(-400, 100), new Vector2(50, 50), 0, Color.WhiteSmoke);
                SpriteBatch.End();
                
                WindowTitle = "FPS: " + Math.Ceiling(1 / deltaTime);
            }

            protected override void Exit()
            {
            }
        }
    }
}
