using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class RenderTarget : IDisposable
    {
        private int _id;
        public Size Size { get; }
        public Texture Texture { get; }

        public RenderTarget(int width, int height)
        {
            _id = GL.GenFramebuffer();

            Size = new Size(width, height);

            Texture = new Texture(new byte[] { 0 }, 0, 0);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Texture.Id, 0);
        }
        
        // todo: check frmaebuffer status
        
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