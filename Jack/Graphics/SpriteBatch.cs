using System;
using System.Collections.Generic;
using System.Drawing;
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
        // todo: maybe fond a way and make this dynamic
        private const int TEXTURE_SLOTS_COUNT = 8;

        private static readonly float[] _quadVertices =
        {
            0.5f,  0.5f,
            0.5f, -0.5f,
            -0.5f, -0.5f,
            -0.5f,  0.5f
        };

        private static readonly float[] _quadTextureCoords =
        {
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f
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

        public Matrix4 ProjectionMatrix { get; set; }
        public Matrix4 ViewMatrix { get; set; }

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
            ProjectionMatrix = Matrix4.CreateOrthographic(jack.WindowSize.Width, jack.WindowSize.Height, -1.0f, 1.0f);
            ViewMatrix = Matrix4.Identity;

            _batchSize = batchSize;

            _quadShader = Shader.FromStrings(_quadVertShaderSource, _quadFragmentShaderSource);

            // the amount of quads times the size of a vertex times 4 vertices per quad
            _indices = GenerateIndices(batchSize);

            _quadVbo = new BufferObject<float>(_vertices, BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ArrayBuffer, VerticesAmount * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            _quadIbo = new BufferObject<int>(_indices, BufferTarget.ElementArrayBuffer);

            _quadVao = new VertexArrayObject<float, int>(_quadVbo, _quadIbo);
            _quadVao.VertexAttribPointer(0, POSITION_SIZE, VertexAttribPointerType.Float, VertexSize, 0);
            _quadVao.VertexAttribPointer(1, COLOR_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE);
            _quadVao.VertexAttribPointer(2, TEX_COORDS_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE + COLOR_SIZE);
            _quadVao.VertexAttribPointer(3, TEX_INDEX_SIZE, VertexAttribPointerType.Float, VertexSize, POSITION_SIZE + COLOR_SIZE + TEX_COORDS_SIZE);

            jack.OnExit += Dispose;
        }

        private int[] GenerateIndices(int quadsAmount)
        {
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
        
        public void TranslateViewMatrix(Vector2 amount)
        {
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(amount.X, amount.Y, 0.0f));
            ViewMatrix *= translation;
        }
        
        public void ScaleViewMatrix(Vector2 amount)
        {
            Matrix4 scale = Matrix4.CreateScale(new Vector3(amount.X, amount.Y, 0.0f));
            ViewMatrix *= scale;
        }

        private bool _beginCalled = false;
        public void Begin()
        {
            if (_beginCalled)
            {
                throw new InvalidOperationException("Begin was already called once");
            }
            _beginCalled = true;

            _vertices = new float[VerticesAmount];
            _textures = new List<Texture>(TEXTURE_SLOTS_COUNT);
            Matrix4 viewProj = ProjectionMatrix * ViewMatrix;

            _quadShader.SetUniform("u_Textures", _textureSlots);
            _quadShader.Bind();
            _quadShader.SetUniform("u_MVP", viewProj);
        }

        private void SetVertexData(Vector2 position, Vector2 size, float rotation, float texIndex, Color color)
        {
            int offset = _quadCount * 4 * VertexSize;

            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(size.X, size.Y, 0.0f));
            Matrix4 rotationM = Matrix4.CreateRotationZ(rotation);

            Matrix4 transform = rotationM * scale * translation;

            for (int i = 0; i < 4; i++)
            {
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

                // set vertex tex coords
                _vertices[offset + 6] = _quadTextureCoords[i * 2];
                _vertices[offset + 7] = _quadTextureCoords[i * 2 + 1];

                // set vertex tex index
                _vertices[offset + 8] = texIndex;

                offset += VertexSize;
            }
        }

        public void DrawQuad(Vector2 position, Vector2 size, float rotation, Texture texture, Color color)
        {
            if (_quadCount >= _batchSize || _textures.Count >= TEXTURE_SLOTS_COUNT)
            {
                End();
                Begin();
            }

            float textureIndex = 0;
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

            SetVertexData(position, size, rotation, textureIndex, color);

            _quadCount++;
        }

        public void DrawQuad(Vector2 position, Vector2 size, float rotation, Color color)
        {
            if (_quadCount >= _batchSize)
            {
                End();
                Begin();
            }
            SetVertexData(position, size, rotation, 0.0f, color);
            _quadCount++;
        }

        public void End()
        {
            if (!_beginCalled)
            {
                throw new InvalidOperationException("Cannot call end before calling begin");
            }
            _beginCalled = false;

            _quadVbo.Bind();
            int size = _quadCount * 4 * VertexSize * sizeof(float);
            GL.BufferSubData<float>(BufferTarget.ArrayBuffer, IntPtr.Zero, size, _vertices);

            for (int i = 0; i < _textures.Count; i++)
            {
                _textures[i].Bind(TextureUnit.Texture0 + i + 1);
            }

            _quadVao.Bind();
            _quadShader.Bind();
            GL.DrawElements(BeginMode.Triangles, _quadCount * 6, DrawElementsType.UnsignedInt, 0);

            _quadCount = 0;
        }

        public void Dispose()
        {
            _quadShader.Dispose();
            _quadVao.Dispose();
            _quadVbo.Dispose();
            _quadIbo.Dispose();
        }
    }
}
