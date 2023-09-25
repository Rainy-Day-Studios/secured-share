
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
