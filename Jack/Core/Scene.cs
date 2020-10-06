using System.Collections.Generic;

namespace Jack.Core
{
    public class Scene
    {
        public List<Node> Children { get; }

        public Scene()
        {
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

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }

        protected virtual void Load() { }

        protected virtual void Update(float deltaTime)
        {

        }

        protected virtual void Draw()
        {

        }
    }
}