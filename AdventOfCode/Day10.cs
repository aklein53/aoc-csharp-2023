using System.Text.RegularExpressions;

public class Day10 : BaseDay
{
    private readonly List<string> _input;

    public Day10()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public record Point(int X, int Y);

    public override ValueTask<string> Solve_1()
    {
        var count = getLoopPoints().Count;

        return new((count / 2).ToString());
    }

    private HashSet<Point> getLoopPoints()
    {
        Dictionary<Point, (Point Conn1, Point Conn2)> connections = new();

        var width = _input[0].Length;
        var height = _input.Count;
        Point? startPoint = null;
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (_input[row][col] == '.') continue;
                if (_input[row][col] == 'S')
                {
                    startPoint = new Point(col, row);
                    var startChar = FindStartingPipeChar(startPoint);
                    var links = getLinks(startChar);
                    connections.Add(
                        new Point(col, row),
                        (
                            new Point(col + links.Shift1.X, row + links.Shift1.Y),
                            new Point(col + links.Shift2.X, row + links.Shift2.Y)));
                }
                else
                {
                    var links = getLinks(_input[row][col]);
                    connections.Add(
                        new Point(col, row),
                        (
                            new Point(col + links.Shift1.X, row + links.Shift1.Y),
                            new Point(col + links.Shift2.X, row + links.Shift2.Y)));
                }
            }
        }

        Point currentPoint = connections[startPoint].Conn1;
        Point prevPoint = startPoint;
        HashSet<Point> pointsInLoop = [startPoint];
        while (currentPoint != startPoint)
        {
            pointsInLoop.Add(currentPoint);
            var nextPoint = connections[currentPoint].Conn1 == prevPoint ? connections[currentPoint].Conn2 : connections[currentPoint].Conn1;
            prevPoint = currentPoint;
            currentPoint = nextPoint;
        }

        return pointsInLoop;
    }

    private void DrawField(HashSet<Point> loop, HashSet<Point> outsidePoints)
    {
        var height = _input.Count;
        var width = _input[0].Length;

        Dictionary<char, char> charReplacements = new() { ['F'] = '┌', ['J'] = '┘', ['7'] = '┐', ['L'] = '└', ['-'] = '─', ['|'] = '│', ['S'] = 'S' };
        Console.Clear();
        for (int row =0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var point = new Point(col, row);
                if (loop.Contains(point)) Console.Write(charReplacements[_input[row][col]]);
                else if (outsidePoints.Contains(point))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("+");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("+");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.WriteLine();
        }

        for (int i = 0; i < 100; i++)
            Console.WriteLine();
    }

    private ((int X, int Y) Shift1, (int X, int Y) Shift2) getLinks(char pipeChar)
    {
        return pipeChar switch
        {
            '|' => ((0, -1), (0, 1)),
            '-' => ((-1, 0), (1, 0)),
            'F' => ((0, 1), (1, 0)),
            'J' => ((0, -1), (-1, 0)),
            'L' => ((0, -1), (1, 0)),
            '7' => ((0, 1), (-1, 0)),
        };
    }

    private HashSet<char> southChars = new HashSet<char> { 'F', '|', '7'};
    private HashSet<char> northChars = new HashSet<char> { '|', 'J', 'L' };
    private HashSet<char> westChars = new HashSet<char> { 'J', '7', '-' };
    private HashSet<char> eastChars = new HashSet<char> { 'F', 'L', '-' };

    private Func<Point, bool> pointHasNorth => (point) => point.Y > 0 && charHasSouth(_input[point.Y - 1][point.X]);
    private Func<Point, bool> pointHasSouth => (point) => point.Y < _input.Count - 1 && charHasNorth(_input[point.Y + 1][point.X]);
    private Func<Point, bool> pointHasEast => (point) => point.X > 0 && charHasWest(_input[point.Y][point.X + 1]);
    private Func<Point, bool> pointHasWest => (point) => point.X < _input[0].Length - 1 && charHasEast(_input[point.Y][point.X - 1]);

    private Func<char, bool> charHasNorth => (x) => northChars.Contains(x);
    private Func<char, bool> charHasSouth => (x) => southChars.Contains(x);
    private Func<char, bool> charHasEast => (x) => eastChars.Contains(x);
    private Func<char, bool> charHasWest => (x) => westChars.Contains(x);

    private char FindStartingPipeChar(Point start)
    {
        return (pointHasNorth(start), pointHasSouth(start), pointHasEast(start), pointHasWest(start)) switch
        {
            (true, true, false, false) => '|',
            (true, false, true, false) => 'L',
            (true, false, false, true) => 'J',
            (false, true, true, false) => 'F',
            (false, true, false, true) => '7',
            (false, false, true, true) => '-',
            _ => throw new Exception("Bad start location")
        };
    }
    
    public override ValueTask<string> Solve_2()
    {
        var loopPoints = getLoopPoints();

        char[,] enlargedField = new char[_input[0].Length * 2, _input.Count * 2];

        for (int row = 0; row < _input.Count; row++)
        {
            for (int col = 0; col < _input[0].Length; col++)
            {
                Point currPoint = new(col, row);
                char currChar = loopPoints.Contains(currPoint) ? _input[row][col] : '.';
                if (currChar == 'S')
                {
                    currChar = FindStartingPipeChar(currPoint);
                }
                enlargedField[col * 2, row * 2] = currChar;
                enlargedField[col * 2 + 1, row * 2 + 1] = '.';
                enlargedField[col * 2, row * 2 + 1] = '.';
                enlargedField[col * 2 + 1, row * 2] = '.';

                if (charHasEast(currChar))
                    enlargedField[col * 2 + 1, row * 2] = '-';
                if (charHasSouth(currChar))
                    enlargedField[col * 2, row * 2 + 1] = '|';
            }
        }

        var enlargedOutsidePoints = getOutsidePoints(enlargedField);
        var outsidePoints = enlargedOutsidePoints.Where(x => x.X % 2 == 0 && x.Y % 2 == 0).Select(x => new Point(x.X / 2, x.Y / 2)).ToHashSet();

        int outsideCount = 0;
        foreach(var point in enlargedOutsidePoints)
        {
            if (point.X  % 2 == 0 && point.Y % 2 == 0)
            {
                outsideCount++;
            }
        }

        var totalCount = _input.Count * _input[0].Length;

        DrawField(loopPoints, outsidePoints);

        return new(((totalCount - outsideCount) - loopPoints.Count).ToString());
    }

    private List<Point> getOutsidePoints(char[,] field)
    {
        Queue<Point> toProcess = new ();
        HashSet<Point> outside = new ();

        for (int i = 0; i < field.GetLength(0); i++)
        {
            toProcess.Enqueue(new Point(i, 0));
            toProcess.Enqueue(new Point(i, field.GetLength(1) - 1));
        }

        for (int i = 0; i < field.GetLength(1); i++)
        {
            toProcess.Enqueue(new Point(0, i));
            toProcess.Enqueue(new Point(field.GetLength(0) - 1, i));
        }

        while(toProcess.Count > 0)
        {
            var point = toProcess.Dequeue();

            if (field[point.X, point.Y] == '.') outside.Add(point);
            else continue;
            var adjacentPoints = getAdjacent(point, field.GetLength(0), field.GetLength(1));
            foreach(var adjacentPoint in adjacentPoints)
            {
                if (!toProcess.Contains(adjacentPoint) && !outside.Contains(adjacentPoint))
                    toProcess.Enqueue(adjacentPoint);
            }
        }

        return outside.ToList();
    }

    private List<Point> getAdjacent(Point point, int width, int height)
    {
        var points = new List<Point>();

        if (point.X >= 1) points.Add(new Point(point.X - 1, point.Y));
        if (point.X < width - 1) points.Add(new Point(point.X + 1, point.Y));
        if (point.Y >= 1) points.Add(new Point(point.X, point.Y - 1));
        if (point.Y < height - 1) points.Add(new Point(point.X, point.Y + 1));

        return points;
    }
}