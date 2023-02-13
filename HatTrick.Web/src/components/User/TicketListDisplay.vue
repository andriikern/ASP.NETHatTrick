<template>
  <h3>Tickets</h3>

  <p v-if="tickets.length === 0"
     class="text-muted">
      No tickets yet.
  </p>
  <div v-if="tickets.length"
       class="list list-group">
    <TicketBriefDisplay v-for="ticket in tickets"
                       :key="'ticket-' + ticket.id"
                       :ticket="ticket" />
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  //import { Ticket } from "../../models"

  import TicketBriefDisplay from "./TicketBriefDisplay.vue"

  export default defineComponent({
    components: {
      TicketBriefDisplay    
    },
    props: {
      tickets: Array/*<Ticket>*/
    },
    created() { }
  })

</script>
