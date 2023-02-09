<template>
  <h1>HatTrick</h1>
  <OfferDisplay :now="fixedNow" />
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import OfferDisplay from './components/OfferDisplay.vue'

  // Set this value to `null` to use the actual current time, or to a
  // different fixed date-time value.  Note that the event offer is not
  // automatically updated, a point to late in time might result in an empty
  // offer.
  const _now: Date | null = new Date('2023-02-01T12:00:00.000000+00:00')

  interface Data {
    now: Date | null,
    fixedNow: Date | null
  }

  export default defineComponent({
    name: 'App',
    components: {
      OfferDisplay
    },
    data(): Data {
      return {
        now: null,
        fixedNow: null
      }
    },
    created() {
      this.now = _now

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
