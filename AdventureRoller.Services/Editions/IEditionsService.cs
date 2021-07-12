namespace AdventureRoller.Services
{
    using System.Collections.Generic;

    public interface IEditionsService
    {
        string GetDefaultRoll(string roll);
        List<string> GetEditions();

        string GetEdition(string edition);

        string ParseRoll(List<int> rolls, string diceParams);

        string CompleteRoll(string roll);

        string ModifyRoll(string roll, List<string> modifiers);

        string GetRoll(string rollName, object[] parameters = null);

        string PrepRoll(string roll);
    }
}
