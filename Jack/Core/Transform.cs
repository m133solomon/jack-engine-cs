using OpenTK;

namespace Jack.Core
{
    public class Transform
    {
        public Vector2 Translation { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0;
    }
}