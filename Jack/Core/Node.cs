using System;
using System.Collections.Generic;

namespace Jack.Core
{
    public class Node
    {
        private static int _currentId = 0;
        public static int NodeCount => _currentId;

        public int Id { get; }
        // note: try and use this if the performance is bad
        public string Name { get; set; } = "NewNode";
        public Transform GlobalTransform { get; set; }
        public Transform LocalTransform { get; set; }

        public List<Node> Children { get; }

        public Node()
        {
            Id = ++_currentId;

            Children = new List<Node>();
        }
    }
}