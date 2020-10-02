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
        public Size Size { get; }
        public Texture Texture { get; }

        public RenderTarget(int width, int height)
        {
            Size = new Size(width, height);

            _id = GL.GenFramebuffer();
            Bind();

            Texture = new Texture(IntPtr.Zero, width, height);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.Id, 0);

            // todo: check the shit out of this
            FramebufferErrorCode fec = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        }
        
        // todo: check framebuffer status
        
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