using OpenTK;
using Jack.Graphics;
using System.Drawing;
using System;

namespace Jack.Graphics
{
    public class Camera
    {
        public Matrix4 ProjectionMatrix { get; set; }
        public Matrix4 ViewMatrix { get; set; }

        // note: i think this is broken
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

        // todo: fix this shit
        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set
            {
                RotateMatrix(value - _rotation);
                _rotation = value;
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
            _position = Vector2.Zero;
            _scale = Vector2.One;

            ViewMatrix = Matrix4.Identity;
            UpdateProjectionMatrix(_width, _height);

            JackApp.OnWindowResize += OnResize;
        }

        private void UpdateProjectionMatrix(int width, int height)
        {
            // origin in top left, y is down
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1.0f, 1.0f);
        }

        private void OnResize()
        {
            _width = JackApp.WindowWidth;
            _height = JackApp.WindowHeight;
            UpdateProjectionMatrix(_width, _height);
        }

        private void TranslateMatrix(Vector2 amount)
        {
            _position += amount;
            // note: find out why I need that minus
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(-amount.X, amount.Y, 0.0f));
            ViewMatrix *= translation;
        }

        private void ScaleMatrix(Vector2 amount)
        {
            _scale *= amount;
            Matrix4 scale = Matrix4.CreateScale(new Vector3(amount.X, amount.Y, 0.0f));
            ViewMatrix *= scale;
        }

        private void ScaleMatrix(float amount)
        {
            ScaleMatrix(new Vector2(amount));
        }

        // todo: methdod to project point to world and back

        private void RotateMatrix(float amount)
        {
            _rotation += amount;
            Matrix4 rotation = Matrix4.CreateRotationZ(amount);
            ViewMatrix *= rotation;
        }
    }
}