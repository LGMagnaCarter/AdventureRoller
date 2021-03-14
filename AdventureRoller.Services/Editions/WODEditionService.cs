using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdventureRoller.Services
{
    public class WODEditionsService : BaseEditionService, IEditionsService
    {
        public WODEditionsService() : base("wod")
        {

        }

        public override string DefaultRoll(string roll)
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
            return response;
        }

        public override string CompleteRoll(string roll)
        {
            return roll;
        }
    }
}
