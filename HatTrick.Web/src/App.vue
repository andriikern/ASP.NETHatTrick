<template>
  <h1>HatTrick</h1>
  <OfferDisplay :now="fixedNow" :userId="userId" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import OfferDisplay from './components/OfferDisplay.vue'

  // Set this value to `null` to use the actual current time, or to a
  // different fixed date-time value.  Note that the event offer is not
  // automatically updated, a point to late in time might result in an empty
  // offer.
  const _now: Date | null = new Date('2023-02-01T12:00:00.000000+00:00')

  // Change this variable to use another user. If the user by the provided id
  // does not exist in the database, errors will arise while using the app.
  // However, the user is hard-coded into the app using this variable, and no
  // authentication functionality is implemented otherwise.
  const _userId: number = 1

  interface Data {
    now: Date | null,
    fixedNow: Date,
    userId: number
  }

  export default defineComponent({
    name: 'App',
    components: {
      OfferDisplay
    },
    data(): Data {
      return {
        now: null,
        fixedNow: new Date(),
        userId: 0
      }
    },
    created() {
      this.now = _now
      this.userId = _userId

      this.resetData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'resetData'
    },
    methods: {
      resetData(): void {
        this.fixedNow = new Date(this.now === null ? Date.now() : this.now)
      }
    }
  })

</script>
