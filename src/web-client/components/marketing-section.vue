<template>
  <div class="flex w-100 sm:w-26rem md:w-30rem mb-8 pb-4 align-items-center">
    <div>
      <slot name="img" class="d-none sm:d-block"/>
    </div>
    <div class="ml-4">
      <h2 class="mb-2"><slot name="title" /></h2>
      <slot name="description" />
    </div>
  </div>
</template>
