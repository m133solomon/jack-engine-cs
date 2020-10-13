using OpenTK;
using System.Drawing;

namespace Jack.Graphics
{
    public class Camera
    {
        private bool _hasChanged = false;

        private Matrix4 _projectionMatrix;
        private Matrix4 _viewMatrix = Matrix4.Identity;

        public Matrix4 ViewProjMatrix
        {
            get
            {
                if (_hasChanged)
                {
                    UpdateViewMatrix();
                }
                // i still don't know why this is working
                // and _pjM * vM not
                return _viewMatrix * _projectionMatrix;
            }
        }

        // camera origin is the center of the screen
        private Vector2 _position = Vector2.Zero;
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                {
                    return;
                }
                _hasChanged = true;
                _position = value;
            }
        }

        private Vector2 _scale = Vector2.One;
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                {
                    return;
                }
                _hasChanged = true;
                _scale = value;
            }
        }

        public float _rotation = 0;
        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value)
                {
                    return;
                }
                _rotation = value;
                _hasChanged = true;
            }
        }

        private Vector2 _origin = Vector2.Zero;
        public Vector2 Origin
        {
            get => _origin;
            set
            {
                if (_origin == value)
                {
                    return;
                }
                _origin = value;
                _hasChanged = true;
            }
        }

        private int _width;
        private int _height;
        public Rectangle Bounds =>
            new Rectangle((int)(_position.X - _width / 2), (int)(_position.Y - _height / 2), _width, _height);

        // todo: different resize modes: strecth aspect ratios etc
        public Camera(int width, int height)
        {
            _width = width;
            _height = height;

            UpdateProjection(_width, _height);
            UpdateViewMatrix();

            JackApp.OnWindowResize += OnResize;
        }

        private void UpdateProjection(int width, int height)
        {
            _width = width;
            _height = height;
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(-_width / 2, _width / 2, _height / 2, -_height / 2, -1.0f, 1.0f);
        }

        private void OnResize()
        {
            UpdateProjection(JackApp.WindowWidth, JackApp.WindowHeight);
        }

        private void UpdateViewMatrix()
        {
            // make it zoom in the center
            Vector3 translation = new Vector3(-Position - new Vector2(_width / 2, _height / 2));

            Matrix4 translationMatrix = Matrix4.CreateTranslation(translation);
            Matrix4 originMatrix = Matrix4.CreateTranslation(new Vector3(_origin));
            Matrix4 scaleMatrix = Matrix4.CreateScale(new Vector3(_scale.X, _scale.Y, 1.0f));
            Matrix4 rotationMatrix = Matrix4.CreateRotationZ(_rotation);

            _viewMatrix = translationMatrix * originMatrix * rotationMatrix * scaleMatrix;
            _hasChanged = false;
        }
    }
}