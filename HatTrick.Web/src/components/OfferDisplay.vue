<template>
  <div v-if="loading" class="card bg-warning text-white">
    <div class="card-header font-weight-bold">Warning</div>
    <div class="card-body text-white">
      Please refresh once the <a class="link" target="_blank" href="http://dotnet.microsoft.com/apps/aspnet"><em>ASP.NET</em></a> backend has started. See <a class="link link-info" target="_blank" href="http://learn.microsoft.com/en-gb/visualstudio/javascript/tutorial-asp-net-core-with-vue">here</a> for more details.
    </div>
  </div>

  <form id="ticket" action="/API/BettingShop" method="post" @submit="submit">
    <OfferCategoryDisplay :promoted="true"
                          @onDataFetched="onDataFetched" 
                          @checkOutcome="checkOutcome" />
    <OfferCategoryDisplay :promoted="false"
                          @onDataFetched="onDataFetched"
                          @checkOutcome="checkOutcome" />
    <BetAmountInputDisplay :loading="loading"
                           @setAmount="setAmount" />
  </form>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Ticket } from "@/models"
  import { dateToISOStringWithOffset } from "@/auxiliaryFunctions"
    
  import { now, userId } from "@/main"
    
  import OfferCategoryDisplay from "./Offer/OfferCategoryDisplay.vue"
  import BetAmountInputDisplay from "./Offer/BetAmountInputDisplay.vue"

  interface Data {
    now: Date | null,
    userId: number,
    selection: Set<number>,
    betAmount: number,
    promoLoading: Map<boolean, boolean>,
    loading: boolean
  }

  export default defineComponent({
    components: {
      OfferCategoryDisplay,
      BetAmountInputDisplay
    },
    data(): Data {
      return {
        now: null,
        userId: 0,
        selection: new Set<number>(),
        betAmount: 0,
        promoLoading: new Map<boolean, boolean>(),
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.userId = userId
      this.selection = new Set<number>()
      this.betAmount = 0
      this.promoLoading = new Map<boolean, boolean>()

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
        this.selection = new Set<number>()
        this.betAmount = 0
        this.promoLoading = new Map<boolean, boolean>([
          [ false, true ],
          [ true, true ]
        ])
        this.loading = true
      },
      onDataFetched(promoted: boolean, loading: boolean) {
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
        //const element = event.target as HTMLInputElement

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

        fetch("/API/BettingShop?" + searchQuery, requestOptions)
          .then(r => r.ok ? r.json() : r.text())
          .then(r => typeof r === 'string' || r instanceof String ?
            alert(r) :
            this.$router.push({ name: 'ticket', params: { id: (r as Ticket).id } })
          )
      }
    }
  })

</script>
