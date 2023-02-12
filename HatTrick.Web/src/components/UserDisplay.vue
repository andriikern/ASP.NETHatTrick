<template>
  <NonExistentUserErrorDisplay v-if="!loading && user === null" />
  <UserProfile v-if="!(loading || user === null)" :user="user" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "../models"
  import { dateToISOStringWithOffset } from "../auxiliaryFunctions"

  import { _now, _userId } from "../main"

  import NonExistentUserErrorDisplay from "./User/NonExistentUserErrorDisplay.vue"
  import UserProfile from "./User/UserProfile.vue"

  interface Data {
    _now: Date | null,
    _userId: Number,
    user: User | null,
    loading: Boolean
  }

  export default defineComponent({
    components: {
      NonExistentUserErrorDisplay,
      UserProfile
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