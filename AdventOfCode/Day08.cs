using System.Text.RegularExpressions;

public class Day08 : BaseDay
{
    private readonly List<string> _input;

    private static Dictionary<string, string> lefts = new();
    private static Dictionary<string, string> rights = new();
    private static Dictionary<char, Func<string, string>> stepFuncs = new()
    {
        ['L'] = (string currentNode) => lefts[currentNode],
        ['R'] = (string currentNode) => rights[currentNode]
    };

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        Regex nodeRegex = new Regex("(...) = \\((...), (...)\\)");
        var instructions = _input[0].ToCharArray();

        lefts.Clear();
        rights.Clear();
        foreach (var line in _input.Skip(2))
        {
            var regexMatch = nodeRegex.Match(line);
            var nodeName = regexMatch.Groups[1].Value;
            var leftNode = regexMatch.Groups[2].Value;
            var rightNode = regexMatch.Groups[3].Value;

            lefts[nodeName] = leftNode;
            rights[nodeName] = rightNode;
        }

        int stepCount = 0;
        int stepIndex = 0;
        string currentNode = "AAA";
        while(currentNode != "ZZZ")
        {
            var step = instructions[stepIndex];
            currentNode = stepFuncs[step](currentNode);
            stepIndex = (stepIndex + 1) % instructions.Length;
            stepCount++;
        }
        return new(stepCount.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        Regex nodeRegex = new Regex("(...) = \\((...), (...)\\)");
        var instructions = _input[0].ToCharArray();

        List<string> currentNodes = new();
        lefts.Clear();
        rights.Clear();
        foreach (var line in _input.Skip(2))
        {
            var regexMatch = nodeRegex.Match(line);
            var nodeName = regexMatch.Groups[1].Value;
            var leftNode = regexMatch.Groups[2].Value;
            var rightNode = regexMatch.Groups[3].Value;

            if (nodeName.EndsWith("A")) currentNodes.Add(nodeName);
            lefts[nodeName] = leftNode;
            rights[nodeName] = rightNode;
        }

        var periods = currentNodes.Select(x => FindPeriod(x, instructions)).ToList();

        
        return new(FindLCM(periods).ToString());
    }

    private long FindPeriod(string startingNode, char[] instructions)
    {
        long stepCount = 0;
        string currentNode = startingNode;
        int instructionIndex = 0;
        while (currentNode[2] != 'Z')
        {
            var step = instructions[instructionIndex];
            currentNode = stepFuncs[step](currentNode);
            stepCount++;
            instructionIndex = (instructionIndex + 1) % instructions.Length;
        }

        return stepCount;
    }

    private long FindLCM(List<long> longs)
    {
        if (longs.Count == 2) return FindLCM(longs[0], longs[1]);
        else return FindLCM(longs[0], FindLCM(longs[1..]));
    }

    private long FindLCM(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }

    private long GCD(long a, long b)
    {
        if (a == 0) return b;
        else if (b == 0) return a;
        else if (a > b) return GCD(a % b, b);
        else return GCD(b % a, a);
    }
}