using AdventureRoller.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
            foreach (var filename in editionsFileNames)
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

        public virtual string GetDefaultRoll(string roll)
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

        [CustomRoll("Chance")]
        public virtual string GetChanceRoll()
        {
            return "1d20";
        }

        public virtual string ModifyRoll(string roll, List<string> modifiers)
        {
            return roll;
        }

        internal virtual string GetRoll<TEntity>(string rollName, object[] parameters = null)
        {
            var rollMethods = typeof(TEntity).GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(typeof(CustomRollAttribute), true).Length > 0);

            if(rollMethods.Count() == 0)
            {
                return string.Empty;
            }

            foreach( var rollMethod in rollMethods)
            {
                var attribute = (CustomRollAttribute)rollMethod.GetCustomAttribute(typeof(CustomRollAttribute));

                if (attribute.GetName().ToLowerInvariant() == rollName.ToLowerInvariant())
                {
                    return rollMethod.Invoke((TEntity)Activator.CreateInstance(typeof(TEntity)), parameters).ToString();
                }
            }

            return string.Empty;
        }

        public virtual string GetRoll(string rollName, object[] parameters)
        {
            return GetRoll<BaseEditionService>(rollName, parameters);
        }

        public virtual string PrepRoll(string roll)
        {
            return roll;
        }

    }
}
