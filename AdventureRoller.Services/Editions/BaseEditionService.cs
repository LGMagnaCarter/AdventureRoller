using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventureRoller.Services
{
    public class BaseEditionService : IEditionsService
    {

        private Regex NumberRegex = new Regex("[0-9]{1,45}");
        private static string TemplateLocation = "../../../Templates";
        public string Version { get; }
        public BaseEditionService(string version = "Default")
        {
            Version = version;
        }

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

        public override string ToString()
        {
            return Version;
        }

        public virtual string DefaultRoll(string roll)
        {
            return roll;
        }

        public virtual string CompleteRoll(string roll)
        {
            DataTable dt = new DataTable();
            return dt.Compute(roll, string.Empty).ToString();
        }

        public virtual string ParseRoll(List<int> rolls, string diceParams)
        {
            return rolls.Sum().ToString();
        }

        internal int GetNumberAfterCharacter(string s, char c)
        {
            var match = NumberRegex.Match(s, s.IndexOf(c));

            return int.Parse(match.Value);
        }
    }
}
