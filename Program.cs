internal class Program
{
    public static int Width = 10, Height = 10;
    private static Game game = new();

    public static Random Rand = new();

    private static void Main(string[] args)
    {
        game.OnGameEnd += GameEnd;
        while (true)
        {
            game.Start();
        }
    }

    public static void GameEnd()
    {
        game = new();
        game.Start();
    }
}