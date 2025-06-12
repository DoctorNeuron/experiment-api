using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Order
{
    public class CreateOrderEndpoint : Endpoint<CreateOrderEndpointRequest, BaseResponse<string>>
    {
        public required ExperimentDBContext DB { get; set; }

        public override void Configure()
        {
            Post("order");
            AllowFormData(true);
        }

        public override async Task<BaseResponse<string>> ExecuteAsync(CreateOrderEndpointRequest req, CancellationToken ct)
        {
            // Cek barang
            var productCount = await DB.Set<MsProduct>()
                .Where(x => req.Order.OrderProducts.Select(y => y.ProductID).Contains(x.ID))
                .Select(x => x.ID)
                .CountAsync(ct);

            if (productCount != req.Order.OrderProducts.Count)
            {
                throw new Exceptions.BadRequestException("Cannot find some of the product IDs");
            }

            // Insert order
            var orderHeaderID = Guid.NewGuid();
            var data = new TrOrderHeader()
            {
                ID = orderHeaderID,
                IssuedDate = DateTime.UtcNow,
                Offer = req.Order.Offer,
                UserID = req.UserID,
                OrderDetails = [..req.Order.OrderProducts.Select(x => new TrOrderDetail()
                {
                    ID = Guid.NewGuid(),
                    OrderHeaderID = orderHeaderID,
                    Quantity = x.Quantity,
                    ProductID = x.ProductID
                })]
            };

            DB.Add(data);
            await DB.SaveChangesAsync(ct);

            return new BaseResponse<string>(orderHeaderID.ToString(), "Success", StatusCodes.Status201Created);
        }
    }

    public class CreateOrderEndpointRequest
    {
        [FromClaim(Constant.Claims.ID)]
        public required Guid UserID { get; set; }

        [FormField]
        [FromForm]
        public required FormDataRecord Order { get; set; }

        public record OrderProductRecord(Guid ProductID, int Quantity);
        public record FormDataRecord (int Offer, List<OrderProductRecord> OrderProducts);
    }

    public class CreateOrderEndpointRequestValidation : Validator<CreateOrderEndpointRequest>
    {
        public CreateOrderEndpointRequestValidation()
        {
            RuleFor(x => x.Order.Offer).GreaterThan(0);
            RuleFor(x => x.Order.OrderProducts).Must(x => x != null && x.Count > 0);
            RuleForEach(x => x.Order.OrderProducts).ChildRules(y => y.RuleFor(z => z.Quantity).GreaterThan(0));
        }
    }
}