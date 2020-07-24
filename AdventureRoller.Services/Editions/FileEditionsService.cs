using System.Collections.Generic;
using System.IO;

namespace AdventureRoller.Services
{
    public class FileEditionsService : IEditionsService
    {
        private static string TemplateLocation = "../../../Templates";

        public List<string> GetEditions()
        {
            var editionsFileNames = Directory.GetFiles(TemplateLocation, "*.json");

            var editionsList = new List<string>();
            foreach(var filename in editionsFileNames)
            {
                var name = filename.Substring(0, filename.IndexOf(".json")).Substring(filename.LastIndexOf('\\')).Replace("\\", "");
                editionsList.Add(name);
            }
            return editionsList;
        }

        public string GetEdition(string edition)
        {
            using (StreamReader r = new StreamReader($"{TemplateLocation}\\{edition}.json"))
            {
                return r.ReadToEnd();
            }
        }
    }
}
