<template>
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

  import { User } from "../../models"
    
  export default defineComponent({
    props: {
      user: User
    },
    emits: [ 'submit', 'setAmount', 'makeTransaction' ],
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
      initialiseData(): void { },
      submit(event: Event): void {
        this.$emit('submit', event)
      },
      setAmount(event: Event): void {
        this.$emit('setAmount', event)
      },
      makeTransaction(event: Event): void {
        this.$emit('makeTransaction', event)
      }
    }
  })

</script>
