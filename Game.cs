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
                if (_width < 1 || _width > 30)
                {
                    Console.WriteLine("Must be bigger than 0. And less than 31");
                    Console.ReadKey(true);
                    continue;
                }
                
                Console.Write("Height: ");
                _height = int.Parse(Console.ReadLine());
                if (_height < 1 || _height > 30)
                {
                    Console.WriteLine("Must be bigger than 0. And less than 31");
                    Console.ReadKey(true);
                    continue;
                }
                
                Console.Write($"Mines amount (max: {_width * _height}): ");
                _mines = int.Parse(Console.ReadLine());
                if (_mines < 0)
                {
                    Console.WriteLine("Must be positive.");
                    Console.ReadKey(true);
                    continue;
                }
                if (_mines > _width * _height)
                {
                    Console.WriteLine($"Can\'t be more than {_width} * {_height}.");
                    Console.ReadKey(true);
                    continue;
                }

                break;
            }
            catch (Exception)
            {
                Console.Write("Only Numbers");
                Console.ReadKey(true);
            }
        }

        mineCount = _mines;
        Program.Height = _height;
        Program.Width = _width;

        grid = new Cell[Program.Height, Program.Width];
    
        Console.SetWindowSize(Program.Width * 2 + 8, Program.Height + 5);
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

        Console.Write("   ");
        for (int i = 0; i < Program.Width; i++)
        {
            if (i % 2 == 0)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (i < 9)
                Console.Write($"{i + 1} ");
            else
                Console.Write(i + 1 + "");
            
            Console.ResetColor();
        }
        Console.WriteLine();

        Console.Write("  +");
        for (int i = 0; i < Program.Width; i++)
            Console.Write("--");
        Console.WriteLine();

        int _minesAround = 0;
        for (int y = 0; y < Program.Height; y++)
        {
            if (y % 2 == 0)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (y < 9)
                Console.Write($"{y + 1} ");
            else
                Console.Write($"{y + 1}");
            
            Console.ResetColor();
            Console.Write("|");

            for (int x = 0; x < Program.Width; x++)
            {
                _minesAround = grid[y, x].CountMinesAround(grid);

                grid[y, x].Show(_minesAround);
                //Console.Write("|");
            }

            //Console.WriteLine();
            //Console.Write("  +");
            //for (int i = 0; i < Program.Width; i++)
            //    Console.Write("--+");
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
                Console.Write("Too small or big X.");
                Console.ReadKey(true);
                return false;
            }
            Console.Write("y: ");
            y = int.Parse(Console.ReadLine()) - 1;
            if (y < 0 || y > Program.Height - 1)
            {
                Console.Write("Too small or big Y.");
                Console.ReadKey(true);
                return false;
            }
        }
        catch (Exception)
        {
            Console.Write("Only Numbers");
            Console.ReadKey(true);
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
                        Console.ReadKey(true);
                        
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
            Console.ReadKey(true);

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