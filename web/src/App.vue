<template>
  <v-app>
    <v-row class="text-center">
      <v-col cols="12">
        <v-img :src="require('@/assets/logo.svg')" class="my-3" contain height="100" />
      </v-col>
    </v-row>
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: "App",
};
</script>
