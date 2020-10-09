using System.Drawing;
using OpenTK;

namespace Jack
{
    public static class Extensions
    {
        public static bool Includes(this Rectangle rectangle, Vector2 position)
        {
            return (
                position.X > rectangle.Left && position.Y > rectangle.Top &&
                position.X < rectangle.Right && position.Y < rectangle.Bottom
            );
        }
    }
}