interface CreateSecretRequestModel {
  name: string | null,
  encryptedValue: string,
  browserIV: string,

  password: string | null,
  expiration: Date,
  maxAccessCount: string | null
}
