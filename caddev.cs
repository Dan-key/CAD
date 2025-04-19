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
    
    private float mouseX = 0;
    private float mouseY = 0;
    private float _lastYaw = 0;
    private float _lastPitch = 0;

    private Vector3 _lastPositionCamera = new Vector3(0f, 0f, 1.5f);
    private Vector3 _lastPositionModel = new Vector3(0f, 0f, 0f);

    private float _scrollPositionCameraZ = 1.5f;
    private float _scrollPositionModelZ = 0f;
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        if (MouseState.ScrollDelta.Y != 0)
        {
            _scrollPositionCameraZ -= MouseState.ScrollDelta.Y * 0.04f;
            _scrollPositionModelZ -= MouseState.ScrollDelta.Y * 0.04f;
        }
        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
            mouseX = MouseState.Position.X;
            mouseY = MouseState.Position.Y;
            _yaw = _lastYaw;
            _pitch = _lastPitch;
        } else if (MouseState.IsButtonDown(MouseButton.Left))
        {
            var mousePosition = MouseState.Position;
            float moveX = mousePosition.X - mouseX;
            float moveY = mousePosition.Y - mouseY;
            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                 if (KeyboardState.IsKeyDown(Keys.X)) {
                    _positionCamera = new Vector3(-moveX*2/this.ClientSize.X, 0, 0) + _lastPositionCamera;
                    _positionModel = new Vector3(-moveX*2/this.ClientSize.X, 0, 0) + _lastPositionModel;
                 } else if (KeyboardState.IsKeyDown(Keys.Y)) {
                    _positionCamera = new Vector3(0, moveY*2/this.ClientSize.Y, 0) + _lastPositionCamera;
                    _positionModel = new Vector3(0, moveY*2/this.ClientSize.Y, 0) + _lastPositionModel;
                 } else {
                    _positionCamera = new Vector3(-moveX*2/this.ClientSize.X, moveY*2/this.ClientSize.Y, 0) + _lastPositionCamera;
                    _positionModel = new Vector3(-moveX*2/this.ClientSize.X, moveY*2/this.ClientSize.Y, 0) + _lastPositionModel;
                 }
            }
            else
            {
                if (KeyboardState.IsKeyDown(Keys.X))
                {
                    _yaw = 0.2f * moveX + _lastYaw;
                    _pitch = _lastPitch;
                } else if (KeyboardState.IsKeyDown(Keys.Y))
                {
                    _yaw = _lastYaw;
                    _pitch = 0.2f * moveY + _lastPitch;
                } else {
                    _yaw = 0.2f * moveX + _lastYaw;
                    _pitch = 0.2f * moveY + _lastPitch;
                }
            }

        } else if (MouseState.IsButtonReleased(MouseButton.Left))
        {
            _lastYaw = _yaw;
            _lastPitch = _pitch;
            _lastPositionCamera = _positionCamera;
            _lastPositionModel = _positionModel;
        } else if (MouseState.IsButtonPressed(MouseButton.Middle)) {
            _yaw = 0;
            _pitch = 0;
            _positionCamera = new Vector3(0f, 0f, 1.5f);
            _positionModel = new Vector3(0f, 0f, 0f);
            _lastYaw = 0;
            _lastPitch = 0;
            _lastPositionCamera = new Vector3(0f, 0f, 1.5f);
            _lastPositionModel = new Vector3(0f, 0f, 0f);
            _scrollPositionCameraZ = 1.5f;
            _scrollPositionModelZ = 0f;
        } 

        _positionCamera.Z = _scrollPositionCameraZ;
        _positionModel.Z = _scrollPositionModelZ;

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
