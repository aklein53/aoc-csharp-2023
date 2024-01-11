using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class Day13 : BaseDay
{
    private readonly List<List<string>> _input = new();

    public Day13()
    {
        var lines = File.ReadAllLines(InputFilePath);
        List<string> currentList = new List<string>();
        _input.Add(currentList);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                currentList = new List<string>();
                _input.Add(currentList);
            }
            else
            {
                currentList.Add(line);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var sum = 0;
        foreach (var field in _input)
        {
            for (int row = 1; row < field.Count; row++)
            {
                if (checkReflectionOnRow(field, row))
                    sum += 100 * row;
            }
            
            for (int col = 1; col < field[0].Length; col++)
            {
                if (checkReflectionOnCol(field, col))
                    sum += col;
            }
        }

        return new(sum.ToString());
    }

    private bool checkReflectionOnRow(List<string> field, int row, int expectedErrors = 0)
    {
        int errorCount = 0;
        for (int iRow = 0; (iRow + row < field.Count && row - iRow > 0); iRow++)
        {
            for (int iCol = 0; iCol < field[0].Length; iCol++)
            {
                var char1 = field[row + iRow][iCol];
                var char2 = field[row - iRow - 1][iCol];
                if (char1 != char2) errorCount++;
                if (errorCount > expectedErrors) return false;
            }
        }

        return errorCount == expectedErrors;
    }
    
    private bool checkReflectionOnCol(List<string> field, int col, int expectedErrors = 0)
    {
        int errorCount = 0;
        for (int iCol = 0; (iCol + col < field[0].Length && col - iCol > 0); iCol++)
        {
            for (int iRow = 0; iRow < field.Count; iRow++)
            {
                var char1 = field[iRow][iCol + col];
                var char2 = field[iRow][col - iCol - 1];
                if (char1 != char2) errorCount++;
                if (errorCount > expectedErrors) return false;
            }
        }

        return errorCount == expectedErrors;
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;
        foreach (var field in _input)
        {
            for (int row = 1; row < field.Count; row++)
            {
                if (checkReflectionOnRow(field, row, 1))
                    sum += 100 * row;
            }
            
            for (int col = 1; col < field[0].Length; col++)
            {
                if (checkReflectionOnCol(field, col, 1))
                    sum += col;
            }
        }

        return new(sum.ToString());
    }
}