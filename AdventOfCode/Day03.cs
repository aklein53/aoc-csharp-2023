

using System.Text.RegularExpressions;

public class Day03 : BaseDay
{
    private readonly List<string> _input;
    
    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

//Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
    public override ValueTask<string> Solve_1()
    {
        List<string> partNumberList = new();
        Regex regex = new Regex("\\d+");
        for (int row = 0; row < _input.Count; row++)
        {
            var matches = regex.Matches(_input[row]);
            for (int i=0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (checkIsPartNumber(row, match.Index, match.Index + match.Length - 1))
                {
                    partNumberList.Add(match.Value);
                }
            }
        }

        return new(partNumberList.Select(x => Convert.ToInt32(x)).Sum().ToString());
    }

    private bool checkIsPartNumber(int rowIndex, int startIndex, int endIndex)
    {
        var startRow = Math.Clamp(rowIndex - 1, 0, _input.Count - 1);
        var endRow = Math.Clamp(rowIndex + 1, 0, _input.Count - 1);
        var startCol = Math.Clamp(startIndex - 1, 0, _input[rowIndex].Length - 1);
        var endCol = Math.Clamp( endIndex + 1, 0, _input[rowIndex].Length - 1);

        for (int row=startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                if (checkIsSymbolChar(_input[row][col]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool checkIsSymbolChar(char input)
    {
        return input != '.' && (input < '0' || input > '9');
    }

    public override ValueTask<string> Solve_2()
    {
        List<int> gearRatioList = new();
        for (int row = 0; row < _input.Count; row++)
        {
            for (int col = 0; col < _input[row].Length; col++)
            {
                if (_input[row][col] == '*')
                {
                    var gearRatio = getGearRatio(row, col);
                    if (gearRatio.HasValue)
                        gearRatioList.Add(gearRatio.Value);
                }
            }
        }

        return new(gearRatioList.Sum().ToString());
    }

    private int? getGearRatio(int targetRow, int targetCol)
    {
        List<string> possiblePartNumbers = new();
        Regex regex = new Regex("\\d+");
        for (int row = Math.Max(0, targetRow-1); row <= Math.Min(_input.Count - 1, targetRow + 1); row++)
        {
            var matches = regex.Matches(_input[row]);
            for (int i=0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (checkIsAdjacent(targetCol, match))
                {
                    possiblePartNumbers.Add(match.Value);
                }
            }
        }

        if (possiblePartNumbers.Count == 2)
        {
            Console.WriteLine($"Found gear at {targetRow}, {targetCol} with ratio {possiblePartNumbers[0]}:{possiblePartNumbers[1]}");
            return Convert.ToInt32(possiblePartNumbers[0]) * Convert.ToInt32(possiblePartNumbers[1]);
        }

        return null;
    }

    private bool checkIsAdjacent(int targetCol, Match match)
    {
        var lowestPossibleIndex = match.Index - 1;
        var highestPossibleIndex = match.Index + match.Length;

        return targetCol >= lowestPossibleIndex && targetCol <= highestPossibleIndex;
    }
}