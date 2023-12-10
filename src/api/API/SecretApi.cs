using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using UseCases.SecretManagement.CreateSecret;
using UseCases.SecretManagement;
using API.Models;
using Entities;
using UseCases.SecretManagement.DTO;

namespace API;

public class SecretApi
{
    private readonly ICreateSecretUseCase _createSecretUseCase;
    private readonly IGetSecretUseCase _getSecretUseCase;
    private readonly IDeleteSecretUseCase _deleteSecretUseCase;
    private readonly ILogger _logger;

    public SecretApi(
        ICreateSecretUseCase createSecretUseCase,
        IGetSecretUseCase getSecretUseCase,
        IDeleteSecretUseCase deleteSecretUseCase,
        ILoggerFactory loggerFac)
    {
        _createSecretUseCase = createSecretUseCase;
        _getSecretUseCase = getSecretUseCase;
        _deleteSecretUseCase = deleteSecretUseCase;

        _logger = loggerFac.CreateLogger<SecretApi>();
    }

    [Function("CreateSecret")]
    public async Task<HttpResponseData> CreateSecret([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "secret")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        if (req.Method.ToLower() == "options") return response;

        try
        {
            var createRequest = await req.ReadFromJsonAsync<CreateSecretRequest>();
            if (createRequest == null)
            {
                await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "Invalid request JSON." });
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            var createResult = await _createSecretUseCase.CreateSecret(new SecuredSecret
            {
                Name = createRequest.Name,
                EncryptedValue = createRequest.EncryptedValue,
                ClientIV = createRequest.ClientIV,
                Metadata = new SecurityMetadata
                {
                    Password = createRequest.Password,
                    Expiration = createRequest.Expiration,
                    MaxAccessCount = createRequest.MaxAccessCount
                }
            });

            if (createResult.Success)
            {
                await response.WriteAsJsonAsync(new ApiResponse<CreateSecretResponse>
                {
                    Success = true,
                    Message = "Successfully created secret.",
                    Model = new CreateSecretResponse
                    {
                        Id = createResult.Model.Id
                    }
                });
            }
            else
            {
                await ResultHandler.HandleFailedResult(createResult, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating secret.");
            await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "An unknown error occurred." });
            response.StatusCode = HttpStatusCode.InternalServerError;

            return response;
        }
    }


    [Function("GetSecretMetadata")]
    public async Task<HttpResponseData> GetSecretMetadata([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secret/{secretId}/metadata")] HttpRequestData req,
        string secretId)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        if (req.Method.ToLower() == "options") return response;

        try
        {
            var getResult = _getSecretUseCase.GetSecretMetadata(secretId);

            if (getResult.Success)
            {
                await response.WriteAsJsonAsync(new ApiResponse<SecurityMetadataResponse>
                {
                    Success = true,
                    Message = "Successfully retrieved security metadata.",
                    Model = new SecurityMetadataResponse
                    {
                        RequiresPassword = getResult.Model.RequiresPassword,
                        Expiration = getResult.Model.Expiration
                    }
                });
            }
            else
            {
                await ResultHandler.HandleFailedResult(getResult, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving secret metadata.");
            await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "An unknown error occurred." });
            response.StatusCode = HttpStatusCode.InternalServerError;

            return response;
        }
    }

    [Function("RetrieveSecret")]
    public async Task<HttpResponseData> RetrieveSecret([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "secret/{secretId}/retrieve")] HttpRequestData req,
        string secretId)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        if (req.Method.ToLower() == "options") return response;

        try
        {
            var retrieveSecretRequest = await req.ReadFromJsonAsync<RetrieveSecretRequest>();
            if (retrieveSecretRequest == null)
            {
                await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "Invalid request JSON." });
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            var getResult = await _getSecretUseCase.GetSecret(new GetSecretRequest { SecretId = secretId, Password = retrieveSecretRequest.Password });

            if (getResult.Success)
            {
                await response.WriteAsJsonAsync(new ApiResponse<RetrieveSecretResponse>
                {
                    Success = true,
                    Message = "Successfully retrieved secret.",
                    Model = new RetrieveSecretResponse
                    {
                        EncryptedSecret = getResult.Model.EncryptedValue,
                        ClientIV = getResult.Model.ClientIV
                    }
                });
            }
            else
            {
                await ResultHandler.HandleFailedResult(getResult, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving secret.");
            await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "An unknown error occurred." });
            response.StatusCode = HttpStatusCode.InternalServerError;

            return response;
        }
    }

    [Function("DeleteSecret")]
    public async Task<HttpResponseData> DeleteSecret([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "secret/{secretId}")] HttpRequestData req,
        string secretId)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        if (req.Method.ToLower() == "options") return response;

        try
        {
            var deleteResult = await _deleteSecretUseCase.DeleteSecret(secretId);

            if (deleteResult.Success)
            {
                await response.WriteAsJsonAsync(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Successfully deleted secret."
                });
            }
            else
            {
                await ResultHandler.HandleFailedResult(deleteResult, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting secret.");
            await response.WriteAsJsonAsync(new ApiResponse<int> { Success = false, Message = "An unknown error occurred." });
            response.StatusCode = HttpStatusCode.InternalServerError;

            return response;
        }
    }
}
