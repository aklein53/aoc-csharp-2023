

using System.Text.RegularExpressions;

public class Day05 : BaseDay
{
    private readonly List<string> _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        List<long> seeds = _input[0].Replace("seeds: ", "").Split(" ").Select(x => Convert.ToInt64(x)).ToList();

        Dictionary<(string, string), Map> maps = new Dictionary<(string, string), Map>();

        bool inMap = false;
        string[] currentMap = new string[2];

        for(int i = 0; i < _input.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(_input[i]))
            {
                inMap = false;
                continue;
            }
            if (_input[i].Contains("map"))
            {
                inMap = true;
                currentMap = (_input[i].Replace(" map:", "").Split("-to-"));
                maps.Add((currentMap[0], currentMap[1]), new Map());
                continue;
            }

            if (inMap)
            {
                var split = _input[i].Split(" ").Select(x => Convert.ToInt64(x)).ToList();
                var destStart = split[0];
                var sourceStart = split[1];
                var length = split[2];

                maps[(currentMap[0], currentMap[1])].Chunks.Add((sourceStart, destStart, length));
            }
        }

        var results = seeds.Select(x =>
        {
            var temp = GetOrDefault(maps[("seed", "soil")],x);
            temp = GetOrDefault(maps[("soil", "fertilizer")],temp);
            temp = GetOrDefault(maps[("fertilizer", "water")],temp);
            temp = GetOrDefault(maps[("water", "light")],temp);
            temp = GetOrDefault(maps[("light", "temperature")],temp);
            temp = GetOrDefault(maps[("temperature", "humidity")],temp);
            temp = GetOrDefault(maps[("humidity", "location")],temp);
            return (x, temp);
        }).ToList();
        return new(results.MinBy(x => x.Item2).temp.ToString());
    }

    private long GetOrDefault(Map map, long key)
    {
        foreach (var chunk in map.Chunks)
        {
            if (key >= chunk.SourceStart && key < chunk.SourceStart + chunk.Length)
            {
                return chunk.DestStart + (key - chunk.SourceStart);
            }
        }

        return key;
    }

    private long GetReverseOrDefault(Map map, long value)
    {
        foreach (var chunk in map.Chunks)
        {
            if (value >= chunk.DestStart && value < chunk.DestStart + chunk.Length)
            {
                return chunk.SourceStart + (value - chunk.DestStart);
            }
        }

        return value;
    }

    public override ValueTask<string> Solve_2()
    {
        List<(long Start, long Length)> seedRanges = _input[0].Replace("seeds: ", "").Split(" ").Chunk(2).Select(x => (Convert.ToInt64(x[0]), Convert.ToInt64(x[1]))).ToList();

        Dictionary<(string, string), Map> maps = new Dictionary<(string, string), Map>();

        bool inMap = false;
        string[] currentMap = new string[2];

        for (int i = 0; i < _input.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(_input[i]))
            {
                inMap = false;
                continue;
            }
            if (_input[i].Contains("map"))
            {
                inMap = true;
                currentMap = (_input[i].Replace(" map:", "").Split("-to-"));
                maps.Add((currentMap[0], currentMap[1]), new Map());
                continue;
            }

            if (inMap)
            {
                var split = _input[i].Split(" ").Select(x => Convert.ToInt64(x)).ToList();
                var destStart = split[0];
                var sourceStart = split[1];
                var length = split[2];

                maps[(currentMap[0], currentMap[1])].Chunks.Add((sourceStart, destStart, length));
            }
        }

        var results = new List<(long, long)>();

        for (long i = 0; i < long.MaxValue; i++)
        {
            if (i % 10000 == 0)
            {
                Console.WriteLine($"Checked {i} locations");
            }
            var temp = GetReverseOrDefault(maps[("humidity", "location")], i);
            temp = GetReverseOrDefault(maps[("temperature", "humidity")], temp);
            temp = GetReverseOrDefault(maps[("light", "temperature")], temp);
            temp = GetReverseOrDefault(maps[("water", "light")], temp);
            temp = GetReverseOrDefault(maps[("fertilizer", "water")], temp);
            temp = GetReverseOrDefault(maps[("soil", "fertilizer")], temp);
            temp = GetReverseOrDefault(maps[("seed", "soil")], temp);

            foreach(var seedRange in seedRanges)
            {
                if (temp >= seedRange.Start && temp < seedRange.Start + seedRange.Length)
                {
                    return new(i.ToString());
                }
            }
        }

        throw new Exception("No solution found");
    }

    class Map
    {
        public List<(long SourceStart, long DestStart, long Length)> Chunks { get; } = new List<(long SourceStart, long DestStart, long Length)>();
    }
}