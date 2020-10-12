using OpenTK.Input;
using OpenTK;

namespace Jack
{
    public static class Input
    {
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

        public delegate void KeyDownEvent(Key key);
        public static event KeyDownEvent OnKeyDown;
        public static event KeyDownEvent OnKeyUp;

        public static void KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (OnKeyDown != null)
            {
                OnKeyDown(e.Key);
            }
        }

        public static void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (OnKeyUp != null)
            {
                OnKeyUp(e.Key);
            }
        }

        private static KeyboardState _keyboardState;

        public static void Update()
        {
            _keyboardState = Keyboard.GetState();
        }

        public static bool IsDown(Key key)
        {
            return _keyboardState.IsKeyDown(key);
        }
    }
}