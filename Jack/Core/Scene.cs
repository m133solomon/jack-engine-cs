using System.Collections.Generic;
using Jack.Graphics;
using System.Drawing;

namespace Jack.Core
{
    public class Scene
    {
        public Node Root { get; private set; }
        public JackApp App { get; set; }
        public Camera Camera { get; set; }

        public string Name { get; set; } = "New Scene";

        public Color ClearColor { get; set; } = Color.FromArgb(20, 20, 20);

        public Scene(JackApp app)
        {
            App = app;
            Root = new Node("Scene Root");
            Camera = new Camera(JackApp.WindowWidth, JackApp.WindowHeight);
            // the root must have a children list
            Root.Children = new List<Node>();
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Update(float deltaTime)
        {
            foreach (Node node in Root.Children)
            {
                if (node is IUpdateable updateable)
                {
                    updateable.Update(deltaTime);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            App.Clear(ClearColor);
            spriteBatch.Begin(Camera);
            foreach (Node node in Root.Children)
            {
                if (node is IDrawable drawable)
                {
                    drawable.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
        }
    }
}