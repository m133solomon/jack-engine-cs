using System;
using System.Drawing;
using OpenTK;

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
                WindowSize = new Size(1000, 700);
                WindowVsync = VSyncMode.On;
            }

            private Camera _uiCamera;

            SpriteFont _font;

            Scene _testScene;

            protected override void Load()
            {
                _uiCamera = new Camera(this, WindowSize.Width, WindowSize.Height);

                _font = new SpriteFont("Menlo", 37);

                _testScene = new TestScene(this);
                CurrentScene = _testScene;
            }

            private float _deltaTime = 0;
            protected override void Update(float deltaTime)
            {
                _deltaTime = deltaTime;
            }

            // NOTE: start working on render target thingy

            private int _xStep = 50;
            private int _yStep = 40;

            protected override void Draw()
            {
                SpriteBatch.Begin(_uiCamera);

                SpriteBatch.FillQuad(new Rectangle(0, 0, WindowSize.Width / 4, WindowSize.Height), 0, Color.Black);

                SpriteBatch.DrawString("FPS: " + MathF.Ceiling(1 / _deltaTime), new Vector2(20, 20), new Vector2(2), Color.White, _font);

                int sx = 30;
                int sy = 120;
                DrawNode(_testScene.Root, ref sx, ref sy, 1);

                SpriteBatch.End();
            }

            private void DrawNode(Node node, ref int startX, ref int startY, int index)
            {
                SpriteBatch.DrawString(index + ". " + node.Name, new Vector2(startX, startY), new Vector2(2, 1.8f), Color.White, _font);
                for (int i = 0; i < node.Children.Count; i++)
                {
                    int sx = startX + _xStep;
                    startY += _yStep;
                    DrawNode(node.Children[i], ref sx, ref startY, i + 1);
                }
            }

            protected override void Exit()
            {
            }
        }
    }
}
