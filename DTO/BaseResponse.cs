namespace ExperimentAPI.DTO
{
    public record BaseResponse<T>(T Data, string Message = "", int StatusCode = StatusCodes.Status200OK);
}