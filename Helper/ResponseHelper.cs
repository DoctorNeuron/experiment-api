using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExperimentAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI.Helper
{
    public static class ResponseHelper
    {
        public static async Task<BaseResponse<PaginationResponse<T>>> MakePaginationResponse<T>(this IQueryable<T> query, int currentPage, int pageSize, CancellationToken ct)
        {
            // var data = await query.ToListAsync(ct);
            var dataCount = await query.CountAsync(ct);
            if (pageSize != 0) query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
            var data = await query.ToListAsync(ct);

            return new BaseResponse<PaginationResponse<T>>(new PaginationResponse<T>()
            {
                Data = data,
                Pagination = pageSize == 0 ? null : new(currentPage, pageSize, dataCount)
            }, "Success");
        }

        public static BaseResponse<Dictionary<string, List<string>>> MakeBadRequestResponse(Dictionary<string, List<string>> errors)
        {
            return new BaseResponse<Dictionary<string, List<string>>>(
                errors,
                "Validation Error",
                StatusCodes.Status400BadRequest
            );
        }
    }
}