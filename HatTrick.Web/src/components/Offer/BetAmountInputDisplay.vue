<template>
  <div v-if="!loading"
       class="container container-fluid row mt-2">
    <div class="col"></div>
    <div class="col">
      <div class="container container-fluid">
        <div class="row">
          <label class="sr-only visually-hidden"
                 id="bet-amount-label"
                 for="bet-amount">
            Pay-in amount
          </label>
          <div class="input-group">
            <input type="number"
                   min="0.25"
                   max="250_000.00"
                   step="0.01"
                   inputmode="numeric"
                   class="form-control font-monospace"
                   id="bet-amount"
                   title="Bet amount"
                   placeholder="Bet amount"
                   aria-label="Bet amount"
                   aria-describedby="bet-amount-label"
                   @input="setAmount"
                   required />
            <span class="input-group-text font-monospace"
                  title="Euro">
              &euro;
            </span>
            <button type="submit"
                    class="btn btn-success"
                    title="Place bet">
              Place bet
            </button>
          </div>
        </div>
        <div v-if="!mcrLoading" class="row">
          <small class="text-muted">
            Manipulative cost of {{ (100 * manipulativeCostRate).toFixed(2) }} % is deducted from the pay-in amount.
          </small>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  interface Data {
    manipulativeCostRate: number | null,
    mcrLoading: boolean
  }

  export default defineComponent({
    props: {
      loading: Boolean
    },
    emits: [ 'setAmount' ],
    data(): Data {
      return {
        manipulativeCostRate: null,
        mcrLoading: true
      }
    },
    created() {
      this.mcrLoading = true
      this.manipulativeCostRate = null

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
        this.mcrLoading = true
        this.manipulativeCostRate = null

        fetch("/API/BettingShop/ManipulativeCostRate")
          .then(r => r.json())
          .then(json => {
            this.manipulativeCostRate = json as number
            this.mcrLoading = false
          })
      },
      setAmount(event: Event): void {
        this.$emit('setAmount', event)
      }
    }
  })

</script>
