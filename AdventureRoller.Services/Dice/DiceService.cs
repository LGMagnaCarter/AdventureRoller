namespace AdventureRoller.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DiceService : IDiceService
    {
        private Regex DiceRegex = new Regex("[0-9]{0,45}[d][0-9]{1,45}[h]?[0-9]?[l]?[0-9]?");

        private Regex NumberRegex = new Regex("[0-9]{1,45}");

        public List<int> RollExactDice(string dice)
        {
            int numberOfRolls = int.Parse(dice.IndexOf('d') == 0 ? "1" : dice.Substring(0, dice.IndexOf('d')));
            int diceSize = GetNumberAfterCharacter(dice, 'd');
            List<int> results = new List<int>();
            Random r = new Random();

            for (int i = 0; i < numberOfRolls; i++)
            {
                results.Add(r.Next(1, diceSize + 1));
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
    }
}
