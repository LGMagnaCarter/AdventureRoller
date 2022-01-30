using AdventureRoller.Services.Attributes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventureRoller.Services
{
    //Star Trek Adventures
    public class STAEditionsService : BaseEditionService, IEditionsService
    {
        private readonly Regex numberModifier = new Regex("[0-9]{1,3}");
        private readonly Regex rerollRegex = new Regex("[r][0-9]{1,3}");
        public STAEditionsService() : base("sta")
        {

        }

        public override string PrepRoll(string roll)
        {
            DataTable dt = new DataTable();
            var value =  20 - int.Parse(dt.Compute(roll, string.Empty).ToString());

            return value.ToString();
        }

        [CustomRoll("Default")]
        public override string GetDefaultRoll(string roll)
        {
            roll = roll ?? "17";

            return $"2d20s{roll}";
        }

        public override string ParseRoll(List<int> rolls, string diceParams)
        {
            var successes = rolls.Count(x => x >= GetNumberAfterCharacter(diceParams, 's') + 1);

            successes += rolls.Count(x => x == 20);

            var modifiedRolls = rolls.Select(x => { x = 21 - x; return x; }).ToList();

            string response = $"{string.Join(',', modifiedRolls)}\r\nSuccesses:{successes}";
            return response;
        }

        public override string CompleteRoll(string roll)
        {
            return roll;
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
