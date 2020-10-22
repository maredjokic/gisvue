<template>
  <v-app>
  <div class='ma-0 pa-0' style="width: 100%;" id="app">
     <!-- <router-link to="/">Home</router-link> -->
    <router-view/>
  </div>
  </v-app>
</template>

<script>

export default {
    name: 'Layout',
    components: {
    }
};
</script>
