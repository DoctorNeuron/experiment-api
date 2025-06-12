using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Order
{
    public class UpdateOrderEndpoint : Endpoint<UpdateOrderEndpointRequest, BaseResponse<string>>
    {
        public required ExperimentDBContext DB { get; set; }

        public override void Configure()
        {
            Patch("order");
            AllowFormData(true);
        }

        public override async Task<BaseResponse<string>> ExecuteAsync(UpdateOrderEndpointRequest req, CancellationToken ct)
        {
            // Cek barang
            var productIDs = req.Order.OrderProducts.Select(y => y.ProductID);
            var productCount = await DB.Set<MsProduct>()
                .Where(x => productIDs.Contains(x.ID))
                .Select(x => x.ID)
                .CountAsync(ct);

            if (productCount != req.Order.OrderProducts.Count)
            {
                throw new Exceptions.BadRequestException("Cannot find some of the product IDs");
            }

            // Cek Order
            var order = await DB.Set<TrOrderHeader>()
                .Where(x => x.ID == req.Order.ID && x.UserID == req.UserID)
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(ct) ?? throw new Exceptions.BadRequestException("Order not found");

            // Delete
            order.OrderDetails = [..
                order.OrderDetails
                .Join(req.Order.OrderProducts, o => o.ProductID, i => i.ProductID, (i, o) =>
                {
                    i.Quantity = o.Quantity;
                    return i;
                })
            ];
            if (req.Order.Offer.HasValue && req.Order.Offer > 0) order.Offer = req.Order.Offer.Value;

            // Add
            var added = req.Order.OrderProducts.Where(x =>
                    !order.OrderDetails.Select(y => y.ProductID).Contains(x.ProductID)
                ).Select(x => new TrOrderDetail()
                {
                    ID = Guid.NewGuid(),
                    OrderHeaderID = order.ID,
                    ProductID = x.ProductID,
                    Quantity = x.Quantity
                });

            // For add, we have to add the child manually
            if (added.Any()) DB.Set<TrOrderDetail>().AddRange(added);
            await DB.SaveChangesAsync(ct);

            return new BaseResponse<string>(order.ID.ToString(), "Success");
        }
    }

    public class UpdateOrderEndpointRequest
    {
        [FromClaim(Constant.Claims.ID)]
        public required Guid UserID { get; set; }

        [FormField]
        [FromForm]
        public required FormDataRecord Order { get; set; }

        public record OrderProductRecord(Guid ProductID, int Quantity);
        public record FormDataRecord(Guid ID, List<OrderProductRecord> OrderProducts, int? Offer = 0);
    }

    public class UpdateOrderEndpointRequestValidation : Validator<UpdateOrderEndpointRequest>
    {
        public UpdateOrderEndpointRequestValidation()
        {
            RuleFor(x => x.Order.OrderProducts).Must(x => x != null && x.Count > 0);
            RuleForEach(x => x.Order.OrderProducts).ChildRules(y => y.RuleFor(z => z.Quantity).GreaterThan(0));
        }
    }
}