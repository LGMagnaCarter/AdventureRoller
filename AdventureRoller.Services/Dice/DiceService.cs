namespace AdventureRoller.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DiceService : IDiceService
    {
        private Regex NumberRegex = new Regex("[0-9]{1,45}");
        private Regex NumberRangeRegex = new Regex("[0-9]{1,45}[-][0-9]{1,45}");
        private Regex AlphabetMatch = new Regex("[A-z]");

        private Random random = new Random();

        public List<int> RollExactDice(string dice)
        {
            int numberOfRolls = int.Parse(dice.IndexOf('d') == 0 ? "1" : dice.Substring(0, dice.IndexOf('d')));
            int diceSize = GetNumberAfterCharacter(dice, 'd');
            List<int> results = new List<int>();

            for (int i = 0; i < numberOfRolls; i++)
            {
                RollDice(results, diceSize, dice);
            }

            if (!(dice.Contains('l') || dice.Contains('h')))
            {
                return results;
            }

            results.Sort();

            int keep;
            if (dice.Contains('h'))
            {
                results.Reverse();
                keep = GetNumberAfterCharacter(dice, 'h');
            }
            else
            {
                keep = GetNumberAfterCharacter(dice, 'l');
            }

            results = results.Take(keep).ToList();

            return results;
        }

        private int GetNumberAfterCharacter(string s, char c)
        {
            var match = NumberRegex.Match(s, s.IndexOf(c));

            return int.Parse(match.Value);
        }

        private Range GetRangeAfterCharacter(string s, char c, int startIndex = 0)
        {

            var rangeMatch = NumberRangeRegex.Match(s, startIndex, s.IndexOf(c));

            if (rangeMatch.Success)
            {
                var range = rangeMatch.Value.Split('-');
                return new Range(int.Parse(range[0]), int.Parse(range[1]));
            }

            var match = NumberRegex.Match(s, s.IndexOf(c));

            return new Range(int.Parse(match.Value), int.MaxValue);
        }

        private List<int> RollDice(List<int> results, int diceSize, string diceParams)
        {
            var result = random.Next(1, diceSize + 1);
            results.Add(result);

            var rerollIndexes = new List<int>();

            for (int i = 0; i < diceParams.Length; i++)
            {
                if (diceParams[i] == 'r') rerollIndexes.Add(i);
            }

            foreach (int rerollIndex in rerollIndexes)
            {
                var rerollVal = GetRangeAfterCharacter(diceParams, 'r', rerollIndex);
                if (result >= rerollVal.Start.Value && result <= rerollVal.End.Value)
                {
                    RollDice(results, diceSize, diceParams);
                }
            }

            return results;
        }
    }
}
