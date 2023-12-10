<template>
  <nav class="p-3 flex">
    <div class="col flex align-items-center">
      <NuxtLink class="flex align-items-center" to="/">
        <img class="mr-3" src="@/assets/images/logo-white.svg" />
        <span class="text-xl font-semibold">Secured Share</span>
      </NuxtLink>
    </div>
    <div class="flex align-items-center">
      <a href="https://github.com/Rainy-Day-Studios/secured-share" target="_blank" class="text-white">
        <i class="pi pi-github text-4xl mr-2"></i>
      </a>
    </div>
  </nav>
</template>

<style scoped>
a {
  text-decoration: none;
  color: var(--text-color)
}
</style>