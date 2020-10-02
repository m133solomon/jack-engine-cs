using System.IO;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Jack.Graphics
{
    public class Shader : IDisposable
    {
        private int _id;

        private Shader(int id)
        {
            _id = id;
        }

        public static Shader FromStrings(string vertexSource, string fragmentSource)
        {
            int vertex = CompileShader(ShaderType.VertexShader, vertexSource);
            int fragment = CompileShader(ShaderType.FragmentShader, fragmentSource);

            int id = GL.CreateProgram();

            GL.AttachShader(id, vertex);
            GL.AttachShader(id, fragment);
            GL.LinkProgram(id);
            GL.GetProgram(id, GetProgramParameterName.LinkStatus, out var status);

            if (status == 0)
            {
                throw new Exception("Could not link shader");
            }

            return new Shader(id);
        }

        public static Shader FromFiles(string vertexPath, string fragmentPath)
        {
            if (!File.Exists(vertexPath))
            {
                throw new FileLoadException("Vertex shader not found: " + vertexPath);
            }
            if (!File.Exists(fragmentPath))
            {
                throw new FileLoadException("Fragment shader not found: " + fragmentPath);
            }

            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            return Shader.FromStrings(vertexSource, fragmentSource);
        }

        private static int CompileShader(ShaderType type, string source)
        {
            int id = GL.CreateShader(type);

            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            string infoLog = GL.GetShaderInfoLog(id);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                string msg = "Could not compile " + type.ToString();
                Console.WriteLine(msg);
                Console.WriteLine(infoLog);

                throw new Exception(msg);
            }

            return id;
        }

        public void Bind()
        {
            GL.UseProgram(_id);
        }

        public void Dispose()
        {
            GL.DeleteProgram(_id);
        }

        private int GetUniformLocation(string uniformName)
        {
            int location = GL.GetUniformLocation(_id, uniformName);
            if (location == -1)
            {
                Console.WriteLine("Could not find uniform: " + uniformName);
            }
            return location;
        }

        public void SetUniform(string uniformName, int value)
        {
            int location = GetUniformLocation(uniformName);
            GL.Uniform1(location, value);
        }

        public void SetUniform(string uniformName, float value)
        {
            int location = GetUniformLocation(uniformName);
            GL.Uniform1(location, value);
        }

        public void SetUniform(string uniformName, Vector4 value)
        {
            int location = GetUniformLocation(uniformName);
            GL.Uniform4(location, value);
        }

        public void SetUniform(string uniformName, Matrix4 value)
        {
            int location = GetUniformLocation(uniformName);
            unsafe
            {
                GL.UniformMatrix4(location, 1, false, (float*)&value);
            }
        }
        
        public void SetUniform(string uniformName, int[] value)
        {
            int location = GetUniformLocation(uniformName);
            GL.Uniform1(location, value.Length, value);
        }
    }
}