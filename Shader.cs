using System;
using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public abstract class Shader
    {
        public int ProgramID { get; }

        protected virtual string vs { get; }
        protected virtual string fs { get; }

        public Shader()
        {
            ProgramID = GL.CreateProgram();

            loadShader(vs, ShaderType.VertexShader, ProgramID);
            loadShader(fs, ShaderType.FragmentShader, ProgramID);
        }

        ~Shader()
        {
            //GL.DeleteProgram(ProgramID);
        }

        public virtual void EnableAttrib() { }

        public virtual void DisableAttrib() { }

        protected void loadShader(string shader, ShaderType type, int program)
        {
            int address = GL.CreateShader(type);

            GL.ShaderSource(address, shader);

            GL.CompileShader(address);
            GL.AttachShader(program, address);

            var log = GL.GetShaderInfoLog(address);

            if (!string.IsNullOrEmpty(log))
                Console.WriteLine(log);

            GL.LinkProgram(ProgramID);
        }
    }
}