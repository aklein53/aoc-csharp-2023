using System.Text.RegularExpressions;

public class Day06 : BaseDay
{
    private readonly List<string> _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        Regex regex = new Regex("\\d+");
        var times = regex.Matches(_input[0]).Select(x => Convert.ToUInt64(x.Value)).ToList();
        var records = regex.Matches(_input[1]).Select(x => Convert.ToUInt64(x.Value)).ToList();

        List<(ulong Time, ulong Record)> races = times.Zip(records, (a, b) => (a,b)).ToList();
        ulong combos = 1;
        foreach ( var race in races)
        {
            var (min, max) = QuadraticSolver(race.Time, race.Record);
            var count = max - min + 1;

            combos *= count;
        }

        return new(combos.ToString());
    }

    public (ulong MinTime, ulong MaxTime) QuadraticSolver(double time, double distance) => QuadraticSolver(1, -time, distance);
    public (ulong MinTime, ulong MaxTime) QuadraticSolver(double a, double b, double c)
    {
        var temp = Math.Sqrt(b * b - 4 * a * c);
        var temp1 = -b + temp;
        var temp2 = -b - temp;
        temp1 = temp1 / (2 * a);
        temp2 = temp2 / (2 * a);
        return ((ulong)Math.Ceiling(Math.Min(temp1, temp2)), (ulong)Math.Floor(Math.Max(temp1, temp2)));
    }

    public override ValueTask<string> Solve_2()
    {
        Regex regex = new Regex("\\d+");
        var times = regex.Matches(_input[0].Replace("Time:","").Replace(" ", "")).Select(x => Convert.ToUInt64(x.Value)).ToList();
        var records = regex.Matches(_input[1].Replace("Distance:", "").Replace(" ", "")).Select(x => Convert.ToUInt64(x.Value)).ToList();

        List<(ulong Time, ulong Record)> races = times.Zip(records, (a, b) => (a, b)).ToList();
        ulong combos = 1;
        foreach (var race in races)
        {
            var (min, max) = QuadraticSolver(race.Time, race.Record);
            var count = max - min + 1;

            combos *= count;
        }

        return new(combos.ToString());
    }
}