namespace AdventureRoller.Models
{
    using System.Collections.Generic;

    public class RollModel
    {
        public string Call { get; set; }
        public string CallRolled { get; set; }
        public List<DiceModel> Rolls { get; set; }
    }
}
