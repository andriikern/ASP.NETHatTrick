<template>
  <div v-if="!loading"
       class="container container-fluid row mt-2">
    <div class="col"></div>
    <div class="col">
      <label class="sr-only visually-hidden" id="bet-amount-label" for="bet-amount">Pay-in amount</label>
      <div class="input-group">
        <input type="number"
               min="0.10"
               max="10000.00"
               step="0.01"
               inputmode="numeric"
               class="form-control"
               id="bet-amount"
               title="Bet amount"
               placeholder="Bet amount"
               aria-label="Bet amount"
               aria-describedby="bet-amount-label"
               @change="setAmount"
               required />
        <span class="input-group-text" title="Euro">&euro;</span>
        <button type="submit" class="btn btn-success" title="Place bet">Place bet</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  export default defineComponent({
    props: {
      loading: Boolean
    },
    emits: [ 'setAmount' ],
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
      initialiseData(): void {
        console.log({ loading: this.loading })
      },
      setAmount(event: Event): void {
        this.$emit('setAmount', event);
      }
    }
  })

</script>
