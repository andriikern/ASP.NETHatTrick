<template>
  <h2>Ticket Information</h2>

  <table class="table table-striped table-hover">
    <tbody>
      <tr :class="statusClass">
        <th scope="row">Status</th>
        <td>{{ ticket.status.name }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Pay-in Time</th>
        <td>{{ new Date(ticket.payInTime).toLocaleString('en-GB') }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Pay-in Amount</th>
        <td class="font-monospace">{{ ticket.payInAmount.toFixed(2) }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Selection Count</th>
        <td>{{ ticket.selections.length }}</td>
      </tr>
      <tr>
        <th scope="row">Total Odds</th>
        <td>{{ ticket.totalOdds }}</td>
      </tr>
      <tr>
        <th scope="row">Active Amount ({{ (100 * ticketFinAmounts.manipulativeCostRate).toFixed(2) }} % manipulative cost)</th>
        <td class="font-monospace">{{ ticketFinAmounts.activeAmount.toFixed(2) }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Gross Potential Win Amount</th>
        <td class="font-monospace">{{ ticketFinAmounts.grossPotentialWinAmount.toFixed(2) }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Tax</th>
        <td class="font-monospace">{{ ticketFinAmounts.tax.toFixed(2) }} &euro;</td>
      </tr>
      <tr>
        <th scope="row">Net Potential Win Amount</th>
        <td class="font-monospace">{{ ticketFinAmounts.netPotentialWinAmount.toFixed(2) }} &euro;</td>
      </tr>
      <tr v-if="ticket.isResolved === true">
        <th scope="row">Resolution Time</th>
        <td>{{ new Date(ticket.resolvedAt).toLocaleString('en-GB') }}</td>
      </tr>
      <tr v-if="ticket.isResolved === true && ticket.winAmount !== null">
        <th scope="row">Pay-out Amount</th>
        <td class="font-monospace">{{ ticket.winAmount.toFixed(2) }} &euro;</td>
      </tr>
    </tbody>
  </table>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket, TicketFinancialAmounts } from "../models"
  import { dateToISOStringWithOffset } from "../auxiliaryFunctions"

  import { _now } from "../main"

  interface Data {
    now: Date | null,
    ticketId: Number,
    ticket: Ticket | null,
    ticketFinAmounts: TicketFinancialAmounts | null,
    statusClass: String | null,
    loading: Boolean
  }

  export default defineComponent({
    data(): Data {
      return {
        now: null,
        ticketId: 0,
        ticket: null,
        ticketFinAmounts: null,
        statusClass: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = _now
      this.ticketId = Number.parseInt(this.$route.params.id as string)
      this.ticket = null
      this.ticketFinAmounts = null
      this.statusClass = null
        
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
        this.statusClass = null

        const searchQuery = new URLSearchParams({
          stateAt: dateToISOStringWithOffset(this.now) || '',
          includeSelections: (true).toString()
        })

        var ticket = fetch("/API/BettingShop/" + this.ticketId + "?" + searchQuery)
          .then(r => r.json())
        var ticketFinAmounts = fetch("/API/BettingShop/" + this.ticketId + "/Amounts?" + searchQuery)
          .then(r => r.json())

        Promise.all([ ticket, ticketFinAmounts ])
          .then(values => {
            this.ticket = values[0] as Ticket
            this.ticketFinAmounts = values[1] as TicketFinancialAmounts
            this.loading = false

            this.updateStatusClass(this.ticket.status.name)
          })
      },
      updateStatusClass(status: string): void {
        this.statusClass = null
        
        switch (status) {
          case 'Active':
            this.statusClass = 'bg-primary text-bg-primary'
            break
          case 'Rejected':
            this.statusClass = 'text-muted'
            break
          case 'Cancelled':
            this.statusClass = 'bg-dark text-bg-dark'
            break
          case 'Cashed out':
            this.statusClass = 'bg-warning text-bg-warning'
            break
          case 'Won':
            this.statusClass = 'bg-success text-bg-success'
            break
          case 'Lost':
            this.statusClass = 'bg-danger text-bg-danger'
            break
          default:
            break
        }
      }
    }
  })

</script>
