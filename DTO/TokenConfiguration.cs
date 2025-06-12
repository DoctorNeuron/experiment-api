namespace ExperimentAPI.DTO
{
    public class TokenConfiguration
    {
        public required string Key { get; set; }
        public required int AccessTokenDurationMinutes { get; set; }
        public required int RefreshTokenDurationMinutes { get; set; }
    }
}