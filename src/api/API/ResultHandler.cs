using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using UseCases.Result;

namespace API;

public static class ResultHandler
{
    public static async Task HandleFailedResult<T>(Result<T> result, HttpResponseData response)
    {
        await response.WriteAsJsonAsync(new ApiResponse<int>
        {
            Success = false,
            Message = result.Message
        });
        response.StatusCode = MapToStatusCode(result.Reason);
    }

    public static HttpStatusCode MapToStatusCode(ResultReason resultReason)
    {
        if (resultReason == ResultReason.NotFound) return HttpStatusCode.NotFound;

        if (resultReason == ResultReason.InvalidRequest) return HttpStatusCode.BadRequest;

        return HttpStatusCode.InternalServerError;
    }
}