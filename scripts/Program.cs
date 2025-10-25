using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using System.Runtime.CompilerServices;
using OpenTK.Windowing.Common.Input;

class RaycastWindow : GameWindow
{
    public RaycastWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
    {
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        Console.WriteLine("Holy shit a new frame. fps: " + 1 / args.Time);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }
}

public class Camera
{
    public Vector3 Pos { get; set; }
    public Vector3 Front { get; set; } = -Vector3.UnitZ;
    public Vector3 Up { get; set; } = Vector3.UnitY;

    public float fieldOfView { get; set; } = MathHelper.DegreesToRadians(70);

    public Camera(Vector3 pos)
    {
        Pos = pos;
    }

    public Matrix4 GetCFrame()
    {
        return Matrix4.LookAt(Pos, Pos + Front, Up);
    }

    public Matrix4 GetPerspectiveCFrame(float AspectRatio1, float AspectRatio2)
    {
        var ResultingAspect = AspectRatio1 / AspectRatio2;
        return Matrix4.CreatePerspectiveFieldOfView(fieldOfView, ResultingAspect, 0.1f, 100f);
    }

}


class Program()
{


    static Camera cam = new Camera(new Vector3(0.3f, 0, 1));

    static void Main()
    {

        var NativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Raycast on steroids",
        };
        var GameWindowSettings = new GameWindowSettings()
        {
            UpdateFrequency = 120
        };

        using var window = new RaycastWindow(GameWindowSettings, NativeWindowSettings);

        // THE vertices
        float[] vertices =
{
                0.0f,0.5f, 0f, // top
                -0.5f,0.5f, 0f, // bottom left
                0.5f,-0.5f, 0f // bottom right
            };

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), in vertices[0], BufferUsage.StaticDraw);

        // triangle shenanigans
        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BindVertexArray(vao);
        GL.Enable(EnableCap.DepthTest);


        int vertexShader = compileShader("scripts/shaders/triangle.vert", ShaderType.VertexShader);
        int fragmentShader = compileShader("scripts/shaders/triangle.frag", ShaderType.FragmentShader);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        GL.UseProgram(shaderProgram);

        window.Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image(16, 16, File.ReadAllBytes("assets/download.jpeg")));
        float camRotXRate = 0.7f;
        float camRotX = 0f;
        window.RenderFrame += (FrameEventArgs fEventArgs) =>
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(vao);
            camRotX += camRotXRate * (float)fEventArgs.Time;
            float rawX = camRotX;
            Double resultX = MathHelper.Sin(rawX);
            cam.Front = new Vector3((float)resultX, 0f, -1.1f);
            Matrix4 view = cam.GetCFrame();
            Matrix4 proj = cam.GetPerspectiveCFrame(4, 3);

            int viewLoc = GL.GetUniformLocation(shaderProgram, "uView");
            int projLoc = GL.GetUniformLocation(shaderProgram, "uProj");
            GL.UniformMatrix4f(viewLoc, 1, false, in view);
            GL.UniformMatrix4f(projLoc, 1, false, in proj);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            window.SwapBuffers();
        };


        window.Run();
    }

    static string loadShader(string path)
    {
        return File.ReadAllText(path);
    }

    static int compileShader(string path, ShaderType type)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, loadShader(path));
        GL.CompileShader(shader);

        int status = GL.GetShaderi(shader, ShaderParameterName.CompileStatus);
        if (status == 0)
        {
            GL.GetShaderInfoLog(shader, out string info);
            throw new Exception("Error while compiling shader '{path}'. Log: " + info);
        }
        return shader;
    }

}
