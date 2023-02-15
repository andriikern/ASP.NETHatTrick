<template>
  <h3 class="mt-3">Selections</h3>
  <li v-if="!loading" class="list list-group">
    <TicketSingleSelectionDisplay v-for="selection in selections"
                                  :key="'selection-' + selection.id + '-' + selection.fixtures[0].id + '-' + selection.fixtures[0].markets[0].id + '-' + selection.fixtures[0].markets[0].outcomes[0].id"
                                  :selection="selection" />
  </li>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Event_, Ticket } from "@/models"
  import { dateToISOStringWithOffset } from "@/auxiliaryFunctions"

  import { now } from "@/main"

  import TicketSingleSelectionDisplay from "./TicketSingleSelectionDisplay.vue"

  interface Data {
    now: Date | null,
    selections: Event_[] | null,
    loading: boolean
  }

  export default defineComponent({
    components: {
      TicketSingleSelectionDisplay
    },
    props: {
      ticket: Ticket
    },
    data(): Data {
      return {
        now: null,
        selections: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.selections = null
        
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

        const searchQuery = new URLSearchParams({
          stateAt: dateToISOStringWithOffset(this.now) || ''
        })

        fetch("/API/BettingShop/" + this.ticket!.id + "/Selections?" + searchQuery)
          .then(r => r.json())
          .then(json => {
            this.selections = json as Event_[]
            this.loading = false
          })
      }
    }
  })

</script>
