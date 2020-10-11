using System;
using System.Drawing;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using Jack.Graphics;
using Jack.Core;

// todo: some audio work
// todo: input work

namespace Jack
{
    public abstract class JackApp : IDisposable
    {
        private static GameWindow _window;

        private static int _windowWidth;
        public static int WindowWidth
        {
            get => _window.Width;
            set
            {
                _windowWidth = value;
                if (_window != null)
                {
                    _window.Width = value;
                }
            }
        }

        private static int _windowHeight;
        public static int WindowHeight
        {
            get => _window.Height;
            set
            {
                _windowHeight = value;
                if (_window != null)
                {
                    _window.Height = value;
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
        public static event JackEventHandler OnExit;
        public static event JackEventHandler OnWindowResize;

        public SpriteBatch SpriteBatch { get; private set; }

        public static Scene CurrentScene { get; set; }

        public JackApp()
        {
            _windowWidth = 800;
            _windowHeight = 600;
            _windowTitle = "Jack Application";
            _windowVsync = VSyncMode.On;
        }

        public void Run()
        {
            _window = new GameWindow(
                _windowWidth, _windowHeight, GraphicsMode.Default, _windowTitle,
                WindowFlags, DisplayDevice.Default, OpenGLMajorVersion, OpenGLMinorVersion, GraphicsContextFlags.Default
            );

            _window.VSync = _windowVsync;

            _window.Load += new EventHandler<EventArgs>(Load);
            _window.UpdateFrame += new EventHandler<FrameEventArgs>(UpadateFrame);
            _window.RenderFrame += new EventHandler<FrameEventArgs>(RenderFrame);
            _window.Unload += new EventHandler<EventArgs>(Unload);
            _window.Resize += new EventHandler<EventArgs>(Resize);
            _window.MouseMove += new EventHandler<MouseMoveEventArgs>(Input.MouseMove);
            _window.MouseDown += new EventHandler<MouseButtonEventArgs>(Input.MouseDown);
            _window.MouseUp += new EventHandler<MouseButtonEventArgs>(Input.MouseUp);
            _window.Run();
        }

        private void Resize(object sender, EventArgs e)
        {
            _windowWidth = _window.Width;
            _windowHeight = _window.Height;
            if (OnWindowResize != null)
            {
                OnWindowResize();
            }
        }

        private void Load(object sender, EventArgs e)
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

        private void UpadateFrame(object sender, FrameEventArgs e)
        {
            Update((float)e.Time);

            if (CurrentScene != null)
            {
                CurrentScene.Update((float)e.Time);
            }

        }

        private void RenderFrame(object sender, FrameEventArgs e)
        {
            if (CurrentScene != null)
            {
                CurrentScene.Draw(SpriteBatch);
            }

            Draw();

            _window.SwapBuffers();
        }

        private void Unload(object sender, EventArgs e)
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