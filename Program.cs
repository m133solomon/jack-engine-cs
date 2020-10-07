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

            private int _xStep = 40;
            private int _yStep = 40;

            protected override void Draw()
            {
                SpriteBatch.Begin(_uiCamera);

                int offset = 20;
                Rectangle panelRect = new Rectangle(offset, offset, WindowSize.Width / 4 - offset * 2, WindowSize.Height - offset * 2);
                SpriteBatch.FillQuad(panelRect, 0, Color.Black);
                SpriteBatch.StrokeRect(panelRect, 2, Color.DeepPink);

                SpriteBatch.DrawString("FPS: " + MathF.Ceiling(1 / _deltaTime), new Vector2(WindowSize.Width - 100, 50), new Vector2(2), Color.White, _font);

                int sx = 50;
                int sy = 50;
                DrawNode(_testScene.Root, sx, ref sy);

                SpriteBatch.End();
            }

            // todo: wrap this into a debug layer
            private void DrawNode(Node node, int startX, ref int startY)
            {
                SpriteBatch.DrawString(node.Name, new Vector2(startX, startY), new Vector2(2, 1.5f), Color.White, _font);

                for (int i = 0; i < node.Children.Count; i++)
                {
                    SpriteBatch.DrawLine(
                        new Vector2(startX, startY + _yStep),
                        new Vector2(startX, startY + _yStep / 2),
                        2, Color.DeepPink
                    );

                    int sx = startX + _xStep;
                    startY += _yStep;

                    SpriteBatch.DrawLine(
                        new Vector2(startX, startY),
                        new Vector2(startX + _xStep - 15, startY),
                        2, Color.DeepPink
                    );
                    DrawNode(node.Children[i], sx, ref startY);
                }
            }

            protected override void Exit()
            {
            }
        }
    }
}
