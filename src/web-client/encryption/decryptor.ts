export async function decryptValue(
    encryptedValueBase64: string,
    ivBase64: string,
    secretKeyBase64: string
): Promise<string> {
    const encryptedValue = Uint8Array.from(atob(encryptedValueBase64), c => c.charCodeAt(0));
    const iv = Uint8Array.from(atob(ivBase64), c => c.charCodeAt(0));
    const secretKey = Uint8Array.from(atob(secretKeyBase64), c => c.charCodeAt(0));


    const importedKey = await crypto.subtle.importKey(
        'raw',
        secretKey,
        { name: 'AES-GCM' },
        false,
        ['decrypt']
    );

    const decryptedValue = await crypto.subtle.decrypt(
        {
            name: 'AES-GCM',
            iv,
        },
        importedKey,
        encryptedValue
    );

    const decryptedText = new TextDecoder().decode(decryptedValue);
    return decryptedText;
}