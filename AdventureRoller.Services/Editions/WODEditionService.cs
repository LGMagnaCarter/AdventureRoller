using AdventureRoller.Services.Attributes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventureRoller.Services
{
    public class WODEditionsService : BaseEditionService, IEditionsService
    {
        private readonly Regex numberModifier = new Regex("[0-9]{1,3}");
        private readonly Regex rerollRegex = new Regex("[r][0-9]{1,3}");
        public WODEditionsService() : base("wod")
        {

        }

        public override string PrepRoll(string roll)
        {
            DataTable dt = new DataTable();
            var value =  int.Parse(dt.Compute(roll, string.Empty).ToString());

            if(value < 1)
            {
                return GetChanceRoll();
            }

            return value.ToString();
        }

        [CustomRoll("Default")]
        public override string GetDefaultRoll(string roll)
        {
            return $"{roll}d10r10s8";
        }

        public override string ParseRoll(List<int> rolls, string diceParams)
        {
            string response = $"Successes:{rolls.Count(x => x >= GetNumberAfterCharacter(diceParams, 's'))}";
            if (diceParams.Contains('r'))
            {
                response += $"\r\nRerolls: {rolls.Count(x => x >= GetNumberAfterCharacter(diceParams, 'r'))}";
            }
            if (diceParams.Contains('f'))
            {
                response += $"\r\nCritical Failures: {rolls.Count(x => x <= GetNumberAfterCharacter(diceParams, 'f'))}";
            }
            return response;
        }

        public override string CompleteRoll(string roll)
        {
            return roll;
        }

        [CustomRoll("Chance")]
        public override string GetChanceRoll()
        {
            return "1d10r10s10f1";
        }

        [CustomRoll("Rote")]
        public string GetRoteRoll(int amount)
        {
            return $"{amount}d10r1-7s10 \\10again";
        }

        [CustomRoll("Initiative")]
        public string GetInitiativeRoll()
        {
            return "1d10";
        }

        public override string ModifyRoll(string roll, List<string> modifiers)
        {
            foreach(string modifier in modifiers)
            {
                if (modifier.Contains("again"))
                {
                    var numberMatch = numberModifier.Match(modifier);
                    
                    if (!numberMatch.Success)
                    {
                        continue;
                    }

                    roll = roll.Insert(roll.Length - 1, $"r{numberMatch.Value}");
                }
            }

            return roll;
        }

        public override string GetRoll(string rollName, object[] parameters = null)
        {
            return GetRoll<WODEditionsService>(rollName, parameters);
        }
    }
}
