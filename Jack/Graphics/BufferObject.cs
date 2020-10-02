using System;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private int _id;
        private BufferTarget _target;

        public BufferObject(Span<TDataType> data, BufferTarget target, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            _target = target;
            _id = GL.GenBuffer();
            Bind();

            int bufferSize;
            unsafe
            {
                bufferSize = data.Length * sizeof(TDataType);
            }
            if (usage == BufferUsageHint.StaticDraw)
            {
                // only put data in the buffer if the buffer usage is static
                GL.BufferData<TDataType>(_target, bufferSize, data.ToArray(), usage);
            }
            else if (usage == BufferUsageHint.DynamicDraw)
            {
                GL.BufferData(_target, bufferSize, IntPtr.Zero, usage);
            }
        }

        public void Bind()
        {
            GL.BindBuffer(_target, _id);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_id);
        }
    }
}
