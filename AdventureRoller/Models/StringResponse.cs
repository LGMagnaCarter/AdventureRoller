namespace AdventureRoller.Models
{
    public class StringResponse : Response
    {
        public string Value { get; set; }

        public StringResponse(bool success, string response) : base(success, response)
        {
            if (success)
                Value = response;
            else
                Error = response;
        }
    }
}
