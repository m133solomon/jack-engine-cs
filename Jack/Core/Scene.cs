using System.Collections.Generic;
using Jack.Graphics;

namespace Jack.Core
{
    public class Scene
    {
        public Camera Camera { get; set; }
        public List<Node> Children { get; }

        public JackApp App { get; }

        public Scene(JackApp app)
        {
            App = app;
            Children = new List<Node>();
        }

        public void AddChild(Node node)
        {
            Children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (node.Id == Children[i].Id)
                {
                    Children.RemoveAt(i);
                    break;
                }
            }
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Draw() { }
    }
}