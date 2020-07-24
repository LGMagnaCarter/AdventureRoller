namespace AdventureRoller.Services
{
    using System.Collections.Generic;

    public interface IEditionsService
    {
        List<string> GetEditions();

        string GetEdition(string edition);
    }
}
