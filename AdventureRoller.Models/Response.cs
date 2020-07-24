namespace AdventureRoller.Models
{
    public class Response
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public Response(bool success)
        {
            Success = success;
        }

        public Response(bool success, string error)
        {
            Success = success;
            if (!success)
                Error = error;
        }

        public static Response SuccessfulResponse { get { return new Response(true); } }
    }
}
