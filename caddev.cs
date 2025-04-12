using System;

namespace CadDev;

using System.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


public class CadDev : GameWindow
{
    public CadDev(int width, int height, string title) :
        base(GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                ClientSize = (width, height),
                Title = title
            }
        )
    {
    }

    float[] vertices = {
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f  //Top vertex
    };
    int VertexBufferObject;
    int VertexArrayObject;
    Shader shader;


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        // DateTime now = DateTime.Now;
        // Console.WriteLine($"Current time: {now:HH:mm:ss:fff}");
        // System.Random random = new System.Random();
        // vertices[0] = random.Next(-100, 100)/100f;

        if(KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        //Code goes here
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        shader = new Shader("shader.vert", "shader.frag");

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        shader.Use();
    }

    private float _pitch = 0f;
    private float _yaw = 0f;

    private Vector3 _positionModel = new Vector3(0f, 0f, 0f);
    private Vector3 _positionCamera = new Vector3(0f, 0f, 1.5f);
    private Vector3 _up = new Vector3(0f, 1.0f, 0f);

    private float _fov = MathHelper.PiOver2;
    private float _aspectRatio = 1.0f;

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _yaw += 128.0f * (float)e.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit);

        Matrix4 model  = Matrix4.Identity * 
            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_pitch)) * 
            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_yaw));

        Matrix4 view = Matrix4.LookAt(_positionCamera, _positionModel, _up);

        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, 0.01f, 100f);

        var location0 = GL.GetUniformLocation(shader.GetHandle(), "model");
        GL.UniformMatrix4(location0, true, ref model);

        var location1 = GL.GetUniformLocation(shader.GetHandle(), "view");
        GL.UniformMatrix4(location1, true, ref view);

        var location2 = GL.GetUniformLocation(shader.GetHandle(), "projection");
        GL.UniformMatrix4(location2, true, ref projection);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        //Code goes here.
        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        _aspectRatio = e.Width / (float)e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
