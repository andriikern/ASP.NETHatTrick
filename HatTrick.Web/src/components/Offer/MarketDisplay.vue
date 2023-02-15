<template>
  <div class="container container-fluid my-2">
    <div class="row">
      <div class="col col-2 text-start">
        {{ market.type.name + (market.value === null ? '' : (': ' +market.value)) }}
      </div>
      <div class="btn-group col col-10 text-center">
        <OutcomeDisplay v-for="outcome in market.outcomes"
                        :key="marketKey + '-' + outcome.id"
                        :promoted="promoted"
                        :marketKey="marketKey"
                        :outcomeKey="marketKey + '-' + outcome.id"
                        :outcome="outcome"
                        @checkOutcome="checkOutcome" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Market } from "@/models"

  import OutcomeDisplay from "./OutcomeDisplay.vue"

  export default defineComponent({
    components: {
      OutcomeDisplay
    },
    props: {
      promoted: Boolean,
      marketKey: String,
      market: Market
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
