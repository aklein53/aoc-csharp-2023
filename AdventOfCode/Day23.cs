public class Day23 : BaseDay
{
    private readonly List<string> _input = new();

    public Day23()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        return new(solve().ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        return new(solve(true).ToString());
    }
    
    private int solve(bool climbSlopes = false)
    {
        // Work backward from end tile
        var endPoint = new Point(_input.Last().IndexOf('.'), _input.Count - 1, new List<Point>());
        var startPoint = new Point(_input.First().IndexOf('.'), 0, new List<Point>());
        
        List<Point> nextPoints = new() { endPoint };
        int stepCount = -1;
        int lengthOfLongest = 0;
        while (nextPoints.Count > 0)
        {
            nextPoints = nextPoints.SelectMany(x=> getNextPoints(x, climbSlopes)).ToList();
            if (nextPoints.Any(x => x.x == startPoint.x && x.y == startPoint.y))
                lengthOfLongest = Math.Max(lengthOfLongest, stepCount + 2);
            stepCount++;
        }

        return lengthOfLongest;
    }

    private List<Point> getNextPoints(Point currPoint, bool climbSlopes = false)
    {
        List<Point> nextPoints = new List<Point>
        {
            currPoint with { x = currPoint.x - 1, prevPoints = new()},
            currPoint with { x = currPoint.x + 1, prevPoints = new()},
            currPoint with { y = currPoint.y - 1, prevPoints = new()},
            currPoint with { y = currPoint.y + 1, prevPoints = new()},
        };

        foreach (var point in nextPoints)
        {
            point.prevPoints.AddRange(currPoint.prevPoints);
            point.prevPoints.Add(currPoint);
        }

        return nextPoints.Where(x => isValidNextPoint(x, currPoint, climbSlopes)).ToList();
    }
    
    private bool isValidNextPoint(Point nextPoint, Point currPoint, bool climbSlopes = false)
    {
        // No going backwards
        if (nextPoint.prevPoints.Any(prevPoint => prevPoint.x == nextPoint.x && prevPoint.y == nextPoint.y)) return false;
        
        // No going out of bounds
        if (nextPoint.x < 0 || nextPoint.y < 0 || nextPoint.x >= _input[0].Length || nextPoint.y >= _input.Count)
            return false;

        // Stay on the trail
        if (_input[nextPoint.y][nextPoint.x] == '#') return false;
        
        // Only go up slopes
        var charAtNextPoint = charAt(nextPoint);

        if (!climbSlopes)
        {
            if (nextPoint.y < currPoint.y && !(charAtNextPoint is 'v' or '.')) return false;
            if (nextPoint.y > currPoint.y && !(charAtNextPoint is '^' or '.')) return false;
            if (nextPoint.x < currPoint.x && !(charAtNextPoint is '>' or '.')) return false;
            if (nextPoint.x > currPoint.x && !(charAtNextPoint is '<' or '.')) return false;
        }
        else
        {
            if (nextPoint.y < currPoint.y && !(charAtNextPoint is 'v' or '^' or '.')) return false;
            if (nextPoint.y > currPoint.y && !(charAtNextPoint is '^' or 'v'or '.')) return false;
            if (nextPoint.x < currPoint.x && !(charAtNextPoint is '>' or '<'or '.')) return false;
            if (nextPoint.x > currPoint.x && !(charAtNextPoint is '<' or '>'or '.')) return false;
        }

        return true;
    }

    private char charAt(Point point)
    {
        return _input[point.y][point.x];
    }

    record Point(int x, int y, List<Point> prevPoints);
}