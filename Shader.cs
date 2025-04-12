namespace CadDev;
using OpenTK.Graphics.OpenGL4;
public class Shader
{
    private int _handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        int VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        GL.CompileShader(VertexShader);

        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int successvertex);
        if (successvertex == 0)
        {
            string infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(FragmentShader);

        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int successfrag);
        if (successfrag == 0)
        {
            string infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine(infoLog);
        }

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, VertexShader);
        GL.AttachShader(_handle, FragmentShader);

        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            Console.WriteLine(infoLog);
        }

        GL.DetachShader(_handle, VertexShader);
        GL.DetachShader(_handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }
    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public int GetHandle()
    {
        return _handle;
    }
}