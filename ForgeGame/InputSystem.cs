using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ForgeGame;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void EventCallback(ref EventType evt);

public static partial class InputSystemNative
{
    private const string DllName = "ForgeEngine.dll";

    [LibraryImport(DllName)]
    public static partial IntPtr inputsystem_pollevent(IntPtr inputSystem);

    [DllImport(DllName)]
    public static extern void inputsystem_registercallback(IntPtr inputSystem, EventCallback callback);
}

public sealed class InputSystem : IDisposable
{
    public Action<EventType> OnEventReceived;

    private IntPtr nativeInputSystem;
    private EventCallback eventCallback;

    public InputSystem(IntPtr nativePtr)
    {
        if (nativePtr == IntPtr.Zero)
            throw new InvalidOperationException("Native pointer for InputSystem is null.");
        nativeInputSystem = nativePtr;

        eventCallback = OnEvent;
        Console.WriteLine("C# Registering callback");
        InputSystemNative.inputsystem_registercallback(nativeInputSystem, eventCallback);
        Console.WriteLine("C# Registered callback");
    }

    private void OnEvent(ref EventType evt)
    {
        Console.WriteLine("Event Received");
        OnEventReceived?.Invoke(evt);
    }

    public Event? PollEvent()
    {
        IntPtr eventPtr = InputSystemNative.inputsystem_pollevent(nativeInputSystem);
        if (eventPtr == IntPtr.Zero)
            return null;

        return new Event(eventPtr);
    }

    private void Dispose(bool disposing)
    { }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~InputSystem()
    {
        Dispose(disposing: false);
    }
}

public enum EventType
{
    Closed, //!< The window requested to be closed (no data)
    Resized, //!< The window was resized (data in event.size)
    LostFocus, //!< The window lost the focus (no data)
    GainedFocus, //!< The window gained the focus (no data)
    TextEntered, //!< A character was entered (data in event.text)
    KeyPressed, //!< A key was pressed (data in event.key)
    KeyReleased, //!< A key was released (data in event.key)
    MouseWheelMoved, //!< The mouse wheel was scrolled (data in event.mouseWheel) (deprecated)
    MouseWheelScrolled, //!< The mouse wheel was scrolled (data in event.mouseWheelScroll)
    MouseButtonPressed, //!< A mouse button was pressed (data in event.mouseButton)
    MouseButtonReleased, //!< A mouse button was released (data in event.mouseButton)
    MouseMoved, //!< The mouse cursor moved (data in event.mouseMove)
    MouseEntered, //!< The mouse cursor entered the area of the window (no data)
    MouseLeft, //!< The mouse cursor left the area of the window (no data)
    JoystickButtonPressed, //!< A joystick button was pressed (data in event.joystickButton)
    JoystickButtonReleased, //!< A joystick button was released (data in event.joystickButton)
    JoystickMoved, //!< The joystick moved along an axis (data in event.joystickMove)
    JoystickConnected, //!< A joystick was connected (data in event.joystickConnect)
    JoystickDisconnected, //!< A joystick was disconnected (data in event.joystickConnect)
    TouchBegan, //!< A touch event began (data in event.touch)
    TouchMoved, //!< A touch moved (data in event.touch)
    TouchEnded, //!< A touch event ended (data in event.touch)
    SensorChanged, //!< A sensor value changed (data in event.sensor)

    Count //!< Keep last -- the total number of event types
}

public static partial class NativeEvent
{
    private const string DllName = "ForgeEngine.dll";

    [LibraryImport(DllName)]
    public static partial EventType event_geteventtype(IntPtr eventPtr);
}

public class Event
{
    private IntPtr nativeEvent;

    internal Event(IntPtr nativePtr)
    {
        if (nativePtr == IntPtr.Zero)
            throw new InvalidOperationException("Native pointer for Event is null.");
        nativeEvent = nativePtr;

    }

    public EventType GetEventType()
    {
        if (nativeEvent == IntPtr.Zero)
            throw new InvalidOperationException("Native pointer for Event is null.");
        return NativeEvent.event_geteventtype(nativeEvent);
    }
}