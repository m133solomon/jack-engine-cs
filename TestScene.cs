using Jack;
using Jack.Graphics;
using Jack.Core;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using System;
using Jack.Core.Nodes;

namespace Jack
{
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
                    rand.Next(width / 2 - 500, width / 2 + 500),
                    rand.Next(height / 2 - 500, height / 2 + 500)
                );
                Vector2 size = new Vector2(rand.Next(40, 150));
                Color color = Color.FromArgb(
                rand.Next(0, 255),
                rand.Next(0, 255),
                rand.Next(0, 255)
                );
                int dir = rand.Next(100) < 50 ? -1 : 1;
                Root.AddChild(new QuadNode(pos, size, color, dir, 1.0f));
            }

            Root.AddChild(new PlayerNode());
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            App.SpriteBatch.Begin(Camera);
            spriteBatch.FillQuad(new Vector2(JackApp.WindowWidth / 2, JackApp.WindowHeight), new Vector2(500, 300), 0, Color.Black);

            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight - 50), new Vector2(1), Color.White, _font);
            spriteBatch.DrawString("hello world", new Vector2(JackApp.WindowWidth / 2 - 200, JackApp.WindowHeight + 50), new Vector2(2.0f, 1.5f), Color.White, _font);

            spriteBatch.Draw(_font.FontTexture, new Vector2(JackApp.WindowWidth * 1.2f, JackApp.WindowHeight * 0.7f), new Vector2(_font.FontTexture.Width, _font.FontTexture.Height), 0, Color.White);

            App.SpriteBatch.End();
        }

        public class PlayerNode : SpriteNode
        {
            private Color _color = Color.DeepPink;
            private float _speed = 500.0f;

            public PlayerNode() : base()
            {
                Transform.Position = new Vector2(JackApp.WindowHeight / 2, JackApp.WindowHeight / 2);
                Transform.Scale = new Vector2(50.0f);
            }

            public override void Update(float deltaTime)
            {
                base.Update(deltaTime);

                if (Input.IsDown(Key.W))
                {
                    Transform.Position += new Vector2(0, -_speed * deltaTime);
                }
                if (Input.IsDown(Key.S))
                {
                    Transform.Position += new Vector2(0, _speed * deltaTime);
                }
                if (Input.IsDown(Key.A))
                {
                    Transform.Position += new Vector2(-_speed * deltaTime, 0);
                }
                if (Input.IsDown(Key.D))
                {
                    Transform.Position += new Vector2(_speed * deltaTime, 0);
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);
                spriteBatch.FillQuad(GlobalTransform.Position, GlobalTransform.Scale, 0, _color);
            }
        }

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

    }
}