using System.Text.RegularExpressions;

public class Day11 : BaseDay
{
    private readonly List<string> _input;

    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public class Point{

        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }
        public long X { get; set; }
        public long Y { get; set; }
    }

    public override ValueTask<string> Solve_1()
    {
        Queue<Point> galaxies = new();
        for (int row = 0; row < _input.Count; row++)
        {
            for (int col = 0; col < _input[0].Length; col++)
            {
                if (_input[row][col] == '#')
                {
                    galaxies.Enqueue(new Point(col, row));
                }
            }
        }

        for (int row = _input.Count - 1; row >= 0 ; row--)
        {
            if (getRow(row).All(c => c == '.'))
            {
                incrementGalaxiesByRow(galaxies, row);
            }
        }

        for (int col = _input[0].Length - 1; col >= 0 ; col--)
        {
            if (getColumn(col).All(c => c == '.'))
            {
                incrementGalaxiesByCol(galaxies, col); 
            }
        }

        long totalDistance = 0;

        while(galaxies.Count > 0)
        {
            var galaxy = galaxies.Dequeue();
            totalDistance += galaxies.Select(g => manhattanDistance(galaxy, g)).Sum();
        }

        return new (totalDistance.ToString());
    }

    private void incrementGalaxiesByRow(IEnumerable<Point> galaxies, int row, int increment = 1)
    {
        foreach (var galaxy in galaxies)
        {
            if (galaxy.Y > row) galaxy.Y += increment;
        }
    }

    private void incrementGalaxiesByCol(IEnumerable<Point> galaxies, int col, int increment = 1)
    {
        foreach (var galaxy in galaxies)
        {
            if (galaxy.X > col) galaxy.X += increment;
        }
    }
    private long manhattanDistance(Point p1, Point p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    private IEnumerable<char> getColumn(int col)
    {
        return _input.Select(row => row[col]);
    }

    private IEnumerable<char> getRow(int row)
    {
        return _input[row].AsEnumerable();
    }


    public override ValueTask<string> Solve_2()
    {
        Queue<Point> galaxies = new();
        for (int row = 0; row < _input.Count; row++)
        {
            for (int col = 0; col < _input[0].Length; col++)
            {
                if (_input[row][col] == '#')
                {
                    galaxies.Enqueue(new Point(col, row));
                }
            }
        }

        for (int row = _input.Count - 1; row >= 0 ; row--)
        {
            if (getRow(row).All(c => c == '.'))
            {
                incrementGalaxiesByRow(galaxies, row, 999_999);
            }
        }

        for (int col = _input[0].Length - 1; col >= 0 ; col--)
        {
            if (getColumn(col).All(c => c == '.'))
            {
                incrementGalaxiesByCol(galaxies, col, 999_999); 
            }
        }

        long totalDistance = 0;

        while(galaxies.Count > 0)
        {
            var galaxy = galaxies.Dequeue();
            totalDistance += galaxies.Select(g => manhattanDistance(galaxy, g)).Sum();
        }

        return new (totalDistance.ToString());
    }
}