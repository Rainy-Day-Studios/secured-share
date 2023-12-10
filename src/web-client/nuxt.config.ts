// https://v3.nuxtjs.org/api/configuration/nuxt.config
export default defineNuxtConfig({
  app: {
    head: {
      title: 'Secured Share'
    }
  },
  css: [
    '@/themes/theme.css',
    'primevue/resources/primevue.css',
    'primeicons/primeicons.css',
    'primeflex/primeflex.css'
  ],
  build: {
    transpile: ['primevue']
  },
  buildModules: ['@nuxtjs/style-resources'],

  styleResources: {
    hoistUseStatements: true
  },
  publicRuntimeConfig: {
    API_URL: process.env.API_URL
  }
});
