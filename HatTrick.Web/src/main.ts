// Import Vue.
import { createApp } from 'vue'
//import { createApp } from 'vue/dist/vue.esm-bundler'
import { createRouter, createWebHashHistory } from 'vue-router'
import { createPinia } from 'pinia'

// Import Bootstrap.
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap-icons/font/bootstrap-icons.css'
import bootstrap from 'bootstrap'

// Import main app.
import App from "./App.vue"

// Import page components.
import OfferDisplay from "./components/OfferDisplay.vue"
import UserDisplay from "./components/UserDisplay.vue"
import WalletDisplay from "./components/WalletDisplay.vue"
import TicketDisplay from "./components/TicketDisplay.vue"

// Set this value to `null` to use the actual current time, or to a different
// fixed date-time value. Note that the event offer is not automatically
// updated, a point too late in time might result in an empty offer.
const now: Date | null = new Date('2023-02-01T12:00:00.000000+00:00')

// Change this variable to use another user. If the user by the provided id
// does not exist in the database, errors will arise while using the app.
// However, the user is hard-coded into the app using this variable, and no
// authentication functionality is implemented otherwise.
const userId: number = 1

// Define app routes.
const routes = [
  {
    name: 'home',
    title: "Today's Offer",
    path: "/",
    component: OfferDisplay
  },
  {
    name: 'user',
    title: 'User Profile',
    path: "/user",
    component: UserDisplay
  },
  {
    name: 'wallet',
    title: 'Wallet',
    path: "/wallet",
    component: WalletDisplay
  },
  {
    name: 'ticket',
    title: 'Ticket Information',
    path: "/ticket/:id",
    component: TicketDisplay
  }
]

// Create router.
const router = createRouter({
  history: createWebHashHistory(),
  routes
})

// Create the app.
const pinia = createPinia()
const app = createApp(App)
  .use(router)
  .use(pinia)

// Mount the app.
app.mount('#app')

// Initially, the idea was to store user information into a dedicated Pinia
// user store. However, for simplicty and better management of which data to
// retrieve and when, it was decided to simply hard-code the user's id number
// and the current point in time, and then to fetch user information when
// needed.

export {
  now,
  userId
}
