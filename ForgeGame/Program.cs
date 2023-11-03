namespace ForgeGame;

public abstract class Program
{
    public static void Main(string[] args)
    {
        Game game = new Game("ForgeGame");

        game.GetInputSystem().OnEventReceived += OnEventReceived;
        
        game.Run();
    }

    private static void OnEventReceived(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.KeyPressed:
                Console.WriteLine("Key pressed");
                break;
        }
    }
}