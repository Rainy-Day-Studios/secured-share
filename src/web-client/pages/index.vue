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
        ></InputText>
        <Button> Share </Button>
      </div>
      <NuxtLink class="text-sm" to="/share-secret">Advanced</NuxtLink>
    </div>
    <div class="marketing-sections py-8 flex justify-content-center px-2">
      <div
        v-for="section in marketingSections"
        :key="section.title"
        class="flex w-26rem"
      >
        <div class="d-none">
          <img src="@/assets/images/shield-check.svg" class="w-2rem" />
        </div>
        <div class="ml-4">
          <h2 class="mb-2">{{ section.title }}</h2>
          <p>{{ section.description }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

const marketingSections = [
  {
    title: "Can't remember your password? Neither can we.",
    description:
      "We encrypt your password before it ever leaves your computer. If we don't know what it is, neither can attackers."
    // img: require('@/assets/images/shield-check.svg')
  }
];
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
