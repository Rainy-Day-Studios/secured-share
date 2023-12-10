export async function encrypt(value: string): Promise<EncryptionResult> {
  const key = await generateEncryptionKey256();
  const { cipherText, iv } = await encryptValue(key, value);

  const encryptedValueString = arrayBufferToBase64(cipherText);
  const keyString = await keyToString(key);
  const ivString = await ivToString(iv);

  return {
    encryptedValue: encryptedValueString,
    encryptionKey: keyString,
    iv: ivString
  };
}

async function generateEncryptionKey256(): Promise<CryptoKey> {
  const key = await window.crypto.subtle.generateKey(
    {
      name: 'AES-GCM',
      length: 256
    },
    true,
    ['encrypt', 'decrypt']
  );
  return key;
}

async function encryptValue(key: CryptoKey, plaintext: string) {
  const encoder = new TextEncoder();
  const data = encoder.encode(plaintext);

  // Generate a random initialization vector (IV)
  const iv = window.crypto.getRandomValues(new Uint8Array(12));

  // Perform the encryption
  const cipherText = await window.crypto.subtle.encrypt(
    {
      name: 'AES-GCM',
      iv: iv
    },
    key,
    data
  );

  // Return both the ciphertext and the IV as a result
  return { cipherText, iv };
}

async function keyToString(key: CryptoKey): Promise<string> {
  const exportedKey = await window.crypto.subtle.exportKey('raw', key);
  const keyArray = Array.from(new Uint8Array(exportedKey));
  const keyString = keyArray.map(byte => String.fromCharCode(byte)).join('');
  return btoa(keyString);
}

function ivToString(iv: Uint8Array): string {
  const ivArray = Array.from(iv);
  const ivString = ivArray.map(byte => String.fromCharCode(byte)).join('');
  return btoa(ivString); // Using base64 encoding
}

function arrayBufferToBase64(arrayBuffer: ArrayBuffer): string {
  const uint8Array = new Uint8Array(arrayBuffer);
  let binaryString = '';
  for (let i = 0; i < uint8Array.length; i++) {
    binaryString += String.fromCharCode(uint8Array[i]);
  }
  return btoa(binaryString);
}