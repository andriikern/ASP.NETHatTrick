<template>
  <div v-if="loading" class="card bg-warning text-white">
    <div class="card-header font-weight-bold">Warning</div>
    <div class="card-body text-white">
      Please refresh once the <a class="link" target="_blank" href="http://dotnet.microsoft.com/apps/aspnet"><em>ASP.NET</em></a> backend has started. See <a class="link link-info" target="_blank" href="http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue">here</a> for more details.
    </div>
  </div>

  <SimpleUserDisplay :now="now" :userId="userId" />

  <form id="ticket" action="BetShop" method="post" @submit="submit">
    <OfferCategoryDisplay :now="now"
                          :promoted="true"
                          @onDataFetched="onDataFetched" 
                          @checkOutcome="checkOutcome" />
    <OfferCategoryDisplay :now="now"
                          :promoted="false"
                          @onDataFetched="onDataFetched"
                          @checkOutcome="checkOutcome" />
    <BetAmountInputDisplay :loading="loading"
                           @setAmount="setAmount" />
  </form>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { dateToISOStringWithOffset } from '../auxiliaryFunctions'

  import SimpleUserDisplay from './SimpleUserDisplay.vue'
  import OfferCategoryDisplay from './OfferCategoryDisplay.vue'
  import BetAmountInputDisplay from './BetAmountInputDisplay.vue'

  interface Data {
    selection: Set<Number>,
    betAmount: Number,
    promoLoading: Map<Boolean, Boolean>,
    loading: Boolean
  }

  export default defineComponent({
    components: {
      SimpleUserDisplay,
      OfferCategoryDisplay,
      BetAmountInputDisplay
    },
    props: {
      now: Date,
      userId: Number
    },
    data(): Data {
      return {
        selection: new Set<Number>(),
        betAmount: 0,
        promoLoading: new Map<Boolean, Boolean>(),
        loading: false
      }
    },
    created() {
      this.selection = new Set<Number>(),
      this.betAmount = 0,
      this.promoLoading = new Map<Boolean, Boolean>(),
      this.loading = true

      // reset the data when the view is created and the data is already being
      // observed
      this.resetData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'resetData'
    },
    methods: {
      resetData(): void {
        this.selection = new Set<Number>()
        this.betAmount = 0
        this.promoLoading = new Map<Boolean, Boolean>([
          [ false, true ],
          [ true, true ]
        ])
        this.loading = true
      },
      onDataFetched(promoted: Boolean, loading: Boolean) {
        this.promoLoading.set(promoted, loading)

        this.loading = Array.from(this.promoLoading.values()).some(l => l)
      },
      checkOutcome(event: Event): void {
        const element = event.target as HTMLInputElement

        if (element.checked)
          this.selection.add(Number.parseInt(element.value))
        else
          this.selection.delete(Number.parseInt(element.value))
      },
      setAmount(event: Event): void {
        const element = event.target as HTMLInputElement

        this.betAmount = Number.parseFloat(element.value)
      },
      submit(event: Event): void {
        //const element = event.target as HTMLFormElement

        event.preventDefault()

        const requestOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            userId: this.userId,
            selectionIds: Array.from(this.selection),
            amount: this.betAmount
          })
        }

        const searchQuery = new URLSearchParams({
          placedAt: dateToISOStringWithOffset(this.now) || ''
        })

        fetch("BetShop?" + searchQuery, requestOptions)
          .then(() => location.reload())
          .catch(error => alert(error))
      }
    }
  })

</script>
