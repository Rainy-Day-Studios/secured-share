<template>
  <div>
    <Navbar></Navbar>
    <slot />
    <Footer></Footer>
    <Toast position="bottom-center" />
  </div>
</template>

<script setup lang="ts">
import Navbar from '@/components/navbar.vue';
import Footer from '@/components/footer.vue';
import Toast from 'primevue/toast';

</script>