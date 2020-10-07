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

            Root.AddChild(new Node("test_1"));
            Root.Children[0].AddChild(new Node("test_1_child_1"));
            Root.Children[0].AddChild(new Node("test_1_child_2"));
            Root.Children[0].Children[1].AddChild(new Node("test_1_child_2_child_1"));

            Root.AddChild(new Node("test_2"));

            Root.AddChild(new Node("test_3"));
            Root.Children[2].AddChild(new Node("test_3_child_1"));
        }

        private float _quadRot = 0;
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _quadRot += 0.5f * deltaTime;
            MoveCamera();
        }

        public override void Draw()
        {
            base.Draw();

            App.Clear(Color.FromArgb(20, 20, 20));

            App.SpriteBatch.Begin(_camera);
            App.SpriteBatch.FillQuad(new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2), new Vector2(250), _quadRot, Color.MediumPurple);
            App.SpriteBatch.FillQuad(new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2), new Vector2(50), -_quadRot, Color.LightPink);
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

            if (state.IsKeyDown(Key.H))
            {
                _camera.Rotation += 0.1f;
            }
            else if (state.IsKeyDown(Key.G))
            {
                _camera.Rotation -= 0.1f;
            }
        }
    }
}