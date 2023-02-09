<template>
  <div v-if="!loading"
       class="container container-fluid text-end">
    <div class="container text-start">
      <div>
        <span class="text-muted">Hello,</span>
        <span class="fw-bold">{{ user.username }}</span>
      </div>
      <div>
        Balance: {{ user.balance.toFixed(2) }} <span class="text-muted">&euro;</span>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from '../models'
  import { dateToISOStringWithOffset } from '../auxiliaryFunctions'

  import OfferCategoryDisplay from './OfferCategoryDisplay.vue'
  import BetAmountInputDisplay from './BetAmountInputDisplay.vue'

  interface Data {
    loading: Boolean,
    user: User | null
  }

  export default defineComponent({
    components: {
      OfferCategoryDisplay,
      BetAmountInputDisplay
    },
    props: {
      now: Date,
      userId: Number
    },
    data(): Data {
      return {
        loading: true,
        user: null
      }
    },
    created() {
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

        const requestOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.userId)
        }

        const searchQuery = new URLSearchParams({
          stateAt: dateToISOStringWithOffset(this.now) || ''
        })

        console.log(this.userId)

        fetch('Account?' + searchQuery, requestOptions)
          .then(r => r.json())
          .then(json => {
            this.user = json as User
            this.loading = false
          })
      }
    }
  })

</script>
