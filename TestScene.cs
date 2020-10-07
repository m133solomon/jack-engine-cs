using Jack;
using Jack.Graphics;
using Jack.Core;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace Jack
{
    public class QuadNode : Node, IUpdateable, IDrawable
    {
        private float _dir;
        private float _rotation = 0;
        private float _speed;
        private SpriteBatch _sb;
        private Vector2 _position;
        private Vector2 _size;
        private Color _color;

        public QuadNode(Vector2 position, Vector2 size, Color color, int dir, float speed, SpriteBatch sb)
        {
            _position = position;
            _size = size;
            _color = color;
            _dir = dir;
            _speed = speed;
            _sb = sb;
        }

        public void Update(float deltaTime)
        {
            _rotation += _dir * _speed * deltaTime;
        }

        public void Draw()
        {
            _sb.FillQuad(_position, _size, _rotation, _color);
        }
    }

    public class TestScene : Scene
    {
        Camera _camera;

        public TestScene(JackApp app) : base(app)
        {
            _camera = new Camera(app, app.WindowSize.Width, app.WindowSize.Height);

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2 - 300, App.WindowSize.Height / 2),
                new Vector2(120), Color.Cyan, 1, 1.0f, App.SpriteBatch
            )
            { Name = "quad_1" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2 + 300, App.WindowSize.Height / 2),
                new Vector2(120), Color.DeepPink, -1, 1.0f, App.SpriteBatch
            )
            { Name = "quad_2" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2),
                new Vector2(200), Color.BlueViolet, -1, 1.0f, App.SpriteBatch
            )
            { Name = "quad_3" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2),
                new Vector2(40), Color.White, 1, 1.0f, App.SpriteBatch
            )
            { Name = "quad_4" });
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            MoveCamera();

            foreach (Node node in Root.Children)
            {
                if (node is IUpdateable updateable)
                {
                    updateable.Update(deltaTime);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            App.Clear(Color.FromArgb(20, 20, 20));

            App.SpriteBatch.Begin(_camera);
            foreach (Node node in Root.Children)
            {
                if (node is IDrawable drawable)
                {
                    drawable.Draw();
                }
            }
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