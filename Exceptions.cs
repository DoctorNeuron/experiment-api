namespace ExperimentAPI
{
    public static class Exceptions
    {
        public class BadRequestException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public BadRequestException(string message)
            {
                Errors = new() { { "Server", [message] } };
            }

            public BadRequestException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }

        public class UnauthorizedException : Exception;
    }
}