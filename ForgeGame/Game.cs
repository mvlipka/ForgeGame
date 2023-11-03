using System.Runtime.InteropServices;

namespace ForgeGame;

public static partial class GameNative
{
    private const string DllName = "ForgeEngine.dll"; // Replace with your actual DLL name without the '.dll' extension.

    [LibraryImport(DllName)]
    public static partial void game_destroy(IntPtr game);

    [LibraryImport(DllName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr game_create(string windowTitle);

    [LibraryImport(DllName)]
    public static partial void game_run(IntPtr game);

    [LibraryImport(DllName)]
    public static partial IntPtr game_get_inputsystem(IntPtr game);
}


public class Game : IDisposable
{
    private InputSystem? inputSystem;
    
    private IntPtr nativeGame;

    public Game(string windowTitle)
    {
        nativeGame = GameNative.game_create(windowTitle);
        if (nativeGame == IntPtr.Zero)
            throw new InvalidOperationException("Failed to create game instance.");
    }

    public void Run()
    {
        GameNative.game_run(nativeGame);
    }
    
    public InputSystem GetInputSystem()
    {
        if (inputSystem != null) return inputSystem;
        
        IntPtr inputSystemPtr = GameNative.game_get_inputsystem(nativeGame);
        if (inputSystemPtr == IntPtr.Zero)
            throw new InvalidOperationException("Failed to get input system instance.");
        if (nativeGame == IntPtr.Zero)
            throw new InvalidOperationException("Native pointer for InputSystem is null.");

        inputSystem = new InputSystem(inputSystemPtr);
        return inputSystem;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (nativeGame != IntPtr.Zero)
        {
            GameNative.game_destroy(nativeGame);
            nativeGame = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~Game()
    {
        Dispose(disposing: false);
    }
}