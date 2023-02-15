<template>
  <h2>Profile</h2>
  <UserProfileBasicInfoDisplay :user="user" />
  <TicketListDisplay :tickets="user.tickets" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "@/models"
    
  import UserProfileBasicInfoDisplay from "./UserProfileBasicInfoDisplay.vue"
  import TicketListDisplay from "./TicketListDisplay.vue"

  export default defineComponent({
    components: {
      UserProfileBasicInfoDisplay,
      TicketListDisplay
    },
    props: {
      user: User
    },
    created() {
      // initialise the data when the view is created and the data is already
      // being observed
      this.initialiseData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'initialiseData'
    },
    methods: {
      initialiseData(): void { }
    }
  })

</script>
