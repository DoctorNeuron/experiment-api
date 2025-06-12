using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExperimentAPI.DTO;
using FastEndpoints;

namespace ExperimentAPI.Endpoint
{
    public class ExceptionPostProcessor : IGlobalPostProcessor
    {
        public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
        {
            if (!context.HasExceptionOccurred) return;
            if (context.ExceptionDispatchInfo.SourceException is Exceptions.UnauthorizedException)
            {
                context.MarkExceptionAsHandled();
                await context.HttpContext.Response.SendAsync(new BaseResponse<string>("Unauthorized", "Unauthorized", StatusCodes.Status401Unauthorized), StatusCodes.Status401Unauthorized, cancellation: ct);
                return;
            }
            if (context.ExceptionDispatchInfo.SourceException is Exceptions.BadRequestException e)
            {
                context.MarkExceptionAsHandled();
                await context.HttpContext.Response.SendAsync(new BaseResponse<Dictionary<string, List<string>>>(e.Errors, "Validation Error", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest, cancellation: ct);
                return;
            }
            context.ExceptionDispatchInfo.Throw();
        }
    }
}