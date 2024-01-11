using System.Numerics;

public class Day24 : BaseDay
{
    private readonly List<HailStone> _input = new();

    public Day24()
    {
        var lines = File.ReadAllLines(InputFilePath).ToList();

        foreach (var line in lines)
        {
            var numbers = line.Split(" @ ").SelectMany(x => x.Split(",").Select(y => Decimal.Parse(y))).ToList();
            _input.Add(new HailStone(numbers[0],numbers[1],numbers[2],numbers[3],numbers[4],numbers[5]));
        }
    }

    public override ValueTask<string> Solve_1()
    {

        return new("");
    }
    
    public override ValueTask<string> Solve_2()
    {
        return new("");
    }

    record HailStone(decimal x, decimal y, decimal z, decimal dx, decimal dy, decimal dz);
    
}