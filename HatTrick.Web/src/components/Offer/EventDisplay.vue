<template>
  <li :class="'list-group-item border-' + (promoted ? 'primary' : 'secondary') + ' text-center'">
    <div class="container container-fluid">
      <div class="row" style="font-size: 0.75rem;">
        <div class="col text-start"><strong>{{ event.name }}</strong></div>
        <div class="col text-center">{{ event.sport.name }}</div>
        <div class="col text-end">
          {{ new Date(event.startsAt).toLocaleString('en-GB', { dateStyle: 'short', timeStyle: 'short' })}}
        </div>
      </div>
    </div>
    <FixtureDisplay v-for="fixture in event.fixtures"
                    :key="eventKey + '-' + fixture.type.id"
                    :promoted="promoted"
                    :fixtureKey ="eventKey + '-' + fixture.type.id"
                    :fixture="fixture"
                    @checkOutcome="checkOutcome" />
  </li>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Event_ } from "../../models"

  import FixtureDisplay from "./FixtureDisplay.vue"

  export default defineComponent({
    components: {
      FixtureDisplay
    },
    props: {
      promoted: Boolean,
      eventKey: String,
      event: Event_
    },
    emits: [ 'checkOutcome' ],
    created() {
      // initialise the data when the view is created and the data is already
      // being observed
      this.initialiseData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'initialiseData'
    },
    methods: {
      initialiseData(): void { },
      checkOutcome(event: Event): void {
        this.$emit('checkOutcome', event)
      }
    }
  })

</script>
