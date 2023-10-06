<template>
    <div class="mt-6 md:mt-8 text-center mb-5 px-1 mx-3">
        <div v-if="isLoading" class="flex align-items-center justify-content-center flex-column">
            <h1 class="text-6xl mb-4 font-bold">Retrieving your secret...</h1>
            <Skeleton width="30rem" height="4rem" class="mb-2"></Skeleton>
            <Skeleton width="20rem" height="2rem"></Skeleton>
        </div>
        <div v-else>
            <h1 class="text-6xl mb-4 font-bold">
                <span v-if="secretInfo.Model.SecretName">"{{ secretInfo.Model.SecretName
                }}" Was Shared With You.</span>
                <span v-else>
                    A Secret Was Shared With You.
                </span>
            </h1>
            <div class="flex align-items-center justify-content-center">
                <div class="flex flex-column w-30rem">
                    <Textarea disabled :value="decryptedSecret"
                        class=" font-bold bg-white pw-input w-full md:w-30rem h-10rem mb-3" :auto-resize="true"></Textarea>
                    <div class="flex flex-row align-items-center w-100">
                        <span class="hidden sm:block">Expires {{ formatDateToMMDDYYYY(secretMetadata.Model.Expiration)
                        }}</span>
                        <div class="flex flex-row justify-content-end w-100 col p-0">
                            <Button type="button" @click="handleDeleteSecret()" class="font-semibold p-button-text"> <i
                                    class="pi pi-trash mr-2 font-bold"></i>
                                Delete
                            </Button>
                            <Button type="button" @click="handleCopySecret()" class="font-bold ml-2"> <i
                                    class="pi pi-copy mr-2 font-bold"></i>
                                Copy
                            </Button>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template> 

<script setup lang="ts">
import Skeleton from "primevue/skeleton";
import Textarea from "primevue/textarea";
import { getSecretMetadata, getSecret } from "@/data-access/secrets/secret-data-access";
import { decryptValue } from "@/encryption/decryptor";
import { useToast } from 'primevue/usetoast';
import { formatDateToMMDDYYYY } from "@/display-utils/date-formatter";

const toast = useToast();

const isLoading = ref(true);
const route = useRoute()
const secretInfo = ref<ApiResponse<RetrieveSecretResponseModel>>(undefined);
const secretMetadata = ref<ApiResponse<SecurityMetadataResponse>>(undefined);
const decryptedSecret = ref('');

onMounted(async () => {
    await loadSecret();
});

async function loadSecret() {

    try {
        const queryParam = route.query.k as String;
        if (!queryParam) {
            alert("invalid URL.");
        }

        const paramPieces = queryParam.split("_");
        const secretId = paramPieces[0];
        const encryptionKey = decodeURIComponent(paramPieces[1]);

        secretMetadata.value = await getSecretMetadata(secretId);
        if (secretMetadata.value.Model.RequiresPassword) {
            // todo handle this
        }
        else {
            const secretResponse = await getSecret(secretId);
            secretInfo.value = secretResponse;
            const secretValue = await decryptValue(
                secretResponse.Model.EncryptedSecret,
                secretResponse.Model.ClientIV,
                encryptionKey);
            decryptedSecret.value = secretValue;
        }
    } finally {
        isLoading.value = false;
    }
}

function handleCopySecret() {
    navigator.clipboard.writeText(decryptedSecret.value);
    toast.add({
        severity: 'info',
        summary: 'Secret Copied',
        detail: 'The secret was copied to your clipboard.',
        life: 3000
    })
}

</script>

<style scoped>
.pw-input {
    color: var(--surface-0);
    opacity: .95;
}
</style>