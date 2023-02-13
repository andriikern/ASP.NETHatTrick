<template>
  <div v-if="!loading && (!promoted || events.length)"
       :class="'card border-' + (promoted ? 'primary' : 'secondary') + ' mb-2'">
    <div :class="'card-header bg-' + (promoted ? 'primary' : 'secondary') + ' text-white fw-bold'">
      Today's {{ promoted ? 'Promoted ' : '' }}Offer
    </div>
    <ul :class="'list-group list-group-flush border-' + (promoted ? 'primary' : 'secondary')">
      <EventDisplay v-for="event in events"
                    :key="(promoted ? 'promoted-' : '') + event.id"
                    :promoted="promoted"
                    :eventKey="(promoted ? 'promoted-' : '') + event.id"
                    :event="event"
                    @checkOutcome="checkOutcome" />
    </ul>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Event_ } from "../../models"
  import { dateToISOStringWithOffset } from "../../auxiliaryFunctions"

  import { now } from "../../main"
    
  import EventDisplay from "./EventDisplay.vue"

  interface Data {
    now: Date | null,
    events: Event_[] | null,
    loading: boolean
  }

  export default defineComponent({
    components: {
      EventDisplay
    },
    props: {
      promoted: Boolean
    },
    emits: [ 'onDataFetched', 'checkOutcome' ],
    data(): Data {
      return {
        now: null,
        events: null,
        loading: true
      }
    },
    created() {
      this.loading = true
      this.now = now
      this.events = null

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
        this.events = null

        const searchQuery = new URLSearchParams({
          availableAt: dateToISOStringWithOffset(this.now) || '',
          promoted: this.promoted.toString()
        })

        fetch("/API/Offer?" + searchQuery)
          .then(r => r.json())
          .then(json => {
            this.events = json as Event_[]
            this.loading = false

            this.onDataFetched(this.promoted, this.loading)
          })
      },
      onDataFetched(promoted: Boolean, loading: Boolean): void {
        this.$emit('onDataFetched', promoted, loading);
      },
      checkOutcome(event: Event): void {
        this.$emit('checkOutcome', event);
      }
    }
  })

</script>
