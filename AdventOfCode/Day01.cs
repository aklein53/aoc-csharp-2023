namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;

    private readonly Dictionary<string, char> numbers = new() {
        ["1"] = '1',
        ["2"] = '2',
        ["3"] = '3',
        ["4"] = '4',
        ["5"] = '5',
        ["6"] = '6',
        ["7"] = '7',
        ["8"] = '8',
        ["9"] = '9',
        ["one"] = '1',
        ["two"] = '2',
        ["three"] = '3',
        ["four"] = '4',
        ["five"] = '5',
        ["six"] = '6',
        ["seven"] = '7',
        ["eight"] = '8',
        ["nine"] = '9'
    };
    
    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private string part1()
    {
        var lines = _input.Split("\r\n");
        return lines.Select(line => {
            var first = firstNum(line);
            var last = firstNum(new string(line.Reverse().ToArray()));
            //Console.WriteLine($"{first}{last}");
            return Convert.ToInt32(new String(new char[] {first, last}));
        }).Sum().ToString();
    }

    private string part2()
    {
        var lines = _input.Split("\r\n");
        return lines.Select(line => {
            var first = firstNum(line);
            var last = firstNum(line, true);
            //Console.WriteLine($"{first}{last}");
            return Convert.ToInt32(new String(new char[] {first, last}));
        }).Sum().ToString();
    }


    private char firstNum(string line, bool reverse = false)
    {
        var workingLine = reverse ? new string(line.Reverse().ToArray()) : line;
        for (int i = 0; i < workingLine.Length; i++)
        {
            foreach(var (key, value) in numbers)
            {
                var workingKey = reverse ? new string(key.Reverse().ToArray()) : key;
                if (workingLine[i..].StartsWith(workingKey))
                {
                    return value;
                }
            }
        }

        return '0';
    }

    public override ValueTask<string> Solve_1() => new(part1());

    public override ValueTask<string> Solve_2() => new(part2());
}
