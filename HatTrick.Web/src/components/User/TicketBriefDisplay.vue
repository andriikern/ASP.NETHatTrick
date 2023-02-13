<template>
  <RouterLink :class="statusClass"
              :to="'ticket/' + ticket.id">
    {{ new Date(ticket.payInTime).toLocaleString('en-GB', { dateStyle: 'short', timeStyle: 'short' }) }}: {{ ticket.status.name }}, (odds {{ ticket.totalOdds.toFixed(2) }}, pay-in {{ ticket.payInAmount.toFixed(2) }} &euro;, {{ ticket.selections.length }} selections)
  </RouterLink>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket } from "../../models"

  interface Data {
    statusClass: string | null
  }

  export default defineComponent({
    props: {
      ticket: Ticket
    },
    data(): Data {
      return {
        statusClass: null
      }
    },
    created() {
      this.statusClass = 'list-group-item list-group-item-action'

      switch (this.ticket!.status.name) {
        case 'Active':
          this.statusClass += ' list-group-item-primary'
          break
        case 'Rejected':
          this.statusClass += ' disabled'
          break
        case 'Cancelled':
          this.statusClass += ' list-group-item-dark disabled'
          break
        case 'Cashed out':
          this.statusClass += ' list-group-item-warning'
          break
        case 'Won':
          this.statusClass += ' list-group-item-success'
          break
        case 'Lost':
          this.statusClass += ' list-group-item-danger'
          break
        default:
          break
      }
    }
  })

</script>
