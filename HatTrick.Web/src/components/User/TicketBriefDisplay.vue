<template>
  <li :class="class">
    {{ new Date(ticket.payInTime).toLocaleString('en-GB', { dateStyle: 'short', timeStyle: 'short' }) }} ({{ ticket.totalOdds.toFixed(2) }} / {{ ticket.selections.length }})
  </li>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket } from "../../models"

  interface Data {
    class: String | null
  }

  export default defineComponent({
    props: {
      ticket: Ticket
    },
    data(): Data {
      return {
        class: null
      }
    },
    created() {
      this.class = 'list-group-item list-group-item-action'

      switch (this.ticket?.status.name) {
        case 'Rejected':
          this.class += ' disabled'
          break
        case 'Cancelled':
          this.class += ' list-group-item-dark disabled'
          break
        case 'Cashed out':
          this.class += ' list-group-item-warning'
          break
        case 'Won':
          this.class += ' list-group-item-success'
          break
        case 'Lost':
          this.class += ' list-group-item-danger'
          break
        default:
          break
      }
    }
  })

</script>
