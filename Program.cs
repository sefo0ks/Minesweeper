internal class Program
{
    public static int Width = 10, Height = 10;
    private static Game game = new();

    public static Random Rand = new();

    private static void Main(string[] args)
    {
        game.OnGameEnd += GameEnd;

        game.Start();
    }

    public static void GameEnd()
    {
        Console.WriteLine("asdasdf");
        game = new();
        game.Start();
    }
}