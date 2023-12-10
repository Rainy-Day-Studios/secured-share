interface CreateSecretRequestModel {
  name: string | null,
  encryptedValue: string,
  clientIV: string,

  password: string | null,
  expiration: Date,
  maxAccessCount: string | null
}
