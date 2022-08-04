public class Cell
{
    private char mineChar = '*';
    private int y, x, minesAround;
    private bool isSafe;

    public int Y { get => y; }
    public int X { get => x; }
    public bool IsSafe { get => isSafe; set => isSafe = value; }
    public bool IsRevealed { get; set; }

    public Cell(int _y, int _x, bool _isSafe)
    {
        y = _y;
        x = _x;

        if (_y < 0)
            y = 0;
        if (_x < 0)
            x = 0;

        isSafe = _isSafe;

        IsRevealed = false;
    }
    
    public void Show(int _minesAround)
    {
        if (IsRevealed)
        {
            if (isSafe)
            {
                if (minesAround > 0)
                    Console.Write(minesAround);
                else
                    Console.Write("██");
            }
            else
            {
                Console.Write(mineChar);
            }
        }
        else
        {
            Console.Write(" ");
        }
    }
    public void Update(Cell[,] _grid, int _y, int _x)
    {
        if (IsRevealed)
            return;

        IsRevealed = true;
        
        for (int yOff = -1; yOff <= 1; yOff++)
        {
            for (int xOff = -1; xOff <= 1; xOff++)
            {
                if (xOff == 0 && yOff == 0)
                    continue;

                if (_y + yOff < 0 || _x + xOff < 0 ||
                    _y + yOff > Program.Height - 1 || _x + xOff >  Program.Width - 1)
                    continue;

                if (_grid[_y + yOff, _x + xOff].IsSafe)
                {
                    if (_grid[_y, _x].CountMinesAround(_grid) == 0)
                    {
                        _grid[_y + yOff, _x + xOff].Update(_grid, _y + yOff, _x + xOff);
                        _grid[_y + yOff, _x + xOff].IsRevealed = true;
                    }
                }
            }
        }
    }
    public int CountMinesAround(Cell[,] _grid)
    {
        int _mines = 0;
        
        for (int yOff = -1; yOff <= 1; yOff++)
        {
            for (int xOff = -1; xOff <= 1; xOff++)
            {
                if (xOff == 0 && yOff == 0)
                    continue;

                if (y + yOff < 0 || x + xOff < 0 ||
                    y + yOff > Program.Height - 1 || x + xOff >  Program.Width - 1)
                    continue;

                if (_grid[y + yOff, x + xOff].IsSafe == false)
                    _mines++;
            }
        }

        minesAround = _mines;
        return minesAround;
    }
}