using Jack;
using Jack.Graphics;
using Jack.Core;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using System;

namespace Jack
{
    public class QuadNode : Node, IUpdateable, IDrawable
    {
        // todo: make spritebatch / app global
        private float _dir;
        private float _speed;
        private Color _color;

        public QuadNode(Vector2 position, Vector2 size, Color color, int dir, float speed) : base()
        {
            Transform.Position = position;
            Transform.Scale = size;
            _color = color;
            _dir = dir;
            _speed = speed;
        }

        public void Update(float deltaTime)
        {
            Transform.Rotation += _dir * _speed * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillQuad(GlobalTransform.Position, GlobalTransform.Scale, GlobalTransform.Rotation, _color);
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
                new Vector2(120), Color.Cyan, 1, 1.0f)
            { Name = "quad_1" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2 + 300, App.WindowSize.Height / 2),
                new Vector2(120), Color.DeepPink, -1, 1.0f
            )
            { Name = "quad_2" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2),
                new Vector2(200), Color.BlueViolet, -1, 1.0f
            )
            { Name = "quad_3" });

            Root.AddChild(new QuadNode(
                new Vector2(App.WindowSize.Width / 2, App.WindowSize.Height / 2),
                new Vector2(40), Color.White, 1, 1.0f
            )
            { Name = "quad_4" });

            // var props = Root.Children[0].GetType().GetProperties();
            // foreach (var prop in props)
            // {
            // Console.WriteLine("{0}-{1}", prop.Name, prop.GetValue(Root.Children[0]));
            // }
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            App.Clear(Color.FromArgb(20, 20, 20));

            spriteBatch.Begin(_camera);
            foreach (Node node in Root.Children)
            {
                if (node is IDrawable drawable)
                {
                    drawable.Draw(spriteBatch);
                }
            }

            App.SpriteBatch.End();
        }

        private void MoveCamera()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.W))
            {
                Root.Transform.Position += new Vector2(0, -5f);
            }
            if (state.IsKeyDown(Key.S))
            {
                Root.Transform.Position += new Vector2(0, 5f);
            }
            if (state.IsKeyDown(Key.A))
            {
                Root.Transform.Position += new Vector2(-5f, 0);
            }
            if (state.IsKeyDown(Key.D))
            {
                Root.Transform.Position += new Vector2(5f, 0);
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