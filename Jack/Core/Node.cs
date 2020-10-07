using System;
using System.Collections.Generic;

namespace Jack.Core
{
    public class Node
    {
        public static int NodeCount { get; private set; }

        public int Id { get; }
        public string Name { get; set; }
        public Transform GlobalTransform { get; set; }
        public Transform LocalTransform { get; set; }

        public Node Parent { get; set; }
        public List<Node> Children { get; }

        public Node(string name = "NewNode")
        {
            Name = name;

            Id = ++NodeCount;

            Children = new List<Node>();
        }

        public void AddChild(Node node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (node.Id == Children[i].Id)
                {
                    node.Parent = null;
                    Children.RemoveAt(i);
                }
            }
        }
    }
}