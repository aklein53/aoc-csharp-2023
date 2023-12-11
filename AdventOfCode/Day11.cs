using System.Text.RegularExpressions;

public class Day11 : BaseDay
{
    private readonly List<string> _input;

    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public class Point{

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
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

        for (int row = 0; row < _input.Count; row++)
        {
            if (getRow(row).All(c => c == '.'))
            {
                Console.WriteLine($"Expand row {row}");
                incrementGalaxiesByRow(galaxies, row);
            }
        }

        for (int col = 0; col < _input[0].Length; col++)
        {
            if (getColumn(col).All(c => c == '.'))
            {
                Console.WriteLine($"Expand column {col}");
                incrementGalaxiesByCol(galaxies, col); 
            }
        }

        long totalDistance = 0;
        var galaxyNum = 1;

        while(galaxies.Count > 0)
        {
            var galaxy = galaxies.Dequeue();

            var distances = galaxies.Select(g => manhattanDistance(galaxy, g));
            var i = 1;
            foreach(var distance in distances)
            {
                Console.WriteLine($"Galaxy {galaxyNum} to {galaxyNum + i++} distance: " + distance);
            }

            totalDistance += distances.Sum();
            galaxyNum++;
        }

        return new (totalDistance.ToString());
    }

    private void incrementGalaxiesByRow(IEnumerable<Point> galaxies, int row)
    {
        foreach (var galaxy in galaxies)
        {
            if (galaxy.Y > row) galaxy.Y++;
        }
    }

    private void incrementGalaxiesByCol(IEnumerable<Point> galaxies, int col)
    {
        foreach (var galaxy in galaxies)
        {
            if (galaxy.X > col) galaxy.X++;
        }
    }
    private int manhattanDistance(Point p1, Point p2)
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
        return new ("");
    }
}