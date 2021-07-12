namespace AdventureRoller.Services.Attributes
{
    using System;
    internal class CustomRollAttribute : Attribute
    {
        private readonly string RollName;
        public CustomRollAttribute(string rollName)
        {
            RollName = rollName;
        }

        public string GetName()
        {
            return RollName;
        }
    }
}
