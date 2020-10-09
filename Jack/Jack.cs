using System;
using System.Drawing;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using Jack.Graphics;
using Jack.Core;

// todo: fullscreen support
// todo: some audio work
// todo: input work

namespace Jack
{
    public abstract class JackApp : IDisposable
    {
        private static GameWindow _window;

        private static Size _windowSize;
        public static Size WindowSize
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

        private static string _windowTitle;
        public static string WindowTitle
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

        private static VSyncMode _windowVsync;
        public static VSyncMode WindowVsync
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

        public static Scene CurrentScene { get; set; }

        private static Vector2 _mousePosition = Vector2.Zero;
        public static Vector2 MousePosition => _mousePosition;

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
            _window.MouseMove += new EventHandler<MouseMoveEventArgs>(OnMouseMove);
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

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.Zero);

            SpriteBatch = new SpriteBatch(this);

            Load();
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            Update((float)e.Time);

            if (CurrentScene != null)
            {
                CurrentScene.Update((float)e.Time);
            }

        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (CurrentScene != null)
            {
                CurrentScene.Draw(SpriteBatch);
            }

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