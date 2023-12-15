using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class Day14 : BaseDay
{
    private readonly string _input;

    public class Rock
    {
    }

    public class RoundRock : Rock { }

    public class CubeRock : Rock
    {
        public CubeRock(int row)
        {
            Row = row;
        }

        public int Row { get; set; }
    }

    public Day14()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var lineCount = _input.Split('\n').Length;

        var rockField = getRockField(_input);

        var weights = rockField.Select(x => getWeightOfColumn(x, lineCount));

        return new(weights.Sum().ToString());
    }

    Dictionary<string, List<List<Rock>>> rockFieldCache = new Dictionary<string, List<List<Rock>>>();
    private List<List<Rock>> getRockField(string inputString)
    {
        if (rockFieldCache.ContainsKey(inputString))
        {
            return rockFieldCache[inputString];
        }

        var input = inputString.Split('\n').ToList();
        List<List<Rock>> rockField = new();
        Enumerable.Range(0, input[0].Length).ToList().ForEach(x => rockField.Add(new()));

        for (int row = 0; row < input.Count; row++)
        {
            for (int col = 0; col < input[0].Length; col++)
            {
                switch (input[row][col])
                {
                    case '#':
                        rockField[col].Add(new CubeRock(row));
                        break;
                    case 'O':
                        rockField[col].Add(new RoundRock());
                        break;
                    default:
                        continue;
                }
            }
        }

        rockFieldCache[inputString] = rockField;
        return rockField;
    }

    private int getWeightOfColumn(List<Rock> column, int totalRows)
    {
        int weight = 0;
        int currentRow = 0;
        foreach (var item in column)
        {
            if (item is CubeRock cr)
            {
                currentRow = cr.Row + 1;
            }
            if (item is RoundRock)
            {
                weight += totalRows - currentRow;
                currentRow++;
            }
        }

        return weight;
    }

    Dictionary<List<List<Rock>>, string> inputCache = new();
    private string getNextInput(List<List<Rock>> rockLists, int width)
    {
        if (inputCache.ContainsKey(rockLists))
        {
            return inputCache[rockLists];
        }

        List<string> output = new();
        foreach(var rockList in rockLists)
        {
            char[] fieldRow = new string('.', width).ToCharArray();

            int currentRow = 0;
            foreach(var rock in rockList)
            {
                if (rock is CubeRock cr)
                {
                    currentRow = cr.Row;
                    fieldRow[currentRow] = '#';
                    currentRow++;
                }
                if (rock is RoundRock)
                {
                    fieldRow[currentRow++] = 'O';
                }
            }
            
            output.Add(new string(fieldRow.Reverse().ToArray()));
        }

        var joinedString = string.Join('\n', output);
        inputCache[rockLists] = joinedString;
        return joinedString;
    }

    public override ValueTask<string> Solve_2()
    {
        var lineCount = _input.Split('\n').Length;

        var input = _input;
        List<List<Rock>> rockField;
        List<string> inputLoop = new();
        HashSet<string> inputHashSet = new();
        inputLoop.Add(input);
        inputHashSet.Add(input);
        var foundLoop = false;

        Dictionary<string, string> nextInput = new Dictionary<string, string>();
        for (int spinCycle = 0; spinCycle < 1_000_000_000; spinCycle++)
        {
            if (spinCycle % 1_000_000 == 0) Console.WriteLine($"Spin cycle: {spinCycle}");
            if (nextInput.ContainsKey(input))
            {
                input = nextInput[input];
                continue;
            }

            var inputAtStartOfSpinCycle = input;
            
            for (int i = 0; i < 4; i++)
            {
                lineCount = input.Split('\n').Length;
                var height = lineCount;
                rockField = getRockField(input);
                input = getNextInput(rockField, height);
            }
            var inputAtEndOfSpinCycle = input;
            if (inputHashSet.Contains(input))
            {
                foundLoop = true;
                var loopStartIndex = inputLoop.IndexOf(input);
                var finalIndex = loopStartIndex + (1_000_000_000 - (spinCycle+1)) % (inputLoop.Count - loopStartIndex);
                input = inputLoop[finalIndex];
                break;
            }
            else
            {
                inputLoop.Add(input);
                inputHashSet.Add(input);
            }
            if (!nextInput.ContainsKey(inputAtStartOfSpinCycle)) nextInput[inputAtStartOfSpinCycle] = inputAtEndOfSpinCycle;

            if (inputAtStartOfSpinCycle == inputAtEndOfSpinCycle) break;
        }

        var rows = input.Split('\n');
        var weight = 0;
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[0].Length; col++)
            {
                if (rows[row][col] == 'O') weight += rows.Length - row;
            }
        }

        return new(weight.ToString());
    }
}