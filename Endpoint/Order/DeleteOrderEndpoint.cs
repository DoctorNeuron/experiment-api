using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Endpoint.Order
{
    public class DeleteOrderEndpoint : Endpoint<DeleteOrderEndpointRequest, BaseResponse<string>>
    {
        public required ExperimentDBContext DB { get; set; }

        public override void Configure()
        {
            Delete("order/{ID}");
        }

        public override async Task<BaseResponse<string>> ExecuteAsync(DeleteOrderEndpointRequest req, CancellationToken ct)
        {
            // Cek Order
            var order = await DB.Set<TrOrderHeader>()
                .Where(x => x.ID == req.ID && x.UserID == req.UserID)
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(ct) ?? throw new Exceptions.BadRequestException("Order not found");
            DB.Remove(order);
            await DB.SaveChangesAsync(ct);

            return new BaseResponse<string>(order.ID.ToString(), "Success");
        }
    }

    public class DeleteOrderEndpointRequest
    {
        [RouteParam]
        public required Guid ID { get; set; }

        [FromClaim(Constant.Claims.ID)]
        public required Guid UserID { get; set; }
    }
}