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
        private float _dir;
        private float _speed;
        private Color _color;
        public Color Color => _color;

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
        SpriteFont _font;

        public TestScene(JackApp app) : base(app)
        {
            int width = JackApp.WindowWidth;
            int height = JackApp.WindowHeight;

            _camera = new Camera(JackApp.WindowWidth, JackApp.WindowHeight);

            _font = new SpriteFont("Menlo", 37);

            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = new Vector2(
                    rand.Next(width / 2 - 300, width / 2 + 300),
                    rand.Next(height / 2 - 300, height / 2 + 300)
                );
                Vector2 size = new Vector2(rand.Next(40, 200));
                Color color = Color.FromArgb(
                rand.Next(0, 255),
                rand.Next(0, 255),
                rand.Next(0, 255)
                );
                int dir = rand.Next(100) < 50 ? -1 : 1;
                Root.AddChild(new QuadNode(pos, size, color, dir, 1.0f));
            }
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

            spriteBatch.FillQuad(new Vector2(JackApp.WindowWidth / 2, JackApp.WindowHeight / 2), new Vector2(500, 300), 0, Color.Black);

            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight / 2 - 50), new Vector2(2), Color.White, _font);
            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight / 2 + 50), new Vector2(2.0f, 1.5f), Color.White, _font);

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