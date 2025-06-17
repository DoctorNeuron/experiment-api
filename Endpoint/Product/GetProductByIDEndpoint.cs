using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Product
{
    public class GetProductByIDEndpoint : Endpoint<GetProductByIDRequest, BaseResponse<GetProductByIDResponse?>>
    {
        public override void Configure()
        {
            Get("product/{ID}");
            Description(b =>
            {
                b.Produces<BaseResponse<GetProductByIDResponse?>>(StatusCodes.Status200OK);
            });
        }

        public required ExperimentDBContext DB { get; set; }

        public override async Task<BaseResponse<GetProductByIDResponse?>> ExecuteAsync(GetProductByIDRequest req, CancellationToken ct)
        {
            var query = DB.Set<MsProduct>()
                .Select(x => new GetProductByIDResponse()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Image = x.Image,
                    Category = x.Category,
                    Price = x.Price,
                    IsRaw = x.IsRaw,
                    Time = x.Time,
                    Level = x.Level,
                    XP = x.XP,
                });
            return new BaseResponse<GetProductByIDResponse?>(await query.FirstOrDefaultAsync(x => x.ID == req.ID, ct));
        }
    }

    public class GetProductByIDRequest
    {
        [RouteParam]
        public Guid ID { get; set; }
    }

    public class GetProductByIDResponse
    {
        public required Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
        public required string Category { get; set; }
        public required double Price { get; set; }
        public required bool IsRaw { get; set; }
        public required int Time { get; set; }
        public required int Level { get; set; }
        public required int XP { get; set; }
    }
}