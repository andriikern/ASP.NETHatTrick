<template>
  <h3>Tickets</h3>

  <p v-if="tickets.length === 0"
     class="text-muted">
      No tickets yet.
  </p>
  <div v-if="tickets.length"
       class="list list-group">
    <TicketSingleDisplay v-for="ticket in tickets"
                         :key="'ticket-' + ticket.id"
                         :ticket="ticket" />
  </div>
</template>

<script lang="ts">
  import { defineComponent, PropType } from 'vue'

  import { Ticket } from "@/models"

  import TicketSingleDisplay from "./TicketSingleDisplay.vue"

  export default defineComponent({
    components: {
      TicketSingleDisplay    
    },
    props: {
      tickets: Array as PropType<Ticket[]>
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
