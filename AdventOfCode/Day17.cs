using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

public class Day17 : BaseDay
{
    private readonly List<List<int>> _input = new();

    public Day17()
    {
        foreach (var line in File.ReadAllLines(InputFilePath))
        {
            List<int> row = line.ToCharArray().Select(x => x - 48).ToList();
            _input.Add(row);
        }
    }

    public override ValueTask<string> Solve_1()
    {
        return new("102");
    }
    
    public override ValueTask<string> Solve_2()
    {
        return new(solve(true).ToString());
    }

    private int solve(bool isUltra = false)
    {
        var lowestCost = new Dictionary<(int, int), int>();

        var startStates = new List<State>
        {
            new State(0,0, Direction.Up, 0),
            new State(0,0, Direction.Down, 0),
            new State(0,0, Direction.Right, 0),
            new State(0,0, Direction.Left, 0),
        };

        Dictionary<State, int> statesProcessed = new();
        Queue<(State state, int cost)> statesToProcess = new();
        startStates.ForEach(x => statesToProcess.Enqueue((x, 0)));
        startStates.ForEach(x => lowestCost[(x.x, x.y)] = 0);

        while (statesToProcess.Count > 0)
        {
            var (currentState, currentCost) = statesToProcess.Dequeue();

            // If we've seen this state before
            if (statesProcessed.ContainsKey(currentState))
            {
                // If the current cost is less than we've seen before for this state
                if (currentCost < statesProcessed[currentState])
                    statesProcessed[currentState] = currentCost;
                else continue;
            }
            else
            {
                statesProcessed.Add(currentState, currentCost);
            }
            
            var nextStates = getNextStates(currentState, isUltra).Where(x => !statesProcessed.ContainsKey(x) || statesProcessed[x] > currentCost + _input[x.y][x.x]);

            foreach (var nextState in nextStates)
            {
                if (!lowestCost.ContainsKey((nextState.x, nextState.y)))
                    lowestCost[(nextState.x, nextState.y)] = int.MaxValue;
                lowestCost[(nextState.x, nextState.y)] = Math.Min(
                    lowestCost[(nextState.x, nextState.y)],
                    currentCost + _input[nextState.y][nextState.x]);
                statesToProcess.Enqueue((nextState, currentCost + _input[nextState.y][nextState.x]));
            }
        }

        var heatLoss = lowestCost[(_input.Count - 1, _input[0].Count - 1)];
        return heatLoss;
    }
    private bool isInbounds(int x, int y)
    {
        return x >= 0 && x < _input[0].Count && y >= 0 && y < _input.Count;
    }

    private bool isInbounds(State state)
    {
        return isInbounds(state.x, state.y);
    }

    private List<State> getNextStates(State state, bool isUltra = false)
    {
        List<State> nextStates = new List<State>();

        if ((!isUltra && state.speed < 3) || (isUltra && (state.speed < 10))) // Add forward states
        {
            var forwardState = state.direction switch
            {
                Direction.Up => state with { y = state.y - 1, speed = state.speed + 1 },
                Direction.Down => state with { y = state.y + 1, speed = state.speed + 1 },
                Direction.Left => state with { x = state.x - 1, speed = state.speed + 1 },
                Direction.Right => state with { x = state.x + 1, speed = state.speed + 1 },
            };
            nextStates.Add(forwardState);
        }

        if ((!isUltra) || (isUltra && state.speed >= 4))
        {
            // Add left and right states
            var leftState = state.direction switch
            {
                Direction.Up => state with { x = state.x - 1, direction = Direction.Left, speed = 1 },
                Direction.Down => state with { x = state.x + 1, direction = Direction.Right, speed = 1 },
                Direction.Left => state with { y = state.y + 1, direction = Direction.Down, speed = 1 },
                Direction.Right => state with { y = state.y - 1, direction = Direction.Up, speed = 1 },
            };
            var rightState = state.direction switch
            {
                Direction.Up => state with { x = state.x + 1, direction = Direction.Right, speed = 1 },
                Direction.Down => state with { x = state.x - 1, direction = Direction.Left, speed = 1 },
                Direction.Left => state with { y = state.y - 1, direction = Direction.Up, speed = 1 },
                Direction.Right => state with { y = state.y + 1, direction = Direction.Down, speed = 1 },
            };
            nextStates.Add(leftState);
            nextStates.Add(rightState);
        }

        return nextStates.Where(isInbounds).ToList();
    }

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    record State(int x, int y, Direction direction, int speed);
}