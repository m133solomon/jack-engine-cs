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
        SpriteFont _font;

        public TestScene(JackApp app) : base(app)
        {
            int width = JackApp.WindowWidth;
            int height = JackApp.WindowHeight;

            _font = new SpriteFont("Terminus (TTF)", 37);

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

            spriteBatch.Begin(Camera);
            foreach (Node node in Root.Children)
            {
                if (node is IDrawable drawable)
                {
                    drawable.Draw(spriteBatch);
                }
            }

            spriteBatch.FillQuad(new Vector2(JackApp.WindowWidth / 2, JackApp.WindowHeight), new Vector2(500, 300), 0, Color.Black);

            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight - 50), new Vector2(1), Color.White, _font);
            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight + 50), new Vector2(2.0f, 1.5f), Color.White, _font);

            spriteBatch.Draw(_font.FontTexture, new Vector2(JackApp.WindowWidth * 1.2f, JackApp.WindowHeight * 0.7f), new Vector2(_font.FontTexture.Width, _font.FontTexture.Height), 0, Color.White);

            App.SpriteBatch.End();
        }

        private void MoveCamera()
        {
            if (Input.IsDown(Key.W))
            {
                Camera.Position += new Vector2(0, -0.01f);
            }
            if (Input.IsDown(Key.S))
            {
                Camera.Position += new Vector2(0, 0.01f);
            }
            if (Input.IsDown(Key.A))
            {
                Camera.Position += new Vector2(-0.01f, 0);
            }
            if (Input.IsDown(Key.D))
            {
                Camera.Position += new Vector2(0.01f, 0);
            }

            if (Input.IsDown(Key.Q))
            {
                Camera.Scale *= 1.01f;
            }
            else if (Input.IsDown(Key.E))
            {
                Camera.Scale *= 0.99f;
            }
        }
    }
}