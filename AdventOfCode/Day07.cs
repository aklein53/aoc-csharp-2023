using System.Text.RegularExpressions;

public class Day07 : BaseDay
{
    private readonly List<string> _input;

    public enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    public class Hand : IComparable<Hand>
    {
        private Dictionary<char, int> cardRanks = new Dictionary<char, int>()
        {
            ['A'] = 14,
            ['K'] = 13,
            ['Q'] = 12,
            ['J'] = 1,
            ['T'] = 10,
            ['9'] = 9,
            ['8'] = 8,
            ['7'] = 7,
            ['6'] = 6,
            ['5'] = 5,
            ['4'] = 4,
            ['3'] = 3,
            ['2'] = 2,
        };

        public HandType HandType;
        public char[] Cards;
        public long Bid;
        public Hand(char[] cards, int bid)
        {
            Cards = cards;
            Bid = bid;
            Dictionary<char, int> cardCounts = new();
            foreach (var card in cards)
            {
                if (!cardCounts.ContainsKey(card)) cardCounts.Add(card, 1);
                else cardCounts[card]++;
            }
            var numJokers = cardCounts.ContainsKey('J') ? cardCounts['J'] : 0;
            if (cardCounts.Count == 1 || (cardCounts.Count == 2 && cardCounts.ContainsKey('J')))
                HandType = HandType.FiveOfAKind;
            else if (cardCounts.Any(x => x.Value == (4 - numJokers)))
                HandType = HandType.FourOfAKind;
            else if ((cardCounts.Count == 2 && cardCounts.Any(x => x.Value == 3) && cardCounts.Any(x => x.Value == 2)) || (cardCounts.Count == 3 && numJokers > 0))
                HandType = HandType.FullHouse;
            else if (cardCounts.Any(x => x.Value + numJokers == 3))
                HandType = HandType.ThreeOfAKind;
            else if ((cardCounts.Count(x => x.Value == 2) == 2) || (cardCounts.Count(x => x.Value == 2) == 1 && numJokers == 1))
                HandType = HandType.TwoPair;
            else if (cardCounts.Count(x => x.Value == 2) == 1 || numJokers == 1)
                HandType = HandType.OnePair;
            else
                HandType = HandType.HighCard;
        }

        public int CompareTo(Hand other)
        {
            if (this.HandType > other.HandType)
                return 1;
            else if (this.HandType < other.HandType)
                return -1;
            else
            {
                for (int i = 0; i < Cards.Length; i++)
                {
                    var thisValue = cardRanks[this.Cards[i]];
                    var otherValue = cardRanks[other.Cards[i]];
                    if (thisValue > otherValue)
                        return 1;
                    else if (thisValue < otherValue)
                        return -1;
                }

                return 0;
            }
        }

        public override string ToString()
        {
            return $"Cards: {new string(Cards)}, HandType: {HandType}, Bid: {Bid}";
        }
    }

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var hands = _input.Select(x =>
        {
            var split = x.Split(" ");
            return new Hand(split[0].ToCharArray(), Convert.ToInt32(split[1]));
        }).OrderBy(x => x).ToList();

        var handsWithJokers = hands.Where(x => !x.Cards.Contains('J')).ToList();
        var totalWinnings = hands.Select((x, i) => x.Bid * ((long)i + 1)).Sum();
        return new(totalWinnings.ToString());
    }
    public override ValueTask<string> Solve_2()
    {
        return new("");
    }
}