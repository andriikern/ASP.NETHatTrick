<template>
  <li v-if="!(loading || user === null)"
      class="nav-item">
    <div class="card bg-light text-bg-light p-1 me-2">
      <div>
        <span class="text-muted">Hello,</span> <RouterLink to="/user" class="link link-dark"><strong>{{ user.username }}</strong></RouterLink>
      </div>
      <div>
        <span class="text-muted">Balance:</span> {{ user.balance.toFixed(2) }} <span class="text-muted">&euro;</span>
      </div>
    </div>
  </li>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'
  import { RouterLink } from 'vue-router'

  import { User } from "../models"
  import { dateToISOStringWithOffset } from "../auxiliaryFunctions"

  import { now, userId } from "../main"

  interface Data {
    now: Date | null,
    userId: number,
    user: User | null,
    loading: boolean
  }

  export default defineComponent({
    components: {
      RouterLink
    },
    data(): Data {
      return {
        now: null,
        userId: 0,
        user: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.userId = userId
      this.user = null

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
      }
    }
  })

</script>
