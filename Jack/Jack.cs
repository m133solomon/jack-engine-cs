using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using Jack.Graphics;

// todo: fullscreen support
// todo: some audio work
// todo: input work
// todo: the node system

namespace Jack
{
    public abstract class JackApp : IDisposable
    {
        private GameWindow _window;

        private Size _windowSize;
        public Size WindowSize
        {
            get => _window.ClientSize;
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

        protected int OpenGLMajorVersion { get; set; } = 3;
        protected int OpenGLMinorVersion { get; set; } = 3;
        protected GameWindowFlags WindowFlags { get; set; } = GameWindowFlags.Default;

        public delegate void JackEventHandler();
        public event JackEventHandler OnExit;
        public event JackEventHandler OnWindowResize;

        public SpriteBatch SpriteBatch { get; private set; }

        public JackApp()
        {
            _windowSize = new Size(800, 600);
            _windowTitle = "Jack Application";
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
            _window.Resize += new EventHandler<EventArgs>(OnResize);
            _window.Run();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _windowSize = new Size(_window.Width, _window.Height);
            if (OnWindowResize != null)
            {
                OnWindowResize();
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.Zero);

            SpriteBatch = new SpriteBatch(this);

            Load();
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            Update((float)e.Time);
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            Draw();

            _window.SwapBuffers();
        }

        private void OnUnload(object sender, EventArgs e)
        {
            Exit();
            if (OnExit != null)
            {
                OnExit();
            }
        }

        protected abstract void Load();
        protected abstract void Update(float deltaTime);
        protected abstract void Draw();
        protected abstract void Exit();

        public void Dispose()
        {
            _window.Dispose();
        }
    }
}