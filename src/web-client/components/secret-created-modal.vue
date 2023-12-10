<template>
    <Dialog v-model:visible="isVisible" modal :closable="true" :draggable="false" :closeOnEscape="true" v-bind="$attrs">
        <div class="flex justify-content-center align-items-center mb-5">
            <i class="pi pi-lock mr-3 text-2xl"></i>
            <div class="font-semibold text-2xl">Secret Created!</div>
        </div>
        <div>
            <InputText v-model="url" readonly class="bg-white pw-input mr-2 md:w-18rem"></InputText>
            <Button type="button" @click="handleCopyUrl()" class="font-bold"> <i class="pi pi-copy mr-2 font-bold"></i> Copy
            </Button>
        </div>
    </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog';
import { useToast } from 'primevue/usetoast';

const toast = useToast();

const props = defineProps({
    url: {
        type: String,
        required: true
    },
    isVisible: {
        type: Boolean,
        required: true
    }
})

function handleCopyUrl() {
    navigator.clipboard.writeText(props.url);
    toast.add({
        severity: 'info',
        summary: 'URL Copied',
        detail: 'The url was copied to your clipboard.',
        life: 3000
    })
}

</script>

<style scoped>
.pw-input {
    color: var(--surface-0);
}
</style>

<style>
.p-dialog div.p-dialog-header {
    padding: 10px;
    display: flex;
    flex-direction: row-reverse;
}

.p-dialog-mask {
    backdrop-filter: blur(2px);
}
</style>