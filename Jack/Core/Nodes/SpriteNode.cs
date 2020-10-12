using Jack.Graphics;
using System.Drawing;
using OpenTK;

namespace Jack.Core.Nodes
{
    // note: will probably make a better one of these when I will use the engine for an acutal game
    public class SpriteNode : Node, IUpdateable, IDrawable
    {
        public Texture Texture;
        public Color Color = Color.White;
        public Rectangle SourceRectangle = Rectangle.Empty;

        public virtual void Update(float deltaTime)
        {
        }

        protected virtual void DrawTextureDefault(SpriteBatch spriteBatch)
        {
            Vector2 size = new Vector2(Texture.Width * GlobalTransform.Scale.X, Texture.Height * GlobalTransform.Scale.Y);
            if (SourceRectangle == Rectangle.Empty)
            {
                spriteBatch.Draw(Texture, GlobalTransform.Position, size, GlobalTransform.Rotation, Color);
            }
            else
            {
                spriteBatch.Draw(Texture, GlobalTransform.Position, size, GlobalTransform.Rotation, SourceRectangle, Color);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}