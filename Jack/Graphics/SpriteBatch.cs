using System;
using System.Drawing;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class SpriteBatch : IDisposable
    {
        private const int POSITION_SIZE = 2;
        private const int COLOR_SIZE = 4;
        private const int TEX_COORDS_SIZE = 2;
        private const int TEX_INDEX_SIZE = 1;
        // todo: maybe find a way and make this dynamic
        private const int TEXTURE_SLOTS_COUNT = 8;

        private static readonly float[] _quadVertices =
        {
            0.5f,  0.5f,
            0.5f, -0.5f,
            -0.5f, -0.5f,
            -0.5f,  0.5f
        };

        private static readonly string _quadVertShaderSource = @"
        #version 330 core

        layout(location = 0) in vec3 a_Position;
        layout(location = 1) in vec4 a_Color;
        layout(location = 2) in vec2 a_TexCoords;
        layout(location = 3) in float a_TexIndex;

        uniform mat4 u_MVP;

        out vec4 f_Color;
        out vec2 f_TexCoords;
        out float f_TexIndex;

        void main() {
            f_Color = a_Color;
            f_TexCoords = a_TexCoords;
            f_TexIndex = a_TexIndex;

            gl_Position = u_MVP * vec4(a_Position, 1.0);
        }
        ";

        private static readonly string _quadFragmentShaderSource = @"
        #version 330 core

        in vec4 f_Color;
        in vec2 f_TexCoords;
        in float f_TexIndex;

        uniform sampler2D u_Textures[8];

        out vec4 color;

        void main() {
            if (f_TexIndex > 0) {
                int id = int(f_TexIndex);
                color = f_Color * texture(u_Textures[id], f_TexCoords);
            } else {
                color = f_Color;
            }
        }
        ";

        private int _batchSize;
        private int _quadCount = 0;

        private Shader _quadShader;

        private int[] _indices;
        private float[] _vertices;
        private int[] _textureSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };

        private BufferObject<float> _quadVbo;
        private BufferObject<int> _quadIbo;
        private VertexArrayObject<float, int> _quadVao;
        private List<Texture> _textures;

        private int VertexSize => POSITION_SIZE + COLOR_SIZE + TEX_COORDS_SIZE + TEX_INDEX_SIZE;
        private int VerticesAmount => _batchSize * VertexSize * 4;

        public SpriteBatch(JackApp jack, int batchSize = 1000)
        {
            _batchSize = batchSize;

            _quadShader = Shader.FromStrings(_quadVertShaderSource, _quadFragmentShaderSource);

            // the amount of quads times the size of a vertex times 4 vertices per quad
            _indices = GenerateIndices(batchSize);

            // make a dynamic vbo and set it's data to null
            _quadVbo = new BufferObject<float>(_vertices, BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ArrayBuffer, VerticesAmount * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            _quadIbo = new BufferObject<int>(_indices, BufferTarget.ElementArrayBuffer);

            // add descriptions for the vertices inside the vao
            _quadVao = new VertexArrayObject<float, int>(_quadVbo, _quadIbo);
            _quadVao.VertexAttribPointer(0, POSITION_SIZE, VertexAttribPointerType.Float, VertexSize, 0);
            _quadVao.VertexAttribPointer(1, COLOR_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE);
            _quadVao.VertexAttribPointer(2, TEX_COORDS_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE + COLOR_SIZE);
            _quadVao.VertexAttribPointer(3, TEX_INDEX_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE + COLOR_SIZE + TEX_COORDS_SIZE);

            JackApp.OnExit += Dispose;
        }

        private int[] GenerateIndices(int quadsAmount)
        {
            // the indices in order to draw a quad are { 0, 1, 2, 2, 3, 0}
            // (2 trangles)
            // fill the entire ibo with this

            int[] indices = new int[quadsAmount * 6];

            int offset = 0;
            for (int i = 0; i < indices.Length; i += 6)
            {
                indices[i + 0] = offset + 0;
                indices[i + 1] = offset + 1;
                indices[i + 2] = offset + 2;

                indices[i + 3] = offset + 2;
                indices[i + 4] = offset + 3;
                indices[i + 5] = offset + 0;

                offset += 4;
            }

            return indices;
        }

        private bool _beginCalled = false;
        private Camera _camera;
        public void Begin(Camera camera)
        {
            // only allow for begin to be called once
            if (_beginCalled)
            {
                throw new Exception("Begin was already called once");
            }
            _beginCalled = true;
            _camera = camera;

            // every time we have a new draw call we need to reset the vertices and the textures
            _vertices = new float[VerticesAmount];
            _textures = new List<Texture>(TEXTURE_SLOTS_COUNT);

            // set the world matrix inside the shader
            // note: i can also send proj matrix and view matrix and 
            // have them multiplied on the gpu
            Matrix4 viewProj = camera.ProjectionMatrix * camera.ViewMatrix;

            _quadShader.Bind();
            _quadShader.SetUniform("u_Textures", _textureSlots);
            _quadShader.SetUniform("u_MVP", viewProj);
        }

        public void End()
        {
            // dont allow for end to be called twice
            if (!_beginCalled)
            {
                throw new Exception("Cannot call end before calling begin");
            }
            _beginCalled = false;

            // on end we have to bind all the data to the vbo
            _quadVbo.Bind();
            int size = _quadCount * 4 * VertexSize * sizeof(float);
            GL.BufferSubData<float>(BufferTarget.ArrayBuffer, IntPtr.Zero, size, _vertices);

            // also bind every texture
            for (int i = 0; i < _textures.Count; i++)
            {
                _textures[i].Bind(TextureUnit.Texture0 + i + 1);
            }

            // and finally draw everything
            _quadVao.Bind();
            _quadShader.Bind();
            GL.DrawElements(BeginMode.Triangles, _quadCount * 6, DrawElementsType.UnsignedInt, 0);

            // also reset the quadCount
            _quadCount = 0;
        }

        private void SetVertexData(
            Vector2 position, Vector2 size, Rectangle sourceRectangle,
            float rotation, float texIndex, int texWidth, int texHeight, Color color
        )
        {

            // add data to the vertex buffer

            // calc transform for the quad
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(size.X, size.Y, 0.0f));
            Matrix4 rotationM = Matrix4.CreateRotationZ(rotation);

            Matrix4 transform = scale * rotationM * translation;

            // normalized sourceRectangle
            // we need this to set the texture coords accordingly
            RectangleF nsrc = new RectangleF(
                (float)sourceRectangle.X / (float)texWidth,
                (float)sourceRectangle.Y / (float)texHeight,
                (float)sourceRectangle.Width / (float)(texWidth),
                (float)sourceRectangle.Height / (float)(texHeight)
            );

            Vector2[] texCoords =
            {
                new Vector2(nsrc.Right, nsrc.Top),
                new Vector2(nsrc.Right, nsrc.Bottom),
                new Vector2(nsrc.Left, nsrc.Bottom),
                new Vector2(nsrc.Left, nsrc.Top)
            };

            // calculate where we are in the vertices array
            int offset = _quadCount * 4 * VertexSize;

            for (int i = 0; i < 4; i++)
            {
                // for every vertex we have to calculate it's position inside the world matrix
                // i don't know why the last paramter needs to be 1 but it won't work w/out it
                Vector4 vertexPosition = new Vector4(_quadVertices[i * 2], _quadVertices[i * 2 + 1], 0.0f, 1.0f);
                vertexPosition *= transform;

                // set vertex position
                _vertices[offset + 0] = vertexPosition.X;
                _vertices[offset + 1] = vertexPosition.Y;

                // set vertex color
                _vertices[offset + 2] = color.R / 255.0f;
                _vertices[offset + 3] = color.G / 255.0f;
                _vertices[offset + 4] = color.B / 255.0f;
                _vertices[offset + 5] = color.A / 255.0f;

                // set the tex coords
                _vertices[offset + 6] = texCoords[i].X;
                _vertices[offset + 7] = texCoords[i].Y;

                // set vertex tex index
                _vertices[offset + 8] = texIndex;

                offset += VertexSize;
            }
        }

        public void FillQuad(Vector2 position, Vector2 size, float rotation, Color color)
        {
            if (_quadCount >= _batchSize)
            {
                End();
                Begin(_camera);
            }
            SetVertexData(position, size, new Rectangle(0, 0, 1, 1), rotation, 0.0f, 1, 1, color);
            _quadCount++;
        }

        public void FillQuad(Rectangle rectangle, float rotation, Color color)
        {
            Vector2 position = new Vector2(
                rectangle.X + rectangle.Width / 2,
                rectangle.Y + rectangle.Height / 2
            );
            Vector2 size = new Vector2(rectangle.Width, rectangle.Height);
            FillQuad(position, size, rotation, color);
        }

        // note: add origin to all of these

        public void Draw(Texture texture, Vector2 position, Vector2 size, float rotation, Rectangle sourceRectangle, Color color)
        {
            if (_quadCount >= _batchSize || _textures.Count >= TEXTURE_SLOTS_COUNT)
            {
                End();
                Begin(_camera);
            }

            int textureIndex = CheckTextureIndex(texture);
            SetVertexData(position, size, sourceRectangle, rotation, textureIndex, texture.Width, texture.Height, color);

            _quadCount++;
        }

        public void Draw(Texture texture, Vector2 position, Vector2 size, float rotation, Color color)
        {
            Draw(texture, position, size, rotation, texture.Rectangle, color);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 size, Rectangle sourceRectangle)
        {
            Draw(texture, position, size, 0, sourceRectangle, Color.White);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 size)
        {
            Draw(texture, position, size, 0, Color.White);
        }

        public void Draw(Texture texture, Rectangle destinationRectangle, float rotation, Rectangle sourceRectangle, Color color)
        {
            Vector2 position = new Vector2(
                destinationRectangle.X + destinationRectangle.Width / 2,
                destinationRectangle.Y + destinationRectangle.Height / 2
            );
            Vector2 size = new Vector2(destinationRectangle.Width, destinationRectangle.Height);
            Draw(texture, position, size, rotation, sourceRectangle, color);
        }

        public void Draw(Texture texture, Rectangle destinationRectangle, float rotation, Color color)
        {
            Draw(texture, destinationRectangle, 0, texture.Rectangle, color);
        }

        public void Draw(Texture texture, Rectangle destinationRectangle)
        {
            Draw(texture, destinationRectangle, 0, Color.White);
        }

        public void DrawLine(Vector2 a, Vector2 b, int thickness, Color color)
        {
            Vector2 edge = a - b;
            float rotation = (float)Math.Atan2(edge.X, edge.Y);
            Vector2 middle = new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);

            FillQuad(middle, new Vector2(thickness, edge.Length), rotation, color);
        }

        public void StrokeRect(Rectangle rectangle, int thickness, Color color)
        {
            DrawLine(
                new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top),
                thickness, color
            );
            DrawLine(
                new Vector2(rectangle.Right, rectangle.Top), new Vector2(rectangle.Right, rectangle.Bottom),
                thickness, color
            );
            DrawLine(
                new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Bottom),
                thickness, color
            );
            DrawLine(
                new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Top),
                thickness, color
            );
        }

        private int CheckTextureIndex(Texture texture)
        {
            int textureIndex = 0;
            for (int i = 0; i < _textures.Count; i++)
            {
                if (_textures[i].Id == texture.Id)
                {
                    textureIndex = i + 1;
                    break;
                }
            }
            if (textureIndex == 0)
            {
                textureIndex = _textures.Count + 1;
                _textures.Add(texture);
            }

            return textureIndex;
        }

        public void DrawString(string text, Vector2 position, Vector2 scale, Color color, SpriteFont font)
        {
            float xStep = (float)(font.GlyphWidth + font.FontSize) / (float)(font.FontTexture.Width);
            float yStep = (float)(font.GlyphHeight + font.FontSize) / (float)(font.FontTexture.Height);

            float x = position.X;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                float cx = (float)(c % font.GlyphsPerLine) * xStep;
                float cy = (float)(c / font.GlyphsPerLine) * yStep;

                int charX = (int)(cx * font.FontTexture.Width);
                // note: it needs to be that weird because, the texture has to be somehwat flipped
                // i dont know, projection matrix, img.Mutate(x => x.flip vertical) check camera and texture
                int charY = (int)(font.FontTexture.Height - (cy * font.FontTexture.Height) - (yStep * font.FontTexture.Height));
                int charWidth = (int)(xStep * font.FontTexture.Width);
                int charHeight = (int)(yStep * font.FontTexture.Height);

                Rectangle srcRect = new Rectangle(charX, charY, charWidth, charHeight);
                Vector2 pos = new Vector2(x, position.Y);
                Vector2 size = new Vector2(font.GlyphWidth * scale.X, font.GlyphHeight * scale.Y);

                Draw(font.FontTexture, pos, size, 0, srcRect, color);

                x += (scale.X * font.CharSpacing);
            }
        }

        // todo: make function to draw string inside quad with word wrap or smth

        public void Dispose()
        {
            _quadShader.Dispose();
            _quadVao.Dispose();
            _quadVbo.Dispose();
            _quadIbo.Dispose();
        }
    }
}
