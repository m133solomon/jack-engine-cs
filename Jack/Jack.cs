using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

// TODO: input management
// TODO: z-sorting?
// TODO: frambuffer - rendertargets

namespace Jack
{
    public abstract class JackApp : IDisposable
    {
        private GameWindow _window;

        private Size _windowSize;
        public Size WindowSize
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                if (_window != null)
                {
                    _window.ClientSize = value;
                }
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                if (_window != null)
                {
                    _window.Title = value;
                }
            }
        }
        
        private VSyncMode _windowVsync;
        public VSyncMode WindowVsync
        {
            get => _windowVsync;
            set
            {
                _windowVsync = value;
                if (_window != null)
                {
                    _window.VSync = value;
                }
            }
        }
        
        private Color _clearColor;
        public Color ClearColor
        {
            get => _clearColor;
            set
            {
                _clearColor = value;
                if (_window != null)
                {
                    GL.ClearColor(value);
                }
            }
        }

        protected int OpenGLMajorVersion { get; set; } = 3;
        protected int OpenGLMinorVersion { get; set; } = 3;
        protected GameWindowFlags WindowFlags { get; set; } = GameWindowFlags.Default;

        public delegate void JackEventHandler();
        public event JackEventHandler OnExit;

        public JackApp()
        {
            _windowSize = new Size(800, 600);
            _windowTitle = "Jack Application";
            _clearColor = Color.CornflowerBlue;
            _windowVsync = VSyncMode.On;
        }

        public void Run()
        {
            _window = new GameWindow(
                _windowSize.Width, _windowSize.Height, GraphicsMode.Default, _windowTitle,
                WindowFlags, DisplayDevice.Default, OpenGLMajorVersion, OpenGLMinorVersion, GraphicsContextFlags.Default
            );

            _window.VSync = _windowVsync;

            _window.Load += new EventHandler<EventArgs>(OnLoad);
            _window.UpdateFrame += new EventHandler<FrameEventArgs>(OnUpdateFrame);
            _window.RenderFrame += new EventHandler<FrameEventArgs>(OnRenderFrame);
            _window.Unload += new EventHandler<EventArgs>(OnUnload);
            _window.Run();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            
            GL.ClearColor(_clearColor);
            Load();
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.Zero);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Draw((float)e.Time);

            _window.SwapBuffers();
        }

        private void OnUnload(object sender, EventArgs e)
        {
            OnExit();
        }

        protected abstract void Load();
        protected abstract void Draw(float deltaTime);
        protected abstract void Exit();

        public void Dispose()
        {
            _window.Dispose();
        }
    }
}