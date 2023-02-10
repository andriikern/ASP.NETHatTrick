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

  import { _now, _userId } from "../main"

  interface Data {
    _now: Date | null,
    _userId: Number,
    user: User | null,
    loading: Boolean
  }

  export default defineComponent({
    components: {
      RouterLink
    },
    data(): Data {
      return {
        _now: null,
        _userId: 0,
        user: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this._now = _now
      this._userId = _userId
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

      const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userId: this._userId,
          includeTickets: true,
          includeTicketSelections: true,
          includeTransactions: true
        })
      }

      const searchQuery = new URLSearchParams({
        stateAt: dateToISOStringWithOffset(this._now) || ''
      })

      fetch("/API/Account?" + searchQuery, requestOptions)
        .then(r => r.ok ? r.json() : null)
        .then(json => {
          this.user = json as User
          this.loading = false
        })
      }
    }
  })

</script>
