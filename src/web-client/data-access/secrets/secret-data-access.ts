
export async function createSecret(req: CreateSecretRequestModel) {
  const config = useRuntimeConfig();
  const createSecretUrl = `${config.API_URL}/secret`;

  let apiResponse = await fetch(createSecretUrl, {
    method: 'post',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(req)
  });

  let responseModel = await apiResponse.json();

  return responseModel;
}


export async function getSecretMetadata(secretId: String): Promise<ApiResponse<SecurityMetadataResponse>> {
  const config = useRuntimeConfig();
  const getMetadataUrl = `${config.API_URL}/secret/${secretId}/metadata`;

  let apiResponse = await fetch(getMetadataUrl, {
    method: 'get',
  });

  
  let responseModel = await apiResponse.json();
  if (responseModel.Model.Expiration) {
    responseModel.Model.Expiration = new Date(responseModel.Model.Expiration);
  }

  return responseModel;
}

export async function getSecret(secretId: String): Promise<ApiResponse<RetrieveSecretResponseModel>> {
  const config = useRuntimeConfig();
  const getSecretUrl = `${config.API_URL}/secret/${secretId}/retrieve`;

  let apiResponse = await fetch(getSecretUrl, {
    method: 'post',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({})
  });

  let responseModel = await apiResponse.json();

  return responseModel;
}

export async function deleteSecret(secretId: String): Promise<ApiResponse<string>> {
  const config = useRuntimeConfig();
  const deleteSecretUrl = `${config.API_URL}/secret/${secretId}`;

  let apiResponse = await fetch(deleteSecretUrl, {
    method: 'delete'
  });

  let responseModel = await apiResponse.json();

  return responseModel;
}