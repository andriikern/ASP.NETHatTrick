<template>
  <NonExistentUserErrorDisplay v-if="!loading && user === null" />
  <UserProfile v-if="!(loading || user === null)" :user="user" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "@/models"
  import { dateToISOStringWithOffset } from "@/auxiliaryFunctions"

  import { now, userId } from "@/main"

  import NonExistentUserErrorDisplay from "./User/NonExistentUserErrorDisplay.vue"
  import UserProfile from "./User/UserProfile.vue"

  interface Data {
    now: Date | null,
    userId: number,
    user: User | null,
    loading: boolean
  }

  export default defineComponent({
    components: {
      NonExistentUserErrorDisplay,
      UserProfile
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
          includeTickets: (true).toString(),
          includeTicketSelections: (true).toString()
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
