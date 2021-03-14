using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdventureRoller.Services
{
    public class DNDEditionsService : BaseEditionService, IEditionsService
    {
        public DNDEditionsService() : base("dnd")
        {

        }

        public override string DefaultRoll(string roll)
        {
            return $"1d20 + {roll}";
        }

        public override string ParseRoll(List<int> rolls, string diceParams)
        {
            return rolls.Sum().ToString();
        }

        public override string CompleteRoll(string roll)
        {
            DataTable dt = new DataTable();
            return dt.Compute(roll, string.Empty).ToString();
        }
    }
}
