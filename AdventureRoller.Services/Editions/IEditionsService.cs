namespace AdventureRoller.Services
{
    using System.Collections.Generic;

    public interface IEditionsService
    {
        string DefaultRoll(string roll);
        List<string> GetEditions();

        string GetEdition(string edition);

        string ParseRoll(List<int> rolls, string diceParams);

        string CompleteRoll(string roll);
    }
}
