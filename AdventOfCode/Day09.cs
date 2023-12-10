using System.Text.RegularExpressions;

public class Day09 : BaseDay
{
    private readonly List<string> _input;

    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var sequences = _input.Select(line => line.Split(" ").Select(numStr => Convert.ToInt32(numStr)).ToList()).ToList();
        var nextValues = sequences.Select(x => GetNext(x)).ToList();
        return new(nextValues.Sum().ToString());
    }

    private int GetPrevious(List<int> input)
    {
        if (input.All(x => x == 0)) return 0;
        else return input.First() - GetPrevious(GetDerivative(input));
    }
    private int GetNext(List<int> input)
    {
        if (input.All(x => x == 0)) return 0;
        else return input.Last() + GetNext(GetDerivative(input));
    }

    private List<int> GetDerivative(List<int> input)
    {
        return input.Take(input.Count - 1).Zip(input.Skip(1), (a, b) => b - a).ToList();
    }
    public override ValueTask<string> Solve_2()
    {
        var sequences = _input.Select(line => line.Split(" ").Select(numStr => Convert.ToInt32(numStr)).ToList()).ToList();
        var nextValues = sequences.Select(x => GetPrevious(x)).ToList();
        return new(nextValues.Sum().ToString());
    }
}