import Vue from "vue";
import VueRouter from "vue-router";
import RevitDemo from "../components/RevitDemo";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: RevitDemo,
  },
];

const router = new VueRouter({
  routes,
});

export default router;
