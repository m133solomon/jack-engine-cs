using Jack;
using Jack.Graphics;
using Jack.Core;
using System.Drawing;
using OpenTK;

namespace Jack
{
    public static class DebugLayer
    {
        public static bool Active { get; set; } = true;

        private static JackApp _app;
        private static Camera _camera;
        private static SpriteFont _font;

        private static Color _panelColor = Color.FromArgb(150, 0, 0, 0);
        private static Color _accentColor = Color.DeepPink;

        private static Panel _nodesPanel;

        public static void Init(JackApp app)
        {
            _app = app;
            _camera = new Camera(_app, _app.WindowSize.Width, _app.WindowSize.Height);
            _font = new SpriteFont("Menlo", 37);

            int panelMargin = 20;
            _nodesPanel = new Panel(
                new Rectangle(
                    panelMargin, panelMargin, _app.WindowSize.Width / 4,
                    _app.WindowSize.Height - panelMargin * 2
                )
            )
            { FillColor = _panelColor, StrokeColor = _accentColor };
        }

        public static void Update(float deltaTime)
        {

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Begin(_camera);
                DrawNodesPanel(spriteBatch);
                spriteBatch.End();
            }
        }

        private static void DrawNodesPanel(SpriteBatch spriteBatch)
        {
            _nodesPanel.Draw(spriteBatch);
            int sx = _nodesPanel.Rectangle.Left + 20;
            int sy = _nodesPanel.Rectangle.Top + 30;
            DrawNode(spriteBatch, _app.CurrentScene.Root, sx, ref sy);
        }

        private static int _yStep = 40;
        private static int _xStep = 40;

        // todo: allow me to click on the node
        private static void DrawNode(SpriteBatch spriteBatch, Node node, int startX, ref int startY)
        {
            spriteBatch.DrawString(node.Name, new Vector2(startX, startY), new Vector2(2, 1.5f), Color.White, _font);

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