using System.ComponentModel;
using ExperimentAPI.Entity;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Order
{
    public class GetOrderByIDEndpoint : Endpoint<GetOrderByIDRequest, GetOrderByIDResponse?>
    {
        public override void Configure()
        {
            Get("order/{ID}");
            Description(b =>
            {
                b.Produces<GetOrderByIDResponse?>(StatusCodes.Status200OK);
            });
        }

        public required ExperimentDBContext DB { get; set; }

        public override async Task<GetOrderByIDResponse?> ExecuteAsync(GetOrderByIDRequest req, CancellationToken ct)
        {
            var query = DB.Set<TrOrderHeader>()
                .Where(x => x.UserID == req.UserID && x.ID == req.ID)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .Select(x => new GetOrderByIDResponse()
                {
                    ID = x.ID,
                    IssuedDate = x.IssuedDate,
                    Offer = x.Offer,
                    OrderDetails = x.OrderDetails.Select(y => new GetOrderByIDResponse.OrderDetailRecord(
                        y.Product.ID,
                        y.Product.Name,
                        y.Product.Image,
                        y.Product.Price,
                        y.Quantity
                    )).ToList()
                });

            return await query.FirstOrDefaultAsync(ct);
        }
    }

    public class GetOrderByIDRequest
    {
        [RouteParam]
        public Guid ID { get; set; }

        [FromClaim(Constant.Claims.ID)]
        public Guid UserID { get; set; }
    }

    public class GetOrderByIDResponse
    {
        public required Guid ID { get; set; }
        public required DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public required int Offer { get; set; }
        public required List<OrderDetailRecord> OrderDetails { get; set; }

        public record OrderDetailRecord(
            Guid ID,
            string Name,
            string Image,
            double Price,
            int Quantity
        );
    }
}