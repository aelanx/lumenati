using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public class LumenShader : Shader
    {
        protected override string vs { get; } = @"
#version 130

//in vec2 aPos;
//in vec2 aUV;
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
uniform int uTexFmt;
uniform vec4 uColorAdd;
uniform vec4 uColorMul;

const int TexFmtNormal = 0;
const int TexFmtRed = 1;
const int TexFmtRedGreen = 2;
const float AlphaTolerance = 0.01;

void main()
{
    vec4 tex = texture2D(uTex, vUV);

    if (uTexFmt == TexFmtRed)
    {
        tex = tex.rrrr;
    }

    vec4 color = (tex * uColorMul) + uColorAdd;

    if (color.a < AlphaTolerance)
        discard;

    if (uTexFmt == TexFmtRedGreen && color.r < AlphaTolerance && color.g < AlphaTolerance)
        discard;

    gl_FragColor = color;
}
";

        public int aPos { get; } = -1;
        public int aUV { get; } = -1;
        public int uTex { get; } = -1;
        public int uTexFmt { get; } = -1;
        public int uColorAdd { get; } = -1;
        public int uColorMul { get; } = -1;
        public int uTransform { get; } = -1;
        public int uView { get; } = -1;

        public LumenShader() : base()
        {
            //aPos = GL.GetAttribLocation(ProgramID, "aPos");
            //aUV = GL.GetAttribLocation(ProgramID, "aUV");
            uTex = GL.GetUniformLocation(ProgramID, "uTex");
            uTexFmt = GL.GetUniformLocation(ProgramID, "uTexFmt");
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