<template>
  <h2>Wallet</h2>

  <form id="wallet" action="/API/Account" method="post" @submit="submit">
    <div v-if="!loading" class="form-group row">
      <label for="balance" class="col col-2 col-form-label">Current balance</label>
      <div class="col col-10">
        <input type="number"
               min="1.00"
               max="250_000.00"
               step="0.01"
               inputmode="numeric"
               class="form-control form-control-plaintext"
               id="balance"
               title="Amount"
               :value="user.balance"
               placeholder="Amount"
               aria-label="Amount"
               aria-describedby="amount-label"
               readonly />
      </div>
    </div>
    <div class="form-group row">
      <label for="amount" class="col col-2 col-form-label">Amount</label>
      <div class="col col-10">
        <input type="number"
               min="1.00"
               max="250_000.00"
               step="0.01"
               inputmode="numeric"
               class="form-control"
               id="amount"
               title="Amount"
               placeholder="Amount"
               aria-label="Amount"
               aria-describedby="amount-label"
               @input="setAmount"
               required />
      </div>
    </div>
    <div class="form-group row">
      <div class="col col-form-label">
        <button type="button"
                class="btn btn-success form-control"
                id="deposit"
                title="Deposit"
                value="1"
                @click="makeTransaction">
          Deposit
        </button>
      </div>
      <div class="col col-form-label">
        <button type="button"
                class="btn btn-danger form-control"
                id="withdraw"
                title="Withdraw"
                value="-1"
                @click="makeTransaction">
          Withdraw
        </button>
      </div>
    </div>
  </form>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "../models"
  import { dateToISOStringWithOffset } from "../auxiliaryFunctions"

  import { now, userId } from "../main"

  interface Data {
    now: Date | null,
    userId: number,
    user: User | null,
    amount: number,
    loading: boolean
  }

  export default defineComponent({
    components: { },
    data(): Data {
      return {
        now: null,
        userId: 0,
        user: null,
        amount: 0,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.userId = userId
      this.user = null
      this.amount = 0

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
        this.user = null

        const searchQuery = new URLSearchParams({
          stateAt: dateToISOStringWithOffset(this.now) || '',
          includeTickets: (false).toString(),
          includeTicketSelections: (false).toString()
        })

        fetch("/API/Account/" + this.userId + "?" + searchQuery)
          .then(r => r.ok ? r.json() : null)
          .then(json => {
            this.user = json as User
            this.loading = false
          })
      },
      setAmount(event: Event): void {
        const element = event.target as HTMLInputElement

        this.amount = Number.parseFloat(element.value)
      },
      submit(event: Event) {
        event.preventDefault()
      },
      makeTransaction(event: Event) {
        const element = event.target as HTMLInputElement
        const type = Number.parseInt(element.value)

        const requestOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            userId: this.userId,
            type: type,
            amount: this.amount
          })
        }

        const searchQuery = new URLSearchParams({
          time: dateToISOStringWithOffset(this.now) || ''
        })

        fetch("/API/Account?" + searchQuery, requestOptions)
          .then(r => r.ok ? r.json() : r.text())
          .then(r => typeof r === 'string' || r instanceof String ?
            alert(r) :
            window.location.reload()
          )
      }
    }
  })

</script>
