using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using System.Numerics;
using OpenTK.Input;
using Silk.NET.Core.Native;

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

class Program()
{
    static void Main()
    {
        var NativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Raycast on steroids",
        };

        using var window = new RaycastWindow(GameWindowSettings.Default, NativeWindowSettings);
        window.Run();
    }

}
