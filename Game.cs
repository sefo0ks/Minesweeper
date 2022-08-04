class Game
{
    public delegate void GameEnded();
    public event GameEnded? OnGameEnd;

    private Cell[,] grid;
    private int mineCount;

    public void Start()
    {
        Setup();

        for (int y = 0; y < Program.Height; y++)
        {
            for (int x = 0; x < Program.Width; x++)
            {
                grid[y, x] = new(y, x, true);
            }
        }

        PlaceMines();

        DoLoop();
    }

    private void Setup()
    {
        int _width = 0;
        int _height = 0;
        int _mines = 0;

        while(true)
        {
            try
            {
                Console.Clear();

                Console.Write("Width: ");
                _width = int.Parse(Console.ReadLine());
                if (_width < 0 || _width > 30)
                {
                    Console.WriteLine("Must be positive. And less than 31");
                    Console.ReadKey();
                    continue;
                }
                
                Console.Write("Height: ");
                _height = int.Parse(Console.ReadLine());
                if (_height < 0 || _height > 30)
                {
                    Console.WriteLine("Must be positive. And less than 31");
                    Console.ReadKey();
                    continue;
                }
                
                Console.Write($"Mines amount (max: {_width * _height}): ");
                _mines = int.Parse(Console.ReadLine());
                if (_mines < 0)
                {
                    Console.WriteLine("Must be positive.");
                    Console.ReadKey();
                    continue;
                }
                if (_mines > _width * _height)
                {
                    Console.WriteLine("Can\'t be more than (width * height).");
                    Console.ReadKey();
                    continue;
                }

                break;
            }
            catch (Exception)
            {
                Console.Write("Only Numbers");
                Console.ReadKey();
            }
        }

        mineCount = _mines;
        Program.Height = _height;
        Program.Width = _width;

        grid = new Cell[Program.Height, Program.Width];
    }

    private void PlaceMines()
    {
        int _minesRemainToPlace = mineCount;

        while (_minesRemainToPlace > 0)
        {
            int _x = Program.Rand.Next(0, Program.Width);
            int _y = Program.Rand.Next(0, Program.Height);

            while (grid[_y, _x].IsSafe == false)
            {
                _x = Program.Rand.Next(0, Program.Width);
                _y = Program.Rand.Next(0, Program.Height);
            }

            grid[_y, _x].IsSafe = false;

            _minesRemainToPlace--;
        }
    }

    private void DoLoop()
    {
        int _x = 0, _y = 0;
        bool run = true;
        while (run)
        {
            Console.Clear();
            DrawGrid();

            if (!GetInput(out _x, out _y))
                continue;
            
            grid[_y, _x].Update(grid, _y, _x);
            CheckForEnd(out run);
        }
    }

    private void DrawGrid()
    {
        Console.SetCursorPosition(0, 0);
        Console.CursorVisible = false;

        Console.Write("  ");
        for (int i = 0; i < Program.Width; i++)
        {
            if (i == 9)
                Console.Write(" ");
            if (i < 9)
                Console.Write($" {i + 1} ");
            else
                Console.Write(i + 1 + " ");
        }
        Console.WriteLine();

        Console.Write("  +");
        for (int i = 0; i < Program.Width; i++)
            Console.Write("--+");
        Console.WriteLine();

        int _minesAround = 0;
        for (int y = 0; y < Program.Height; y++)
        {
            if (y < 9)
                Console.Write($"{y + 1} |");
            else
                Console.Write($"{y + 1}|");

            for (int x = 0; x < Program.Width; x++)
            {
                _minesAround = grid[y, x].CountMinesAround(grid);

                grid[y, x].Show(_minesAround);
                if (_minesAround == 0 && grid[y, x].IsRevealed && grid[y, x].IsSafe)
                    Console.Write("|");
                else
                    Console.Write(" |");
            }

            Console.WriteLine();
            Console.Write("  +");
            for (int i = 0; i < Program.Width; i++)
                Console.Write("--+");
            Console.WriteLine();
        }
    }

    private bool GetInput(out int x, out int y)
    {
        x = 0;
        y = 0;

        try
        {
            Console.Write("x: ");
            x = int.Parse(Console.ReadLine()) - 1;
            if (x < 0 || x > Program.Width - 1)
            {
                Console.WriteLine("Too small or big X.");
                Console.ReadKey();
                return false;
            }
            Console.Write("y: ");
            y = int.Parse(Console.ReadLine()) - 1;
            if (y < 0 || y > Program.Height - 1)
            {
                Console.WriteLine("Too small or big Y.");
                Console.ReadKey();
                return false;
            }
        }
        catch (Exception)
        {
            Console.Write("Only Numbers");
            Console.ReadKey();
            return false;
        }

        return true;
    }

    public void CheckForEnd(out bool run)
    {
        int revealedCount = 0;
        for (int y = 0; y < Program.Height; y++)
        {
            for (int x = 0; x < Program.Width; x++)
            {
                if (grid[y, x].IsRevealed)
                {
                    if (!grid[y, x].IsSafe == false)
                        revealedCount++;
                    else
                    {
                        for (int _y = 0; _y < Program.Height; _y++)
                        {
                            for (int _x = 0; _x < Program.Width; _x++)
                            {
                                grid[_y, _x].IsRevealed = true;
                            }
                        }
                        Console.Clear();
                        DrawGrid();
                        Console.WriteLine("YOU LOST!");
                        Console.ReadKey();
                        
                        End();
                        run = false;
                        return;
                    }
                }
            }
        }
        
        if (revealedCount == (Program.Height * Program.Width) - mineCount)
        {
            for (int y = 0; y < Program.Height; y++)
            {
                for (int x = 0; x < Program.Width; x++)
                {
                    grid[y, x].IsRevealed = true;
                }
            }
            Console.Clear();
            DrawGrid();
            Console.WriteLine("YOU WON!");
            Console.ReadKey();

            End();
            run = false;
            return;
        }

        run = true;
    }
    private void End()
    {
        OnGameEnd?.Invoke();
    }
}