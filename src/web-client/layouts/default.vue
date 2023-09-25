<template>
  <div>
    <Navbar></Navbar>
    <slot />
    <Footer></Footer>
  </div>
</template>

<script setup lang="ts">
import Navbar from '@/components/navbar.vue';
import Footer from '@/components/footer.vue';

</script>