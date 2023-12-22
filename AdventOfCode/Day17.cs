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
        return new(solve().ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        return new(solve(true).ToString());
    }
    
    private int solve(bool isUltra = false)
    {
        var targetX = _input[0].Count - 1;
        var targetY = _input.Count - 1;
        
        PriorityQueue<State, int> toProcess = new PriorityQueue<State, int>();
        toProcess.Enqueue(new State(0,0, Orientation.Horizontal), 0);
        toProcess.Enqueue(new State(0,0, Orientation.Vertical), 0);

        Dictionary<State, int> minCost = new();

        while (toProcess.Count > 0)
        {
            toProcess.TryDequeue(out State currState, out int currCost);

            if (currState.x == targetX && currState.y == targetY)
            {
                return currCost;
            }
            
            var nextStates = getNextStates(currState, isUltra);

            foreach (var nextState in nextStates)
            {
                var nextCost = getCost((currState.x, currState.y), (nextState.x, nextState.y));

                if (minCost.ContainsKey(nextState) && minCost[nextState] <= currCost + nextCost)
                    continue;
                
                minCost[nextState] = currCost + nextCost;
                
                toProcess.Enqueue(nextState, currCost + nextCost);
            }
        }

        throw new Exception("Couldn't find a path to the target");
    }
    private bool isInbounds(int x, int y)
    {
        return x >= 0 && x < _input[0].Count && y >= 0 && y < _input.Count;
    }

    private bool isInbounds(State state)
    {
        return isInbounds(state.x, state.y);
    }

    private int getCost((int x, int y) start, (int x, int y) end)
    {
        if (start.x != end.x && start.y != end.y)
            throw new Exception("Straight lines only");
        int cost = 0;

        if (start.y == end.y)
        {
            int currX = start.x;
            while (currX != end.x)
            {
                currX = currX < end.x ? currX + 1 : currX - 1;
                cost += getCost(currX, start.y);
            }
        }
        else if (start.x == end.x)
        {
            int currY = start.y;
            while (currY != end.y)
            {
                currY = currY < end.y ? currY + 1 : currY - 1;
                cost += getCost(start.x, currY);
            }
        }

        return cost;
    }
    
    private int getCost(int x, int y)
    {
        return _input[y][x];
    }

    private Func<int, State, State>[] newStateFuncs = {
        (inc, state) => state with { x = state.x + inc, orientation = Orientation.Horizontal},
        (inc, state) => state with { y = state.y + inc, orientation = Orientation.Vertical},
    };
    
    private List<State> getNextStates(State state, bool isUltra = false)
    {
        List<State> nextStates = new List<State>();
        
        Func<int, State, State> newStateFunc = newStateFuncs[(int)state.orientation];

        var minSteps = isUltra ? 4 : 1;
        var maxSteps = isUltra ? 10 : 3;

        for (int i = minSteps; i <= maxSteps; i++)
        {
            var leftState = newStateFunc(-i, state);
            var rightState = newStateFunc(i, state);
            
            nextStates.Add(leftState);
            nextStates.Add(rightState);
        }

        return nextStates.Where(isInbounds).ToList();
    }

    enum Orientation
    {
        Vertical,
        Horizontal
    }

    record State(int x, int y, Orientation orientation);
}