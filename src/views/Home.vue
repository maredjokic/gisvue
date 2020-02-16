<template>
  <div class="home">  
    <div id="container">
      <div id="map"></div>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css';
// Vue.component('v-map', L.Map);
// Vue.component('v-tilelayer', L.TileLayer);

export default {
  name: 'Home',
  data:
  {
    map:null
  },
  components: {
    HelloWorld,
    L
  },
    mounted() {
        this.initMap();
    },
    methods: {
        initMap() {
          var map = L.map('map').setView([44, 21], 5);

         L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
              attribution: ''
                  }).addTo(map);

          // L.marker([51.5, -0.09]).addTo(map)
          //     .bindPopup('A pretty CSS3 popup.<br> Easily customizable.')
          //     .openPopup();
        }
    } 
}


</script>

<style>
#container
{
  max-width: 100%;
  max-height: 500px;
}
#map {
        display: grid;
        min-width: 100%;
        min-height: 500px;
}

</style>
