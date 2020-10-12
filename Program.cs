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

        public class Application : Jack.JackApp
        {
            public Application() : base()
            {
                WindowTitle = "Jack Sandbox";
                WindowWidth = 1000;
                WindowHeight = 700;
                WindowVsync = VSyncMode.On;
            }

            Scene _testScene;

            protected override void Load()
            {
                _testScene = new TestScene(this);
                CurrentScene = _testScene;

                DebugLayer.Init(this);

                Input.OnKeyDown += delegate (Key key)
                {
                    if (key == Key.F1)
                    {
                        DebugLayer.Active = !DebugLayer.Active;
                    }
                };
            }

            private float _deltaTime = 0;
            protected override void Update(float deltaTime)
            {
                _deltaTime = deltaTime;
                DebugLayer.Update(deltaTime);
            }

            protected override void Draw()
            {
                DebugLayer.Draw(SpriteBatch);
            }

            protected override void Exit()
            {
            }
        }
    }
}
