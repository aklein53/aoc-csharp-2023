using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class Day12 : BaseDay
{
    public class ConditionRecord
    {
        public string Springs { get; set; }
        public List<int> GroupCounts { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Springs.GetHashCode(), string.Join(',', GroupCounts).GetHashCode());
        }
    }

    private readonly List<string> _input;
    private readonly List<ConditionRecord> conditionRecords = new List<ConditionRecord>();
    private Regex groupRegex = new Regex("#+");

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
        foreach(var line in _input)
        {
            ConditionRecord cr = new();
            var split = line.Split(' ');
            cr.Springs = duplicate(split[0],5,'?');
            cr.GroupCounts = duplicate(split[1],5,',').Split(',').Select(x => Convert.ToInt32(x)).ToList();
            conditionRecords.Add(cr);
        }
    }

    private string duplicate(string input, int times, char? separator = null)
    {
        var outputLength = separator is null ? input.Length * times : input.Length * times + (times - 1);
        char[] outputChars = new char[outputLength];
        for (int i = 0; i < times; i++)
        {
            if (separator is null)
                input.CopyTo(0, outputChars, input.Length * i, input.Length);
            else
            {
                input.CopyTo(0, outputChars, input.Length * i + i, input.Length);
                if(i != 0)
                    outputChars[(input.Length * i) + (i-1)] = separator.Value;
            }
        }

        return new string(outputChars);
    }

    public override ValueTask<string> Solve_1()
    {
        var possibilityCount = 0;
        ConcurrentBag<int> counts = new();

        Parallel.ForEach(conditionRecords, cr =>
        {
            List<int> mysteryIndices = new();
            for (int i = 0; i < cr.Springs.Length; i++)
            {
                if (cr.Springs[i] == '?') mysteryIndices.Add(i);
            }

            var validComboCount = getValidComboCount(cr.Springs, mysteryIndices, cr.GroupCounts);
            counts.Add(validComboCount);
        });

        return new(counts.Sum().ToString());
    }

    private int getValidComboCount(string springs, List<int> mysteryIndices, List<int> expectedGroupCounts)
    {
        var count = 0;
        List<string> returnList = new();
        char[] workingSprings = new char[springs.Length];
        springs.ToCharArray().CopyTo(workingSprings, 0);
        
        var maxCombos = (int)Math.Pow(2, mysteryIndices.Count);

        for (int comboNum = 0; comboNum < maxCombos; comboNum++)
        {
            for (int i = 0; i< mysteryIndices.Count; i++)
            {
                var newChar = (comboNum >> i) & 0b01;
                workingSprings[mysteryIndices[i]] = newChar == 1 ? '.' : '#';
            }

            if (isValid2(new ConditionRecord() {Springs = new string(workingSprings), GroupCounts = expectedGroupCounts}))
                count++;
        }

        return count;
    }

    private bool isValid2(ConditionRecord cr)
    {
        var springs = cr.Springs;
        var expectedGroupCounts = cr.GroupCounts;
        
        List<int> actualGroupCounts = new();
        bool inGroup = false;
        int groupCount = 0;
        for (int i = 0; i < springs.Length; i++)
        {
            if (springs[i] == '#')
            {
                inGroup = true;
                groupCount++;
            }
            else
            {
                inGroup = false;
                if (groupCount > 0)
                {
                    actualGroupCounts.Add(groupCount);
                    groupCount = 0;
                }
            }
        }

        if (groupCount > 0)
        {
            actualGroupCounts.Add(groupCount);
        }

        return expectedGroupCounts.Count == actualGroupCounts.Count &&
            expectedGroupCounts.Zip(actualGroupCounts, (a, b) => a == b).All(x => x);
    }

    public override ValueTask<string> Solve_2()
    {
        return new("");
    }

    private Dictionary<ConditionRecord, long> solutionMemo = new();
    private long CountSolutions(ConditionRecord conditionRecord)
    {
        if (solutionMemo.TryGetValue(conditionRecord, out var answer))
            return answer;

        if (conditionRecord.Springs.All(x => x != '?'))
        {
            if (isValid2(conditionRecord))
            {
                solutionMemo[conditionRecord] = 1;
                return 1;
            }
            else
            {
                solutionMemo[conditionRecord] = 0;
                return 0;
            }
        }

        var springs = conditionRecord.Springs;
        var expectedGroupCounts = conditionRecord.GroupCounts;



        return 0;
    }
}