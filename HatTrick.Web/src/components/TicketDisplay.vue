<template>
  <h2>Ticket Information</h2>

  <NonExistentTicketErrorDisplay v-if="!loading && ticket === null" />
  <TicketInfoDisplay v-if="!loading && ticket !== null" :ticket="ticket" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket } from "../models"
  import { dateToISOStringWithOffset } from "../auxiliaryFunctions"

  import { now } from "../main"

  import NonExistentTicketErrorDisplay from "./Ticket/NonExistentTicketErrorDisplay.vue"
  import TicketInfoDisplay from "./Ticket/TicketInfoDisplay.vue"

  interface Data {
    now: Date | null,
    ticketId: number,
    ticket: Ticket | null,
    loading: boolean
  }

  export default defineComponent({
    components: {
      NonExistentTicketErrorDisplay,
      TicketInfoDisplay
    },
    data(): Data {
      return {
        now: null,
        ticketId: 0,
        ticket: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.ticketId = Number.parseInt(this.$route.params.id as string)
      this.ticket = null
        
      // fetch the data when the view is created and the data is already being
      // observed
      this.fetchData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    },
    methods: {
      fetchData(): void {
        this.loading = true
        this.ticketId = Number.parseInt(this.$route.params.id as string)

        const searchQuery = new URLSearchParams({
          stateAt: dateToISOStringWithOffset(this.now) || '',
          includeSelections: (true).toString()
        })

        fetch("/API/BettingShop/" + this.ticketId + "?" + searchQuery)
          .then(r => r.ok ? r.json() : null)
          .then(json => {
            this.ticket = json as Ticket
            this.loading = false
          })
      }
    }
  })

</script>
