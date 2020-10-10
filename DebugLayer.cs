using Jack;
using Jack.Graphics;
using Jack.Core;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System;

namespace Jack
{
    public static class DebugLayer
    {
        public static bool Active { get; set; } = true;

        private static JackApp _app;
        private static Camera _camera;
        private static SpriteFont _font;

        private static Color _panelColor = Color.FromArgb(200, 0, 0, 0);
        private static Color _accentColor = Color.DeepPink;

        private static Panel _hierarchyPanel;
        private static Panel _propertiesPanel;

        public static void Init(JackApp app)
        {
            _app = app;
            _camera = new Camera(JackApp.WindowWidth, JackApp.WindowHeight);
            _font = new SpriteFont("Courier", 22);

            MakePanels();

            JackApp.OnWindowResize += MakePanels;
        }

        private static void MakePanels()
        {
            int panelMargin = 20;
            int panelWidth = 400;
            _hierarchyPanel = new Panel(
                new Rectangle(
                    panelMargin, panelMargin, panelWidth,
                    JackApp.WindowHeight - panelMargin * 2
                )
            )
            { FillColor = _panelColor, StrokeColor = _accentColor };

            _propertiesPanel = new Panel(
                new Rectangle(
                    JackApp.WindowWidth - panelWidth - panelMargin, panelMargin,
                    panelWidth, JackApp.WindowHeight - panelMargin * 2
                )
            )
            { FillColor = _panelColor, StrokeColor = _accentColor };
        }

        private static float _deltaTime = 0;
        public static void Update(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Begin(_camera);
                DrawNodesPanel(spriteBatch);
                DrawInfo(spriteBatch);

                if (_focusedNode != null)
                {
                    DrawPropertiesPanel(spriteBatch);
                }
                spriteBatch.End();
            }
        }

        private static void DrawInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                "FPS: " + MathF.Ceiling(1.0f / _deltaTime), new Vector2(JackApp.WindowWidth / 2 + 200, 50),
                new Vector2(2.0f), Color.White, _font
            );
            spriteBatch.DrawString(
                "Total Nodes: " + (JackApp.CurrentScene.Root.Children.Count + 1), new Vector2(JackApp.WindowWidth / 2 - 200 - 100, 50),
                new Vector2(2.0f), Color.White, _font
            );
        }

        private static void DrawNodesPanel(SpriteBatch spriteBatch)
        {
            _hierarchyPanel.Draw(spriteBatch);
            int sx = _hierarchyPanel.Rectangle.Left + 20;
            int sy = _hierarchyPanel.Rectangle.Top + 30;
            DrawNode(spriteBatch, JackApp.CurrentScene.Root, sx, ref sy);
        }

        private static int _yStep = 40;
        private static int _xStep = 40;

        // todo: allow me to click on the node
        private static void DrawNode(SpriteBatch spriteBatch, Node node, int startX, ref int startY)
        {
            if (startY > JackApp.WindowHeight)
            {
                // note: maybe add some scroll instead of stopping drawing
                return;
            }
            Rectangle textBounds = _font.GetBounds(node.Name);
            Rectangle rectangle = new Rectangle(
                // note: multiply with scale
                startX - 10, startY - 20, textBounds.Width, textBounds.Height
            );
            bool hovered = rectangle.Includes(JackApp.MousePosition);
            spriteBatch.StrokeRect(rectangle, 1, hovered ? _accentColor : Color.White);

            if (hovered)
            {
                MouseState ms = Mouse.GetState();
                if (ms.IsButtonDown(MouseButton.Left))
                {
                    _focusedNode = node;
                }
            }

            spriteBatch.DrawString(node.Name, new Vector2(startX, startY), new Vector2(2.0f), Color.White, _font);

            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    spriteBatch.DrawLine(
                        new Vector2(startX, startY + _yStep),
                        new Vector2(startX, startY + _yStep / 2),
                        2, Color.DeepPink
                    );

                    int sx = startX + _xStep;
                    startY += _yStep;

                    spriteBatch.DrawLine(
                        new Vector2(startX, startY),
                        new Vector2(startX + _xStep - 15, startY),
                        2, Color.DeepPink
                    );
                    DrawNode(spriteBatch, node.Children[i], sx, ref startY);
                }
            }
        }

        private static Node _focusedNode = null;

        private static void DrawPropertiesPanel(SpriteBatch spriteBatch)
        {
            _propertiesPanel.Draw(spriteBatch);
            spriteBatch.DrawString("Properties: ", new Vector2(
                _propertiesPanel.Rectangle.Left + 20, _propertiesPanel.Rectangle.Top + _yStep
            ), new Vector2(2.0f), Color.White, _font);

            var props = _focusedNode.GetType().GetProperties();
            int sy = 130;
            int sx = _propertiesPanel.Rectangle.Left;
            foreach (var property in props)
            {
                object propValue = property.GetValue(_focusedNode);
                // draw info different based on property type
                if (property.PropertyType == typeof(Transform))
                {
                    string text = "- " + property.Name;
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;

                    Transform transform = (Transform)propValue;
                    text = string.Format("Position: {0}", transform.Position);
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep * 2, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;
                    text = string.Format("Scale: {0}", transform.Scale);
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep * 2, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;
                    text = string.Format("Rotation: {0}", transform.Rotation);
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep * 2, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;
                }

                // todo: find  a better way to check if type is list
                // note: only works for node
                else if (property.PropertyType == typeof(List<Node>) && propValue != null)
                {
                    var list = propValue as List<Node>;
                    string text = string.Format("- {0} Count: {1}", property.Name, list.Count);
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;
                }
                else
                {
                    string text = "- " + property.Name + ": " + propValue;
                    spriteBatch.DrawString(text, new Vector2(sx + _xStep, sy), new Vector2(2.0f), Color.White, _font);
                    sy += _yStep;
                }
            }
        }

        // note: maybe wrap this into a ui class
        private class Panel
        {
            public Rectangle Rectangle { get; set; }
            public Color FillColor { get; set; } = Color.Black;
            public Color StrokeColor { get; set; } = Color.White;
            public int StrokeWidth { get; set; } = 2;

            public Panel(Rectangle rectangle)
            {
                Rectangle = rectangle;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.FillQuad(Rectangle, 0, FillColor);
                spriteBatch.StrokeRect(Rectangle, StrokeWidth, StrokeColor);
            }
        }
    }
}
