using System;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class VertexArrayObject<TVerexType, TIndexType> : IDisposable
        where TVerexType : unmanaged
        where TIndexType : unmanaged
    {

        private int _id;

        public VertexArrayObject(BufferObject<TVerexType> vbo, BufferObject<TIndexType> ibo)
        {
            _id = GL.GenVertexArray();
            Bind();
            vbo.Bind();
            ibo.Bind();
        }

        public void VertexAttribPointer(int index, int count, VertexAttribPointerType type, int vertexSize, int offset)
        {
            unsafe
            {
                int vertexTypeSize = (int)sizeof(TVerexType);
                GL.VertexAttribPointer(index, count, type, false, vertexSize * vertexTypeSize, offset * vertexTypeSize);
            }
            GL.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            GL.BindVertexArray(_id);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_id);
        }
    }
}
