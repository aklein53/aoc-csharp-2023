using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

public class Day16 : BaseDay
{
    private readonly List<string> _input;

    public Day16()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        return new(getEnergizedTiles(new Ray(-1, 0, Direction.Right)).ToString());
    }

    private int getEnergizedTiles(Ray start)
    {
        List<Ray> rays = new();
        rays.Add(start);

        HashSet<Ray> visited = new();
        HashSet<(int x, int y)> energized = new();

        while (rays.Count > 0)
        {
            rays = rays.SelectMany(step).Where(x => !visited.Contains(x)).ToList();
            rays.ForEach(x => energized.Add((x.x, x.y)));
            visited.UnionWith(rays);
        }

        return energized.Count;
    }

    private bool isInbounds(int x, int y)
    {
        return x >= 0 && x < _input[0].Length && y >= 0 && y < _input.Count;
    }

    private List<Ray> step(Ray ray)
    {
        var newX = ray.direction switch { Direction.Left => ray.x - 1, Direction.Right => ray.x + 1, _ => ray.x };
        var newY = ray.direction switch { Direction.Up => ray.y - 1, Direction.Down => ray.y + 1, _ => ray.y };

        if (!isInbounds(newX, newY)) return new();

        var character = _input[newY][newX];
        if (character == '.')
        {
            return new()
            {
                ray with { x= newX, y = newY }
            };
        }
        else if (character == '/')
        {
            return new List<Ray>()
            {
                ray.direction switch
                {
                    Direction.Up => ray with { x = newX, y = newY, direction = Direction.Right },
                    Direction.Right => ray with { x = newX, y = newY, direction = Direction.Up },
                    Direction.Down => ray with { x = newX, y = newY, direction = Direction.Left },
                    Direction.Left => ray with { x = newX, y = newY, direction = Direction.Down },
                }
                
            };
        }
        else if (character == '\\')
        {
            return new List<Ray>()
            {
                ray.direction switch
                {
                    Direction.Up => ray with { x = newX, y = newY, direction = Direction.Left },
                    Direction.Left => ray with { x = newX, y = newY, direction = Direction.Up },
                    Direction.Down => ray with { x = newX, y = newY, direction = Direction.Right },
                    Direction.Right => ray with { x = newX, y = newY, direction = Direction.Down },
                }
            };
        }
        else if (character == '-')
        {
            if (ray.direction is Direction.Up or Direction.Down)
            {
                return new List<Ray>()
                {
                    ray with { x = newX, y = newY, direction = Direction.Left },
                    ray with { x = newX, y = newY, direction = Direction.Right }
                };
            }
            else
            {
                return new List<Ray>()
                {
                    ray with { x = newX, y = newY, direction = ray.direction },
                };
            }
        }
        else if (character == '|')
        {
            if (ray.direction is Direction.Left or Direction.Right)
            {
                return new List<Ray>()
                {
                    ray with { x = newX, y = newY, direction = Direction.Up },
                    ray with { x = newX, y = newY, direction = Direction.Down }
                };
            }
            else
            {
                return new List<Ray>()
                {
                    ray with { x = newX, y = newY, direction = ray.direction }
                };
            }
        }

        throw new Exception("Shouldn't get here");
    }

    public override ValueTask<string> Solve_2()
    {
        int currMax = 0;
        for (int i = 0; i < _input.Count; i++)
        {
            var leftMax = getEnergizedTiles(new Ray(-1, i, Direction.Right));
            var rightMax = getEnergizedTiles(new Ray(_input[0].Length, i, Direction.Left));
            currMax = Math.Max(currMax, Math.Max(leftMax, rightMax));
        }

        for (int i = 0; i < _input[0].Length; i++)
        {
            var topMax = getEnergizedTiles(new Ray(i, -1, Direction.Down));
            var bottomMax = getEnergizedTiles(new Ray(i, _input.Count, Direction.Up));
            currMax = Math.Max(currMax, Math.Max(topMax, bottomMax));
        }

        return new(currMax.ToString());
    }

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    record Ray(int x, int y, Direction direction);
}