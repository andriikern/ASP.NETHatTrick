<template>
  <TicketDetailsDisplay :ticket="ticket" />
  <TicketSelectionsDisplay :ticket="ticket" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket } from "@/models"

  import TicketDetailsDisplay from "./TicketDetailsDisplay.vue"
  import TicketSelectionsDisplay from "./TicketSelectionsDisplay.vue"

  export default defineComponent({
    components: {
      TicketDetailsDisplay,
      TicketSelectionsDisplay
    },
    props: {
      ticket: Ticket
    },
    created() {
      // fetch the data when the view is created and the data is already being
      // observed
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
