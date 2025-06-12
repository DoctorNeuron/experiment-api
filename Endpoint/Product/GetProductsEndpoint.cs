using System.ComponentModel;
using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using ExperimentAPI.Helper;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Product
{
    public class GetProductsEndpoint : Endpoint<GetProductsRequest, BaseResponse<PaginationResponse<GetProductsResponse>>>
    {
        public override void Configure()
        {
            Get("product");
            Description(b =>
            {
                b.Produces<BaseResponse<PaginationResponse<GetProductsResponse>>>(StatusCodes.Status200OK);
            });
        }

        public required ExperimentDBContext DB { get; set; }

        public override async Task<BaseResponse<PaginationResponse<GetProductsResponse>>> ExecuteAsync(GetProductsRequest req, CancellationToken ct)
        {
            var query = DB.Set<MsProduct>()
                .Where(x =>
                    (string.IsNullOrWhiteSpace(req.Name) || EF.Functions.TrigramsAreWordSimilar(req.Name, x.Name)) &&
                    (req.Category == null || req.Category.Count == 0 || req.Category.Contains(x.Category)) &&
                    (req.MinPrice == 0 || req.MinPrice >= x.Price) &&
                    (req.MaxPrice == 0 || req.MaxPrice <= x.Price) &&
                    (req.Level == 0 || req.Level >= x.Price) &&
                    (!req.IsRaw.HasValue || req.IsRaw.Value == x.IsRaw)
                )
                .Select(x => new GetProductsResponse()
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
            if (string.IsNullOrWhiteSpace(req.Name)) query = query.OrderBy(x => x.Level);
            else query = query.OrderByDescending(x => EF.Functions.TrigramsWordSimilarity(req.Name, x.Name)).ThenBy(x => x.Level);

            return await query.MakePaginationResponse(req.CurrentPage, req.PageSize, ct);
        }
    }

    public class GetProductsRequest
    {
        [QueryParam]
        [DefaultValue(1)]
        public int CurrentPage { get; set; } = 1;

        [QueryParam]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        [QueryParam]
        public string? Name { get; set; }

        [QueryParam]
        public List<string>? Category { get; set; } = [];

        [QueryParam]
        public bool? IsRaw { get; set; }

        [QueryParam]
        public int? Level { get; set; } = 0;

        [QueryParam]
        public int? MaxPrice { get; set; } = 0;

        [QueryParam]
        public int? MinPrice { get; set; } = 0;
    }
    public class GetProductsRequestValidation : Validator<GetProductsRequest>
    {
        public GetProductsRequestValidation()
        {
            RuleFor(x => x.Name).MinimumLength(3).When(x => !string.IsNullOrWhiteSpace(x.Name));
            RuleFor(x => x.Level).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0);

            RuleFor(x => x.CurrentPage).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize)
                .Must(x => x % 5 == 0)
                .WithMessage("Must be divisible by 5")
                .LessThanOrEqualTo(100);
        }
    }

    public class GetProductsResponse
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