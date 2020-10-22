import Vue from 'vue';
import Layout from './Layout.vue';
import router from './router';
import store from './store';
import Vuetify from 'vuetify';
import 'vuetify/dist/vuetify.min.css';

Vue.use(Vuetify)

Vue.config.productionTip = false
 
new Vue({
  router,
  store,
  vuetify: new Vuetify(),
  render: h => h(Layout)
}).$mount('#app')
