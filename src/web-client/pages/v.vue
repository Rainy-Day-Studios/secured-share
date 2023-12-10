<template>
    <div class="py-8 sm:text-center px-1 px-3 surface-overlay">
        <div class="py-8">
            <div v-if="isLoading" class="flex align-items-center justify-content-center flex-column">
                <h1 class="text-6xl mb-4 font-bold">Retrieving your secret...</h1>
                <Skeleton width="30rem" height="4rem" class="mb-2"></Skeleton>
                <Skeleton width="20rem" height="2rem"></Skeleton>
            </div>
            <div v-else-if="!isSecretDeleted && !errorMsg">
                <h1 class="text-6xl mb-4 font-bold">
                    <span v-if="secretValue && secretValue.Model.SecretName">"{{ secretValue.Model.SecretName
                    }}" Was Shared With You.</span>
                    <span v-else>
                        A Secret Was Shared With You.
                    </span>
                </h1>
                <div class="flex align-items-center justify-content-center">
                    <div v-if="!secretValue">
                        <Button type="button" @click="loadSecretValue()" class="font-semibold ml-2 text-2xl mt-4"> <i
                                class="pi pi-unlock mr-2 pr-1 font-semibold text-2xl"></i>
                            View
                        </Button>
                    </div>
                    <div v-else class="flex flex-column w-full sm:w-30rem mt-4">
                        <Textarea disabled :value="decryptedSecret"
                            class=" font-semibold surface-800 pw-input w-full h-10rem mb-3" :auto-resize="true"></Textarea>
                        <div class="flex flex-row align-items-center w-100">
                            <span class="hidden sm:block">Expires {{
                                formatDateToMMDDYYYY(secretMetadata.Model.Expiration)
                            }}</span>
                            <div class="flex flex-row justify-content-end w-100 col p-0">
                                <Button type="button" @click="handleDeleteSecret()" class="font-semibold p-button-text">
                                    <i class="pi pi-trash mr-2 font-bold"></i>
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
            <div v-else-if="errorMsg" class="my-8 py-8">
                <h2>{{ errorMsg }}</h2>
            </div>
            <div v-else class="my-8 py-8">
                <h2>Secret Deleted.</h2>
            </div>
        </div>
    </div>
</template> 

<script setup lang="ts">
import Skeleton from "primevue/skeleton";
import Textarea from "primevue/textarea";
import { getSecretMetadata, getSecret, deleteSecret } from "@/data-access/secrets/secret-data-access";
import { decryptValue } from "@/encryption/decryptor";
import { useToast } from 'primevue/usetoast';
import { formatDateToMMDDYYYY } from "@/display-utils/date-formatter";

const toast = useToast();

const isLoading = ref(true);
const errorMsg = ref('');

const route = useRoute()
const secretValue = ref<ApiResponse<RetrieveSecretResponseModel>>(undefined);
const secretMetadata = ref<ApiResponse<SecurityMetadataResponse>>(undefined);
const decryptedSecret = ref('');

onMounted(async () => {
    await loadSecretMetadata();
});

const queryParam = route.query.k as String;
if (!queryParam) {
    errorMsg.value = 'Invalid link.';
}

const paramPieces = queryParam.split("_");
const secretId = paramPieces[0];
const encryptionKey = decodeURIComponent(paramPieces[1]);

const isSecretDeleted = ref(false);

async function loadSecretMetadata() {
    try {
        secretMetadata.value = await getSecretMetadata(secretId);
        if (!secretMetadata.value.Success) {
            errorMsg.value = secretMetadata.value.Message;
            return;
        }

        if (secretMetadata.value.Model.RequiresPassword) {
            // todo handle this
        }
    } finally {
        isLoading.value = false;
    }
}

async function loadSecretValue() {
    try {
        const secretResponse = await getSecret(secretId);

        if (!secretResponse.Success) {
            errorMsg.value = secretResponse.Message;
            return;
        }

        if (!secretResponse.Model.EncryptedSecret) {
            isSecretDeleted.value = true;
            return;
        }

        secretValue.value = secretResponse;
        decryptedSecret.value = await decryptValue(
            secretResponse.Model.EncryptedSecret,
            secretResponse.Model.ClientIV,
            encryptionKey);
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

async function handleDeleteSecret() {
    try {
        const result = await deleteSecret(secretId);
        toast.add({
            severity: 'success',
            summary: 'Secret Deleted',
            detail: 'The secret was deleted from storage.',
            life: 3000
        })
        isSecretDeleted.value = true;
    } catch {
        toast.add({
            severity: 'error',
            summary: 'Error Deleting Secret',
            detail: 'An error occurred while attempting to delete the secret.',
            life: 3000
        })
    }
}

</script>

<style scoped>
.pw-input {
    color: var(--surface-0);
    opacity: 1;
}

h1,
h1 span {
    text-wrap: balance
}
</style>