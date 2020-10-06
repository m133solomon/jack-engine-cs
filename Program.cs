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

            private Camera _uiCamera;

            SpriteFont _font;

            TestScene _testScene;

            protected override void Load()
            {
                _uiCamera = new Camera(this, WindowSize.Width, WindowSize.Height);

                _font = new SpriteFont("Courier", 37);
                _font.CharSpacing = (int)(_font.FontSize * 0.25f);

                _testScene = new TestScene(this);
            }

            private float _deltaTime = 0;
            protected override void Update(float deltaTime)
            {
                _deltaTime = deltaTime;
                _testScene.Update(deltaTime);
            }

            // NOTE: start working on render target thingy

            protected override void Draw()
            {
                Clear(Color.FromArgb(20, 20, 20));

                _testScene.Draw();

                SpriteBatch.Begin(_uiCamera);
                SpriteBatch.DrawString("FPS: " + MathF.Ceiling(1 / _deltaTime), new Vector2(50, 50), new Vector2(2), Color.White, _font);
                SpriteBatch.End();
            }

            protected override void Exit()
            {
            }
        }
    }
}
