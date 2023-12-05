
public class Day02 : BaseDay
{
    private readonly string _input;

    private readonly int redLimit = 12, greenLimit = 13, blueLimit = 14;
    
    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

//Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
    public override ValueTask<string> Solve_1()
    {
        List<int> validGames = new();
        bool currentlyValid = true;
        foreach(var line in _input.Split("\r\n"))
        {
            var gameNum = Convert.ToInt32(line.Split(":")[0].Split(" ")[1]);
            var gameData = line.Split(":")[1];
            currentlyValid = true;

            Console.WriteLine($"Processing game {gameNum}");
            foreach(var round in gameData.Split(";"))
            {
                Console.WriteLine($"Processing round '{round}'");
                foreach(var hand in round.Split(","))
                {
                    Console.WriteLine($"Processing round '{round}'");
                    var color = hand.Trim().Split(" ")[1];
                    var num = Convert.ToInt32(hand.Trim().Split(" ")[0]);
                    if (color == "red" && num > redLimit)
                    {
                        currentlyValid = false;
                        break;
                    }
                    else if (color == "green" && num > greenLimit)
                    {
                        currentlyValid = false;
                        break;
                    }
                    else if (color == "blue" && num > blueLimit)
                    {
                        currentlyValid = false;
                        break;
                    }

                    if (!currentlyValid) break;
                }

                if (!currentlyValid) break;
            }

            if (currentlyValid) validGames.Add(gameNum);
        }
        
        Console.WriteLine(validGames.Count);
        return new(validGames.Distinct().Sum().ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        List<int> powers = new();
        foreach(var line in _input.Split("\r\n"))
        {
            var gameNum = Convert.ToInt32(line.Split(":")[0].Split(" ")[1]);
            var gameData = line.Split(":")[1];
            int minRed = 0, minGreen = 0, minBlue = 0;

            Console.WriteLine($"Processing game {gameNum}");
            foreach(var round in gameData.Split(";"))
            {
                Console.WriteLine($"Processing round '{round}'");
                foreach(var hand in round.Split(","))
                {
                    Console.WriteLine($"Processing round '{round}'");
                    var color = hand.Trim().Split(" ")[1];
                    var num = Convert.ToInt32(hand.Trim().Split(" ")[0]);
                    if (color == "red" && num > minRed)
                    {
                        minRed = num;
                    }
                    else if (color == "green" && num > minGreen)
                    {
                        minGreen = num;
                    }
                    else if (color == "blue" && num > minBlue)
                    {
                        minBlue = num;
                    }
                }
            }

            powers.Add(minRed * minGreen * minBlue);
        }

        return new (powers.Sum().ToString());
    }
}