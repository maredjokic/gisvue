<template>
  <v-card flat style="width: 100%; heigth: 100%;">
    <div class="home">  
      <v-card flat class="pb-2" style="width: 100%; height: 80%;">
        <div id="map"></div>
      </v-card>
  </div>
    <v-card flat class="d-flex flex-row mx-10">
        <v-card flat>
        <v-btn color="blue" dark style="margin: 10px;" @click="clear()" class="pa-2">Clear</v-btn>
        <v-btn color="blue" dark style="margin: 10px;" @click="klik()" class="pa-2">Proračunaj i iscrtaj</v-btn>
        <v-container fluid>
            <input type="radio" id="one" value="Vozilo1" v-model="radio">
            <label for="Vozilo1">Vozilo1</label>
            <br>
            <input type="radio" id="two" value="Vozilo2" v-model="radio">
            <label for="Vozilo2">Vozilo2</label>
            <br>
            <br>
            <label>Predjeno tog dana:</label>
            <br>
            <span>{{ kmPerDayComputed }} km</span>
            <br>
            <br>
            <label>Predjeno tog meseca:</label>
            <br>
            <span>{{ kmPerMonthComputed }} km</span>
          </v-container>
        </v-card>
        <v-card class="mx-10" flat>
            <v-date-picker dark color="blue"
              v-model="date"
              min="2012-10-22"
              max="2014-09-30"
            ></v-date-picker>
        </v-card>
        <v-card flat class="d-flex flex-column mx-10">
              <label>Latituda:</label>
              <span>{{ lat }}</span>
              <br>
              <br>
              <label>Longituda:</label>
              <span>{{ lng }}</span>
              <br>
              <br>
              <label>Prosecna brzina u tacki:</label>
              <span>{{ averageSpeed }} km/h</span>
          </v-card>

      </v-card>
  </v-card>
</template>

<script>
// @ is an alias to /src
import L from 'leaflet'
import 'leaflet/dist/leaflet.css';
import 'leaflet.markercluster';
// Vue.component('v-map', L.Map);
// Vue.component('v-tilelayer', L.TileLayer);
import axios from 'axios'

export default {
  name: 'Home',
  data (){
    return {
      date: '2013-10-10',
      map: null,
      radio: 'Vozilo1',
      kmPerDay: 0.0,
      kmPerMonth: 0.0,
      line: new Array(),
      lng: 0.0,
      lat: 0.0,
      averageSpeed: 0.0
    }
  },
  components: {
    L
  },
    mounted() {
        this.initMap();
    },
    computed: {
      kmPerDayComputed () {
        return (this.kmPerDay / 1000).toFixed(2);
      },
      kmPerMonthComputed () {
        return (this.kmPerMonth / 1000).toFixed(2);
      }
    },
    methods: {
        initMap() {
          this.map = L.map('map').setView([43.33, 21.9], 13);

         L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
              attribution: ''
                  }).addTo(this.map);

          this.map.on('click', this.klikNaMapu);
        },
        klikNaMapu (e) {
            console.log("Lat, Lon : " + e.latlng.lat + ", " + e.latlng.lng);
            var vid = this.radio === 'Vozilo1' ? '4578' : '4573';
            axios.get('https://localhost:5001/Point/AverageSpeedInPoint?dateTime=' + this.date + '&vid=' + vid + '&Lon=' +  e.latlng.lng + '&Lat=' + e.latlng.lat )
            .then((response) => {
              console.log(response.data);
              this.averageSpeed = response.data;
            });
            console.log(this.date);
            this.lng = e.latlng.lng;
            this.lat = e.latlng.lat;
            L.circle({'lon': this.lng, 'lat': this.lat} ,{radius: 3, color: 'purple'}).addTo(this.map);
        },
        klik () {
          this.map.remove();
          this.initMap();

          var vid = this.radio === 'Vozilo1' ? '4578' : '4573';
          console.log('https://localhost:5001/Point/PointsByDate?dateTime=' + this.date + '&vid=' + vid);
          axios.get('https://localhost:5001/Point/PointsByDate?dateTime=' + this.date + '&vid=' + vid)
          .then((response) => {
            var polyline = L.polyline(response.data, {color: 'blue'}).addTo(this.map);
            this.map.fitBounds(polyline.getBounds());

          // response.data.forEach(element => {
          // L.circle(element,{radius: 10, color: 'blue'}).addTo(this.map);
          // });

          L.circle(response.data[0],{radius: 30, color: 'green'}).addTo(this.map);
          L.circle(response.data[response.data.length - 1],{radius: 30, color: 'red'}).addTo(this.map);

          this.kmPerDayCount();
          this.kmPerMonthCount();
          });
        },
        clear () {
          this.map.remove();
          this.initMap();
          this.kmPerDay = 0;
          this.kmPerMonth = 0;
          this.lng = 0.0;
          this.lat = 0.0;
          this.averageSpeed = 0.0;
        },
        kmPerDayCount()
        {
          var vid = this.radio === 'Vozilo1' ? '4578' : '4573';
          axios.get('https://localhost:5001/Point/LengthByDate?dateTime=' + this.date + '&vid=' + vid)
          .then((response) => {
            console.log(response.data);
            this.kmPerDay = response.data;
          });
        },
        kmPerMonthCount()
        {
          var vid = this.radio === 'Vozilo1' ? '4578' : '4573';
          axios.get('https://localhost:5001/Point/LengthByMonth?dateTime=' + this.date + '&vid=' + vid)
          .then((response) => {
            console.log(response.data);
            this.kmPerMonth = response.data;
          });
        },
        addToLine(coordinate)
        {
          this.line.push(coordinate);
        }
    } 
}
</script>

<style>
#map {
        display: grid;
        width: 100%;
        height: 50vh;
}

</style>
