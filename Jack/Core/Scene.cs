using System.Collections.Generic;
using Jack.Graphics;

namespace Jack.Core
{
    public class Scene
    {
        public Node Root { get; private set; }

        public JackApp App { get; set; }

        public string Name { get; set; } = "New Scene";

        public Scene(JackApp app)
        {
            App = app;
            Root = new Node("Scene Root");
            // the root must have a children list
            Root.Children = new List<Node>();
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}