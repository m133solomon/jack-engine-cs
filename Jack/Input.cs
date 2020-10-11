using OpenTK.Input;
using OpenTK;

namespace Jack
{
    public static class Input
    {
        // todo: also add keys
        public delegate void MouseButtonEvent(MouseButtonEventArgs e);
        public static event MouseButtonEvent OnMouseDown;
        public static event MouseButtonEvent OnMouseUp;

        public delegate void MouseWheelEvent(MouseWheelEventArgs e);
        public static event MouseWheelEvent OnMouseWheel;

        public static void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnMouseDown != null)
            {
                OnMouseDown(e);
            }
        }

        public static void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (OnMouseUp != null)
            {
                OnMouseUp(e);
            }
        }

        public static void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (OnMouseWheel != null)
            {
                OnMouseWheel(e);
            }
        }

        // maybe: find a better value for the start position
        public static Vector2 _mousePosition = new Vector2(-100, -100);
        public static Vector2 MousePosition => _mousePosition;

        public static void MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;
        }
    }
}