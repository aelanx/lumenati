using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public class LumenShader : Shader
    {
        protected override string vs { get; } = @"
#version 130

//int vec2 aUV;
out vec2 vUV;

void main()
{
    vUV = gl_MultiTexCoord0.xy;
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
}
";

        protected override string fs { get; } = @"
#version 130

in vec2 vUV;
uniform sampler2D uTex;
uniform vec4 uColorAdd;
uniform vec4 uColorMul;

void main()
{
    vec4 tex = texture2D(uTex, vUV);
    tex = tex.rrrr;

    if (tex.a < 0.01)
        discard;

    gl_FragColor = (tex * uColorMul) + uColorAdd;
}
";

        public int aPos { get; } = -1;
        public int aUV { get; } = -1;
        public int uTex { get; } = -1;
        public int uATI { get; } = -1;
        public int uColorAdd { get; } = -1;
        public int uColorMul { get; } = -1;
        public int uTransform { get; } = -1;
        public int uView { get; } = -1;

        public LumenShader() : base()
        {
            //aPos = GL.GetAttribLocation(ProgramID, "aPos");
            //aUV = GL.GetAttribLocation(ProgramID, "aUV");
            uTex = GL.GetUniformLocation(ProgramID, "uTex");
            //uATI = GL.GetUniformLocation(ProgramID, "uATI");
            uColorAdd = GL.GetUniformLocation(ProgramID, "uColorAdd");
            uColorMul = GL.GetUniformLocation(ProgramID, "uColorMul");
            //uTransform = GL.GetUniformLocation(ProgramID, "uTransform");
            //uView = GL.GetUniformLocation(ProgramID, "uView");
        }

        public override void EnableAttrib()
        {
            //GL.EnableVertexAttribArray(aPos);
            //GL.EnableVertexAttribArray(aUV);
        }

        public override void DisableAttrib()
        {
            //GL.DisableVertexAttribArray(aPos);
            //GL.DisableVertexAttribArray(aUV);
        }
    }
}