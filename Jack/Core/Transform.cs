using OpenTK;

namespace Jack.Core
{
    public class Transform
    {
        public Node Node { get; set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
            }
        }

        private Vector2 _scale;
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
            }
        }

        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
            }
        }

        public override string ToString()
        {
            return Position.ToString() + " " + Scale.ToString() + " " + Rotation.ToString();
        }
    }
}