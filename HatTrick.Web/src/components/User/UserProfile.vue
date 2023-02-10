<template>
  <h2>Profile</h2>
  <UserProfileBasicInfoDisplay :user="user" />
  <TicketListDisplay :tickets="user.tickets" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "../../models"
    
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
    created() { }
  })

</script>
