<template>
  <div class="mt-6 md:mt-8">
    <div class="text-center mb-5 px-1">
      <h1 class="text-6xl mb-3 font-bold">Stop Emailing Passwords.</h1>
      <span class="text-xl text-primary"
        >Share secrets securely in one click</span
      >
    </div>
    <div class="text-center mb-8 pb-8 px-1">
      <div class="mb-2">
        <InputText
          class="bg-white pw-input mr-2 md:w-18rem"
          placeholder="Password"
          type="password"
          v-model="pwInput"
        ></InputText>
        <Button :loading="isSubmittingPw" type="button" @click="handleCreateSecret()"> Share </Button>
      </div>
      <NuxtLink class="text-sm" to="/share-secret">Advanced</NuxtLink>
    </div>
    <div
      class="marketing-sections py-8 flex flex-column align-items-center px-2"
    >
      <MarketingSection>
        <template v-slot:img>
          <img src="@/assets/images/shield-check.svg" class="w-8rem" />
        </template>
        <template v-slot:title>
          Can't remember your password? Neither can we.
        </template>
        <template v-slot:description>
          <p>
            We encrypt your password before it ever leaves your computer. If we
            don't know what it is, neither can attackers.
          </p>
        </template>
      </MarketingSection>
      <MarketingSection>
        <template v-slot:img>
          <img src="@/assets/images/account.svg" class="w-8rem" />
        </template>
        <template v-slot:title> Control access to your secrets. </template>
        <template v-slot:description>
          <p class="mb-2">
            Expire your sharing links after time passes, or after they have been
            viewed.
          </p>
          <p>You control exactly how and when your passwords are viewed.</p>
        </template>
      </MarketingSection>
      <MarketingSection>
        <template v-slot:img>
          <img src="@/assets/images/screen.svg" class="w-8rem" />
        </template>
        <template v-slot:title> Don't trust us? Read our code. </template>
        <template v-slot:description>
          <p class="mb-2">Our platform is 100% open source.</p>
          <a
            href="https://github.com/Rainy-Day-Studios/secured-share"
            target="_blank"
            ><p>Check out the code.</p></a
          >
        </template>
      </MarketingSection>
    </div>
  </div>
</template>

<script setup lang="ts">
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import MarketingSection from '@/components/marketing-section.vue';
import { createSecret } from '@/data-access/secrets/secret-data-access';
import { encrypt } from '@/encryption/encryptor';

const pwInput = ref(null);
const isSubmittingPw = ref(false);

async function handleCreateSecret() {
  try {
    if (pwInput.value) {
      if (pwInput.value.length > 200) {
        // TODO handle this
        return;
      }

      isSubmittingPw.value = true;

      const encryptionResult = await encrypt(pwInput.value);

      const pwExpiration = new Date();
      pwExpiration.setDate(new Date().getDate() + 7);

      const reqModel = {
        encryptedValue: encryptionResult.encryptedValue,
        browserIV: encryptionResult.iv,
        expiration: pwExpiration
      } as CreateSecretRequestModel;

      let result = await createSecret(reqModel);

      console.log(result);
    }
  } finally {
    isSubmittingPw.value = false;
  }
}
</script>

<style scoped>
.pw-input {
  color: var(--surface-0);
}

.pw-input::placeholder {
  color: var(--surface-400);
}

.marketing-sections {
  background-color: var(--surface-overlay);
}
</style>
