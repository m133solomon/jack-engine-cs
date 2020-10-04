using OpenTK;
using Jack.Graphics;
using System.Drawing;

namespace Jack
{
    public class Camera
    {
        public Matrix4 ProjectionMatrix { get; set; }
        public Matrix4 ViewMatrix { get; set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                TranslateMatrix(value - _position);
                _position = value;
            }
        }

        private Vector2 _scale;
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                ScaleMatrix(new Vector2(_scale.X / value.X, _scale.Y / value.Y));
                _scale = value;
            }
        }

        private Size _size;
        public Rectangle Bounds =>
            new Rectangle((int)(_position.X - _size.Width / 2), (int)(_position.Y - _size.Height / 2), _size.Width, _size.Height);

        // todo: different resize modes: strecth aspect ratios etc
        private JackApp _app;
        public Camera(JackApp app, int width, int height)
        {
            _size = new Size(width, height);
            _position = Vector2.Zero;
            _scale = Vector2.One;

            ViewMatrix = Matrix4.Identity;
            UpdateProjectionMatrix(_size.Width, _size.Height);

            _app = app;
            // _app.OnWindowResize += OnResize;
        }

        private void UpdateProjectionMatrix(int width, int height)
        {
            // origin in center, y is up
            ProjectionMatrix = Matrix4.CreateOrthographic(width, height, -1.0f, 1.0f);

            // origin in top left, y is down
            // ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1.0f, 1.0f);
        }

        private void OnResize()
        {
            _size = _app.WindowSize;
            UpdateProjectionMatrix(_size.Width, _size.Height);
        }

        public void TranslateMatrix(Vector2 amount)
        {
            _position += amount;
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(amount.X, amount.Y, 0.0f));
            ViewMatrix *= translation;
        }

        public void ScaleMatrix(Vector2 amount)
        {
            _scale *= amount;
            Matrix4 scale = Matrix4.CreateScale(new Vector3(amount.X, amount.Y, 0.0f));
            ViewMatrix *= scale;
        }
        public void ScaleMatrix(float amount)
        {
            ScaleMatrix(new Vector2(amount)); ;
        }
    }
}