namespace AdventureRoller.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DiceService : IDiceService
    {
        private Regex NumberRegex = new Regex("[0-9]{1,45}");

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

        private List<int> RollDice(List<int> results, int diceSize, string diceParams)
        {
            var result = random.Next(1, diceSize + 1);
            results.Add(result);

            if(diceParams.Contains('r'))
            {
                var rerollVal = GetNumberAfterCharacter(diceParams, 'r');
                if(result >= rerollVal)
                {
                    RollDice(results, diceSize, diceParams);
                }
            }

            return results;
        }
    }
}
