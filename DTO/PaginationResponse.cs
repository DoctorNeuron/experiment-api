using System.Text.Json.Serialization;

namespace ExperimentAPI.DTO
{
    public class PaginationResponse<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationRecord? Pagination { get; set; } = null;
        public required List<T> Data { get; set; }
    }
    public record PaginationRecord(int CurrentPage, int PageSize, int TotalData)
    {
        public bool IsLastPage => TotalData < CurrentPage * PageSize;
    }
}