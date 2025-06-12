using System.ComponentModel;
using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using ExperimentAPI.Helper;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Order
{
    public class GetOrdersEndpoint : Endpoint<GetOrdersRequest, BaseResponse<PaginationResponse<GetOrdersResponse>>>
    {
        public override void Configure()
        {
            Get("order");
            Description(b =>
            {
                b.Produces<BaseResponse<PaginationResponse<GetOrdersResponse>>>(StatusCodes.Status200OK);
            });
        }

        public required ExperimentDBContext DB { get; set; }

        public override async Task<BaseResponse<PaginationResponse<GetOrdersResponse>>> ExecuteAsync(GetOrdersRequest req, CancellationToken ct)
        {
            var query = DB.Set<TrOrderHeader>()
                .Where(x => x.UserID == req.UserID)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .Select(x => new GetOrdersResponse()
                {
                    ID = x.ID,
                    IssuedDate = x.IssuedDate,
                    Offer = x.Offer,
                    OrderDetails = x.OrderDetails.Select(y => new GetOrdersResponse.OrderDetailRecord(
                        y.Product.ID,
                        y.Product.Name,
                        y.Product.Image,
                        y.Product.Price,
                        y.Quantity
                    )).ToList()
                })
                .OrderBy(x => x.IssuedDate);

            return await query.MakePaginationResponse(req.CurrentPage, req.PageSize, ct);
        }
    }

    public class GetOrdersRequest
    {
        [QueryParam]
        [DefaultValue(1)]
        public int CurrentPage { get; set; } = 1;

        [QueryParam]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        [FromClaim(Constant.Claims.ID)]
        public Guid UserID { get; set; }
    }
    public class GetOrdersRequestValidation : Validator<GetOrdersRequest>
    {
        public GetOrdersRequestValidation()
        {
            RuleFor(x => x.CurrentPage).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize)
                .Must(x => x % 5 == 0)
                .WithMessage("Must be divisible by 5")
                .LessThanOrEqualTo(100);
        }
    }

    public class GetOrdersResponse
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