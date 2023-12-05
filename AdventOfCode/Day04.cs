

using System.Text.RegularExpressions;

public class Day04 : BaseDay
{
    private readonly List<string> _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var count = 1;
        var total = 0;
        foreach(var line in _input)
        {
            var usefulPart = line.Split(":")[1];
            var winningNumbers = usefulPart.Split("|")[0].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToHashSet();
            var numbersWeHave = usefulPart.Split("|")[1].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToList();

            var cardTotal = 0;
            foreach (var numberWeHave in numbersWeHave)
            {
                if (winningNumbers.Contains(numberWeHave))
                {
                    if (cardTotal == 0) cardTotal = 1;
                    else cardTotal *= 2;
                }
            }
            total += cardTotal;
        }
        return new(total.ToString());
    }

    

    public override ValueTask<string> Solve_2()
    {
        var totalCardsAvailable = _input.Count;
        int[] numMatches = new int[totalCardsAvailable];
        int[] copies = new int[totalCardsAvailable];

        for (int i = 0; i < totalCardsAvailable; i++)
        {
            numMatches[i] = numberMatches(_input[i]);
            copies[i] = 1;
        }

        for (int i = 0; i < totalCardsAvailable; i++)
        {
            for (int j = i + 1; j <= i + numMatches[i]; j++)
            {
                copies[j] += copies[i];
            }
        }

        return new(copies.Sum().ToString());
    }

    private int numberMatches(string line)
    {
        var usefulPart = line.Split(":")[1];
        var winningNumbers = usefulPart.Split("|")[0].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToHashSet();
        var numbersWeHave = usefulPart.Split("|")[1].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToList();

        return numbersWeHave.Count(x => winningNumbers.Contains(x));
    }
}