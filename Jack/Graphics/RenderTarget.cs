using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class RenderTarget : IDisposable
    {
        // todo: still need to figure out how to use this
        private int _id;
        public Texture Texture { get; }

        public RenderTarget(int width, int height)
        {
            Texture = new Texture(IntPtr.Zero, width, height);
            Texture.Bind();

            _id = GL.GenFramebuffer();
            Bind();

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.Id, 0);

            FramebufferErrorCode fec = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            if (fec != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer not complete");
            }
            Unbind();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(_id);
        }
    }
}